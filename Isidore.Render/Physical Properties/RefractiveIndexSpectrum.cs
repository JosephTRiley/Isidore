using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isidore.Render
{
    /// <summary>
    /// Refractive index spectrum of a material
    /// </summary>
    public class RefractIdxSpectrum : Spectrum<double, double>
    {
        /// <summary>
        /// Wavelength dependance of the material's spectum
        /// </summary>
        public double[] Wavelength { get { return base.sample; } set { sample = value; } }

        /// <summary>
        /// Refractive index at each discrete point in the spectrum
        /// </summary>
        public double[] Index { get { return base.value; } set { base.value = value; } }

        /// <summary>
        /// Refractive index spectrum constructor
        /// </summary>
        /// <param name="Wavelength"> Wavelength sample points </param>
        /// <param name="Index"> Refractive index at each wavelength </param>
        public RefractIdxSpectrum(double[] Wavelength = null, double[] Index = null)
            : base(Wavelength, Index)
        {
            base.sample = Wavelength ?? new double[] { 1e-6 };
            this.value = Index ?? new double[] { 1.0 };
        }

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        public new RefractIdxSpectrum Clone()
        {
            RefractIdxSpectrum newN = new RefractIdxSpectrum((double[])this.Wavelength.Clone(), 
                (double[])this.Index.Clone());
            return newN;
        }
    }
}
