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
        /// Less than or equal or equal comparison of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> Less than or equal comparison value </returns>
        public static bool LessThanOrEqual<T>(T val0, T val1) 
        { 
            return Operator<T>.LessThanOrEqual(val0, val1); 
        }

        /// <summary>
        /// Less than or equal comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Less than or equal comparison array </returns>
        public static bool[] LessThanOrEqual<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.LessThanOrEqual, arr0, arr1);
        }

        /// <summary>
        /// Less than or equal comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Less than or equal comparison array </returns>
        public static bool[] LessThanOrEqual<T>(T val, T[] arr)
        {
            return Element.Op(Operator<T>.LessThanOrEqual, val, arr);
        }

        /// <summary>
        /// Less than or equal comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Less than or equal comparison array </returns>
        public static bool[] LessThanOrEqual<T>(T[] arr, T val)
        {
            return Element.Op(Operator<T>.LessThanOrEqual, arr, val);
        }

        /// <summary>
        /// Less than or equal comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Less than or equal comparison array </returns>
        public static bool[,] LessThanOrEqual<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.LessThanOrEqual, arr0, arr1); 
        }

        /// <summary>
        /// Less than or equal comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Less than or equal comparison array </returns>
        public static bool[,] LessThanOrEqual<T>(T val, T[,] arr)
        {
            return Element.Op(Operator<T>.LessThanOrEqual, val, arr);
        }

        /// <summary>
        /// Less than or equal comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Less than or equal comparison array </returns>
        public static bool[,] LessThanOrEqual<T>(T[,] arr, T val)
        {
            return Element.Op(Operator<T>.LessThanOrEqual, arr, val);
        }

        # endregion Generic Expressions
    }
}
