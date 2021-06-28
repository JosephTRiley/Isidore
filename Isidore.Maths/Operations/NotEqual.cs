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
        /// Inequality comparison of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> Inequality comparison value </returns>
        public static bool NotEqual<T>(T val0, T val1) 
        { 
            return Operator<T>.NotEqual(val0, val1); 
        }

        /// <summary>
        /// Inequality comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Inequality comparison array </returns>
        public static bool[] NotEqual<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.NotEqual, arr0, arr1);
        }

        /// <summary>
        /// Inequality comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Inequality comparison array </returns>
        public static bool[] NotEqual<T>(T val, T[] arr)
        {
            return Element.Op(Operator<T>.NotEqual, val, arr);
        }

        /// <summary>
        /// Inequality comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Inequality comparison array </returns>
        public static bool[] NotEqual<T>(T[] arr, T val)
        {
            return Element.Op(Operator<T>.NotEqual, arr, val);
        }

        /// <summary>
        /// Inequality comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Inequality comparison array </returns>
        public static bool[,] NotEqual<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.NotEqual, arr0, arr1); 
        }

        /// <summary>
        /// Inequality comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Inequality comparison array </returns>
        public static bool[,] NotEqual<T>(T val, T[,] arr)
        {
            return Element.Op(Operator<T>.NotEqual, val, arr);
        }

        /// <summary>
        /// Inequality comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Inequality comparison array </returns>
        public static bool[,] NotEqual<T>(T[,] arr, T val)
        {
            return Element.Op(Operator<T>.NotEqual, arr, val);
        }

        # endregion Generic Expressions
    }
}
