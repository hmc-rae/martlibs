using communistOverhaul;
using martgamelib;
using martgamelib.src;
using martlib;
using SFML.Window;
using System;
using static System.Formats.Asn1.AsnWriter;


namespace martgamelib
{
    public class GameScene
    {
        private martgame martgame;
        private GameWindow window;
        private InputManager input;
        private Pool<GameObject> objectPool, tempPool;
        private DistributorPool distributorPool;
        private Runtimer timeA, timeB;

        /// <summary>
        /// We now have our own internal list of rendertargets, meaning scenes can be cached.
        /// </summary>
        internal List<RenderTarget> renderTargets;

        public martgame Game => martgame;
        public GameWindow GameWindow => window;
        public InputManager Input => input;
        public Runtimer FrameTime => timeA;
        public Runtimer TickTime => timeB;

        public GameScene(uint poolSize, uint workerCount, martgame game)
        {
            window = game.Window;
            input = game.Input;
            timeA = game.FrameTime;
            timeB = game.TickTime;
            martgame = game;

            objectPool = new Pool<GameObject>(poolSize);
            tempPool = new Pool<GameObject>(poolSize);

            distributorPool = new DistributorPool(objectPool);

            if (workerCount == 0) 
                workerCount = 1;

            for (int i = 0; i < workerCount; i++)
                new WorkerPool(distributorPool);

            renderTargets = new List<RenderTarget>();
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
    internal class TickRunner
    {
        public bool ContinueRunning;
        internal GameScene scene;
        internal martgame game;
        internal Thread thread;

        public TickRunner(GameScene scene, martgame game)
        {
            ContinueRunning = true;
            this.game = game;
            this.scene = scene;
        }

        public void Start()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        internal void Run()
        {
            scene.TickTime.Start();
            while (ContinueRunning)
            {
                scene.StartFrame();
                scene.Synchronize();

                scene.EndFrame();

                //if scene ready to change, change it locally and raise flag in game
                
                //game.ChangedScene = true;

                scene.TickTime.Wait();
            }
        }
    }
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
            obj.behavior();
        }
    }
}
