using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Isidore.Render
{
    /// <summary>
    /// Refractive index spectrum property for a span of wavelengths.
    /// An index of 1.0 is vacuum propagation
    /// </summary>
    public class SpectralRefractiveIndex : Property
    {
        #region Accessors

        /// <summary>
        /// Spectral reflectance
        /// </summary>
        new public Spectrum<double, double> Value { get; set; }

        /// <summary>
        /// Spectral reflectance wavelength sampling
        /// </summary>
        public double[] Wavelength { get { return Value.Sample; } }

        /// <summary>
        /// Spectral refractive index at every wavelength sample
        /// </summary>
        public double[] RefractiveIndex { get { return Value.Value; } }

        #endregion Accessors
        #region Constructors

        /// <summary>
        /// Constructor (Input is not cloned)
        /// </summary>
        /// <param name="refractiveIndex"> spectral refractive index value </param>
        /// <param name="units"> spectral refractive index units (Sample, Value) </param>
        public SpectralRefractiveIndex(Spectrum<double, double> refractiveIndex = null,
            string units = "meters, unitless")
        {
            Value = refractiveIndex;
            Units = units;
        }

        /// <summary>
        /// Constructor using wavelength and reflectance ratio arrays
        /// </summary>
        /// <param name="wavelength"> Wavelength dependance of each irradiance value </param>
        /// <param name="refractiveIndex"> Refractive index at each wavelength </param>
        /// <param name="units"> spectral refractive index units (Sample, Value) </param>
        public SpectralRefractiveIndex(double[] wavelength, double[] refractiveIndex,
            string units = "meters, unitless")
        {
            Spectrum<double, double> newSpec = new Spectrum<double, double>
                (wavelength, refractiveIndex);
            Units = units;
        }

        /// <summary>
        /// Constructor that references another spectral refractive index
        /// </summary>
        /// <param name="spectralRefractiveIndex"></param>
        public SpectralRefractiveIndex(SpectralRefractiveIndex spectralRefractiveIndex):
            this(spectralRefractiveIndex.Value,spectralRefractiveIndex.Units)
        { }

        /// <summary>
        /// Single value refractive index for all wavelengths
        /// </summary>
        /// <param name="refractiveIndex"> Refractive index value </param>
        public SpectralRefractiveIndex(double refractiveIndex = 1.0) :
            this(new double[] { 0 }, new double[] { refractiveIndex })
        { }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        new public SpectralRefractiveIndex Clone()
        {
            return (SpectralRefractiveIndex)CloneImp();
        }

        /// <summary>
        /// Deep-copy clones this instance
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        new protected virtual Property CloneImp()
        {
            // Shallow copies from base
            var newCopy = (SpectralRefractiveIndex)base.CloneImp();

            // Deep-copies all data this is referenced by default
            newCopy.Value = (Spectrum<double,double>)Value.Clone();

            return newCopy;
        }
        #endregion Methods
    }
}
