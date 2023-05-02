using System;

namespace martlib
{
    public static class Communism
    {
        public const string VERSION = "1.0";
        /// <summary>
        /// An object that manages multiple threads operating on one list of objects, distributing work as needed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Distributor<T>
        {
            private object mLock;
            private int mRPos;

            internal Barrier SynchLatch, StartLatch;

            private List<Worker<T>> mWorkers;

            public Distributor ()
            {
                mLock = new object();
                mRPos = 0;
                mWorkers = new List<Worker<T>>();
                SynchLatch = new Barrier(1);
                StartLatch = new Barrier(1);

                flipswitch = true;
            }

            /// <summary>
            /// Returns whether or not the threads are all done collaborating.
            /// </summary>
            /// <returns></returns>
            public bool IsComplete()
            {
                lock (mLock)
                {
                    return isComplete(mRPos);
                }
            }


            internal bool flipswitch;
            /// <summary>
            /// This method resynchronizes the threads. Call after starting.
            /// </summary>
            public void Synchronize()
            {
                if (flipswitch) return;
                SynchLatch.SignalAndWait();
                mRPos = 0;
                complete();
                flipswitch = true;
            }

            /// <summary>
            /// This method starts the thread loops. Call before synchronizing.
            /// </summary>
            public void Start()
            {
                if (!flipswitch) return;
                StartLatch.SignalAndWait();
                flipswitch = false;
            }
            public T? GetObject()
            {
                lock (mLock)
                {
                    return getFromCollection(mRPos++);
                }
            }

            /// <summary>
            /// Override this with optional behavior to execute post-synchronization.
            /// </summary>
            protected internal virtual void complete() { }
            /// <summary>
            /// Override this with code to return an item from the collection at index 'i'. You can assume that this code will run in a single thread.
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            protected internal abstract T? getFromCollection(int i);
            /// <summary>
            /// Override this with code to signify whether or not index 'i' is valid (hence, completed processing).
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            protected internal abstract bool isComplete(int i);

            internal void addWorker(Worker<T> worker)
            {
                mWorkers.Add(worker);
            }
            /// <summary>
            /// Call this to destroy all threads.
            /// </summary>
            public void Dispose()
            {
                for (int i = 0; i < mWorkers.Count; i++)
                {
                    mWorkers[i].dispose();
                }
                Synchronize();
                Start();
            }
        }
        /// <summary>
        /// An object that operates on items passed to it by a provided client distributor. <br></br>
        /// Override functions in 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Worker<T>
        {
            internal Thread mThread;
            internal Distributor<T> mClient;
            internal Barrier SynchLatch, StartLatch;
            internal object mBreakerLock;
            internal bool mBreaker;

            public Worker(Distributor<T> client)
            {
                mClient = client;

                StartLatch = client.StartLatch;
                StartLatch.AddParticipant();

                SynchLatch = client.SynchLatch;
                SynchLatch.AddParticipant();

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
                    StartLatch.SignalAndWait();
                    lock (mBreakerLock)
                        if (mBreaker) return;

                    while (true)
                    {
                        T? obj = mClient.GetObject();

                        if (obj == null)
                            break;

                        ProcessObject(obj);
                    }
                    SynchLatch.SignalAndWait();
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
