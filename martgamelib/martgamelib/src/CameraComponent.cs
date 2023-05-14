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
            Vector relativePos = target.Transform.Position - this.parent.Transform.Position;
            Vector rotatedPos = relativePos ^ this.parent.Transform.Rotation.Flip;
            return rotatedPos;
        }
        public bool IsVisible(Vector pos)
        {
            return pos.Absolute < DetectRegion;
        }
        /// <summary>
        /// Returns the position passed relative to the map region, where (1, 1) is the top right corner of the mapped region.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector GetMappedPosition(Vector pos)
        {
            if (!CanRender) return Vector.UNIT_X;

            pos /= MapRegion;
            pos *= target.PixelRadius;
            pos = pos.Flip;
            pos += target.PixelRadius;
            return pos;
        }
        /// <summary>
        /// Sets the MapRegion of this camera to the number of 'units' that exist on the target this camera holds.
        /// <br></br>
        /// Sets the DetectRegion to MapRegion + DetectDelta
        /// </summary>
        /// <param name="UPP"></param>
        /// <param name="DetectDelta"></param>
        public void SetUnitsPerPixel(Vector UPP, Vector DetectDelta)
        {
            if (!CanRender) return;
            Vector screenSize = target.PixelSize;

            MapRegion = screenSize / UPP;
            DetectRegion = MapRegion + DetectDelta;
        }
        /// <summary>
        /// Maps a MousePosition vector (ideally provided by the InputManager - a vector representing some real point on the screen) to an abstract position relative to this camera, based on what the camera can see.
        /// </summary>
        /// <param name="MousePosition"></param>
        /// <returns></returns>
        public Vector GetRelativeMousePosition(Vector MousePosition)
        {
            if (!CanRender) return MousePosition;

            //step 1: get the vector origin of the target
            Console.WriteLine($"POS: {target.MainWindowPosition}");
            Vector tOrig = target.MainWindowPosition;

            //2: relative to orig
            MousePosition -= tOrig;

            //3: scaled relative to orig
            MousePosition /= target.PixelScale;

            //4: -1 <= v <= 1 on targ
            MousePosition /= target.PixelRadius;

            //5: Mult by camera mapreg
            MousePosition *= MapRegion;

            //6: Rotate by hruiahui
            MousePosition ^= Parent.Transform.Rotation.Flip;

            return MousePosition;
        }

        public void Render(Drawable obj)
        {
            if (!CanRender) return;

            target.Draw(obj);
        }
    }
}
