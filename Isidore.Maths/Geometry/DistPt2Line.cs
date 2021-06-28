using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Finds the distance between basic geometric data types
    /// </summary>
    public partial class Distance
    {
        /// <summary>
        /// Finds the closest approach of a line with a point, returning
        /// the nearest distance, closest point on line, travel distance 
        /// from the origin, and the line directional vector.
        /// </summary>
        /// <param name="Point"> Point </param>
        /// <param name="Origin"> Line origin </param>
        /// <param name="Dir"> Line direction</param>
        /// <returns> A tuple containing the nearest distance, closest 
        /// point, travel distance from the origin, and the line 
        /// directional vector. </returns>
        public static Tuple<double, Point, double> Point2Line(Point Point, 
            Point Origin, Vector Dir)
        {
            // Checks if direction has been normalized
            // Should also normalize Dir via reference
            if (Dir.MagSq() != 1) Dir.Normalize();

            // Finds distance propagated to achieve closest 
            // approach distance
            double tca = Vector.Dot(Dir, (Vector)(Point - Origin));

            // Closest point 
            Point closestPoint;
            // If tca < 0. then the line is heading away from the sphere
            if(tca <= 0 )
                closestPoint = Origin.Clone();
            else // If tca > 0, the closest point is along the ray
                closestPoint = Origin + (Point)(tca * Dir);

            // Closest distance
            double distance = Math.Sqrt(Point.DistSquared(closestPoint));

            // Formats data
            Tuple<double, Point, double> output = 
                new Tuple<double, Point, double>(distance, 
                closestPoint, tca);

            return output;
        }

        /// <summary>
        /// Finds the closest approach of a line with a point, returning
        /// the nearest distance, closest point on line, travel distance 
        /// from the origin, and the line directional vector.
        /// </summary>
        /// <param name="Point"> Point </param>
        /// <param name="Pt0"> First line endpoint </param>
        /// <param name="Pt1"> Second line endpoint </param>
        /// <returns> A tuple containing the nearest distance, 
        /// closest point, travel distance from the origin, and the line 
        /// directional vector. </returns>
        public static Tuple<double, Point, double, Vector> Point2Line(
            Point Point, Point Pt0, Point Pt1)
        {
            // Converts line to a ray
            Point deltaPt = Pt1 - Pt0; // Separation of the two points
            Vector Dir = new Vector(deltaPt);

            // Normalizes
            Dir.Normalize();

            // Calls DistPt2Line (above)
            var data = Point2Line(Point, Pt0, Dir);

            // If tca <0, then the closest point is the first point
            if(data.Item3 < 0)
            {
                double dist = Point.Distance(Pt0);
                return new Tuple<double, Point, double, Vector>
                    (dist, Pt0.Clone(), 0, Dir);
            }

            // Otherwise, the line might terminate before 
            // reaching the sphere
            double lineLenSq = Pt1.DistSquared(Pt0);

            // If the travel is less than the line length then 
            // the data is fine
            if (lineLenSq >= data.Item3 * data.Item3)
                return new Tuple<double, Point, double, Vector>
                (data.Item1, data.Item2, data.Item3, Dir);

            // Otherwise the closest point is the end point
            double closestDist = Point.Distance(Pt1);
            return new Tuple<double, Point, double, Vector>
                (closestDist, Pt1.Clone(), Math.Sqrt(lineLenSq), Dir);

        }
    }
}
