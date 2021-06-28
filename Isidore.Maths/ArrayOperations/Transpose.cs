namespace Isidore.Maths
{
    public static partial class Arr
    {
        /// <summary>
        /// Transposes a matrix
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="m"> Matrix </param>
        /// <returns> Matrix transpose </returns>
        public static T[,] Transpose<T>(T[,] m)
        {
            int len0 = m.GetLength(0);
            int len1 = m.GetLength(1);
            T[,] retm = new T[len1, len0];
            for (int idx0 = 0; idx0 < len1; idx0++)
                for (int idx1 = 0; idx1 < len0; idx1++)
                    retm[idx0, idx1] = m[idx1, idx0];
            return retm;
        }

        /// <summary>
        /// Transposes a matrix
        /// </summary>
        /// <param name="m"> Matrix </param>
        /// <returns> Matrix transpose </returns>
        public static int[,] Transpose(int[,] m)
        {
            int len0 = m.GetLength(0);
            int len1 = m.GetLength(1);
            int[,] retm = new int[len1, len0];
            for (int idx0 = 0; idx0 < len1; idx0++)
                for (int idx1 = 0; idx1 < len0; idx1++)
                    retm[idx0, idx1] = m[idx1, idx0];
            return retm;
        }

        /// <summary>
        /// Transposes a matrix
        /// </summary>
        /// <param name="m"> Matrix </param>
        /// <returns> Matrix transpose </returns>
        public static double[,] Transpose(double[,] m)
        {
            int len0 = m.GetLength(0);
            int len1 = m.GetLength(1);
            double[,] retm = new double[len1, len0];
            for (int idx0 = 0; idx0 < len1; idx0++)
                for (int idx1 = 0; idx1 < len0; idx1++)
                    retm[idx0, idx1] = m[idx1, idx0];
            return retm;
        }
    }
}
