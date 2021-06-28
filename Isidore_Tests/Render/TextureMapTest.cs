using System;
using System.IO;
using System.Drawing;
using Isidore.Render;
using Isidore.Maths;
using Isidore.ImgProcess;
using Isidore.Matlab;

namespace Isidore_Tests
{
    class TextureMapTest
    {
        /// <summary>
        /// Runs test
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {

            // Loads directed from file
            String bname =  new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String dirName = bname.Remove(bname.IndexOf("Isidore_Tests")) + "Isidore.Library\\Resources\\";
            string FileName = dirName + "R.png";
            Bitmap bitmap = new Bitmap(FileName);
            double[,] grayImg = ConvertImg.toGrayScale<double>(ConvertImg.toColor(bitmap));

            // Displays data
            Figure fig = new Figure();
            fig.Disp(grayImg, "Gray Image");

            // Loads from resources
            double[,] grayRes = ConvertImg.toGrayScale<double>(ConvertImg.toColor(Isidore.Library.Images.R()));
            // Image texture map
            MapTexture mapTexture = new MapTexture(grayRes);

            // Checks that both images have the same content
            double[,] diffImage = Operator.Subtract(grayImg,grayRes);
            double[] range = Stats.MinMax(diffImage);
            if (range[0] != 0 || range[1] != 0)
                return false;

            // MatLab processing
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Render\\TextureMap";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data, Put automatically performs the transpose to match C#'s array to MatLab's
            MatLab.Put(matlab, "grayImg", grayImg);
            MatLab.Put(matlab, "grayRes", grayRes);
            //matlab.PutWorkspaceData("grayImg", "base", grayImg);
            //matlab.PutWorkspaceData("grayRes", "base", grayRes);
            resStr = matlab.Execute("TextureMapCheck");

            return true;
        }
    }
}
