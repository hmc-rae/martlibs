using System;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text.Json.Serialization;
using martgamelib;
using martlib;

namespace martgamelib
{
    public class BehaviorComponent
    {
        //Hidden thingies that get assigned on creation
#pragma warning disable CS8618
        [JsonIgnore]
        internal GameScene scene;
        internal GameObject parent;
        internal Runtimer timeA, timeB;
        internal InputManager inputManager;
#pragma warning restore CS8618

        //Visible get-only aspects

        public GameScene Scene => scene;
        public GameObject Parent => parent;
        public Runtimer FrameTime => timeA;
        public Runtimer TickTime => timeB;
        public GameWindow GameWindow => scene.GameWindow;
        public InputManager Input => inputManager;

        //override methods
        public virtual void OnCreate() { }
        public virtual void OnDestroy() { }
        public virtual void OnTick() { }
        public virtual void OnFrame() { }

        //detection methods
        private static Dictionary<Type, List<GameObject>> objectTypeListings = new Dictionary<Type, List<GameObject>>();
        internal void registerComponent()
        {
            Type myType = this.GetType();

            if (!objectTypeListings.ContainsKey(myType))
            {
                List<GameObject> temp = new List<GameObject>();
                objectTypeListings.Add(myType, temp);
            }

            var list = objectTypeListings[myType];
            list.Add(this.Parent);
        }
        internal void deregisterComponent()
        {
            Type myType = this.GetType();

            if (!objectTypeListings.ContainsKey(myType)) return;

            var list = objectTypeListings[myType];
            
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].objid == parent.objid)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        public List<GameObject>? GetObjectsWithType()
        {
            if (!objectTypeListings.ContainsKey(GetType())) return null;
            return objectTypeListings[GetType()];
        }

        /// <summary>
        /// Returns an empty GameObject ready to have components attached. It will be added to the scene at the end of the current tick.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject Instantiate()
        {
            return Scene.Instantiate();
        }
        /// <summary>
        /// Returns an empty GameObject ready to have components attached. It will be added to the scene at the end of the current tick.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject Instantiate(Transform origin)
        {
            return Scene.Instantiate(origin);
        }
        /// <summary>
        /// Returns a GameObject with a prefab applied. It will be added to the scene at the end of the current tick.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Instantiate(Prefab prefab)
        {
            return Scene.Instantiate(prefab);
        }
        /// <summary>
        /// Returns a GameObject with a prefab applied. It will be added to the scene at the end of the current tick.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Instantiate(Prefab prefab, Transform origin)
        {
            return Scene.Instantiate(prefab, origin);
        }

        /// <summary>
        /// Returns an empty GameObject ready to have components attached. It'll be added instantly to the scene.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject InstantiateUrgent()
        {
            return Scene.InstantiateUrgent();
        }
        /// <summary>
        /// Returns an empty GameObject ready to have components attached. It'll be added instantly to the scene.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject InstantiateUrgent(Transform origin)
        {
            return Scene.InstantiateUrgent(origin);
        }
        /// <summary>
        /// Returns a GameObject with a prefab applied. It'll be added instantly to the scene.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject InstantiateUrgent(Prefab prefab)
        {
            return Scene.InstantiateUrgent(prefab);
        }
        /// <summary>
        /// Returns a GameObject with a prefab applied. It'll be added instantly to the scene.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject InstantiateUrgent(Prefab prefab, Transform origin)
        {
            return Scene.InstantiateUrgent(prefab, origin);
        }
    }
}
