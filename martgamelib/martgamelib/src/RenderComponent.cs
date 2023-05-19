using martlib;
using SFML.Graphics;
using System;
using System.Text.Json.Serialization;

//TODO: Render function for SpriteComponent

namespace martgamelib
{
    public class RenderComponent : BehaviorComponent
    {
        [MonSerializer.MonIgnore, EditorHidden]
        public CameraComponent RenderCamera;
        internal GameScene.CameraEntry camEntry;

        [MonSerializer.MonInclude, EditorVisible]
        internal int camID;
        [EditorHidden]
        public int CameraID
        {
            get
            {
                return camID;
            }
            set
            {
                camID = value;
                if (parent != null)
                    camEntry = parent.scene.getCamera(camID);
            }
        }

        public override void OnCreate()
        {
            //Acquire camera by CameraLayer
            camEntry = parent.Scene.getCamera(CameraID);
        }
        public virtual void Render() { }

        public bool CanRender()
        {
            if (camEntry == null)
            {
                camEntry = parent.Scene.getCamera(CameraID);
            }

            if (camEntry.camera != null)
            {
                RenderCamera = camEntry.camera;
                return true;
            }
            return false;
        }
    }

    public class SpriteRenderer : RenderComponent
    {
        [MonSerializer.MonInclude]
        public int EntityID;

        [MonSerializer.MonIgnore, EditorHidden]
        public EntityEntry EntityAnimations;

        [MonSerializer.MonIgnore, EditorVisible]
        public int AnimState, AnimFrame;

        [MonSerializer.MonIgnore, EditorHidden]
        public bool CompletedAnim;

        public override void OnCreate()
        {
            base.OnCreate();

            //Assume that EntityID is set by now
            AnimState = 0;
            AnimFrame = 0;

            EntityAnimations = SpriteHandler.GetEntityAnimations(EntityID);
            CompletedAnim = false;
        }

        public override void Render()
        {
            if (!CanRender()) return;

            Vector relative = RenderCamera.GetRelativePosition(Parent);
            if (!RenderCamera.IsVisible(relative)) return;

            relative = RenderCamera.GetMappedPosition(relative);

            Sprite spr = EntityAnimations.GetFrame(AnimState, AnimFrame);
            if (spr == null) return;

            spr.Scale = martgame.ToSFMLVector(parent.Transform.Scale);
            spr.Position = martgame.ToSFMLVector(relative);
            spr.Rotation = (float)(parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);

            RenderCamera.Render(spr);
        }
        public override void OnTick()
        {
            AnimFrame++;
            if (AnimFrame >= EntityAnimations.GetFrameCount(AnimState))
            {
                AnimFrame = 0;
                CompletedAnim = true;
            }
        }
    }
    public class BoxRenderer : RenderComponent
    {
        [MonSerializer.MonIgnore, EditorHidden]
        private static RectangleShape _shape = new RectangleShape(new SFML.System.Vector2f(1, 1));

        [MonSerializer.MonInclude]
        public Color color;

        public override void OnCreate()
        {
            base.OnCreate();
            _shape.Origin = new SFML.System.Vector2f(0.5f, 0.5f);
        }
        public override void Render()
        {
            if (!CanRender()) return;

            Vector relative = RenderCamera.GetRelativePosition(Parent);
            if (!RenderCamera.IsVisible(relative)) return;

            relative = RenderCamera.GetMappedPosition(relative);

            _shape.Scale = martgame.ToSFMLVector(parent.Transform.Scale * RenderCamera.PixelsPerUnit);
            _shape.Position = martgame.ToSFMLVector(relative);
            _shape.Rotation = (float)(parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);
            _shape.FillColor = color;

            RenderCamera.Render(_shape);

        }
    }
    public class TextRenderer : RenderComponent
    {
        internal Text _text = new Text();
        internal Font font;

        [EditorHidden]
        public double UnitSize
        {
            get
            {
                return CharacterSize / RenderCamera.pixelsPerUnit.X;
            }
            set
            {
                if (CanRender())
                    CharacterSize = (uint)(value * RenderCamera.PixelsPerUnit.X);
                else
                    CharacterSize = 30;
            }
        }

        [EditorHidden]
        public string FontPath
        {
            get
            {
                return fontPath;
            }
            set
            {
                fontPath = value;
                font = new Font(fontPath);
                if (text != null)
                    _text.Font = font;
            }
        }
        [MonSerializer.MonInclude, EditorVisible]
        internal string fontPath;

        [EditorHidden]
        public Vector Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
                FloatRect bounds = _text.GetLocalBounds();
                _text.Origin = new SFML.System.Vector2f(bounds.Left + ((float)origin.X * bounds.Width), bounds.Top + ((float)origin.Y * bounds.Height));
            }
        }
        [MonSerializer.MonInclude, EditorVisible]
        internal Vector origin;

        [EditorHidden]
        public uint CharacterSize
        {
            get
            {
                return characterSize;
            }
            set
            {
                characterSize = value;
                _text.CharacterSize = characterSize;
                Origin = origin;
            }
        }
        [MonSerializer.MonInclude, EditorVisible]
        internal uint characterSize = 30;

        [EditorHidden]
        public string DisplayedString
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                _text.DisplayedString = text;
                Origin = origin;
            }
        }
        [MonSerializer.MonInclude, EditorVisible]
        internal string text;

        [EditorHidden]
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                _text.FillColor = color;
            }
        }
        [MonSerializer.MonInclude, EditorVisible]
        internal Color color;

        public TextRenderer() { }
        public TextRenderer(string path, string text)
        {
            fontPath = path;
            this.text = text;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            if (File.Exists(fontPath))
            {
                font = new Font(fontPath);
                _text = new Text("", font);
            }
            else
            {
                _text = new Text();
            }

            _text.FillColor = color;

            _text.DisplayedString = text;

            _text.CharacterSize = characterSize;

            Origin = origin;
        }

        public override void Render()
        {
            if (!CanRender()) return;

            Vector relative = RenderCamera.GetRelativePosition(Parent);
            if (!RenderCamera.IsVisible(relative)) return;

            relative = RenderCamera.GetMappedPosition(relative);

            _text.Scale = martgame.ToSFMLVector(parent.Transform.Scale);
            _text.Position = martgame.ToSFMLVector(relative);
            _text.Rotation = (float)(parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);
            _text.FillColor = color;

            RenderCamera.Render(_text);
        }
    }
    public class TargetRenderer : RenderComponent
    {
        [MonSerializer.MonInclude, EditorVisible]
        internal uint targetWidth, targetHeight;
        [EditorHidden]
        public uint TargetWidth
        {
            get
            {
                return targetWidth;
            }
            set
            {
                //only set if not yet constructed
                if (!targetMade)
                    targetWidth = value;
            }
        }
        [EditorHidden]
        public uint TargetHeight
        {
            get
            {
                return targetHeight;
            }
            set
            {
                //only set if not yet constructed
                if (!targetMade)
                    targetHeight = value;
            }
        }

        internal bool targetMade = false;

        [EditorHidden]
        public RenderTexture Target => target;
        private RenderTexture target;
        private Sprite renderSprite;

        public override void OnCreate()
        {
            base.OnCreate();
            target = new RenderTexture(targetWidth, targetHeight);
            renderSprite = new Sprite(target.Texture);
            targetMade = true;
        }

        public override void Render()
        {
            base.Render();

            target.Display();

            //draw sprite
            if (!CanRender()) return;

            Vector relative = RenderCamera.GetRelativePosition(Parent);
            if (!RenderCamera.IsVisible(relative)) return;

            relative = RenderCamera.GetMappedPosition(relative);

            renderSprite.Scale = martgame.ToSFMLVector(parent.Transform.Scale);
            renderSprite.Position = martgame.ToSFMLVector(relative);
            renderSprite.Rotation = (float)(parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);

            RenderCamera.Render(renderSprite);

            target.Clear();
        }
    }
}
