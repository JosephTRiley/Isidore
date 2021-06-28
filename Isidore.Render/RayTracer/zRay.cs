using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    // Z rays are used with z-buffers.  They don't have a direction
    public class zRay
    {

        // keeping it simple
        public Vector2D Pt;
        public Vector Dir = Vector.Null; 

        public zRay(Vector2D Pt)
        {
            this.Pt = Pt;
        }

        public zRay(zRay ray0)
            : this(ray0.Pt)
        {
        }
    }

}
