namespace Isidore.Maths
{
    public static partial class Arr
    {
        /// <summary>
        /// Returns a subset array of a larger array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Full array </param>
        /// <param name="corner0"> Window corner in the first 
        /// dimension </param>
        /// <param name="corner1"> Window corner in the second
        /// dimension </param>
        /// <param name="length0"> Window length in the first 
        /// dimension </param>
        /// <param name="length1"> Window length in the second 
        /// dimension </param>
        /// <returns> Array subset within window </returns>
        public static T[,] Window<T>(T[,] arr, int corner0, int corner1,
        int length0, int length1)
        {
            T[,] win = new T[length0, length1];
            for (int i0 = 0; i0 < length0; i0++)
                for (int i1 = 0; i1 < length1; i1++)
                    win[i0, i1] = arr[i0 + corner0, i1 + corner1];
            return win;
        }
    }
}
