using System.Collections.Generic;
using Isidore.Maths;

namespace Isidore.Render
{
    /// <summary>
    /// Abstract light classes
    /// </summary>
    public abstract class Source : Item
    {
        /// <summary>
        /// Source position
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Source pointing direction
        /// </summary>
        public Vector Dir { get; set; }

        /// <summary>
        /// Source "up" direction (Completes coordinate system)
        /// </summary>
        public Vector Up { get; set; }
      
        /// <summary>
        /// Source's incident data processes from an intersection data class
        /// </summary>
        /// <param name="ray"> Intersection data class </param>
        /// <returns> Incident related data </returns>
        public abstract IncidentData Incident(RenderRay ray);
    }

    /// <summary>
    /// List of light instances
    /// </summary>
    public class Sources : List<Source>
    {
    }
}
