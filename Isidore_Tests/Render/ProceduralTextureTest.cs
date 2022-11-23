using System;
using System.Linq;
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
    class ProceduralTextureTest
    {
        /// <summary>
        /// Tests that the noise models & procedural textures are working as 
        /// expected
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Procedural Texture Function Check");

            ///////////////////////////////////////////////////////////////////
            // 1: Projector images an infinite phase screen with a Perlin fBm
            // texture moving perpendicular to the optical axis
            ///////////////////////////////////////////////////////////////////

            // Orthonormal projector located on the origin
            int len0 = 140;
            int len1 = 120;
            RectangleProjector proj = new RectangleProjector(len0, len1, 0.01, 0.01, 0, 0);
            double[] pos0 = proj.Pos0;
            double[] pos1 = proj.Pos1;

            // Infinite plane
            Isidore.Render.Plane plane = new Isidore.Render.Plane(
                Point.Zero(3), -1.0 * Normal.Unit(3, 2));

            // Assembles scene
            Scene scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(plane);

            // Makes a Perlin fBm texture
            PerlinNoiseFunction noiseFunc = new PerlinNoiseFunction(1234, 10);
            fBmNoise fBm1 = new fBmNoise(noiseFunc, 0.125, 512, 1.0/3.0, 2);
            ProceduralTexture fBmText = new ProceduralTexture(fBm1);

            // Adds the texture to a material stack and to the plane
            // Terminates ray at intersection
            // Anchors the texture to the body local coordinate & not to
            // global space
            TextureValue textVal = new TextureValue(fBmText, true);
            // This will anchor the texture to the global coordinate
            // so the texture will not change as the plane shifts
            //var textVal = new TextureValue(fBmText, true, false);
            MaterialStack matStack = new MaterialStack(textVal);
            plane.Materials.Add(matStack);

            // Sets the plane 10m in front of the camera, 
            // moving at 1 m/s along the X-axis
            Transform trans1 = Transform.Translate(0, 0, 10);
            Transform trans2 = Transform.Translate(2, 0, 10);
            plane.TransformTimeLine.AddKeys(trans1, 0.0);
            plane.TransformTimeLine.AddKeys(trans2, 2);

            // Samples every 1/20 sec for 2 sec
            double[] time = Enumerable.Range(0, 41).
                Select(x => x / 20.0).ToArray();
            int len2 = time.Length;

            // Book-keeping
            double[,,] textArr1 = new double[len0, len1, len2];
            double[,,] travelArr1 = new double[len0, len1, len2];
            int[,,] intArr1 = new int[len0, len1, len2];
            double[,,] uArr1 = new double[len0, len1, len2];
            double[,,] vArr1 = new double[len0, len1, len2];
            double[,,] i0Arr1 = new double[len0, len1, len2];
            double[,,] i1Arr1 = new double[len0, len1, len2];
            double[,,] i2Arr1 = new double[len0, len1, len2];

            // Cycles through each time step
            Stopwatch watch = new Stopwatch();
            for (int idx = 0; idx < time.Length; idx++)
            {
                // Runs time stepRetrieves the ray
                watch.Start();

                scene.AdvanceToTime(time[idx], true);

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
                            // Standard intersection data
                            intArr1[idx0, idx1, idx] =
                                Operator.Convert<bool, int>(iData.Hit);
                            travelArr1[idx0, idx1, idx] = iData.Travel;
                            i0Arr1[idx0, idx1, idx] = iData.IntersectPt.Comp[0];
                            i1Arr1[idx0, idx1, idx] = iData.IntersectPt.Comp[1];
                            i2Arr1[idx0, idx1, idx] = iData.IntersectPt.Comp[2];

                            // Body specific data
                            ShapeSpecificData sData = thisRay.IntersectData.BodySpecificData
                                as ShapeSpecificData;
                            uArr1[idx0, idx1, idx] = sData.U;
                            vArr1[idx0, idx1, idx] = sData.V;

                            // Texture properties
                            string unit = iData.GetPropertyData<string,Scalar>("Units");
                            double val = iData.GetPropertyData<double,Scalar>("Value");
                            textArr1[idx0, idx1, idx] = val;
                        }
                    }
                }
            }

            Console.WriteLine("Transverse texture material trace, " +
                "total render time = {0}s", watch.ElapsedMilliseconds/1000.0);


            ///////////////////////////////////////////////////////////////////
            // 3: Same as 1, but there is a time shift in the 4th dimension
            ///////////////////////////////////////////////////////////////////

            // This sets the time shift
            textVal.useTimeShift = true;
            textVal.timeSegmentLength = 1.0;

            // Book-keeping
            double[,,] textArr3 = new double[len0, len1, len2];
            double[,,] travelArr3 = new double[len0, len1, len2];
            int[,,] intArr3 = new int[len0, len1, len2];
            double[,,] uArr3 = new double[len0, len1, len2];
            double[,,] vArr3 = new double[len0, len1, len2];
            double[,,] i0Arr3 = new double[len0, len1, len2];
            double[,,] i1Arr3 = new double[len0, len1, len2];
            double[,,] i2Arr3 = new double[len0, len1, len2];

            // Cycles through each time step
            watch.Reset();
            for (int idx = 0; idx < time.Length; idx++)
            {
                // Runs time stepRetrieves the ray
                watch.Start();

                scene.AdvanceToTime(time[idx], true);

                watch.Stop();

                // Extracts data
                bool[,] hit = proj.GetIntersectValue<bool>("Hit");
                double[,] travel = proj.GetIntersectValue<double>("Travel");
                Point[,] pt = proj.GetIntersectValue<Point>("IntersectPt");
                double[,] u = proj.GetIntersectValue<double>("U");
                double[,] v = proj.GetIntersectValue<double>("V");
                double[,] text = proj.GetPropertyData<double, Scalar>("Value");

                // Stores data
                // Cycles through ray tree
                for (int idx0 = 0; idx0 < len0; idx0++)
                    for (int idx1 = 0; idx1 < len1; idx1++)
                        // Records data if hit (Should always hit)
                        if (hit[idx0, idx1])
                        {
                            // Standard intersection data
                            intArr3[idx0, idx1, idx] =
                                Operator.Convert<bool, int>(hit[idx0, idx1]);
                            travelArr3[idx0, idx1, idx] = travel[idx0, idx1];
                            i0Arr3[idx0, idx1, idx] = pt[idx0, idx1].Comp[0];
                            i1Arr3[idx0, idx1, idx] = pt[idx0, idx1].Comp[1];
                            i2Arr3[idx0, idx1, idx] = pt[idx0, idx1].Comp[2];
                            uArr3[idx0, idx1, idx] = u[idx0, idx1];
                            vArr3[idx0, idx1, idx] = v[idx0, idx1];
                            textArr3[idx0, idx1, idx] = text[idx0, idx1];
                        }
            }

            Console.WriteLine("Time shift texture material trace, " +
                "total render time = {0}s", watch.ElapsedMilliseconds / 1000.0);

            ///////////////////////////////////////////////////////////////////
            // 2: Same as 1, but the plane's motion is moving along the
            // optical axis
            ///////////////////////////////////////////////////////////////////

            // Sets the plane 10m in front of the camera, 
            // moving at 1m/s  along the X
            trans2 = Transform.Translate(0, 0, 12);
            plane.TransformTimeLine = new KeyFrameTrans();
            plane.TransformTimeLine.AddKeys(trans1, 0.0);
            plane.TransformTimeLine.AddKeys(trans2, 2);

            // This will anchor the texture to the global coordinate
            // so the texture will not change as the plane shifts
            textVal.anchorTextureToBody = false;

            // Shuts off the time shift
            textVal.useTimeShift = false;

            // Book-keeping
            double[,,] textArr2 = new double[len0, len1, len2];
            double[,,] travelArr2 = new double[len0, len1, len2];
            int[,,] intArr2 = new int[len0, len1, len2];
            double[,,] uArr2 = new double[len0, len1, len2];
            double[,,] vArr2 = new double[len0, len1, len2];
            double[,,] i0Arr2 = new double[len0, len1, len2];
            double[,,] i1Arr2 = new double[len0, len1, len2];
            double[,,] i2Arr2 = new double[len0, len1, len2];

            // Cycles through each time step
            watch.Reset();
            for (int idx = 0; idx < time.Length; idx++)
            {
                // Runs time stepRetrieves the ray
                watch.Start();

                scene.AdvanceToTime(time[idx], true);

                watch.Stop();

                // Extracts data
                bool[,] hit = proj.GetIntersectValue<bool>("Hit");
                double[,] travel = proj.GetIntersectValue<double>("Travel");
                Point[,] pt = proj.GetIntersectValue<Point>("IntersectPt");
                double[,] u = proj.GetIntersectValue<double>("U");
                double[,] v = proj.GetIntersectValue<double>("V");
                double[,] text = proj.GetPropertyData<double, Scalar>("Value");

                // Stores data
                // Cycles through ray tree
                for (int idx0 = 0; idx0 < len0; idx0++)
                    for (int idx1 = 0; idx1 < len1; idx1++)
                        // Records data if hit (Should always hit)
                        if (hit[idx0, idx1])
                        {
                            // Standard intersection data
                            intArr2[idx0, idx1, idx] =
                                Operator.Convert<bool, int>(hit[idx0, idx1]);
                            travelArr2[idx0, idx1, idx] = travel[idx0, idx1];
                            i0Arr2[idx0, idx1, idx] = pt[idx0, idx1].Comp[0];
                            i1Arr2[idx0, idx1, idx] = pt[idx0, idx1].Comp[1];
                            i2Arr2[idx0, idx1, idx] = pt[idx0, idx1].Comp[2];
                            uArr2[idx0, idx1, idx] = u[idx0, idx1];
                            vArr2[idx0, idx1, idx] = v[idx0, idx1];
                            textArr2[idx0, idx1, idx] = text[idx0, idx1];
                        }
            }

            Console.WriteLine("In-line texture material trace, " +
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
            MatLab.Put(matlab, "intArr1", intArr1);
            MatLab.Put(matlab, "travelArr1", travelArr1);
            MatLab.Put(matlab, "i0Arr1", i0Arr1);
            MatLab.Put(matlab, "i1Arr1", i1Arr1);
            MatLab.Put(matlab, "i2Arr1", i2Arr1);
            MatLab.Put(matlab, "textArr1", textArr1);
            MatLab.Put(matlab, "uArr1", uArr1);
            MatLab.Put(matlab, "vArr1", vArr1);
            MatLab.Put(matlab, "intArr2", intArr2);
            MatLab.Put(matlab, "travelArr2", travelArr2);
            MatLab.Put(matlab, "i0Arr2", i0Arr2);
            MatLab.Put(matlab, "i1Arr2", i1Arr2);
            MatLab.Put(matlab, "i2Arr2", i2Arr2);
            MatLab.Put(matlab, "textArr2", textArr2);
            MatLab.Put(matlab, "uArr2", uArr2);
            MatLab.Put(matlab, "vArr2", vArr2);
            MatLab.Put(matlab, "intArr3", intArr3);
            MatLab.Put(matlab, "travelArr3", travelArr3);
            MatLab.Put(matlab, "i0Arr3", i0Arr3);
            MatLab.Put(matlab, "i1Arr3", i1Arr3);
            MatLab.Put(matlab, "i2Arr3", i2Arr3);
            MatLab.Put(matlab, "textArr3", textArr3);
            MatLab.Put(matlab, "uArr3", uArr3);
            MatLab.Put(matlab, "vArr3", vArr3);
            resStr = matlab.Execute("ProceduralTextureCheck");

            Console.WriteLine("\n Procedural Texture Check Passed");

            return true;
        }
    }
}
