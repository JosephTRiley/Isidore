using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Checks material classes and supporting code
    /// </summary>
    public class MaterialTest
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            // error monitors
            List<bool> passed = new List<bool>();

            // Mono-Static BRDF Reflectance Sphere test
            passed.Add(MatTraceReflectance.Run());

            // Transparent Sphere test
            passed.Add(MatTraceRefractiveIndex.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
