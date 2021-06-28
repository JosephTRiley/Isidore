using System;
using System.IO;
using Isidore.Maths;

namespace Isidore_Tests
{
    class KeyFrameTransTest
    {
        public static bool Run()
        {
            // Y-axis rotation data 
            double angRate = Math.PI / 2.0; // Rotation Rate;
            double[] times = new double[] { 0.0, 1.0, 1.5, 2.0, 2.5, 2.75, 3.0, 4.0 }; // key frame angles
            int tLen = times.Length;
            double[] angY = new double[tLen]; // Angles for each time point
            for (int idx = 0; idx < tLen; idx++)
                angY[idx] = angRate * times[idx];

            // Animated transform
            Transform[] trans = new Transform[tLen];
            for (int idx = 0; idx < angY.Length; idx++)
                trans[idx] = Transform.RotY(angY[idx]);
            KeyFrameTrans aTrans = new KeyFrameTrans(trans, times);
            
            // Times to interpolate to
            int iLen = 41;
            double[] iTime = new double[iLen];
            for (int idx = 0; idx < iLen; idx++)
                iTime[idx] = (double)idx * 0.1;

            //Interpolated transforms
            Transform[] iTrans = new Transform[iLen];
            for (int idx = 0; idx < iLen; idx++)
                iTrans[idx] = aTrans.InterpolateToTime(iTime[idx]);

            // Positional data for a point starting 1m down the Z-axis
            double rad = 1.0; // Radial arm length
            Point radPt = new Point(0.0, 0.0, rad); // Point corresponding to the radial arm

            Point testPt = radPt.Clone();
            testPt.Comp[1] += 1.0;

            // Interpolated points
            Point[] iPt = new Point[iLen];
            for (int idx = 0; idx < iLen; idx++)
            {
                iPt[idx] = radPt.Clone();
                iPt[idx].Transform(iTrans[idx]);
            }

            // Straight transformed points
            Point[] tPt = new Point[tLen];
            double[] tX = new double[tLen]; double[] tY = new double[tLen]; double[] tZ = new double[tLen];
            for (int idx = 0; idx < tLen; idx++)
            {
                tPt[idx] = radPt.Clone();
                tPt[idx].Transform(trans[idx]);
                tX[idx] = tPt[idx].Comp[0]; tY[idx] = tPt[idx].Comp[1]; tZ[idx] = tPt[idx].Comp[2];
            }

            // Validation data
            Point[] vPt = new Point[iTime.Length]; // Points using the transform
            for (int idx = 0; idx < iLen; idx++)
            {
                double ang = iTime[idx] * angRate;
                vPt[idx] = new Point(new double[]{rad * Math.Sin(ang), 0.0, rad * Math.Cos(ang)});
            }

            // Validation
            double[] iX = new double[iLen]; double[] vX = new double[iLen];
            double[] iY = new double[iLen]; double[] vY = new double[iLen];
            double[] iZ = new double[iLen]; double[] vZ = new double[iLen];
            double[] err = new double[iLen];
            double totErr = 0.0;
            for (int idx = 0; idx < iLen; idx++)
            {
                iX[idx] = iPt[idx].Comp[0]; vX[idx] = vPt[idx].Comp[0];
                iY[idx] = iPt[idx].Comp[1]; vY[idx] = vPt[idx].Comp[1];
                iZ[idx] = iPt[idx].Comp[2]; vZ[idx] = vPt[idx].Comp[2];
                Point dPt = iPt[idx] - vPt[idx];
                err[idx] = Math.Sqrt(dPt.Comp[0] * dPt.Comp[0] + dPt.Comp[1] * dPt.Comp[1] + dPt.Comp[2] * dPt.Comp[2]);
                totErr += Math.Abs(err[idx]);
            }

            // MatLab display
            MLApp.MLApp matlab = new MLApp.MLApp();
            String strAppDir = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String res = matlab.Execute("clear;");
            res = matlab.Execute("cd('" + strAppDir + "');");
            matlab.PutWorkspaceData("times", "base", times);
            matlab.PutWorkspaceData("angY", "base", angY);
            matlab.PutWorkspaceData("iTime", "base", iTime);
            matlab.PutWorkspaceData("iX", "base", iX);
            matlab.PutWorkspaceData("iY", "base", iY);
            matlab.PutWorkspaceData("iZ", "base", iZ);
            matlab.PutWorkspaceData("tX", "base", tX);
            matlab.PutWorkspaceData("tY", "base", tY);
            matlab.PutWorkspaceData("tZ", "base", tZ);
            matlab.PutWorkspaceData("vX", "base", vX);
            matlab.PutWorkspaceData("vY", "base", vY);
            matlab.PutWorkspaceData("vZ", "base", vZ);
            matlab.Execute("mkBold(1); figure; plot(times,tX,'o',times,tY,'o',times,tZ,'o','MarkerSize',8); grid on");
            matlab.Execute("hold on; plot(iTime,vX,'-+',iTime,vY,'-+',iTime,vZ,'-+'); hold off");
            matlab.Execute("hold on; plot(iTime,iX,'--x',iTime,iY,'--x',iTime,iZ,'--x'); hold off");
            matlab.Execute("legend('Keys X','Keys Y','Keys Z','Interp X','Interp Y','Interp Z','Trans X','Trans Y','Trans Z','Location','Best')");
            matlab.Execute("save KeyFrameInterpolate.mat; mkBold(0)");
            matlab.Execute("printFig('SLERPTest'); Convert2pdf; delete *.eps;");

            return (totErr < 1.0e-6);

        }
    }
}
