using System;
using Isidore.Maths;

namespace Isidore_Tests
{
    public class AreTest
    {
        public static bool Run()
        {

            ////////////////////////////////
            // Checks Random Generator
            ////////////////////////////////
            Console.WriteLine("\nAre Test.");

            Rand randGen = new Rand(123456); // Random generator
            int[] rand1 = randGen.Int(20, 0, 10); // 1D integer random array
            int[,] rand2 = randGen.Int(4, 4, 0, 10);
            
            // Predicate for values = 8
            int lookfor = 8;
            Predicate<int> isEqual = n => n == lookfor; // Lambda expression

            ///////////////
            // Any check //
            ///////////////

            // 1D array
            bool areAnyT = Are.Any(rand1, isEqual); // Should come back true
            bool areAnyF = Are.Any(rand1, n => n > 10); // Should come back false
            // 2D array
            bool[] areAnyT2D = Are.Any(rand2, isEqual);
            bool[] areAnyF2D = Are.Any(rand2, n => n > 10);
            // 2D to single tag
            bool areAnyT1D = Are.Any(areAnyT2D); // Should come back true
            bool areAnyF1D = Are.Any(areAnyF2D); // Should come back false

            ///////////////
            // All check //
            ///////////////

            // 1D array
            bool areAllF = Are.All(rand1, isEqual); // Should come back false
            bool areAllT = Are.All(rand1, n => n <= 10); // Should come back true
            // 2D array
            bool[] areAllF2D = Are.All(rand2, isEqual);
            bool[] areAllT2D = Are.All(rand2, n => n <= 10);
            // 2D to single tag
            bool areAllT1D = Are.All(areAllT2D); // Should come back false
            bool areAllF1D = Are.All(areAllF2D); // Should come back true

            bool passed = areAnyT && !areAnyF && areAnyT1D && !areAnyF1D &&
                !areAllF && areAllT && !areAllF1D && areAllT1D;

                    if(passed)
                Console.WriteLine("Are evaluation Successful.");
            else
                Console.WriteLine("Are evaluation Failed.");
            return passed;
        }
    }
}
