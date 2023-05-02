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
    }
    public class Prefab
    {
        public string PrefabName;

        //Prefab data
        public FlagStruct flags;

        public ComponentFab[] components;

        public void Construct()
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].ctype = ComponentManager.GetTypeFromName(components[i].ComponentType);
            }
        }

        public class ComponentFab
        {
            public string ComponentType;
            public byte[] ComponentMON;

            [MonSerializer.MonIgnore]
            internal Type ctype;
        }
    }
    public static class SceneLoader
    {
        public static void LoadNewScene(GameScene currentScene)
        {

        }
    }
}
