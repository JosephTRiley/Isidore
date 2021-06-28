using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Top level test for Isidore.Models
    /// </summary>
    public class ModelsTests
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n*******************");
            Console.WriteLine("\n Models Check");
            Console.WriteLine("\n*******************");

            // error monitors
            List<bool> passed = new List<bool>();

            // Turbulent Noise Point Test
            passed.Add(TurbulentNoiseTest.Run());

            // WFS Turbulence Noise Point Test
            passed.Add(TurbulencePointWFSTest.Run());

            // Scatter Point Turbulence Noise Point Test
            passed.Add(ScatterPointTurbulenceTest.Run());

            // Reference Point Turbulence Noise Point Test
            passed.Add(ReferencePointTurbulenceTest.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
