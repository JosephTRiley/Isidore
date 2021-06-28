using System;
using System.Collections.Generic;
using System.Linq;

namespace Isidore.Maths
{

    public static partial class Arr
    {
        /// <summary>
        /// Returns a random permutation of an array using a Fisher-Yates 
        /// shuffle
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="array"> Original sequence </param>
        /// <param name="rng"> Random instance for shuffling </param>
        /// <returns> Permuted array </returns>
        public static IEnumerable<T> Shuffle<T>(T[] array, 
            Random rng = null)
        {
            Random Rng = rng ?? new Random();

            // Makes a new copy
            T[] elements = array.ToArray();

            // Cycles down the array, stopping at idx 1 because 0 will 
            // already be sorted
            for(int idx = elements.Length-1; idx > 0; idx--)
            {
                // Finds the index for swapping by random selection of
                // a random integer equal to or less than idx
                int swapIdx = Rng.Next(idx + 1);

                // Swapping operation
                T hold = elements[idx];
                elements[idx] = elements[swapIdx];
                elements[swapIdx] = hold;
            }
            return elements;
        }

        /// <summary>
        /// Returns a permuted array of integers using the Shuffle method
        /// </summary>
        /// <param name="length"> Array length </param>
        /// <param name="rng"> Random realization </param>
        /// <returns> Permuted array </returns>
        public static IEnumerable<int> Permutation(int length, Random rng)
        {
            Random Rng = rng ?? new Random();

            // Builds an ascending integer list
            int[] elements = Enumerable.Range(0, length).ToArray();

            var shuffled = Shuffle(elements, Rng);

            return shuffled;
        }
    }
}
