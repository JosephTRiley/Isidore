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
        /// Not value of a value
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="val"> value </param>
        /// <returns> Not value </returns>
        public static T Not<T>(T val) 
        { 
            return Operator<T>.Not(val); 
        }

        /// <summary>
        /// Not value of an array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Not value of the array </returns>
        public static T[] Not<T>(T[] arr)
        {
            return Element.Op(Operator<T>.Not, arr);
        }

        /// <summary>
        /// Not value of an array (Multiples by -1)
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Not value of the array </returns>
        public static T[,] Not<T>(T[,] arr) 
        { 
            return Element.Op(Operator<T>.Not, arr); 
        }

        # endregion Generic Expressions
    }
}
