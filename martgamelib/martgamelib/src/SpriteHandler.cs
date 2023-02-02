using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib.src
{
    public static class SpriteHandler
    {

    }
    public class EntityEntry
    {
        public uint EntityID;
        public uint EntityAnimationCount;

        public AnimationEntry[] EntityAnimations;
        public class AnimationEntry
        {
            public uint AnimationID;
            public uint AnimationFrameCount;

            public SpriteFrame[] SpriteFrames;

            public class SpriteFrame
            {
                public Sprite sprite;
                public Texture parentTexture;
                public IntRect cullingRect;

                public SpriteFrame(Texture tex, int x, int y, int w, int h)
                {
                    parentTexture = tex;
                    cullingRect = new IntRect(new Vector2i(x, y), new Vector2i(w, h));
                    sprite = new Sprite(parentTexture, cullingRect);
                }
            }

            public AnimationEntry(EntityEntryTheory.AnimationEntryTheory theory)
            {
                AnimationID = theory.AnimationID;
                AnimationFrameCount = theory.AnimationFrameCount;

                SpriteFrames = new SpriteFrame[AnimationFrameCount];

                for (int i = 0; i < AnimationFrameCount; ++i)
                {

                }
            }
        }

        public EntityEntry(EntityEntryTheory theory)
        {
            EntityID = theory.EntityID;
            EntityAnimationCount = theory.EntityAnimationCount;

            EntityAnimations = new AnimationEntry[EntityAnimationCount];

            for (int i = 0; i < EntityAnimationCount; ++i)
            {

            }
        }

        
    }

    public class EntityEntryTheory
    {
        public uint EntityID;
        public uint EntityAnimationCount;

        public AnimationEntryTheory[] Animations;
        public class AnimationEntryTheory
        {
            public uint AnimationID;
            public uint AnimationFrameCount;

            public SpriteFrameTheory[] Frames;

            public class SpriteFrameTheory
            {
                public uint SpriteSheetID;

                public int SpriteSheetX, SpriteSheetY;
                public int SpriteWidth, SpriteHeight;
            }
        }
    }
}
