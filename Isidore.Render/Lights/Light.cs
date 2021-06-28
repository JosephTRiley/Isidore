using System.Collections.Generic;
using Isidore.Maths;

namespace Isidore.Render
{
    /// <summary>
    /// Abstract light classes
    /// </summary>
    public abstract class Light : Item
    {
        /// <summary>
        /// Light source position
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Light source pointing direction
        /// </summary>
        public Vector PointingDir { get; set; }
      
        /// <summary>
        /// Light source's incident data processes from an intersection data class
        /// </summary>
        /// <param name="ray"> Intersection data class </param>
        /// <returns> Incident related data </returns>
        public abstract IncidentData Incident(RenderRay ray);
    }

    /// <summary>
    /// List of light instances
    /// </summary>
    public class Lights : List<Light>
    {
    }
}
