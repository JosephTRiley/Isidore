using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Isidore.Arr;

namespace Isidore
{
    public class Animate
    {
        public IntSectData[,] iData;
        public double currentTime = -1.0;

        #region Animation Generators

        public void makeADM_Animation(Layout layout, int matFrames, string dirPath, string fileName)
        {
            // Allocation
            double[, ,] Depth, cosIncAng, U, V, Temp, Reflect, Alpha;
            int[, ,] ID;
            string[] IDList;

            double[] sTimes = layout.sampleTimes;
            int sLen = sTimes.Length;
            int matSegs = (int)Math.Ceiling((double)sLen / (double)matFrames);

            int matSeg0 = matSegs - 1;
            string strSeg = matSeg0.ToString();

            for (int Idx = 0; Idx < matSegs; Idx++)
            {
                int start, stop;
                if (matSegs == 1)
                {
                    start = 0;
                    stop = sLen - 1;
                }
                else
                {
                    start = matFrames * Idx;
                    stop = matFrames * (Idx + 1) - 1;
                    stop = (sLen - 1 < stop) ? sLen - 1 : stop;
                }
                IEnumerable<double> ieTime = sTimes.Where(n => n >= sTimes[start] && n <= sTimes[stop]);
                int tLen = ieTime.Count();
                double[] Time = new double[tLen];
                for (int tIdx = 0; tIdx < tLen; tIdx++)
                    Time[tIdx] = ieTime.ElementAt(tIdx);

                Console.WriteLine("Segment: " + Idx.ToString() + "/" + strSeg);

                makeADM_Animation(layout, Time, out Depth, out cosIncAng, 
                    out U, out V, out ID, out IDList, out Reflect, out Temp,
                    out Alpha);

                #region save to MatLab
                // Saves to a MatLab secession
                string fName = fileName + (Idx + 1) + ".mat";
                DirectoryInfo di = new DirectoryInfo(dirPath);
                String strAppDir = di.FullName;
                MLApp.MLApp matlab = new MLApp.MLApp();
                matlab.Execute("cd '" + strAppDir + "';");
                matlab.Execute("clear;");
                MatLab.PutInMatLab(matlab, "xPos", layout.Camera.xPos);
                MatLab.PutInMatLab(matlab, "yPos", layout.Camera.yPos);
                MatLab.PutInMatLab(matlab, "xAng", layout.Camera.xAng);
                MatLab.PutInMatLab(matlab, "yAng", layout.Camera.yAng);
                MatLab.PutInMatLab(matlab, "Depth", Depth);
                MatLab.PutInMatLab(matlab, "cosIncAng", cosIncAng);
                MatLab.PutInMatLab(matlab, "U", U);
                MatLab.PutInMatLab(matlab, "V", V);
                MatLab.PutInMatLab(matlab, "Reflect", Reflect);
                MatLab.PutInMatLab(matlab, "Temp", Temp);
                MatLab.PutInMatLab(matlab, "Alpha", Alpha);
                MatLab.PutInMatLab(matlab, "ID", ID);
                MatLab.PutInMatLab(matlab, "IDList", IDList);
                MatLab.PutInMatLab(matlab, "Time", Time);
                matlab.Execute("save('" + fName + "')");
                matlab.Quit();
                #endregion
            }
        }

        public void makeADM_Animation(Layout layout, double[] sampleTimes, 
            out double[, ,] DepthArr, out double[, ,] cosIncAngArr, 
            out double[, ,] UArr, out double[, ,] VArr, out int[, ,] IDArr,
            out string[] IDList, out double[, ,] ReflectArr, 
            out double[, ,] TempArr, out double[, ,] AlphaArr)
        {
            // Retrieves ID list
            IDList = layout.getIDList();

            DepthArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            cosIncAngArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            UArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            VArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            ReflectArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            TempArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            AlphaArr = new double[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];
            IDArr = new int[layout.Camera.xLen, layout.Camera.yLen, sampleTimes.Length];

            int gLen = layout.AniGroups.Count;
            Transform[] Trans = new Transform[gLen];

            int tFrames = sampleTimes.Length - 1;
            string strFrames = tFrames.ToString();

            for (int Idx = 0; Idx < sampleTimes.Length; Idx++)
            {
                double time = sampleTimes[Idx];

                Console.WriteLine("Frame: " + Idx.ToString() + "/" + strFrames);

                mkCurrentFrame(layout, layout.Camera, time);
                for (int fxIdx = 0; fxIdx < layout.Camera.xLen; fxIdx++)
                    for (int fyIdx = 0; fyIdx < layout.Camera.yLen; fyIdx++)
                    {
                        DepthArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].Dist;
                        cosIncAngArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].cosIncAng;
                        UArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].U;
                        VArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].V;
                        IDArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].ID;
                        ReflectArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].Reflect;
                        TempArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].Temp;
                        AlphaArr[fxIdx, fyIdx, Idx] = iData[fxIdx, fyIdx].Alpha;
                    }
            }
        }

        public void mkReflectAnimation(Layout layout, int matFrames,string dirPath, string fileName,
            int sampling)
        {
            // Allocation
            double[, ,] Reflect;

            double[] sTimes = layout.sampleTimes;
            int sLen = sTimes.Length;
            int matSegs = (int)Math.Ceiling((double)sLen / (double)matFrames);

            int matSeg0 = matSegs - 1;
            string strSeg = matSeg0.ToString();

            for (int Idx = 0; Idx < matSegs; Idx++)
            {
                int start, stop;
                if (matSegs == 1)
                {
                    start = 0;
                    stop = sLen - 1;
                }
                else
                {
                    start = matFrames * Idx;
                    stop = matFrames * (Idx + 1) - 1;
                    stop = (sLen - 1 < stop) ? sLen - 1 : stop;
                }
                IEnumerable<double> ieTime = sTimes.Where(n => n >= sTimes[start] && n <= sTimes[stop]);
                int tLen = ieTime.Count();
                double[] Time = new double[tLen];
                for (int tIdx = 0; tIdx < tLen; tIdx++)
                    Time[tIdx] = ieTime.ElementAt(tIdx);

                Console.WriteLine("Segment: " + Idx.ToString() + "/" + strSeg);

                makeReflectAnimation(layout, Time, sampling, out Reflect);

                // Saves to a MatLab secession
                string fName = fileName + Idx.ToString("D3") + ".mat";
                DirectoryInfo di = new DirectoryInfo(dirPath);
                String strAppDir = di.FullName;
                MLApp.MLApp matlab = new MLApp.MLApp();
                matlab.Execute("cd '" + strAppDir + "';");
                matlab.Execute("clear;");
                MatLab.PutInMatLab(matlab, "xPos", layout.Camera.xPos);
                MatLab.PutInMatLab(matlab, "yPos", layout.Camera.yPos);
                MatLab.PutInMatLab(matlab, "xAng", layout.Camera.xAng);
                MatLab.PutInMatLab(matlab, "yAng", layout.Camera.yAng);
                MatLab.PutInMatLab(matlab, "Reflect", Reflect);
                MatLab.PutInMatLab(matlab, "Time", Time);
                matlab.Execute("save('" + fName + "')");
                matlab.Quit();
            }
        }

        public void makeReflectAnimation(Layout layout, double[] sampleTimes, int sampling, 
            out double[, ,] ReflectArr)
        {
            // Time index
            int tFrames = sampleTimes.Length - 1;
            string strFrames = tFrames.ToString();

            // Tries first frame
            Console.WriteLine("Frame: 0/" + strFrames);
            double[,] thisFrame = mkReflectFrame(layout, layout.Camera, sampleTimes[0], sampling);

            // Reflection Arr
            ReflectArr = new double[thisFrame.GetLength(0), thisFrame.GetLength(1), sampleTimes.Length];
            for (int fxIdx = 0; fxIdx < thisFrame.GetLength(0); fxIdx++)
                for (int fyIdx = 0; fyIdx < thisFrame.GetLength(1); fyIdx++)
                    ReflectArr[fxIdx, fyIdx, 0] = thisFrame[fxIdx, fyIdx];

            int gLen = layout.AniGroups.Count;
            Transform[] Trans = new Transform[gLen];

            //Fig<double> imgFig = new Fig<double>();

            if (sampling <= 1)
            {
                for (int Idx = 1; Idx < sampleTimes.Length; Idx++)
                {
                    double time = sampleTimes[Idx];
                    Console.WriteLine("Frame: " + Idx.ToString() + "/" + strFrames);
                    thisFrame = mkReflectFrame(layout, layout.Camera, time, sampling);
                    for (int fxIdx = 0; fxIdx < thisFrame.GetLength(0); fxIdx++)
                        for (int fyIdx = 0; fyIdx < thisFrame.GetLength(1); fyIdx++)
                            ReflectArr[fxIdx, fyIdx, Idx] = thisFrame[fxIdx, fyIdx];
                    //imgFig.Disp<double>(thisFrame, "Frame: " + Idx.ToString());
                }
            }
            else
            {
                for (int Idx = 1; Idx < sampleTimes.Length; Idx++)
                {
                    double time = sampleTimes[Idx];
                    Console.WriteLine("Frame: " + Idx.ToString() + "/" + strFrames);
                    thisFrame = mkReflectFrame(layout, layout.Camera, time, sampling);
                    for (int fxIdx = 0; fxIdx < thisFrame.GetLength(0); fxIdx++)
                        for (int fyIdx = 0; fyIdx < thisFrame.GetLength(1); fyIdx++)
                            ReflectArr[fxIdx, fyIdx, Idx] = thisFrame[fxIdx, fyIdx];
                    //imgFig.Disp<double>(thisFrame, "Frame: " + Idx.ToString());
                }
            }
        }

        #endregion

        #region Interactive calls

        public double[,] mkAlphaFrame(Layout layout, double time)
        {
            return mkAlphaFrame(layout, layout.Camera, time);
        }

        public double[,] mkAlphaFrame(Layout layout, Camera Cam, double time)
        {
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            double[,] Alpha = new double[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                    Alpha[xIdx, yIdx] = iData[xIdx, yIdx].Alpha;
            return Alpha;
        }

        public double[,] mkDepthFrame(Layout layout, double time)
        {
            return mkDepthFrame(layout, layout.Camera, time);
        }

        public double[,] mkDepthFrame(Layout layout, Camera Cam, double time)
        {
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            double[,] Depth = new double[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                    Depth[xIdx, yIdx] = iData[xIdx, yIdx].Dist;
            return Depth;
        }

        public int[,] mkIDFrame(Layout layout, double time)
        {
            return mkIDFrame(layout, layout.Camera, time);
        }

        public int[,] mkIDFrame(Layout layout, Camera Cam, double time)
        {
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            int[,] ID = new int[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                    ID[xIdx, yIdx] = iData[xIdx, yIdx].ID;
            return ID;
        }

        public double[,] mkReflectFrame(Layout layout, Camera Cam, double time, int sampling)
        {
            if(Cam.xLen%sampling > 0 || Cam.xLen%sampling > 0)
            {
                Console.Error.WriteLine("Camera downsampling must be whole integers.");
            }

            double[,] upSampled = mkReflectFrame(layout, layout.Camera, time);

            int uxLen = upSampled.GetLength(0);
            int uyLen = upSampled.GetLength(1);
            int xLen = uxLen/sampling;
            int yLen = uyLen/sampling;

            double[,] downSampled = new double[xLen, yLen];
            double sampFac = 1/(sampling*sampling);
            for (int xIdx = 0; xIdx < uxLen; xIdx++)
            {
                int xLoc = xIdx / sampling;
                for (int yIdx = 0; yIdx < uyLen; yIdx++)
                {
                    int yLoc = yIdx / sampling;
                    downSampled[xLoc, yLoc] += upSampled[xIdx, yIdx] * sampFac;
                }
            }

            return downSampled;
        }

        public double[,] mkReflectFrame(Layout layout, double time)
        {
            return mkReflectFrame(layout, layout.Camera, time);
        }

        public double[,] mkReflectFrame(Layout layout, Camera Cam, double time)
        {
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            double[,] Reflect = new double[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                    Reflect[xIdx, yIdx] = iData[xIdx, yIdx].Reflect;
            return Reflect;
        }

        public double[,] mkTempFrame(Layout layout, double time)
        {
            return mkTempFrame(layout, layout.Camera, time);
        }

        public double[,] mkTempFrame(Layout layout, Camera Cam, double time)
        {
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            double[,] Temp = new double[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                    Temp[xIdx, yIdx] = iData[xIdx, yIdx].Temp;
            return Temp;
        }

        public void mkUVFrame(Layout layout, double time, out double[,] U, out double[,] V)
        {
            mkUVFrame(layout, layout.Camera, time, out U, out V);
        }
        public void mkUVFrame(Layout layout, Camera Cam, double time, out double[,] U, out double[,] V)
        {
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            U = new double[xLen, yLen];
            V = new double[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                {
                    U[xIdx, yIdx] = iData[xIdx, yIdx].U;
                    V[xIdx, yIdx] = iData[xIdx, yIdx].V;
                }
        }

        public double[,] mkExitanceFrame(Layout layout, double time,
            double wLenLow, double wLenHigh, int wLenSteps)
        {
            return mkExitanceFrame(layout, layout.Camera, time, wLenLow, 
                wLenHigh, wLenSteps);
        }

        public double[,] mkExitanceFrame(Layout layout, Camera Cam, double time,
            double wLenLow, double wLenHigh, int wLenSteps)
        {
            // Checks for current frame
            if (time != currentTime)
                mkCurrentFrame(layout, Cam, time);

            int xLen = iData.GetLength(0);
            int yLen = iData.GetLength(1);
            double[,] Exitance = new double[xLen, yLen];
            for (int xIdx = 0; xIdx < xLen; xIdx++)
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                {
                    if (iData[xIdx, yIdx].Surface != null)
                        if (iData[xIdx, yIdx].Surface.thermalTexture == null)
                            Exitance[xIdx, yIdx] = ThermalMaterial.getExitanceBB(
                                iData[xIdx, yIdx].Temp, wLenLow, wLenHigh, wLenSteps);
                        else
                            Exitance[xIdx, yIdx] = iData[xIdx, yIdx].Surface.
                                thermalTexture.getExitance(iData[xIdx, yIdx].U,
                                iData[xIdx, yIdx].V, wLenLow, wLenHigh, wLenSteps);
                    Exitance[xIdx, yIdx] *= iData[xIdx, yIdx].Alpha;
                }
            return Exitance;
        }

        public void RadiantHeating(Layout layout, Laser laser, double time, double deltaTime)
        {
            mkCurrentFrame(layout, laser, time);

            // Calculates change in temperature, tags rays for particle generators
            bool[,] smokeTag = new bool[laser.xLen, laser.yLen];
            bool[,] burnTag = new bool[laser.xLen, laser.yLen];
            bool[,] sputterTag = new bool[laser.xLen, laser.yLen];
            for (int xIdx = 0; xIdx < laser.xLen; xIdx++)
                for (int yIdx = 0; yIdx < laser.yLen; yIdx++)
                    if (iData[xIdx, yIdx].Hit)
                        if (iData[xIdx, yIdx].Surface.thermalTexture != null)
                        {
                            iData[xIdx, yIdx].Surface.thermalTexture.Irr2dTemp(
                            iData[xIdx, yIdx].U, iData[xIdx, yIdx].V,
                            iData[xIdx, yIdx].cosIncAng,
                            iData[xIdx, yIdx].Alpha * laser.Irr[xIdx, yIdx],
                            deltaTime, out smokeTag[xIdx, yIdx], 
                            out burnTag[xIdx, yIdx], out sputterTag[xIdx, yIdx]);
                        }
            
            // Applies temperature change to approriate surfaces
            foreach (Surface iSurf in layout.Surfaces)
                if (iSurf.thermalTexture != null)
                    iSurf.thermalTexture.Add_dTemp();

            // Adds new smoke/fire/sputter generators
            bool firstCallSmoke=true, firstCallSputter=true, firstCallFire=true;
            double sepdist2 = -1;
            for (int xIdx = 0; xIdx < laser.xLen; xIdx++)
                for (int yIdx = 0; yIdx < laser.yLen; yIdx++)
                {
                    if (smokeTag[xIdx, yIdx])
                    {
                        if (!firstCallSmoke)
                            sepdist2 = 0.0009f;
                        else
                        {
                            sepdist2 = -1;
                            firstCallSmoke = false;
                        }
                        addParticles(time, layout.maxSimTime - time,
                            typeof(Smoke), layout.Surfaces, iData[xIdx, yIdx],
                            sepdist2, (int)(time * 1000) + xIdx + yIdx);

                        //Point intPt = iData[xIdx, yIdx].Ray.Pt +
                        //        iData[xIdx, yIdx].Ray.Dir * iData[xIdx, yIdx].Dist;
                        //double nearestSq = findNearSq(layout.Surfaces, intPt, typeof(Smoke),
                        //    iData[xIdx, yIdx].Name);
                        //// 3cm sep is good for smoke
                        //if (nearestSq > 0.009)
                        //{
                        //    Smoke newSmoke = new Smoke(time,
                        //        layout.maxSimTime - time, 3, 100, new tPath(),
                        //        intPt, (int)(time * 1000) + xIdx + yIdx);
                        //    newSmoke.Name = iData[xIdx, yIdx].Name;
                        //    layout.Surfaces.Add(newSmoke);
                        //}
                    }
                    if (sputterTag[xIdx, yIdx])
                    {
                        if (!firstCallSputter)
                            sepdist2 = 0.0009f;
                        else
                        {
                            sepdist2 = -1;
                            firstCallSputter = false;
                        }
                        addParticles(time, layout.maxSimTime - time,
                            typeof(Sputter), layout.Surfaces, iData[xIdx, yIdx], sepdist2,
                            (int)(time * 100) + xIdx + yIdx);
                    }
                    if (burnTag[xIdx, yIdx])
                    {
                        if (!firstCallFire)
                            sepdist2 = 0.0009f;
                        else
                        {
                            sepdist2 = -1;
                            firstCallFire = false;
                        }
                        addParticles(time, layout.maxSimTime - time,
                            typeof(Fire), layout.Surfaces, iData[xIdx, yIdx],
                            sepdist2, (int)(time * 100) + xIdx + yIdx);
                    }
                }
        }

        private void addParticles(double time, double lifeTime, Type partType, 
            Surfaces surfs, IntSectData iData, double sepSq, int Seed)
        {
            Point intPt = iData.Ray.Pt + iData.Ray.Dir * iData.Dist;
            double nearestSq = findNearSq(surfs, intPt, partType, iData.Name);
            if (nearestSq > sepSq)
            {
                if(partType == typeof(Smoke))
                {
                    Smoke newSurf = new Smoke(time, lifeTime, 3, 100,
                        new tPath(), intPt + 0.01f * iData.Norm, Seed);
                    newSurf.Name = iData.Name;
                    surfs.Add(newSurf);
                }
                else if(partType == typeof(Sputter))
                {
                    Sputter newSurf = new Sputter(time, lifeTime, 5,
                        new tPath(), intPt + 0.01f * iData.Norm, iData.Norm, Seed);
                    newSurf.Name = iData.Name;
                    surfs.Add(newSurf);
                }
                else if (partType == typeof(Fire))
                {
                    Fire newSurf = new Fire(intPt + 0.01f * iData.Norm, iData.Norm, .4f, 1.6f, 2000, 1, 2, 1, 0);
                    newSurf.Name = iData.Name;
                    surfs.Add(newSurf);
                }
            }
        }


        // Checks nearest point of same type
        private double findNearSq(Surfaces surfs, Point Pos, Type type, string Name)
        {
            double nearest = double.MaxValue;
            double thisDist;
            foreach (Surface surf in surfs)
                if(surf.GetType() == type) // checks type
                    if (Name == surf.Name) // checks names
                    {
                        // finds distance
                        thisDist = Pos.DistSquared(surf.AnchorPoint);
                        nearest = (nearest > thisDist) ? thisDist : nearest;
                    }
            return nearest;
        }

        #endregion

        #region Raytrace Interface

        public void mkCurrentFrame(Layout layout, Camera Cam, double time)
        {
            // runs updates
            layout.Update(time); 

            // Transforms objects
            int gLen = layout.AniGroups.Count;
            Transform[] Trans = new Transform[gLen];
            for (int gIdx = 0; gIdx < gLen; gIdx++)
            {
                Trans[gIdx] = layout.AniGroups[gIdx].atTime(time);
                foreach (Obj ao in layout.AniGroups[gIdx].Objects)
                    ao.Transform(Trans[gIdx].m);
            }

            //sendToTracer(layout, Cam);
            Scene fScene = new Scene();
            fScene.Camera = Cam;
            layout.Surfaces.ForEach(surf => fScene.Surfaces.Add(surf));
            fScene.time = time;
            RayTracer trace = new RayTracer();
            trace.RayTrace(fScene, out iData);

            // Returns to scene original state
            for (int gIdx = 0; gIdx < gLen; gIdx++)
                foreach (Obj ao in layout.AniGroups[gIdx].Objects)
                {
                    ao.Transform(Trans[gIdx].im);
                    if (ao.GetType() == typeof(Laser)) // need a more general solution
                    {
                        int xLen = iData.GetLength(0);
                        int yLen = iData.GetLength(1);
                        for (int xIdx = 0; xIdx < xLen; xIdx++)
                            for (int yIdx = 0; yIdx < yLen; yIdx++)
                                iData[xIdx, yIdx].Transform(Trans[gIdx].im);
                    }
                }

            currentTime = time;
        }

        #endregion
    }
}
