using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public interface IThermalTexture : IMaterialTexture
    {
        new ThermalMaterial getMaterial(double U, double V); // Replaces Materials return
        double getTemp(double U, double V);
        void Irr2Temp(double U, double V, double Angle, double Irr, double exposurePeriod);
        double getExitance(double u, double V, double wlenMin, double wlenMax, int samples);
        void resetHitCount();
    }

    // default material: Lambertian BRDF
    public class ThermalTexture : MaterialTexture
    {
        public MapTexture<double> temperature;
        public MapTexture<double> thickness;
        public ThermalMaterials thermalMats;
        private MapTexture<double> dTemp;
        private MapTexture<int> hitCount;

        public ThermalTexture(MapTexture<double> Temperature,
            MapTexture<double> Thickness, MapTexture<int> MaterialID,
            ThermalMaterials ThermalMats)
        {
            temperature = Temperature;
            thickness = Thickness;
            MatID = MaterialID;
            thermalMats = ThermalMats;
            dTemp = null;
            hitCount = null;
        }

        public ThermalTexture(MapTexture<double> Temperature, 
            MapTexture<double> Thickness, ThermalMaterials ThermalMats):
            this(Temperature, Thickness, new MapTexture<int>(
                Temperature.map.GetLength(0), Temperature.map.GetLength(1)), 
                ThermalMats)
        {
        }

        public ThermalTexture(ThermalTexture ThermalTexture0)
            : this(ThermalTexture0.temperature, ThermalTexture0.thickness,
            ThermalTexture0.MatID, ThermalTexture0.thermalMats)
        {
        }

        public ThermalTexture():this(null,null,null,null)
        {
        }

        public double getTemp(double U, double V)
        {
            return temperature.GetVal(U, V);
        }

        public new ThermalMaterial getMaterial(double U, double V) // hides base class
        {
            int iMatID = MatID.GetVal(U,V);
            return thermalMats[iMatID];
        }

        public double getExitance(double U, double V, double wlenMin, double wlenMax, int samples)
        {
            ThermalMaterial thisMat = getMaterial(U,V);
            double thisTemp = getTemp(U, V);
            double thisExitance = thisMat.getExitance(thisTemp, wlenMin, wlenMax, samples);
            return thisExitance;
        }

        public void resetHitCount()
        {
            hitCount = new MapTexture<int>(temperature.map.GetLength(0), temperature.map.GetLength(1));
        }

        public void Irr2dTemp(double U, double V, double cosIncAng, double Irr, double exposurePeriod)
        {
            if (dTemp == null)
                dTemp = new MapTexture<double>(new double[temperature.iWidth(), temperature.iHeigth()]);
            if (hitCount == null)
                hitCount = new MapTexture<int>(new int[temperature.iWidth(), temperature.iHeigth()]);

            double dTpS = Irr2dTempPerSec(U, V, cosIncAng, Irr) * exposurePeriod;
            double dT = dTemp.GetVal(U, V);
            int hit = hitCount.GetVal(U, V);
            // increments hit count, accumulates temperature change
            hitCount.SetVal(hit + 1, U, V);
            dTemp.SetVal(dT + dTpS, U, V);
        }

        public void Irr2dTemp(double U, double V, double cosIncAng, double Irr, double exposurePeriod,
            out bool smokeTag, out bool burnTag, out bool sputterTag)
        {
            bool[] genTag = new bool[3]; 
            if (dTemp == null)
                dTemp = new MapTexture<double>(new double[temperature.iWidth(), temperature.iHeigth()]);
            if (hitCount == null)
                hitCount = new MapTexture<int>(new int[temperature.iWidth(), temperature.iHeigth()]);

            double dTpS = Irr2dTempPerSec(U, V, cosIncAng, Irr) * exposurePeriod;
            double dT = dTemp.GetVal(U, V);
            int hit = hitCount.GetVal(U, V);
            // increments hit count, accumulates temperature change
            hitCount.SetVal(hit + 1, U, V);
            dTemp.SetVal(dT + dTpS, U, V);

            // Passes particle generation tag
            double newTemp = temperature.GetVal(U, V) + (dT + dTpS) / (hit + 1);
            int matIdx = MatID.GetVal(U, V);
            newTemp = (newTemp > thermalMats[matIdx].MaxTemp) ? thermalMats[matIdx].MaxTemp : newTemp;
            smokeTag = (newTemp >= thermalMats[matIdx].SmokingTemp);
            burnTag = (newTemp >= thermalMats[matIdx].BuringTemp);
            sputterTag = (newTemp >= thermalMats[MatID.GetVal(U, V)].SputterTemp);
        }

        public void Add_dTemp()
        {
            if (hitCount != null)
            {
                int xLen = hitCount.iWidth();
                int yLen = hitCount.iHeigth();
                for (int xIdx = 0; xIdx < xLen; xIdx++)
                    for (int yIdx = 0; yIdx < yLen; yIdx++)
                        if (hitCount.map[xIdx, yIdx] > 0)
                        {
                            double newTemp = temperature.map[xIdx, yIdx];
                            newTemp += dTemp.map[xIdx, yIdx] / hitCount.map[xIdx, yIdx];
                            double maxTemp = thermalMats[MatID.map[xIdx, yIdx]].MaxTemp;
                            temperature.map[xIdx, yIdx] = (newTemp > maxTemp) ? maxTemp : newTemp;
                        }
                // resets the index
                hitCount.reset();
                dTemp.reset();
            }
        }

        public double Irr2dTempPerSec(double U, double V, double cosIncAng, double Irr)
        {
            double d = thickness.GetVal(U, V);
            double T = temperature.GetVal(U, V);
            ThermalMaterial mat = getMaterial(U, V);
            double CC = mat.getAlphaCC(T);
            return Irr * cosIncAng * CC / d;
        }
    }
}
