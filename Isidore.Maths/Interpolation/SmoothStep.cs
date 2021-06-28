using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Implements the first seven smoothstep polynomial (S0-S6).  
    /// The first is a linear fit and the rest are Hermite polynomials.
    /// The second polynomial (S1) implements a third order Hermite 
    /// polynomial. Smoothstep polynomials are clamped at 0 and 1.
    /// </summary>
    public class SmoothStep
    {
        #region Fields & Properties

        /// <summary>
        /// Polynomial order
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Operating Hermite polynomial
        /// </summary>
        public Func<double, double> Polynomial
        {
            get
            {
                return polynomial;
            }
        }

        // This function holds the smoothing function
        private Func<double, double> polynomial;

        /// <summary>
        /// All Hermite polynomials orders available
        /// </summary>
        public Func<double,double>[] Polynomials {
            get { return polynomials; } }

        /// <summary>
        /// All Hermite polynomials orders available
        /// </summary>
        private Func<double, double>[] polynomials = 
            new Func<double, double>[]
        {
            x => x,
            x => (-2 * x + 3) * x * x,
            x => ((6 * x - 15) * x + 10) * x * x * x,
            x => ((((-20 * x + 70) * x - 84) * x) + 35) * x * x * x * x,
            x => ((((70 * x - 315) * x + 540) * x - 420) * x + 126) *
                x * x * x * x * x,
            x => ((((((-252 * x + 1386) * x - 3080) * x + 3465) * 
                x - 1980) * x) + 462) * x * x * x * x * x * x,
            x => (((((((924 * x - 6006) * x + 16380) * x - 24024) * x + 
                20020) * x - 9009) * x) + 1716) * x * x * x * x * x * x * x
        };
            

        #endregion Fields & Properties
        #region Constructors

        /// <summary>
        /// Smoothstep constructor
        /// </summary>
        /// <param name="order"> Polynomial order (0-6) </param>
        public SmoothStep(int order = 2)
        {
            // Sets the smooth step
            if (order >= 0 && order < 7)
                polynomial = polynomials[order];
            else
                throw new System.ArgumentException(
                "Valid Perlin noise smooth step values are 0-6", "order");

            // Held for user convenience
            Order = order;
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Evaluates the polynomial at the given position.  Values are 
        /// clamped outside of 0 and 1 to 0 and 1, respectively.
        /// </summary>
        /// <param name="position"> Position at which to evaluate the 
        /// polynomial </param>
        /// <returns> Interpolated value </returns>
        public double Evaluate(double position)
        {
            // Clamps low
            if (position < 0)
                return 0;

            // Clamps high
            if (position > 1)
                return 1;

            // Interpolated value
            return polynomial(position);
        }

        /// <summary>
        /// Deep-copy clone
        /// </summary>
        /// <returns> Deep copy clone </returns>
        public SmoothStep Clone()
        {
            return new SmoothStep(Order);
        }

        #endregion Methods
    }
}
