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
    /// Tests ray tracing the sphere shape 
    /// </summary>
    class ShapeTraceSphere
    {
        /// <summary>
        /// Runs test
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            //////////////
            // Render 1 //
            //////////////
            
            // Checks incident data

            Stopwatch watch = new Stopwatch();

            // one meter sphere
            Isidore.Render.Sphere sphere = new Isidore.Render.Sphere(new
                Isidore.Maths.Point(0, 0, 0), 0.5);
            //Isidore.Render.Sphere sphere = new Isidore.Render.Sphere();

            // Orthonormal projector located -10m from the shape
            RectangleProjector proj = new RectangleProjector(120, 140, 0.01, 0.01, 0, 0);
            proj.TransformTimeLine = new KeyFrameTrans(Transform.Translate(new double[]{0,0,-10}));

            // Adds a list of properties to each render ray
            // Initial reflectance (E.G., Using a polarizer)
            //Reflectance reflectance = new Reflectance(0.5);
            // Generic refractive index
            //SpectralRefractIndex specRefractProp = new SpectralRefractIndex();
            // Spectral Reflectance (probably should be 1 instead of 1/pi)
            Reflectance  specRefl = new Reflectance(
                new double[] { 1e-6, 2e-6, 3e-6 }, new double[] { 0, 1/Math.PI, 0 });
            // Clones for testing
            Reflectance specReflCopy = (Reflectance)specRefl.Clone();

            //proj.AddProperty(reflectance);
            //proj.AddProperty(specRefractProp);
            //proj.AddProperty(specRefl);
            proj.AddProperty(specReflCopy);

            // Checks type
            Type[] propTypes = new Type[proj.LocalRays[0].Properties.Count];
            for (int idx = 0; idx < propTypes.Length; idx++)
                propTypes[idx] = proj.LocalRays[0].Properties[idx].GetType();

            // Adds the projector & shape to the scene
            Scene scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(sphere);

            // Renders at t=0.0
            watch.Start();

            scene.AdvanceToTime(0.0);

            watch.Stop();
            Console.WriteLine("Sphere Shape Trace Render 1: {0}ms", watch.ElapsedMilliseconds);

            // Checks if AdvanceToTime is able to reject same times (via Item)
            sphere.AdvanceToTime(0.0);

            // Retrieves data
            int len0 = proj.Pos0.Length;
            int len1 = proj.Pos1.Length;
            double[,] x = new double[len0, len1];
            double[,] y = new double[len0, len1];
            int[,] intImg = new int[len0, len1];
            double[,] cosIncImg = new double[len0, len1];
            double[,] depthImg = new double[len0, len1];
            double[,] uImg = new double[len0, len1];
            double[,] vImg = new double[len0, len1];

            // Cycles through ray tree to get, casting lets us fill in the blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    // Retrieves the ray tree for this pixel
                    RayTree thisTree = proj.Ray(idx0, idx1);
                    // And the ray (Could do just this step, was checking above)
                    RenderRay thisRay = proj.Ray(idx0, idx1).Rays[0];

                    // Records position (World space)
                    x[idx0, idx1] = thisRay.Origin.Comp[0];
                    y[idx0, idx1] = thisRay.Origin.Comp[1];

                    // Checks to see if the ray has hit
                    if (thisRay.IntersectData.Hit)
                    {
                        intImg[idx0, idx1] = 1;
                        depthImg[idx0, idx1] = thisRay.IntersectData.Travel;

                        var sData = thisRay.IntersectData.BodySpecificData 
                            as ShapeSpecificData;

                        cosIncImg[idx0, idx1] = sData.CosIncAng;
                        uImg[idx0, idx1] = sData.U;
                        vImg[idx0, idx1] = sData.V;
                    }
                }

            Figure fig1 = new Figure();
            fig1.Disp(vImg, "v Coordinate, Step 3");

            // MatLab processing
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Render\\ShapeTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "x", x);
            MatLab.Put(matlab, "y", y);
            MatLab.Put(matlab, "pos0", proj.Pos0);
            MatLab.Put(matlab, "pos1", proj.Pos1);
            resStr = matlab.Execute("ShapeTraceSphereCheck1");


            //////////////
            // Render 2 //
            //////////////

            // Repeats the first render with both the projector and sphere overlapping at the origin

            // Checks incident data

            watch = new Stopwatch();

            // Orthonormal projector located -10m from the shape
            proj.TransformTimeLine = new KeyFrameTrans(Transform.Translate(new double[] { 0, 0, 0 }));

            // Checks type
            propTypes = new Type[proj.LocalRays[0].Properties.Count];
            for (int idx = 0; idx < propTypes.Length; idx++)
                propTypes[idx] = proj.LocalRays[0].Properties[idx].GetType();

            // Renders at t=0.0, Must force since the internal time hasn't changed
            watch.Start();
            scene.AdvanceToTime(0.0, true);
            watch.Stop();
            Console.WriteLine("Sphere Shape Trace Render 1: {0}ms", watch.ElapsedMilliseconds);

            // Checks if AdvanceToTime is able to reject same times (via Item)
            sphere.AdvanceToTime(0.0);

            // Retrieves data
            x = new double[len0, len1];
            y = new double[len0, len1];
            intImg = new int[len0, len1];
            cosIncImg = new double[len0, len1];
            depthImg = new double[len0, len1];
            uImg = new double[len0, len1];
            vImg = new double[len0, len1];

            // Cycles through ray tree to get, casting lets us fill in the blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    // Retrieves the ray tree for this pixel
                    RayTree thisTree = proj.Ray(idx0, idx1);
                    // And the ray (Could do just this step, was checking above)
                    RenderRay thisRay = proj.Ray(idx0, idx1).Rays[0];

                    // Records position (World space)
                    x[idx0, idx1] = thisRay.Origin.Comp[0];
                    y[idx0, idx1] = thisRay.Origin.Comp[1];

                    // Checks to see if the ray has hit
                    if (thisRay.IntersectData.Hit)
                    {
                        intImg[idx0, idx1] = 1;
                        depthImg[idx0, idx1] = thisRay.IntersectData.Travel;

                        var sData = thisRay.IntersectData.BodySpecificData
                            as ShapeSpecificData;

                        cosIncImg[idx0, idx1] = sData.CosIncAng;
                        uImg[idx0, idx1] = sData.U;
                        vImg[idx0, idx1] = sData.V;
                    }
                }

            Figure fig1a = new Figure();
            fig1a.Disp(vImg, "v Coordinate, Step 3");

            // MatLab processing
            resStr = matlab.Execute("clear;");
            // Outputs data
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "x", x);
            MatLab.Put(matlab, "y", y);
            MatLab.Put(matlab, "pos0", proj.Pos0);
            MatLab.Put(matlab, "pos1", proj.Pos1);
            resStr = matlab.Execute("ShapeTraceSphereCheck2");


            //////////////
            // Render 3 //
            //////////////

            // Checks the key-framing

            // Sample times
            double[] times = Distribution.Increment(0.0, 11.0, 1.0);
            int len2 = times.Length;

            // Resizes radius
            //double[] rTime = { 0.0, 2.5, 5.0 };
            //double[] rSize = { 0.25, 0.5, 0.25 };
            //KeyFrame<double> radius = new KeyFrame<double>(rSize, rTime);

            // Sphere moving left to right, rotating by 180 degrees
            //sphere = new Isidore.Render.Sphere(new Point(0, 0, 0), radius);
            sphere = new Isidore.Render.Sphere(new Isidore.Maths.Point(0, 0, 0), 0.25);
            Transform trans0 = Transform.Translate(-0.5,0,0);
            // Shift to other side, 180deg rotation
            Transform shift1 = Transform.Translate(0.5,0,0);
            Transform rot1 = Transform.RotY(Math.PI);
            Transform trans1 = shift1 * rot1;
            // Shifts back, another 180deg rotation
            Transform shift2 = Transform.Translate(-0.5, 0, 0);
            Transform rot2 = Transform.RotY(2.0*Math.PI);
            Transform trans2 = shift2 * rot2;
            sphere.TransformTimeLine.AddKeys(trans0, 0.0);
            sphere.TransformTimeLine.AddKeys(trans1, 5.0);
            sphere.TransformTimeLine.AddKeys(trans2, 10.0);

            // Resizes radius
            double[] rTime = {0.0, 2.5, 5.0};
            double[] rSize = {0.25, 0.5, 0.25};
            KeyFrame<double> radius = new KeyFrame<double>(rSize, rTime);
            sphere.radius = radius;

            // Adds alpha mapping
            Color[,] cR = ConvertImg.toColor(Isidore.Library.Images.R());
            double[,] dR = ConvertImg.toGrayScale<double>(cR);
            MapTexture alphaMap = new MapTexture(dR);
            sphere.Alpha = alphaMap;

            // Removes old sphere, adds new one
            scene.Bodies.RemoveAt(0);
            scene.Bodies.Add(sphere);

            // Book-keeping
            int[,,] intArr = new int[len0, len1,len2];
            double[,,] cosIncArr = new double[len0, len1,len2];
            double[,,] depthArr = new double[len0, len1,len2];
            double[,,] uArr = new double[len0, len1,len2];
            double[,,] vArr = new double[len0, len1,len2];
            double[,] u3 = new double[len0, len1];

            // Cycles through time steps
            int bfOffIdx = 7;
            Figure fig2 = new Figure();
            for (int idx = 0; idx < times.Length;idx++ )
            {
                // For the third frame, shut off back face tracing
                if (idx == bfOffIdx)
                    sphere.IntersectBackFaces = false;
                else
                    sphere.IntersectBackFaces = true;

                // Retrieves the ray
                watch.Reset(); 
                watch.Start();

                scene.AdvanceToTime(times[idx], true);

                watch.Stop();
                Console.WriteLine("Sphere Shape Trace Render 2, render {0}: {1}ms", idx + 1, watch.ElapsedMilliseconds);

                // Displays frame
                double[,] frame2 = Arr.Extract(vArr, 3);
                for (int idx0 = 0; idx0 < len0; idx0++)
                    for (int idx1 = 0; idx1 < len1; idx1++)
                    {
                        var sData = proj.Ray(idx0, idx1).Rays[0].IntersectData.
                            BodySpecificData as ShapeSpecificData;
                        if (sData != null)
                            frame2[idx0, idx1] = sData.V;
                    }
                fig2.Disp(frame2, "Frame: " + idx);

                // Extracts data
                // Cycles through ray tree
                for (int idx0 = 0; idx0 < len0; idx0++)
                {
                    for (int idx1 = 0; idx1 < len1; idx1++)
                    {
                        RenderRay thisRay = proj.Ray(idx0, idx1).Rays[0];

                        // Checks to see if the ray has hit
                        if (thisRay.IntersectData.Hit)
                        {
                            intArr[idx0, idx1, idx] = 1;
                            depthArr[idx0, idx1, idx] = 
                                thisRay.IntersectData.Travel;

                            var sData = thisRay.IntersectData.BodySpecificData
                                as ShapeSpecificData;
                            cosIncArr[idx0, idx1, idx] = sData.CosIncAng;
                            uArr[idx0, idx1, idx] = sData.U;
                            vArr[idx0, idx1, idx] = sData.V;
                        }
                    }
                }
            }

            // Displays 3rd frame
            double[,] frame3 = Arr.Extract(vArr, 3);
            Figure fig3 = new Figure();
            fig3.Disp(frame3, "V Coordinate, Step 3");

            // MatLab display
            resStr = matlab.Execute("clear;");
            MatLab.Put(matlab, "dArr", depthArr);
            MatLab.Put(matlab, "cArr", cosIncArr);
            MatLab.Put(matlab, "uArr", uArr);
            MatLab.Put(matlab, "vArr", vArr);
            MatLab.Put(matlab, "alphaMap", dR);
            //matlab.PutWorkspaceData("alphaMap", "base", iR);
            matlab.PutWorkspaceData("pos0", "base", proj.Pos0);
            matlab.PutWorkspaceData("pos1", "base", proj.Pos1);
            matlab.PutWorkspaceData("bfOffIdx", "base", bfOffIdx);
            matlab.Execute("ShapeTraceSphereCheck3;");

            return true;
        }
    }
}
