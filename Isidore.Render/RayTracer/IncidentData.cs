using Isidore.Maths;

namespace Isidore.Render
{
    /// <summary>
    /// IncidentData is a class used for recording ray/object intersection information.
    /// </summary>
    public class IncidentData
    {
        ///// <summary>
        ///// Surfaces intersected
        ///// </summary>
        //public Surfaces BlockSurfaces;
        /// <summary>
        /// Light sources
        /// </summary>
        public Light light;
        /// <summary>
        /// Ray/Object incident vectors
        /// </summary>
        public Vector IncidentVec;
        /// <summary>
        /// Ray distance traveled to intersection
        /// </summary>
        public double Dist;
        /// <summary>
        /// Light intensity
        /// </summary>
        public double Intensity;
        /// <summary>
        /// Cosine of the angle of incidence
        /// </summary>
        public double cosIncAng;
        /// <summary>
        /// Cosine of the camera angle
        /// </summary>
        public double cosCamAng;

        /// <summary>
        /// Constructor
        /// </summary>
        public IncidentData()
        {
        }

        //public void Transform(Matrix4x4 m)
        //{
        //    IncidentVec.Transform(m);
        //}
    }
}
