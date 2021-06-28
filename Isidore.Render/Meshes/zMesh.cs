using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    public partial class zMesh : zSurface
    {
        public String GroupName;
        public String MaterialName;
        private Vector[] v;
        private int vLen;
        private Vector[] vn;
        private double[,] vt;
        private int[,] f;
        private int fLen;
        private int[] inViewFac;
        private int inViewLen;
        private Vector[] N;
        private double[] D;
        private Plane[] plane;
        private Vector[] e1;
        private Vector[] e2;
        private double[] denom;
        private BBox[] fBox;
        private BBox mBox;
        private double[] zMin;
        private double[] xMin;
        private double[] yMin;
        private double[] zMax;
        private double[] xMax;
        private double[] yMax;
        private double zBoxMin;
        private double xBoxMin;
        private double yBoxMin;
        private double zBoxMax;
        private double xBoxMax;
        private double yBoxMax;

        public Vector[] V { get { return v; } }
        public Vector[] Vn { get { return vn; } }
        public double[,] Vt { get { return vt; } }
        public int[,] F { get { return f; } }

        public zMesh()
        {
        }

        public zMesh(List<String[]> v, List<String[]> vn, List<String[]> vt, List<String[]> f, ref int fOffset)
        {
            // vertices
            vLen = v.Count;
            this.v = new Vector[vLen];
            for (int Idx = 0; Idx < vLen; Idx++)
            {
                string[] vS = v[Idx];
                this.v[Idx].x = double.Parse(vS[0]);
                this.v[Idx].y = double.Parse(vS[1]);
                this.v[Idx].z = double.Parse(vS[2]);
            }

            // Normals
            this.vn = new Vector[vn.Count];
            for (int Idx = 0; Idx < vn.Count; Idx++)
            {
                string[] vnS = vn[Idx];
                this.vn[Idx].x = double.Parse(vnS[0]);
                this.vn[Idx].y = double.Parse(vnS[1]);
                this.vn[Idx].z = double.Parse(vnS[2]);
            }

            // texture
            this.vt = new double[vt.Count,2];
            for (int Idx = 0; Idx < vt.Count; Idx++)
            {
                string[] vtS = vt[Idx];
                this.vt[Idx,0] = double.Parse(vtS[0]);
                this.vt[Idx,1] = double.Parse(vtS[1]);
            }

            // facets
            fLen = f.Count;
            this.f = new int[fLen, 3];
            this.N = new Vector[fLen];
            this.plane = new Plane[fLen];
            for (int Idx = 0; Idx < fLen; Idx++)
            {
                string[] fS = f[Idx];
                this.f[Idx, 0] = int.Parse(fS[0]) - (fOffset);
                this.f[Idx, 1] = int.Parse(fS[3]) - (fOffset);
                this.f[Idx, 2] = int.Parse(fS[6]) - (fOffset);
                Vector v0v1 = this.v[this.f[Idx, 1]] - this.v[this.f[Idx, 0]];
                Vector v0v2 = this.v[this.f[Idx, 2]] - this.v[this.f[Idx, 0]];
                this.N[Idx] = v0v1.Cross(v0v2); // Planar normal
            }

            update();

            // updates facet count offset
            fOffset += vLen;
        }
    }


    public class zMeshes : List<zMesh>
    {

    }
}
