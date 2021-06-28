using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Provides methods for generating various distributions
    /// </summary>
    public static partial class Distribution
    {
        # region 1 dimension array
        /// <summary>
        /// Generates a Gaussian distribution
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="peakVal"> Distribution value at its peak </param>
        /// <param name="sigma0"> Distribution width </param>
        /// <param name="Length0"> Array length </param>
        /// <returns> Gaussian distribution </returns>
        public static T[] Gaussian<T>(T peakVal, double sigma0, int Length0)
        {
            double peakValD = Convert.ToDouble(peakVal);
            double[] gDist = Gaussian(peakValD, sigma0, Length0);
            return Operator.Convert<double, T>(gDist);
        }

        /// <summary>
        /// Generates a Gaussian distribution
        /// </summary>
        /// <param name="peakVal"> Distribution value at its peak </param>
        /// <param name="sigma0"> Distribution width </param>
        /// <param name="Length0"> Array length </param>
        /// <returns> Gaussian distribution </returns>
        public static double[] Gaussian(double peakVal, double sigma0, 
            int Length0)
        {
            if (Length0 < 1)
            {
                Exception err = new Exception(
                    "Gaussian data array must be larger than one");
                throw err;
            }
            double[] Dist = new double[Length0];

            // Scaling values
            double denom0 = -0.5 / (sigma0 * sigma0);

            // Distribution
            double centPt0 = 0.5 * (double)(Length0 - 1);
            for (int i0 = 0; i0 < Length0; i0++)
            {
                double dist0 = Math.Abs((double)i0 - centPt0);
                Dist[i0] = Math.Exp(denom0 * (dist0 * dist0)) * peakVal;
            }

            return Dist;
        }

        # endregion 1 dimension array
        # region 2 dimension array

        /// <summary>
        /// Generates a Gaussian distribution
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="peakVal"> Distribution value at its peak </param>
        /// <param name="sigma0">  Distribution width in 
        /// dimension 0 </param>
        /// <param name="sigma1"> Distribution width in 
        /// dimension 1 </param>
        /// <param name="Length0"> Array length in dimension 0 </param>
        /// <param name="Length1"> Array length in dimension 1 </param>
        /// <returns> Gaussian distribution </returns>
        public static T[,] Gaussian<T>(T peakVal, double sigma0, 
            double sigma1, int Length0, int Length1)
        {
            double peakValD = Convert.ToDouble(peakVal);
            double[,] gDist = Gaussian(peakValD, sigma0, sigma1, 
                Length0, Length1);
            return Operator.Convert<double, T>(gDist);
        }

        /// <summary>
        /// Generates a Gaussian distribution
        /// </summary>
        /// <param name="peakVal"> Distribution value at its peak </param>
        /// <param name="sigma0">  Distribution width in 
        /// dimension 0 </param>
        /// <param name="sigma1"> Distribution width in 
        /// dimension 1 </param>
        /// <param name="Length0"> Array length in dimension 0 </param>
        /// <param name="Length1"> Array length in dimension 1 </param>
        /// <returns> Gaussian distribution </returns>
        public static double[,] Gaussian(double peakVal, double sigma0, 
            double sigma1, int Length0, int Length1)
        {

            if (Length0 < 1 || Length1 < 1)
            {
                Exception err = new Exception("Gaussian data array " + 
                    "must have all dimensions larger than one");
                throw err;
            }
            double[,] Dist = new double[Length0, Length1];

            // Scaling values
            double denom0 = -0.5 / (sigma0 * sigma0);
            double denom1 = -0.5 / (sigma1 * sigma1);

            // Distribution
            double centPt0 = 0.5 * (double)(Length0 - 1);
            double centPt1 = 0.5 * (double)(Length1 - 1);
            for (int i0 = 0; i0 < Length0; i0++)
            {
                double dist0 = Math.Abs((double)i0 - centPt0);
                for (int i1 = 0; i1 < Length1; i1++)
                {
                    double dist1 = Math.Abs((double)i1 - centPt1);
                    Dist[i0, i1] = Math.Exp(denom0 * (dist0 * dist0) +
                        denom1 * (dist1 * dist1)) * peakVal;
                }
            }
            return Dist;
        }

        # endregion 2 dimension array
    }
}
