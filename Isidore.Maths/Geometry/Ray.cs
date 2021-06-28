namespace Isidore.Maths
{
    /// <summary>
    /// Rays are one dimensional quantities that an origin in space and
    /// a direction of propagation.
    /// </summary>
    public class Ray
    {
        # region Fields

        /// <summary>
        /// Ray spatial origin point
        /// </summary>
        public Point Origin;

        /// <summary>
        /// Ray direction
        /// </summary>
        public Vector Dir;

        # endregion Fields
        # region Constructors

        /// <summary>
        /// Ray constructor
        /// </summary>
        /// <param name="Origin"> Origin Point in space </param>
        /// <param name="Dir"> Projection vector </param>
        public Ray(Point Origin = null, Vector Dir = null)
        {
            this.Origin = Origin ?? Point.Zero();
            this.Dir = Dir ?? Vector.Unit(this.Origin.Comp.Length,
                this.Origin.Comp.Length - 1);
        }

        # endregion Constructors
        # region Methods

        /// <summary>
        /// Returned the point in space where the ray will be when
        /// propagated a distance "travel"
        /// </summary>
        /// <param name="travel"> Travel distance </param>
        /// <returns> The point associated with the propagation 
        /// distance </returns>
        public Point Propagate(double travel)
        {
            Point newPt = Origin + (Point)Dir * travel;
            return newPt;
        }

        /// <summary>
        /// Applies the transformation matrix to this ray.  
        /// The matrix is a member of Transform.
        /// </summary>
        /// <param name="trans"> Transform instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        public void Transform(Transform trans, bool inverse = false)
        {
            // Standard parameters
            Origin.Transform(trans, inverse);
            Dir.Transform(trans, inverse);
        }

        /// <summary>
        /// Applies the transformation matrix to a copy of this ray.  
        /// The matrix is a member of Transform.
        /// </summary>
        /// <param name="trans"> Transform instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        /// <returns> A copy of this vector in M-space </returns>
        public Ray CopyTransform(Transform trans, bool inverse = false)
        {
            Ray newRay = Clone();
            newRay.Transform(trans, inverse);
            return newRay;
        }

        /// <summary>
        /// Returns a clone of this ray via deep copy
        /// </summary>
        /// <returns> The cloned copy </returns>
        public Ray Clone()
        {
            // This is a standard method for cloning
            // It uses the memberwise clone and then specifically 
            // clones members that are normally referenced
            var newRay = (Ray)MemberwiseClone();
            newRay.Origin = Origin.Clone();
            newRay.Dir = Dir.Clone();

            return newRay;

            // This is the old method
            // Makes a copy of the ray via constructor
            //return new Ray(Origin.Clone(), Dir.Clone());
        }

        # endregion Methods
    }
}
