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
        /// And value of two values
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val0"> value </param>
        /// <param name="val1"> value </param>
        /// <returns> And value </returns>
        public static T And<T>(T val0, T val1) 
        { 
            return Operator<T>.And(val0, val1); 
        }

        /// <summary>
        /// And array of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> And array of the arrays </returns>
        public static T[] And<T>(T[] arr0, T[] arr1)
        {
            return Element.Op(Operator<T>.And, arr0, arr1);
        }

        /// <summary>
        /// And array of two array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr0"> Array </param>
        /// <param name="arr1"> Array </param>
        /// <returns> And array of the arrays </returns>
        public static T[,] And<T>(T[,] arr0, T[,] arr1) 
        { 
            return Element.Op(Operator<T>.And, arr0, arr1); 
        }

        # endregion Generic Expressions
    }
}
