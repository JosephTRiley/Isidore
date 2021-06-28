using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public class FlameTexture : PerturbTexture
    {

        # region Fields & Properties

        public double xVel;
        public double yVel;
        public double zVel;

        # endregion

        # region Constructors

        public FlameTexture(double[] AvgTempGrad, double PerturbFac,
            double xScale, double yScale, double zScale, double RandSeed, 
            double xVel, double yVel, double zVel):base(AvgTempGrad,PerturbFac,
            xScale,yScale,zScale,RandSeed,RandSeed,RandSeed,8, 0.5)
        {
            this.xVel = xVel; this.yVel = yVel; this.zVel = zVel;
            useTurbulence = true;
        }

        public FlameTexture(double[] AvgTempGrad, double PerturbFac, 
            double SizeScale, double RandSeed)
            : this(AvgTempGrad, PerturbFac, SizeScale, SizeScale, SizeScale,
            RandSeed, 0, 0, 0) { }

        public FlameTexture(double SizeScale, double RandSeed)
            : this(mkGenericGrad(50, 50, 200, 1), 50, SizeScale, RandSeed) { }

        public FlameTexture() : this(1.0, 0.0) { }

        public FlameTexture(FlameTexture f0) :
            this(f0.vGrad, f0.perturbFac, f0.xScale, f0.yScale, f0.zScale,
            f0.xSetPt, f0.xVel, f0.yVel, f0.zVel) { }

        # endregion Fields & Properties

        public override double GetVal(double x, double y)
        {
            return GetVal(x, y, 0);
        }

        public override double GetVal(double x, double y, double z)
        {
            double y2 = y * y;
            double dx = x - xVel * y2;
            double dy = y - yVel * y;
            double dz = z - zVel * y2;

            return base.GetVal(dx, dy, dz);
        }

        # region Methods

        # endregion
    }
}
