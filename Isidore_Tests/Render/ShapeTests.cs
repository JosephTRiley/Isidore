using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Checks shape classes and supporting code
    /// </summary>
    public class ShapeTests
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            // error monitors
            List<bool> passed = new List<bool>();

            //// Plane shape Test
            //passed.Add(ShapeTracePlane.Run());

            //// Sphere shape Tests
            //passed.Add(ShapeTraceSphere.Run());

            // Mesh shape Tests
            passed.Add(ShapeTraceMesh.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
