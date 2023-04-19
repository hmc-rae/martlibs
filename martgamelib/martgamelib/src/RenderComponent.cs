using System;
using System.Text.Json.Serialization;

//TODO: Render function for SpriteComponent

namespace martgamelib
{
    public class RenderComponent : BehaviorComponent
    {
        [JsonIgnore]
        public CameraComponent RenderCamera;
        [JsonInclude]
        internal int CameraLayer;
        public override void OnCreate()
        {
            //Acquire camera by CameraLayer
        }
        public virtual void Render() { }
    }

    public class SpriteRenderer : RenderComponent
    {
        [JsonInclude]
        public int EntityID;

        [JsonIgnore]
        public EntityEntry EntityAnimations;
        [JsonIgnore]
        public int AnimState, AnimFrame;
        [JsonIgnore]
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
            
        }
        public override void OnFrame()
        {
            AnimFrame++;
            if (AnimFrame >= EntityAnimations.GetFrameCount(AnimState))
            {
                AnimFrame = 0;
                CompletedAnim = true;
            }
        }
    }
}
