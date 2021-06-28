using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isidore.Render
{
    /// <summary>
    /// Spectral property is a subclass of property that supports spectral
    /// distributions via a spectrum class
    /// </summary>
    /// <typeparam name="TypeSample"></typeparam>
    /// <typeparam name="TypeValue"></typeparam>
    public class SpectralProperty<TypeSample, TypeValue> : Property<Spectrum<TypeSample, TypeValue>>
    {
        #region Accessors

        ///// <summary>
        ///// Property data field
        ///// </summary>
        //public Spectrum<TypeSample, TypeValue> Data { get { return data; } set { base.data = data; } }

        /// <summary>
        /// Individual data point positions in the spectrum
        /// </summary>
        public TypeSample[] Sample
        {
            get { return data.Sample; }
            set { data.Sample = value; }
        }
                
        /// <summary>
        /// The value of each data point in the spectrum
        /// </summary>
        public TypeValue[] Value
        {
            get { return data.Value; }
            set { data.Value = value; }
        }

        #endregion Accessors
        #region Constructors

        /// <summary>
        /// Constructor using a spectrum data type
        /// </summary>
        /// <param name="spectrum"> Input spectrum data </param>
        public SpectralProperty(Spectrum<TypeSample, TypeValue> spectrum = null) : base(spectrum)
        {
        }

        /// <summary>
        /// Construtor using a samples and values arrays used to create the spectrum Data field
        /// </summary>
        /// <param name="samples"> Individual data point positions in the spectrum </param>
        /// <param name="values"> Individual data points in the spectrum </param>
        public SpectralProperty(TypeSample[] samples, TypeValue[] values) :
            base(new Spectrum<TypeSample, TypeValue>(samples, values))
        {
        }

        # endregion Constructors
        # region Methods

        /// <summary>
        /// Retrieves the linear interpolated value from the spectral
        /// property array
        /// </summary>
        /// <param name="sampleLocation"> Sample location of the point </param>
        /// <returns> The linearly interpolated value at the sample point </returns>
        public TypeValue Retrieve(TypeSample sampleLocation)
        {
            return Maths.Interpolate.Linear(sampleLocation, data.Sample, data.Value);
        }

        /// <summary>
        /// Retrieves the linear interpolated value array from the spectral
        /// property array
        /// </summary>
        /// <param name="sampleLocations"> Array od sample locations </param>
        /// <returns> And array of linearly interpolated values </returns>
        public TypeValue[] Retrieve(TypeSample[] sampleLocations)
        {
            TypeValue[] vals = new TypeValue[sampleLocations.Length];
            for(int idx=0; idx<vals.Length;idx++)
                vals[idx] = Maths.Interpolate.Linear(sampleLocations[idx], 
                    data.Sample, data.Value);
            return vals;
        }

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns></returns>
        public new IProperty Clone()
        {
            if (data == null)
                return new SpectralProperty<TypeSample, TypeValue>();
            else
                return new SpectralProperty<TypeSample, TypeValue>
                    ((Spectrum<TypeSample, TypeValue>)data.Clone());
        }

        # endregion Methods
    }
}
