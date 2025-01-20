using System;
using System.IO;
using System.Diagnostics;
using Isidore.Render;
using Isidore.Maths;
using Isidore.Matlab;
using Isidore.Load;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing of the voxel volume
    /// </summary>
    class ShapeTraceMesh
    {
        /// <summary>
        /// Runs a test of the shape mesh by implementing
        /// a couple of cameras; one on-axis, the other off-
        /// axis
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            string dname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            Stopwatch watch = new Stopwatch();

            ////////////////////////////
            // Ray-Triangle Intersect //
            ////////////////////////////
            Isidore.Maths.Point p0 = new Isidore.Maths.Point(0.5, -0.5, 0);
            Isidore.Maths.Point p1 = new Isidore.Maths.Point(-0.5, 0.5, 0);
            Isidore.Maths.Point p2 = new Isidore.Maths.Point(0.5, 0.5, 0);
            RenderRay ray = new RenderRay(new Isidore.Maths.Point(0.1, 0.1, -10),
                new Vector(0, 0, 1));
            Tuple<bool, double, double[]> iData = Mesh.RayTriangleIntersect(ray, p0, p1, p2);
            if (!iData.Item1) return false;

            ////////////////////////////
            // Ray-R Intersect    //
            ////////////////////////////

            // Loads the Hilux 3D model
            //Polyshape mesh = Isidore.Library.Models.Hilux();
            //var scale = Transform.Scale(0.5, 0.5, 0.5);
            //var rotY = Transform.RotY(-0.75 * Math.PI);
            //var rotX = Transform.RotX(-0.25 * Math.PI);
            //var shift = Transform.Translate(0, -.25, 0);
            //var trans = rotX * rotY * shift * scale;
            //mesh.TransformTimeLine = new KeyFrameTrans(trans);

            // Loads the R 3D model
            //Mesh mesh = Isidore.Library.Models.R();
            //Mesh R = Isidore.Library.Models.R();
            //Polyshape mesh = new Polyshape(R);
            string fileStr = dname.Remove(dname.IndexOf("bin")) + "Inputs\\Rhino3D Files\\R3D.obj";
            Polyshape mesh = OBJ.Load(fileStr);
            mesh.Shapes[0].ID = 4;
            // Shifts to be on axis
            mesh.TransformTimeLine = new KeyFrameTrans(Transform.Translate(
                new double[] { -0.5, -0.5, 0 }));
            // Turns off back face intersection
            mesh.IntersectBackFaces = false;
            // Adjusts the mesh tree depth
            Mesh imesh = mesh.Shapes[0] as Mesh;
            imesh.maxOctreeDepth = 5;

            // Checks that this ray intersect
            mesh.AdvanceToTime(0.0);

            ray = new RenderRay(new Isidore.Maths.Point(-0.325, -0.005, -10), 
                new Vector(0,0,1));
            mesh.Intersect(ref ray);

            // On-axis, Orthonormal projector 
            // Located -10m from the shape
            RectangleProjector proj1 = new RectangleProjector(120 * 4, 140 * 4,
                0.01 / 4, 0.01 / 4, 0, 0);
            proj1.TransformTimeLine = new KeyFrameTrans(Transform.Translate(
                new double[] { 0, 0, -10 }));

            // Off-axis, Orthonormal projector
            // Located -10m back and -10m off-axis
            RectangleProjector proj2 = new RectangleProjector(120 * 4, 140 * 4,
                0.01 / 4, 0.01 / 4, 0, 0);
            Isidore.Maths.Point offPt = new Isidore.Maths.Point(new
                double[] { 10, 0, -10 });
            Transform lookAt = Transform.LookAt(offPt,
                Isidore.Maths.Point.Zero(), Vector.Unit(3, 1));
            Transform transform = lookAt;
            proj2.TransformTimeLine = new KeyFrameTrans(transform);

            // Scene
            Scene scene = new Scene();
            //  Adds the projector & shape to the scene
            scene.Projectors.Add(proj1);
            scene.Projectors.Add(proj2);
            scene.Bodies.Add(mesh);

            // Multiple rendering calls to check multi-threading
            scene.UseMultiCores = false;
            for (int idx = 0; idx < 10; idx++)
            {
                watch.Restart();
                watch.Start();
                scene.AdvanceToTime(0.0, true);
                watch.Stop();
                Console.WriteLine("Serial Mesh trace {0}, " +
                    "total render time = {1}s",
                    idx, watch.ElapsedMilliseconds / 1000.0);
            }
            scene.UseMultiCores = true;
            for (int idx = 0; idx < 10; idx++)
            {
                watch.Restart();
                watch.Start();
                scene.AdvanceToTime(0.0, true);
                watch.Stop();
                Console.WriteLine("Parallel Mesh trace {0}, " +
                    "total render time = {1}s",
                    idx, watch.ElapsedMilliseconds / 1000.0);
            }

            // Uses the get GetIntersectValue normally used with MatLab
            bool[,] Hit = proj1.GetIntersectValue<bool>("Hit");

            // Retrieves data
            int len0 = proj1.Pos0.Length;
            int len1 = proj1.Pos1.Length;
            double[,,] x = new double[len0, len1, 2];
            double[,,] y = new double[len0, len1, 2];
            double[,,] z = new double[len0, len1, 2];
            int[,,] intImg = new int[len0, len1,2];
            int[,,] idImg = new int[len0, len1, 2];
            double[,,] cosIncImg = new double[len0, len1, 2];
            double[,,] depthImg = new double[len0, len1, 2];
            double[,,] uImg = new double[len0, len1, 2];
            double[,,] vImg = new double[len0, len1, 2];

            // Cycles through ray tree to get, casting lets us fill in the 
            // blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    RenderRay thisRay1 = proj1.Ray(idx0, idx1).Rays[0];
                    RenderRay thisRay2 = proj2.Ray(idx0, idx1).Rays[0];

                    // Records position (World space)
                    x[idx0, idx1, 0] = thisRay1.Origin.Comp[0];
                    y[idx0, idx1, 0] = thisRay1.Origin.Comp[1];
                    z[idx0, idx1, 0] = thisRay1.Origin.Comp[2];
                    x[idx0, idx1, 1] = thisRay2.Origin.Comp[0];
                    y[idx0, idx1, 1] = thisRay2.Origin.Comp[1];
                    z[idx0, idx1, 0] = thisRay1.Origin.Comp[2];

                    // Checks to see if the ray has hit
                    if (thisRay1.IntersectData.Hit)
                    {
                        intImg[idx0, idx1, 0] = 1;
                        idImg[idx0, idx1, 0] = thisRay1.IntersectData.Body.ID;
                        depthImg[idx0, idx1, 0] = 
                            thisRay1.IntersectData.Travel;
                        ShapeSpecificData sData = thisRay1.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        cosIncImg[idx0, idx1, 0] = sData.CosIncAng;
                        uImg[idx0, idx1, 0] = sData.U;
                        vImg[idx0, idx1, 0] = sData.V;
                    }
                    if (thisRay2.IntersectData.Hit)
                    {
                        intImg[idx0, idx1, 1] = 1;
                        idImg[idx0, idx1, 1] = thisRay2.IntersectData.Body.ID;
                        depthImg[idx0, idx1, 1] =
                            thisRay2.IntersectData.Travel;
                        ShapeSpecificData sData = thisRay2.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        cosIncImg[idx0, idx1, 1] = sData.CosIncAng;
                        uImg[idx0, idx1, 1] = sData.U;
                        vImg[idx0, idx1, 1] = sData.V;
                    }
                }

            // MatLab processing
            // Finds output directory location
            string fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            string matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Render\\MeshTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            string resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "time_ms", watch.ElapsedMilliseconds);
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "id", idImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "x", x);
            MatLab.Put(matlab, "y", y);
            MatLab.Put(matlab, "z", z);
            MatLab.Put(matlab, "pos1", proj1.Pos0);
            MatLab.Put(matlab, "pos2", proj1.Pos1);
            MatLab.Put(matlab, "Hit", Hit);
            resStr = matlab.Execute("ShapeTraceMeshCheck");

            ///////////////////////////////////////////////////////////////////
            /// This section checks the array contrsuctor.  It will         ///
            /// mostly be used by 3rd party apps like Matlab                ///
            ///////////////////////////////////////////////////////////////////

            // Loads a source mesh
            Mesh sourceMesh = Isidore.Library.Models.Pyramid();

            // Facet generation
            // Mesh facet data prep
            var facets = sourceMesh.Facets;
            var flen = facets.Count;
            int fdepth = 0;

            // Determines the maximum vertices per facet
            // (In case 4+ vertices facets are supported) 
            foreach (var facet in facets)
                if (facet.Length > fdepth)
                    fdepth = facet.Length;

            // Makes mesh facet list
            var facetList = new int[flen, fdepth];
            for (int lidx = 0; lidx < flen; lidx++)
                for (int didx = 0; didx < fdepth; didx++)
                    facetList[lidx, didx] = facets[lidx][didx];

            // Mesh generation
            // Mesh vertex data prep
            var verts = sourceMesh.LocalVertices;
            var vlen = verts.Count;

            // Makes mesh facet list
            var vPosList = new double[vlen, 3];
            var vNormList = new double[vlen, 3];
            var vUVList = new double[vlen, 2];
            for (int lidx = 0; lidx < vlen; lidx++)
            {
                // Local copy
                var vert = verts[lidx];
                // Extracts vertex position and normal
                // (Should have the same dimension)
                for (int didx = 0; didx < vert.Position.Comp.Length; didx++)
                {
                    vPosList[lidx, didx] = vert.Position.Comp[didx];
                    vNormList[lidx, didx] = vert.Normal.Comp[didx];
                }
                // Extracts vertex normals (
                for (int didx = 0; didx < 2; didx++)
                    vUVList[lidx, didx] = vert.UV[didx];
            }

            // Builds mesh from lists
            var meshList = new Mesh(facetList, vPosList, vNormList, vUVList);

            // Adds mehs to a new polyshape
            var newPoly = new Polyshape(meshList);

            // Replaces existing mesh
            mesh = newPoly;
            mesh.Shapes[0].ID = 2;
            mesh.TransformTimeLine = new KeyFrameTrans(Transform.Translate(
                new double[] { 0, -0.5, 0 }));

            // Replaces the old mesh with this mesh (Required)
            scene.Bodies[0] = mesh;

            // Repeats renders (Being lazy, should improve later)
            scene.UseMultiCores = false;
            for (int idx = 0; idx < 10; idx++)
            {
                watch.Restart();
                watch.Start();
                scene.AdvanceToTime(0.0, true);
                watch.Stop();
                Console.WriteLine("Serial Mesh trace {0}, " +
                    "total render time = {1}s",
                    idx, watch.ElapsedMilliseconds / 1000.0);
            }
            scene.UseMultiCores = true;
            for (int idx = 0; idx < 10; idx++)
            {
                watch.Restart();
                watch.Start();
                scene.AdvanceToTime(0.0, true);
                watch.Stop();
                Console.WriteLine("Parallel Mesh trace {0}, " +
                    "total render time = {1}s",
                    idx, watch.ElapsedMilliseconds / 1000.0);
            }

            // Retrieves data
            intImg = new int[len0, len1, 2];
            idImg = new int[len0, len1, 2];
            cosIncImg = new double[len0, len1, 2];
            depthImg = new double[len0, len1, 2];
            uImg = new double[len0, len1, 2];
            vImg = new double[len0, len1, 2];

            // Cycles through ray tree to get, casting lets us fill in the 
            // blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    RenderRay thisRay1 = proj1.Ray(idx0, idx1).Rays[0];
                    RenderRay thisRay2 = proj2.Ray(idx0, idx1).Rays[0];

                    // Checks to see if the ray has hit
                    if (thisRay1.IntersectData.Hit)
                    {
                        intImg[idx0, idx1, 0] = 1;
                        idImg[idx0, idx1, 0] = thisRay1.IntersectData.Body.ID;
                        depthImg[idx0, idx1, 0] =
                            thisRay1.IntersectData.Travel;
                        ShapeSpecificData sData = thisRay1.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        cosIncImg[idx0, idx1, 0] = sData.CosIncAng;
                        uImg[idx0, idx1, 0] = sData.U;
                        vImg[idx0, idx1, 0] = sData.V;
                    }
                    if (thisRay2.IntersectData.Hit)
                    {
                        intImg[idx0, idx1, 1] = 1;
                        idImg[idx0, idx1, 1] = thisRay2.IntersectData.Body.ID;
                        depthImg[idx0, idx1, 1] =
                            thisRay2.IntersectData.Travel;
                        ShapeSpecificData sData = thisRay2.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        cosIncImg[idx0, idx1, 1] = sData.CosIncAng;
                        uImg[idx0, idx1, 1] = sData.U;
                        vImg[idx0, idx1, 1] = sData.V;
                    }
                }

            // MatLab processing
            resStr = matlab.Execute("clear;");
            // Outputs data
            MatLab.Put(matlab, "time_ms", watch.ElapsedMilliseconds);
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "id", idImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "pos1", proj1.Pos0);
            MatLab.Put(matlab, "pos2", proj1.Pos1);
            MatLab.Put(matlab, "Hit", Hit);
            resStr = matlab.Execute("ShapeTraceMeshListCheck");

            ///////////////////////////////////////////
            // Ray-Cube without texture Intersect    //
            ///////////////////////////////////////////

            // Loads the cube model with no texture (i.e. UV coordinates)
            string fileStr1 = dname.Remove(dname.IndexOf("bin")) + "Inputs\\Rhino3D Files\\Cube_NoTexture.obj";
            mesh = OBJ.Load(fileStr1);
            mesh.Shapes[0].ID = 1;

            // Shifts to be on axis
            mesh.TransformTimeLine = new KeyFrameTrans(Transform.Translate(
                new double[] { -0.5, -0.5, 0 }));
            // Turns off back face intersection
            mesh.IntersectBackFaces = false;

            // Checks that this ray intersect
            mesh.AdvanceToTime(0.0);
            ray = new RenderRay(new Isidore.Maths.Point(-0.325, -0.005, -10),
                new Vector(0, 0, 1));
            mesh.Intersect(ref ray);

            // On-axis, Orthonormal projector 
            // Located -10m from the shape
            proj1 = new RectangleProjector(800, 600,
                0.02, 0.02, 0, 0);
            proj1.TransformTimeLine = new KeyFrameTrans(Transform.Translate(
                new double[] { 0, 0, -10 }));

            // Off-axis, Orthonormal projector
            // Located -10m back and -10m off-axis
            proj2 = new RectangleProjector(800, 600,
                0.02, 0.02, 0, 0);
            offPt = new Isidore.Maths.Point(new double[] { 10, 0, -10 });
            lookAt = Transform.LookAt(offPt,
                Isidore.Maths.Point.Zero(), Vector.Unit(3, 1));
            transform = lookAt;
            proj2.TransformTimeLine = new KeyFrameTrans(transform);

            // Scene
            scene = new Scene();
            //  Adds the projector & shape to the scene
            scene.Projectors.Add(proj1);
            scene.Projectors.Add(proj2);
            scene.Bodies.Add(mesh);

            // Rendering
            watch.Start();
            scene.AdvanceToTime(0.0);
            watch.Stop();

            // Uses the get GetIntersectValue normally used with MatLab
            Hit = proj1.GetIntersectValue<bool>("Hit");

            // Retrieves data
            len0 = proj1.Pos0.Length;
            len1 = proj1.Pos1.Length;
            x = new double[len0, len1, 2];
            y = new double[len0, len1, 2];
            z = new double[len0, len1, 2];
            intImg = new int[len0, len1, 2];
            idImg = new int[len0, len1, 2];
            cosIncImg = new double[len0, len1, 2];
            depthImg = new double[len0, len1, 2];
            uImg = new double[len0, len1, 2];
            vImg = new double[len0, len1, 2];

            // Cycles through ray tree to get, casting lets us fill in the 
            // blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    RenderRay thisRay1 = proj1.Ray(idx0, idx1).Rays[0];
                    RenderRay thisRay2 = proj2.Ray(idx0, idx1).Rays[0];

                    // Records position (World space)
                    x[idx0, idx1, 0] = thisRay1.Origin.Comp[0];
                    y[idx0, idx1, 0] = thisRay1.Origin.Comp[1];
                    z[idx0, idx1, 0] = thisRay1.Origin.Comp[2];
                    x[idx0, idx1, 1] = thisRay2.Origin.Comp[0];
                    y[idx0, idx1, 1] = thisRay2.Origin.Comp[1];
                    z[idx0, idx1, 0] = thisRay1.Origin.Comp[2];

                    // Checks to see if the ray has hit
                    if (thisRay1.IntersectData.Hit)
                    {
                        intImg[idx0, idx1, 0] = 1;
                        idImg[idx0, idx1, 0] = thisRay1.IntersectData.Body.ID;
                        depthImg[idx0, idx1, 0] =
                            thisRay1.IntersectData.Travel;
                        ShapeSpecificData sData = thisRay1.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        cosIncImg[idx0, idx1, 0] = sData.CosIncAng;
                        uImg[idx0, idx1, 0] = sData.U;
                        vImg[idx0, idx1, 0] = sData.V;
                    }
                    if (thisRay2.IntersectData.Hit)
                    {
                        intImg[idx0, idx1, 1] = 1;
                        idImg[idx0, idx1, 1] = thisRay2.IntersectData.Body.ID;
                        depthImg[idx0, idx1, 1] =
                            thisRay2.IntersectData.Travel;
                        ShapeSpecificData sData = thisRay2.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        cosIncImg[idx0, idx1, 1] = sData.CosIncAng;
                        uImg[idx0, idx1, 1] = sData.U;
                        vImg[idx0, idx1, 1] = sData.V;
                    }
                }

            // MatLab processing
            resStr = matlab.Execute("clear;");
            // Outputs data
            MatLab.Put(matlab, "time_ms", watch.ElapsedMilliseconds);
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "id", idImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "x", x);
            MatLab.Put(matlab, "y", y);
            MatLab.Put(matlab, "z", z);
            MatLab.Put(matlab, "pos1", proj1.Pos0);
            MatLab.Put(matlab, "pos2", proj1.Pos1);
            MatLab.Put(matlab, "Hit", Hit);
            resStr = matlab.Execute("ShapeTraceNoTextureCheck");

            return true;
        }
    }
}
