using martlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib
{
    public class CameraComponent : BehaviorComponent
    {
        public Vector DetectRegion, MapRegion;
        public int CameraID;
        internal RenderTarget target;

        public Vector GetRelativePosition(GameObject target)
        {
            Vector relativePos = target.transformComponent.Position - this.parent.Transform.Position;
            Vector rotatedPos = relativePos ^ this.parent.Transform.Rotation;
            return rotatedPos;
        }
    }
}
