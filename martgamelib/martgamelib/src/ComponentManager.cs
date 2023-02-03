using System;
using System.Reflection;
using martlib.src;


namespace martgamelib.src
{
    public static class ComponentManager
    {
        internal static List<Type> types;
        internal static Dictionary<string, Type> typeLookup;
        public static void Initialize(string directory)
        {
            types = new List<Type>();
            typeLookup = new Dictionary<string, Type>();

            Functions.Seek(directory, findComponents);
        }

        public static Type GetTypeFromName(string name)
        {
            if (typeLookup.ContainsKey(name))
                return typeLookup[name];
            return null;
        }

        internal static int findComponents(string filename)
        {
            if (!filename.ToLower().EndsWith(".dll")) return -1;

            var DLL = Assembly.LoadFrom(filename);

            foreach (Type type in DLL.GetTypes())
            {
                var obj = Activator.CreateInstance(type);

                if (obj as BehaviorComponent == null || obj is BehaviorComponent) continue;

                //Add the type to the list of types unless it exists
                if (typeLookup.ContainsKey(type.Name)) return -2;
                typeLookup.Add(type.Name, type);
                types.Add(type);
            }

            return 0;
        }
    }

    public class SceneManifest
    {
        public uint PoolSize;
        public uint WorkerCount;
        public EntityManifest[] Entities;
    }

    public class EntityManifest
    {
        public FlagStruct Flags;
        public ComponentManifest[] Components;
    }
    public class ComponentManifest
    {
        public string TypeName;
        public string JSONFields;
    }
}
