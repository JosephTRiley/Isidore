using System;

namespace Isidore.Maths
{
    public static partial class Stats
    {
        # region Integers

        /// <summary>
        /// Finds the total sum of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array sum </returns>
        public static int Sum(int[] arr)
        {
            int sum = 0;

            int len0 = arr.Length;
            for (int idx0 = 0; idx0 < len0; idx0++)
                    sum += arr[idx0];
            return sum;
        }

        /// <summary>
        /// Finds the total sum of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array sum </returns>
        public static int Sum(int[,] arr)
        {
            int sum = 0;

            int len0 = arr.GetLength(0);
            int len1 = arr.GetLength(1);
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                    sum += arr[idx0, idx1];
            return sum;
        }

        # endregion Integers
        # region Doubles

        /// <summary>
        /// Finds the total sum of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array sum </returns>
        public static double Sum(double[] arr)
        {
            double sum = 0;

            int len0 = arr.Length;
            for (int idx0 = 0; idx0 < len0; idx0++)
                sum += arr[idx0];
            return sum;
        }

        /// <summary>
        /// Finds the total sum of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array sum </returns>
        public static double Sum(double[,] arr)
        {
            double sum = 0;

            int len0 = arr.GetLength(0);
            int len1 = arr.GetLength(1);
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                    sum += arr[idx0, idx1];
            return sum;
        }

        # endregion Doubles
        # region Generics

        /// <summary>
        /// Finds the total sum of an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Array sum </returns>
        public static T Sum<T>(T[] arr)
        {
            // Add operator
            Func<T,T,T> add = Operator<T>.Add;

            T sum = default(T);

            int len = arr.Length;
            for (int idx0 = 0; idx0 < len; idx0++)
                sum = add(sum, arr[idx0]);

            return sum;
        }

        /// <summary>
        /// Finds the total sum of an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Array sum </returns>
        public static T Sum<T>(T[,] arr)
        {
            Func<T, T, T> add = Operator<T, T>.Add;
            //Func<T,Tout> add = Operator<T,Tout>.Add;

            T sum = default(T);

            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            for (int i0 = 0; i0 < dim0; i0++)
                for (int i1 = 0; i1 < dim1; i1++)
                    sum = add(sum, arr[i0, i1]);

            return sum;
        }

        # endregion Generics
    }
}
