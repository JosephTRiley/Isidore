using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Provides a definition for performing basic sphere calculations
    /// </summary>
    public class Sphere
    {
        #region Fields & Properties

        /// <summary>
        /// Sphere's center or radius, effectively the transformation
        /// matrix translation
        /// </summary>
        private Point center;

        /// <summary>
        /// Sphere's radius
        /// </summary>
        private double radius;

        /// <summary>
        /// Sphere's radius squared
        /// </summary>
        private double r2;

        /// <summary>
        /// Used for returning no intersects indicators
        /// </summary>
        private Tuple<double[], double[], Point[], Normal[]> noInter = 
            new Tuple<double[], double[], Point[], Normal[]>(
            new double[]{ double.NaN, double.NaN }, 
            new double[]{ double.NaN, double.NaN },
            new Point[]{ Point.NaN(), Point.NaN() }, 
            new Normal[]{ Normal.NaN(), Normal.NaN() });

        /// <summary>
        /// Sphere's center of radius.
        /// </summary>
        public Point Center { get { return center; } }

        /// <summary>
        /// Sphere's radius.
        /// </summary>
        public double Radius { get { return radius; } }

        # endregion Fields & Properties
        #region Constructors

        /// <summary>
        /// Sphere Constructor
        /// </summary>
        /// <param name="Center"> Sphere's center or radius </param>
        /// <param name="Radius"> Sphere's radius </param>
        public Sphere(Point Center = null, double Radius = 1.0)
        {
            // We always clone to avoid any unintended automatic
            // referencing issues
            this.center = Center ?? Point.Zero(3);
            this.radius = Radius; // Radius = 1.0 by default
            this.r2 = Radius * Radius; // Radius squared
        }

        #endregion Constructors
        # region Methods

        /// <summary>
        /// Finds both intersection points between a sphere and ray.
        /// Returns a tuple containing the propagation distances, 
        /// cosine of the angle of incidence, intersection point, 
        /// and the surface normal.  NaNs returned if sphere is missed.
        /// </summary>
        /// <param name="sphere"> Sphere to be intersected </param>
        /// <param name="ray"> Ray to intersect </param>
        /// <returns> Tuple containing the propagation distance, 
        /// cosine of the object of incidence, intersection point, 
        /// and the surface normal </returns>
        public static Tuple<double[], double[], Point[], Normal[]> 
            RayIntersection(Sphere sphere, Ray ray)
        {
            // This is the optimized solution presented in 
            // Real -Time Rendering (section 16.6.2)
            // The code is matched to the illustration in figure 16.6
            // This is superior because it doesn't require transformation
            // and the quadratic is applied only after the intersect test

            // Spacing between the sphere center and ray origin
            Vector l = new Vector(sphere.center - ray.Origin);
            // adjacent edge of triangle formed by l and m
            double s = l.Dot(ray.Dir);
            double l2 = l.Dot(l);

            // If the length from the ray origin to the sphere is negative
            // And the leg "l" is less than the radius, then it's a miss
            if (s < 0 && l2 > sphere.r2)
                return Tuple.Create(new double[]{ double.NaN, double.NaN }, 
                    new double[]{ double.NaN, double.NaN },
                    new Point[]{ Point.NaN(), Point.NaN() }, 
                    new Normal[]{ Normal.NaN(), Normal.NaN() });

            // Opposite side
            double m2 = l2 - s * s;

            // If the opposite side is greater than the radius, it's a miss
            if (m2 > sphere.r2)
                return Tuple.Create(new double[]{ double.NaN, double.NaN }, 
                    new double[]{ double.NaN, double.NaN },
                    new Point[]{ Point.NaN(), Point.NaN() }, 
                    new Normal[]{ Normal.NaN(), Normal.NaN() });

            // Last side of the triangle
            double q = Math.Sqrt(sphere.r2 - m2);

            // Finds the travel
            double[] t = new double[] { s - q, s + q }; // travel distances

            // Finds the cosine of incidence
            double[] cosIncAng = new double[]{ q / sphere.radius,
                -q/sphere.radius};

            // Finds the intersection point
            Point[] intPt = new Point[] { ray.Propagate(t[0]),
                ray.Propagate(t[1]) };

            // Finds surface normal
            Normal[] normal = new Normal[]{(Normal)(intPt[0] - 
                sphere.center), (Normal)(intPt[1] - sphere.center)};
            normal[0].Normalize();
            normal[1].Normalize();

            return Tuple.Create(t, cosIncAng, intPt, normal);
        }

        /// <summary>
        /// Finds both intersection points between this sphere and a ray.
        /// Returns a tuple containing the propagation distances, 
        /// cosine of the angle of incidences, intersection points, 
        /// and the surface normals.
        /// NaNs returned if sphere is missed.
        /// </summary>
        /// <param name="ray"> Ray to intersect </param>
        /// <returns> Tuple containing the propagation distance, 
        /// cosine of the object of incidence, intersection point, 
        /// and the surface normal </returns>
        public Tuple<double[], double[], Point[], Normal[]> 
            RayIntersection(Ray ray)
        {
            return RayIntersection(this, ray);
        }

        /// <summary>
        /// Finds the nearest intersection point between a sphere and ray.
        /// Returns a tuple containing the propagation distance, 
        /// cosine of the angle of incidence, intersection point, 
        /// and the surface normal.
        /// NaNs returned if sphere is missed.
        /// </summary>
        /// <param name="sphere"> Sphere to be intersected </param>
        /// <param name="ray"> Ray to intersect </param>
        /// <returns> Tuple containing the propagation distance, 
        /// cosine of the object of incidence, intersection point, 
        /// and the surface normal </returns>
        public static Tuple<double, double, Point, Normal> 
            NearestRayIntersection(Sphere sphere, Ray ray)
        {
            Tuple<double[], double[], Point[], Normal[]> both = RayIntersection(sphere, ray);
            // Picks the closest intersection
            if (both.Item1[1] < both.Item1[0])
                return Tuple.Create(both.Item1[1], both.Item2[1], 
                    both.Item3[1], both.Item4[1]);
            else
                return Tuple.Create(both.Item1[0], both.Item2[0], 
                    both.Item3[0], both.Item4[0]);
        }

        /// <summary>
        /// Finds the nearest intersection point between this sphere
        /// and a ray.  Returns a tuple containing the propagation 
        /// distance, cosine of the angle of incidence, intersection 
        /// point, and the surface normal.  NaNs returned if sphere 
        /// is missed.
        /// </summary>
        /// <param name="ray"> Ray to intersect </param>
        /// <returns> Tuple containing the propagation distance, 
        /// cosine of the object of incidence, intersection point, 
        /// and the surface normal </returns>
        public Tuple<double, double, Point, Normal> 
            NearestRayIntersection(Ray ray)
        {
            return NearestRayIntersection(this, ray);
        }

        /// <summary>
        /// Clones a Sphere instance by deep copy.
        /// </summary>
        /// <returns> A copy of this Sphere </returns>
        public Sphere Clone()
        {
            // Sphere members
            return new Sphere(this.center.Clone(), this.radius);
        }

        # endregion Methods
    }
}
