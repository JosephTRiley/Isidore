using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Top level test for Isidore.Render
    /// </summary>
    public class RenderTests
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n*******************");
            Console.WriteLine("\n Render Check");
            Console.WriteLine("\n*******************");

            // error monitors
            List<bool> passed = new List<bool>();

            // Class Test (For checking class lists like Shapes)
            passed.Add(ClassTest.Run());

            // Property Test (For checking that cloning is working
            passed.Add(PropertyTest.Run());

            // Image Texture Test
            passed.Add(TextureMapTest.Run());

            // Shape Tests
            passed.Add(ShapeTests.Run());

            // Volume Tests
            passed.Add(VolumeTests.Run());

            // Octree Test
            passed.Add(OctreeTraceTest.Run());

            // Surface Tests
            passed.Add(MaterialTest.Run());

            // Noise function & procedural texture tests
            passed.Add(TextureTest.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
