namespace Isidore.Maths
{

    /// <summary>
    /// Provides operators for scalar and array data types
    /// </summary>
    public static partial class Operator
    {
        // Since logical operators in arrays aren't as intensive, or common,
        // only the LINQ Expression methods are used 

        # region Generic Expressions

        /// <summary>
        /// Greater than or equal or equal comparison of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> Greater than or equal comparison value </returns>
        public static bool GreaterThanOrEqual<T>(T val0, T val1) 
        { 
            return Operator<T>.GreaterThanOrEqual(val0, val1); 
        }

        /// <summary>
        /// Greater than or equal comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Greater than or equal comparison array </returns>
        public static bool[] GreaterThanOrEqual<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.GreaterThanOrEqual, arr0, arr1);
        }

        /// <summary>
        /// Greater than or equal comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Greater than or equal comparison array </returns>
        public static bool[] GreaterThanOrEqual<T>(T val, T[] arr)
        {
            return Element.Op(Operator<T>.GreaterThanOrEqual, val, arr);
        }

        /// <summary>
        /// Greater than or equal comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Greater than or equal comparison array </returns>
        public static bool[] GreaterThanOrEqual<T>(T[] arr, T val)
        {
            return Element.Op(Operator<T>.GreaterThanOrEqual, arr, val);
        }

        /// <summary>
        /// Greater than or equal comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Greater than or equal comparison array </returns>
        public static bool[,] GreaterThanOrEqual<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.GreaterThanOrEqual, arr0, arr1); 
        }

        /// <summary>
        /// Greater than or equal comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Greater than or equal comparison array </returns>
        public static bool[,] GreaterThanOrEqual<T>(T val, T[,] arr)
        {
            return Element.Op(Operator<T>.GreaterThanOrEqual, val, arr);
        }

        /// <summary>
        /// Greater than or equal comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Greater than or equal comparison array </returns>
        public static bool[,] GreaterThanOrEqual<T>(T[,] arr, T val)
        {
            return Element.Op(Operator<T>.GreaterThanOrEqual, arr, val);
        }

        # endregion Generic Expressions
    }
}
