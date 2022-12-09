using communistOverhaul;
using martgamelib;
using martlib;
using SFML.Window;
using System;


namespace martgamelib
{
    public class GameScene
    {
        private GameWindow window;
        private InputManager manager;
        private Pool<GameObject> objectPool;
        private DistributorPool distributorPool;

        public GameWindow GameWindow { get { return window; } }
        public InputManager Input { get { return manager; } }

        public GameScene(uint w, uint h, string title, Vector dpu, uint poolSize, uint workerCount, Styles style)
        {
            manager = new InputManager();
            window = new GameWindow(w, h, title, dpu, manager, style);

            objectPool = new Pool<GameObject>(poolSize);

            distributorPool = new DistributorPool(objectPool);

            if (workerCount == 0) 
                workerCount = 1;

            for (int i = 0; i < workerCount; i++)
                new WorkerPool(distributorPool);

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
