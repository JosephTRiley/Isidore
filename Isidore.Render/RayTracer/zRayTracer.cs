using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace RayTracer
{
    // z buffer
    public partial class RayTracer
    {
        /*public void zRayTraceV1(Scene scene, out double[,] Depth, out double[,] IncAng, out double[,] U, out double[,] V, out int[,] ID, out string[] IDList)
        {

            // If screen isn't on origin, rotates scene to match
            if ((scene.Screen.Pt.Pos != Vector.Null))
            {
                Vector svec = scene.Screen.Pt.Pos;
                svec *= -1;
                foreach (zSurface surf in scene.zSurfaces)
                    surf.Translate(svec);
            }
            if ((scene.Screen.Pt.Dir != Vector.Unit))
            {
                throw new Exception("Screen point down the Zaxis");
            }

            // Tracer section
            int xLen = scene.Screen.xAxis.Length;
            int yLen = scene.Screen.yAxis.Length;
            Depth = new double[xLen, yLen];
            IncAng = new double[xLen, yLen];
            U = new double[xLen, yLen];
            V = new double[xLen, yLen];
            ID = new int[xLen, yLen]; // objectID

            // Creates ID list
            IDList = new string[scene.zSurfaces.Count + 1];
            IDList[0] = "Miss"; // zero value is a miss
            for (int idx = 0; idx < scene.zSurfaces.Count; idx++)
            {
                IDList[idx + 1] = scene.zSurfaces[idx].Name;
                scene.zSurfaces[idx].ID = idx + 1;
            }

            // finds surfaces in view
            double xMin = scene.Screen.xAxis[0];
            double xMax = scene.Screen.xAxis[scene.Screen.xAxis.Length - 1];
            double yMin = scene.Screen.yAxis[0];
            double yMax = scene.Screen.yAxis[scene.Screen.yAxis.Length - 1];
            foreach (zSurface zSurf in scene.zSurfaces)
                zSurf.inView(xMin, xMax, yMin, yMax);

            // Calls tracer
            bool para = false;//true;
            if (para)
                pzTraceV1(scene, Depth, IncAng, U, V, ID);
            else
                zTraceV1(scene, Depth, IncAng, U, V, ID);

        }

        // Serial tracer
        public void zTraceV1(Scene scene, double[,] Depth, double[,] IncAng, 
            double[,] U, double[,] V, int[,] ID)
        {
            int xLen = scene.Screen.xAxis.Length;
            int yLen = scene.Screen.yAxis.Length;
            for (int xIdx = 0; xIdx < xLen; xIdx++)
            {
                double x = scene.Screen.xAxis[xIdx];
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                {
                    // Buffer call
                    IntSectData nearest = zBuffSurfV1(x, scene.Screen.yAxis[yIdx], scene);
                    // if hit is detected, records data
                    if (nearest.Hit)
                    {
                        Depth[xIdx, yIdx] = nearest.Dist;
                        IncAng[xIdx, yIdx] = nearest.IncAng;
                        U[xIdx, yIdx] = nearest.UV[0];
                        V[xIdx, yIdx] = nearest.UV[1];
                        ID[xIdx, yIdx] = nearest.ID;
                    }
                }
            }
        }

        // Parallel tracer
        public void pzTraceV1(Scene scene, double[,] Depth, double[,] IncAng, 
            double[,] U, double[,] V, int[,] ID)
        {
            //Console.WriteLine("Parallel Tag");
            int xLen = scene.Screen.xAxis.Length;
            int yLen = scene.Screen.yAxis.Length;
            Parallel.For(0, xLen, xIdx =>
            {
                double x = scene.Screen.xAxis[xIdx];
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                {
                    // Buffer call
                    double y = scene.Screen.yAxis[yIdx];
                    IntSectData nearest = zBuffSurfV1(x, y, scene);
                    // if hit is detected, records data
                    if (nearest.Hit)
                    {
                        Depth[xIdx, yIdx] = nearest.Dist;
                        IncAng[xIdx, yIdx] = nearest.IncAng;
                        U[xIdx, yIdx] = nearest.UV[0];
                        V[xIdx, yIdx] = nearest.UV[1];
                        ID[xIdx, yIdx] = nearest.ID;
                    }
                }
            });
        }

        // Serial buffer
        public IntSectData zBuffSurfV1(double x, double y, Scene scene)
        {
            IntSectData nearest = new IntSectData();
            double dist = double.MaxValue;
            foreach (IzSurface surface in scene.zSurfaces)
            {
                IntSectData surfIntData = surface.Intersect(x, y, ref dist);
                if (surfIntData.Hit)
                    nearest = surfIntData;
            }
            return nearest;
        }*/

        ////////////////////////////////////////////////
        // different version
        ////////////////////////////////////////////////

        // interface for z-Buffers are nice because they're so damn fast
        public void zRayTrace(Scene scene, out double[,] Depth, out double[,] IncAng, out double[,] U, out double[,] V, out int[,] ID, out string[] IDList)
        {

            // If screen isn't on origin, rotates scene to match
            if ((scene.Screen.Pt != Point.Null) || (scene.Screen.Dir != Vector.Unit))
            {
                Point lookPt = new Point(scene.Screen.Dir);
                lookPt = lookPt + scene.Screen.Pt;
                Transform trans = new Transform();
                Vector up = new Vector(0,1,0);
                Point sPt = scene.Screen.Pt;
                trans.LookAt(ref sPt, ref lookPt, ref up);
                //throw new Exception("Screen point down the Zaxis");
            }

            // Tracer section
            int xLen = scene.Screen.xAxis.Length;
            int yLen = scene.Screen.yAxis.Length;
            Depth = new double[xLen, yLen];
            IncAng = new double[xLen, yLen];
            U = new double[xLen, yLen];
            V = new double[xLen, yLen];
            ID = new int[xLen, yLen]; // objectID

            // Creates ID list
            IDList = new string[scene.Surfaces.Count + 1];
            IDList[0] = "Miss"; // zero value is a miss
            for (int idx = 0; idx < scene.Surfaces.Count; idx++)
            {
                IDList[idx + 1] = scene.Surfaces[idx].Name;
                scene.Surfaces[idx].ID = idx + 1;
            }

            // finds surfaces in view
            double xMin = scene.Screen.xAxis[0];
            double xMax = scene.Screen.xAxis[scene.Screen.xAxis.Length - 1];
            double yMin = scene.Screen.yAxis[0];
            double yMax = scene.Screen.yAxis[scene.Screen.yAxis.Length - 1];
            foreach (Surface surf in scene.Surfaces)
                surf.zInView(xMin, xMax, yMin, yMax);

            // Calls tracer
                //pzTrace(scene, Depth, IncAng, U, V, ID);
                zTrace(scene, Depth, IncAng, U, V, ID);
        }

        // Serial tracer
        public void zTrace(Scene scene, double[,] Depth, double[,] IncAng,
            double[,] U, double[,] V, int[,] ID)
        {
            int xLen = scene.Screen.xAxis.Length;
            int yLen = scene.Screen.yAxis.Length;
            for (int xIdx = 0; xIdx < xLen; xIdx++)
            {
                double x = scene.Screen.xAxis[xIdx];
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                {
                    // Buffer call
                    IntSectData nearest = zBuffSurf(x, scene.Screen.yAxis[yIdx], scene);
                    // if hit is detected, records data
                    if (nearest.Hit)
                    {
                        Depth[xIdx, yIdx] = nearest.Dist;
                        IncAng[xIdx, yIdx] = nearest.IncAng;
                        U[xIdx, yIdx] = nearest.UV[0];
                        V[xIdx, yIdx] = nearest.UV[1];
                        ID[xIdx, yIdx] = nearest.ID;
                    }
                }
            }
        }

        // Parallel tracer
        public void pzTrace(Scene scene, double[,] Depth, double[,] IncAng,
            double[,] U, double[,] V, int[,] ID)
        {
            //Console.WriteLine("Parallel Tag");
            int xLen = scene.Screen.xAxis.Length;
            int yLen = scene.Screen.yAxis.Length;
            Parallel.For(0, xLen, xIdx =>
            {
                double x = scene.Screen.xAxis[xIdx];
                for (int yIdx = 0; yIdx < yLen; yIdx++)
                {
                    // Buffer call
                    double y = scene.Screen.yAxis[yIdx];
                    IntSectData nearest = zBuffSurf(x, y, scene);
                    // if hit is detected, records data
                    if (nearest.Hit)
                    {
                        Depth[xIdx, yIdx] = nearest.Dist;
                        IncAng[xIdx, yIdx] = nearest.IncAng;
                        U[xIdx, yIdx] = nearest.UV[0];
                        V[xIdx, yIdx] = nearest.UV[1];
                        ID[xIdx, yIdx] = nearest.ID;
                    }
                }
            });
        }

        public IntSectData zBuffSurf(double x, double y, Scene scene)
        {
            IntSectData nearest = new IntSectData();
            double dist = double.MaxValue;
            foreach (ISurface surface in scene.Surfaces)
            {
                IntSectData surfIntData = surface.zIntersect(x, y, ref dist);
                if (surfIntData.Hit)
                    nearest = surfIntData;
            }
            return nearest;
        }
    }
}
