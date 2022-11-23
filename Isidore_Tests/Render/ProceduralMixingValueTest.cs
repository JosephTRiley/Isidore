using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests the procedural mixing value material
    /// </summary>
    class ProceduralMixingValueTest
    {
        /// <summary>
        /// Tests the procedural mixing value material is working as 
        /// expected.
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Material Procedural Mixing Value Check");

            // Simulation time & sampling period
            double simTime = 5;
            double samplePeriod = 0.05;
            int totFrames = (int)(simTime / samplePeriod) + 1;
            double[] time = Enumerable.Range(0, totFrames).
                Select(x => x * samplePeriod).ToArray();
            int len2 = time.Length;

            // Orthonormal projector located on the origin
            int len0 = 140;
            int len1 = 120;
            RectangleProjector proj = new RectangleProjector(len0, len1, 0.01, 0.01, 0, 0);
            double[] pos0 = proj.Pos0;
            double[] pos1 = proj.Pos1;

            // Infinite plane
            Isidore.Render.Plane surface = new Isidore.Render.Plane(new Point(0, 0, 10),
                -1.0 * Normal.Unit(3, 2), Vector.Unit(3, 1));

            // Assembles scene
            Scene scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(surface);

            // Makes a Perlin fBm noise instance
            PerlinNoiseFunction noiseFunc = new PerlinNoiseFunction(1234, 10);
            fBmNoise fBm = new fBmNoise(noiseFunc, 0.125, 512, 1.0 / 3.0, 2);

            // Creates a list of procedural data point
            List<ProceduralPoint> procPts = new List<ProceduralPoint>();
            Point coordPt0 = new Point(0, -0.4, 0);
            Point coordPt1 = new Point(0, 0.4, 0);
            Vector shift = new Vector(new double[] { 12.3, 23.4, 34.5, 0 });
            Vector vel0 = new Vector(new double[] { 2.0, 0, 0, 0 });
            Vector vel1 = new Vector(new double[] { 0.5, 0, 0, 0 });
            procPts.Add(new ProceduralPoint(coordPt0, vel0, shift, 1, 0));
            procPts.Add(new ProceduralPoint(coordPt1, vel1, shift, 1, 0));

            // Makes a Perlin Turbulence instance
            PerlinNoiseFunction noiseFunc1 = new PerlinNoiseFunction(2345, 10);
            bool absValNoise = false; // 0 = shift on both sides
            Vector perturbVel = new Vector(1, 0, 3);
            Vector shiftP = new Vector(23.4, 34.5, 45.6);
            //double scaleP = 0.1;
            double scaleP = 0.5;
            NoiseParameters pertParams = new NoiseParameters(shiftP, scaleP);
            //var turb = new PerlinTurbulenceNoise(noiseFunc1, 0.125, 512,
            //    absValNoise, 2, pertParams);
            //var turb = new PerlinTurbulenceNoise(noiseFunc1, 16, 64,
            //    absValNoise, 2, pertParams);
            PerlinTurbulenceNoise turb = new PerlinTurbulenceNoise(noiseFunc1, 4, 64,
                absValNoise, 2, pertParams);

            // Uses the noise instance in a material
            Noise noise = fBm; // Base noise 
            Noise perturbNoise = turb; // Perturbation noise
            int polyOrder = 2; // Smoothstep order for interpolating
            ProcInterp interp = ProcInterp.Params; // Interpolation method
            double perturbSmoothFrac = 2; // Fraction of unit dist to smooth
            int perturbPolyOrder = 2; // Perturbation smoothing order
            ProcMix perturbMix = ProcMix.Remap; // Mixing method
            bool anchorToBody = true; // Anchors the texture to the body
            ProceduralMixingValue procMat = new ProceduralMixingValue(noise, procPts, 
                perturbNoise, polyOrder, interp, perturbSmoothFrac, 
                perturbPolyOrder, perturbMix, perturbVel, anchorToBody);

            // Adds the material to a material stack & to the plane
            MaterialStack matStack = new MaterialStack(procMat);
            surface.Materials.Add(matStack);

            // Enumerated procedural interpolation values
            string[] piStrs = Enum.GetNames(typeof(ProcInterp));
            Array piVals = Enum.GetValues(typeof(ProcInterp));
            ProcInterp[] piEnum = piVals as ProcInterp[];
            int len3 = piVals.Length;

            // Enumerated procedural mixing values
            string[] pmStrs = Enum.GetNames(typeof(ProcMix));
            Array pmVals = Enum.GetValues(typeof(ProcMix));
            ProcMix[] pmEnum = pmVals as ProcMix[];
            int len4 = pmVals.Length;

            // Book-keeping
            double[,,,] textArr1 = new double[len0, len1, len2, len3];
            double[,,,] textArr2 = new double[len0, len1, len2, len3];
            double[,,] uArr = new double[len0, len1, len2];
            double[,,] vArr = new double[len0, len1, len2];

            Stopwatch watch = new Stopwatch();

            // Cycles through procedural mixing
            for (int midx = 0; midx < len4; midx++)
            { 
                procMat.mixMeth = pmEnum[midx];

                // Cycles through procedural interpolation options
                for (int iidx = 0; iidx < len3; iidx++)
                {
                    // Sets which interpolation method to use
                    procMat.interp = piEnum[iidx];

                    // Cycles through each time step
                    for (int tidx = 0; tidx < time.Length; tidx++)
                    {
                        // Runs time stepRetrieves the ray
                        watch.Start();
                        scene.AdvanceToTime(time[tidx], true);
                        watch.Stop();

                        // Extracts data
                        // Cycles through ray tree
                        for (int idx0 = 0; idx0 < len0; idx0++)
                        {
                            for (int idx1 = 0; idx1 < len1; idx1++)
                            {
                                RenderRay thisRay = proj.Ray(idx0, idx1).Rays[0];
                                IntersectData iData = thisRay.IntersectData;

                                // Extracts data if hit (Should always hit)
                                if (iData.Hit)
                                {
                                    // Body specific data
                                    ShapeSpecificData sData = thisRay.IntersectData.BodySpecificData
                                        as ShapeSpecificData;

                                    if (iidx == 0 && midx == 0)
                                    {
                                        uArr[idx0, idx1, tidx] = sData.U;
                                        vArr[idx0, idx1, tidx] = sData.V;
                                    }

                                    // Texture properties
                                    string unit = iData.GetPropertyData<string, Scalar>("Units");
                                    double val = iData.GetPropertyData<double, Scalar>("Value");
                                    if (midx == 0)
                                        textArr1[idx0, idx1, tidx, iidx] = val;
                                    else
                                        textArr2[idx0, idx1, tidx, iidx] = val;
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Procedural value material trace, " +
                "total render time = {0}s", watch.ElapsedMilliseconds / 1000.0);

            ///////////////////////////////////////////////////////////////////
            // Matlab processing
            ///////////////////////////////////////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                "OutputData\\Render\\ProceduralTexture";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            // Outputs data
            MatLab.Put(matlab, "time", time);
            MatLab.Put(matlab, "pos0", pos0);
            MatLab.Put(matlab, "pos1", pos1);
            MatLab.Put(matlab, "textArr1", textArr1);
            MatLab.Put(matlab, "textArr2", textArr2);
            MatLab.Put(matlab, "uArr", uArr);
            MatLab.Put(matlab, "vArr", vArr);
            MatLab.Put(matlab, "piStrs", piStrs);
            MatLab.Put(matlab, "pmStrs", pmStrs);
            resStr = matlab.Execute("ProceduralMixingValueCheck");

            Console.WriteLine("\n Procedural Mixing Value Check Passed");

            return true;
        }
    }
}
