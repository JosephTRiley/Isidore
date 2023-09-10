using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Matlab;
using Isidore.Models;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests procedural textures and noise functions
    /// </summary>
    class TurbulentNoiseTest
    {
        /// <summary>
        /// Tests that the noise models & procedural textures are working as 
        /// expected
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {

            Console.WriteLine("\n-Starting Turbulent Noise Check");

            // (Improved) Perlin noise function
            PerlinNoiseFunction pnoise = new PerlinNoiseFunction(12345, 10);

            // General noise parameters
            //double minFreq = 0.25;
            //double maxFreq = 512;
            //double minFreq = 1;
            //double maxFreq = 1;
            //double minFreq = 1;
            //double maxFreq = 16;
            //double minFreq = 0.5;
            //double maxFreq = 8;
            double minFreq = 0.25;
            double maxFreq = 16;
            double H = 1 / 3;
            NoiseDistribution normDist = 0;
            NoiseDistribution lognormDist = NoiseDistribution.LogNormal;
            Vector noisePos = new Vector(new double[]
                { 123.4, 234.5, 345.6, 456.7 });

            // Standard normal
            TurbulentNoise noiseSN = new TurbulentNoise(pnoise, minFreq, maxFreq, H, 
                0, 1, normDist, noisePos);

            // 5 sig, normal
            TurbulentNoise noise5s = new TurbulentNoise(pnoise, minFreq, maxFreq, H,
                0, 5, normDist, noisePos);

            // 9 mu, 3 sig, normal
            TurbulentNoise noise9m3s = new TurbulentNoise(pnoise, minFreq, maxFreq, H,
                9, 3, normDist, noisePos);

            // 9 mu, 3 sig, log-normal
            TurbulentNoise noise9m3sLN = new TurbulentNoise(pnoise, minFreq, maxFreq, H,
                9, 3, lognormDist, noisePos);

            // Time length
            int xLen = 4096;
            int xMax = 256;
            double[] iArr = Distribution.Increment(0.0, 1.0, 1.0 / (xLen - 1));
            double[] xArr = iArr.Select(x => x * xMax).ToArray();

            // Cycles through each time point
            double[,] pData = new double[xLen, 4];
            Point coord = new Point(new double[] { 1, 2, 3, 0 });
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int idx = 0; idx < xLen; idx++)
            {
                // New coordinate
                coord.Comp[coord.Comp.Length - 1] = xArr[idx];

                // Calculates noise values
                double noise = noiseSN.GetVal(coord);
                pData[idx, 0] = noise;
                noise = noise5s.GetVal(coord);
                pData[idx, 1] = noise;
                noise = noise9m3s.GetVal(coord);
                pData[idx, 2] = noise;
                noise = noise9m3sLN.GetVal(coord);
                pData[idx, 3] = noise;
            }

            // Total simulations time
            watch.Stop();
            Console.WriteLine("Turbulent noise time: {0}ms",
                watch.ElapsedMilliseconds);

            /////////////////////////////////
            // Matlab processing
            /////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                "OutputData\\Models\\TurbulentNoise";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "xArr", xArr);
            MatLab.Put(matlab, "pData", pData);
            resStr = matlab.Execute("TurbulentNoiseCheck");

            Console.WriteLine("\n Turbulent Noise Check Passed");

            return true;
        }
    }
}
