using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Isidore.Render
{
    /// <summary>
    /// Reflectance spectrum property for a span of wavelengths.
    /// Reflectance should be used in wave optics and does not
    /// spawn children rays.
    /// </summary>
    public class SpectralReflectance : Property
    {
        #region Fields & Accessors

        /// <summary>
        /// Spectral reflectance
        /// </summary>
        new public Spectrum<double, double> Value { get; set; }

        /// <summary>
        /// Spectral reflectance wavelength sampling
        /// </summary>
        public double[] Wavelength { get { return Value.Sample; } }

        /// <summary>
        /// Spectral reflectnace at every wavelength sample
        /// </summary>
        public double[] Reflectance { get { return Value.Value; } }

        #endregion Fields & Accessors
        #region Constructors

        /// <summary>
        /// Constructor (Input is not cloned)
        /// </summary>
        /// <param name="reflectance"> spectral reflectance value </param>
        /// <param name="units"> spectral reflectance units (Sample, Value) </param>
        public SpectralReflectance(Spectrum<double, double> reflectance,
            string units = "meters, unitless")
        {
            Value = reflectance;
            Units = units;
        }

        /// <summary>
        /// Constructor using wavelength and reflectance ratio arrays
        /// </summary>
        /// <param name="wavelength"> Wavelength dependance of each irradiance value </param>
        /// <param name="reflectance"> Reflectance at each wavelength </param>
        /// <param name="units"> spectral refractive index units (Sample, Value) </param>
        public SpectralReflectance(double[] wavelength, double[] reflectance,
            string units = "meters, unitless") :
            this(new Spectrum<double, double>(wavelength, reflectance), units)
        { }

        /// <summary>
        /// Constructor that references (points) to another spectral reflectance instnace
        /// </summary>
        /// <param name="spectralReflectance"> Spectral reflectance to point to </param>
        public SpectralReflectance(SpectralReflectance spectralReflectance) :
            this(spectralReflectance.Value, spectralReflectance.Units)
        { }

        /// <summary>
        /// Single value reflectance for all wavelengths.
        /// </summary>
        /// <param name="reflectance"> Reflectance value </param>
        public SpectralReflectance(double reflectance = 1.0 / Math.PI) :
            this(new double[] { 0 }, new double[] { reflectance })
        { }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        new public SpectralReflectance Clone()
        {
            return (SpectralReflectance)CloneImp();
        }

        /// <summary>
        /// Deep-copy clones this instance
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        new protected virtual Property CloneImp()
        {
            // Shallow copies from base
            var newCopy = (SpectralReflectance)base.CloneImp();

            // Deep-copies all data this is referenced by default
            if (Value != null)
                newCopy.Value = (Spectrum<double, double>)Value.Clone();

            return newCopy;
        }
        #endregion Methods
    }
}
