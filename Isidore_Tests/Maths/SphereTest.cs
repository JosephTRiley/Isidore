using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Isidore.Maths;

namespace Isidore_Tests
{
    class SphereTest
    {
        public static bool Run()
        {
            Console.WriteLine("Evaluating Sphere");

            //double threshErr = 0.0001; // Acceptable error (1 = 100%)


            // Defines sphere
            Sphere sphere = new Sphere(new Point(1, 1, 10), 2);

            //////////////////////////////////
            ////// Checks ray intersect
            //////////////////////////////////

            // Defines ray grid
            int totCnt = 101;
            double corner = -5;
            double step = Math.Abs(2.0 * corner / (double)(totCnt-1));
            Ray[,] rays = new Ray[totCnt,totCnt];
            for (int idx1 = 0; idx1 < totCnt; idx1++)
                for (int idx0 = 0; idx0 < totCnt; idx0++)
                    rays[idx0, idx1] = new Ray(new Point(corner + step * (double)idx0, corner + step * (double)idx1, 0.0),
                        Vector.Unit(3, 2));
            double[] d0 = Distribution.Increment(corner, -corner, step);
            double[] d1 = (double[])d0.Clone();

            // Output arrays
            // Near intersects
            double[,] travelN = new double[totCnt, totCnt];
            double[,] cosIncAngN = new double[totCnt, totCnt];
            double[,] incAngN = new double[totCnt, totCnt];
            // Far intersects
            double[,] travelF = new double[totCnt, totCnt];
            double[,] cosIncAngF = new double[totCnt, totCnt];
            double[,] incAngF = new double[totCnt, totCnt];

            // Calls intersects
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int idx1 = 0; idx1 < totCnt; idx1++)
                //for (int idx0 = 0; idx0 < totCnt; idx0++)
                Parallel.For(0, totCnt, idx0 =>
                {
                    var data = sphere.RayIntersection(rays[idx0, idx1]);
                    travelN[idx0, idx1] = data.Item1[0];
                    cosIncAngN[idx0, idx1] = data.Item2[0];
                    incAngN[idx0, idx1] = Math.Acos(data.Item2[0]) * 180 / Math.PI;
                    travelF[idx0, idx1] = data.Item1[1];
                    cosIncAngF[idx0, idx1] = data.Item2[1];
                    incAngF[idx0, idx1] = Math.Acos(data.Item2[1]) * 180 / Math.PI;
                });
            watch.Stop();
            Console.WriteLine("Sphere:RayIntersection Direct time: {0}ms", watch.ElapsedMilliseconds);

            // Checks in MatLab
            MLApp.MLApp matlab = new MLApp.MLApp();
            String strAppDir = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String res = matlab.Execute("clear;");
            res = matlab.Execute("cd('" + strAppDir + "');");
            matlab.PutWorkspaceData("d0", "base", d0);
            matlab.PutWorkspaceData("d1", "base", d1);
            matlab.PutWorkspaceData("travelN", "base", travelN);
            matlab.PutWorkspaceData("incAngN", "base", incAngN);
            matlab.PutWorkspaceData("travelF", "base", travelF);
            matlab.PutWorkspaceData("incAngF", "base", incAngF);
            matlab.Execute("mkBold(1); figure; subplot(2,2,1); imagesc(d0,d1,travelN); grid on; axis image; colorbar; title('Near Travel')");
            matlab.Execute("subplot(2,2,2); imagesc(d0,d1,incAngN); grid on; axis image; colorbar; title('Near Incidence Angle')");
            matlab.Execute("subplot(2,2,3); imagesc(d0,d1,travelF); grid on; axis image; colorbar; title('Far Travel')");
            matlab.Execute("subplot(2,2,4); imagesc(d0,d1,incAngF); grid on; axis image; colorbar; title('Far Incidence Angle')"); 
            matlab.Execute("save Sphere.mat; mkBold(0)");
            matlab.Execute("printFig('SphereTest'); Convert2pdf; delete *.eps;");

            //////////////////////////////////////////////////
            ////// Checks ray intersect from within the sphere
            //////////////////////////////////////////////////

            // Defines a sphere where some of the rays originate inside the sphere
            Sphere sphereC = new Sphere(Point.Zero(), 2);

            // Output arrays
            // Near intersects
            travelN = new double[totCnt, totCnt];
            cosIncAngN = new double[totCnt, totCnt];
            incAngN = new double[totCnt, totCnt];
            // Far intersects
            travelF = new double[totCnt, totCnt];
            cosIncAngF = new double[totCnt, totCnt];
            incAngF = new double[totCnt, totCnt];

            // Calls intersects
            watch = new Stopwatch();
            watch.Start();
            for (int idx1 = 0; idx1 < totCnt; idx1++)
                //for (int idx0 = 0; idx0 < totCnt; idx0++)
                Parallel.For(0, totCnt, idx0 =>
                {
                    var data = sphereC.RayIntersection(rays[idx0, idx1]);
                    travelN[idx0, idx1] = data.Item1[0];
                    cosIncAngN[idx0, idx1] = data.Item2[0];
                    incAngN[idx0, idx1] = Math.Acos(data.Item2[0]) * 180 / Math.PI;
                    travelF[idx0, idx1] = data.Item1[1];
                    cosIncAngF[idx0, idx1] = data.Item2[1];
                    incAngF[idx0, idx1] = Math.Acos(data.Item2[1]) * 180 / Math.PI;
                });
            watch.Stop();
            Console.WriteLine("Sphere:RayIntersection Direct time: {0}ms", watch.ElapsedMilliseconds);

            // Checks in MatLab
            res = matlab.Execute("clear;");
            res = matlab.Execute("cd('" + strAppDir + "');");
            matlab.PutWorkspaceData("d0", "base", d0);
            matlab.PutWorkspaceData("d1", "base", d1);
            matlab.PutWorkspaceData("travelN", "base", travelN);
            matlab.PutWorkspaceData("incAngN", "base", incAngN);
            matlab.PutWorkspaceData("travelF", "base", travelF);
            matlab.PutWorkspaceData("incAngF", "base", incAngF);
            matlab.Execute("mkBold(1); figure; subplot(2,2,1); imagesc(d0,d1,travelN); grid on; axis image; colorbar; title('Near Travel')");
            matlab.Execute("subplot(2,2,2); imagesc(d0,d1,incAngN); grid on; axis image; colorbar; title('Near Incidence Angle')");
            matlab.Execute("subplot(2,2,3); imagesc(d0,d1,travelF); grid on; axis image; colorbar; title('Far Travel')");
            matlab.Execute("subplot(2,2,4); imagesc(d0,d1,incAngF); grid on; axis image; colorbar; title('Far Incidence Angle')");
            matlab.Execute("save Sphere.mat; mkBold(0)");
            matlab.Execute("printFig('SphereInsideTest'); Convert2pdf; delete *.eps;");


            Console.WriteLine("Ray/Sphere Intersection Check Successful");
            return true;
        }

        private static double perErr(Point val, Point truth)
        {
            // I think this works
            double maxDelta = -1, maxVal = 0, maxTruth = 0;
            for(int idx = 0; idx < val.Comp.Length; idx++)
            {
                double thisDelta = Math.Abs(val.Comp[idx] - truth.Comp[idx]);
                if(thisDelta > maxDelta)
                {
                    maxDelta = thisDelta;
                    maxVal = val.Comp[idx];
                    maxTruth = truth.Comp[idx];
                }
            }
            return Aids.perErr(maxVal, maxTruth);
        }
    }
}
