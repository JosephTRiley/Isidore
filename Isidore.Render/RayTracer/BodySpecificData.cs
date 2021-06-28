using System;

namespace Isidore.Render
{
    /// <summary>
    /// Base class for recording intersection data that is specific to each
    /// subclass of the body class.  Any child class can have it's own
    /// SpecificData class.
    /// </summary>
    public class BodySpecificData : ICloneable
    {
        #region Methods

        /// <summary>
        /// Deep-copy (Non-referenced) clone
        /// </summary>
        /// <returns> Cloned copy </returns>
        public BodySpecificData Clone()
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
        protected virtual BodySpecificData CloneImp()
        {
            // Shallow copy
            return MemberwiseClone() as BodySpecificData;
        }

        #endregion Methods
    }
}
