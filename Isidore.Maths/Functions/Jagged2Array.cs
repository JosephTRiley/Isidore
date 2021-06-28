namespace Isidore.Maths
{
    /// <summary>
    /// General programming function
    /// </summary>
    public partial class Function
    {
        /// <summary>
        /// Converts a jagged array to a 2D array using filler values for 
        /// any short array
        /// </summary>
        /// <typeparam name="T"> data type </typeparam>
        /// <param name="jag"> jagged array </param>
        /// <param name="filler"> Value to use as filler </param>
        /// <returns> 2D array </returns>
        public static T[,] Jagged2Array<T>(T[][] jag, 
            T filler = default(T))
        {
            int len = jag.Length;
            int wide = 0;

            // Checks for longest width
            for (int idx = 0; idx < len; idx++)
                wide = (jag[idx].Length > wide) ? jag[idx].Length : wide;

            // New array
            T[,] arr = new T[len, wide];

            // Records values
            // For each line
            for (int idx0 = 0; idx0 < len; idx0++)
            {
                // Jagged array element length
                int thisWide = jag[idx0].Length;
                for (int idx1 = 0; idx1 < wide; idx1++)
                {
                    if (idx1 < thisWide) // If there's data
                        arr[idx0, idx1] = jag[idx0][idx1];
                    else // Otherwise use filler
                        arr[idx0, idx1] = filler;
                }
            }
            return arr;
        }
    }
}
