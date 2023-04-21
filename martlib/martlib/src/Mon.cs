using System;
using System.Reflection;


namespace martlib
{
    public static class MonSerializer
    {
        private static ulong bufferdefault = 1024;
        public static byte[] Serialize<T>(T obj)
        {
            List<objectEntry> objs = new List<objectEntry>();

            ulong objid = 0;

            //Console.WriteLine(obj.GetType().GetFields().Length); //4 here
            process(obj, objs, ref objid);

            ulong bytecount = 0;

            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].wrap();
                objs[i].startposition = bytecount;
                bytecount += objs[i].position;
            }

            byte[] data = new byte[bytecount];

            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].completelinks(objs, i);
            }

            ulong bytepos = 0;
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].appenddata(data, ref bytepos);
            }

            return data;
        }
        public static object? Deserialize(byte[] data, Type type)
        {
            dynamic obj = Activator.CreateInstance(type);

            List<objectEntry> objects = new List<objectEntry>();
            ulong pos = 0;

            objectEntry result = dprocess(obj, type, objects, data, pos);

            return result.obj;
        }
        public static T Deserialize<T>(byte[] data)
        {
            T obj = Activator.CreateInstance<T>();

            List<objectEntry> objects = new List<objectEntry>();
            ulong pos = 0;

            objectEntry result = dprocess(obj, obj.GetType(), objects, data, pos);

            return (T)result.obj;
        }


        //SERIALIZATION
        /// <summary>
        /// Recursive method to process all objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objects"></param>
        /// <param name="objid"></param>
        private static objectEntry process<T>(T obj, List<objectEntry> objects, ref ulong objid)
        {
            //Add this object to the list
            objectEntry obje = new objectEntry(obj, objid++, bufferdefault);
            objects.Add(obje);


            processmain(obj, obje, objects, ref objid);

            return obje;
        }
        private static void processmain<T>(T obj, objectEntry obje, List<objectEntry> objects, ref ulong objid)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                dynamic val = field.GetValue(obj);
                bool isclass, isstruct, isarray, isprimitive, isstring;
                gettypes(field, out isclass, out isstruct, out isarray, out isprimitive, out isstring);

                obje.data = Functions.BitReaders.Write(obje.data, field.Name, ref obje.position);
                if ((isprimitive && !(isclass || isstruct || isarray)) || field.FieldType == typeof(string))
                {
                    obje.data = Functions.BitReaders.Write(obje.data, val, ref obje.position);
                }
                else if (isstruct)
                {
                    processmain(val, obje, objects, ref objid);
                    obje.data = Functions.BitReaders.Write(obje.data, false, ref obje.position);
                }
                else if (isclass)
                {
                    //Check if the object is already processed - add reference to its ID and flag it to be updated later.
                    objectEntry? target = null;
                    for (int k = 0; k < objects.Count; k++)
                    {
                        if (objects[k].obj == val)
                        {
                            target = objects[k];
                            break;
                        }
                    }

                    if (target == null)
                    {
                        //create new objectentry
                        target = process(val, objects, ref objid);
                    }

                    //refernce its id to complete later
                    if (obje.linkpointid >= obje.linkpoints.Length)
                        obje.linkpoints = Functions.BitReaders.Double(obje.linkpoints);
                    obje.linkpoints[obje.linkpointid++] = obje.position;
                    obje.data = Functions.BitReaders.Write(obje.data, target.identifier, ref obje.position);
                }
                else if (isarray)
                {
                    //todo: arrays
                }
            }
        }

        //DESERIALIZATION
        private static objectEntry dprocess(object? obj, Type type, List<objectEntry> objects, byte[] data, ulong pos)
        {
            objectEntry obje = new objectEntry(obj, pos, bufferdefault)
            {
                position = pos
            };

            if (obj.GetType().IsClass)
            {
                objects.Add(obje);
                obje.identifier = pos;
            }

            //Once it reads a 0 character it'll terminate.
            while (data[pos] != 0)
            {
                string field;
                Functions.BitReaders.Read(data, ref pos, out field);

                FieldInfo? fieldInfo = type.GetField(field);
                if (fieldInfo == null)
                {
                    throw new FieldAccessException($"Invalid field {field} in type {type}\t\t@0x{pos.ToString("X")} ({pos}) in byte stream.");
                }

                bool isclass, isstruct, isarray, isprimitive, isstring;
                gettypes(fieldInfo, out isclass, out isstruct, out isarray, out isprimitive, out isstring);

                if (isprimitive)
                {
                    dynamic val = Activator.CreateInstance(fieldInfo.FieldType);
                    val = Functions.BitReaders.Read(data, ref pos, val);
                    fieldInfo.SetValue(obj, val);
                }
                else if (isstring)
                {
                    string val = "";
                    Functions.BitReaders.Read(data, ref pos, out val);
                    fieldInfo.SetValue(obj, val);
                }
                else if (isstruct)
                {
                    dynamic val = Activator.CreateInstance(fieldInfo.FieldType);

                    //restart the recursive process on the struct
                    objectEntry tmp = dprocess(val, fieldInfo.FieldType, objects, data, pos);
                    val = tmp.obj;
                    fieldInfo.SetValue(obj, val);
                    pos = tmp.position;

                }
                else if (isclass)
                {
                    dynamic val = Activator.CreateInstance(fieldInfo.FieldType);

                    ulong id;
                    Functions.BitReaders.Read(data, ref pos, out id);

                    //look to see if the object has already been created
                    bool found = false;
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i].identifier == id)
                        {
                            val = objects[i].obj;
                            found = true;
                            break;
                        }
                    }

                    //if not created, create
                    if (!found)
                    {
                        objectEntry tmp = dprocess(val, fieldInfo.FieldType, objects, data, id);
                        val = tmp.obj;
                    }

                    fieldInfo.SetValue(obj, val);
                }

            }

            obje.position = pos + 1;

            return obje;
        }

        private static void gettypes(FieldInfo field, out bool isclass, out bool isstruct, out bool isarray, out bool isprimitive, out bool isstring)
        {
            isclass = field.FieldType.IsClass;
            isstruct = field.FieldType.IsValueType && !field.FieldType.IsEnum && !field.FieldType.IsPrimitive;
            isarray = field.FieldType.IsArray;
            isprimitive = field.FieldType.IsPrimitive;
            isstring = field.FieldType == typeof(string);
        }
        private class objectEntry
        {
            public object obj;
            public ulong identifier;
            public ulong position;
            public ulong startposition = 0;

            public bool processed;

            public byte[] data; //byte data stored here
            public int linkpointid;
            public ulong[] linkpoints; //where to check to update refs once all is complete

            public objectEntry(object obj, ulong id, ulong bufferdefault)
            {
                this.obj = obj;
                identifier = id;

                position = 0;
                processed = false;
                data = new byte[bufferdefault];
                linkpointid = 0;
                linkpoints = new ulong[64];
            }

            public void completelinks(List<objectEntry> objectEntries, int ignoreidx)
            {
                for (int i = 0; i < linkpointid; i++)
                {
                    ulong pos = linkpoints[i];

                    ulong id = 0;
                    Functions.BitReaders.Read(data, ref pos, out id);

                    pos = linkpoints[i];

                    for (int k = 0; k < objectEntries.Count; k++)
                    {
                        if (objectEntries[k].identifier == id)
                        {
                            data = Functions.BitReaders.Write(data, objectEntries[k].startposition, ref pos);
                            break;
                        }
                    }
                }
            }

            public void wrap()
            {
                Functions.BitReaders.Write(data, false, ref position);
            }

            public void appenddata(byte[] d, ref ulong idx)
            {
                for (ulong i = 0; i < position; i++)
                {
                    d[idx++] = data[i];
                }
            }
        }
    }
}
