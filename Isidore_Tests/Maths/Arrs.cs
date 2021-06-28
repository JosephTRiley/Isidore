using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    public class ArrTest
    {
        public static bool Run()
        {
            Console.WriteLine("\n Array Operation Checks \n");

            // error monitors
            List<bool> passed = new List<bool>();

            // Check calls
            passed.Add(ArrInverse.Run());
            passed.Add(ArrSuffle.Run());

            // Checker
            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            if (passedAll)
                Console.WriteLine("All Array Operation Checks Successful");
            else
                Console.WriteLine("At Least One Array Operation Check Failed");

            return passedAll;
        }
    }
}
