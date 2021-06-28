using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    public partial class zMesh : zSurface
    {
        private void update()
        {
            D = new double[fLen]; 
            e1 = new Vector[fLen]; e2 = new Vector[fLen]; denom = new double[fLen];
            zMin = new double[fLen]; yMin = new double[fLen]; xMin = new double[fLen];
            zMax = new double[fLen]; yMax = new double[fLen]; xMax = new double[fLen];
            xBoxMin = double.MaxValue; yBoxMin = double.MaxValue; zBoxMin = double.MaxValue;
            xBoxMax = double.MinValue; yBoxMax = double.MinValue; xBoxMax = double.MinValue;
            for (int Idx = 0; Idx < fLen; Idx++)
            {
                // Barycentric coordinates
                Vector vp0 = v[f[Idx, 0]];
                Vector vp1 = v[f[Idx, 1]];
                Vector vp2 = v[f[Idx, 2]];
                D[Idx] = -(vp1.x * N[Idx].x + vp1.y * N[Idx].y + vp1.z * N[Idx].z);
                e1[Idx] = vp1 - vp0;
                e2[Idx] = vp2 - vp0;
                denom[Idx] = 1 / (e2[Idx].y * e1[Idx].x - e2[Idx].x * e1[Idx].y);
                // Min/Max bounding boxes
                zMin[Idx] = (v[f[Idx, 0]].z < v[f[Idx, 1]].z) ? v[f[Idx, 0]].z : v[f[Idx, 1]].z;
                zMin[Idx] = (v[f[Idx, 2]].z < zMin[Idx]) ? v[f[Idx, 2]].z : zMin[Idx];
                yMin[Idx] = (v[f[Idx, 0]].y < v[f[Idx, 1]].y) ? v[f[Idx, 0]].y : v[f[Idx, 1]].y;
                yMin[Idx] = (v[f[Idx, 2]].y < yMin[Idx]) ? v[f[Idx, 2]].y : yMin[Idx];
                xMin[Idx] = (v[f[Idx, 0]].x < v[f[Idx, 1]].x) ? v[f[Idx, 0]].x : v[f[Idx, 1]].x;
                xMin[Idx] = (v[f[Idx, 2]].x < xMin[Idx]) ? v[f[Idx, 2]].x : xMin[Idx];
                zMax[Idx] = (v[f[Idx, 0]].z > v[f[Idx, 1]].y) ? v[f[Idx, 0]].z : v[f[Idx, 1]].z;
                zMax[Idx] = (v[f[Idx, 2]].z > zMax[Idx]) ? v[f[Idx, 2]].z : zMax[Idx];
                yMax[Idx] = (v[f[Idx, 0]].y > v[f[Idx, 1]].y) ? v[f[Idx, 0]].y : v[f[Idx, 1]].y;
                yMax[Idx] = (v[f[Idx, 2]].y > yMax[Idx]) ? v[f[Idx, 2]].y : yMax[Idx];
                xMax[Idx] = (v[f[Idx, 0]].x > v[f[Idx, 1]].x) ? v[f[Idx, 0]].x : v[f[Idx, 1]].x;
                xMax[Idx] = (v[f[Idx, 2]].x > xMax[Idx]) ? v[f[Idx, 2]].x : xMax[Idx];
                zBoxMin = (zMin[Idx] < zBoxMin) ? zMin[Idx] : zBoxMin;
                xBoxMin = (xMin[Idx] < xBoxMin) ? xMin[Idx] : xBoxMin;
                yBoxMin = (yMin[Idx] < yBoxMin) ? yMin[Idx] : yBoxMin;
                zBoxMax = (zMax[Idx] > zBoxMax) ? zMax[Idx] : zBoxMax;
                xBoxMax = (xMax[Idx] > xBoxMax) ? xMax[Idx] : xBoxMax;
                yBoxMax = (yMax[Idx] > yBoxMax) ? yMax[Idx] : yBoxMax;
            }
            AnchorPoint = new Point(new Vector(0.5 * (zBoxMax + zBoxMin), 0.5 * (yBoxMax + yBoxMin), 0.5 * (zBoxMax + zBoxMin)));
        }
    }
}
