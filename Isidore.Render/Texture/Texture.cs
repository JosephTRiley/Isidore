using System;

namespace Isidore.Render
{
    /// <summary>
    /// Base texture class.  Textures are used to provide a scalar value for
    /// a shape or material.
    /// </summary>
    public abstract class Texture : ICloneable
    {
        #region Fields & Properties

        /// <summary>
        /// Indicates whether the texture can be used in the rendering process
        /// </summary>
        public bool On = true;

        /// <summary>
        /// Uses UV coordinates instead on the intersection point
        /// </summary>
        public bool UseUV { get { return useUV; } }

        /// <summary>
        /// Uses UV coordinates instead on the intersection point
        /// </summary>
        protected bool useUV = false;

        #endregion Fields & Properties
        #region Constructors

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Returns value of texture for a given coordinate.
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <param name="z"> Z coordinate </param>
        /// <returns> Texture value at that location </returns>
        public abstract double GetVal(double x, double y, double z);

        /// <summary>
        /// Returns value of texture for a given coordinate.
        /// </summary>
        /// <param name="x"> X coordinate </param>
        /// <param name="y"> Y coordinate </param>
        /// <returns> Texture value at that location </returns>
        public abstract double GetVal(double x, double y);

        /// <summary>
        /// Returns value of texture for a given coordinate.
        /// </summary>
        /// <param name="coords"> UV (XY) or XYZ coordinate space array </param>
        /// <returns> Texture value at that location </returns>
        public abstract double GetVal(double[] coords);

        /// <summary>
        /// Deep-copy (Non-referenced) clone
        /// </summary>
        /// <returns> Cloned copy </returns>
        public Texture Clone()
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
        protected virtual Texture CloneImp()
        {
            // Shallow copy
            var newCopy = (Texture)MemberwiseClone();
            
            return newCopy;
        }

        #endregion Methods
    }
}
