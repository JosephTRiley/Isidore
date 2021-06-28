using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    /// <summary>
    /// Noise is a standard implementation of Perlin noise and its derivatives:
    /// Turbulence and fBm.  This method uses a hash table and 256 indices LUT
    /// </summary>
    public class Noise
    {
        /// <summary>
        /// Fractional Brownian motion Perlin noise generator. Returns a single noise point.
        /// </summary>
        /// <param name="x"> X-axis lattice location </param>
        /// <param name="y"> Y-axis lattice location </param>
        /// <param name="z"> Z-axis lattice location </param>
        /// <param name="Octaves"> Number of octaves to simulate </param>
        /// <param name="OctavePowerStep"> Power step of each octave </param>
        /// <returns> single fBM noise point </returns>
        public static double FBM(double x, double y, double z, double Octaves, double OctavePowerStep)
        {
            int iOctaves = (int)Octaves;
            double total = 0, freq = 1, pow = 1;
            for (int Idx = 0; Idx < iOctaves; ++Idx) // to stop at  Idx < octaves-1
            {
                total += pow * Perlin(freq * x, freq * y, freq * z);
                freq *= 1.99;
                pow *= OctavePowerStep;
            }

            double pOctave = Octaves - iOctaves;
            total += pow * SmoothStep(0.3, 0.7, pOctave) * Perlin(freq * x, freq * y, freq * z);
            return total;
        }

        /// <summary>
        /// Perlin turbulence noise generator. Returns a single noise point.
        /// </summary>
        /// <param name="x"> X-axis lattice location </param>
        /// <param name="y"> Y-axis lattice location </param>
        /// <param name="z"> Z-axis lattice location </param>
        /// <param name="Octaves"> Number of octaves to simulate </param>
        /// <param name="OctavePowerStep"> Power step of each octave </param>
        /// <returns> single turbulence noise point </returns>
        public static double Turbulence(double x, double y, double z, double Octaves, double OctavePowerStep)
        {
            // should expand later to handle larger descriptions
            // should also make a version with scalar octaves 

            int iOctaves = (int)Octaves;
            double total = 0, freq = 1, pow = 1;
            for (int Idx = 0; Idx < iOctaves; ++Idx) // to stop at  Idx < octaves-1
            {
                total += pow * Math.Abs(Perlin(freq * x, freq * y, freq * z));
                freq *= 1.99;
                pow *= OctavePowerStep;
            }

            double pOctave = Octaves - iOctaves;
            total += pow * SmoothStep(0.3, 0.7, pOctave) * Math.Abs(Perlin(freq * x, freq * y, freq * z));
            return total;
        }

        /// <summary>
        /// Perlin noise generator. Returns a single noise point.
        /// </summary>
        /// <param name="x"> X-axis lattice location </param>
        /// <param name="y"> Y-axis lattice location </param>
        /// <param name="z"> Z-axis lattice location </param>
        /// <returns> Perlin noise point</returns>
        public static double Perlin(double x, double y, double z)
        {
            // Compute noise cell coordinates and offsets
            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            int iz = (int)Math.Floor(z);
            double dx = x - ix;
            double dy = y - iy;
            double dz = z - iz;

            // Compute gradient weights
            ix &= pnLen - 1;
            iy &= pnLen - 1;
            iz &= pnLen - 1;

            double w000 = Grad(ix, iy, iz, dx, dy, dz);
            double w100 = Grad(ix + 1, iy, iz, dx - 1, dy, dz);
            double w010 = Grad(ix, iy + 1, iz, dx, dy - 1, dz);
            double w110 = Grad(ix + 1, iy + 1, iz, dx - 1, dy - 1, dz);
            double w001 = Grad(ix, iy, iz + 1, dx, dy, dz - 1);
            double w101 = Grad(ix + 1, iy, iz + 1, dx - 1, dy, dz - 1);
            double w011 = Grad(ix, iy + 1, iz + 1, dx, dy - 1, dz - 1);
            double w111 = Grad(ix + 1, iy + 1, iz + 1, dx - 1, dy - 1, dz - 1);

            // Compute trilinear interpolation of weights
            double wx = NoiseWeight(dx);
            double wy = NoiseWeight(dy);
            double wz = NoiseWeight(dz);
            double x00 = Lerp(wx, w000, w100);
            double x10 = Lerp(wx, w010, w110);
            double x01 = Lerp(wx, w001, w101);
            double x11 = Lerp(wx, w011, w111);
            double y0 = Lerp(wy, x00, x10);
            double y1 = Lerp(wy, x01, x11);
            double val = Lerp(wz, y0, y1);

            if (val>1000)
            {
                Console.WriteLine("Ouch");
            }

            return val;
        }

        // Gradients for relating noise
        private static double Grad(int x, int y, int z, double dx, double dy, double dz)
        {
            // basically a cheap pseudo-random integer
            int rInt = pnLUT[pnLUT[pnLUT[x] + y] + z];
            rInt &= 15;
            double u = rInt < 8 || rInt == 12 || rInt == 13 ? dx : dy;
            double v = rInt < 4 || rInt == 12 || rInt == 13 ? dy : dz;
            int remU = rInt & 1;
            int remV = rInt & 2;
            double sum = ((remU>0) ? -u : u) + ((remV>0) ? -v : v);
            return sum;
        }

        // Weights to pull points to integers
        private static double NoiseWeight(double t)
        {
            double t3 = t * t * t;
            double t4 = t3 * t;
            return 6 * t4 * t - 15 * t4 + 10 * t3;
        }

        // Quick linear interpolator ( for grid points
        private static double Lerp(double t, double v1, double v2)
        {
            return (1 - t) * v1 + t * v2;
        }

        // Smoothing function
        private static double SmoothStep(double min, double max, double value)
        {
            double v = Clamp((value - min) / (max - min), 0, 1);
            return v * v * (-2 * v + 3);
        }

        // Clamps (limits) operational range
        private static double Clamp(double val, double low, double high)
        {
            if (val < low) return low;
            else if (val > high) return high;
            else return val;
        }

        #region conventional LUT for Perlin Noise
        private static int pnLen = 256;
        private static int[] pnLUT = new int[]
        {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 
            7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
            190,  6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219,
            203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174,
            20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 
            27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230,
            220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,  65, 25, 63,
            161, 1, 216, 80, 73, 209, 76, 132, 187, 208,  89, 18, 169, 200,
            196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
            64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 
            82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
            223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163,  70, 221, 153,
            101, 155, 167,  43, 172, 9, 129, 22, 39, 253,  19, 98, 108, 110, 
            79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 
            34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,  81, 51, 145,
            235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184,
            84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 
            222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 
            180, 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 
            7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
            6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 
            35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 
            171, 168,  68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 
            158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 
            55, 46, 245, 40, 244, 102, 143, 54,  65, 25, 63, 161,  1, 216, 80,
            73, 209, 76, 132, 187, 208,  89, 18, 169, 200, 196, 135, 130, 116,
            188, 159, 86, 164, 100, 109, 198, 173, 186,  3, 64, 52, 217, 226, 
            250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 
            206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 
            213, 119, 248, 152,  2, 44, 154, 163,  70, 221, 153, 101, 155, 
            167,  43, 172, 9, 129, 22, 39, 253,  19, 98, 108, 110, 79, 113,
            224, 232, 178, 185,  112, 104, 218, 246, 97, 228, 251, 34, 242,
            193, 238, 210, 144, 12, 191, 179, 162, 241,  81, 51, 145, 235, 
            249, 14, 239, 107, 49, 192, 214,  31, 181, 199, 106, 157, 184,  
            84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205,
            93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 
            61, 156, 180
        };
        # endregion

    }
}
