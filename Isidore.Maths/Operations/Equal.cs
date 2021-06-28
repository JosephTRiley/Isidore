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
        /// Equality comparison of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> Equality comparison value </returns>
        public static bool Equal<T>(T val0, T val1) 
        { 
            return Operator<T>.Equal(val0, val1); 
        }

        /// <summary>
        /// Equality comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Equality comparison array </returns>
        public static bool[] Equal<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.Equal, arr0, arr1);
        }

        /// <summary>
        /// Equality comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Equality comparison array </returns>
        public static bool[] Equal<T>(T val, T[] arr)
        {
            return Element.Op(Operator<T>.Equal, val, arr);
        }

        /// <summary>
        /// Equality comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Equality comparison array </returns>
        public static bool[] Equal<T>(T[] arr, T val)
        {
            return Element.Op(Operator<T>.Equal, arr, val);
        }

        /// <summary>
        /// Equality comparison of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Equality comparison array </returns>
        public static bool[,] Equal<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.Equal, arr0, arr1); 
        }

        /// <summary>
        /// Equality comparison of a single value and array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> Value </param>
        /// <param name="arr"> Array </param>
        /// <returns> Equality comparison array </returns>
        public static bool[,] Equal<T>(T val, T[,] arr)
        {
            return Element.Op(Operator<T>.Equal, val, arr);
        }

        /// <summary>
        /// Equality comparison of an array and a single value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <param name="val"> Value </param>
        /// <returns> Equality comparison array </returns>
        public static bool[,] Equal<T>(T[,] arr, T val)
        {
            return Element.Op(Operator<T>.Equal, arr, val);
        }

        # endregion Generic Expressions
    }
}
