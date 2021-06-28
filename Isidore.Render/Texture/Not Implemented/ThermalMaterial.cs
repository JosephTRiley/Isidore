
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public class ThermalMaterial
    {
        private static double h = 6.626e-34f; // Plank's
        private static double c = 2.99e8f; //SoL
        private static double kb = 1.381e-23f; // Boltzmann
        private static double nFac = (2.0 * Math.PI * h * c * c); // numerator factor
        private static double eFac = h * c / kb; // denominator exponential factor

        public double SmokingTemp;
        public double SputterTemp;
        public double BuringTemp;
        public double MaxTemp;
        private Emiss emissivity;
        private IrrCC irrCoupCoeff;

        public Emiss Emissivity { get { return emissivity; } set { emissivity = value; } }
        public IrrCC IrrCoupCoeff { get { return irrCoupCoeff; } set { irrCoupCoeff = value; } }

        public ThermalMaterial(IrrCC IrrCoupCoeff, Emiss Emissivity, double SmokingTemp,
            double SputterTemp, double BuringTemp, double MaxTemp)
        {
            this.SmokingTemp = SmokingTemp;
            this.SputterTemp = SputterTemp;
            this.BuringTemp = BuringTemp;
            this.MaxTemp = MaxTemp;
            irrCoupCoeff = IrrCoupCoeff;
            emissivity = Emissivity;
        }

        public ThermalMaterial(IrrCC IrrCoupCoeff, Emiss Emissivity, double SmokingTemp,
            double SputterTemp, double BuringTemp)
            : this(IrrCoupCoeff, Emissivity,
                SmokingTemp, SputterTemp, BuringTemp, double.MaxValue) { }

        public double getAlphaCC(double Temp)
        {
            return irrCoupCoeff.getAlpha(Temp);
        }


        public double getEmiss(double wavelength)
        {
            return emissivity.getFactor(wavelength);
        }

        // Might replace with lookup table
        // Integrated exitance [W/m^2]
        public double getExitance(double Temp, double wlenMin, double wlenMax, 
            int samples)
        {
            // spectral step size
            double dwlen = (wlenMax-wlenMin)/samples;
            // wavelength integration
            // centers sample wavelength in bin
            double wlen = wlenMin + dwlen/2;
            double exitance = 0;
            double specExit;
            for (int Idx = 0; Idx < samples; Idx++)
            {
                specExit = getSpectralExitance(Temp, wlen);
                // Graybody exitance
                exitance = specExit * dwlen * emissivity.getFactor(wlen);  
                wlen += dwlen;
            }
            return exitance;
        }

        public static double getExitanceBB(double Temp, double wlenMin, 
            double wlenMax, int samples)
        {
            // spectral step size
            double dwlen = (wlenMax - wlenMin) / samples;
            // wavelength integration
            double wlen = wlenMin + dwlen / 2; // centers sample wavelength in bin
            double exitance = 0;
            double specExit;
            for (int Idx = 0; Idx < samples; Idx++)
            {
                specExit = getSpectralExitance(Temp, wlen);
                exitance = specExit * dwlen;  // Graybody exitance
                wlen += dwlen;
            }
            return exitance;
        }

        // Spectral exitance [W/m^2/m]
        public static double getSpectralExitance(double Temp, double wlen)
        {
            double specExit = (nFac/(Math.Exp(eFac/wlen/Temp)-1));
            specExit /= wlen * wlen * wlen * wlen * wlen;
            return specExit;
        }
    }

    public class ThermalMaterials : List<ThermalMaterial>
    {
    }
}
