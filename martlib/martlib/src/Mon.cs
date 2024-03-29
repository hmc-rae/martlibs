﻿using System;
using System.Reflection;
using System.Xml.Linq;


namespace martlib
{
    /// <summary>
    /// A form of object notation which represents a data structure in a byte array, preserving reference semantics and using minimal space.
    /// </summary>
    public static class MonSerializer
    {
        /// <summary>
        /// Version of MonSerializer being used. Some versions of MonSerializer may format differently and therefore be incompatible.
        /// </summary>
        public const string VERSION = "0.3";
        /// <summary>
        /// The default amount of bytes allocated to each object when converting to Mon (1kb). The buffer will automatically double whenever the limit is reached.
        /// </summary>
        private static ulong bufferdefault = 1024;
        /// <summary>
        /// Converts a given object into a byte array in Mart Object Notation. <br></br>
        /// By default, all public fields are interpreted unless tagged with attribute MonIgnore. <br></br>
        /// References are preserved, allowing for circular references by default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T obj)
        {
            List<objectEntry> objs = new List<objectEntry>();

            ulong objid = 0;

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
        /// <summary>
        /// Deserializes a byte array from Mart Object Notation into the given object type, preserving original references. <br></br>
        /// Throws FieldAccessException if an invalid field is read at any point.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? Deserialize(byte[] data, Type type)
        {
            dynamic obj = Activator.CreateInstance(type);

            List<objectEntry> objects = new List<objectEntry>();
            ulong pos = 0;

            objectEntry result = dprocess(obj, type, objects, data, pos);

            return result.obj;
        }
        /// <summary>
        /// Deserializes a byte array from Mart Object Notation into the given object type, preserving original references. <br></br>
        /// Throws FieldAccessException if an invalid field is read at any point.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            List<objectEntry> objects = new List<objectEntry>();
            ulong pos = 0;

            bool isclass, isstruct, isarray, isprimitive, isstring, include;
            gettypes(typeof(T), out isclass, out isstruct, out isarray, out isprimitive, out isstring, out include);

            objectEntry result = null;

            if (isarray)
            {
                result = dprocessarr(typeof(T).GetElementType(), objects, data, pos);
            }
            else
            {
                T obj = Activator.CreateInstance<T>();
                result = dprocess(obj, obj.GetType(), objects, data, pos);
            }
            return (T)result.obj;
        }
        /// <summary>
        /// Reads a file into a byte array, and deserializes said array from MON into the given type.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static object? Deserialize(string filename, Type type)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException(filename);
            byte[] data = File.ReadAllBytes(filename);
            return Deserialize(data, type);
        }
        /// <summary>
        /// Reads a file into a byte array, and deserializes said array from MON into the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T Deserialize<T>(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException(filename);
            byte[] data = File.ReadAllBytes(filename);
            return Deserialize<T>(data);
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
            if (obj as Array != null)
            {
                //process as array
                Array? arr = obj as Array;
                obje.data = Functions.BitReaders.Write(obje.data, arr.Length, ref obje.position);

                for (int i = 0; i < arr.Length; i++)
                {
                    dynamic val = arr.GetValue(i);
                    Type typ = arr.GetValue(i).GetType();

                    bool isclass, isstruct, isarray, isprimitive, isstring, include;
                    gettypes(typ, out isclass, out isstruct, out isarray, out isprimitive, out isstring, out include);

                    processfield(obj, val, obje, objects, ref objid, isclass, isstruct, isarray, isprimitive, isstring);
                }
                return;
            }
            else if (obj.GetType().GetInterface(nameof(ICollection<T>)) != null)
            {
                //process as generic collection
                return;
            }

            //process as class/struct
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                bool isclass, isstruct, isarray, isprimitive, isstring, include;
                gettypes(field, out isclass, out isstruct, out isarray, out isprimitive, out isstring, out include);

                if (!include)
                {
                    continue;
                }

                obje.data = Functions.BitReaders.Write(obje.data, field.Name, ref obje.position);
                dynamic val = field.GetValue(obj);

                processfield(obj, val, obje, objects, ref objid, isclass, isstruct, isarray, isprimitive, isstring);
            }
        }
        private static void processfield<T>(T obj, dynamic val, objectEntry obje, List<objectEntry> objects, ref ulong objid, bool isclass, bool isstruct, bool isarray, bool isprimitive, bool isstring)
        {
            if ((isprimitive && !(isclass || isstruct || isarray)) || isstring)
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

                FieldInfo? fieldInfo = type.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    throw new FieldAccessException($"Invalid field {field} in type {type}\t\t@0x{pos.ToString("X")} ({pos}) in byte stream.");
                }

                bool isclass, isstruct, isarray, isprimitive, isstring, include;
                gettypes(fieldInfo, out isclass, out isstruct, out isarray, out isprimitive, out isstring, out include);

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
                else if (isarray)
                {
                    Type eltype = fieldInfo.FieldType.GetElementType();

                    ulong id;
                    Functions.BitReaders.Read(data, ref pos, out id);

                    dynamic val = new dynamic[0]; 

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

                    if (!found)
                    {
                        objectEntry tmp = dprocessarr(eltype, objects, data, id);
                        val = tmp.obj;
                    }


                    fieldInfo.SetValue(obj, val);
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
        private static objectEntry dprocessarr(Type childtype, List<objectEntry> objects, byte[] data, ulong pos)
        {
            int length;
            Functions.BitReaders.Read(data, ref pos, out length);

            Array array = Array.CreateInstance(childtype, length);

            objectEntry obje = new objectEntry(array, pos, bufferdefault)
            {
                position = pos
            };

            objects.Add(obje);
            obje.identifier = pos;

            bool isclass, isstruct, isarray, isprimitive, isstring, include;
            gettypes(childtype, out isclass, out isstruct, out isarray, out isprimitive, out isstring, out include);

            for (int idx = 0; idx < length; idx++)
            {
                if (isprimitive)
                {
                    dynamic val = Activator.CreateInstance(childtype);
                    val = Functions.BitReaders.Read(data, ref pos, val);
                    array.SetValue(val, idx);
                }
                else if (isstring)
                {
                    string val = "";
                    Functions.BitReaders.Read(data, ref pos, out val);
                    array.SetValue(val, idx);
                }
                else if (isstruct)
                {
                    dynamic val = Activator.CreateInstance(childtype);

                    //restart the recursive process on the struct
                    objectEntry tmp = dprocess(val, childtype, objects, data, pos);
                    val = tmp.obj;
                    array.SetValue(val, idx);
                    pos = tmp.position;

                }
                else if (isarray)
                {
                    Type eltype = childtype.GetElementType();

                    ulong id;
                    Functions.BitReaders.Read(data, ref pos, out id);

                    dynamic val = new object[0];

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

                    if (!found)
                    {
                        objectEntry tmp = dprocessarr(eltype, objects, data, id);
                        val = tmp.obj;
                    }

                    array.SetValue(val, idx);
                }
                else if (isclass)
                {
                    dynamic val = Activator.CreateInstance(childtype);

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
                        objectEntry tmp = dprocess(val, childtype, objects, data, id);
                        val = tmp.obj;
                    }

                    array.SetValue(val, idx);
                }
            }

            return obje;
        }

        private static void gettypes(FieldInfo field, out bool isclass, out bool isstruct, out bool isarray, out bool isprimitive, out bool isstring, out bool include)
        {
            isclass = field.FieldType.IsClass;
            isstruct = field.FieldType.IsValueType && !field.FieldType.IsEnum && !field.FieldType.IsPrimitive;
            isarray = field.FieldType.IsArray;
            isprimitive = field.FieldType.IsPrimitive;
            isstring = field.FieldType == typeof(string);

            include = field.IsPublic;

            foreach (CustomAttributeData data in field.CustomAttributes)
            {
                if (data.AttributeType == typeof(MonIgnore)) include = false;
                if (data.AttributeType == typeof(MonInclude)) include = true;
            }
        }
        private static void gettypes(Type field, out bool isclass, out bool isstruct, out bool isarray, out bool isprimitive, out bool isstring, out bool include)
        {
            include = true;
            isclass = field.IsClass;
            isstruct = field.IsValueType && !field.IsEnum && !field.IsPrimitive;
            isarray = field.IsArray;
            isprimitive = field.IsPrimitive;
            isstring = field == typeof(string);
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

        /// <summary>
        /// Attach this to a field to skip over when converting to Mon.
        /// </summary>
        public class MonIgnore : Attribute
        {

        }
        /// <summary>
        /// Attach this to a field to ensure it is not skipped when converting to Mon.
        /// </summary>
        public class MonInclude : Attribute
        {

        }
    }
}
