﻿using communistOverhaul;
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
        private Runtimer fTime, tTime;

        internal bool updateflip = false;
        internal bool frameflip = false;

        public uint EntityCount
        {
            get
            {
                return objectPool.OccupiedSize + tempPool.OccupiedSize;
            }
        }
        /// <summary>
        /// gets the gameobject at position i
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        internal GameObject get(uint i)
        {
            if (i < objectPool.OccupiedSize)
            {
                return objectPool.Get((int)i);
            }
            else
            {
                i -= objectPool.OccupiedSize;
                return tempPool.Get((int)i);
            }
        }

        internal bool ChangeScene = false;
        internal GameScene? nextScene;

        //A destroy queue - logs the position in a list here.
        internal int[] rems;
        internal uint remspos;

        //Our render layers for this scene
        internal List<RenderLayer> renderlayers;

        public RenderLayer? GetRenderLayer(int RenderLayerID)
        {
            for (int l = 0, h = renderlayers.Count, i = renderlayers.Count / 2, p = -1; l < h && p != i; p = i, i = (l + h) / 2)
            {
                if (renderlayers[i].LayerID == RenderLayerID)
                {
                    return renderlayers[i];
                }
                else if (renderlayers[i].LayerID > RenderLayerID)
                {
                    l = i;
                }
                else if (renderlayers[i].LayerID < RenderLayerID)
                {
                    h = i;
                }
            }
            return null;
        }
        public void RegisterRenderLayer(RenderLayer layer)
        {
            layer.scene = this;
            layer.Create();
            for (int i = 0; i < renderlayers.Count; i++)
            {
                if (renderlayers[i].LayerID < layer.LayerID) continue;
                if (renderlayers[i].LayerID == layer.LayerID) throw new IDException(layer.LayerID, 1, "RenderLayer");
                if (renderlayers[i].LayerID > layer.LayerID)
                {
                    renderlayers.Insert(i, layer);
                    return;
                }
            }
            renderlayers.Add(layer);
        }
        internal class IDException : Exception 
        {
            private int id;
            private int code;
            private string str;

            public IDException(int i, int c, string t)
            {
                id = i;
                code = c;
                str = t;
            }
            public override string ToString()
            {
                return $"Tried to create duplicate instance of {str} #{id}\terror {code}";
            }
        }

        //Cameras for this scene
        internal List<CameraEntry> cameras;
        
        internal CameraEntry getCamera(int cameraID)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                if (cameras[i].ID == cameraID)
                {
                    return cameras[i];
                }
            }
            var cam = new CameraEntry();
            cam.ID = cameraID;
            cameras.Add(cam);
            return cam;
        }
        


        internal class CameraEntry
        {
            public int ID;
            public CameraComponent? camera;
        }

        public martgame Game => martgame;
        public GameWindow GameWindow => window;
        public InputManager Input => input;
        public Runtimer FrameTime => fTime;
        public Runtimer TickTime => tTime;

        public GameScene(uint poolSize, uint workerCount, martgame game)
        {
            window = game.Window;
            input = game.Input;
            fTime = game.FrameTime;
            tTime = game.TickTime;
            martgame = game;

            objectPool = new Pool<GameObject>(poolSize);
            tempPool = new Pool<GameObject>(poolSize);

            distributorPool = new DistributorPool(objectPool, this);

            if (workerCount == 0) 
                workerCount = 1;

            for (int i = 0; i < workerCount; i++)
                new WorkerPool(distributorPool);

            renderlayers = new List<RenderLayer>();

            rems = new int[poolSize];
            remspos = 0;
            cameras = new List<CameraEntry>();
        }

        internal void StartFrame()
        {
            input.prepoll();
            window.DispatchEvents();

            updateflip = !updateflip;
            distributorPool.Start();
        }

        internal void Synchronize()
        {
            distributorPool.Synchronize();
        }

        internal void EndFrame()
        {
            //Step 1 - Cycle backwards through rems, removing all that you encounter.
            for (uint i = remspos - 1; i >= 0 && i < rems.Length; i--)
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
                obj.updateflip = updateflip;
            }
            tempPool.Flush();
            tempPool.PurgeUnused();
        }

        internal void Render()
        {
            for (int i = 0; i < objectPool.OccupiedSize; i++)
            {
                GameObject obj = objectPool.Get(i);

                if (obj.frameflip == frameflip) continue;

                obj.frameflip = frameflip;
                obj.render();
            }
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
            obj.Alive = false;

            obj.updateflip = updateflip;
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
            obj.Alive = false;

            obj.updateflip = updateflip;
            return obj;
        }
        public GameObject Instantiate(Prefab prefab)
        {
            if (objectPool.OccupiedSize >= objectPool.MaxSize)
            {
                //TODO: THROW CONSOLE ERROR
                return null;
            }

            GameObject obj = new GameObject(this);
            obj.objid = scid++;
            tempPool.Add(obj);

            prefab.Attach(obj);
            obj.Alive = false;

            obj.updateflip = updateflip;
            return obj;
        }
        public GameObject Instantiate(Prefab prefab, Transform origin)
        {
            if (objectPool.OccupiedSize >= objectPool.MaxSize)
            {
                //TODO: THROW CONSOLE ERROR
                return null;
            }

            GameObject obj = new GameObject(this);
            obj.objid = scid++;
            tempPool.Add(obj);

            prefab.Attach(obj);

            obj.Transform = origin;
            obj.Alive = false;

            obj.updateflip = updateflip;
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

            obj.Alive = true;

            obj.updateflip = !updateflip;
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
            obj.Alive = true;
            
            obj.updateflip = !updateflip;
            return obj;
        }
        public GameObject InstantiateUrgent(Prefab prefab)
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

            prefab.Attach(obj);
            obj.Alive = true;

            obj.updateflip = !updateflip;
            return obj;
        }
        public GameObject InstantiateUrgent(Prefab prefab, Transform origin)
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

            prefab.Attach(obj);
            obj.Alive = true;

            obj.updateflip = !updateflip;
            return obj;
        }

        /// <summary>
        /// Returns a GameObject that has the given tag. Returns null if none exist. <br></br> Don't use this function every frame.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject? Find(string tag)
        {
            for (uint i = 0; i < EntityCount; i++)
            {
                GameObject obj = get(i);
                if (obj.Tag == tag)
                    return obj;
            }
            return null;
        }
        /// <summary>
        /// Returns an array of GameObjects that have the given tag. <br></br> Don't use this function every frame.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public GameObject[] FindAll(string tag)
        {
            int c = 0;
            for (uint i = 0; i < EntityCount; i++)
            {
                if (get(i).Tag == tag)
                    c++;
            }

            GameObject[] lis = new GameObject[c];
            for (uint i = 0, k = 0; i < EntityCount && k < c; i++)
            {
                GameObject obj = get(i);
                if (obj.Tag == tag)
                    lis[k++] = obj;
            }
            return lis;
        }

        /// <summary>
        /// Returns a GameObject that has the given flag string. Returns null if none exist. <br></br> Don't use this function every frame.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="exactMatch"></param>
        /// <returns></returns>
        public GameObject? Find(FlagStruct flags, bool exactMatch)
        {
            for (uint i = 0; i < EntityCount; i++)
            {
                GameObject obj = get(i);
                if (exactMatch)
                {
                    if (obj.Flags == flags)
                        return obj;
                }
                else
                {
                    if (obj.Flags.Has(flags))
                        return obj;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns an array of GameObjects that have the given flag string. <br></br> Don't use this function every frame.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="exactMatch"></param>
        /// <returns></returns>
        public GameObject[] FindAll(FlagStruct flags, bool exactMatch)
        {
            int c = 0;
            for (uint i = 0; i < EntityCount; i++)
            {
                GameObject obj = get(i);
                if (exactMatch)
                {
                    if (obj.Flags == flags)
                        c++;
                }
                else
                {
                    if (obj.Flags.Has(flags))
                        c++;
                }
            }

            GameObject[] lis = new GameObject[c];
            for (uint i = 0, k = 0; i < EntityCount && k < c; i++)
            {
                GameObject obj = get(i);
                if (exactMatch)
                {
                    if (obj.Flags == flags)
                        lis[k++] = obj;
                }
                else
                {
                    if (obj.Flags.Has(flags))
                        lis[k++] = obj;
                }
            }
            return lis;
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
            if (obj == null) return;

            lock (obj.updatelock)
            {
                if (obj.updateflip == obj.scene.updateflip)
                {
                    return;
                }
                obj.updateflip = obj.scene.updateflip;
            }

            if (!obj.destroy && obj.Alive)
            {
                obj.behavior();
            }
        }
    }
}
