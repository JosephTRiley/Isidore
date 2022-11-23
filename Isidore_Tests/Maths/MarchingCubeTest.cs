using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Isidore.Maths;

namespace Isidore_Tests
{
    public class MarchingCubeTest
    {
        public static bool Run()
        {
            // test passed boolean
            bool passed = true;
            Console.WriteLine("\nMarching Cube Test.");

            //////////////////////////////
            // Marching cube table test
            //////////////////////////////

            Console.WriteLine("Marching Cube Table Test.");

            // Generates marching cube table
            int[][] cubeTable = MarchingCube.CubeTable();

            // Converts to 2D array, similar to standard form
            int[,] cubeTableArr = Function.Jagged2Array(cubeTable,-1);

            // MatLab processing
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Maths\\MarchingCubes";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            resStr = matlab.Execute("clear;");
            matlab.PutWorkspaceData("cubeTable", "base", cubeTableArr);
            resStr = matlab.Execute("save('CubeTable.mat');");

            //////////////////////////////////
            // Individual marching cube tests
            //////////////////////////////////

            int[] resolution = { 2, 2, 2 };
            double[] lowCorner = { 2, 2, 2 };
            double[] spacing = { 1, 1, 1 };
            PointGrid grid = new PointGrid(resolution, lowCorner, spacing);

            // Every combination of point
            double[,] ptAbove = new double[256, 8];
            for (int idx = 0; idx < 256; idx++)
                for (int cIdx = 0; cIdx < 8; cIdx++)
                    if((idx >> cIdx & 1) > 0)
                        ptAbove[idx, cIdx] = 1;

            // Marching cube instance
            MarchingCube gCube = new MarchingCube(ref grid);

            resStr = matlab.Execute("clear;");
            // Cycles through each point
            for (int idx = 0; idx < 256; idx++)
            {
                // Resets grid
                grid.SetPtsOutside();  // Effectively resets

                // Sets points values
                for (int cIdx = 0; cIdx < 8; cIdx++)

                    grid.Points[cIdx].Value = ptAbove[idx, cIdx];

                // Generates facet mesh
                gCube.Polygonize(0.0, MarchingCube.Mode.Tetra);

                // Gets facet
                Tuple<double[,], int[,]> data = gCube.getFacetMesh();
                double[,] v = data.Item1;
                int[,] f = data.Item2;

                // Gets points locations and on status
                Tuple<double[,], int[,], bool[], bool[], double[]> gData = gCube.pointGrid.ConvertToArrays();
                double[,] pts0 = gData.Item1; // Points
                bool[] in0 = gData.Item3; // On switch
                
                matlab.PutWorkspaceData("pts", "base", pts0);
                matlab.PutWorkspaceData("in", "base", in0);
                matlab.PutWorkspaceData("v", "base", v);
                matlab.PutWorkspaceData("f", "base", f);
                matlab.PutWorkspaceData("gIdx", "base", idx+1);
                matlab.Execute("inArr{gIdx}=in;");
                matlab.Execute("vArr{gIdx}=v;");
                matlab.Execute("fArr{gIdx}=f;");
            }
            matlab.Execute("MarchingCubeCheck");

            //////////////////////////////
            // Full marching cube test
            //////////////////////////////

            resolution = new int[]{ 51, 51, 51 };
            lowCorner = new double[]{ -1, -1, -1 };
            spacing = Operator.Divide(Operator.Multiply(Operator.Absolute(lowCorner), 2.0),
                Operator.Convert<int, double>(Operator.Subtract(resolution, 1)));

            // Makes grid
            PointGrid ptGrid = new PointGrid(resolution, lowCorner, spacing);
            Tuple<double[,], int[,], bool[], bool[], double[]> ptsData = ptGrid.ConvertToArrays();
            double[,] pts = ptsData.Item1; // Positional data, don't care if referenced
            double[,] pt2 = Operator.Multiply(pts, pts);

            // Makes sphere surface grid
            // Makes sphere surface grid
            double[] R2 = new double[pt2.GetLength(0)]; // distance^2 from origin
            double radius2 = Math.Pow(0.75, 2.0); // radius2 squared
            // For each point
            for (int idx = 0; idx < R2.Length; idx++)
            {
                // Calculates distance from center
                R2[idx] = Arr.Extract(pt2, 1, idx).Sum();
                // Gives points within the sphere a positive value
                ptGrid.Points[idx].Value = radius2 - R2[idx];
            }
            // Applies threshold
            ptGrid.ApplyThreshold(0.0);
            
            // Identifies surface points
            ptGrid.IdentifySurfacePts();


            // Makes marching cube object, automatically applies polygonize
            MarchingCube mCube = new MarchingCube(ref ptGrid, 0.0);
            //MarchingCube mCube = new MarchingCube(ref ptGrid,0.0, MarchingCube.Mode.Tetra);
            //MarchingCube mCube = new MarchingCube(ref ptGrid, 0.0, MarchingCube.Mode.Cube);

            // Get the facet mesh data
            Tuple<double[,], int[,]> meshData = mCube.getFacetMesh();
            double[,] V = meshData.Item1;
            int[,] F = meshData.Item2;

            // Writes point grid data to be passed to MatLab from reference in mCube
            mCube.pointGrid.IdentifySurfacePts();
            ptsData = mCube.pointGrid.ConvertToArrays();
            bool[] on = ptsData.Item3; // On switch
            bool[] surf = ptsData.Item4; // Surface point to compare to vertices

            // Applies marching cube for speed check
            Stopwatch watch = new Stopwatch();
            watch.Start();
            mCube.Polygonize(0.0, MarchingCube.Mode.Cube);
            watch.Stop();
            Console.WriteLine("Marching Cube Polygonize Operation Time: {0}ms", watch.ElapsedMilliseconds);

            // Outputs
            resStr = matlab.Execute("clear;");
            matlab.PutWorkspaceData("pts", "base", pts);
            matlab.PutWorkspaceData("on", "base", on);
            matlab.PutWorkspaceData("onSurf", "base", surf);
            matlab.PutWorkspaceData("V", "base", V);
            matlab.PutWorkspaceData("F", "base", F);
            matlab.Execute("MarchingCubeTest");

            return passed;
        }
    }
}
