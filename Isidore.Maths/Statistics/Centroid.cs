namespace Isidore.Maths
{
    public partial class Stats
    {
        # region 1D

        /// <summary>
        /// Finds the weighted centroid of a 1D array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <param name="thresh"> Pixel level threshold </param>
        /// <returns> Centroid location </returns>
        public static double Centroid(double[] arr, double thresh = 0)
        {

            double total = 0; // total power
            double sum0 = 0; // Positional sum in first dimension

            int dim0 = arr.Length;
            for (int idx0 = 0; idx0 < dim0; idx0++)
                    if(arr[idx0]>thresh)
                    {
                        total += arr[idx0];
                        sum0 += arr[idx0] * (double)idx0;
                    }

            return sum0 / total;
        }

        /// <summary>
        /// Finds the weighted centroid of a 2D array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="thresh"> Pixel level threshold </param>
        /// <returns> Centroid location </returns>
        public static double Centroid<T>(T[] arr, T thresh=default(T))
        {
            // Converts data and passes to double method
            double[] darr = Operator.Convert<T,double>(arr);
            double dThresh = Operator.Convert<T, double>(thresh);
            return Centroid(darr, dThresh);
        }

        # endregion 1D
        # region 2D

        /// <summary>
        /// Finds the weighted centroid of a 2D array
        /// </summary>
        /// <param name="arr"> Array </param>
        /// <param name="thresh"> Pixel level threshold </param>
        /// <returns> Centroid location, 
        /// (First then second dimension) </returns>
        public static double[] Centroid(double[,] arr, double thresh = 0)
        {

            double total = 0; // total power
            double sum0 = 0; // Positional sum in first dimension
            double sum1 = 0; // Positional sum in second dimension

            int dim0 = arr.GetLength(0);
            int dim1 = arr.GetLength(1);
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    if (arr[idx0, idx1] > thresh)
                    {
                        total += arr[idx0, idx1];
                        sum0 += arr[idx0, idx1] * (double)idx0;
                        sum1 += arr[idx0, idx1] * (double)idx1;
                    }

            double[] position = new double[] { sum0 / total, sum1 / total };
            return position;
        }

        /// <summary>
        /// Finds the weighted centroid of a 2D array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="thresh"> Pixel level threshold </param>
        /// <returns> Centroid location, 
        /// (First then second dimension) </returns>
        public static double[] Centroid<T>(T[,] arr, T thresh = default(T))
        {
            // Converts data and passes to double method
            double[,] darr = Operator.Convert<T, double>(arr);
            double dThresh = Operator.Convert<T, double>(thresh);
            return Centroid(darr, dThresh);
        }

        # endregion 2D
    }
}
