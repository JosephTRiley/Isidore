using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render.Texture
{
    /// <summary>
    /// Generates a radial texture  based on a point, then applied to the local surface
    /// </summary>
    class RadialTexture : Texture<double>, ITexture<double>
    {
        public double RadialLength;
        public double uLength;
        public double vLength;
    }
}
