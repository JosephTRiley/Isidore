using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    public partial class zMesh : zSurface
    {
        public override void RotX(double Radians)
        {
            double cosRad = Math.Cos(Radians);
            double sinRad = Math.Sin(Radians);
            for (int Idx = 0; Idx < vLen; Idx++)
            {
                v[Idx].RotX(cosRad,sinRad);
                vn[Idx].RotX(cosRad, sinRad);
            }
            for (int Idx = 0; Idx < fLen; Idx++)
                N[Idx].RotX(cosRad, sinRad);
            update();
        }

        public override void RotY(double Radians)
        {
            double cosRad = Math.Cos(Radians);
            double sinRad = Math.Sin(Radians);
            for (int Idx = 0; Idx < vLen; Idx++)
            {
                v[Idx].RotY(cosRad, sinRad);
                vn[Idx].RotY(cosRad, sinRad);
            }
            for (int Idx = 0; Idx < fLen; Idx++)
                N[Idx].RotY(cosRad, sinRad);
            update();
        }

        public override void RotZ(double Radians)
        {
            double cosRad = Math.Cos(Radians);
            double sinRad = Math.Sin(Radians);
            for (int Idx = 0; Idx < vLen; Idx++)
            {
                v[Idx].RotZ(cosRad, sinRad);
                vn[Idx].RotZ(cosRad, sinRad);
            }
            for (int Idx = 0; Idx < fLen; Idx++)
                N[Idx].RotZ(cosRad, sinRad);
            update();
        }

        public override void Translate(Vector Shift)
        {
            for (int Idx = 0; Idx < vLen; Idx++)
                v[Idx].Translate(Shift);
            update();
        }

        public override void Scale(Vector ScaleFactor)
        {
            for (int Idx = 0; Idx < vLen; Idx++)
                v[Idx].Scale(ScaleFactor);
            update();
        }

        public void AngleSpaceZ()
        {
            for (int Idx = 0; Idx < vLen; Idx++)
            {
                v[Idx].x /= v[Idx].z;
                v[Idx].y /= v[Idx].z;
            }
            update();
        }
    }
}
