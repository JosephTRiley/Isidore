using System;
using Isidore.Maths;

// Noise function generate the synthetic values for procedural texturing
// By using an OOP approach, we are able to keep a general solution while
// maintaining individual noise realizations.

namespace Isidore.Render
{
    /// <summary>
    /// The NoiseFunction class is used to support synthetic noise models
    /// like Perlin noise.  It is used by the Noise class to generate
    /// single scale noise
    /// </summary>
    public abstract class NoiseFunction : ICloneable
    {
        #region Methods

        /// <summary>
        /// Returns the noise value associated with the given coordinates.
        /// </summary>
        /// <param name="coord"> Noise coordinates </param>
        /// <returns> Noise value </returns>
        public abstract double GetVal(Point coord);

        /// <summary>
        /// Deep-copy (Non-referenced) clone
        /// </summary>
        /// <returns> Cloned copy </returns>
        public NoiseFunction Clone()
        {
            return CloneImp();
        }

        /// <summary>
        /// Deep-copy (Non-referenced) clone casted as an object class
        /// </summary>
        /// <returns> Object class clone </returns>
        object ICloneable.Clone()
        {
            return CloneImp();
        }

        /// <summary>
        /// Clone implementation. Uses MemberwiseClone to clone, and 
        /// inheriting classes will implement the cloning of
        /// specific data types 
        /// </summary>
        /// <returns> Clone copy </returns>
        protected virtual NoiseFunction CloneImp()
        {
            // Shallow copy
            NoiseFunction newCopy = (NoiseFunction)MemberwiseClone();

            // Deep copy

            return newCopy;
        }

        #endregion Methods
    }
}
