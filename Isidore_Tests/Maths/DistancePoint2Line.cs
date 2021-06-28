using System;
using Isidore.Maths;

namespace Isidore_Tests
{
    class DistancePoint2Line
    {
        public static bool Run()
        {
            Console.WriteLine("Evaluating Point to Line Distances");

            double threshErr = 0.0001; // Acceptable error (1 = 100%)

            // Test Data
            Point Point = Point.Zero();
            Point P0 = new Point(-13.0, 9.0, 12.0);
            Point P1 = new Point(15, -7, -6);
            Vector Dir = (Vector)(P1-P0);
            Dir.Normalize();
            double tcaA = (-5 - P0.Comp[0])/Dir.Comp[0];
            double tcaB = (5 - P0.Comp[0])/Dir.Comp[0];
            Point P1A = new Point(P0 + (Point)(tcaA*Dir));
            Point P0A = new Point(P0 + (Point)(tcaB*Dir));

            // Full propagation
            var fullProp = Distance.Point2Line(Point, P0, Dir);

            //  Full prop Truth values
            double truthDist = 3.115564935615240;
            Point truthCA = new Point(1.862170087976541, 0.507331378299121, 2.445747800586510);
            double truthTca = 19.603399070874541;
            double tca = truthTca;

            // Error check
            double perErrDist = perErr(fullProp.Item1, truthDist);
            double perErrCA = perErr(fullProp.Item2, truthCA);
            double perErrTca = perErr(fullProp.Item3, truthTca);
            if (perErrDist > threshErr || perErrCA > threshErr || perErrTca > threshErr)
            {
                Console.WriteLine("Point to Line Distance Check Failed");
                return false;
            }

            // Full prop using points
            var fullPtProp = Distance.Point2Line(Point, P0, P1);

            perErrDist = perErr(fullPtProp.Item1, truthDist);
            perErrCA = perErr(fullPtProp.Item2, truthCA);
            perErrTca = perErr(fullPtProp.Item3, truthTca);
            if (perErrDist > threshErr || perErrCA > threshErr || perErrTca > threshErr)
            {
                Console.WriteLine("Point to Line Distance Check Failed");
                return false;
            }

            // Checks near termination
            var shortProp = Distance.Point2Line(Point, P0, P1A);
            truthDist = Point.Distance(P1A);
            truthCA = P1A;
            truthTca = tcaA;
            perErrDist = perErr(shortProp.Item1, truthDist);
            perErrCA = perErr(shortProp.Item2, truthCA);
            perErrTca = perErr(shortProp.Item3, truthTca);
            if (perErrDist > threshErr || perErrCA > threshErr || perErrTca > threshErr)
            {
                Console.WriteLine("Point to Line Distance Check Failed");
                return false;
            }

            // Checks far termination
            var farProp = Distance.Point2Line(Point, P0A, P1);
            truthDist = Point.Distance(P0A);
            truthCA = P0A;
            truthTca = 0;
            perErrDist = perErr(farProp.Item1, truthDist);
            perErrCA = perErr(farProp.Item2, truthCA);
            perErrTca = farProp.Item3 - truthTca; // both are zero
            if (perErrDist > threshErr || perErrCA > threshErr || perErrTca > threshErr)
            {
                Console.WriteLine("Point to Line Distance Check Failed");
                return false;
            }

            Console.WriteLine("Point to Line Distance Check Successful");
            return true;
        }

        private static double perErr(double val, double truth)
        {
            return Math.Abs((val - truth)/val);
        }

        private static double perErr(Point val, Point truth)
        {
            // I think this works
            double maxDelta = -1, maxVal = 0, maxTruth = 0;
            for(int idx = 0; idx < val.Comp.Length; idx++)
            {
                double thisDelta = Math.Abs(val.Comp[idx] - truth.Comp[idx]);
                if(thisDelta > maxDelta)
                {
                    maxDelta = thisDelta;
                    maxVal = val.Comp[idx];
                    maxTruth = truth.Comp[idx];
                }
            }

            return perErr(maxVal, maxTruth);
        }
    }
}
