using System;
using Isidore.Maths;

namespace Isidore_Tests
{
    class PlaneTest
    {
        public static bool Run()
        {
            Console.WriteLine("Evaluating Plane");

            double threshErr = 0.0001; // Acceptable error (1 = 100%)

            //////////////////////////////////
            ////// Checks point to plane
            //////////////////////////////////

            // Test points
            Points pts = new Points();
            pts.Add(new Point(-9, 25, 11));
            pts.Add(new Point(12, 24, 0));
            pts.Add(new Point(-11, -5, -4));
            pts.Add(new Point(1, -18, -13));

            // Corresponding Distances
            double[] testDist = new double[] { -9.4869, -24.0, 7.5179, 7.3708};

            // Plane Definition
            double  x = -0.500, y = -0.7500, z = 0.4330;
            Normal N = new Normal(x, y, z);
            N.Normalize();
            Point planePt = Point.Zero();
            Plane plane = new Plane(planePt, N);

            // Calculated distance
            double[] dist = new double[testDist.Length];
            for (int idx = 0; idx < testDist.Length; idx++)
                dist[idx] = plane.DistanceToPoint(pts[idx]);

            // Checks distances to truth
            double[] perDiff = new double[testDist.Length];
            bool[] good = new bool[testDist.Length];
            for (int idx = 0; idx < testDist.Length; idx++)
            {
                perDiff[idx] = Aids.perErr(testDist[idx], dist[idx]);
                good[idx] = perDiff[idx] <= threshErr;
            }

            // If any difference is larger than threshold, returns false
            for (int idx = 0; idx < testDist.Length; idx++)
                if(!good[idx])
                {
                    Console.WriteLine("Point to Plane Distance Check Failed");
                    return false;
                }

            Console.WriteLine("Point to Plane Distance Check Successful");

            //////////////////////////////////
            ////// Checks ray intersect
            //////////////////////////////////

            Point rayEnd0 = new Point(-17, -16, -12);
            Point rayEnd1 = new Point(28, 13, 18);
            // Front facing ray
            Vector rayDir0 = new Vector(rayEnd1 - rayEnd0);
            rayDir0.Normalize();
            Ray rayFront = new Ray(rayEnd0, rayDir0);
            // Back facing ray
            Vector rayDir1 = new Vector(rayEnd0 - rayEnd1);
            rayDir1.Normalize();
            Ray rayBack = new Ray(rayEnd1, rayDir1);

            double testInt = 30.0440; // Theoretical travel until intersection

            // Calculates travel
            var interData = plane.RayIntersection(rayFront);
            double intDiff = Aids.perErr(interData.Item1, testInt);

            // Checks that we miss the back face
            var interDataBack = plane.RayIntersection(rayBack, false);
            var interDataBack1 = Plane.RayIntersection(plane.Normal,
            plane.D, rayBack); // This one does interest
            var interDataBack2 = Plane.RayIntersection(-1*plane.Normal,
            -plane.D, rayBack); // This one does interest

            if (intDiff > threshErr || !double.IsNaN(interDataBack.Item1))
            {
                Console.WriteLine("Ray/Plane Intersection Check Failed");
                return false;
            }

            ///////////////////////////////
            ////// Checks a miss ray/plane
            ///////////////////////////////
            Ray rayMiss = new Ray(new Point(0, 0, 10),
                new Vector(0, 0, 1));

            // Excludes back face interface
            var interDataMiss = plane.RayIntersection(rayMiss, false);
            var interDataMiss1 = Plane.RayIntersection(plane.Normal,
            plane.D, rayMiss);

            if(!double.IsNaN(interDataMiss.Item1) || 
                !double.IsNaN(interDataMiss1.Item1))
            {
                Console.WriteLine("Ray/Plane Intersection Miss Check Failed");
                return false;
            }

            Console.WriteLine("Ray/Plane Intersection Check Successful");
            return true;
        }

        //private static double perErr(Point val, Point truth)
        //{
        //    // I think this works
        //    double maxDelta = -1, maxVal = 0, maxTruth = 0;
        //    for(int idx = 0; idx < val.Comp.Length; idx++)
        //    {
        //        double thisDelta = Math.Abs(val.Comp[idx] - truth.Comp[idx]);
        //        if(thisDelta > maxDelta)
        //        {
        //            maxDelta = thisDelta;
        //            maxVal = val.Comp[idx];
        //            maxTruth = truth.Comp[idx];
        //        }
        //    }

        //    return Aids.perErr(maxVal, maxTruth);
        //}
    }
}
