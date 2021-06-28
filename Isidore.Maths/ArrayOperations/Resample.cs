using System;

namespace Isidore.Maths
{
    public static partial class Arr
    {
        /// <summary>
        /// Produces an array sized to the scale factor by projecting 
        /// array area.  Value is conserved while value density is not.
        /// Output is a double array.
        /// </summary>
        /// <typeparam name="T"> Data Type </typeparam>
        /// <param name="arr"> Source array </param>
        /// <param name="factor"> Resampling factor </param>
        /// <returns> Resampled array </returns>
        public static double[] Resample<T>(T[] arr, double factor)
        {
            double accuracy = 1e-8; // relative accuracy confidence level

            double[] sarr = Operator.Convert<T, double>(arr);
            if (factor == 1)
                return sarr;

            // Scaling data
            // source length
            int sLen0 = arr.GetLength(0);
            // destination length
            int dLen0 = (int)Math.Truncate(sLen0 / factor);

            // Destination array
            double[] darr = new double[dLen0];

            // Source pixel boundary projection
            double dsLen0 = (double)sLen0 / factor;
            double offset0 = 0.5 * ((double)dLen0 - dsLen0);
            double[] sBound0 = Distribution.Increment(offset0, 
                offset0 + dsLen0 + accuracy, 1 / factor);

            // Rounds to avoid numeric precession errors
            for (int idx = 0; idx < sLen0; idx++)
                sBound0[idx] = Math.Round(sBound0[idx]/accuracy)*accuracy;

            // This is probably the fastest way to average
            // without running up against numeric errors
            int sIdx0 = 0; // Source index
            double overlap0; // area overlap
            for (int dIdx0 = 0; dIdx0 < dLen0; dIdx0++)
            {
                double ddIdx0 = (double)(dIdx0);

                // Forwards to closest pixel
                while (sBound0[sIdx0 + 1] <= ddIdx0)
                    sIdx0++;

                // Any overlapping
                while (sBound0[sIdx0] < ddIdx0 + 1)
                {
                    overlap0 = AreaOverlap(sBound0[sIdx0], ddIdx0,
                            sBound0[sIdx0 + 1], ddIdx0 + 1);
                    darr[dIdx0] += sarr[sIdx0] * overlap0;
                    sIdx0++;
                }
                sIdx0--; // Prevents overflows
            }
            return darr;
        }

        /// <summary>
        /// Produces an array sized to the scale factor by projecting 
        /// array area.  Value is conserved while value density is not.  
        /// Output is a double array.
        /// </summary>
        /// <typeparam name="T"> Data Type </typeparam>
        /// <param name="arr"> Source array </param>
        /// <param name="factor"> Resampling factor </param>
        /// <returns> Resampled array </returns>
        public static double[,] Resample<T>(T[,] arr, double factor)
        {
            double accuracy = 1e-8; // relative accuracy confidence level

            double[,] sarr = Operator.Convert<T, double>(arr);
            if (factor == 1)
                return sarr;

            // Scaling data
            // source length
            int sLen0 = arr.GetLength(0); 
            int sLen1 = arr.GetLength(1);
            // destination length
            int dLen0 = (int)Math.Truncate(sLen0 / factor); 
            int dLen1 = (int)Math.Truncate(sLen1 / factor); 

            // Destination array
            double[,] darr = new double[dLen0, dLen1];

            // Source pixel boundary projection
            double dsLen0 = (double)sLen0 / factor;
            double dsLen1 = (double)sLen1 / factor;
            double offset0 = 0.5 * ((double)dLen0 - dsLen0);
            double offset1 = 0.5 * ((double)dLen1 - dsLen1);
            double[] sBound0 = Distribution.Increment(offset0,
                offset0 + dsLen0 + accuracy, 1 / factor);
            double[] sBound1 = Distribution.Increment(offset1,
                offset1 + dsLen1 + accuracy, 1 / factor);

            // Rounds to avoid numeric precession errors
            for (int idx = 0; idx < sLen0; idx++)
                sBound0[idx] = Math.Round(sBound0[idx] / accuracy) * 
                    accuracy;
            for (int idx = 0; idx < sLen1; idx++)
                sBound1[idx] = Math.Round(sBound1[idx] / accuracy) * 
                    accuracy;

            // Resampling loops
            int sIdx0 = 0; // Source index
            double overlap0, overlap1; // area overlap
            //First dimension
            for (int dIdx0 = 0; dIdx0 < dLen0; dIdx0++)
            {
                double ddIdx0 = (double)(dIdx0);
                while (sBound0[sIdx0 + 1] <= ddIdx0)
                    sIdx0++;

                while (sBound0[sIdx0] < ddIdx0 + 1)
                {
                    overlap0 = AreaOverlap(sBound0[sIdx0], ddIdx0,
                            sBound0[sIdx0 + 1], ddIdx0 + 1);

                    //Second dimension
                    int sIdx1 = 0; // Source index
                    for (int dIdx1 = 0; dIdx1 < dLen1; dIdx1++)
                    {
                        double ddIdx1 = (double)(dIdx1);
                        while (sBound1[sIdx1 + 1] <= ddIdx1)
                            sIdx1++;

                        // Any overlapping
                        while (sBound1[sIdx1] < ddIdx1 + 1)
                        {
                            overlap1 = AreaOverlap(sBound1[sIdx1], ddIdx1,
                                    sBound1[sIdx1 + 1], ddIdx1 + 1);
                            darr[dIdx0, dIdx1] += sarr[sIdx0, sIdx1] * 
                                overlap0 * overlap1;
                            sIdx1++;
                        }
                        sIdx1--; // Prevents boarder skips
                    }
                    sIdx0++;
                }
                sIdx0--; // Prevents boarder skips
            }
            return darr;
        }

        private static double AreaOverlap(double min0, double min1, 
            double max0, double max1)
        {
            double overlap = Math.Min(max0, max1);
            overlap -= Math.Max(min0, min1);
            return overlap;
        }
    }
}
