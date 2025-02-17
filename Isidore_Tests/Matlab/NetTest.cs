using Isidore.Maths;
using Isidore.Matlab;
using Isidore.Render;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace Isidore_Tests
{
    class NetTest
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Stopwatch watch = new Stopwatch();

            // Uniformly distributed random array
            int alen = 100; // Array length
            var rand = new Rand(123456);
            var rArr = rand.Double(alen, 3);

            // Builds a 1D array of vectors
            var rVec = new Vector[alen];
            // The first value will be null
            for (int idx0 = 0; idx0 < alen; idx0++)
                rVec[idx0] = new Vector(rArr[idx0, 0], rArr[idx0, 1],
                    rArr[idx0, 2]);
            // Extracts the Comp vector from the Vector array
            watch.Start();
            double[,] reArr = Net.GetValue<Vector, double>(rVec, "Comp");
            watch.Stop();
            Console.WriteLine("1D Array Net Extraction time, Vector: {0}ms", 
                watch.ElapsedMilliseconds);

            // Repeats with a 2D array of vectors
            var alen1 = alen + 1;
            var aVec = new Vector[alen, alen + 1];
            var arArr = new double[alen, alen + 1, 3];
            // Save each random component to both a double and vector array
            for (int idx0 = 0; idx0 < alen; idx0++)
            {
                var iarArr = rand.Double(alen1, 3);
                for (int idx1 = 0; idx1 < alen1; idx1++)
                {
                    // Saves to vector
                    aVec[idx0, idx1] = new Vector(iarArr[idx1, 0],
                        iarArr[idx0, 1], iarArr[idx0, 2]);
                    // Saves to array
                    for (int idx2 = 0; idx2 < 3; idx2++)
                        arArr[idx0, idx1, idx2] = iarArr[idx1, idx2];
                }
            }
            // Extracts the Comp vector from the Vector array
            watch.Restart();
            double[,,] areArr = Net.GetValue<Vector, double>(aVec, "Comp");
            watch.Stop();
            Console.WriteLine("2D Array Net Extraction time, Vector: {0}ms",
                watch.ElapsedMilliseconds);

            // Repeats with a bool value
            var sArr = new bool[alen];
            var sVec = new ExNetClass[alen];
            for (int idx0 = 0; idx0 < alen; idx0++)
            {
                sArr[idx0] = rArr[idx0, 0] >= 0.5;
                sVec[idx0] = new ExNetClass(sArr[idx0]);
            }
            watch.Restart();
            bool[,] seArr = Net.GetValue<ExNetClass, bool>(sVec, "Tag");
            watch.Stop();
            Console.WriteLine("1D Array Net Extraction time, Variable: {0}ms", 
                watch.ElapsedMilliseconds);

            // MatLab processing
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Matlab\\NetTest";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            matlab.PutWorkspaceData("rArr", "base", rArr);
            matlab.PutWorkspaceData("reArr", "base", reArr);
            matlab.PutWorkspaceData("arArr", "base", rArr);
            matlab.PutWorkspaceData("areArr", "base", reArr);
            matlab.PutWorkspaceData("sArr", "base", sArr);
            matlab.PutWorkspaceData("seArr", "base", seArr);
            resStr = matlab.Execute("NetCheck");
            // Retrieves comparision
            bool passedVec = matlab.GetVariable("passedVec", "base");
            bool passedArr = matlab.GetVariable("passedArr", "base");
            bool passedBool = matlab.GetVariable("passedBool", "base");
            bool passedMat = matlab.GetVariable("passedMat", "base");

            var passedNet = passedVec && passedArr && passedBool && passedMat;
            return passedNet;
        }
    }

    public class ExNetClass
    {
        public bool Tag = false;

        public ExNetClass(bool tag = false)
        {
            Tag = tag;
        }
    }
}
