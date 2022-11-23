using System;
using System.IO;
using System.Collections.Generic;
using Isidore.Maths;

namespace Isidore_Tests
{
    /// <summary>
    /// This test is derived from the MatLab script that was used to develop
    /// the KDTree class
    /// </summary>
    public class KDTreeTest
    {
        public static bool Run()
        {
            // test passed boolean
            bool passed = true;
            Console.WriteLine("\n K-D Tree Test.");

            // MatLab input processing
            // Finds output directory location
            String fname = new FileInfo(
                System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + 
                "OutputData\\Maths\\KDTree";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            resStr = matlab.Execute("KDTree_In");
            // Retrieves data
            double[,] arrX = matlab.GetVariable("X", "base");
            double scaleLim = matlab.GetVariable("scaleLim", "base");

            // Makes point array
            List<Point> X = new List<Point>();
            for(int idx = 0; idx<arrX.GetLength(0); idx++)
            {
                double[] Comp = new double[] { arrX[idx, 0], arrX[idx, 1] };
                X.Add(new Point(Comp));
            }

            // Builds the K-D tree
            KDTree kdtree = new KDTree(X);

            // Search point
            Point Y = new Point(new double[] { 40, 60 });

            // Nearest Neighbor
            Tuple<int, double> nn = kdtree.Nearest(Y);

            // Range search
            Tuple<int[], double[]> rs = kdtree.LocateNear(Y, 10);
            Tuple<int[], double[]> rs3 = kdtree.LocateNear(Y, 10, 3);

            // Extracts the box-node tree
            double[,] corn = new double[kdtree.boxes.Length, 2];
            double[,] wide = new double[kdtree.boxes.Length, 2];
            for(int idx0 =0; idx0 < kdtree.boxes.Length; idx0++)
            {
                double[] lo = kdtree.boxes[idx0].lo.Comp;
                double[] hi = kdtree.boxes[idx0].hi.Comp;
                for (int idx1=0; idx1<2;idx1++)
                {
                    corn[idx0, idx1] = Math.Max(0, lo[idx1]);
                    wide[idx0, idx1] = Math.Min(scaleLim,hi[idx1])
                        - corn[idx0, idx1];
                }
            }

            // Returns data
            matlab.PutWorkspaceData("Y", "base", Y.Comp);
            matlab.PutWorkspaceData("corn", "base", corn);
            matlab.PutWorkspaceData("wide", "base", wide);
            matlab.PutWorkspaceData("nnIdx", "base", nn.Item1);
            matlab.PutWorkspaceData("nnDist", "base", nn.Item2);
            matlab.PutWorkspaceData("rsIdx", "base", rs.Item1);
            matlab.PutWorkspaceData("rsDist", "base", rs.Item2);
            matlab.PutWorkspaceData("rs3Idx", "base", rs3.Item1);
            matlab.PutWorkspaceData("rs3Dist", "base", rs3.Item2);

            // Runs plotting scripts
            resStr = matlab.Execute("KDTree_Out");

            return passed;

        }
    }
}
