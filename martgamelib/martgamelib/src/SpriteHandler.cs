using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.IO;
using martlib;

namespace martgamelib
{
    public static class SpriteHandler
    {
        internal static EntityRegister entRegister;
        internal static TextureRegister texRegister;

        public static void Initialize(string texPath, string entPath)
        {
            if (File.Exists(texPath))
            {
                texRegister = MonSerializer.Deserialize<TextureRegister>(texPath);
            }
            else
            {
                texRegister = new TextureRegister();
            }
            texRegister.Build();

            if (File.Exists(entPath))
            {
                entRegister = MonSerializer.Deserialize<EntityRegister>(entPath);
            }
            else
            {
                entRegister = new EntityRegister();
            }
            entRegister.Build();

        }

        /// <summary>
        /// Returns the EntityEntry for the respective entity - a package representing all animations of the entity.<br></br>
        /// A single entity can have multiple states, with each state having multiple frames of animation.
        /// </summary>
        /// <param name="EntityID"></param>
        /// <returns></returns>
        public static EntityEntry? GetEntityAnimations(int EntityID)
        {
            return entRegister.getentry(EntityID);
        }
    }
    public class EntityRegister
    {
        [MonSerializer.MonInclude]
        private int entC; 

        [MonSerializer.MonInclude]
        private EntityEntry[] entries;

        [MonSerializer.MonIgnore]
        public int EntityCount
        {
            get
            {
                return entC;
            }
        }

        [MonSerializer.MonIgnore]
        private Dictionary<int, EntityEntry> map;

        public EntityRegister()
        {
            entries = new EntityEntry[0];
            entC = 0;
            map = new Dictionary<int, EntityEntry>();
        }
        internal void Build()
        {
            map = new Dictionary<int, EntityEntry>(entC);
            for (int i = 0; i < entC; i++)
            {
                map.Add(entries[i].ID, entries[i]);
            }
        }

        internal EntityEntry? getentry(int id)
        {
            if (!map.ContainsKey(id)) return null;
            return map[id];
        }

        
    }
    internal class TextureRegister
    {
        [MonSerializer.MonInclude]
        internal int texC;
        [MonSerializer.MonInclude]
        internal TextureEntry[] textures;
        [MonSerializer.MonInclude]
        internal string basePath;

        [MonSerializer.MonIgnore]
        internal Dictionary<int, TextureEntry> map;

        internal TextureRegister()
        {
            texC = 0;
            basePath = "";
            map = new Dictionary<int, TextureEntry>();
            textures = new TextureEntry[0];
        }
        internal Texture? GetTexture(int id)
        {
            if (!map.ContainsKey(id)) return null;

            return map[id].getTexture(basePath);
        }

        internal void Build()
        {
            map = new Dictionary<int, TextureEntry>(texC);
            for (int i = 0; i < texC; i++)
            {
                map.Add(textures[i].id, textures[i]);
            }
        }
        internal class TextureEntry
        {
            [MonSerializer.MonIgnore]
            private Texture texture;

            [MonSerializer.MonIgnore]
            public bool loaded = false;

            public int id;
            public string address;

            public TextureEntry()
            {
                id = -1;
                address = "";
                loaded = false;
            }
            public Texture getTexture(string basepath)
            {
                if (id == -1) return null;
                if (!loaded)
                {
                    texture = new Texture($"{basepath}\\{address}");
                    loaded = true;
                }
                return texture;
            }
        }
    }

    public class EntityEntry
    {
        public int ID => id;
        public int AnimationCount => animC;

        [MonSerializer.MonInclude]
        internal AnimationEntry[] Animations;
        [MonSerializer.MonInclude]
        internal int id;
        [MonSerializer.MonInclude]
        internal int animC;

        [MonSerializer.MonIgnore]
        internal bool built = false;

        public EntityEntry()
        {
            Animations = new AnimationEntry[0];
            id = 0;
            animC = 0;
            built = false;
        }

        public bool ValidAnimationFrame(int anim, int frame)
        {
            if (anim < AnimationCount && anim > 0)
            {
                if (frame < Animations[anim].FrameCount && frame > 0)
                    return true;
            }
            return false;
        }
        public Sprite? GetFrame(int anim, int frame)
        {
            if (ValidAnimationFrame(anim, frame))
                return Animations[anim].Frames[frame].sprite;
            return null;
        }
        public int GetFrameCount(int anim)
        {
            if (anim < AnimationCount && anim > 0)
            {
                return Animations[anim].FrameCount;
            }
            return 0;
        }

        internal void Build(TextureRegister texReg)
        {
            if (built) return;
            for (int i = 0; i < Animations.Length; i++)
            {
                Animations[i].Build(texReg);
            }
            built = true;
        }

        //Animation entry class
        internal class AnimationEntry
        {
            public int ID;
            public int FrameCount;

            [MonSerializer.MonInclude]
            internal SpriteFrame[] Frames;

            internal AnimationEntry()
            {
                ID = 0;
                FrameCount = 0;
                Frames = new SpriteFrame[0];
            }

            internal void Build(TextureRegister texReg)
            {
                for (int i = 0; i < Frames.Length; i++)
                {
                    Frames[i].Build(texReg);
                }
            }

            internal class SpriteFrame
            {
                public int ID;
                public int X, Y;
                public int W, H;

                [MonSerializer.MonIgnore]
                public Sprite sprite;
                [MonSerializer.MonIgnore]
                public Texture parentTexture;
                [MonSerializer.MonIgnore]
                public IntRect cullingRect;

                private static Sprite defsprite = new Sprite();
                private static Texture deftex = new Texture(1, 1);

                internal SpriteFrame()
                {
                    parentTexture = deftex;
                    sprite = defsprite;
                }

                internal void Build(TextureRegister texReg)
                {
                    parentTexture = texReg.GetTexture(ID);
                    cullingRect = new IntRect(new Vector2i(X, Y), new Vector2i(W, H));
                    sprite = new Sprite(parentTexture, cullingRect);
                }
            }
        }
    }
}
