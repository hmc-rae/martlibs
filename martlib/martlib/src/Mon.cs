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

        /// <summary>
        /// Recursive method to process all objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="objects"></param>
        /// <param name="objid"></param>
        private static void process<T>(T obj, List<objectEntry> objects, ref ulong objid)
        {
            //Add this object to the list
            objectEntry obje = new objectEntry(obj, objid++, bufferdefault);
            objects.Add(obje);

            //Write its ID into the the buffer it holds if it is a class of some form
            if (obj.GetType().IsClass)
                obje.data = Functions.BitReaders.Write(obje.data, obje.identifier, ref obje.position);

            processmain(obj, obje, objects, ref objid);
        }

        private static void processmain<T>(T obj, objectEntry obje, List<objectEntry> objects, ref ulong objid)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                dynamic val = field.GetValue(obj);
                bool isclass = field.GetValue(obj).GetType().IsClass;
                bool isstruct = field.GetValue(obj).GetType().IsValueType && !field.GetValue(obj).GetType().IsEnum && !field.GetValue(obj).GetType().IsPrimitive;
                bool isarray = field.GetValue(obj).GetType().IsArray;
                bool isprimitive = field.GetValue(obj).GetType().IsPrimitive;

                obje.data = Functions.BitReaders.Write(obje.data, field.Name, ref obje.position);
                if ((isprimitive && !(isclass || isstruct || isarray)) || field.GetValue(obj).GetType() == typeof(string))
                {
                    obje.data = Functions.BitReaders.Write(obje.data, val, ref obje.position);
                }
                else if (isstruct)
                {
                    processmain(val, obje, objects, ref objid);
                }
                else if (isclass)
                {
                    Console.WriteLine("class");
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
                        if (k == ignoreidx) continue;

                        if (objectEntries[k].identifier == id)
                        {
                            data = Functions.BitReaders.Write(data, objectEntries[k].startposition, ref pos);
                            break;
                        }
                    }
                }
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
