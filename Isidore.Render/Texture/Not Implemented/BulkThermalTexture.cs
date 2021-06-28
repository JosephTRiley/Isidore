using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public class BulkThermalTexture : ThermalTexture
    {
        //public Texture<double> temperature;
        //public Texture<double> thickness;
        //public ThermalMaterials thermalMats;
        //private Texture<int> hitCount;

        //public BulkThermalTexture(Texture<double> Temperature, Texture<double> Thickness, ThermalMaterials ThermalMats)
        //    : base(Temperature, Thickness, ThermalMats)
        //{
        //}

        //public BulkThermalTexture(BulkThermalTexture BulkThermalTexture0)
        //    : base(BulkThermalTexture0.temperature, BulkThermalTexture0.thickness, BulkThermalTexture0.thermalMats)
        //{
        //}

        public new double Irr2dTempPerSec(double U, double V, double cosIncAng, double Irr)
        {
            double d = thickness.GetVal(U, V);
            double T = temperature.GetVal(U, V);
            ThermalMaterial mat = getMaterial(U, V);
            double CC = mat.getAlphaCC(T);
            return Irr * cosIncAng * CC;
        }
    }
}
