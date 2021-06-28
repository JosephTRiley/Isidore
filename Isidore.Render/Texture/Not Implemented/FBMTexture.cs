using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Isidore.Render
{
    public class FBMTexture : PerlinTexture
    {
        public int octaves;
        public double roughness;
        public bool useTurbulence;

        public FBMTexture(double XScale, double YScale, double ZScale,
            double XSetPt, double YSetPt, double ZSetPt,
            int Octaves, double Roughness, double Threshold)
            : base(XScale, YScale, ZScale, XSetPt, YSetPt, ZSetPt, Threshold)
        {
            octaves = Octaves;
            roughness = Roughness;
        }

        public FBMTexture(double XScale, double YScale, double ZScale,
            double XSetPt, double YSetPt, double ZSetPt,
            int Octaves, double Roughness)
            : base(XScale, YScale, ZScale, XSetPt, YSetPt, ZSetPt)
        {
            octaves = Octaves;
            roughness = Roughness;
        }

        public FBMTexture(double Scale, double RandSeed, int Octaves, 
            double Roughness, double Threshold)
            : this(Scale, Scale, Scale, RandSeed, RandSeed, RandSeed, Octaves,
            Roughness, Threshold)
        {
        }

        public FBMTexture(double Scale, double RandSeed, int Octaves, double Roughness)
            : this(Scale, Scale, Scale, RandSeed, RandSeed, RandSeed, Octaves, Roughness)
        {
        }

        public FBMTexture(double Scale, int Octaves, double Roughness)
            : this(Scale, 0, Octaves, Roughness) { }

        public FBMTexture()
            : base()
        {
            octaves = 8;
            roughness = 0.5;
        }

        public FBMTexture(FBMTexture pt0)
            : this(pt0.xScale, pt0.yScale, pt0.zScale, 
            pt0.xSetPt, pt0.ySetPt, pt0.zSetPt, pt0.octaves, pt0.roughness)
        {
        }

        public override double GetVal(double x, double y)
        {
            return GetVal(x, y, 0);
        }

        public override double GetVal(double x, double y, double z)
        {
            double val;
            if (useTurbulence)
                val = Noise.Turbulence(x * xScale + xSetPt, y * xScale + ySetPt,
                    z * zScale + zSetPt, octaves, roughness);
            else
                val = Noise.FBM(x * xScale + xSetPt, y * xScale + ySetPt, 
                    z * zScale + zSetPt, octaves, roughness);
            val *= scaleFac;
            val += offsetFac;
            if (val > threshold)
                return val;
            else
                return 0;
            //return (val > threshold) ? val - threshold : 0;
        }
    }
}