using System;

namespace Isidore.Maths
{
    // Provides transformation matrices
    public partial class Transform
    {
        /// <summary>
        /// Returns an 4x4 identity matrix transform
        /// </summary>
        /// <returns> 4x4 identity transform </returns>
        public static Transform Identity()
        {
            return new Transform();
        }

        # region Translation

        /// <summary>
        /// Creates a translation transform from a shift vector of any size.
        /// </summary>
        /// <param name="shift"> Shifting offset vector </param>
        /// <returns> Translation transform </returns>
        public static Transform Translate(double[] shift)
        {
            int len = shift.Length;
            double[,] m = Distribution.Identity(len + 1);
            double[,] im = Distribution.Identity(len + 1);
            // Adds translation to the matrix
            for (int idx = 0; idx < len; idx++)
            {
                m[idx, len] = shift[idx];
                im[idx, len] = -shift[idx];
            }

            return new Transform(m, im);
        }

        /// <summary>
        /// Creates a 4x4 translation transform from three scalar components.
        /// </summary>
        /// <param name="x"> X axis translation component </param>
        /// <param name="y"> Y axis translation component </param>
        /// <param name="z"> Z axis translation component </param>
        /// <returns> Translation Transform </returns>
        public static Transform Translate(double x, double y, double z)
        {
            return Translate(new double[] { x, y, z });
        }

        /// <summary>
        /// Translation transformation using a single point
        /// </summary>
        /// <param name="moveDist"> Point representing the distance to move </param>
        /// <returns> Translation Transform </returns>
        public static Transform Translate(Point moveDist)
        {
            return Translate(moveDist.Comp);
        }

        /// <summary>
        /// Translation transform using the difference of two points
        /// </summary>
        /// <param name="fromPt"> Point of origin </param>
        /// <param name="toPt"> Point of destination </param>
        /// <returns> Translation Transform </returns>
        public static Transform Translate(Point fromPt, Point toPt)
        {
            Point movePt = toPt - fromPt;
            return Translate(movePt);
        }

        # endregion Translation

        # region Scale

        /// <summary>
        /// Creates a scaling transformation from an scaling vector of any size
        /// </summary>
        /// <param name="scale"> Scaling vector </param>
        /// <returns> Scaling transform </returns>
        public static Transform Scale(double[] scale)
        {
            int len = scale.Length;
            double[,] m = Distribution.Identity(len + 1);
            double[,] im = Distribution.Identity(len + 1);
            // Adds scaling to the matrix
            for (int idx = 0; idx < len; idx++)
            {
                m[idx, idx] = scale[idx];
                im[idx, idx] = 1.0/scale[idx];
            }

            return new Transform(m, im);
        }

        /// <summary>
        /// Creates a scaling transformation from three values
        /// </summary>
        /// <param name="x"> X-axis scale </param>
        /// <param name="y"> Y-axis scale </param>
        /// <param name="z"> Z-axis scale</param>
        /// <returns> Scaling transform </returns>
        public static Transform Scale(double x, double y, double z)
        {
            return Scale(new double[] { x, y, z });
        }

        # endregion Scale

        # region Rotation

        /// <summary>
        /// X-axis rotational transformation
        /// </summary>
        /// <param name="Radian"> Radians of rotation </param>
        /// <returns> X-axis rotation transform </returns>
        public static Transform RotX(double Radian)
        {
            double cosA = Math.Cos(Radian);
            double sinA = Math.Sin(Radian);
            double[,] d = new double[,]{
                {1,    0,     0, 0},
                {0, cosA, -sinA, 0},
                {0, sinA,  cosA, 0},
                {0,    0,     0, 1}};

            Transform t = new Transform(d, Arr.Transpose(d));
            return t;
        }

        /// <summary>
        /// Y-axis rotational transformation
        /// </summary>
        /// <param name="Radian"> Radians of rotation </param>
        /// <returns> Y-axis rotation transform </returns>
        public static Transform RotY(double Radian)
        {
            double cosA = Math.Cos(Radian);
            double sinA = Math.Sin(Radian);
            double[,] d = new double[,]{
                { cosA, 0, sinA, 0},
                {    0, 1,    0, 0},
                {-sinA, 0, cosA, 0},
                {    0, 0,    0, 1}};

            Transform t = new Transform(d, Arr.Transpose(d));
            return t;
        }

        /// <summary>
        /// Z-axis rotational transformation
        /// </summary>
        /// <param name="Radian"> Radians of rotation </param>
        /// <returns> Z-axis rotation transform </returns>
        public static Transform RotZ(double Radian)
        {
            double cosA = Math.Cos(Radian);
            double sinA = Math.Sin(Radian);
            double[,] d = new double[,]{
                {cosA, -sinA, 0, 0},
                {sinA,  cosA, 0, 0},
                {   0,     0, 1, 0},
                {   0,     0, 0, 1}};

            Transform t = new Transform(d, Arr.Transpose(d));
            return t;
        }

        # endregion Rotation
    }
}
