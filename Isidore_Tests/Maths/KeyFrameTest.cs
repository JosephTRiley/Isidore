using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Isidore.Maths;

namespace Isidore_Tests
{
    class KeyFrameTest
    {
        public static bool Run()
        {
            // test passed boolean
            bool passed = true;

            //////////////////////
            // Linear tests
            //////////////////////

            // 1-D Array test
            double[] arrPts = new double[] {-1.8, -0.8, 0.0, 0.5, 0.7, 1.6, 2.5}; // array location
            double[] arrVals = new double[] {0.0, 0.8, -1.23, -0.7, 0.3, -1.1, -1.9}; // array values

            // Interpolation points
            int totPts = (int)(10.0 * (arrPts.Last() - arrPts.First())) + 1;
            double[] intPts = new double[totPts];
            for (int idx = 0; idx < totPts; idx++)
                intPts[idx] = arrPts.First() + 0.1 * (double)idx;

            // Key frame
            KeyFrame<double> keyframe = new KeyFrame<double>(arrVals, arrPts);

            // Method call
            double[] intArr = new double[totPts];
            Stopwatch watch = new Stopwatch();
            for (int idx = 0; idx < totPts; idx++)
            {
                watch.Start();
                intArr[idx] = keyframe.InterpolateToTime(intPts[idx]);
                watch.Stop();
            }

            // Direct Interpolation
            double[] slope = new double[arrPts.Length - 1];
            int[] IntIdx = new int[totPts];
            int iIdx = 0;
            for (int idx = 0; idx < arrPts.Length - 1; idx++)
            {
                // Slopes
                slope[idx] = (arrVals[idx + 1] - arrVals[idx]) / (arrPts[idx + 1] - arrPts[idx]);
                // Places point within coarse array
                while (intPts[iIdx] < arrPts[idx+1])
                    IntIdx[iIdx++] = idx;
            }
            // Handles final point
            IntIdx[iIdx] = arrPts.Length - 2;

            // Interpolation from slopes
            double[] dintArr = new double[totPts]; // Interpolation
            for (int idx = 0; idx < totPts; idx++)
            {
                iIdx = IntIdx[idx];
                dintArr[idx] = slope[iIdx]*(intPts[idx] - arrPts[iIdx]) + arrVals[iIdx];
            }
            
            // Checks difference
            double thresh = 0.0001; // Threshold difference
            double[] aDiff = new double[totPts];
            for (int idx = 0; idx < totPts; idx++)
            {
                aDiff[idx] = Math.Abs(intArr[idx] - dintArr[idx]);
                if ( aDiff[idx] > thresh) passed = false;
            }

            Console.WriteLine("Keyframe interpolation time = {0}s",
                watch.ElapsedMilliseconds / 1000.0);

            MLApp.MLApp matlab = new MLApp.MLApp();
            String strAppDir = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String res = matlab.Execute("clear;");
            res = matlab.Execute("cd('" + strAppDir + "');");
            matlab.PutWorkspaceData("arrPts", "base", arrPts);
            matlab.PutWorkspaceData("arrVals", "base", arrVals);
            matlab.PutWorkspaceData("intPts", "base", intPts);
            matlab.PutWorkspaceData("intArr", "base", intArr);
            matlab.PutWorkspaceData("dintArr", "base", dintArr);
            matlab.Execute("mkBold(1); figure; plot(arrPts, arrVals,'o','MarkerSize',8); grid on");
            matlab.Execute("hold on; plot(intPts,dintArr,'-+'); hold off");
            matlab.Execute("hold on; plot(intPts,intArr,'--x'); hold off");
            matlab.Execute("legend('Keys','KeyFrames','Theory','Location','Best')");
            matlab.Execute("save KeyFrames.mat; mkBold(0)");
            matlab.Execute("printFig('KeyFrameTest'); Convert2pdf; delete *.eps;");

            return passed;
        }
    }
}
