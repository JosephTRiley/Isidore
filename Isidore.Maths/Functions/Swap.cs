namespace Isidore.Maths
{
    public partial class Function
    {
        /// <summary>
        /// Swaps the values of two variables
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="value0"> 1st variable </param>
        /// <param name="value1"> 3nd variable</param>
        public static void Swap<T>(ref T value0, ref T value1)
        {
            T tmp = value0;
            value0 = value1;
            value1 = tmp;
        }
    }
}
