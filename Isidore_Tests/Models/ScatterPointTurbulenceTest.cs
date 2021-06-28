using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Models;
using Isidore.Matlab;
using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests the WFS turbulence point
    /// </summary>
    class ScatterPointTurbulenceTest
    {
        /// <summary>
        /// Tests the procedural mixing value material is working as 
        /// expected.
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Scatter Point Turbulence Check");

            //////////////////////////////////////////
            // Points to measure & simulation time
            //////////////////////////////////////////

            // Point grid
            int len0 = 140, len1 = 120, len2 = 2;
            var lowerCorner = new double[] { 0, 0, 0 };
            var separation = new double[] { 0.01, 0.01, 3 };
            var ptGrid = new Point[140, 120, 2];
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                    for (int idx2 = 0; idx2 < len2; idx2++)
                        ptGrid[idx0, idx1, idx2] = new Point(
                            separation[0] * idx0, separation[1] * idx1, 
                            separation[2] * idx2);

            // Simulation time & sampling period
            double simTime = 10;
            double samplePeriod = 0.05;
            int totFrames = (int)(simTime / samplePeriod) + 1;
            var time = Enumerable.Range(0, totFrames).
                Select(x => x * samplePeriod).ToArray();
            var lent = time.Length;

            //////////////////////////////////
            // 1st Turbulence point & inputs
            //////////////////////////////////

            // General noise parameters
            int tablePower = 10; // Permutable table size (2-based)
            bool standNorm = true; // Standard normal flag
            double minFreq = 0.125; // Minimum frequency
            double maxFreq = 512; // Maximum frequency
            double lacunarity = 2; // Noise lacunarity
            var shift = new Vector(new double[] 
                { 12.3, 23.4, 34.5, 0 }); // Shift in noise space

            // WFE Magnitude Noise: Independent standard fBm
            int seed_WFE = 1234; // Random realization seed
            var H_WFS = 1.0 / 3.0; // Hurst exponent
            double std_WFE = 1.0; // The STD noise multiplier
            double mean_WFE = 0; // The noise mean
            var noiseFuncWFE = new PerlinNoiseFunction(seed_WFE, tablePower, 
                standNorm);
            var NoiseWFE = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            int seed_Dir = 12345; // Random realization seed
            var H_Dir = 0.8; // Hurst exponent
            double std_Dir = 0.05; // The STD noise multiplier
            double mean_Dir = Math.PI * 1 / 2; // The noise mean
            var noiseFuncDir = new PerlinNoiseFunction(seed_Dir, tablePower,
                standNorm);
            var NoiseDir = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir, mean_Dir);

            // Speed Noise: Independent log-normal mBm
            int seed_Speed = 123456; // Random realization seed
            var H_Speed = 0.8; // Hurst exponent
            double std_Speed = 0.01; // The STD noise multiplier
            double mean_Speed = 0.5; // The noise mean
            var noiseFuncSpeed = new PerlinNoiseFunction(seed_Speed,
                tablePower, standNorm);
            var NoiseSpeed = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed, mean_Speed);
            // Sets distribution to log-normal
            NoiseSpeed.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            var pos = new Point(0, 0, 0); // Point position
            var trans = Transform.RotZ(Math.PI / 2); // Orientation transform
            double timeStep = samplePeriod; // Turbulence walk steps

            // Creates a turbulence point instance
            var turbPt = new TurbulencePointWFS(pos, NoiseWFE, NoiseDir,
                NoiseSpeed, trans, timeStep, 0);

            //////////////////////////////////
            // 2nd Turbulence point & inputs
            //////////////////////////////////

            // WFE Magnitude Noise: Independent standard fBm
            var NoiseWFE2 = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            double std_Dir2 = std_Dir * 2; // The STD noise multiplier
            double mean_Dir2 = mean_Dir * 2; // The noise mean
            var NoiseDir2 = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir2, mean_Dir2);

            // Speed Noise: Independent log-normal mBm
            double std_Speed2 = std_Speed * 2.0; // The STD noise multiplier
            double mean_Speed2 = mean_Speed * 2.0; // The noise mean
            var NoiseSpeed2 = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed2, mean_Speed2);
            // Sets distribution to log-normal
            NoiseSpeed2.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            var pos2 = new Point(len0 * separation[0],
                len1 * separation[1], 0); // Point position

            // Creates a turbulence point instance
            var turbPt2 = new TurbulencePointWFS(pos2, NoiseWFE2, NoiseDir2,
                NoiseSpeed2, trans, timeStep, 0);

            //////////////////////////////////
            // 3rd Turbulence point & inputs
            //////////////////////////////////

            // WFE Magnitude Noise: Independent standard fBm
            var NoiseWFE3 = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            double std_Dir3 = std_Dir * 3; // The STD noise multiplier
            double mean_Dir3 = mean_Dir * 3; // The noise mean
            var NoiseDir3 = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir3, mean_Dir3);

            // Speed Noise: Independent log-normal mBm
            double std_Speed3 = std_Speed * 3.0; // The STD noise multiplier
            double mean_Speed3 = mean_Speed * 3.0; // The noise mean
            var NoiseSpeed3 = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed3, mean_Speed3);
            // Sets distribution to log-normal
            NoiseSpeed2.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            var pos3 = new Point(0,
                len1 * separation[1], 0); // Point position

            // Creates a turbulence point instance
            var turbPt3 = new TurbulencePointWFS(pos3, NoiseWFE3, NoiseDir3,
                NoiseSpeed3, trans, timeStep, 0);

            //////////////////////////////////
            // 3rd Turbulence point & inputs
            //////////////////////////////////

            // WFE Magnitude Noise: Independent standard fBm
            var NoiseWFE4 = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            double std_Dir4 = std_Dir * 4; // The STD noise multiplier
            double mean_Dir4 = mean_Dir * 4; // The noise mean
            var NoiseDir4 = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir4, mean_Dir4);

            // Speed Noise: Independent log-normal mBm
            double std_Speed4 = std_Speed * 4.0; // The STD noise multiplier
            double mean_Speed4 = mean_Speed * 4.0; // The noise mean
            var NoiseSpeed4 = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed4, mean_Speed4);
            // Sets distribution to log-normal
            NoiseSpeed2.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            var pos4 = new Point(len0 * separation[0], 0,
                0); // Point position

            // Creates a turbulence point instance
            var turbPt4 = new TurbulencePointWFS(pos4, NoiseWFE4, NoiseDir4,
                NoiseSpeed4, trans, timeStep, 0);

            //////////////////////////////////////////////////
            // Scatter point turbulence
            /////////////////////////////////////////////////
            var pts = new List<TurbulencePointWFS>(4);
            pts.Add(turbPt);
            pts.Add(turbPt2);
            pts.Add(turbPt3);
            pts.Add(turbPt4);
            var turbulence = new ScatterPointTurbulence(pts, 2, -1);

            //////////////////////////////////////////////////
            // Rendering
            /////////////////////////////////////////////////

            // Book-keeping
            var noiseVal = new double[len0, len1, lent, len2];

            // Cycles through coherence rates
            var watch = new Stopwatch();
            for (int idx2 = 0; idx2 < len2; idx2++)
            {
                // Cycles through each time step
                for (int tidx = 0; tidx < time.Length; tidx++)
                {
                    // Cycles through each point
                    for (int idx0 = 0; idx0 < len0; idx0++)
                    {
                        for (int idx1 = 0; idx1 < len1; idx1++)
                        {
                            // Point at this location
                            Point pt = ptGrid[idx0, idx1, idx2];

                            // Retrieves the noise value at each point
                            watch.Start();
                            var val = turbulence.GetVal(pt, time[tidx]);
                            watch.Stop();

                            // Records noise value
                            noiseVal[idx0, idx1, tidx, idx2] = val;
                        }
                    }
                }
            }

            Console.WriteLine("Scatter Point Turbulence total time = {0}s", 
                watch.ElapsedMilliseconds / 1000.0);

            ///////////////////////////////////////////////////////////////////
            // Matlab processing
            ///////////////////////////////////////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                "OutputData\\Models\\ScatterPointTurbulence";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            // Outputs data
            MatLab.Put(matlab, "time", time);
            MatLab.Put(matlab, "noiseVal", noiseVal);
            MatLab.Put(matlab, "separation", separation);
            resStr = matlab.Execute("ScatterPointTurbulenceCheck");

            Console.WriteLine("\n Scatter Point Turbulence Check Passed");

            return true;
        }
    }
}
