using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Checks noise functions and texture classes and supporting code
    /// </summary>
    public class TextureTest
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            // error monitors
            List<bool> passed = new List<bool>();

            // Calculates and tests the Perlin scaling factor
            passed.Add(PerlinNoiseStandardFactorTest.Run());

            // Noise functions (Mostly Perlin based)
            passed.Add(NoiseFunctionTest.Run());

            // Traveling through Kolmogorov turbulence
            passed.Add(ProceduralTextureTest.Run());

            // Procedural texture value interpolation
            passed.Add(ProceduralValueTest.Run());

            // Perturbation textures
            passed.Add(PerturbedTextureTest.Run());

            // Procedural texture value interpolation
            passed.Add(ProceduralMixingValueTest.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
