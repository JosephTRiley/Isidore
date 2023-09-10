using System;
using System.Linq;
using System.IO;
using Isidore.Maths;

namespace Isidore_Tests
{
    class PointGridTest
    {
        public static bool Run()
        {
            // test passed boolean
            bool passed = true;

            int[] resolution = {4,5,4};
            double[] lowCorner = {2.5,-3,2};
            double[] spacing = { 0.5,0.5,0.5};

            // Makes grid
            PointGrid ptGrid = new PointGrid(resolution, lowCorner, spacing);

            // Test copy transform
            Transform trans = Transform.Perspective(2);
            PointGrid perGrid = ptGrid.CopyTransform(trans);

            // Processes data for MatLab
            Tuple<double[,], int[,], bool[], bool[], double[]> ptsData = ptGrid.ConvertToArrays();
            double[,] pts = ptsData.Item1; // Positional data, don't care if referenced
            bool[] spTag = ptsData.Item4; // Surface point tag
            Tuple<double[,], int[,], bool[], bool[], double[]> pPtsData = perGrid.ConvertToArrays();
            double[,] pPts = pPtsData.Item1;

            MLApp.MLApp matlab = new MLApp.MLApp();
            String strAppDir = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String res = matlab.Execute("clear; mkBold(1);");
            res = matlab.Execute("cd('" + strAppDir + "');");
            matlab.PutWorkspaceData("pts", "base", pts);
            matlab.PutWorkspaceData("corners", "base", ptGrid.Cubes);
            matlab.PutWorkspaceData("pPts", "base", pPts);
            matlab.PutWorkspaceData("spTag", "base", spTag);
            matlab.PutWorkspaceData("spacing", "base", spacing);
            matlab.Execute("figure; plot3(pts(spTag,1),pts(spTag,2),pts(spTag,3),'x')");
            matlab.Execute("hold on; plot3(pts(~spTag,1),pts(~spTag,2),pts(~spTag,3),'x'); hold off");
            matlab.Execute("legend('Surface Points','Volume Points')");
            matlab.Execute("title('Surface & Volume Point Check');");
            matlab.Execute("figure; plot3(pts(:,1),pts(:,2),pts(:,3),'o',pPts(:,1),pPts(:,2),pPts(:,3),'x')");
            matlab.Execute("title('Global and Perspective Projected Point Grid');");

            ////////////////////
            // finds distance //
            ////////////////////

            int[] resolution0 = { 21, 21, 21 };
            //int[] resolution0 = { 5, 5, 5 };
            double[] lowCorner0 = { -1, -1, -1 };
            double[] spacing0 = Operator.Divide(Operator.Multiply(Operator.Absolute(lowCorner0),2.0),
                Operator.Convert<int, double>(Operator.Subtract(resolution0,1)));

            // Makes grid
            PointGrid ptGrid0 = new PointGrid(resolution0, lowCorner0, spacing0);
            Tuple<double[,], int[,], bool[], bool[], double[]> ptsData0 = ptGrid0.ConvertToArrays();
            double[,] pts0 = ptsData0.Item1; // Positional data, don't care if referenced
            bool[] surf0 = ptsData0.Item4; // Positional data, don't care if referenced
            double[,] pt2 = Operator.Multiply(pts0, pts0);

            // Reduces points to a sphere
            // Makes sphere surface grid
            double[] R2 = new double[pt2.GetLength(0)]; // distance^2 from origin
            double radius2 = Math.Pow(0.75, 2.0); // radius2 squared
            // For each point
            for (int idx = 0; idx < R2.Length; idx++)
            {
                // Calculates distance from center
                R2[idx] = Arr.Extract(pt2, 1, idx).Sum();
                // Gives points within the sphere a positive value
                ptGrid0.Points[idx].Value = radius2 - R2[idx];
            }
            // Applies threshold
            ptGrid0.ApplyThreshold(0.0);

            // Identifies surface points
            ptGrid0.IdentifySurfacePts();

            Tuple<double[,], int[,], bool[], bool[], double[]> ptsData1 = ptGrid0.ConvertToArrays();
            bool[] on1 = ptsData1.Item3; // On switch
            bool[] surf1 = ptsData1.Item4; // OnSurface Switch

            // Outputs
            matlab.PutWorkspaceData("pts0", "base", pts0);
            matlab.PutWorkspaceData("surf0", "base", surf0);
            matlab.PutWorkspaceData("R2", "base", R2);
            matlab.PutWorkspaceData("on1", "base", on1);
            matlab.PutWorkspaceData("surf1", "base", surf1);
            matlab.PutWorkspaceData("spacing", "base", spacing);
            matlab.Execute("figure; plot3(pts0(surf1,1),pts0(surf1,2),pts0(surf1,3),'.'); axis vis3d");
            //matlab.Execute("figure; plot3(pts0(surf1,1),pts0(surf1,2),pts0(surf1,3),'o',pts0(on1,1),pts0(on1,2),pts0(on1,3),'.');");
            matlab.Execute("title('Surface point');");

            return passed;
        }
    }
}
