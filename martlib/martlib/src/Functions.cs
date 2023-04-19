using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace martlib
{
    public static class Functions
    {
        /// <summary>
        /// Performs an operation on all files in a directory. 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="action"></param>
        public static void Seek(string directory, Func<string, int> action)
        {
            string[] files = Directory.GetFiles(directory);
            for (int i = 0; i < files.Length; i++)
            {
                action(files[i]);
            }

            string[] folders = Directory.GetDirectories(directory);
            for (int i = 0; i < folders.Length; i++)
            {
                Seek(folders[i], action);
            }
        }

        /// <summary>
        /// A collection of functions for manipulation of byte arrays.
        /// </summary>
        public static class BitReaders
        {
            public static T[] Double<T>(T[] list)
            {
                T[] newlist = new T[list.Length * 2];
                for (int i = 0; i < list.Length; i++)
                {
                    newlist[i] = list[i];
                }
                return newlist;
            }
            public static byte[] Write(byte[] output, object? value, ref ulong position)
            {
                if (value == null) return output;

                //TODO default object shit

                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, value);
                    byte[] tmp = ms.ToArray();
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (position >= (ulong)output.Length)
                            output = Double(output);
                        output[position++] = tmp[i];
                    }
                }
                return output;
            }

            public static byte[] Write(byte[] output, long value, ref ulong position)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)(value & 0xff);
                    value >>= 8;
                }
                return output;
            }
            public static byte[] Write(byte[] output, ulong value, ref ulong position)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)(value & 0xff);
                    value >>= 8;
                }
                return output;
            }
            public static byte[] Write(byte[] output, byte value, ref ulong position)
            {
                if (position >= (ulong)output.Length)
                    output = Double(output);
                output[position++] = (byte)(value & 0xff);
                return output;
            }
            public static byte[] Write(byte[] output, sbyte value, ref ulong position)
            {
                if (position >= (ulong)output.Length)
                    output = Double(output);
                output[position++] = (byte)(value & 0xff);
                return output;
            }
            public static byte[] Write(byte[] output, short value, ref ulong position)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)(value & 0xff);
                    value >>= 8;
                }
                return output;
            }
            public static byte[] Write(byte[] output, ushort value, ref ulong position)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)(value & 0xff);
                    value >>= 8;
                }
                return output;
            }
            public static byte[] Write(byte[] output, int value, ref ulong position)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)(value & 0xff);
                    value >>= 8;
                }
                return output;
            }
            public static byte[] Write(byte[] output, uint value, ref ulong position)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)(value & 0xff);
                    value >>= 8;
                }
                return output;
            }
            

            public static byte[] Write(byte[] output, float value, ref ulong position)
            {
                int temp = BitConverter.SingleToInt32Bits(value);
                return Write(output, temp, ref position);
            }
            public static byte[] Write(byte[] output, double value, ref ulong position)
            {
                long temp = BitConverter.DoubleToInt64Bits(value);
                return Write(output, temp, ref position);
            }

            public static byte[] Write(byte[] output, bool value, ref ulong position)
            {
                if (position >= (ulong)output.Length)
                    output = Double(output);
                output[position++] = (byte)(value ? 0xff : 0);
                return output;
            }

            public static byte[] Write(byte[] output, string value, ref ulong position)
            {
                char[] chars = value.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (position >= (ulong)output.Length)
                        output = Double(output);
                    output[position++] = (byte)chars[i];
                }
                return output;
            }

            public static void Read(byte[] data, ref ulong position, out short output)
            {
                output = 0;
                for (int i = 0; i < 2; i++)
                {
                    output = (short)(data[position++] << (i * 8));
                }
            }
            public static void Read(byte[] data, ref ulong position, out ushort output)
            {
                output = 0;
                for (int i = 0; i < 2; i++)
                {
                    output = (ushort)(data[position++] << (i * 8));
                }
            }
            public static void Read(byte[] data, ref ulong position, out int output)
            {
                output = 0;
                for (int i = 0; i < 2; i++)
                {
                    output = (int)(data[position++] << (i * 8));
                }
            }
            public static void Read(byte[] data, ref ulong position, out uint output)
            {
                output = 0;
                for (int i = 0; i < 2; i++)
                {
                    output = (uint)(data[position++] << (i * 8));
                }
            }
            public static void Read(byte[] data, ref ulong position, out long output)
            {
                output = 0;
                for (int i = 0; i < 2; i++)
                {
                    output = (long)(data[position++] << (i * 8));
                }
            }
            public static void Read(byte[] data, ref ulong position, out ulong output)
            {
                output = 0;
                for (int i = 0; i < 2; i++)
                {
                    output = (ulong)(data[position++] << (i * 8));
                }
            }

            public static void Read(byte[] data, ref ulong position, out bool output)
            {
                output = data[position++] != 0;
            }

            public static void Read(byte[] data, ref ulong position, out float output)
            {
                int tmp;
                Read(data, ref position, out tmp);
                output = BitConverter.Int32BitsToSingle(tmp);
            }
            public static void Read(byte[] data, ref ulong position, out double output)
            {
                long tmp;
                Read(data, ref position, out tmp);
                output = BitConverter.Int64BitsToDouble(tmp);
            }

            public static void Read(byte[] data, ref ulong position, out string output)
            {
                output = "";
                for (; data[position] != 0; position++)
                {
                    output += (char)data[position];
                }
            }
        }
    }
}
