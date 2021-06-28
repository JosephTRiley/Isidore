using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests Perlin noise functions
    /// </summary>
    class PerlinNoiseFunctionTest
    {
        /// <summary>
        /// Tests that the noise models & procedural textures are working as 
        /// expected
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Kensler Perlin Noise Check");

            /////////////////////////////////
            // Checks noise functions
            /////////////////////////////////

            // Grid size
            int xLen = 1000; // Need it to be at least 256 for WL analysis
            double xRng = 100; // image range
            var xPos = Distribution.Increment(0.0, xRng, xRng / xLen);

            // Creates noise function instance
            var pnf = new PerlinNoiseFunctionKensler(123, 9); // 512
            var pnoise = new Noise(pnf, new double[] { 123.4, 234.5 });

            Console.WriteLine("Checking Original Perlin noise");
            double[,] pDataOrig = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataOrig[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            Console.WriteLine("Checking Original Perlin noise with 5th order");
            pnf.interpolant = Interp.Quintic;
            double[,] pDataImpInt = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataImpInt[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            Console.WriteLine("Checking Original Perlin noise, wide interp");
            pnf.interpolant = Interp.Wide;
            double[,] pDataWideInt = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataWideInt[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            Console.WriteLine("Checking Perlin noise, 5th, & Radial Filter");
            pnf.interpolant = Interp.Quintic;
            pnf.surfletFilter = SurfletFilter.Radial;
            double[,] pDataRadial = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataRadial[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            Console.WriteLine("Checking Improved Perlin noise");
            pnf.interpolant = Interp.Quintic;
            pnf.noiseMethod = NoiseMethod.PerlinImproved;
            double[,] pDataImprove = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataImprove[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            Console.WriteLine("Checking Perlin noise value");
            pnf.noiseMethod = NoiseMethod.Value;
            double[,] pDataValue = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataValue[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            Console.WriteLine("Checking Perlin noise with jitter");
            pnf.noiseMethod = NoiseMethod.PerlinOriginal;
            pnf.applyJitter = true;
            double[,] pDataJit = new double[xLen, xLen];
            for (int idx0 = 0; idx0 < xLen; idx0++)
                for (int idx1 = 0; idx1 < xLen; idx1++)
                    pDataJit[idx0, idx1] = pnoise.GetVal(
                        new double[] { xPos[idx0], xPos[idx1] });

            /////////////////////////////////
            // Matlab processing
            /////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                "OutputData\\Render\\ProceduralTexture";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "xPos", xPos);
            MatLab.Put(matlab, "pDataOrig", pDataOrig);
            MatLab.Put(matlab, "pDataImpInt", pDataImpInt);
            MatLab.Put(matlab, "pDataWideInt", pDataWideInt);
            MatLab.Put(matlab, "pDataImprove", pDataImprove);
            MatLab.Put(matlab, "pDataRadial", pDataRadial);
            MatLab.Put(matlab, "pDataValue", pDataValue);
            MatLab.Put(matlab, "pDataJit", pDataJit);
            MatLab.Put(matlab, "gradient", pnf.Gradient);
            MatLab.Put(matlab, "jitter", pnf.Jitter);
            resStr = matlab.Execute("PerlinNoiseFunctionCheck");

            Console.WriteLine("\n Perlin Noise Check Passed");

            return true;
        }
    }
}
