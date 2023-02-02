using System;
using System.Numerics;
using martlib;

namespace martgamelib
{
    public class BehaviorComponent
    {
        //Hidden thingies that get assigned on creation
#pragma warning disable CS8618
        internal GameScene scene;
        internal GameObject parent;
        internal Runtimer time;
        internal InputManager inputManager;
#pragma warning restore CS8618

        //Visible get-only aspects

        public GameScene Scene => scene;
        public GameObject Parent => parent;
        public Runtimer Time => time;
        public GameWindow GameWindow => scene.GameWindow;
        public InputManager Input => inputManager;

        //override methods
        public virtual void OnCreate() { }
        public virtual void OnDestroy() { }
        public virtual void OnFrame() { }
        
        /// <summary>
        /// Returns an empty GameObject ready to have a list applied.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public GameObject Instantiate()
        {
            //TODO
            return new GameObject(Scene);
        }

    }
}
