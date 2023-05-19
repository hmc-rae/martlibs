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
        [EditorHidden]
        public Vector DetectRegion
        {
            get
            {
                return detectRegion;
            }
            set
            {
                detectRegion = value;
            }
        }
        /// <summary>
        /// The regions of detection - everything within the DetectRegion is located, and mapped to the MapRegion. <br></br>
        /// Usually the DetectRegion should encapsulate a wider range than the MapRegion. <br></br>
        /// Changing this will automatically update the PixelsPerUnit value.
        /// </summary>
        [EditorHidden]
        public Vector MapRegion
        {
            get
            {
                return mapRegion;
            }
            set
            {
                mapRegion = value;

                if (!CanRender) return;

                Vector screenSize = target.PixelSize;
                pixelsPerUnit = screenSize / mapRegion;
            }
        }
        [EditorVisible, MonSerializer.MonInclude]
        internal Vector detectRegion, mapRegion;

        /// <summary>
        /// The number of pixels that will exist in a single unit - (1, 1).
        /// </summary>
        public Vector PixelsPerUnit
        {
            get
            {
                return pixelsPerUnit;
            }
            set
            {
                pixelsPerUnit = value;
                if (!CanRender) return;

                Vector screenSize = target.PixelSize;
                mapRegion = screenSize / pixelsPerUnit;
                detectRegion = mapRegion + (Vector.XY * 2);
            }
        }
        [EditorVisible, MonSerializer.MonInclude]
        internal Vector pixelsPerUnit = new Vector(32, 32);

        [EditorHidden]
        public int CameraID => camID;
        [EditorHidden]
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

                if (!CanRender) return;

                MapRegion = mapRegion;
            }
        }

        [MonSerializer.MonInclude, EditorVisible]
        internal int camID;
        [MonSerializer.MonInclude, EditorVisible]
        internal int layID;

        public CameraComponent()
        {
            pixelsPerUnit = new Vector(32, 32);
            mapRegion = new Vector(10, 10);
            detectRegion = new Vector(12, 12);
        }
        public CameraComponent(int CameraID, int LayerID)
        {
            this.layID = LayerID;
            this.camID = CameraID;

            pixelsPerUnit = new Vector(32, 32);
            mapRegion = new Vector(8, 8);
            detectRegion = new Vector(12, 12);
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
            if (pos == Vector.ZERO) return Vector.ZERO + target.PixelRadius;

            pos /= MapRegion;
            pos *= target.PixelRadius;
            pos = pos.Flip;
            pos += target.PixelRadius;
            return pos;
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
            Vector tOrig = target.MainWindowPosition;

            //2: relative to orig
            MousePosition -= tOrig;

            //3: scaled relative to orig
            MousePosition /= target.PixelScale;

            //4: -1 <= v <= 1 on targ
            MousePosition /= target.PixelRadius;

            //5: Mult by camera mapreg
            MousePosition *= MapRegion;

            //6: Flip
            MousePosition = MousePosition.Flip;

            return MousePosition;
        }
        public Vector GetRealMousePosition(Vector MousePosition)
        {
            MousePosition = GetRelativeMousePosition(MousePosition).Flip;

            //6: Rotate by camera rotation
            MousePosition ^= Parent.Transform.Rotation.Flip;

            //7: Offset by camera natural position
            MousePosition += Parent.Transform.Position.Flip;

            return MousePosition.Flip;
        }

        public void Render(Drawable obj)
        {
            if (!CanRender) return;

            target.Draw(obj);
        }
    }
}
