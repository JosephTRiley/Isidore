using System;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests procedural textures and noise functions
    /// </summary>
    class PerlinNoiseStandardFactorTest
    {
        /// <summary>
        /// Tests that the noise models & procedural textures are working as 
        /// expected
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            /////////////////////////////////
            // Checks noise functions
            /////////////////////////////////

            Console.WriteLine("\n-Starting Perlin Noise standard normal factor check");

            // Noise function parameters
            int tablePower = 15; // Permutable hash table size (2 ^ tabelPower)
            int slen = 1 << tablePower; // Data series length
            int noiseSeed = 12345; // Seed used in the permuted table
            // There are two separate noise functions: 3D & 4D
            Vector noiseOffset3D = new Vector(new double[]
                { 123.456, 234.567, 345.678});
            Vector noiseOffset4D = new Vector(new double[]
                { 123.456, 234.567, 345.678, 456.789 });

            // Sampling points
            int factor = 16; // Number of points across lattice cells
            int flen = slen * factor; // Spans entire noise function
            double dt = 1.0 / factor; // Step size
            double[] time = Distribution.Increment(dt, slen, dt); // time
            double[,] coords3D = new double[flen, 3]; // 3D Coordinate array
            double[,] coords4D = new double[flen, 4]; // 4D Coordinate array
            // Populates coordinate array
            // Populates coordinate array
            for (int idx = 0; idx < flen; idx++)
            {
                coords3D[idx, 0] = time[idx];
                coords4D[idx, 0] = time[idx];
            }

            // Cycles through each realization
            int numReal = 500; // Number of realizations
            double[] pnoise3DArr = new double[flen];
            double[] pnoise4DArr = new double[flen];
            double[,] mean3D = new double[2, numReal]; // Perlin 3D STD
            double[,] mean4D = new double[2, numReal]; // Perlin 4D STD
            double[,] std3D = new double[2, numReal]; // Perlin 3D STD
            double[,] std4D = new double[2, numReal]; // Perlin 4D STD

            double[] eTime = new double[2];
            Stopwatch watch = new Stopwatch();
            for (int standIdx = 0; standIdx < 2; standIdx++)
            {
                watch.Restart();
                watch.Start();
                for (int realIdx = 0; realIdx < numReal; realIdx++)
                {
                    // Calculates the coordinates
                    Point[] pts3D = Point.Array(Operator.Add(coords3D, 
                        0.01 * realIdx));
                    Point[] pts4D = Point.Array(Operator.Add(coords4D,
                        0.01 * realIdx));

                    // Creates noise function
                    int thisSeed = noiseSeed + realIdx;
                    PerlinNoiseFunction perlinNoise = new PerlinNoiseFunction(thisSeed,
                        tablePower, standIdx == 1);

                    // Creates 3D & 4D instances
                    Noise pnoise3D = new Noise(perlinNoise, noiseOffset3D);
                    Noise pnoise4D = new Noise(perlinNoise, noiseOffset4D);

                    // Retrieves the noise values
                    pnoise3DArr = pnoise3D.GetVal(pts3D);
                    pnoise4DArr = pnoise4D.GetVal(pts4D);

                    // Calculates the standard deviations
                    double[] stats = Stats.STD(pnoise3DArr);
                    std3D[standIdx, realIdx] = stats[0];
                    mean3D[standIdx, realIdx] = stats[1];
                    stats = Stats.STD(pnoise4DArr);
                    std4D[standIdx, realIdx] = stats[0];
                    mean4D[standIdx, realIdx] = stats[1];

                    Console.WriteLine("Perlin Noise, Standard factor " +
                        "applied {0}, realization: {1}", standIdx == 1, 
                        realIdx);
                }
                watch.Stop();
                eTime[standIdx] = watch.ElapsedMilliseconds;
                Console.WriteLine("Perlin Noise Standard Factor, Func={0}: {1}ms", 
                    standIdx==0, watch.ElapsedMilliseconds);
            }

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
            MatLab.Put(matlab, "time", time);
            MatLab.Put(matlab, "pnoise3DArr", pnoise3DArr);
            MatLab.Put(matlab, "pnoise4DArr", pnoise4DArr);
            MatLab.Put(matlab, "std3D", std3D);
            MatLab.Put(matlab, "std4D", std4D);
            MatLab.Put(matlab, "mean3D", mean3D);
            MatLab.Put(matlab, "mean4D", mean4D);
            MatLab.Put(matlab, "eTime", eTime);

            resStr = matlab.Execute("PerlinNoiseStandardFactorCheck");

            //Console.WriteLine("\n Noise Function Check Passed");

            return true;
        }
    }
}
