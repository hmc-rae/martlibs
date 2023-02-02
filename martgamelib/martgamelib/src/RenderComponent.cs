using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib.src
{
    public class RenderComponent : BehaviorComponent
    {
        public CameraComponent RenderCamera;

        public virtual void Render() { }
    }

    public class SpriteRenderer : RenderComponent
    {

    }
}
