using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isidore.Render
{

    /// <summary>
    /// Reflectance spectrum property for a span of wavelengths
    /// </summary>
    public class SpectralReflectanceProperty : Property <ReflectanceSpectrum>, IReflectance
    {
        /// <summary>
        /// Reflectance value
        /// </summary>
        public ReflectanceSpectrum Value { get { return value; } set { base.value = value; } }

        /// <summary>
        /// Reflectance constructor (Input is not cloned)
        /// </summary>
        /// <param name="reflectance"> reflectance value </param>
        public SpectralReflectanceProperty(ReflectanceSpectrum reflectance = null) : base(reflectance)
        {
        }

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        public new IProperty Clone()
        {
            if (value == null)
                return new SpectralReflectanceProperty();
            else
                return new SpectralReflectanceProperty(value.Clone());
        }
    }
}
