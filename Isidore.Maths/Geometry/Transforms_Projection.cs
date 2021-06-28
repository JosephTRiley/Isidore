using System;

namespace Isidore.Maths
{
    public partial class Transform
    {
        /// <summary>
        /// Generates a orthographic transform
        /// </summary>
        /// <param name="leftLim"> Left transform limit (-1 after transform) </param>
        /// <param name="rightLim"> Right transform limit (1 after transform)</param>
        /// <param name="bottomLim"> Bottom transform limit (-1 after transform) </param>
        /// <param name="topLim"> Top transform limit (1 after transform) </param>
        /// <param name="nearLim"> Near transform limit (0 after transform) </param>
        /// <param name="farLim">  Far transform limit (1 after transform) </param>
        /// <returns> Orthographic transform </returns>
        public static Transform Orthographic(double leftLim, double rightLim,
            double bottomLim, double topLim, double nearLim = 0.0, 
            double farLim = double.PositiveInfinity)
        {
            double[,] m = Distribution.Identity(4);
            if (double.IsPositiveInfinity(farLim))
            {
                m[0, 0] = 2 / (rightLim - leftLim);
                m[1, 1] = 2 / (topLim - bottomLim);
                m[0, 3] = -(rightLim + leftLim) / (rightLim - leftLim);
                m[1, 3] = -(topLim + bottomLim) / (topLim - bottomLim);
                m[2, 3] = -nearLim;
            }
            else
            {
                m[0, 0] = 2 / (rightLim - leftLim);
                m[1, 1] = 2 / (topLim - bottomLim);
                m[2, 2] = 1 / (farLim - nearLim);
                m[0, 3] = -(rightLim + leftLim) / (rightLim - leftLim);
                m[1, 3] = -(topLim + bottomLim) / (topLim - bottomLim);
                m[2, 3] = -nearLim / (farLim - nearLim);
            }

            // This is a non-invertible transform so allow constructor to do inverse
            Transform t = new Transform(m);
            return t;
        }

        /// <summary>
        /// Generates an orthographic transform in only the Z-axis
        /// </summary>
        /// <param name="nearLim"> Near plane location down the Z-axis </param>
        /// <param name="farLim"> Far plane location down the Z-axis </param>
        /// <returns> Orthographic transform </returns>
        public static Transform Orthographic(double nearLim, double farLim)
        {
            return Orthographic(-1.0, 1.0, -1.0, 1.0, nearLim, farLim);
        }

        /// <summary>
        /// Provides a perspective transformation.  Assume planar bounding points 
        /// are located on the near limit plane.
        /// </summary>
        /// <param name="leftLim"> Lower corner left position </param>
        /// <param name="rightLim"> Upper corner right position </param>
        /// <param name="bottomLim"> Lower corner bottom position </param>
        /// <param name="topLim"> Upper corner top position </param>
        /// <param name="nearLim"> Near limit plane </param>
        /// <param name="farLim"> Far limit plane </param>
        /// <returns> Perspective transformation </returns>
        public static Transform Perspective(double leftLim, double rightLim,
            double bottomLim, double topLim, double nearLim = 1.0,
            double farLim = double.PositiveInfinity)
        {
            //This is from real-time rendering
            //Matrix4x4 m = new Matrix4x4();
            double[,] m = Distribution.Identity(4);
            if (double.IsPositiveInfinity(farLim))
            {
                m[0, 0] = 2 * nearLim / (rightLim - leftLim);
                m[1, 1] = 2 * nearLim / (topLim - bottomLim);
                m[2, 2] = 1.0;
                m[0, 2] = -(rightLim + leftLim) / (rightLim - leftLim);
                m[1, 2] = -(topLim + bottomLim) / (topLim - bottomLim);
                m[3, 2] = 1.0;
                m[2, 3] = -nearLim;
                m[3, 3] = 0.0;
            }
            else
            {
                m[0, 0] = 2 * nearLim / (rightLim - leftLim);
                m[1, 1] = 2 * nearLim / (topLim - bottomLim);
                m[2, 2] = farLim / (farLim - nearLim);
                m[0, 2] = -(rightLim + leftLim) / (rightLim - leftLim);
                m[1, 2] = -(topLim + bottomLim) / (topLim - bottomLim);
                m[3, 2] = 1.0;
                m[2, 3] = -farLim * nearLim / (farLim - nearLim);
                m[3, 3] = 0.0;
            }

            // This is a non-invertible transform so allow constructor to do inverse
            Transform t = new Transform(m);
            return t;
        }

        /// <summary>
        /// Provides a perspective transformation using an image field of view.
        /// </summary>
        /// <param name="FOV"> Field of view [radians] </param>
        /// <param name="nearLim"> Near limit plane </param>
        /// <param name="farLim"> Far limit plane </param>
        /// <returns> Perspective transformation </returns>
        public static Transform Perspective(double FOV, double nearLim, double farLim)
        {
            // This is from PBRT
            // Foreshortening 
            double[,] m = Distribution.Identity(4);
            if (double.IsPositiveInfinity(farLim))
            {
                m[3, 2] = 1.0;
                m[2, 3] = -nearLim;
                m[3, 3] = 0.0;
            }
            else
            {
                m[2, 2] = farLim/(farLim-nearLim);
                m[3, 2] = 1.0;
                m[2, 3] = -farLim*nearLim/(farLim-nearLim);
                m[3, 3] = 0.0;
            }

            //Scaling
            double iTan = 1.0 / Math.Tan(0.5 * FOV);
            Transform Scale = Transform.Scale(iTan, iTan, 1.0);

            // Product
            m = Maths.Arr.MatrixMultiply(Scale.m, m);
            double[,] im = Arr.Inverse(m);

            // This is a non-invertible transform so allow constructor to do inverse
            Transform t = new Transform(m);
            return t;
        }

        /// <summary>
        /// Performs a perspective projection in N dimensional space along a given axis.
        /// Coordinates along this axis will always be 1.  To get the angular projection,
        /// take the arctangent of the transverse components.
        /// </summary>
        /// <param name="N"> Number of dimensions to space </param>
        /// <param name="axis"> Axis to use for perspective </param>
        /// <returns> Perspective projection transform </returns>
        public static Transform Perspective(int N, int axis)
        {
            // My solution, based on PBRT's derivation
            double[,] m = Distribution.Identity(N+1);
            m[N, N] = 0;
            m[N, axis] = 1;
            m[axis, N] = -1;

            Transform t = new Transform(m);
            return t;
        }

        /// <summary>
        /// Performs a perspective projection in 3 dimensional space along a given axis. 
        /// Coordinates along this axis will always be 1.  To get the angular projection,
        /// take the arctangent of the transverse components.
        /// </summary>
        /// <param name="axis"> Axis to use for perspective </param>
        /// <returns> 4x4 Perspective projection transform </returns>
        public static Transform Perspective(int axis = 2)
        {
            return Perspective(3, axis);
        }

    }
}
