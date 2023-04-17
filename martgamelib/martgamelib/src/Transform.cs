using martlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib.src
{
    public struct Transform
    {
        public Vector Position, Rotation, Scale;

        public Transform()
        {
            Position = new Vector();
            Rotation = new Vector();
            Scale = new Vector();
        }
        public Transform(double x, double y)
        {
            Position = new Vector(x, y);
            Rotation = new Vector();
            Scale = Vector.XY;
        }
        public Transform(Vector position)
        {
            this.Position = position;
            Rotation = new Vector();
            Scale = Vector.XY;
        }
        public Transform(double x, double y, double w, double z, double a, double b)
        {
            Position = new Vector(x, y);
            Rotation = new Vector(w, z);
            Scale = new Vector(a, b);
        }
        public Transform(Vector position, Vector rotation, Vector scale)
        {
            Rotation = rotation;
            Position = position;
            Scale = scale;
        }
    }
}
