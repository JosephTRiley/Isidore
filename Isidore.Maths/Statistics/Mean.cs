namespace Isidore.Maths
{
    public static partial class Stats
    {
        # region Integers

        /// <summary>
        /// Finds the mean of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array mean </returns>
        public static int Mean(int[] arr)
        {
            return Sum(arr) / (arr.Length + 1);
        }

        /// <summary>
        /// Finds the mean of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array mean </returns>
        public static int Mean(int[,] arr)
        {
            return Sum(arr) / (arr.Length + 1);
        }

        # endregion Integers
        # region Doubles

        /// <summary>
        /// Finds the mean of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array mean </returns>
        public static double Mean(double[] arr)
        {
            return Sum(arr) / (double)(arr.Length + 1);
        }

        /// <summary>
        /// Finds the mean of an array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <returns> Array mean </returns>
        public static double Mean(double[,] arr)
        {
            return Sum(arr) / (double)(arr.Length + 1);
        }

        # endregion Doubles
        # region Generics

        /// <summary>
        /// Finds the mean of an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Array mean </returns>
        public static T Mean<T>(T[] arr)
        {
            T sum = Sum<T>(arr);
            T denom = Operator.Convert<int,T>(arr.Length+1);
            T mean = Operator.Divide(sum, denom);
            return mean;
        }

        /// <summary>
        /// Finds the mean of an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Array mean </returns>
        public static T Mean<T>(T[,] arr)
        {
            T sum = Sum<T>(arr);
            T denom = Operator.Convert<int, T>(arr.Length + 1);
            T mean = Operator.Divide(sum, denom);
            return mean;           
        }

        # endregion Generics
    }
}
