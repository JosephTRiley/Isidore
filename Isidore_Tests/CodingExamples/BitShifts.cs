using System;

namespace Isidore_Tests
{
    /// <summary>
    /// This code provides coding examples that are common in programming
    /// that aren't commonly seen in scientific programming or scripting
    /// </summary>
    partial class CodingExamples
    {
        /// <summary>
        /// Example of bit shifting
        /// </summary>
        public static void BitShifts()
        {
            // Bit shift check
            Console.WriteLine("Bit shifting is a easy way to increase by powers of two.");
            int i = 1;
            int a = 0;
            int lsi = 0; // need to assign a value due to for loop
            int len = 6;
            for (int idx = 0; idx < len; idx++)
            {
                lsi = i << a;
                Console.WriteLine("Right shift " + i + " by " + a++ + " = " + lsi);
            }
            i = lsi;
            a = 0;
            for (int idx = 0; idx < len + 2; idx++)
            {
                lsi = i >> a;
                Console.WriteLine("Left shift " + i + " by " + a++ + " = " + lsi);
            }
            Console.WriteLine("And can be used to determine if an integer is a bit (i.e. 1).");
        }

        /// <summary>
        /// Shows how to generate a self-similar array.  This can be used to 
        /// reference surrounding elements in an N-dimensional array
        /// </summary>
        public static void FractalArray()
        {
            // Grid resolution
            int[] res = { 3, 4, 5, 6 };

            // Each cube will have 2^D the number of points
            int rank = res.Length;
            int totPts = 1 << rank;

            // Construct a template for mapping
            int[,] template = new int[totPts, rank];
            
            // Method 1
            for (int pIdx = 0; pIdx < totPts; pIdx++)
                for (int dIdx = 0; dIdx < rank; dIdx++) // each dimension
                    template[pIdx, dIdx] = pIdx >> dIdx & 1; // Adjacent points

            Console.WriteLine("\n Generating a Binary Fractal Array is simple with code.");
            for (int idx = 0; idx < totPts; idx++)
            {
                Console.WriteLine("Template Point " + idx + " : " + template[idx,0] + ", " +
                    template[idx, 1] + ", " + template[idx, 2] + ", " + template[idx, 3]);
            }
        }
    }
}
