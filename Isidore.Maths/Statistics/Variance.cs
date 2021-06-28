using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Statistics function
    /// </summary>
    public static partial class Stats
    {
        
        /// <summary>
        /// Variance of a summation of data points
        /// </summary>
        /// <param name="sum"> summation of all data points </param>
        /// <param name="sumSq"> summation of the squares of all data 
        /// points </param>
        /// <param name="sampleSize"> total number of data points </param>
        /// <returns> Variance and mean of the data set </returns>
        public static double[] Variance(double sum, double sumSq, 
            int sampleSize)
        {
            double N = (double)sampleSize;
            // Mean and quadratic term
            double mean = sum / N;
            double var = sumSq - 2.0 * mean * sum + N * mean * mean;
            var /= N + 1;
            return new double[] { var, mean };
        }

        /// <summary>
        /// Variance of a summation of data points
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="sum"> summation of all data points </param>
        /// <param name="sumSq"> summation of the squares of all data 
        /// points </param>
        /// <param name="sampleSize"> total number of data points </param>
        /// <returns> Variance and mean of the data set </returns>
        public static double[] Variance<T>(T sum, T sumSq, int sampleSize)
        {
            Func<T, double> convert = Operator<T, double>.Convert;
            // Converts data to double
            double dsum = convert(sum);
            double dsumSq = convert(sumSq);
            return Variance(dsum, dsumSq, sampleSize);
        }

        /// <summary>
        /// Variance of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Variance and mean of the data set </returns>
        public static double[] Variance(double[] arr)
        {
            double sum=0, sumSq=0; // Sum and sum of squares
            // Steps through arrays
            int len = arr.Length;
            for (int idx = 0; idx < len; idx++)
            {
                sum += arr[idx];
                sumSq += arr[idx] * arr[idx];
            }

            return Variance(sum, sumSq, arr.Length);
        }

        /// <summary>
        /// Variance of an array
        /// </summary>
        /// <typeparam name="T"> Input data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Variance and mean of the data set </returns>
        public static double[] Variance<T>(T[] arr)
        {
            // Converts array to double type
            double[] darr = Operator.Convert<T, double>(arr);
            // Passes to Variance
            return Variance(darr);
        }

        /// <summary>
        /// Variance of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Variance and mean of the data set </returns>
        public static double[] Variance(double[,] arr)
        {
            double sum = 0, sumSq = 0; // Sum and sum of squares

            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                {
                    sum += arr[idx0, idx1];
                    sumSq += arr[idx0, idx1] * arr[idx0, idx1];
                }

            double[] varStats = Variance(sum, sumSq, arr.Length);

            return Variance(sum, sumSq, arr.Length);
        }

        /// <summary>
        /// Variance of an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Variance and mean of the data set </returns>
        public static double[] Variance<T>(T[,] arr)
        {
            // Converts array to double
            double[,] darr = Operator.Convert<T, double>(arr);
            // Passes to variance
            return Variance(darr);
        }

        /// <summary>
        /// Finds the variance of a data array marked with a tag array
        /// </summary>
        /// <param name="arr"> Data array </param>
        /// <param name="tag"> Tag array </param>
        /// <returns> Variance of data marked with tag </returns>
        public static double[] Variance(double[,] arr, bool[,] tag)
        {
            // Checks that arr and tag are the same size
            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            if (dim0 != tag.GetLength(0) || dim1 != tag.GetLength(1))
                throw new System.ArgumentException(
                    "Data and tag arrays must be the same size", "arr");

            // Data needed for variance
            double sum = 0, sumSq = 0;
            int count = 0;
            // Loops through array
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    if (tag[idx0, idx1]) // If tags, add to sample
                    {
                        sum += arr[idx0, idx1];
                        sum += arr[idx0, idx1] * arr[idx0, idx1];
                        count++;
                    }
            // Calls Variance
            return Variance(sum, sumSq, count);
        }

        /// <summary>
        /// Finds the variance of a data array marked with a tag array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Data array </param>
        /// <param name="tag"> Tag array </param>
        /// <returns> Variance of data marked with tag </returns>
        public static double[] Variance<T>(T[,] arr, bool[,] tag)
        {
            // Converts to double and passes to Variance
            double[,] darr = Operator.Convert<T, double>(arr);
            return Variance(darr);
        }

        /// <summary>
        /// Finds the variance of a data array marked with a tag array
        /// </summary>
        /// <param name="arr"> Data array </param>
        /// <param name="tag"> Tag array </param>
        /// <returns> Variance of data marked with tag </returns>
        public static double[] Variance(double[] arr, bool[] tag)
        {
            // Checks that arr and tag are the same size
            int dim0 = arr.Length;
            if (dim0 != tag.Length)
                throw new System.ArgumentException(
                    "Data and tag arrays must be the same size", "arr");

            // Data needed for variance
            double sum = 0, sumSq = 0;
            int count = 0;
            // Loops through array
            for (int idx0 = 0; idx0 < dim0; idx0++)
                    if (tag[idx0]) // If tags, add to sample
                    {
                        sum += arr[idx0];
                        sum += arr[idx0] * arr[idx0];
                        count++;
                    }
            // Calls Variance
            return Variance(sum, sumSq, count);
        }

        /// <summary>
        /// Finds the variance of a data array marked with a tag array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Data array </param>
        /// <param name="tag"> Tag array </param>
        /// <returns> Variance of data marked with tag </returns>
        public static double[] Variance<T>(T[] arr, bool[,] tag)
        {
            // Converts to double and passes to Variance
            double[] darr = Operator.Convert<T, double>(arr);
            return Variance(darr);
        }
    }
}
