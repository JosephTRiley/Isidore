using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    public class Pixel
    {
        public List<IntSect> SurfaceHitList;
        public int HitCount = 0;
        public Rays RayBundle;

        /// <summary>
        /// Pixel class constructor for an orthogonal pixel (IFOV= 0)
        /// Pixels are defaults in the XY plane and must be rotated to the desired orientation.
        /// </summary>
        /// <param name="location"> Center location of the pixel</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="MaxRayCount"></param>
        public Pixel(Vector location, double width, double height, int MaxRayCount)
        {
            // First ray is always in the pixel center
            Vector Norm = new Vector(0, 0, 0);
            Ray oneRay = new Ray(location, Norm);
            RayBundle.Add(oneRay);

            // Builds Rays
            if (MaxRayCount > 1)
            {
                Random rand = new Random();// Random offset
                // Generates extra ray bundle
                for (int rayCnt = 1; rayCnt < MaxRayCount; rayCnt++)
                {
                    Ray moreRay = new Ray(oneRay);
                    double dw = rand.NextDouble() * width / 2;
                    double dh = rand.NextDouble() * height / 2;
                    // adds offset to planar pixel
                    moreRay.Pt.x += dw;
                    moreRay.Pt.y += dh;
                    RayBundle.Add(moreRay);
                }
            }
        }
    }

    public class Pixels : List<Pixel>
    {
    }
}
