using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isidore.Render
{
    /// <summary>
    /// Reflectance spectrum of a material.  A wavelength array with
    /// a single positive infinity value is considered universal
    /// considered all spectral ranges
    /// </summary>
    public class ReflectanceSpectrum : Spectrum<double, double>
    {

        #region Fields

        /// <summary>
        /// Wavelength dependance of the material's spectum
        /// </summary>
        public double[] Wavelength { get { return sample; } set { sample = value; } }

        /// <summary>
        /// Spectral reflectance ration of the reflective vs. incident power
        /// </summary>
        public double[] Ratio { get { return value; } set { base.value = value; } }

        #endregion Fields
        #region Constructors

        /// <summary>
        /// Reflectance spectrum constructor
        /// </summary>
        /// <param name="Wavelength"> Wavelength sample points </param>
        /// <param name="Ratio"> ReflectanceSpectrum ratio at each wavelength </param>
        public ReflectanceSpectrum(double[] Wavelength = null, double[] Ratio = null)
            : base(Wavelength, Ratio)
        {
            sample = Wavelength ?? new double[] { double.PositiveInfinity };
            value = Ratio ?? new double[] { 1.0};
        }

        /// <summary>
        /// Constructor for single value reflectance spectrum (delta function)
        /// </summary>
        /// <param name="Sample"> Spectrum sample point </param>
        /// <param name="Value"> Spectrum value at the sample </param>
        public ReflectanceSpectrum(double Sample, double Value)
            :base(Sample, Value)
        {
        }

        /// <summary>
        /// Constructor for universal reflectance spectrum
        /// </summary>
        /// <param name="Value"> Spectral reflectance for the entire spectrum </param>
        public ReflectanceSpectrum(double Value):base()
        {
            sample = new double[] { double.PositiveInfinity };
            value = new double[]{ Value };
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        public new ReflectanceSpectrum Clone()
        {
            ReflectanceSpectrum newN = new ReflectanceSpectrum((double[])Wavelength.Clone(), 
                (double[])Ratio.Clone());
            return newN;
        }

        #endregion Methods
    }
}
