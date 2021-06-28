using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    class Geometry
    {
        public static bool Run()
        {
            Console.WriteLine("\n Euclidean Checks \n");

            // error monitors
            List<bool> passed = new List<bool>();

            // Check calls
            passed.Add(PointTest.Run());
            passed.Add(DistancePoint2Line.Run());
            passed.Add(KDTreeTest.Run());
            passed.Add(PointGridTest.Run());
            passed.Add(MarchingCubeTest.Run());
            passed.Add(PlaneTest.Run());
            passed.Add(SphereTest.Run());

            // Checker
            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            if (passedAll)
                Console.WriteLine("All Euclidean Checks Successful");
            else
                Console.WriteLine("At Least One Euclidean Check Failed");

            return passedAll;
        }
    }
}
