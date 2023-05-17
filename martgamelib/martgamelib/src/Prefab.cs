using martlib;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace martgamelib
{
    public class PrefabLibrary
    {
        public List<Prefab> Prefabs; //YIPPEE

        public PrefabLibrary()
        {
            Prefabs = new List<Prefab>();
        }

        /// <summary>
        /// Returns the prefab by the provided name. Will return null if it doesn't exist.
        /// TODO: Search in log n time instead of n time.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Prefab? GetPrefab(string name)
        {
            for (int i = 0; i < Prefabs.Count; i++)
            {
                if (Prefabs[i].PrefabName.Equals(name))
                    return Prefabs[i];
            }
            return null;
        }
        public void LoadPrefabs(string directory)
        {
            Prefabs = new List<Prefab>();

            if (Directory.Exists(directory))
                Functions.Seek(directory, LoadPrefabsFromFile);
        }

        /// <summary>
        /// Specifically load .PFAB files
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal int LoadPrefabsFromFile(string filename)
        {
            if (!filename.ToLower().EndsWith(".pfab")) return -1;

            Prefab[]? tempPrefabArray = MonSerializer.Deserialize<Prefab[]>(filename);

            if (tempPrefabArray == null) return -1;

            for (int i = 0; i < tempPrefabArray.Length; i++)
            {
                tempPrefabArray[i].Construct();
                Prefabs.Add(tempPrefabArray[i]);
            }

            return 0;
        }

        /// <summary>
        /// Generates a prefab of the given object underneath the name
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public Prefab GeneratePrefab(GameObject obj, string prefabName)
        {
            Prefab temp = new Prefab();

            temp.PrefabName = prefabName;
            temp.flags = obj.Flags;
            temp.transform = obj.Transform;

            temp.components = new Prefab.ComponentFab[obj.componentCount];
            for (int i = 0; i < temp.components.Length; i++)
            {
                temp.components[i] = new Prefab.ComponentFab();
                temp.components[i].ComponentType = obj.components[i].GetType().ToString();
                temp.components[i].ComponentMON = MonSerializer.Serialize(obj.components[i]);
            }

            Prefabs.Add(temp);
            temp.Construct();

            return temp;
        }
    }
    public class Prefab
    {
        public string PrefabName;
        [MonSerializer.MonInclude]
        internal FlagStruct flags;
        [MonSerializer.MonInclude]
        internal Transform transform;
        [MonSerializer.MonInclude]
        internal ComponentFab[] components;

        internal void Construct()
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].ctype = ComponentManager.GetTypeFromName(components[i].ComponentType);
            }
        }

        internal class ComponentFab
        {
            public string ComponentType;
            public byte[] ComponentMON;

            [MonSerializer.MonIgnore]
            internal Type ctype;

            internal BehaviorComponent? getComponent()
            {
                return MonSerializer.Deserialize(ComponentMON, ctype) as BehaviorComponent;
            }
        }

        internal void Attach(GameObject obj)
        {
            obj.Flags = flags;
            obj.Transform = transform;
            for (int i = 0; i < components.Length; i++)
            {
                BehaviorComponent comp = components[i].getComponent();
                if (comp == null)
                    continue;
                obj.AddBehavior(comp);
            }
        }
    }
    public static class SceneLoader
    {
        public static void LoadNewScene(GameScene currentScene, string scenePath, string sceneName)
        {
            if (!Directory.Exists(scenePath)) throw new DirectoryNotFoundException($"Could not find {scenePath}");

            //Load scene entity prefabs
            PrefabLibrary sceneEntities = new PrefabLibrary();
            sceneEntities.LoadPrefabsFromFile(scenePath);

            //Load render layers
            RenderLayer[] layers = MonSerializer.Deserialize<RenderLayer[]>($"{scenePath}\\{sceneName}.lyr");
            
            //Apply the render layers to the scene
            for (int i = 0; i < layers.Length; i++)
            {
                currentScene.RegisterRenderLayer(layers[i]);
            }

            //Create an instance of every prefab in the scene
            List<Prefab> prefabs = sceneEntities.Prefabs;
            for (int i = 0; i < prefabs.Count; i++)
            {
                currentScene.Instantiate(prefabs[i]);
            }
        }
    }
}
