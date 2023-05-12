using System;
/*
    VECTOR OPERATORS:
        v + v       Translate/Add
        v - v       Translate/Subtract

        v * v       Multiply terms
        v * s       Multiply by scalar     

        v / v       Divide terms
        v / s       Divide by scalar

        v % v       Modulo terms
        v % s       Modulo by scalar

        v ^ v       Project/rotate

        ==          Equivalence
        !=          Nonequivalence
        <           Strictly less than (both axes)
        <=          Less than (either axis)
        >           Strictly greater than (both axes)
        >=          Greater than (either axis)
*/
namespace martlib
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public struct Vector
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public const double RAD_CONST = 180.0 / Math.PI;
        public const string VERSION = "1.0.3";
        /// <summary>
        /// A vector representing <0, 0>
        /// </summary>
        public static Vector ZERO
        {
            get { return new Vector(0, 0); }
        }
        /// <summary>
        /// A vector representing the X axis, <1, 0>
        /// </summary>
        public static Vector UNIT_X
        {
            get { return new Vector(1, 0); }
        }
        /// <summary>
        /// A vector representing the Y axis, <0, 1>
        /// </summary>
        public static Vector UNIT_Y
        {
            get { return new Vector(0, 1); }
        }
        /// <summary>
        /// A vector representing both axis as one, <1, 1>
        /// </summary>
        public static Vector XY
        {
            get { return new Vector(1, 1); }
        }

        public double X, Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Add (translate) a vector by a vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }
        /// <summary>
        /// Subtract a vector from a vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }
        /// <summary>
        /// Multiplies vector by a singular.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.X * b, a.Y * b);
        }
        /// <summary>
        /// Multiplies vector by a singular.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector operator *(double b, Vector a)
        {
            return new Vector(a.X * b, a.Y * b);
        }
        /// <summary>
        /// Multiplies terms in one vector by another vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator *(Vector a, Vector b)
        {
            return new Vector(a.X * b.X, a.Y * b.Y);
        }
        /// <summary>
        /// Projects vector a onto vector b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator ^(Vector a, Vector b)
        {
            Vector o = b.UnitVector;
            return (o.Orthogonal * a.Y) + (o * a.X);
        }
        /// <summary>
        /// Divides a vector by a singular.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator /(Vector a, double b)
        {
            return new Vector(a.X / b, a.Y / b);
        }
        /// <summary>
        /// Modulo a vector by a singular
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator %(Vector a, double b)
        {
            return new Vector(a.X % b, a.Y % b);
        }
        /// <summary>
        /// Divides a vector over another vector. Isn't really a legitimate operation but that's okay.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator /(Vector a, Vector b)
        {
            return new Vector(a.X / b.X, a.Y / b.Y);
        }
        /// <summary>
        /// Modulo a vector over another vector. Isn't really legitimate either.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator %(Vector a, Vector b)
        {
            return new Vector(a.X % b.X, a.Y % b.Y);
        }
        //Comparisons
        /// <summary>
        /// True if both vectors have the exact same values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// Negation of '=='.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        /// <summary>
        /// True if a is less than b on both axes.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(Vector a, Vector b)
        {
            return a.X <= b.X && a.Y <= b.Y;
        }

        /// <summary>
        /// True if a is greater than b on both axes.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(Vector a, Vector b)
        {
            return a.X >= b.X && a.Y >= b.Y;
        }

        /// <summary>
        /// True if a is less than b on either axis (or, not greater on both axes).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <=(Vector a, Vector b)
        {
            return a.X <= b.X || a.Y <= b.Y;
        }

        /// <summary>
        /// True if a is greater than b on either axis (or, not less on both axes)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >=(Vector a, Vector b)
        {
            return a.X >= b.X || a.Y >= b.Y;
        }

        /// <summary>
        /// Set the position of this vector.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// The orthogonal projection of this vector - the vector rotated by 90 degrees, counter-clockwise (or, rotated by the Unit Y vector). 
        /// </summary>
        public Vector Orthogonal
        {
            get
            {
                return new Vector(-Y, X); 
            }
        }

        /// <summary>
        /// The clockwise orthogonal projection of this vector - the vector rotated by 90 degrees, clockwise (or, rotated by the inverse Unit Y vector). 
        /// </summary>
        public Vector OrthogonalC
        {
            get
            {
                return new Vector(Y, -X);
            }
        }

        /// <summary>
        /// The flipped version of this vector along the Y axis - rotating a vector by the flipped version of itself will result in <1, 0>
        /// </summary>
        public Vector Flip
        {
            get
            {
                return new Vector(X, -Y);
            }
        }

        /// <summary>
        /// The squared length of this vector.
        /// </summary>
        public double SqrMagnitude
        {
            get
            {
                return (X * X) + (Y * Y);
            }
        }

        /// <summary>
        /// The length of this vector.
        /// </summary>
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(SqrMagnitude);
            }
        }
        /// <summary>
        /// The unit vector form of this vector.
        /// </summary>
        public Vector UnitVector
        {
            get
            {
                double m = Magnitude;
                if (m == 0) m = 1;
                return new Vector(X / m, Y / m);
            }
        }
        /// <summary>
        /// The negation of this vector: add them together to have the vector (0, 0).
        /// </summary>
        public Vector Inverse
        {
            get
            {
                return new Vector(-X, -Y);
            }
        }
        /// <summary>
        /// The absolute version of this vector.
        /// </summary>
        public Vector Absolute
        {
            get
            {
                return new Vector(X < 0 ? -X : X, Y < 0 ? -Y : Y);
            }
        }
        /// <summary>
        /// Returns a vector raised to a given exponent.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Vector Power(double e)
        {
            return new Vector(Math.Pow(X, e), Math.Pow(Y, e));
        }

        /// <summary>
        /// Returns true if this vector is within a specified offset (off) from heading
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="off"></param>
        /// <returns></returns>
        public bool Within(Vector heading, Vector off)
        {
            double max = (UNIT_X - off).SqrMagnitude;
            double dir = (heading - this).SqrMagnitude;
            return dir <= max;
        }

        /// <summary>
        /// Returns a string representation of the vector in format <X, Y>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(2);
        }
        /// <summary>
        /// Returns a string representation of the vector in format <X, Y>, with a limited number of decimal places (rounded).
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public string ToString(int decimals)
        {
            string format = "0.";
            for (int i = 0; i < decimals; i++)
            {
                format += "#";
            }
            return $"<{X.ToString(format)}, {Y.ToString(format)}>";
        }

        /// <summary>
        /// Gets the representation of this unit vector in radians, with 0 radians being (1, 0)
        /// </summary>
        public double Radians
        {
            get
            {
                double tmp = Math.Acos(X);
                if (Y < 0) tmp = -tmp;
                return Math.Acos(X);
            }
            set
            {
                X = Math.Cos(value);
                Y = Math.Sin(value);
            }
        }

        /// <summary>
        /// Gets the representation of this unit vector in degrees, with 0 degrees being (1, 0)
        /// </summary>
        public double Degrees
        {
            get
            {
                double tmp = Math.Acos(X) * RAD_CONST;
                if (Y < 0) tmp = -tmp;
                return tmp;
            }
            set
            {
                X = Math.Cos(value / RAD_CONST);
                Y = Math.Sin(value / RAD_CONST);
            }
        }
    }
}
