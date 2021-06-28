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
        /// XOR value of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> XOR value </returns>
        public static T Xor<T>(T val0, T val1) 
        { 
            return Operator<T>.Xor(val0, val1); 
        }

        /// <summary>
        /// XOR array of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> XOR array of the arrays </returns>
        public static T[] Xor<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.Xor, arr0, arr1);
        }

        /// <summary>
        /// XOR array of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> XOR array of the arrays </returns>
        public static T[,] Xor<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.Xor, arr0, arr1); 
        }

        # endregion Generic Expressions
    }
}
