using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Isidore.Render;
using Isidore.Maths;
using Isidore.Matlab;
using Isidore.Load;
using System.Xml.Linq;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing the sphere shape 
    /// </summary>
    class MatTraceReflective
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Stopwatch watch = new Stopwatch();

            // Orthonormal projector, located -10m back and -10m off-axis
            RectangleProjector proj = new RectangleProjector(480, 560,
                0.0025, 0.0025, 0, 0);
            Isidore.Maths.Point offPtProj = new Isidore.Maths.Point(new
                double[] { -10, 0, -10 });
            Transform lookAtProj = Transform.LookAt(offPtProj,
                Isidore.Maths.Point.Zero(), Vector.Unit(3, 1));
            proj.TransformTimeLine = new KeyFrameTrans(lookAtProj);

            // R 3D model, located -10m back and 10m off-axis
            Mesh R = Isidore.Library.Models.R();
            Polyshape mesh = new Polyshape(R);
            Isidore.Maths.Point offPtR = new Isidore.Maths.Point(new
                double[] { 10 - 0.5, -0.5, -10 });
            Transform lookAtR = Transform.LookAt(offPtR,
                Isidore.Maths.Point.Zero(), Vector.Unit(3, 1));
            mesh.TransformTimeLine = new KeyFrameTrans(lookAtR);
            mesh.Shapes[0].ID = 3;
            
            // Adds plane using a billboard
            Billboard plane = new Billboard(new Isidore.Maths.Point(-1, -1, 0),
                new Normal(0, 0, -1), new Vector(0, 1, 0), 2, 2);

            // Adds straight refractive index material layer to the plane
            Reflective reflective = new Reflective();
            MaterialStack planeMats = new MaterialStack(reflective);
            plane.Materials.Add(planeMats);

            // Adds a reflectance to the R to test GetPropertyData
            Reflectance reflect = new Reflectance(new double[]
            { 0.0, 1.0e-6, 10.0e-6 }, new double[] 
            { 1.0 / Math.PI, 2.0 / Math.PI, 11.0 / Math.PI });
            Reflector reflector = new Reflector(reflect);
            MaterialStack rMats = new MaterialStack(reflector);
            R.Materials.Add(rMats);

            //  Adds the projector & shape to the scene
            Scene scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(plane);
            scene.Bodies.Add(mesh);

            // Renders at t=0.0
            watch.Start();

            scene.AdvanceToTime(0.0);

            watch.Stop();
            Console.WriteLine("Material Trace Reflective Render: {0}ms", watch.ElapsedMilliseconds);

            // Checks if AdvanceToTime is able to reject same times (via Item)
            //sphere.AdvanceToTime(0.0);

            // Retrieves data
            double[] pos0 = proj.Pos0;
            double[] pos1 = proj.Pos1;
            int len0 = pos0.Length;
            int len1 = pos1.Length;
            int len2 = scene.Bodies.Count;
            int[,,] intImg = new int[len0, len1, len2];
            double[,,] cosIncImg = new double[len0, len1, len2];
            double[,,] depthImg = new double[len0, len1, len2];
            double[,,] uImg = new double[len0, len1, len2];
            double[,,] vImg = new double[len0, len1, len2];
            int[,,] rImg = new int[len0, len1, len2];
            int[,,] idImg = new int[len0, len1, len2];
            // Cycles through ray tree to get, casting lets us fill in the blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    // Retrieves the ray tree for this pixel
                    RayTree thisTree = proj.Ray(idx0, idx1);
                    
                    // For each possible ray
                    for (int idx2=0;idx2 < len2; idx2++)
                    {
                        RenderRay thisRay = thisTree.Rays[idx2];

                        // Checks to see if the ray has hit
                        if (thisRay.IntersectData.Hit)
                        {
                            bool hit = thisRay.IntersectData.Hit;
                            double depth = thisRay.IntersectData.Travel;
                            int id = thisRay.IntersectData.Body.ID;
                            ShapeSpecificData sData = 
                                thisRay.IntersectData.BodySpecificData
                                as ShapeSpecificData;
                            double cosIncAng = sData.CosIncAng;
                            double u = sData.U;
                            double v = sData.V;

                            intImg[idx0, idx1, idx2] = 1;
                            cosIncImg[idx0, idx1, idx2] = cosIncAng;
                            depthImg[idx0, idx1, idx2] = depth;
                            uImg[idx0, idx1, idx2] = u;
                            vImg[idx0, idx1, idx2] = v;
                            rImg[idx0, idx1, idx2] = thisRay.Rank + 1;
                            idImg[idx0, idx1, idx2] = id;
                        }
                    }
                }

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
            MatLab.Put(matlab, "pos0", pos0);
            MatLab.Put(matlab, "pos1", pos1);
            MatLab.Put(matlab, "renderTime", watch.Elapsed.Milliseconds);
            resStr = matlab.Execute("MatTraceReflectiveCheck");

            return true;
        }
    }
}
