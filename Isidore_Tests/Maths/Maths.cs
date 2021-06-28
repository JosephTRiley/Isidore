using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    public class Maths
    {
        public static bool Run()
        {
            Console.WriteLine("\n*******************");
            Console.WriteLine("\n Maths Check");
            Console.WriteLine("\n*******************");

            // error monitors
            List<bool> passed = new List<bool>();

            // Maths Tests
            passed.Add(Geometry.Run());
            passed.Add(AreTest.Run());
            passed.Add(ArrTest.Run());
            passed.Add(Randoms.Run());
            passed.Add(Operations.Run());
            passed.Add(Interpolates.Run());
            passed.Add(KeyFrameTest.Run());
            passed.Add(KeyFrameTransTest.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
