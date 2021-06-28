using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    /// <summary>
    /// Spectral dependent emissivity material class.  Provides the gray body
    /// factor (0-1) for a range of wavelengths.
    /// </summary>
    public class Emiss : Material
    {
        /// <summary>
        /// Spectral emissivity wavelength sample values
        /// </summary>
        public double[] wavelength { get { return index; } set { index = value; } }
        /// <summary>
        /// Gray body emissivity ratio at discreet wavelengths
        /// </summary>
        public double[] ratio { get { return this.value; } set { this.value = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Wavelengths"> sample wavelengths </param>
        /// <param name="Ratios"> gray body ratios for each wavelengths </param>
        public Emiss(double[] Wavelengths, double[] Ratios)
        {
            wavelength = Wavelengths;
            ratio = Ratios;
        }


        /// <summary>
        /// Lambertian black body constructor
        /// </summary>
        public Emiss(): this(new double[] { 0 }, new double[] { 1 })
        {
        }

        /// <summary>
        /// Perfect gray body constructor
        /// </summary>
        /// <param name="GraybodyFactor"></param>
        public Emiss(double GraybodyFactor)
            : this(new double[] { 0 }, new double[] { GraybodyFactor })
        {
        }

        /// <summary>
        /// Returns gray body factor for a given wavelength
        /// </summary>
        /// <param name="Wavelength"> sample wavelength </param>
        /// <returns> gray body ration for specified Wavelength </returns>
        public double getFactor(double Wavelength)
        {
            return GetVal(Wavelength);
        }

        /// <summary>
        /// Returns the emissivity value at a specified wavelength
        /// </summary>
        /// <param name="Wavelength"> wavelength to sample at </param>
        /// <returns> emissivity value at sample wavelength </returns>
        public override double GetVal(double Wavelength)
        {
            return Maths.Interpolate.Linear(Wavelength, wavelength, ratio);
        }
    }
}
