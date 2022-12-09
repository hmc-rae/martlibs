using System;

namespace martlib
{
    public static class Communism
    {
        /// <summary>
        /// An object that manages multiple threads operating on one list of objects, distributing work as needed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Distributor<T>
        {
            private object mLock;
            private int mRPos;

            internal Barrier mBarrier;

            private List<Worker<T>> mWorkers;

            public Distributor ()
            {
                mLock = new object();
                mRPos = 0;
                mWorkers = new List<Worker<T>>();
                mBarrier = new Barrier(1);
            }

            public bool IsComplete()
            {
                lock (mLock)
                {
                    return isComplete(mRPos);
                }
            }

            public void Reset()
            {
                mRPos = 0;
                mBarrier.SignalAndWait();
            }
            public T? GetObject()
            {
                lock (mLock)
                {
                    return getFromCollection(mRPos++);
                }
            }

            protected internal abstract T? getFromCollection(int i);
            protected internal abstract bool isComplete(int i);

            internal void addWorker(Worker<T> worker)
            {
                mWorkers.Add(worker);
            }

            public void Dispose()
            {
                for (int i = 0; i < mWorkers.Count; i++)
                {
                    mWorkers[i].dispose();
                }
            }
        }
        /// <summary>
        /// A class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Worker<T>
        {
            internal Thread mThread;
            internal Distributor<T> mClient;
            internal Barrier mBarrier;
            internal object mBreakerLock;
            internal bool mBreaker;

            public Worker(Distributor<T> client)
            {
                mClient = client;
                mBarrier = client.mBarrier;
                mBarrier.AddParticipant();
                mThread = new Thread(new ThreadStart(run));
                mThread.Start();

                mBreakerLock = new object();
                mBreaker = false;

                client.addWorker(this);
            }

            private void run()
            {
                while (true)
                {
                    mBarrier.SignalAndWait(); 
                    lock (mBreakerLock)
                        if (mBreaker) return;

                    while (true)
                    {
                        T? obj = mClient.GetObject();

                        if (obj == null)
                            break;

                        ProcessObject(obj);
                    }
                }
            }

            protected internal abstract void ProcessObject(T? obj);

            internal void dispose()
            {
                lock (mBreakerLock)
                {
                    mBreaker = true;
                }
            }
        }
    }
}
