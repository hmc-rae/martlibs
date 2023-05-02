using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using martlib;

namespace martgamelib
{
    public static class SpriteHandler
    {
        internal static EntityEntry[] entEntry;

        internal static TextureDirectory textureDirectory;
        public static void Initialize(string directoryPath, string entityEntryPath)
        {
            textureDirectory = JsonSerializer.Deserialize<TextureDirectory>(directoryPath);
            entEntry = JsonSerializer.Deserialize<EntityEntry[]>(entityEntryPath);

            if (textureDirectory == null) throw new ArgumentException($"Path {directoryPath} is invalid for textureDirectory.");
            if (entEntry == null) throw new ArgumentException($"Path {entityEntryPath} is invalid for entEntry");
        }
        /// <summary>
        /// Returns the EntityEntry for the respective entity - a package representing all animations of the entity.<br></br>
        /// A single entity can have multiple states, with each state having multiple frames of animation.
        /// </summary>
        /// <param name="EntityID"></param>
        /// <returns></returns>
        public static EntityEntry? GetEntityAnimations(int EntityID)
        {
            for (int i = 0; i < entEntry.Length; i++)
            {
                if (entEntry[i].ID == EntityID)
                {
                    if (!entEntry[i].built)
                        entEntry[i].Build(textureDirectory);
                    return entEntry[i];
                }
            }
            return null;
        }
    }
    public class EntityEntry
    {
        public int ID;
        public int AnimationCount;

        [MonSerializer.MonInclude]
        internal AnimationEntry[] Animations;

        [MonSerializer.MonIgnore]
        internal bool built = false;

        public class AnimationEntry
        {
            public int ID;
            public int FrameCount;

            [MonSerializer.MonInclude]
            internal SpriteFrame[] Frames;

            internal class SpriteFrame
            {
                [MonSerializer.MonInclude]
                public int ID;
                public int X, Y;
                public int W, H;

                [MonSerializer.MonIgnore]
                public Sprite sprite; 
                [MonSerializer.MonIgnore]
                public Texture parentTexture;
                [MonSerializer.MonIgnore]
                public IntRect cullingRect;
                
                public void Build(TextureDirectory directory)
                {
                    parentTexture = directory.GetTexture(ID);
                    cullingRect = new IntRect(new Vector2i(X, Y), new Vector2i(W, H));
                    sprite = new Sprite(parentTexture, cullingRect);
                }
            }

            internal void Build(TextureDirectory dir)
            {
                for (int i = 0; i < Frames.Length; i++)
                {
                    Frames[i].Build(dir);
                }
            }
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
        public Sprite GetFrame(int anim, int frame)
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

        internal void Build(TextureDirectory dir)
        {
            for (int i = 0; i < Animations.Length; i++)
            {
                Animations[i].Build(dir);
            }
        }
    }
    public class TextureDirectory
    {
        public int textureCount;
        public TextureEntry[] textures;
        public class TextureEntry
        {
            [MonSerializer.MonIgnore]
            public Texture texture; 
            [MonSerializer.MonIgnore]
            public bool loaded;
            public string address;
            public TextureEntry(string line)
            {
                address = line;
                loaded = false;
            }
            public Texture getTexture()
            {
                if (!loaded)
                {
                    texture = new Texture(address);
                    loaded = true;
                }
                return texture;
            }
        }
        public TextureDirectory(string filename)
        {
            if (!File.Exists(filename)) return;

            string[] lines = File.ReadAllLines(filename);
            textures = new TextureEntry[lines.Length];
            textureCount = lines.Length;

            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = new TextureEntry(lines[i]);
            }
        }

        public Texture GetTexture(int id)
        {
            if (id >= textureCount) return null;

            return textures[id].getTexture();
        }
    }
}
