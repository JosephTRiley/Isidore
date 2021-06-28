using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Isidore.Render;
using Isidore.Maths;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing of an octree
    /// </summary>
    class OctreeTraceTest
    {
        /// <summary>
        /// Runs a test for the AABB triangle overlap and octree
        /// sub-division
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {            
            
            Stopwatch watch = new Stopwatch();

            // Checks the triangle/AABB overlap test
            // AABB located [1,2,3] - [3,5,7]
            AABB box = new AABB(new Point(2, 3.5, 5), 
                new double[] { 2, 3, 4 });

            // Case 1: Triangle is outside the box
            Point p1_0 = Point.Zero();
            Point p1_1 = new Point(-1, -0.5, -1.5);
            Point p1_2 = new Point(1.5, 1.5, -1);
            bool case1 = box.TriangleOverlap(p1_0, p1_1, p1_2);
            if (case1) return false;

            // Case 2: Triangle has one point in the box
            Point p2_0 = new Point(1.5, 4.5, 6);
            Point p2_1 = new Point(0, 6.5, 3.5);
            Point p2_2 = new Point(-1, 7.5, 6.5);
            bool case2 = box.TriangleOverlap(p2_0, p2_1, p2_2);
            if (!case2) return false;

            // Case 3: Triangle overlaps but all points are outside the box
            Point p3_0 = new Point(6.5, 2.5, -0.5);
            Point p3_1 = new Point(-4, 4.5, 4.5);
            Point p3_2 = new Point(7, 4, 11.5);
            bool case3 = box.TriangleOverlap(p3_0, p3_1, p3_2);
            if (!case3) return false;

            // Case 4: Same as Case 3, but intersecting other sides
            Point p4_0 = new Point(2, 4.5, 0);
            Point p4_1 = new Point(1.5, 2.5, 8.5);
            Point p4_2 = new Point(2.5, 3, 7.5);
            bool case4 = box.TriangleOverlap(p4_0, p4_1, p4_2);
            if (!case4) return false;

            // A 50cm octree
            OctBox octree = new MeshOctBox(Isidore.Maths.Point.Zero(),
                Distribution.Uniform<double>(3, 5));

            // Subdivide into four subspaces
            octree.Subdivide();

            // Subdivides two children
            octree.ChildBoxes[2].Subdivide();
            octree.ChildBoxes[7].Subdivide();

            // Further subdivides one of the third level subspaces
            octree.ChildBoxes[2].ChildBoxes[6].Subdivide();

            // Extracts the center points and indices
            List<double[]> posList = new List<double[]>();
            List<int[]> idxList = new List<int[]>();
            AddPoints(ref posList, ref idxList, octree);

            // Converts to 2D matrices for matLab
            int numPts = posList.Count;
            int numDim = posList[0].Length;
            double[,] posArray = new double[numPts, numDim];
            double[,] idxArray = Isidore.Maths.Distribution.Uniform(
                numPts, 4, -1.0);
            for (int idx0 = 0; idx0 < posList.Count; idx0++)
            {
                for (int idx1 = 0; idx1 < posList[idx0].Length; idx1++)
                    posArray[idx0, idx1] = posList[idx0][idx1];
                for (int idx1 = 0; idx1 < idxList[idx0].Length; idx1++)
                    idxArray[idx0, idx1] = idxList[idx0][idx1];
            }

            // MatLab call
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                    "OutputData\\Render\\MeshTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            // This method does the axis flip
            //MatLab.Put(matLab, "posArray", posArray);
            //MatLab.Put(matLab, "idxArray", idxArray);
            // This one doesn't
            matlab.PutWorkspaceData("posArray", "base", posArray);
            matlab.PutWorkspaceData("idxArray", "base", idxArray);
            matlab.Execute("OctreeTraceCheck;");

            ////////////////////////////////
            // Repeats for a mesh octbox  //
            ////////////////////////////////
            // A 50cm octree
            MeshOctBox moctree = new MeshOctBox(Isidore.Maths.Point.Zero(),
                Distribution.Uniform<double>(3, 5));

            // Subdivide into four subspaces
            moctree.Subdivide();

            // Subdivides two children
            moctree.ChildBoxes[2].Subdivide();
            moctree.ChildBoxes[7].Subdivide();

            // Further subdivides one of the third level subspaces
            moctree.ChildBoxes[2].ChildBoxes[6].Subdivide();

            // Extracts the center points and indices
            posList = new List<double[]>();
            idxList = new List<int[]>();
            AddPoints(ref posList, ref idxList, octree);

            // Converts to 2D matrices for matLab
            numPts = posList.Count;
            numDim = posList[0].Length;
            posArray = new double[numPts, numDim];
            idxArray = Isidore.Maths.Distribution.Uniform(
                numPts, 4, -1.0);
            for (int idx0 = 0; idx0 < posList.Count; idx0++)
            {
                for (int idx1 = 0; idx1 < posList[idx0].Length; idx1++)
                    posArray[idx0, idx1] = posList[idx0][idx1];
                for (int idx1 = 0; idx1 < idxList[idx0].Length; idx1++)
                    idxArray[idx0, idx1] = idxList[idx0][idx1];
            }

            // MatLab call
            // Finds output directory location
            matlab.PutWorkspaceData("posArray", "base", posArray);
            matlab.PutWorkspaceData("idxArray", "base", idxArray);
            matlab.Execute("MeshOctreeTraceCheck;");

            return true;
        }

        /// <summary>
        /// decomposes the octbox tree into lists of positions and indices
        /// </summary>
        /// <param name="posList"> List of  center point positions </param>
        /// <param name="idxList"> List of indices </param>
        /// <param name="octbox"> Octbox to decompose </param>
        public static void AddPoints(ref List<double[]> posList, 
            ref List<int[]> idxList, OctBox octbox)
        {
            // Records center-point
            posList.Add(octbox.CenterPoint.Comp);
            // Records index
            idxList.Add(octbox.Index);

            // All child octrees
            if(octbox.ChildBoxes !=null)
            {
                for (int idx = 0; idx < octbox.ChildBoxes.Length; idx++)
                {
                    OctBox iOctree = octbox.ChildBoxes[idx];
                    AddPoints(ref posList, ref idxList, iOctree);
                }
            }
        }

        ///// <summary>
        ///// decomposes the mesh octbox tree into lists of positions and indices
        ///// </summary>
        ///// <param name="posList"> List of  center point positions </param>
        ///// <param name="idxList"> List of indices </param>
        ///// <param name="octbox"> Octbox to decompose </param>
        //public static void AdhPoints(ref List<double[]> posList,
        //    ref List<int[]> idxList, MeshOctBox octbox)
        //{
        //    // Records center-point
        //    posList.Add(octbox.CenterPoint.Comp);
        //    // Records index
        //    idxList.Add(octbox.Index);

        //    // All child octrees
        //    if (octbox.ChildBoxes != null)
        //    {
        //        for (int idx = 0; idx < octbox.ChildBoxes.Length; idx++)
        //        {
        //            MeshOctBox iOctree = octbox.ChildBoxes[idx];
        //            AddPoints(ref posList, ref idxList, iOctree);
        //        }
        //    }
        //}
    }
}
