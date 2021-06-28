using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Quaternion data structure. Used for interpolating transformations
    /// </summary>
    public struct Quaternion
    {
        # region Fields & Properties

        /// <summary>
        /// Vector First three scalars of a quaternion
        /// </summary>
        public Vector v;
        /// <summary>
        /// Final quaternion scalar
        /// </summary>
        public double w;

        # endregion Fields & Properties

        # region Constructors

        /// <summary>
        /// V = 0, W = 0
        /// </summary>
        public static Quaternion Zero;
        /// <summary>
        /// V = NaN, W = NaN
        /// </summary>
        public static Quaternion NaN;

        static Quaternion()
        {
            Zero = new Quaternion(Vector.Zero(), 0.0);
            NaN = new Quaternion(Vector.NaN(), Double.NaN);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="V"> vector portion </param>
        /// <param name="S"> scalar portion (w) </param>
        public Quaternion(Vector V, double S)
        {
            v = V.Clone();
            w = S;
        }

        /// <summary>
        /// Constructor using a transformation matrix (4x4)
        /// </summary> 
        /// <param name="m"> 4x4 transform matrix </param>
        public Quaternion(double[,] m)
        {
            double tr = m[0, 0] + m[1, 1] + m[2, 2]; // matrix trace
            // Broken down by each quadrant.  Much easier to see this way
            double s, qw, qx, qy, qz;
            if (tr > 0)
            {
                s = Math.Sqrt(tr + 1.0) * 2.0;
                qw = 0.25 * s;
                qx = (m[2, 1] - m[1, 2]) / s;
                qy = (m[0, 2] - m[2, 0]) / s;
                qz = (m[1, 0] - m[0, 1]) / s;
            }
            else if (m[0, 0] > m[1, 1] && (m[0, 0] > m[2, 2]))
            {
                s = Math.Sqrt(m[0, 0] - m[1, 1] - m[2, 2] + 1.0) * 2.0;
                qw = (m[2, 1] - m[1, 2]) / s;
                qx = 0.25 * s;
                qy = (m[0, 1] + m[1, 0]) / s;
                qz = (m[0, 2] + m[2, 0]) / s;
            }
            else if (m[1, 1] > m[2, 2])
            {
                s = Math.Sqrt(-m[0, 0] + m[1, 1] - m[2, 2] + 1.0) * 2.0;
                qw = (m[0, 2] - m[2, 0]) / s;
                qx = (m[0, 1] + m[1, 0]) / s;
                qy = 0.25 * s;
                qz = (m[1, 2] + m[2, 1]) / s;
            }
            else
            {
                s = Math.Sqrt(-m[0, 0] - m[1, 1] + m[2, 2] + 1.0) * 2.0;
                qw = (m[1, 0] - m[0, 1]) / s;
                qx = (m[0, 2] + m[2, 0]) / s;
                qy = (m[1, 2] + m[2, 1]) / s;
                qz = 0.25 * s;
            }
            w = qw;

            v = new Vector(new double[] { qx, qy, qz });
        }

        /// <summary>
        /// Constructor using a transformation class
        /// </summary>
        /// <param name="t"> transformation </param>
        public Quaternion(Transform t) : this(t.M) { }

        # endregion Constructors

        # region Operators

        /// <summary>
        /// Adds two quaternions
        /// </summary>
        /// <param name="q0"> First quaternion </param>
        /// <param name="q1"> Second quaternion </param>
        /// <returns> quaternions' sum </returns>
        static public Quaternion operator +(Quaternion q0, Quaternion q1)
        {
            Quaternion q;
            q.v = q0.v + q1.v;
            q.w = q0.w + q1.w;
            return q;
        }

        /// <summary>
        /// Subtracts two quaternions
        /// </summary>
        /// <param name="q0"> First quaternion </param>
        /// <param name="q1"> Second quaternion </param>
        /// <returns> quaternions' difference </returns>
        static public Quaternion operator -(Quaternion q0, Quaternion q1)
        {
            Quaternion q;
            q.v = q0.v - q1.v;
            q.w = q0.w - q1.w;
            return q;
        }

        /// <summary>
        /// Multiplies a quaternion by a scaling factor
        /// </summary>
        /// <param name="q0"> First quaternion </param>
        /// <param name="factor"> Scalar factor </param>
        /// <returns> Scaled quaternion </returns>
        static public Quaternion operator *(Quaternion q0, double factor)
        {
            Quaternion q;
            q.v = q0.v * factor;
            q.w = q0.w * factor;
            return q;
        }

        /// <summary>
        /// Divides two quaternions quaternion by a scaling factor
        /// </summary>
        /// <param name="q0"> First quaternion </param>
        /// <param name="factor"> Scalar factor </param>
        /// <returns> Scaled quaternion </returns>
        static public Quaternion operator /(Quaternion q0, double factor)
        {
            Quaternion q;
            q.v = q0.v /factor;
            q.w = q0.w / factor;
            return q;
        }

        # endregion Operators

        # region Methods

        /// <summary>
        /// Dot product of between this and another quaternion
        /// </summary>
        /// <param name="q1"> The second quaternion </param>
        /// <returns> The dot product of the two quaternion </returns>
        public double Dot(Quaternion q1)
        {
            return Dot(this, q1);
        }

        /// <summary>
        /// Normalizes the Quaternion
        /// </summary>
        public void Normalize()
        {
            this = this/Math.Sqrt(Dot(this, this));
        }


        /// <summary>
        /// Dot product of two quaternion
        /// </summary>
        /// <param name="q0"> First quaternion </param>
        /// <param name="q1"> Second quaternion </param>
        /// <returns> Dot product </returns>
        static public double Dot(Quaternion q0, Quaternion q1)
        {
            return Vector.Dot(q0.v, q1.v) + q0.w * q1.w;
        }

        # endregion Methods
    }
}
