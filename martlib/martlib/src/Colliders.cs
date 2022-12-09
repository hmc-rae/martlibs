using System;


namespace martlib
{
    public static class Colliders
    {
        public class Line
        {
            public Vector start, end;

            public Line()
            {
                start = new Vector();
                end = new Vector();
            }

            public Line(Vector start, Vector end)
            {
                this.start = start;
                this.end = end;
            }

            public Line(double x1, double y1, double x2, double y2)
            {
                start = new Vector(x1, y1);
                end = new Vector(x2, y2);
            }

            /// <summary>
            /// Translates the line by a movement vector.
            /// </summary>
            /// <param name="translation"></param>
            public void Translate(Vector translation)
            {
                start += translation;
                end += translation;
            }
            /// <summary>
            /// Rotates the line around the origin point by the rotation vector.
            /// </summary>
            /// <param name="rotation"></param>
            public void Rotate(Vector rotation)
            {
                end = (end - start) ^ rotation;
                end += start;
            }

            public bool Intersects(Line line)
            {

                return pointswithin(line, start, end) && pointswithin(line, end, start);
            }

            internal bool pointswithin(Line line, Vector s, Vector e)
            {
                return false;
            }
        }
    
        public class Box
        {
            public Vector center;
            public Vector dimensions;

            /// <summary>
            /// Creates a box for for the radius x, y vector provided. Origin is at (0, 0).
            /// </summary>
            /// <param name="dimensions"></param>
            public Box(Vector dimensions)
            {
                this.dimensions = dimensions;
                center = new Vector();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="center"></param>
            /// <param name="dimensions"></param>
            public Box(Vector center, Vector dimensions) : this(dimensions)
            {
                this.center = center;
            }

            /// <summary>
            /// Returns true if the two boxes intersect.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Intersect(Box other)
            {
                Vector dis = (other.center - this.center).Absolute;

                Vector sep = other.dimensions + this.dimensions;

                return dis < sep;
            }

            /// <summary>
            /// Returns true if this box contains a given point.
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public bool Contains(Vector point)
            {
                Vector dis = (point - this.center).Absolute;

                return dis < dimensions;
            }
        }
    }
}
