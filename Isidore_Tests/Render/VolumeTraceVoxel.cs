using System;
using System.IO;
using System.Diagnostics;
using Isidore.Render;
using Isidore.Maths;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing of the voxel volume
    /// </summary>
    class VolumeTraceVoxel
    {
        /// <summary>
        /// Runs a test of the voxel volumetric by implementing
        /// a couple of cameras; one on-axis, the other off-
        /// axis
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {            
            
            Stopwatch watch = new Stopwatch();

            // Creates a 5x5x5 voxel grid, each cell 10cm
            // located at the origin
            Voxel voxel = new Voxel(Isidore.Maths.Point.Zero(),
                Distribution.Uniform(3, 5), Distribution.Uniform(3, 0.1));

            // On-axis, Orthonormal projector 
            // Located -10m from the shape
            RectangleProjector proj0 = new RectangleProjector(120, 140, 
                0.01, 0.01, 0, 0);
            proj0.TransformTimeLine = new KeyFrameTrans(Transform.Translate(
                new double[]{0,0,-10}));

            // Off-axis, Orthonormal projector
            // Located -10m back and -10m off-axis
            RectangleProjector proj1 = new RectangleProjector(120, 140,
                0.01, 0.01, 0, 0);
            Isidore.Maths.Point offPt = new Isidore.Maths.Point(new 
                double[] { -10, 0, -10 });
            Transform lookAt = Transform.LookAt(offPt,
                Isidore.Maths.Point.Zero(), Vector.Unit(3, 1));
            Transform transform = lookAt;
            proj1.TransformTimeLine = new KeyFrameTrans(transform);

            // Increases the number of ray cast since each voxel counts as one
            proj0.RayTreeDepthLimit = 20;
            proj1.RayTreeDepthLimit = 20;          

            // Scene
            Scene scene = new Scene();
            //  Adds the projector & shape to the scene
            scene.Projectors.Add(proj0);
            scene.Projectors.Add(proj1);
            scene.Bodies.Add(voxel);

            // Rendering
            watch.Start();
            scene.AdvanceToTime(0.0);
            watch.Stop();

            // Retrieves data
            int len0 = proj0.Pos0.Length;
            int len1 = proj0.Pos1.Length;
            int[,] intCnt0 = new int[len0, len1];
            int[,] intCnt1 = new int[len0, len1];
            int[,] surfID0 = new int[len0, len1];
            int[,] surfID1 = new int[len0, len1];
            double[,] volume0 = new double[len0, len1];
            double[,] volume1 = new double[len0, len1];

            // Cycles through ray tree to get, casting lets us fill in the blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    // Retrieves the ray tree for each projector
                    RayTree thisTree0 = proj0.Ray(idx0, idx1);
                    RayTree thisTree1 = proj1.Ray(idx0, idx1);

                    // Retrieves the first ray from each projector
                    RenderRay thisRay0 = thisTree0.Rays[0];
                    RenderRay thisRay1 = thisTree1.Rays[0];

                    // Number intersection
                    intCnt0[idx0, idx1] = thisTree0.Rays.Count;
                    intCnt1[idx0, idx1] = thisTree1.Rays.Count;

                    // Intersected surface voxel ID
                    var vData0 = thisRay0.IntersectData.BodySpecificData as
                        VolumeSpecificData;
                    if(vData0 != null)
                        surfID0[idx0, idx1] = vData0.IntersectIndex[0];
                    var vData1 = thisRay1.IntersectData.BodySpecificData as
                        VolumeSpecificData;
                    if(vData1 != null)
                        surfID1[idx0, idx1] = vData1.IntersectIndex[0];

                    // Sums up ray travel through volume
                    for(int idx=0; idx< thisTree0.Rays.Count; idx++)
                    {
                        RenderRay Ray = thisTree0.Rays[idx];
                        var vData = Ray.IntersectData.BodySpecificData as
                        VolumeSpecificData;
                        if (vData != null)
                            volume0[idx0, idx1] += Ray.IntersectData.Travel;
                    }
                    for (int idx = 0; idx < thisTree1.Rays.Count; idx++)
                    {
                        RenderRay Ray = thisTree1.Rays[idx];
                        var vData = Ray.IntersectData.BodySpecificData as
                        VolumeSpecificData;
                        if (vData != null)
                            volume1[idx0, idx1] += Ray.IntersectData.Travel;
                    }
                }

            // MatLab display
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + 
                    "OutputData\\Render\\VolumeTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            MatLab.Put(matlab, "intCnt0", intCnt0);
            MatLab.Put(matlab, "intCnt1", intCnt1);
            MatLab.Put(matlab, "surfID0", surfID0);
            MatLab.Put(matlab, "surfID1", surfID1);
            MatLab.Put(matlab, "volume0", volume0);
            MatLab.Put(matlab, "volume1", volume1);
            matlab.PutWorkspaceData("pos0", "base", proj0.Pos0);
            matlab.PutWorkspaceData("pos1", "base", proj0.Pos1);
            matlab.Execute("VolumeTraceVoxelCheck;");

            return true;
        }
    }
}
