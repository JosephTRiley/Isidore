using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using Isidore.Render;
using Isidore.Maths;
using Isidore.Matlab;
using Isidore.ImgProcess;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing of the plane shape 
    /// </summary>
    class ShapeTraceBillboard
    {
        /// <summary>
        /// Runs test
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Stopwatch watch = new Stopwatch();

            // 1m square billboard
            // Positioned at the origin, point down the optical axis
            double ulen = 1.0;
            double vlen = 1.0;
            Isidore.Maths.Point centPt = new
                Isidore.Maths.Point(-0.5 * ulen, -0.5 * vlen, 0);
            Normal surfNorm = new Normal(0, 0, -1);
            Vector up = new Vector(0, 1, 0);

            Isidore.Render.Billboard billboard = new
                Isidore.Render.Billboard(centPt, surfNorm, up, ulen, vlen);

            // Maps an R to the reflection map
            Color[,] cR = ConvertImg.toColor(Isidore.Library.Images.R());
            double[,] dR = ConvertImg.toGrayScale<double>(cR);
            double[] minMaxR = Stats.MinMax(dR);
            dR = Operator.Divide(dR, minMaxR[1]);
            MapTexture reflectTexture = new MapTexture(dR);

            // Adds straight reflectance material layer to the sphere
            Reflector reflectance = new Reflector(0.5 / Math.PI);
            reflectance.Scalar = reflectTexture;
            MaterialStack reflMats = new MaterialStack(reflectance);
            billboard.Materials.Add(reflMats);

            // Infinite plane animation
            // All translate 5m in Z-Axis
            Transform trans0 = Transform.Translate(0, 0, 5);
            // Shift to other side, 180deg rotation
            Transform trans1 = Transform.Translate(0, 0, 5) * 
                Transform.RotY(Math.PI);
            // Shifts back, another 180deg rotation
            Transform trans2 = Transform.Translate(0, 0, 5) *
                Transform.RotY(2.0 * Math.PI);
            billboard.TransformTimeLine = new KeyFrameTrans();
            billboard.TransformTimeLine.AddKeys(trans0, 0.0);
            billboard.TransformTimeLine.AddKeys(trans1, 5.0);
            billboard.TransformTimeLine.AddKeys(trans2, 10.0);

            // Orthonormal projector 
            // Located -10m from the shape
            RectangleProjector proj = new RectangleProjector(120, 140, 
                0.01, 0.01, 0, 0);
            proj.TransformTimeLine = new
                KeyFrameTrans(Transform.Translate(new double[] { 0, 0, -10 }));

            // Projector rays' spectral reflectance property 
            Reflectance  specRefl = new Reflectance(
                new double[] { 1e-6, 2e-6, 3e-6 }, new double[] 
                { 0, 1/Math.PI, 0 });
            // Clones for testing
            Reflectance specReflCopy = specRefl.Clone();
            proj.AddProperty(specReflCopy);


            // Scene
            Scene scene = new Scene();
            //  Adds the projector & shape to the scene
            scene.Projectors.Add(proj);
            scene.Bodies.Add(billboard);

            // Rendering 
            // Sample times
            double[] times = Distribution.Increment(0.0, 11.0, 1.0);

            // Selects the third frame for turning off the back-face intersect
            int bfOffIdx = 4;

            // Data prep
            int len0 = proj.Pos0.Length;
            int len1 = proj.Pos1.Length;
            int len2 = times.Length;
            double[,] x = new double[len0, len1];
            double[,] y = new double[len0, len1];
            int[,,] intArr = new int[len0, len1, len2];
            double[,,] cosIncArr = new double[len0, len1, len2];
            double[,,] incAngArr = new double[len0, len1, len2];
            double[,,] depthArr = new double[len0, len1, len2];
            double[,,] uArr = new double[len0, len1, len2];
            double[,,] vArr = new double[len0, len1, len2];
            double[,,] rArr = new double[len0, len1, len2];

            // Cycles through time steps
            for (int idx = 0; idx < times.Length; idx++)
            {
                // For the third frame, shut off back face tracing
                if (idx == bfOffIdx)
                    billboard.IntersectBackFaces = false;
                else
                    billboard.IntersectBackFaces = true;

                // Render call
                watch.Reset();
                watch.Start();
                scene.AdvanceToTime(times[idx], true);
                watch.Stop();
                Console.WriteLine("Billboard Shape Trace Render, " +
                    "frame {0}: {1}ms", idx + 1, watch.ElapsedMilliseconds);

                // Extracts data
                // Cycles through ray tree
                for (int idx0 = 0; idx0 < len0; idx0++)
                {
                    for (int idx1 = 0; idx1 < len1; idx1++)
                    {
                        RenderRay thisRay = proj.Ray(idx0, idx1).Rays[0];

                        // Records position (World space) on first frame.
                        // Since the projector doesn't move, only need one
                        if (idx == 0)
                        {
                            x[idx0, idx1] = thisRay.Origin.Comp[0];
                            y[idx0, idx1] = thisRay.Origin.Comp[1];
                        }

                        // Checks to see if the ray has hit
                        IntersectData iData = thisRay.IntersectData;
                        ShapeSpecificData sData = iData.BodySpecificData as 
                            ShapeSpecificData;
                        if (thisRay.IntersectData.Hit)
                        {
                            intArr[idx0, idx1, idx] = 1;
                            cosIncArr[idx0, idx1, idx] = sData.CosIncAng;
                            incAngArr[idx0, idx1, idx] =
                                Math.Acos(sData.CosIncAng) * 180 / Math.PI;
                            depthArr[idx0, idx1, idx] = 
                                thisRay.IntersectData.Travel;
                            uArr[idx0, idx1, idx] = sData.U;
                            vArr[idx0, idx1, idx] = sData.V;

                            // Properties extraction
                            int index = 0; // Indexer
                            for (int ridx = 0; ridx < 
                                thisRay.IntersectData.Properties.Count; ridx++)
                            {
                                // Extracts type
                                string pType = thisRay.IntersectData.Properties[ridx].
                                    GetType().ToString();

                                if (pType == "Isidore.Render.Reflectance")
                                {
                                    Reflectance thisReflect = (Reflectance)
                                        thisRay.IntersectData.Properties[ridx];
                                    rArr[idx0, idx1, idx] = thisReflect.Coefficient[0];
                                }
                                index++;
                            }
                        }
                    }
                }
            }

            //////////////////////////////////////
            // MatLab display
            /////////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + 
                "OutputData\\Render\\ShapeTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            MatLab.Put(matlab, "reflectMap", dR);
            MatLab.Put(matlab, "intArr", intArr);
            MatLab.Put(matlab, "cosIncArr", cosIncArr);
            MatLab.Put(matlab, "incAngArr", incAngArr);
            MatLab.Put(matlab, "depthArr", depthArr);
            MatLab.Put(matlab, "uArr", uArr);
            MatLab.Put(matlab, "vArr", vArr);
            MatLab.Put(matlab, "rArr", rArr);
            MatLab.Put(matlab, "reflectMap", dR);
            //matlab.PutWorkspaceData("alphaMap", "base", iR);
            matlab.PutWorkspaceData("pos0", "base", proj.Pos0);
            matlab.PutWorkspaceData("pos1", "base", proj.Pos1);
            matlab.PutWorkspaceData("bfOffIdx", "base", bfOffIdx);
            matlab.Execute("ShapeTraceBillboardCheck;");

            return true;
        }
    }
}
