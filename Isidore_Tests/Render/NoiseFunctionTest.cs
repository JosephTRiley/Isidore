using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests procedural textures and noise functions
    /// </summary>
    class NoiseFunctionTest
    {
        /// <summary>
        /// Tests that the noise models & procedural textures are working as 
        /// expected
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            /////////////////////////////////
            // Checks noise functions
            /////////////////////////////////

            Console.WriteLine("\n-Starting Noise Function Check");

            // Default grid size
            int xLen = 256; // Need it to be at least 256 for WL analysis
            var xArr = Distribution.Increment(0.0, 1.0, 1.0 / (xLen - 1));

            /////////////////////////////////
            // Improved Perlin noise 1D check
            /////////////////////////////////

            // The zeros is correct because the cardinal axis will frequently
            // have sequential zero pairs due to how the gradients are
            // calculated from the permuted table and dx,dy,dz,dw
            // This could probably be corrected for by adding a jitter term

            Console.WriteLine("Checking Perlin noise in 1D");

            // Different grid sizes
            int[] linSize = new int[] { 1, 2, 4, 10, 25, 50, 100, 150,
                175, 200 };

            // (Improved) Perlin Noise
            PerlinNoiseFunction pnoise1D = new PerlinNoiseFunction(123, 9);

            // Uses Noise to extract out the different grid sizes
            double[,] pData1D = new double[xLen, linSize.Length];
            // Cycles through each grid size
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var coord = new Point(1, 2, 3);
            for (int idx = 0; idx < linSize.Length; idx++)
            {
                // Scales grid
                var thisArr = xArr.Select(x => x * linSize[idx])
                    .ToArray();

                // Cycles through each point
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[coord.Comp.Length - 1] = thisArr[idx0];

                    // Calculates noise, reads the noise from the tuple
                    var noise = pnoise1D.GetVal(coord);
                    pData1D[idx0, idx] = noise;
                }
            }


            // Repeats in log space
            var shiftVec = new Vector(new double[] 
                { 123.4, 234.5, 345.6, 456.7 });
            var distFunc = Noise.DistFunc(NoiseDistribution.LogNormal);
            var noiseParams = new NoiseParameters(shiftVec, 5, 10, distFunc);
            //var noiseParams = new NoiseParameters(shiftVec, 1, 0, distFunc);
            Noise logNoise = new Noise(pnoise1D, noiseParams);

            // Time cycle
            double[] lnData1D = new double[xLen];
            var cArr = xArr.Select(x => x * 50).ToArray();
            for (int idx0 = 0; idx0 < xLen; idx0++)
            {
                coord.Comp[coord.Comp.Length - 1] = cArr[idx0];
                var noise = logNoise.GetVal(coord);
                lnData1D[idx0] = noise;
            }

            /////////////////////////////////
            // Frequency Noise
            /////////////////////////////////
            // FrequencyNoise normally is not called.  This section checks
            // that it gives the same answer as Noise as well as
            // the noise array and point array functionality

            // Perlin noise function and noise instance
            int tablePower = 15; // Permutable hash table size (2 ^ tabelPower)
            int slen = 1 << tablePower; // Data series length
            int noiseSeed = 12345; // Seed used in the permuted table
            var noiseOffset = new Vector(
                new double[] { 123.4, 234.5, 345.6, 456.7 });
            var perlinNoise = new PerlinNoiseFunction(noiseSeed, tablePower);
            var pnoiseC = new Noise(perlinNoise, noiseOffset);

            // Frequency noise instance
            var fnoiseC = new FrequencyNoise(perlinNoise, 1, 1, 2,
                noiseOffset);

            // fBm noise instance
            var bnoiseC = new fBmNoise(perlinNoise, 1, 1, 1, 2, noiseOffset);

            // Sampling points
            int factor = 8; // Number of points across lattice cells
            int flen = slen * factor; // Spans entire noise function
            double dt = 1.0 / factor; // Step size
            double[] time = Distribution.Increment(dt, slen, dt); // time
            var coords = new double[flen, 4]; // Coordinate array
            // Populates coordinate array
            for (int idx = 0; idx < flen; idx++)
                coords[idx, 0] = time[idx];
            var pts = Point.Array(coords);

            // Noise values
            var pnoiseArr = pnoiseC.GetVal(pts);
            var fnoiseArr = fnoiseC.GetVal(pts);
            var bnoiseArr = bnoiseC.GetVal(pts);

            /////////////////////////////////
            // Improved Perlin noise
            /////////////////////////////////

            Console.WriteLine("Checking Perlin noise");

            // Different grid sizes
            int[] gridSize = new int[] { 1, 2, 4, 10, 25, 50, 100, 150,
                175, 200 };

            // (Improved) Perlin Noise
            PerlinNoiseFunction pnf = new PerlinNoiseFunction(123,9);
            // Checks if cloning works
            var pnfClone = pnf.Clone();
            if (pnf.LUT.Length != pnfClone.LUT.Length)
            {
                Console.WriteLine("PerlineNoiseFunction Clone failed");
                return false;
            }
            var pnfList = new List<NoiseFunction>();
            pnfList.Add(pnf);
            pnfList.Add(pnfClone);
     
            // Checks that we can clone a Noise class
            var pnoise = new Noise(pnf, new Vector(
                new double[] { 123.4, 234.5, 345.6, 456.7 }));
            // Checks if cloning works
            var pnoiseClone = pnoise.Clone();

            // Uses Noise to extract out the different grid sizes
            double[,,] pData = new double[xLen, xLen, gridSize.Length];
            // Cycles through each grid size
            watch.Start();
            coord = new Point(4);
            for (int idx = 0; idx < gridSize.Length; idx++)
            {
                // Scales grid
                var thisArr = xArr.Select(x => x * gridSize[idx])
                    .ToArray();

                // Cycles through each point
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[0] = thisArr[idx0];
                    for (int idx1 = 0; idx1 < xLen; idx1++)
                    {
                        coord.Comp[1] = thisArr[idx1];
                        // Calculates noise, reads the noise from the tuple
                        var noise = pnoise.GetVal(coord);
                        pData[idx0, idx1, idx] = noise;
                    }
                }
            }

            // Total simulations time
            watch.Stop();
            Console.WriteLine("Perlin noise time: {0}ms",
                watch.ElapsedMilliseconds);


            /////////////////////////////////
            // fBm (In 4D)
            /////////////////////////////////

            Console.WriteLine("Checking fBm noise");

            // Produces fBm for nine Hurst exponents
            var H = Distribution.Increment(0.1, 0.9, 0.1);
            var Hlen = H.Length;
            var lacunarity = 2; // dyadic scale

            // Sets octaves to anti-aliasing condition
            var freqRng = new double[] { Math.Pow(lacunarity, -4),
                Math.Pow(lacunarity, 7) };
            var octaveNum = (int)Math.Log(freqRng.Last(), lacunarity) -
                (int)Math.Log(freqRng.First(), lacunarity) + 1;

            // Creates 4D coordinates
            var shift4D = new Vector(
                new double[] { 123.4, 234.5, 345.6, 456.7 });

            //Cycles through each turbulence refinement
            var pfData = new double[xLen, xLen, Hlen];
            var pfDataComp = new double[xLen, xLen, Hlen, octaveNum];
            for (int idx = 0; idx < Hlen; idx++)
            {
                // Perlin turbulence
                var pfnoise = new fBmNoise(pnf, freqRng[0],
                    freqRng[1], H[idx], lacunarity, shift4D);

                // Checks cloning
                var pfnoiseClone = pfnoise.Clone();

                // Cycles through each point
                watch.Reset();
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[0] = xArr[idx0];
                    for (int idx1 = 0; idx1 < xLen; idx1++)
                    {
                        coord.Comp[1] = xArr[idx1];
                        // Reads the noise
                        watch.Start();
                        var noise = pfnoise.GetVal(coord);
                        watch.Stop();

                        pfData[idx0, idx1, idx] = noise;

                        // Retrieves the noise from GetCoord
                        var tnoise = pfnoise.GetComponents(coord);
                        for (int iidx = 0; iidx < octaveNum; iidx++)
                            pfDataComp[idx0, idx1, idx, iidx] =
                                tnoise[iidx];
                    }
                }
                Console.WriteLine("Perlin fBm H={0}, time: {1}ms",
                    H[idx], watch.ElapsedMilliseconds);
            }

            /////////////////////////////////
            // multiplicative cascade mBm
            /////////////////////////////////

            Console.WriteLine("Checking multiplicative cascade mBm");

            // Produces fBm for nine Hurst exponents
            double H1 = 0.5;
            var shift = new Vector(
                new double[] { 987.6, 654.3, 321.0, 539.4 });
            //var offset1 = Distribution.Increment(0.0, 1.0, 0.1);
            //var offset2 = Distribution.Increment(2.0, 10.0, 1.0);
            //var offset = offset1.Concat(offset2).ToArray();
            var offset1 = Distribution.Increment(0.0, 3.0, 0.2);
            var offset2 = Distribution.Increment(3.5, 10.0, 0.5);
            var offset = new double[offset1.Length + offset2.Length + 1];
            offset1.CopyTo(offset, 0);
            offset2.CopyTo(offset, offset1.Length);
            offset[offset.Length - 1] = 100;
            var offlen = offset.Length;

            //Cycles through each turbulence refinement
            var pmData = new double[xLen, xLen, offlen];
            var pmDataComp = new double[xLen, xLen, offlen, octaveNum];
            for (int idx = 0; idx < offlen; idx++)
            {
                // Perlin turbulence
                var pmnoise = new mBmCascadeNoise(pnf, freqRng[0], freqRng[1],
                    H1, lacunarity, offset[idx], shift);

                // Checks clone
                var pmnoiseClone = pmnoise.Clone();
                pmnoiseClone.Hurst = -1.0;
                pmnoiseClone.Lacunarity = 10;
                pmnoiseClone.maxFreq = 10e3;
                pmnoiseClone.minFreq = 1.0e-3;
                pmnoiseClone.cascadeOffset = 10.0;
                pmnoiseClone.shift = new Vector(3);

                // Cycles through each point
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[0] = xArr[idx0];
                    for (int idx1 = 0; idx1 < xLen; idx1++)
                    {
                        coord.Comp[1] = xArr[idx1];
                        // Calculates noise, reads the noise from the tuple
                        var noise = pmnoise.GetVal(coord);
                        pmData[idx0, idx1, idx] = noise;

                        // Retrieves the noise from GetCoord
                        var tnoise = pmnoise.GetComponents(coord);
                        for (int iidx = 0; iidx < octaveNum; iidx++)
                            pmDataComp[idx0, idx1, idx, iidx] =
                                tnoise[iidx];
                    }
                }
            }

            /////////////////////////////////
            // Perlin turbulence
            /////////////////////////////////

            Console.WriteLine("Checking Perlin turbulence");

            // Produces turbulence for eight different dyadic depths
            double minFreq = freqRng[0];
            var maxFreq = Enumerable.Range(0, 10).
                Select(x => Math.Pow(lacunarity, x)).ToArray();
            flen = maxFreq.Length;
            var olen = (int)Math.Log(maxFreq.Last(), lacunarity) -
                (int)Math.Log(minFreq, lacunarity) + 1;

            //Cycles through each turbulence refinement
            var ptData = new double[xLen, xLen, flen];
            var ptDataComp = new double[xLen, xLen, flen, olen];
            for (int idx = 0; idx < flen; idx++)
            {
                // Perlin turbulence
                var ptnoise = new PerlinTurbulenceNoise(pnf, minFreq,
                    maxFreq[idx], false, 2, shift);

                // Cycles through each point
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[0] = xArr[idx0];
                    for (int idx1 = 0; idx1 < xLen; idx1++)
                    {
                        coord.Comp[1] = xArr[idx1];

                        // Calculates noise, reads the noise from the tuple
                        var noise = ptnoise.GetVal(coord);
                        ptData[idx0, idx1, idx] = noise;

                        // Retrieves the noise from GetCoord
                        var tnoise = ptnoise.GetComponents(coord);
                        for (int iidx = 0; iidx < tnoise.Length; iidx++)
                            ptDataComp[idx0, idx1, idx, iidx] =
                                tnoise[iidx];
                    }
                }
            }

            // Repeats for absolute values
            var ptpData = new double[xLen, xLen, flen];
            var ptpDataComp = new double[xLen, xLen, flen, olen];
            for (int idx = 0; idx < flen; idx++)
            {
                // Perlin turbulence
                var ptpnoise = new PerlinTurbulenceNoise(pnf,
                    minFreq, maxFreq[idx], true, 2, shift);

                // Cycles through each point
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[0] = xArr[idx0];
                    for (int idx1 = 0; idx1 < xLen; idx1++)
                    {
                        coord.Comp[1] = xArr[idx1];

                        // Calculates noise, reads the noise from the tuple
                        var noise = ptpnoise.GetVal(coord);
                        ptpData[idx0, idx1, idx] = noise;

                        // Retrieves the noise from GetCoord
                        var tnoise = ptpnoise.GetComponents(coord);
                        for (int iidx = 0; iidx < tnoise.Length; iidx++)
                            ptpDataComp[idx0, idx1, idx, iidx] =
                                tnoise[iidx];
                    }
                }
            }

            /////////////////////////////////
            // Perlin Marble
            /////////////////////////////////

            Console.WriteLine("Checking Perlin marble");

            // Produces marbling multipliers
            var marbleMult = Enumerable.Range(0, 10).
                Select(x => (double)x).ToArray();
            var mlen = marbleMult.Length;
            // Brightness oscillation frequency
            double marbleFreq = 3;
            // Dimension to apply marbling
            int marbleDim = 1;

            //Cycles through each turbulence refinement
            var pmmData = new double[xLen, xLen, mlen];
            var pmmDataComp = new double[xLen, xLen, mlen, olen];
            for (int idx = 0; idx < mlen; idx++)
            {
                // Perlin marble
                var pmmnoise = new PerlinMarbleNoise(pnf, minFreq,
                    maxFreq.Last(), marbleMult[idx], marbleFreq, marbleDim, 
                    2, shift);

                // Cycles through each point
                for (int idx0 = 0; idx0 < xLen; idx0++)
                {
                    coord.Comp[0] = xArr[idx0];
                    for (int idx1 = 0; idx1 < xLen; idx1++)
                    {
                        coord.Comp[1] = xArr[idx1];

                        // Calculates noise, reads the noise from the tuple
                        var noise = pmmnoise.GetVal(coord);
                        pmmData[idx0, idx1, idx] = noise;

                        // Retrieves the noise from GetCoord
                        var tnoise = pmmnoise.GetComponents(coord);
                        for (int iidx = 0; iidx < tnoise.Length; iidx++)
                            pmmDataComp[idx0, idx1, idx, iidx] =
                                tnoise[iidx];
                    }
                }
            }

            /////////////////////////////////
            // Perlin Spectrum Noise
            /////////////////////////////////

            Console.WriteLine("Checking Perlin spectrum noise");

            var psData = new double[xLen, xLen, 2];

            // Default Perlin spectrum noise
            var psnoise = new SpectrumNoise(pnf, null, shift);
            var powSpec0 = psnoise.powerSpectrum;

            // Cycles through each point
            for (int idx0 = 0; idx0 < xLen; idx0++)
            {
                coord.Comp[0] = xArr[idx0];
                for (int idx1 = 0; idx1 < xLen; idx1++)
                {
                    coord.Comp[1] = xArr[idx1];

                    // Calculates noise, reads the noise from the tuple
                    var noise = psnoise.GetVal(coord);
                    psData[idx0, idx1, 0] = noise;
                }
            }

            int len = 41;
            int offLen = 10;
            int totLen = len + offLen;
            var freq = new double[totLen];
            var power = new double[totLen];
            for (int idx = 0; idx < len; idx++)
            {
                freq[idx] = Math.Pow(2, idx);
                power[idx] = Math.Pow(2, -idx / 3);
            }
            for (int idx = len; idx < totLen; idx++)
            {
                freq[idx] = Math.Pow(2, idx - len - offLen);
                power[idx] = 1.0;
            }

            var powSpec1 = new PowerSpectrum(freq, power);

            var pssnoise = new SpectrumNoise(pnf, powSpec1, shift);

            // Cycles through each point
            for (int idx0 = 0; idx0 < xLen; idx0++)
            {
                coord.Comp[0] = xArr[idx0];
                for (int idx1 = 0; idx1 < xLen; idx1++)
                {
                    coord.Comp[1] = xArr[idx1];

                    // Calculates noise, reads the noise from the tuple
                    var noise = pssnoise.GetVal(coord);
                    psData[idx0, idx1, 1] = noise;
                }
            }

            /////////////////////////////////
            // Matlab processing
            /////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) +
                "OutputData\\Render\\ProceduralTexture";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "xArr", xArr);
            MatLab.Put(matlab, "linSize", linSize);
            MatLab.Put(matlab, "pData1D", pData1D);
            MatLab.Put(matlab, "pData", pData);
            MatLab.Put(matlab, "time", time);
            MatLab.Put(matlab, "lnData1D", lnData1D);
            MatLab.Put(matlab, "pnoiseArr", pnoiseArr);
            MatLab.Put(matlab, "fnoiseArr", fnoiseArr);
            MatLab.Put(matlab, "bnoiseArr", bnoiseArr);
            MatLab.Put(matlab, "gridSize", gridSize);
            MatLab.Put(matlab, "pfData", pfData);
            MatLab.Put(matlab, "pfDataComp", pfDataComp);
            MatLab.Put(matlab, "H", H);
            MatLab.Put(matlab, "pmData", pmData);
            MatLab.Put(matlab, "pmDataComp", pmDataComp);
            MatLab.Put(matlab, "offset", offset);
            MatLab.Put(matlab, "ptData", ptData);
            MatLab.Put(matlab, "ptDataComp", ptDataComp);
            MatLab.Put(matlab, "maxFreq", maxFreq);
            MatLab.Put(matlab, "ptpData", ptpData);
            MatLab.Put(matlab, "ptpDataComp", ptpDataComp);
            MatLab.Put(matlab, "pmmData", pmmData);
            MatLab.Put(matlab, "pmmDataComp", pmmDataComp);
            MatLab.Put(matlab, "marbleMult", marbleMult);
            MatLab.Put(matlab, "psFreq", freq);
            MatLab.Put(matlab, "psPower", power);
            MatLab.Put(matlab, "psData", psData);
            resStr = matlab.Execute("NoiseFunctionCheck");

            //Console.WriteLine("\n Noise Function Check Passed");

            return true;
        }
    }
}
