using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martlib.src
{
    /// <summary>
    /// A type of rotating queue. Objects pushed into the buffer will take up an addressable index, but after enough objects are pushed they will rotate out of the buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CyclicBuffer<T>
    {
        internal T[] data;
        internal int maxSize;
        internal int currentSize;
        internal int currentIndex;

        public CyclicBuffer(int maxSize) 
        {
            data = new T[maxSize];
            this.maxSize = maxSize;
            currentIndex = maxSize - 1;
            currentSize = 0;
        }

        public int Count => currentSize;

    }
}
