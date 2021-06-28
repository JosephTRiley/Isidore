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
        /// Or value of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> Or value </returns>
        public static T Or<T>(T val0, T val1) 
        { 
            return Operator<T>.Or(val0, val1); 
        }

        /// <summary>
        /// Or array of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Or array of the arrays </returns>
        public static T[] Or<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.Or, arr0, arr1);
        }

        /// <summary>
        /// Or array of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> Or array of the arrays </returns>
        public static T[,] Or<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.Or, arr0, arr1); 
        }

        # endregion Generic Expressions
    }
}
