using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Evaluates every element of an array, returning the result
    /// </summary>
    public class Are
    {
        /// <summary>
        /// Given a 1D boolean array, returns a boolean indicating
        /// whether any value is true
        /// </summary>
        /// <param name="data"> 1D boolean array </param>
        /// <returns> Tag indicating whether any data is true </returns>
        public static bool Any(bool[] data)
        {
            // Steps through each values
            for (int idx0 = 0; idx0 < data.Length; idx0++)
                if (data[idx0]) // Applies predicate logic
                    return true; // returns if true
            return false;
        }

        /// <summary>
        /// Given a 1D boolean array, returns a boolean indicating
        /// whether all values are true
        /// </summary>
        /// <param name="data"> 1D boolean array </param>
        /// <returns> Tag indicating whether all data is true </returns>
        public static bool All(bool[] data)
        {
            // Steps through each values
            for (int idx0 = 0; idx0 < data.Length; idx0++)
                if (!data[idx0]) // Applies predicate logic
                    return false; // returns if true
            return true;
        }

        /// <summary>
        /// Given a 2D boolean array, returns a 1D boolean array 
        /// indicating whether any data per line is true
        /// </summary>
        /// <param name="data"> 2D boolean array </param>
        /// <returns> 1D Array indicating if any value in the second 
        /// dimension is true </returns>
        public static bool[] Any(bool[,] data)
        {
            int len0 = data.GetLength(0);
            int len1 = data.GetLength(1);
            bool[] ret = new bool[len0];
            // Steps through each values
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                    if (data[idx0, idx1]) // Applies predicate logic
                        ret[idx0] = true;
            return ret;
        }

        /// <summary>
        /// Given a 2D boolean array, returns a 1D boolean array 
        /// indicating whether all data per line is true
        /// </summary>
        /// <param name="data"> 2D boolean array </param>
        /// <returns> 1D Array indicating if all values in the second 
        /// dimension is true </returns>
        public static bool[] All(bool[,] data)
        {
            int len0 = data.GetLength(0);
            int len1 = data.GetLength(1);
            bool[] ret = new bool[len0];
            // Steps through each values
            for (int idx0 = 0; idx0 < len0; idx0++)
            {
                ret[idx0] = true; // Set to true
                for (int idx1 = 0; idx1 < len1; idx1++)
                    if (!data[idx0, idx1]) // Applies predicate logic
                        ret[idx0] = false; // sets to false
            }
            return ret;
        }

        /// <summary>
        /// Given a 1D array of data and a predicate delegate, will return
        /// a boolean indicating whether any data meets the condition
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="data"> 1D array to evaluate </param>
        /// <param name="predicate"> Predicate delegate used for 
        /// criteria </param>
        /// <returns> Tag of whether any value meets the predicate 
        /// criteria </returns>
        public static bool Any<T>(T[] data, Predicate<T> predicate )
        {
            // Steps through each values
            for (int idx0 = 0; idx0 < data.Length; idx0++)
                if (predicate(data[idx0])) // Applies predicate logic
                    return true; // returns if true
            return false;
        }

        /// <summary>
        /// Given a 1D array of data and a predicate delegate, will return
        /// a boolean indicating whether all data meets the condition
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="data"> 1D array to evaluate </param>
        /// <param name="predicate"> Predicate delegate used for 
        /// criteria </param>
        /// <returns> Tag of whether all value meets the predicate 
        /// criteria </returns>
        public static bool All<T>(T[] data, Predicate<T> predicate)
        {
            // Steps through each values
            for (int idx0 = 0; idx0 < data.Length; idx0++)
                if (!predicate(data[idx0])) // Applies predicate logic
                    return false; // returns if false
            return true;
        }

        /// <summary>
        /// Given a 2D array of data and a predicate delegate, will return
        /// a boolean array indicating whether any data per line meets 
        /// the condition
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="data"> 1D array to evaluate </param>
        /// <param name="predicate"> Predicate delegate used for 
        /// criteria </param>
        /// <returns> Array of whether any value in the second 
        /// dimension meets the predicate criteria </returns>
        public static bool[] Any<T>(T[,] data, Predicate<T> predicate)
        {
            int len0 = data.GetLength(0);
            int len1 = data.GetLength(1);
            bool[] ret = new bool[len0];
            // Steps through each values & applies predicate logic
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                    if (predicate(data[idx0, idx1]))
                        ret[idx0] = true;
            return ret;
        }

        /// <summary>
        /// Given a 2D array of data and a predicate delegate, will return
        /// a boolean array indicating whether all data per line meets 
        /// the condition
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="data"> 1D array to evaluate </param>
        /// <param name="predicate"> Predicate delegate used for 
        /// criteria </param>
        /// <returns> Array of whether all value in the second dimension 
        /// meets the predicate criteria </returns>
        public static bool[] All<T>(T[,] data, Predicate<T> predicate)
        {
            int len0 = data.GetLength(0);
            int len1 = data.GetLength(1);
            bool[] ret = new bool[len0];
            // Steps through each values & applies predicate logic
            for (int idx0 = 0; idx0 < len0; idx0++)
            {
                ret[idx0] = true; // Set to true
                for (int idx1 = 0; idx1 < len1; idx1++)
                    if (!predicate(data[idx0, idx1]))
                        ret[idx0] = false; // sets to false
            }
            return ret;
        }
    }


}
