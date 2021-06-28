using System;
using Isidore.Maths;

namespace Isidore_Tests
{
    public class Randoms
    {
        public static bool Run()
        {
            // test passed boolean
            bool passed = true;

            ////////////////////////////////
            // Checks Random Generator
            ////////////////////////////////
            Console.WriteLine("\nRandom Generator Check.");

            Rand rand = new Rand(123456);
            Random rand1 = new Random();

            // Checks inherited integer for values 0 - 100
            int len = 100000;
            int maxVal = 100; // maximum value in draw
            int[] rVec = new int[len];
            // histogram stuff
            int numBins = 10; // number of bins
            int[] rCnt = new int[numBins];
            int binSize = maxVal / numBins;
            for (int idx = 0; idx < len; idx++)
            {
                rVec[idx] = rand.Next(0, maxVal);
                // Improvised histogram
                int bin = rVec[idx] / binSize;
                rCnt[bin]++;
            }
            // Checks for linear distribution
            double goodVar = 0.1; // Percentage of mean needed for a pass
            double BinPop = (double)(len / numBins);
            for (int idx = 0; idx < numBins; idx++)
            {
                double ratio = (double)(Math.Abs(rCnt[idx] - BinPop)) / BinPop;
                if (passed & ratio > goodVar)
                    passed = false;
            }
            if (!passed)
                return false;
            Console.WriteLine("Random evaluation within tolerance.");

            // Checks normal distribution
            len = 1000;
            double sigma = 10;
            double mean = 55;
            double[,] rArr = rand.NormDouble(len, len, sigma, mean);
            double[] stdrArr = Stats.STD(rArr);
            double[] rnVec = rand.NormDouble(len, sigma, mean);
            double[] stdrnVec = Stats.STD(rnVec);
            // 2D array
            passed = Math.Abs(stdrArr[0] - sigma) < 0.05 * sigma && Math.Abs(stdrArr[1] - mean) < 0.05 * sigma;
            if (!passed)
                return false;
            // 1D array
            passed = Math.Abs(stdrnVec[0] - sigma) < 0.05 * sigma && Math.Abs(stdrnVec[1] - mean) < 0.05 * sigma;
            if (!passed)
                return false;
            Console.WriteLine("Random evaluation within tolerance.");

            // MatLab display
            //MLApp.MLApp matLab = new MLApp.MLApp();
            //String strAppDir = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            //String res = matlab.Execute("clear;");
            //res = matlab.Execute("cd('" + strAppDir + "');");
            //matlab.PutWorkspaceData("rVec", "base", rVec);
            //matlab.PutWorkspaceData("dVec", "base", dVec);
            //matlab.PutWorkspaceData("rArr", "base", rArr);
            //matlab.PutWorkspaceData("rnVec", "base", rnVec);

            return passed;
        }
    }
}
