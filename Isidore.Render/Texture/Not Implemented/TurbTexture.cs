using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Isidore.Render
{
    public class TurbTexture : FBMTexture
    {
        public TurbTexture(double XScale, double YScale, double ZScale, 
            double XSetPt, double YSetPt, double ZSetPt,
            int Octaves, double Roughness)
            : base(XScale, YScale, ZScale, XSetPt, YSetPt, ZSetPt,
            Octaves, Roughness) { }

        public TurbTexture(double Scale, double RandSeed, int Octaves, double Roughness)
            : base(Scale, RandSeed, Octaves, Roughness) { }

        public TurbTexture()
            : base() { }

        public TurbTexture(FBMTexture pt0) : base(pt0) { }
        public TurbTexture(TurbTexture pt0) : base(pt0) { }

        public override double GetVal(double x, double y)
        {
            return GetVal(x, y, 0);
        }

        public override double GetVal(double x, double y, double z)
        {
            double val = Noise.Turbulence(x * xScale + xSetPt, y * xScale + ySetPt,
                z * zScale + zSetPt, octaves, roughness);
            return val;
        }
    }
}