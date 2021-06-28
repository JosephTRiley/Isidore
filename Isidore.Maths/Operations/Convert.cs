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
        /// Converts a value type to another type
        /// </summary>
        /// <typeparam name="Tin"> Input data type </typeparam>
        /// <typeparam name="T"> Output data type </typeparam>
        /// <param name="val"> Value </param>
        /// <returns> Casted value </returns>
        public static T Convert<Tin, T>(Tin val) 
        {
            return Operator<Tin, T>.Convert(val);
        }

        /// <summary>
        /// Converts an array type to another type
        /// </summary>
        /// <typeparam name="Tin"> Input data type </typeparam>
        /// <typeparam name="T"> Output data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Casted value </returns>
        public static T[] Convert<Tin, T>(Tin[] arr)
        {
            return Element.Op(Operator<Tin, T>.Convert, arr);
        }

        /// <summary>
        /// Converts an array type to another type
        /// </summary>
        /// <typeparam name="Tin"> Input data type </typeparam>
        /// <typeparam name="T"> Output data type </typeparam>
        /// <param name="arr"> Array </param>
        /// <returns> Casted value </returns>
        public static T[,] Convert<Tin, T>(Tin[,] arr) 
        {
            return Element.Op(Operator<Tin, T>.Convert, arr); 
        }

        # endregion Generic Expressions
    }
}
