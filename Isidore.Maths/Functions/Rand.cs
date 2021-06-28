using System;

namespace Isidore.Maths
{
    /// <summary>
    /// Child of the Random class which adds a Box-Muller normal random 
    /// generator and a few random array outputs.
    /// </summary>
    public class Rand : Random
    {
        /// <summary>
        /// Counter for switching between cos and sin
        /// </summary>
        private bool odd = true;

        /// <summary>
        /// The square root of twice the negative of the log of the 
        /// first random variable 
        /// </summary>
        private double root;

        /// <summary>
        /// The second random variable used in the exponential
        /// </summary>
        private double r2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seed"> Initialization seed </param>
        public Rand(int seed) : base(seed) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rand() : base() { }

        /// <summary>
        /// Returns a random draw fitted to a normal 
        /// Gaussian distribution
        /// </summary>
        /// <param name="Sigma"> Gaussian width </param>
        /// <param name="Mean"> Gaussian center </param>
        /// <returns> random number </returns>
        public double NormDouble(double Sigma = 1.0, double Mean = 0.0)
        {
            // Box-Muller normal distribution
            double bmVal; // Box-Muller random variable
            if (odd)
            {
                double r1 = base.NextDouble();
                root = Math.Sqrt(-2.0 * Math.Log(r1));
                r2 = base.NextDouble();
                bmVal = (root * Math.Cos(2.0 * Math.PI * r2));
            }              
            else
                bmVal = (root * Math.Sin(2.0 * Math.PI * r2));
            odd = !odd; // Switches between methods
            // Scaled by sigma and offset by Mean
            return bmVal * Sigma + Mean;
        }

        /// <summary>
        /// Returns a random draw 1D array fitted to a normal 
        /// Gaussian distribution
        /// </summary>
        /// <param name="length"> Array length </param>
        /// <param name="Sigma"> Distribution sigma </param>
        /// <param name="Mean"> Distribution mean </param>
        /// <returns> Array of random numbers </returns>
        public double[] NormDouble(int length, double Sigma = 1.0, 
            double Mean = 0.0)
        {
            double[] arr = new double[length];
            for (int idx = 0; idx < length; idx++)
                arr[idx] = NormDouble(Sigma,Mean);
            return arr;
        }

        /// <summary>
        /// Returns a random draw 2D array fitted to a normal 
        /// Gaussian distribution
        /// </summary>
        /// <param name="dim0"> Array length in first dimension </param>
        /// <param name="dim1"> Array length in second dimension </param>
        /// <param name="Sigma"> Distribution sigma </param>
        /// <param name="Mean"> Distribution mean </param>
        /// <returns> 2D array of random numbers </returns>
        public double[,] NormDouble(int dim0, int dim1, double Sigma = 1.0, 
            double Mean = 0.0)
        {
            double[,] arr = new double[dim0, dim1];
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    arr[idx0, idx1] = NormDouble(Sigma,Mean);
            return arr;
        }

        /// <summary>
        /// Returns a random draw 1D array spanning 0-1
        /// </summary>
        /// <param name="length"> Array length </param>
        /// <returns> Array of random numbers </returns>
        public double[] Double(int length)
        {
            double[] arr = new double[length];
            for (int idx = 0; idx < length; idx++)
                    arr[idx] = NextDouble();
            return arr;
        }

        /// <summary>
        /// Returns a random draw 2D array spanning 0-1
        /// </summary>
        /// <param name="dim0"> Array length in first dimension </param>
        /// <param name="dim1"> Array length in second dimension </param>
        /// <returns> 2D array of random numbers </returns>
        public double[,] Double(int dim0, int dim1)
        {
            double[,] arr = new double[dim0, dim1];
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    arr[idx0, idx1] = NextDouble();
            return arr;
        }

        /// <summary>
        /// Returns a random draw 1D array spanning integers from 
        /// minVal to maxVal
        /// </summary>
        /// <param name="length"> Array length </param>
        /// <param name="minVal"> Minimum value in random draw </param>
        /// <param name="maxVal"> Maximum value in random draw </param>
        /// <returns> Array of random numbers </returns>
        public int[] Int(int length, int minVal = int.MinValue, 
            int maxVal = int.MaxValue)
        {
            int[] arr = new int[length];
            for (int idx = 0; idx < length; idx++)
                arr[idx] = Next(minVal, maxVal);
            return arr;
        }

        /// <summary>
        /// Returns a random draw 2D array spanning integers from 
        /// minVal to maxVal
        /// </summary>
        /// <param name="dim0"> Array length in first dimension </param>
        /// <param name="dim1"> Array length in second dimension </param>
        /// <param name="minVal"> Minimum value in random draw </param>
        /// <param name="maxVal"> Maximum value in random draw </param>
        /// <returns> 2D array of random numbers </returns>
        public int[,] Int(int dim0, int dim1, int minVal = int.MinValue, 
            int maxVal = int.MaxValue)
        {
            int[,] arr = new int[dim0, dim1];
            for (int idx0 = 0; idx0 < dim0; idx0++)
                for (int idx1 = 0; idx1 < dim1; idx1++)
                    arr[idx0, idx1] = Next(minVal, maxVal);
            return arr;
        }
    }
}
