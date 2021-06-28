using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    public class IrrCC : Material
    {

        public double[] Temp { get { return index; } set { index = value; } }
        public double[] Alpha { get { return this.value; } set { this.value = value; } }

        // Lambertian
        public IrrCC()
        {
            Temp = null;
            Alpha = null;
        }

        public IrrCC(double[] Temp, double[] Alpha)
        {
            this.Temp = Temp;
            this.Alpha = Alpha;
        }

        /// <summary>
        /// Returns Reflectance for supplied angle
        /// </summary>
        /// <param name="Angle"> Incidence angle (From normal) </param>
        /// <returns> Interpolated Reflectance </returns>
        public double getAlpha(double Temp)
        {
            return GetVal(Temp);
        }

        public override double GetVal(double Temp)
        {
            return Maths.Interpolate.Linear(Temp, this.index, this.value);
        }
    }
}
