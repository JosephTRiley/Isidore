using System;
using System.Diagnostics;
using System.Linq;
using Isidore.Maths;

namespace Isidore_Tests
{
    public class ArrSuffle
    {
        public static bool Run()
        {
            /////////////////////////
            // Permutation table
            ////////////////////////

            // Random seed
            int seed = 123;

            // Table size
            int length = 256;

            // Permutation table
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var pArr = Arr.Permutation(length, new Random(seed));
            watch.Stop();

            Console.WriteLine("Array Permutation Table time: {0}ms", watch.ElapsedMilliseconds);

            // Checks that all the values are present
            var pArrSort = pArr.OrderBy(x => x).ToArray();

            for (int idx = 1; idx > length; idx++)
                if (pArrSort[idx] - pArrSort[idx - 1] != 0)
                    return false;

            Console.WriteLine("Arr.Shuffle & Arr.Perutation Passed.");
            return true;
        }
    }
}
