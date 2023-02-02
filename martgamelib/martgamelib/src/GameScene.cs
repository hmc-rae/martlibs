using communistOverhaul;
using martgamelib;
using martgamelib.src;
using martlib;
using SFML.Window;
using System;


namespace martgamelib
{
    public class GameScene
    {
        private martgame martgame;
        private GameWindow window;
        private InputManager input;
        private Pool<GameObject> objectPool, tempPool;
        private DistributorPool distributorPool;
        private Runtimer time;

        public martgame Game => martgame;
        public GameWindow GameWindow => window;
        public InputManager Input => input;
        public Runtimer Time => time;

        public GameScene(uint poolSize, uint workerCount, martgame game)
        {
            window = game.Window;
            input = game.Input;
            time = game.Time;
            martgame = game;

            objectPool = new Pool<GameObject>(poolSize);
            tempPool = new Pool<GameObject>(poolSize);

            distributorPool = new DistributorPool(objectPool);

            if (workerCount == 0) 
                workerCount = 1;

            for (int i = 0; i < workerCount; i++)
                new WorkerPool(distributorPool);
        }

        internal void StartFrame()
        {
            distributorPool.Start();
        }

        internal void Synchronize()
        {
            distributorPool.Synchronize();
        }
        internal void EndFrame()
        {
            for (int i = 0; i < tempPool.OccupiedSize; ++i)
            {
                var obj = tempPool.Get(i);
                if (obj == null) break;
                obj.create();
                objectPool.Add(obj);
            }
            tempPool.Flush();
            tempPool.PurgeUnused();
        }
    }
}

namespace communistOverhaul
{
    internal class DistributorPool : Communism.Distributor<GameObject>
    {
        private Pool<GameObject> objectPool;

        public DistributorPool(Pool<GameObject> objects)
        {
            objectPool = objects;
        }

        protected override GameObject? getFromCollection(int i)
        {
            return objectPool.Get(i);
        }
        protected override bool isComplete(int i)
        {
            return i > objectPool.OccupiedSize;
        }
    }
    internal class WorkerPool : Communism.Worker<GameObject>
    {
        public WorkerPool(Communism.Distributor<GameObject> pool) : base(pool) { }

        protected override void ProcessObject(GameObject? obj)
        {
            //Call all internal methods on the object
        }
    }
}
