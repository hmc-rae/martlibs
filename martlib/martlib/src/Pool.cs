using System;

namespace martlib
{
    public class Pool<T>
    {
        internal T[] data;
        internal uint pos, max;
        public Pool(uint capacity) {
            data = new T[capacity];
            pos = 0;
            max = capacity;
        }

        /// <summary>
        /// Current size of the pool.
        /// </summary>
        public uint OccupiedSize
        {
            get { return pos; }
        }
        /// <summary>
        /// Maximum size of the pool.
        /// </summary>
        public uint MaxSize
        {
            get { return max; }
        }

        /// <summary>
        /// Returns the entry at a given position. Returns default if greater than the occupied size of the pool.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T? Get(int i)
        {
            if (i > pos || i < 0) return default(T);
            return data[i];
        }
        public void Add(T item)
        {
            if (pos >= max) throw new ObjectPoolOverflowException(this);
            data[pos++] = item;
        }
        public void Insert(int i, T item)
        {
            if (i < 0) throw new IndexOutOfRangeException();
            if (i >= pos)
            {
                Add(item);
                return;
            }

            Add(data[i]);
            data[i] = item;
        }
        public void Remove(int i)
        {
            if (i < 0 || i >= pos) throw new IndexOutOfRangeException();
            data[i] = data[--pos];
#pragma warning disable CS8601 // Possible null reference assignment.
            data[pos] = default(T);
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        /// <summary>
        /// Resets the pointer to 0.
        /// </summary>
        public void Flush()
        {
            pos = 0;
        }

        /// <summary>
        /// Wipes all unused (out of range) positions.
        /// </summary>
        public void PurgeUnused()
        {
            for (uint i = pos; i < max; ++i)
            {
                data[i] = default;
            }
        }

        /// <summary>
        /// Does nothing lol. Fix it later.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="item"></param>
        public void InsertOrdered(int i, T item)
        {

        }
        /// <summary>
        /// Does nothing lol. Fix it later. 
        /// </summary>
        /// <param name="i"></param>
        public void RemoveOrdered(int i) 
        { 
            
        }



        internal class ObjectPoolOverflowException : Exception
        {
            public Pool<T> origin_pool;
            public ObjectPoolOverflowException(Pool<T> p)
            {
                origin_pool = p;
            }
        }
    }
}
