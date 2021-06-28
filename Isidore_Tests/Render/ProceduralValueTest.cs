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
    /// Tests the procedural value material
    /// </summary>
    class ProceduralValueTest
    {
        /// <summary>
        /// Tests that the noise models & procedural textures are working as 
        /// expected.
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Material Procedural Value Check");

            // Simulation time & sampling period
            double simTime = 5;
            double samplePeriod = 0.05;
            int totFrames = (int)(simTime / samplePeriod) + 1;

            // Orthonormal projector located on the origin
            int len0 = 140;
            int len1 = 120;
            //int len0 = 2;
            //int len1 = 2;
            var proj = new RectangleProjector(len0, len1, 0.01, 0.01, 0, 0);
            var pos0 = proj.Pos0;
            var pos1 = proj.Pos1;

            // Sets the camera 10m behind of the plane 
            var transProj1 = Transform.Translate(0, 0, -10);
            var transProj2 = Transform.Translate(0, 10, -10);
            //var transProj2 = Transform.Translate(0, 1, -10);
            proj.TransformTimeLine.AddKeys(transProj1, 0.0);
            proj.TransformTimeLine.AddKeys(transProj2, simTime);

            // Infinite plane
            Isidore.Render.Plane plane = new Isidore.Render.Plane(
                Point.Zero(3), -1.0 * Normal.Unit(3, 2));

            // Assembles scene
            var scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(plane);

            // Makes a Perlin fBm noise instance
            var noiseFunc = new PerlinNoiseFunction(1234, 10);
            var fBm = new fBmNoise(noiseFunc, 0.125, 512, 1.0/3.0, 2);

            // Creates a list of procedural data point
            List<ProceduralPoint> procPts = new List<ProceduralPoint>();
            double dPos = 2;
            double pos = 0;
            var shift = new Vector(new double[] { 12.3, 23.4, 34.5, 0 });
            var vel = new Vector(new double[] { 0, 0, 0, 1 });
            procPts.Add(new ProceduralPoint(new Point(0, pos, 0), vel, shift, 
                1, 10));
            pos += dPos;
            procPts.Add(new ProceduralPoint(new Point(0, pos, 0), vel, shift,
                50, 0));
            pos += dPos;
            procPts.Add(new ProceduralPoint(new Point(0, pos, 0), vel, shift, 
                1, 10));
            pos += dPos;
            procPts.Add(new ProceduralPoint(new Point(0, pos, 0), vel, shift,
                10, 0));
            pos += dPos;
            procPts.Add(new ProceduralPoint(new Point(0, pos, 0), vel, shift,
                1, 10));
            pos += dPos;
            procPts.Add(new ProceduralPoint(new Point(0, pos, 0), vel, shift,
                50, 0));


            // Uses the noise instance in a material
            bool anchorToBody = true;
            var procMat = new ProceduralValue(fBm, procPts, 0, 
                ProcInterp.Params, anchorToBody);

            // Adds the material to a material stack & to the plane
            var matStack = new MaterialStack(procMat);
            plane.Materials.Add(matStack);

            // Samples every 1/20 sec for 5 sec
            var time = Enumerable.Range(0, totFrames).
                Select(x => x * samplePeriod).ToArray();
            var len2 = time.Length;

            // Enumeration values
            var piStrs = Enum.GetNames(typeof(ProcInterp));
            var piVals = Enum.GetValues(typeof(ProcInterp));
            var piEnum = piVals as ProcInterp[];
            var len3 = piVals.Length;

            // Book-keeping
            var textArr = new double[len0, len1, len2, len3];
            var uArr = new double[len0, len1, len2, len3];
            var vArr = new double[len0, len1, len2, len3];

            var watch = new Stopwatch();

            // Cycles though interpolation options
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
                            var thisRay = proj.Ray(idx0, idx1).Rays[0];
                            var iData = thisRay.IntersectData;

                            // Extracts data if hit (Should always hit)
                            if (iData.Hit)
                            {
                                // Body specific data
                                var sData = thisRay.IntersectData.BodySpecificData
                                    as ShapeSpecificData;
                                uArr[idx0, idx1, tidx, iidx] = sData.U;
                                vArr[idx0, idx1, tidx, iidx] = sData.V;

                                // Texture properties
                                var unit = iData.GetPropertyData<string, Scalar>("Units");
                                var val = iData.GetPropertyData<double, Scalar>("Value");
                                textArr[idx0, idx1, tidx, iidx] = val;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Procedural value material trace, " +
                "total render time = {0}s", watch.ElapsedMilliseconds/1000.0);

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
            MatLab.Put(matlab, "textArr", textArr);
            MatLab.Put(matlab, "uArr", uArr);
            MatLab.Put(matlab, "vArr", vArr);
            MatLab.Put(matlab, "piStrs", piStrs);
            resStr = matlab.Execute("ProceduralValueCheck");

            Console.WriteLine("\n Procedural Value Check Passed");

            return true;
        }
    }
}
