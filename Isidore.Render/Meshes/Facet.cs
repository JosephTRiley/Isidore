using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    public class Facet : Surface
    {
        // Vertices, triangles only.
        private Vector v1;
        private Vector v2;
        private Vector v3;
        private Vector vn1;
        private Vector vn2;
        private Vector vn3;
        private double[] vt1 = new double[2];
        private double[] vt2 = new double[2];
        private double[] vt3 = new double[2];
        private BBox bBox;
        private Vector e1;
        private Vector e2;
        private Plane plane;
        private double zDenom;

        //public IMaterial Material;

        #region Assessors

        public Vector V1 { get { return v1; } set { v1 = value; update();} }
        public Vector V2 { get { return v2; } set { v2 = value; update();} }
        public Vector V3 { get { return v3; } set { v3 = value; update();} }
        public Vector Vn1 { get { return vn1; } set { vn1 = value; } }
        public Vector Vn2 { get { return vn2; } set { vn2 = value; } }
        public Vector Vn3 { get { return vn3; } set { vn3 = value; } }
        public double[] Vt1 { get { return vt1; } set { vt1 = value; } }
        public double[] Vt2 { get { return vt2; } set { vt2 = value; } }
        public double[] Vt3 { get { return vt3; } set { vt3 = value; } }
        public Plane Planar { get { return plane; } }
        public BBox BoundingBox { get { return bBox; } }

        #endregion

        #region Constructors
        public Facet()
        {
        }

        public Facet(Vector v1, Vector v2, Vector v3,Vector vn1, Vector vn2, Vector vn3,
            double[] vt1, double[] vt2, double[] vt3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.vn1 = vn1;
            this.vn2 = vn2;
            this.vn3 = vn3;
            this.vt1 = vt1;
            this.vt2 = vt2;
            this.vt3 = vt3;
            update();
        }
        #endregion

        #region Methods

        private void update()
        {
            this.e1 = v2 - v1;
            this.e2 = v3 - v1;
            Vector N = e1.Cross(e2);
            N.Normalize();
            this.plane = new Plane(v1, N);
            this.zDenom = 1 / (this.e2.y * this.e1.x - this.e2.x * this.e1.y);
            this.bBox = new BBox(this);
        }

        private double[] getUVXYZ(double b1, double b2)
        {
            double b0 = 1 - b1 - b2;
            double[] UVXYZ = new double[5];
            UVXYZ[0] = b0 * vt1[0] + b1 * vt2[0] + b2 * vt3[0];
            UVXYZ[1] = b0 * vt1[1] + b1 * vt2[1] + b2 * vt3[1];
            UVXYZ[2] = b0 * vn1.x + b1 * vn2.x + b2 * vn3.x;
            UVXYZ[3] = b0 * vn1.y + b1 * vn2.y + b2 * vn3.y;
            UVXYZ[4] = b0 * vn1.z + b1 * vn2.z + b2 * vn3.z;
            return UVXYZ;
        }

        private double[] getUVZ(double b1, double b2)
        {
            double b0 = 1 - b1 - b2;
            double[] UVZ = new double[3];
            UVZ[0] = b0 * vt1[0] + b1 * vt2[0] + b2 * vt3[0];
            UVZ[1] = b0 * vt1[1] + b1 * vt2[1] + b2 * vt3[1];
            UVZ[2] = b0 * vn1.z + b1 * vn2.z + b2 * vn3.z;
            return UVZ;
        }

        #endregion

        //#region Transforms
        //public override void RotX(double Radians)
        //{
        //    v1.RotX(Radians); v2.RotX(Radians); v3.RotX(Radians);
        //    vn1.RotX(Radians); vn2.RotX(Radians); vn3.RotX(Radians);
        //    update();
        //}

        //public override void RotY(double Radians)
        //{
        //    v1.RotY(Radians); v2.RotY(Radians); v3.RotY(Radians);
        //    vn1.RotY(Radians); vn2.RotY(Radians); vn3.RotY(Radians);
        //    update();
        //}

        //public override void RotZ(double Radians)
        //{
        //    v1.RotZ(Radians);
        //    v2.RotZ(Radians);
        //    v3.RotZ(Radians);
        //    vn1.RotZ(Radians);
        //    vn2.RotZ(Radians);
        //    vn3.RotZ(Radians);
        //    update();
        //}

        //public override void Translate(Vector Shift)
        //{
        //    v1.Translate(Shift); v2.Translate(Shift); v3.Translate(Shift);
        //    update();
        //}

        //public override void Scale(Vector ScaleFactor)
        //{
        //    v1.Scale(ScaleFactor); v2.Scale(ScaleFactor); v3.Scale(ScaleFactor);
        //    update();
        //}
        //#endregion

        public override void Transform(Matrix4x4 m)
        {
            v1.Transform(m);
            v2.Transform(m);
            v3.Transform(m);
            vn1.Transform(m);
            vn2.Transform(m);
            vn3.Transform(m);
            update();
        }

        #region Ray Intersection

        // Barycentric coordinate system, Formulism: Phayy, Humphreys; Physically Based Rendering 2ed, 140-145
        private bool IntRay(Ray ray, out double t, out double b1, out double b2)
        {
            
            // file values
            t = double.MaxValue;
            b1 = b2 = 0;

            // Divisor of expression 3.5
            Vector s1 = ray.Dir.Cross(this.e2); // Coordiante substitutions
            double denom = s1.Dot(this.e1);
            if (denom == 0) // If zero, then plane is parallel to ray
                return false;
            double invDenom = 1 / denom; // For efficency, use inverse;

            // First Barcentric coordiante
            Vector d = new Vector(ray.Pt) - this.v1;
            b1 = d.Dot(s1) * invDenom;
            if (b1 < 0 || b1 > 1)
                return false;

            // Second Barcentric coordiante
            Vector s2 = d.Cross(this.e1);
            b2 = ray.Dir.Dot(s2) * invDenom;
            if (b2 < 0 || b1 + b2 > 1)
                return false;

            // At this point, the ray intesects the triangle, but may not be inbounds
            t = this.e2.Dot(s2) * invDenom; // Distance the ray intersects the triangle
            if (t <= 0 || t > double.MaxValue)
                return false;
            return true;
        }

        // Intesect method
        public override IntSectData Intersect(Ray ray, ref double nearDist)
        {
            IntSectData data = new IntSectData();

            // if true, looks from facet intersection
            double dist;
            double b1, b2;
            data.Hit = IntRay(ray, out dist, out b1, out b2);
            // returns intersection interface
            if (data.Hit)
            {
                // Checks for nearest
                if (dist >= nearDist || dist <= 0)
                {
                    data.Hit = false;
                    return data;
                }

                nearDist = dist;

                // UV data
                double[] UVXYZ = getUVXYZ(b1,b2);
                Vector XYZ = new Vector(UVXYZ[2], UVXYZ[3], UVXYZ[4]);

                data.Surface = this;
                data.ID = this.ID;
                data.Name = this.Name;
                data.Ray = ray;
                data.Dist = dist;
                data.IncAng = Math.Acos(-ray.Dir.Dot(XYZ.Normal()));
                data.UV = new double[] { UVXYZ[0], UVXYZ[1] }; // Replace with UV grid later
            }
            return data;
        }

        #endregion

        #region Z-buffer Intersection

        public bool zIntXY(double x, double y, ref double t, out double b0, out double b1)
        {


            bool hit = false;
            
            b0 = -1;
            b1 = -1;
            double alpha = -1;
            double beta = -1;

            double u0 = x - this.v1.x;
            double v0;
            if (this.e1.x ==0)
            {
                beta = u0 / this.e2.x;
                if (beta >= 0 && beta <= 1)
                {
                    v0 = y - this.v1.y;
                    alpha = (v0 - beta * this.e2.y) / this.e1.y;
                }
            }
            else
            {
                v0 = y - v1.y;
                beta = (v0 * this.e1.x - u0 * this.e1.y) * this.zDenom;
                if (beta >= 0 && beta <= 1)
                    alpha = (u0 - beta * e2.x) / e1.x;
            }
            if (alpha >= 0 && beta >= 0 && alpha + beta <= 1)
            {
                double dist = -(this.plane.D + this.plane.Norm.x * x + this.plane.Norm.y * y) / this.plane.Norm.z;
                if (dist > 0 && dist < t)
                {
                    hit = true;
                    t = dist;
                    b0 = alpha;
                    b1 = beta;
                }
            }
            return hit;
        }

        // Intesect method
        public override IntSectData zIntersect(double x, double y, ref double t)
        {
            IntSectData data = new IntSectData();

            //if (Available)
            //{
                if (bBox.zbufferIn(x, y, t))
                {
                    double b1, b2;
                    data.Hit = zIntXY(x, y, ref t, out b1, out b2);

                    // returns intersection interface
                    if (data.Hit)
                    {
                        // UV data
                        double[] UVZ = getUVZ(b1, b2);

                        data.Surface = this;
                        data.ID = this.ID;
                        data.Name = this.Name;
                        data.Dist = t;
                        data.IncAng = Math.Acos(-UVZ[2]);
                        data.UV = new double[] { UVZ[0], UVZ[1] }; // Replace with UV grid later
                    }
                //}
            }
            return data;
        }

        public override bool zInView(double xMinScn, double xMaxScn, double yMinScn, double yMaxScn)
        {
            this.Available = this.bBox.zInView(xMinScn, xMaxScn, yMinScn, yMaxScn);
            return this.Available;
        }

        #endregion
    }
}
