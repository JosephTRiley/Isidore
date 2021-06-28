using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Checks volume classes and supporting code
    /// </summary>
    public class VolumeTests
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            // error monitors
            List<bool> passed = new List<bool>();

            // Voxel Volume Test
            passed.Add(VolumeTraceVoxel.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
