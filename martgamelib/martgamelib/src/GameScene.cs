using communistOverhaul;
using martgamelib;
using martlib;


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

        internal bool ChangeScene = false;
        internal GameScene? nextScene;

        //A destroy queue - logs the position in a list here.
        internal int[] rems;
        internal uint remspos;

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

            distributorPool = new DistributorPool(objectPool, this);

            if (workerCount == 0) 
                workerCount = 1;

            for (int i = 0; i < workerCount; i++)
                new WorkerPool(distributorPool);

            renderTargets = new List<RenderTarget>();

            rems = new int[poolSize];
            remspos = 0;
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
            //Step 1 - Cycle backwards through rems, removing all that you encounter.
            for (uint i = remspos - 1; i >= 0; i--)
            {
                objectPool.Remove(rems[i]);
            }

            if (remspos > 0)
            {
                remspos = 0;
                objectPool.PurgeUnused();
            }

            //Add all EOF objects.
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

        private uint scid;
        public GameObject Instantiate()
        {
            if (objectPool.OccupiedSize >= objectPool.MaxSize)
            {
                //TODO: THROW CONSOLE ERROR
                return null;
            }

            GameObject obj = new GameObject(this);
            obj.objid = scid++;
            tempPool.Add(obj);

            return obj;
        }
        public GameObject Instantiate(Transform origin)
        {
            if (objectPool.OccupiedSize >= objectPool.MaxSize)
            {
                //TODO: THROW CONSOLE ERROR
                return null;
            }

            GameObject obj = new GameObject(this, origin);
            obj.objid = scid++;
            tempPool.Add(obj);

            return obj;
        }

        public GameObject InstantiateUrgent()
        {
            if (objectPool.OccupiedSize >= objectPool.MaxSize)
            {
                //TODO: THROW CONSOLE ERROR
                return null;
            }

            GameObject obj = new GameObject(this);
            obj.objid = scid++;
            objectPool.Add(obj);

            obj.freshMade = false;

            return obj;
        }
        public GameObject InstantiateUrgent(Transform origin)
        {
            if (objectPool.OccupiedSize >= objectPool.MaxSize)
            {
                //TODO: THROW CONSOLE ERROR
                return null;
            }

            GameObject obj = new GameObject(this, origin);
            obj.objid = scid++;
            objectPool.Add(obj);

            obj.freshMade = false;

            return obj;
        }
    }
}

namespace communistOverhaul
{
    internal class TickRunner
    {
        internal bool ContinueRunning;
        internal GameScene scene;
        internal martgame game;
        private Thread thread;

        public TickRunner(GameScene scene, martgame game)
        {
            ContinueRunning = true;
            this.game = game;
            this.scene = scene;
            thread = new Thread(Run);
        }

        internal void Start()
        {
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

                scene.TickTime.Wait();

                //if scene ready to change, change it locally and raise flag in game
                if (scene.ChangeScene)
                {
                    this.scene = scene.nextScene;
                    game.ChangedScene = true;
                }
            }
        }
    }
    internal class DistributorPool : Communism.Distributor<GameObject>
    {
        private Pool<GameObject> objectPool;
        internal GameScene scene;

        public DistributorPool(Pool<GameObject> objects, GameScene scene)
        {
            objectPool = objects;
            this.scene = scene;
        }

        protected override GameObject? getFromCollection(int i)
        {
            GameObject? obj = objectPool.Get(i);
            if (obj != null)
            {
                if (obj.destroy)
                {
                    scene.rems[scene.remspos++] = i;
                }
            }
            return obj;
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
            if (obj != null && !obj.destroy && obj.Alive)
                obj.behavior();
        }
    }
}
