﻿using System;
using System.Text.Json;
using System.Reflection;
using martgamelib.src;
using martlib;

//TODO: Finish the function to add behavior to a game object & enqueue it

namespace martgamelib
{
    public class GameObject
    {
        //private stuff that cannot be edited but has public gets
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8618
        internal GameScene scene;
#pragma warning restore IDE0079 // Remove unnecessary suppression
        internal Runtimer time;
        internal GameWindow window;
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning restore CS8618

        public GameScene Scene => scene;
#pragma warning restore IDE0079 // Remove unnecessary suppression
        public Runtimer Time => time;
        public GameWindow GameWindow => window;

        internal Dictionary<Type, BehaviorComponent> table;
        internal List<BehaviorComponent> components;
        internal int componentCount;

        internal Transform transformComponent;
        internal RenderComponent renderComponent;

        /// <summary>
        /// Generates a fresh GameObject at a given position.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="PositionData"></param>
        public GameObject(GameScene scene, string PositionData)
        {
            this.scene = scene;
            time = scene.Time;
            window = scene.GameWindow;

            table = new Dictionary<Type, BehaviorComponent>(32);
            components = new List<BehaviorComponent>(32);

            transformComponent = AddBehavior(typeof(Transform), PositionData) as Transform;
        }
        /// <summary>
        /// Generates a fresh GameObject at a default position.
        /// </summary>
        /// <param name="scene"></param>
        public GameObject(GameScene scene)
        {
            this.scene = scene;
            time = scene.Time;
            window = scene.GameWindow;

            table = new Dictionary<Type, BehaviorComponent>(32);
            components = new List<BehaviorComponent>(32);

            transformComponent = AddBehavior(typeof(Transform)) as Transform;
        }

        /// <summary>
        /// Adds a new behavior component to this gameobject.
        /// Returns null if it could not add the type to the object (invalid type, or already exists).
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public BehaviorComponent? AddBehavior(Type componentType)
        {
            var obj = Activator.CreateInstance(componentType);
            if (obj as BehaviorComponent == null || obj is BehaviorComponent)
                return null;

            //Add it and queue for EoF update
            addToObject(obj as BehaviorComponent, componentType);

            return obj as BehaviorComponent;
        }

        /// <summary>
        /// Adds a new behavior component to this gameobject, providing it default values as specified by the json string
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public BehaviorComponent? AddBehavior(Type componentType, string jsonString)
        {
            var obj = Activator.CreateInstance(componentType);
            if (obj as BehaviorComponent == null || obj is BehaviorComponent)
                return null;

            obj = JsonSerializer.Deserialize(jsonString, componentType);

            //Add it and queue for EoF update
            addToObject(obj as BehaviorComponent, componentType);

            return obj as BehaviorComponent;
        }

        //Return true if successfully added
        internal bool addToObject(BehaviorComponent component, Type componentType)
        {
            component.parent = this;
            component.scene = scene;
            component.time = time;
            component.inputManager = scene.Input;
            
            if (table.ContainsKey(componentType)) return false; //cant add if already exist

            table.Add(componentType, component);
            components.Insert(componentCount, component);

            ++componentCount;

            return true;
        }
        //runs the create functions on all components
        internal void create()
        {
            for (int i = 0; i < componentCount; i++)
            {
                components[i].OnCreate();
            }
        }
    }
}