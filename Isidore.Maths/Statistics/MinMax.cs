using System;

namespace Isidore.Maths
{
    public static partial class Stats
    {
        # region Integers

        /// <summary>
        /// Finds the minimum and maximum values in an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> smallest and largest values in the array </returns>
        public static int[] MinMax(int[] arr)
        {
            // Sets min and max to the first value in the array
            int min, max;
            min = max = arr[0];

            int len = arr.GetLength(0);
            for (int idx = 0; idx < len; idx++)
                if (arr[idx] < min)
                    min = arr[idx];
                else if (arr[idx] > max)
                    max = arr[idx];

            int[] minMax = new int[] { min, max };
            return minMax;
        }

        /// <summary>
        /// Finds the minimum and maximum values in an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> smallest and largest values in the array </returns>
        public static int[] MinMax(int[,] arr)
        {
            // Sets min and max to the first value in the array
            int min, max; min = max = arr[0, 0];


            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            //Parallel.For(0, dim0, i0 =>
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    if (arr[idx0, idx1] < min)
                        min = arr[idx0, idx1];
                    else if (arr[idx0, idx1] > max)
                        max = arr[idx0, idx1];
            //);

            int[] minMax = new int[] { min, max };
            return minMax;
        }

        # endregion Integers
        # region Doubles

        /// <summary>
        /// Finds the minimum and maximum values in an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> smallest and largest values in the array </returns>
        public static double[] MinMax(double[] arr)
        {
            // Sets min and max to the first value in the array
            double min, max; 
            min = max = arr[0];

            int len = arr.GetLength(0);
            for (int idx = 0; idx < len; idx++)
                if (arr[idx] < min)
                    min = arr[idx];
                else if (arr[idx] > max)
                    max = arr[idx];

            double[] minMax = new double[] { min, max };
            return minMax;
        }

        /// <summary>
        /// Finds the minimum and maximum values in an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> smallest and largest values in the array </returns>
        public static double[] MinMax(double[,] arr)
        {
            // Sets min and max to the first value in the array
            double min, max; min = max = arr[0, 0];


            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            //Parallel.For(0, dim0, i0 =>
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    if (arr[idx0, idx1] < min)
                        min = arr[idx0, idx1];
                    else if (arr[idx0, idx1] > max)
                        max = arr[idx0, idx1];
            //);

            double[] minMax = new double[] { min, max };
            return minMax;
        }

        # endregion Doubles
        # region Generics

        /// <summary>
        /// Finds the minimum and maximum values in an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> smallest and largest values in the array </returns>
        public static T[] MinMax<T>(T[] arr)
        {
            Func<T, T, bool> gt = Operator<T>.GreaterThan;
            Func<T, T, bool> lt = Operator<T>.LessThan;

            T min, max; min = max = arr[0];
            int len = arr.GetLength(0);

            for (int idx = 0; idx < len; idx++)
            {
                T pix = arr[idx];
                if (lt(pix, min))
                    min = pix;
                else if (gt(pix, max))
                    max = pix;
            }

            T[] minMax = new T[] { min, max };
            return minMax;
        }

        /// <summary>
        /// Finds the minimum and maximum values in an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> smallest and largest values in the array </returns>
        public static T[] MinMax<T>(T[,] arr)
        {
            Func<T, T, bool> gt = Operator<T>.GreaterThan;
            Func<T, T, bool> lt = Operator<T>.LessThan;

            // Loops through array
            T min, max; min = max = arr[0, 0];
            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            //Parallel.For(0, dim0, i0 =>
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                {
                    T pix = arr[idx0, idx1];
                    if (lt(pix, min))
                        min = pix;
                    else if (gt(pix, max))
                        max = pix;
                }
            //);
            T[] minMax = new T[] { min, max };
            return minMax;
        }

        # endregion Generics
    }
}
