using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    public partial class zMesh : zSurface
    {

        public bool IntXY(double x, double y, ref double t, out int fLoc, out double Alpha, out double Beta)
        {
            bool hit = false;
            double u0;
            double v0;
            fLoc = -1;
            Alpha = -1;
            Beta = -1;
            double alpha = -1;
            double beta = -1;

            for (int fIdx = 0; fIdx < inViewLen; fIdx++)
            {
                int Idx = inViewFac[fIdx];
                if (zMin[Idx] < t && xMin[Idx] <= x && xMax[Idx] >= x && yMin[Idx] <= y && yMax[Idx] >= y)
                {
                    u0 = x - v[f[Idx, 0]].x;
                    if(e1[Idx].x==0)
                    {
                        beta = u0 / e2[Idx].x;
                        if (beta >= 0 && beta <= 1)
                        {
                            v0 = y - v[f[Idx, 0]].y;
                            alpha = (v0 - beta * e2[Idx].y) / e1[Idx].y;
                        }
                    }
                    else
                    {
                        v0 = y - v[f[Idx, 0]].y;
                        beta = (v0 * e1[Idx].x - u0 * e1[Idx].y) * denom[Idx];
                        if (beta >= 0 && beta <= 1)
                            alpha = (u0 - beta * e2[Idx].x) / e1[Idx].x;
                    }
                    if (alpha >= 0 && beta >= 0 && alpha + beta <= 1)
                    {
                        double this_t = -(D[Idx] + N[Idx].x * x + N[Idx].y * y) / N[Idx].z;
                        if (this_t < t && this_t > 0)
                        {
                            hit = true;
                            t = this_t;
                            fLoc = Idx;
                            Alpha = alpha;
                            Beta = beta;
                        }
                    }
                }
            }
            return hit;
        }

        // Intesect method
        public override IntSectData Intersect(double x, double y, ref double t)
        {
            IntSectData data = new IntSectData();

            if (Available)
            {
                if (xBoxMin <= x && xBoxMax >= x && yBoxMin <= y && yBoxMax >= y && zBoxMin < t)
                {
                    double b1, b2;
                    int fLoc;
                    data.Hit = IntXY(x, y, ref t, out fLoc, out b1, out b2);

                    // returns intersection interface
                    if (data.Hit)
                    {
                        // UV data
                        double b0 = 1 - b1 - b2;
                        double U = b0 * vt[f[fLoc, 0], 0] + b1 * vt[f[fLoc, 1], 0] + b2 * vt[f[fLoc, 2], 0];
                        double V = b0 * vt[f[fLoc, 0], 1] + b1 * vt[f[fLoc, 1], 1] + b2 * vt[f[fLoc, 2], 1];
                        double Z = b0 * vn[f[fLoc, 0]].z + b1 * vn[f[fLoc, 1]].z + b2 * vn[f[fLoc, 2]].z;

                        data.ID = this.ID;
                        data.Name = this.Name;
                        data.Dist = t;
                        data.IncAng = Math.Acos(-Z);
                        data.UV = new double[] { U, V }; // Replace with UV grid later
                    }
                }
            }
            return data;
        }

        public override bool inView(double xMinScn, double xMaxScn, double yMinScn, double yMaxScn)
        {
            // Bounding box check
            if (xBoxMax < xMinScn || yBoxMax < yBoxMin || zBoxMax < 0)
            {
                Available = false;
                return false;
            }
            if (xBoxMin > xMaxScn || yBoxMin > yMaxScn)
            {
                Available = false;
                return false;
            }

            // facet in view processing
            inViewLen = 0;
            int[] inFac = new int[fLen];
            for (int Idx = 0; Idx < fLen; Idx++)
            {
                if (zMax[Idx] > 0)
                    // overlapping dimension
                    if(xMin[Idx] <= xMaxScn && xMax[Idx] >= xMinScn)
                        if (yMin[Idx] <= yMaxScn && yMax[Idx] >= yMinScn)
                        {
                            inFac[inViewLen] = Idx;
                            inViewLen++;
                        }
            }

            inViewFac = new int[inViewLen];
            if (inViewLen > 0)
            {
                Available = true;
                for (int Idx = 0; Idx < inViewLen; Idx++)
                    inViewFac[Idx] = inFac[Idx];
                return true;
            }

            Available = false;
            return false;
        }
    }
}
