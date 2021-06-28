using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Isidore.Render
{
    public class PerlinTexture : Texture<double>, ITexture<double> //ProTexture<double>
    {
        public double xSetPt, ySetPt, zSetPt, xScale, yScale, zScale, threshold;
        public double scaleFac = 1.0, offsetFac = 0.0;

        public PerlinTexture(double XScale, double YScale, double ZScale,
            double XSetPt, double YSetPt, double ZSetPt, double Threshold)
        {
            xScale = XScale; yScale = YScale; zScale = ZScale;
            xSetPt = XSetPt; ySetPt = YSetPt; zSetPt = ZSetPt;
            threshold = Threshold;
        }

        public PerlinTexture(double XScale, double YScale, double ZScale,
            double XSetPt, double YSetPt, double ZSetPt):
            this(XScale, YScale, ZScale, XSetPt, YSetPt, ZSetPt, double.MinValue)
        {
        }

        public PerlinTexture(double scale, double RandSeed)
            : this(scale, scale, scale, RandSeed, RandSeed, RandSeed)
        {
        }

        public PerlinTexture(double scale)
            : this(scale, 0)
        {
        }

        public PerlinTexture(PerlinTexture pt0)
            : this(pt0.xScale, pt0.yScale, pt0.zScale, 
            pt0.xSetPt, pt0.ySetPt, pt0.zSetPt)
        {
        }

        public PerlinTexture()
        {
            xSetPt = 0; ySetPt = 0; zSetPt = 0;
            xScale = 1; yScale = 1; zScale = 1;
        }

        public override double GetVal(double x, double y)
        {
            return GetVal(x, y, 0);
        }

        public override double GetVal(double x, double y, double z)
        {
            double val = Noise.Perlin(x*xScale + xSetPt, y*xScale + ySetPt,
                z*zScale + zSetPt)*scaleFac + offsetFac;
            if (val > threshold)
                return val;
            else
                return 0;
            //return (val > threshold) ? val - threshold : 0;
        }
    }
}