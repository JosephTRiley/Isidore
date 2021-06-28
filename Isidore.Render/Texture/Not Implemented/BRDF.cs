using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public class BRDF : Material
    {

        public double[] cosIncAngle { get { return index; } set { index = value; } }
        public double[] Reflect { get { return this.value; } set { this.value = value; } }

        // Lambertian
        public BRDF()
		{
            cosIncAngle = new double[91];
            Reflect = new double[91];
            cosIncAngle[0] = 0;
            double dAng = (Math.PI/90);
            Reflect[0] = (1/Math.PI);
            for(int k1 = 1; k1<91; k1++)
            {
                cosIncAngle[k1] = cosIncAngle[k1 - 1] + dAng;
                Reflect[k1] = Reflect[0];
            }
		}

        public BRDF(double[] cosIncAngles, double[] Reflectances)
        {
            cosIncAngle = cosIncAngles;
            Reflect = Reflectances;
        }

        public BRDF(double fracLamberian):this()
        {
            for(int Idx = 0; Idx < Reflect.Length; Idx++)
                this.value[Idx] *= fracLamberian;
        }

        /// <summary>
        /// Returns Reflectance for supplied cosine of incidence angle
        /// </summary>
        /// <param name="Angle"> Cosine of incidence angle (From normal) </param>
        /// <returns> Interpolated Reflectance </returns>
        public double getReflect(double cosIncAngle)
        {
            return GetVal(cosIncAngle);
        }

        public override double GetVal(double cosIncAngle)
        {
            return Maths.Interpolate.Linear(cosIncAngle, this.index, this.value);
        }
    }

    public class BRDFs : List<BRDF>
    {
    }
}
