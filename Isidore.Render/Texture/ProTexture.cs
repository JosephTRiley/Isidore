using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RayTracer
{
    // Procedural texture class to be used with alpha and bump maps
    // This version uses the UV coordiantes as the anchor
    // Further efforts will involve a texture3D
    public class ProTexture:Texture<double>
    {
        public double xSetPt, ySetPt, zSetPt, xScale, yScale, zScale, valScale;

        public ProTexture(double XSetPt, double YSetPt, double ZSetPt, 
            double XScale, double YScale, double ZScale, double ValScale)
        {
            xSetPt = XSetPt; ySetPt = YSetPt; zSetPt = ZSetPt;
            xScale = XScale; yScale = YScale; zScale = ZScale;
            valScale = ValScale;
        }

        public ProTexture(double scale, double ValScale)
            : this(scale, scale, scale, 0, 0, 0, ValScale)
        {
        }

        public ProTexture(ProTexture pt0)
            : this(pt0.xSetPt, pt0.ySetPt, pt0.zSetPt, pt0.xScale, pt0.yScale, 
            pt0.zScale, pt0.valScale)
        {
        }

        public ProTexture()
        {
            xSetPt = 0; ySetPt = 0; zSetPt = 0;
            xScale = 1; yScale = 1; zScale = 1;
            valScale = 1;
        }

        public override double getVal(double x, double y)
        {
            return getVal(x, y, 0);
        }

        public override double getVal(double x, double y, double z)
        {
            double val = Noise.Perlin(x*xScale + xSetPt, y*xScale + ySetPt,
                z*zScale + zSetPt);
            return val*valScale;
        }
    }

    public class FBMTexture : ProTexture
    {
        public int octaves;
        public double roughness;

        public FBMTexture(double XSetPt, double YSetPt, double ZSetPt,
            double XScale, double YScale, double ZScale, double ValScale,
            int Octaves, double Roughness)
            : base(XSetPt, YSetPt, ZSetPt, XScale, YScale, ZScale, ValScale)
        {
            octaves = Octaves;
            roughness = Roughness;
        }

        public FBMTexture(FBMTexture pt0)
            : this(pt0.xSetPt, pt0.ySetPt, pt0.zSetPt, pt0.xScale, pt0.yScale,
            pt0.zScale, pt0.valScale, pt0.octaves, pt0.roughness)
        {
        }

        public FBMTexture(TurbTexture pt0) : this((FBMTexture)pt0) { }

        public FBMTexture(double scale, double ValScale, int Octaves, double Roughness)
            : this(0, 0, 0, scale, scale, scale, ValScale, Octaves, Roughness)
        {
        }

        public FBMTexture()
            : base()
        {
            octaves = 8;
            roughness = 0.5;
        }

        public override double getVal(double x, double y)
        {
            return getVal(x, y, 0);
        }

        public new double getVal(double x, double y, double z)
        {
            double val = Noise.FBM(x * xScale + xSetPt, y * xScale + ySetPt,
                z * zScale + zSetPt,octaves,roughness);
            return val * valScale;
        }
    }

    public class TurbTexture : FBMTexture
    {
        public TurbTexture(double XSetPt, double YSetPt, double ZSetPt,
            double XScale, double YScale, double ZScale, double ValScale,
            int Octaves, double Roughness)
            : base(XSetPt, YSetPt, ZSetPt, XScale, YScale, ZScale, ValScale,
            Octaves, Roughness) { }

        public TurbTexture(double scale, double ValScale, int Octaves, double Roughness)
            : base(scale, ValScale, Octaves, Roughness) { }

        public TurbTexture()
            : base() { }

        public TurbTexture(TurbTexture pt0) : base(pt0) { }
        public TurbTexture(FBMTexture pt0) : base(pt0) { }

        public new double getVal(double x, double y, double z)
        {
            double val = Noise.Turbulence(x * xScale + xSetPt, y * xScale + ySetPt,
                z * zScale + zSetPt, octaves, roughness);
            return val * valScale;
        }
    }
}
