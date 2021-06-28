using System;

namespace Isidore.Maths
{
    public partial class Function
    {
        /// <summary>
        /// Finds the roots of the quadratic equation 
        /// Ax^2 + Bx + C = 0
        /// </summary>
        /// <param name="A"> Quadratic coefficient </param>
        /// <param name="B"> Linear coefficient </param>
        /// <param name="C"> Free term </param>
        /// <returns> The two roots </returns>
        static public double[] Quadratic(double A, double B, double C)
        {
            // Finds square of discriminant
            double D = B * B - 4.0 * A * C;

            // If discriminant is zero or less, there's no solution
            if (D <= 0)
                return new double[]{double.NaN, double.NaN};

            // Now calculates the discriminant
            D = Math.Sqrt(D);

            // This approach help negate small number error when B ~ D
            // Quadratic portion of the solution
            double quad = -0.5*(B + Math.Sign(B)*D);
            double t0 = quad / A; // B << or on the order of A and C
            double t1 = C / quad; // B >> A or C

            // Swaps so smallest is returned first
            if (t0 > t1)
                Function.Swap(ref t0, ref t1);

            // Returns tuple
            return new double[]{t0, t1};
        }
    }
}
