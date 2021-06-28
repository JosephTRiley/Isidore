using System.Collections.Generic;
using System.Linq;

namespace Isidore.Maths
{

    public static partial class Arr
    {
        /// <summary>
        /// Extracts a vector from an array using dimension for axis and
        /// place for the location.  Returns as an iterator.
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="array"> Data array </param>
        /// <param name="dimension"> Dimension to extract from 
        /// (0/1) </param>
        /// <param name="place"> Location in the array </param>
        /// <returns> Iterator vector </returns>
        public static IEnumerable<T> IExtract<T>(T[,] array, 
            int dimension, int place)
        {
            switch (dimension)
            {
                case 0:
                    for (int idx = 0; idx < array.GetLength(0); idx++)
                        yield return array[idx, place];
                    break;
                case 1:
                    for (int idx = 0; idx < array.GetLength(1); idx++)
                        yield return array[place, idx];
                    break;
            }
        }

        /// <summary>
        /// Extracts a vector from an array using dimension for axis and
        /// place for the location.  Returns as an iterator.
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="array"> Data array </param>
        /// <param name="dimension"> Dimension to extract from 
        /// (0/1) </param>
        /// <param name="place"> Location in the array </param>
        /// <param name="frame"> Location along the third 
        /// dimension </param>
        /// <returns> Iterator vector </returns>
        public static IEnumerable<T> IExtract<T>(T[,,] array, 
            int dimension, int place, int frame)
        {
            switch (dimension)
            {
                case 0:
                    for (int idx = 0; idx < array.GetLength(0); idx++)
                        yield return array[idx, place, frame];
                    break;
                case 1:
                    for (int idx = 0; idx < array.GetLength(1); idx++)
                        yield return array[place, idx, frame];
                    break;
            }
        }

        /// <summary>
        /// Extracts a vector from an array using dimension for axis and
        /// place for the location.  Returns as an 1D array.
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="array"> Data array </param>
        /// <param name="dimension"> Dimension to extract from 
        /// (0/1) </param>
        /// <param name="place"> Location in the array </param>
        /// <returns> Vector </returns>
        public static T[] Extract<T>(T[,] array, int dimension, int place)
        {
            return IExtract(array, dimension, place).ToArray();
        }

        /// <summary>
        /// Extracts a 2D array from a 3D array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="array"> Data array </param>
        /// <param name="frame"> Location along the 3rd dimension 
        /// to extract </param>
        /// <returns> Extracted 2D array </returns>
        public static T[,] Extract<T>(T[,,] array, int frame)
        {
            int len0 = array.GetLength(0);
            int len1 = array.GetLength(1);
            T[,] data = new T[len0, len1];
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                    data[idx0, idx1] = array[idx0, idx1, frame];
            return data;
        }
    }
}
