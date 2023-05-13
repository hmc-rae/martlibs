using martlib;
using SFML.Graphics;
using System;
using System.Text.Json.Serialization;

//TODO: Render function for SpriteComponent

namespace martgamelib
{
    public class RenderComponent : BehaviorComponent
    {
        [JsonIgnore]
        public CameraComponent RenderCamera;
        internal GameScene.CameraEntry camEntry;
        [JsonInclude]
        internal int camID;
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

        [MonSerializer.MonIgnore]
        public EntityEntry EntityAnimations;
        [MonSerializer.MonIgnore]
        public int AnimState, AnimFrame;
        [MonSerializer.MonIgnore]
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
        [MonSerializer.MonIgnore]
        private static RectangleShape _shape = new RectangleShape(new SFML.System.Vector2f(1, 1));

        [MonSerializer.MonInclude]
        public Color color;

        double counter = 0;

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

            _shape.Scale = martgame.ToSFMLVector(parent.Transform.Scale);
            _shape.Position = martgame.ToSFMLVector(relative);
            _shape.Rotation = (float)(parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);
            _shape.FillColor = color;

            RenderCamera.Render(_shape);

        }
    }
}
