using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Not being used

namespace Isidore.Render
{
    /// <summary>
    /// Abstract base property class (it's empty)
    /// </summary>
    public abstract class Property
    {
    }

    /// <summary>
    /// Wavelength property class
    /// </summary>
    public class Wavelength: Property
    {
        /// <summary>
        /// Wavelength value
        /// </summary>
        public double value;

        /// <summary>
        /// Wavelength contructor
        /// </summary>
        /// <param name="wavelength"> Wavelength of instance </param>
        public Wavelength(double wavelength = 1.0e-6)
        {
            value = wavelength;
        }
    }

    /// <summary>
    /// Single wavelength irradiance
    /// </summary>
    public class Irradiance : Property
    {
        /// <summary>
        /// wavelength sample
        /// </summary>
        public double wavelength;
        /// <summary>
        /// Irradiance at the wavelength
        /// </summary>
        public double irradiance;

        /// <summary>
        /// Single wavelength irradiance construtor
        /// </summary>
        /// <param name="wavelength"> Wavelength of irradiance </param>
        /// <param name="irradiance"> Irradiance at this wavelength </param>
        public Irradiance(double wavelength = 1.0e-6, double irradiance = 1)
        {
            this.wavelength = wavelength; // m
            this.irradiance = irradiance; // W m^-2
        }
    }

    /// <summary>
    /// Spectral irradiance bandpass
    /// </summary>
    public class IrradianceBand : Property
    {
        /// <summary>
        /// Irradiance spectum
        /// </summary>
        public SpectralIrradiance spectrum;

        /// <summary>
        /// Irradiance bandpass constructor
        /// </summary>
        /// <param name="irradianceSpectrum"> Irradiance spectrum </param>
        public IrradianceBand(SpectralIrradiance irradianceSpectrum)
        {
            spectrum = irradianceSpectrum;
        }

        public IrradianceBand(double[] wavelength, double[] irradiance):
            this(new SpectralIrradiance(wavelength, irradiance))
        { }

        public IrradianceBand(double wavelength, double irradiance) :
            this(new double[] { wavelength }, new double[] { irradiance })
        { }
    }
}
