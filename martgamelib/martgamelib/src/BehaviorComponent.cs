using System;
using System.Numerics;
using System.Text.Json.Serialization;
using martgamelib.src;
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
        
        /// <summary>
        /// Returns an empty GameObject ready to have a list applied. It will be added to the scene at the end of frame.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject Instantiate()
        {
            return Scene.Instantiate();
        }
        /// <summary>
        /// Returns an empty GameObject ready to have a list applied. It will be added to the scene at the end of frame.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject Instantiate(Transform origin)
        {
            return Scene.Instantiate(origin);
        }
        public GameObject Instantiate(Prefab prefab)
        {
            return new GameObject(Scene);
        }

        /// <summary>
        /// Returns an empty GameObject ready to have a list applied. It'll be added instantly to the scene.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject InstantiateUrgent()
        {
            return Scene.InstantiateUrgent();
        }
        /// <summary>
        /// Returns an empty GameObject ready to have a list applied. It'll be added instantly to the scene.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject InstantiateUrgent(Transform origin)
        {
            return Scene.InstantiateUrgent(origin);
        }
    }
}
