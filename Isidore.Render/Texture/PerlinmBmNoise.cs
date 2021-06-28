using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isidore.Maths;

// This Perlin mBm is found in the text Texture and Modeling, pg. 440
namespace Isidore.Render
{
    /// <summary>
    /// PerlinmBmNoise is a child of PerlinNoise that simulates
    /// multi-fractional Brownian motion using multiplicative cascade with 
    /// a scalar offset
    /// </summary>
    public class PerlinmBmNoise : PerlinNoise
    {
        #region Fields & Accessors

        /// <summary>
        /// Minimum noise frequency (Must be 0 &lt; x)
        /// </summary>
        public double minFreq
        {
            get { return minfreq; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Frequency must be greater and 0", "minFreq");
                else
                    minfreq = value;
            }
        }
        private double minfreq;

        /// <summary>
        /// Maximum noise frequency (Must be 0 &lt; x)
        /// </summary>
        public double maxFreq
        {
            get { return maxfreq; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Frequency must be greater and 0", "maxFreq");
                else
                    maxfreq = value;
            }
        }
        private double maxfreq;

        /// <summary>
        /// The fBm Hurst exponent
        /// </summary>
        public double H { get; set; }

        /// <summary>
        /// The lacunarity, i.e. the texturing spacing between frequencies
        /// as described by Voss, "Random fractal forgeries" 1985 
        /// (Normally 0 &lt; 1/x &lt;= 1)
        /// </summary>
        public double lacunarity { get; set; }

        /// <summary>
        /// Multiplicative cascade offset applied to each scale 
        /// (Controls the degree of multi-fractal effect)
        /// </summary>
        public double offset { get; set; }

        #endregion Fields & Accessors
        #region Constructors

        /// <summary>
        /// Constructor for a realization of Perlin fBm
        /// </summary>
        /// <param name="rng"> Random realization </param>
        /// <param name="minFreq"> Minimum frequency used in noise 
        /// creation </param>
        /// <param name="maxFreq"> Maximum frequency used in noise 
        /// creation </param>
        /// <param name="H"> The fBm Hurst exponent </param>
        /// <param name="lacunarity"> The texture spacing between frequencies
        /// for each step up in frequency (Normally 0 &lt; x &lt;= 1) </param>
        /// <param name="offset"> Multiplicative cascade offset applied to 
        /// each scale </param>
        /// <param name="tablePower"> Exponent power of two  for the table 
        /// size (default=8 (256 element table)) </param>
        public PerlinmBmNoise(Random rng, double minFreq = 1,
            double maxFreq = 512, double H = 0.75,
            double lacunarity = 2.0, double offset = 0.8, int tablePower = 8) : 
            base(rng, tablePower)
        {
            this.minFreq = minFreq;
            this.maxFreq = maxFreq;
            this.H = H;
            this.lacunarity = lacunarity;
            this.offset = offset;
        }

        /// <summary>
        /// Constructor for a realization of Perlin fBm
        /// </summary>
        /// <param name="minFreq"> Minimum frequency used in noise 
        /// creation </param>
        /// <param name="maxFreq"> Maximum frequency used in noise 
        /// creation </param>
        /// <param name="H"> The fBm Hurst exponent </param>
        /// <param name="lacunarity"> The texture spacing between frequencies
        /// (Normally 0 &lt; 1/x &lt;= 1) </param>
        /// <param name="offset"> Multiplicative cascade offset applied to 
        /// each scale </param>
        /// <param name="randomSeed"> Random realization seed </param>
        /// <param name="tablePower"> Exponent power of two  for the table 
        /// size (default=8 (256 element table)) </param>
        public PerlinmBmNoise(double minFreq = 1,
            double maxFreq = 512, double H = 0.75,
            double lacunarity = 2, double offset = 0.8, int? randomSeed = null,
            int tablePower = 8) : this((randomSeed == null) ? new Random() :
                new Random((int)randomSeed), minFreq, maxFreq, H, lacunarity,
                tablePower)
        {
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Returns the noise value associated with the given coordinates.
        /// </summary>
        /// <param name="coord"> Noise coordinates </param>
        /// <returns> Noise value and components at the given 
        /// coordinates</returns>
        public override Tuple<double, List<double>> GetVal(double[] coord)
        {
            // Cycles through octaves
            double[] scaleCoord = coord.Clone() as double[];
            double noise = 1;
            List<double> lnoise = new List<double>();
            for (double freq = minfreq; freq <= maxfreq; freq *= lacunarity)
            {
                var pnoise = base.GetVal(scaleCoord);
                double power = Math.Pow(freq, -H);
                lnoise.Add(power * (pnoise.Item1 + offset));
                noise *= lnoise.Last();

                // Adds a bit to the scale size for appearances
                scaleCoord = scaleCoord.Select(x => x *= lacunarity).ToArray();
            }

            return new Tuple<double, List<double>>(noise, lnoise);
        }


        /// <summary>
        /// Clones this copy by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        new public PerlinmBmNoise Clone()
        {
            return (PerlinmBmNoise)CloneImp();
        }

        /// <summary>
        /// Deep-copy clone of this instance
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        new protected virtual PerlinNoise CloneImp()
        {
            // Shallow copies from base
            var newCopy = base.CloneImp() as PerlinmBmNoise;

            // Deep-copies all data this is referenced by default

            return newCopy;
        }

        #endregion Methods
    }
}
