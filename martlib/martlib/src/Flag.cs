using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martlib.src
{
    /// <summary>
    /// Represents a string of 64 flags, either high (1) or low (0). Depicted to the user by a 64-bit number.
    /// </summary>
    public struct FlagStruct    
    {
        internal static ulong FLAG_MAX = ulong.MaxValue;
        public static FlagStruct ZERO => new FlagStruct(0);
        public static FlagStruct MAX => new FlagStruct(FLAG_MAX);


        public ulong Flags;

        public FlagStruct(uint initial)
        {
            Flags = initial;
        }
        public FlagStruct(ulong initial = 0)
        {
            Flags = initial;
        }
        public FlagStruct(object obj)
        {
            Flags = 0;
            dynamic temp = obj;
            Flags = (ulong)temp;
        }
        public override string ToString()
        {
            return $"{Flags}";
        }

        /// <summary>
        /// Sets the flags to the value provided.
        /// </summary>
        /// <param name="a"></param>
        public void Set(ulong a)
        {
            Flags = a;
        }

        /// <summary>
        /// Sets the flags to the flag string if it is valid; else, does nothing.
        /// </summary>
        /// <param name="obj"></param>
        public void Set(object obj)
        {
            if (obj is Enum)
            {
                dynamic temp = obj;
                Flags = (ulong)temp;
            }
        }
        /// <summary>
        /// Sets the flags to the same flag string as the provided FlagStruct.
        /// </summary>
        /// <param name="a"></param>
        public void Set(FlagStruct a)
        {
            Set(a.Flags);
        }

        /// <summary>
        /// Returns true if the flag has some of the same flags as a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool Has(ulong a)
        {
            return (Flags | a) != 0;
        }
        /// <summary>
        /// Returns true if the flag has some of the same flags as a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool Has(FlagStruct a)
        {
            return (Flags | a.Flags) != 0;
        }
        /// <summary>
        /// Returns true if the flag has some of the same flags as a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool Has(object b)
        {
            if (b is Enum)
                return (Flags | (ulong)((dynamic)b)) != 0;
            return false;
        }

        public static FlagStruct operator +(FlagStruct a, ulong b)
        {
            a.Flags |= b;
            return a;
        }
        public static FlagStruct operator +(FlagStruct a, FlagStruct b)
        {
            a.Flags |= b.Flags;
            return a;
        }
        public static FlagStruct operator +(FlagStruct a, object b)
        {
            if (b is Enum)
            {
                dynamic temp = b;
                a.Flags |= (ulong)temp;
            }
            return a;
        }

        public static FlagStruct operator -(FlagStruct a, ulong b)
        {
            a.Flags &= ~b;
            return a;
        }
        public static FlagStruct operator -(FlagStruct a, FlagStruct b)
        {
            a.Flags &= ~b.Flags;
            return a;
        }
        public static FlagStruct operator -(FlagStruct a, object b)
        {
            if (b is Enum)
            {
                dynamic temp = b;
                a.Flags &= ~(ulong)temp;
            }
            return a;
        }

        public static Boolean operator ==(FlagStruct a, ulong b)
        {
            return a.Flags == b;
        }
        public static Boolean operator ==(FlagStruct a, FlagStruct b)
        {
            return a.Flags == b.Flags;
        }
        public static Boolean operator ==(FlagStruct a, object b)
        {
            if (b is Enum)
            {
                dynamic temp = b;
                return a.Flags == (ulong)temp;
            }
            return false;
        }

        public static Boolean operator !=(FlagStruct a, ulong b)
        {
            return !(a == b);
        }
        public static Boolean operator !=(FlagStruct a, FlagStruct b)
        {
            return !(a == b);
        }
        public static Boolean operator !=(FlagStruct a, object b)
        {
            if (b is Enum)
            {
                dynamic temp = b;
                return a.Flags != (ulong)temp;
            }
            return true;
        }

    }
}
