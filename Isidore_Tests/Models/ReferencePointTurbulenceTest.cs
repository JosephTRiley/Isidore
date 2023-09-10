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
    class ReferencePointTurbulenceTest
    {
        /// <summary>
        /// Tests the procedural mixing value material is working as 
        /// expected.
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Reference Point Turbulence Check");

            //////////////////////////////////////////
            // Points to measure & simulation time
            //////////////////////////////////////////

            // Point grid
            int len0 = 140, len1 = 120, len2 = 2;
            double[] lowerCorner = new double[] { 0, 0, 0 };
            double[] separation = new double[] { 0.01, 0.01, 3 };
            Point[,,] ptGrid = new Point[140, 120, 2];
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
            double[] time = Enumerable.Range(0, totFrames).
                Select(x => x * samplePeriod).ToArray();
            int lent = time.Length;

            //////////////////////////////////
            // 1st Turbulence point & inputs
            //////////////////////////////////

            // General noise parameters
            int tablePower = 10; // Permutable table size (2-based)
            bool standNorm = true; // Standard normal flag
            double minFreq = 0.125; // Minimum frequency
            double maxFreq = 512; // Maximum frequency
            double lacunarity = 2; // Noise lacunarity
            Vector shift = new Vector(new double[] 
                { 12.3, 23.4, 34.5, 0 }); // Shift in noise space

            // WFE Magnitude Noise: Independent standard fBm
            int seed_WFE = 1234; // Random realization seed
            double H_WFS = 1.0 / 3.0; // Hurst exponent
            double std_WFE = 1.0; // The STD noise multiplier
            double mean_WFE = 0; // The noise mean
            PerlinNoiseFunction noiseFuncWFE = new PerlinNoiseFunction(seed_WFE, tablePower, 
                standNorm);
            fBmNoise NoiseWFE = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            int seed_Dir = 12345; // Random realization seed
            double H_Dir = 0.8; // Hurst exponent
            double std_Dir = 0.05; // The STD noise multiplier
            double mean_Dir = Math.PI * 1 / 2; // The noise mean
            PerlinNoiseFunction noiseFuncDir = new PerlinNoiseFunction(seed_Dir, tablePower,
                standNorm);
            fBmNoise NoiseDir = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir, mean_Dir);

            // Speed Noise: Independent log-normal mBm
            int seed_Speed = 123456; // Random realization seed
            double H_Speed = 0.8; // Hurst exponent
            double std_Speed = 0.01; // The STD noise multiplier
            double mean_Speed = 0.5; // The noise mean
            PerlinNoiseFunction noiseFuncSpeed = new PerlinNoiseFunction(seed_Speed,
                tablePower, standNorm);
            fBmNoise NoiseSpeed = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed, mean_Speed);
            // Sets distribution to log-normal
            NoiseSpeed.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            Point pos = new Point(0, 0, 0); // Point position
            Transform trans = Transform.RotZ(Math.PI / 2); // Orientation transform
            double timeStep = samplePeriod; // Turbulence walk steps

            // Creates a turbulence point instance
            TurbulencePointWFS turbPt = new TurbulencePointWFS(pos, NoiseWFE, NoiseDir,
                NoiseSpeed, trans, timeStep, 0);

            //////////////////////////////////
            // 2nd Turbulence point & inputs
            //////////////////////////////////

            // WFE Magnitude Noise: Independent standard fBm
            fBmNoise NoiseWFE2 = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            double std_Dir2 = std_Dir * 2; // The STD noise multiplier
            double mean_Dir2 = mean_Dir * 2; // The noise mean
            fBmNoise NoiseDir2 = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir2, mean_Dir2);

            // Speed Noise: Independent log-normal mBm
            double std_Speed2 = std_Speed * 2.0; // The STD noise multiplier
            double mean_Speed2 = mean_Speed * 2.0; // The noise mean
            fBmNoise NoiseSpeed2 = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed2, mean_Speed2);
            // Sets distribution to log-normal
            NoiseSpeed2.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            Point pos2 = new Point(len0 * separation[0],
                len1 * separation[1], 0); // Point position

            // Creates a turbulence point instance
            TurbulencePointWFS turbPt2 = new TurbulencePointWFS(pos2, NoiseWFE2, NoiseDir2,
                NoiseSpeed2, trans, timeStep, 0);

            //////////////////////////////////
            // 3rd Turbulence point & inputs
            //////////////////////////////////

            // WFE Magnitude Noise: Independent standard fBm
            fBmNoise NoiseWFE3 = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            double std_Dir3 = std_Dir * 3; // The STD noise multiplier
            double mean_Dir3 = mean_Dir * 3; // The noise mean
            fBmNoise NoiseDir3 = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir3, mean_Dir3);

            // Speed Noise: Independent log-normal mBm
            double std_Speed3 = std_Speed * 3.0; // The STD noise multiplier
            double mean_Speed3 = mean_Speed * 3.0; // The noise mean
            fBmNoise NoiseSpeed3 = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed3, mean_Speed3);
            // Sets distribution to log-normal
            NoiseSpeed2.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            Point pos3 = new Point(0,
                len1 * separation[1], 0); // Point position

            // Creates a turbulence point instance
            TurbulencePointWFS turbPt3 = new TurbulencePointWFS(pos3, NoiseWFE3, NoiseDir3,
                NoiseSpeed3, trans, timeStep, 0);

            //////////////////////////////////
            // 4th Turbulence point & inputs
            //////////////////////////////////

            // WFE Magnitude Noise: Independent standard fBm
            fBmNoise NoiseWFE4 = new fBmNoise(noiseFuncWFE, minFreq, maxFreq,
                H_WFS, lacunarity, shift, std_WFE, mean_WFE);

            // Direction Noise: Independent standard fBm
            double std_Dir4 = std_Dir * 4; // The STD noise multiplier
            double mean_Dir4 = mean_Dir * 4; // The noise mean
            fBmNoise NoiseDir4 = new fBmNoise(noiseFuncDir, minFreq, maxFreq,
                H_Dir, lacunarity, shift, std_Dir4, mean_Dir4);

            // Speed Noise: Independent log-normal mBm
            double std_Speed4 = std_Speed * 4.0; // The STD noise multiplier
            double mean_Speed4 = mean_Speed * 4.0; // The noise mean
            fBmNoise NoiseSpeed4 = new fBmNoise(noiseFuncSpeed, minFreq, maxFreq,
                H_Speed, lacunarity, shift, std_Speed4, mean_Speed4);
            // Sets distribution to log-normal
            NoiseSpeed2.distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);

            // Turbulence point parameters
            Point pos4 = new Point(len0 * separation[0], 0,
                0); // Point position

            // Creates a turbulence point instance
            TurbulencePointWFS turbPt4 = new TurbulencePointWFS(pos4, NoiseWFE4, NoiseDir4,
                NoiseSpeed4, trans, timeStep, 0);

            ////////////////////////////////////
            //// Reference points
            ////////////////////////////////////
            //var refPt = turbPt as ReferencePoint;
            //var refPt2 = turbPt2 as ReferencePoint;
            //var refPt3 = turbPt3 as ReferencePoint;
            //var refPt4 = turbPt4 as ReferencePoint;
            //// Makes a list of turbulence points
            //var pts = new List<Point>(4);
            //pts.Add(turbPt);
            //pts.Add(turbPt2);
            //pts.Add(turbPt3);
            //pts.Add(turbPt4);
            //var pts2 = new List<Point>(pts);
            //var pts3 = new List<Point>(pts);
            //var pts4 = new List<Point>(pts);
            //// Removes the next turbulence points oppos from the reference list
            //pts.RemoveAt(1);
            //pts2.RemoveAt(2);
            //pts3.RemoveAt(3);
            //pts4.RemoveAt(0);
            //// Adds turbulence points to each reference list
            //refPt.ReferencePoints = pts;
            //refPt2.ReferencePoints = pts2;
            //refPt3.ReferencePoints = pts3;
            //refPt4.ReferencePoints = pts4;
            //// Makes a list of reference points
            //var refPts = new List<ReferencePoint>(4);
            //refPts.Add(refPt);
            //refPts.Add(refPt2);
            //refPts.Add(refPt3);
            //refPts.Add(refPt4);

            //////////////////////////////////
            // Reference points
            //////////////////////////////////
            ReferencePoint refPt = turbPt as ReferencePoint;
            ReferencePoint refPt2 = turbPt2 as ReferencePoint;
            ReferencePoint refPt3 = turbPt3 as ReferencePoint;
            ReferencePoint refPt4 = turbPt4 as ReferencePoint;
            // Makes two lists of turbulence points
            List<Point> pts0 = new List<Point>(2);
            pts0.Add(turbPt);
            pts0.Add(turbPt3);
            List<Point> pts1 = new List<Point>(2);
            pts1.Add(turbPt2);
            pts1.Add(turbPt4);
            // Adds turbulence points to each reference list
            refPt.ReferencePoints = pts0;
            refPt2.ReferencePoints = pts1;
            refPt3.ReferencePoints = pts0;
            refPt4.ReferencePoints = pts1;
            // Makes a list of reference points
            List<ReferencePoint> refPts = new List<ReferencePoint>(4);
            refPts.Add(refPt);
            refPts.Add(refPt2);
            refPts.Add(refPt3);
            refPts.Add(refPt4);

            //////////////////////////////////
            // Interpolating reference points
            //////////////////////////////////
            // Half positionsPositions
            Point pos5 = new Point(len0 * separation[0] / 2.0, 0,
                0); // Point position
            Point pos6 = new Point(len0 * separation[0] / 2.0,
                len1 * separation[1], 0); // Point position
            // Point lists
            List<Point> p5list = new List<Point>(2);
            p5list.Add(refPt);
            p5list.Add(refPt4);
            List<Point> p6list = new List<Point>(2);
            p6list.Add(refPt3);
            p6list.Add(refPt2);
            ReferencePoint refPt5 = new ReferencePoint(pos5.Comp, p5list, 
                ReferencePoint.Category.Interpolation);
            ReferencePoint refPt6 = new ReferencePoint(pos6.Comp, p6list,
                ReferencePoint.Category.Interpolation);
            refPts.Add(refPt5);
            refPts.Add(refPt6);

            //////////////////////////////////////////////////
            // Reference point turbulence
            /////////////////////////////////////////////////
            SmoothStep polynom = new SmoothStep(6);
            ReferencePointTurbulence turbulence = new ReferencePointTurbulence(refPts, polynom);

            //////////////////////////////////////////////////
            // Rendering
            /////////////////////////////////////////////////

            // Book-keeping
            double[,,,] noiseVal = new double[len0, len1, lent, len2];

            // Cycles through coherence rates
            Stopwatch watch = new Stopwatch();
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
                            double val = turbulence.GetVal(pt, time[tidx]);
                            watch.Stop();

                            // Records noise value
                            noiseVal[idx0, idx1, tidx, idx2] = val;
                        }
                    }
                }
            }

            Console.WriteLine("Reference Point Turbulence total time = {0}s", 
                watch.ElapsedMilliseconds / 1000.0);

            ///////////////////////////////////////////////////////////////////
            // Matlab processing
            ///////////////////////////////////////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                "OutputData\\Models\\ReferencePointTurbulence";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            // Outputs data
            MatLab.Put(matlab, "time", time);
            MatLab.Put(matlab, "noiseVal", noiseVal);
            MatLab.Put(matlab, "separation", separation);
            resStr = matlab.Execute("ReferencePointTurbulenceCheck");

            Console.WriteLine("\n Reference Point Turbulence Check Passed");

            return true;
        }
    }
}
