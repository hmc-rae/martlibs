using martlib;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib
{
    public sealed class CameraComponent : BehaviorComponent
    {
        /// <summary>
        /// The regions of detection - everything within the DetectRegion is located, and mapped to the MapRegion. <br></br>
        /// Usually the DetectRegion should encapsulate a wider range than the MapRegion.
        /// </summary>
        public Vector DetectRegion, MapRegion;
        public int CameraID => camID;
        public int LayerID
        {
            get
            {
                return layID;
            }
            set
            {
                layID = value;
                target = parent.scene.GetRenderLayer(layID);
            }
        }

        [MonSerializer.MonInclude]
        internal int camID;
        [MonSerializer.MonInclude]
        internal int layID;

        public CameraComponent()
        {

        }
        public CameraComponent(int CameraID, int LayerID)
        {
            this.layID = LayerID;
            this.camID = CameraID;
        }

        internal RenderLayer? target; 
        public bool CanRender
        {
            get
            {
                return target != null;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //Register this camera in the scene
            GameScene.CameraEntry entry = parent.Scene.getCamera(camID);
            if (entry.camera != null)
            {
                throw new GameScene.IDException(camID, 2, "Camera");
            }
            entry.camera = this;

            //Find the layer entity
            target = parent.scene.GetRenderLayer(LayerID);
        }
        public Vector GetRelativePosition(GameObject target)
        {
            Vector relativePos = target.transformComponent.Position - this.parent.Transform.Position;
            Vector rotatedPos = relativePos ^ this.parent.Transform.Rotation.Flip;
            return rotatedPos;
        }
        public bool IsVisible(Vector pos)
        {
            return pos.Absolute < DetectRegion;
        }
        public Vector GetMappedPosition(Vector pos)
        {
            if (!CanRender) return Vector.UNIT_X;

            pos /= MapRegion;
            pos *= target.PixelRadius;
            pos = pos.Flip;
            pos += target.PixelRadius;
            return pos;
        }

        public void Render(Drawable obj)
        {
            if (!CanRender) return;

            target.Draw(obj);
        }
    }
}
