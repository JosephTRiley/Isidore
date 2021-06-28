using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public class PerturbTexture : FBMTexture
    {
        # region Fields & Properties

        public double[] vGrad;
        public double perturbFac;

        # endregion

        # region Constructors

        public PerturbTexture(double[] VAxisGrad, double PerturbFac, 
            double XScale, double YScale, double ZScale, double XSetPt, 
            double YSetPt, double ZSetPt, int Octaves, double Roughness)
            : base(XScale, YScale, ZScale, XSetPt, YSetPt, ZSetPt,
            Octaves, Roughness) 
        {
            vGrad = VAxisGrad;
            perturbFac = PerturbFac;
        }

        public PerturbTexture(double[] VAxisGrad, double PerturbFac,
            double Scale, double RandSeed, int Octaves, double Roughness)
            : base(Scale, RandSeed, Octaves, Roughness)
        {
            vGrad = VAxisGrad;
            perturbFac = PerturbFac;
        }

        public PerturbTexture(double[] VAxisGrad, double PerturbFac,
            double Scale, int Octaves, double Roughness)
            : base(Scale, Octaves, Roughness)
        {
            vGrad = VAxisGrad;
            perturbFac = PerturbFac;
        }

        public PerturbTexture():base()
        {
            vGrad = mkGenericGrad(100,100,100,1);
            perturbFac = 1;
        }

        public PerturbTexture(PerturbTexture pt0)
            : this(pt0.vGrad,
                pt0.perturbFac, pt0.xScale, pt0.yScale, pt0.zScale, pt0.xSetPt,
                pt0.ySetPt, pt0.zSetPt, pt0.octaves, pt0.roughness) { }

        # endregion Fields & Properties

        public override double GetVal(double x, double y)
        {
            return GetVal(x, y, 0);
        }

        public override double GetVal(double x, double y, double z)
        {
            double Px = x*xScale + xSetPt;
            double Py = y*yScale + ySetPt;
            double Pz = z*zScale + zSetPt;

            double perturbVal = base.GetVal(Px, Py, Pz);
            int pix = (int)(perturbVal * perturbFac);
            int gLen = vGrad.Length - 1;
            pix = (int)(gLen * y) - pix;
            pix = (pix > gLen) ? gLen : (pix < 0) ? 0 : pix;
            return vGrad[pix];
        }

        # region Methods

        public static double[] mkGenericGrad(int segLen1, int segLen2, int segLen3, double maxVal)
        {
            int totLen = segLen1 + segLen2 + segLen3;
            double[] gVec = new double[totLen];
            for (int Idx = 0; Idx < segLen1; Idx++)
                gVec[Idx] = maxVal;
            for (int Idx = segLen1; Idx < segLen1+segLen2; Idx++)
            {
                gVec[Idx] = (Math.Cos((Idx - segLen1) / segLen1));
                gVec[Idx] *= maxVal * gVec[Idx];
            }
            for(int Idx = segLen1+segLen2; Idx<totLen;Idx++)
                gVec[Idx] = 0;
            return gVec;
        }

        # endregion
    }
}
