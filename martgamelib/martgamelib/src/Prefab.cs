using martlib.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib.src
{
    public static class PrefabLibrary
    {
        public static Prefab[] Prefabs; //YIPPEE

        public static Prefab GetPrefab(string name)
        {
            for (int i = 0; i < Prefabs.Length; i++)
            {
                if (Prefabs[i].PrefabName.Equals(name))
                    return Prefabs[i];
            }
            return null;
        }
    }
    public class Prefab
    {
        public string PrefabName;

        //Prefab data
        public FlagStruct flags;

        public ComponentFab[] components;

        public class ComponentFab
        {
            public Type ComponentType;
            public string ComponentJSON;
        }
    }
}
