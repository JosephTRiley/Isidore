using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Isidore.Render;
using Isidore.Maths;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing the sphere shape 
    /// </summary>
    class MatTraceRefractiveIndex
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Stopwatch watch = new Stopwatch();

            // Orthonormal projector located -10m from the shape
            //RectangleProjector proj = new RectangleProjector(180, 160, 0.01, 0.01, 0, 0);
            RectangleProjector proj = new RectangleProjector(1000, 500, 0.0025, 0.0025, 0, 0);
            //RectangleProjector proj = new RectangleProjector(1, 1, 0.0025, 0.0025, 0, 0); // Used to check ray tree
            proj.TransformTimeLine = new KeyFrameTrans(Transform.Translate(new double[] { 0, 0, -10 }));

            // one meter sphere, centered at the origin
            Isidore.Render.Sphere sphere = new Isidore.Render.Sphere(
                Isidore.Maths.Point.Zero(3), 0.5, Vector.Unit(3, 1), 
                -1.0*Vector.Unit(3, 2));

            // Adds billboard
            Billboard billboard = new Billboard(new Isidore.Maths.Point(-1, -1, 1),
                new Normal(0, 0, -1), new Vector(0, 1, 0), 2, 2);

            // Adds straight refractive index material layer to the sphere
            Transparency glass = new Transparency(1.05);
            glass.CastReflectedRays = false;
            MaterialStack transMats = new MaterialStack(glass);
            sphere.Materials.Add(transMats);

            // Adds a reflectance to the billboard to test GetPropertyData
            Reflectance SpecReflect = new Reflectance(new double[]
            { 0.0, 1.0e-6, 10.0e-6 }, new double[] 
            { 1.0 / Math.PI, 2.0 / Math.PI, 11.0 / Math.PI });
            Reflector sReflectance = new Reflector(SpecReflect);
            MaterialStack sReflMats = new MaterialStack(sReflectance);
            billboard.Materials.Add(sReflMats);

            // Adds a wavelength
            Wavelength wlen = new Wavelength(1.5e-6);
            foreach (RenderRay ray in proj.LocalRays)
                ray.Properties.Add(wlen);

            //  Adds the projector & shape to the scene
            Scene scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(sphere);
            scene.Bodies.Add(billboard);

            // Renders at t=0.0
            watch.Start();

            scene.AdvanceToTime(0.0);

            watch.Stop();
            Console.WriteLine("Material Trace Reflectance Render 1: {0}ms", watch.ElapsedMilliseconds);

            // Checks if AdvanceToTime is able to reject same times (via Item)
            //sphere.AdvanceToTime(0.0);

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
            int[,] rImg = new int[len0, len1];
            int[,] idImg = new int[len0, len1];
            // Cycles through ray tree to get, casting lets us fill in the blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    // Retrieves the ray tree for this pixel
                    RayTree thisTree = proj.Ray(idx0, idx1);
                    // And the ray (Could do just this step, was checking above)
                    RenderRay thisRay = thisTree.Rays.Last();

                    // Records position (World space)
                    x[idx0, idx1] = thisRay.Origin.Comp[0];
                    y[idx0, idx1] = thisRay.Origin.Comp[1];

                    // Checks to see if the ray has hit
                    if (thisRay.IntersectData.Hit)
                    {
                        bool hit = thisRay.IntersectData.GetFieldValue<bool>("Hit");
                        double depth = thisRay.IntersectData.GetFieldValue<double>("Travel");
                        int id = thisRay.IntersectData.GetFieldValue<int>("ID");
                        Body body = thisRay.IntersectData.GetFieldValue<Body>("Body");
                        double cosIncAng = thisRay.IntersectData.GetFieldValue<double>("CosIncAng");

                        intImg[idx0, idx1] = 1;
                        depthImg[idx0, idx1] = thisRay.IntersectData.Travel;
                        rImg[idx0, idx1] = (thisRay.IntersectData.Hit) ?
                            thisRay.Rank + 1 : thisRay.Rank;
                        idImg[idx0, idx1] = thisRay.IntersectData.Body.ID;

                        ShapeSpecificData sData = thisRay.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        if (sData != null)
                        {
                            cosIncImg[idx0, idx1] = sData.CosIncAng;
                            uImg[idx0, idx1] = sData.U;
                            vImg[idx0, idx1] = sData.V;
                        }
                    }
                }

            // Uses the get GetIntersectValue normally used with MatLab
            bool[,] Hit = proj.GetIntersectValue<bool>("Hit");
            double[,] Depth = proj.GetIntersectValue<double>("Travel");
            double[,] CosInc = proj.GetIntersectValue<double>("CosIncAng");
            int[,] ID = proj.GetIntersectValue<int>("ID");
            double[,] U = proj.GetIntersectValue<double>("U");
            double[,] V = proj.GetIntersectValue<double>("V");
            double[,] Reflect = proj.GetPropertyData<double, Reflectance>
                ("MeanCoeff");

            // MatLab processing
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Render\\MatTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "rayRank", rImg);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "id", idImg);
            MatLab.Put(matlab, "Hit", Hit);
            MatLab.Put(matlab, "Depth", Depth);
            MatLab.Put(matlab, "CosInc", CosInc);
            MatLab.Put(matlab, "ID", ID);
            MatLab.Put(matlab, "U", U);
            MatLab.Put(matlab, "V", V);
            MatLab.Put(matlab, "Reflect", Reflect);
            MatLab.Put(matlab, "x", x);
            MatLab.Put(matlab, "y", y);
            MatLab.Put(matlab, "pos0", proj.Pos0);
            MatLab.Put(matlab, "pos1", proj.Pos1);
            MatLab.Put(matlab, "renderTime", watch.Elapsed.Milliseconds);
            resStr = matlab.Execute("MatTraceTransCheck");

            return true;
        }
    }
}
