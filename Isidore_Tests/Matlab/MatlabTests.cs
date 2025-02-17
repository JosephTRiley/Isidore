using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Top level test for Isidore.Matlab
    /// </summary>
    internal class MatlabTests
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n*******************");
            Console.WriteLine("\n Matlab Check");
            Console.WriteLine("\n*******************");

            // error monitors
            List<bool> passed = new List<bool>();

            // Net Test (For checking class lists like Shapes)
            passed.Add(NetTest.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
