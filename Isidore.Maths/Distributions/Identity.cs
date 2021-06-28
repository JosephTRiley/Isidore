using System;

namespace Isidore.Maths
{
    public static partial class Distribution
    {
        /// <summary>
        /// Produces an identity matrix of size N.
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="N"> Matrix length </param>
        /// <returns> Identity matrix </returns>
        public static T[,] Identity<T>(int N)
        {
            // Uses function (the complicated but fast way)
            Func<double, T> conv = Operator<double, T>.Convert;
            T one = conv(1.0);

            T[,] m = new T[N, N];
            for (int idx = 0; idx < N; idx++)
                m[idx, idx] = one;
            return m;
        }

        /// <summary>
        /// Produces an identity matrix of size N of type double.
        /// </summary>
        /// <param name="N"> Matrix length </param>
        /// <returns> Identity matrix </returns>
        public static double[,] Identity(int N)
        {
            return Identity<double>(N);
        }
    }
}
