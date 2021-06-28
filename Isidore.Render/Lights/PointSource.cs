using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Isidore.Maths;

namespace Isidore.Render.LightSources
{
    /// <summary>
    /// Represents a CGI point light source.
    /// </summary>
    public class PointSource :Light
    {
        // Inherited from Light
        //Vector PointingDir { get; set; }
        //Point Position { get; set; }

        private Vector pointingDir = Vector.Zero();
        private double flux; // dQ/dt
        private double intensity; // dQ/dt/dSA
        /// <summary>
        /// Point source pointing direction
        /// </summary>
        public Vector pointDir{get{return pointingDir;}}
        /// <summary>
        /// Point source flux
        /// </summary>
        public double Flux{get{return flux;} set{ flux = value; intensity = flux/(4.0*Math.PI);}}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location"> point source location </param>
        /// <param name="flux"> point source flux </param>
        public PointSource(Point location, double flux)
        {
            this.Position = location;
            this.Flux = flux;
            this.On = true;
        }

        /// <summary>
        /// Finds the source's incidence at a ray/object intersection
        /// </summary>
        /// <param name="Ray"> Ray containing intersection data </param>
        /// <returns> Data pertaining to incident light </returns>
        public override IncidentData Incident(RenderRay Ray)
        {
            IncidentData intData = new IncidentData();
            //Surfaces BlockSurfaces; // Add later
            intData.light = this;
            if (this.On)
            {
                intData.IncidentVec = new Vector(this.Position - Ray.IntersectData.IntersectPt);
                intData.Dist = intData.IncidentVec.Mag();
                intData.IncidentVec /= intData.Dist; // saves a sqrt operation
                intData.Intensity = intensity;
                intData.cosIncAng = -intData.IncidentVec.Dot(Ray.IntersectData.SurfaceNormal);
                intData.cosCamAng = -intData.IncidentVec.Dot(Ray.Dir);
            }

            return intData;
        }

        /// <summary>
        /// Transformation method
        /// </summary>
        /// <param name="trans"> Transformation instance </param>
        /// <param name="inverse"> Switch for using the inverse transform </param>
        public void Transform(Transform trans, bool inverse = false)
        {
            Point newPoint = Position;
            newPoint.Transform(trans, inverse);
            Position = newPoint;
        }
    }
}
