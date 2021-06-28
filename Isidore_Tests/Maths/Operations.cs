using System;
using System.Diagnostics;
using Isidore.Maths;

namespace Isidore_Tests
{
    public class Operations
    {
        public static bool Run()
        {
            // test passed boolean
            bool passed = true;

            ////////////////////////////////
            // Checks Conversions
            ////////////////////////////////
            Console.WriteLine("Conversion Functions");
            double dScale = Math.PI; // A good irrational number
            int[] iArr1 = new int[] { 1, 2, 3, 4 }; // 1D int array
            int[,] iArr2 = new int[,] {{ 1, 2, 3, 4 },{11,12,13,14},
                {21,22,23,24},{31,32,33,34}}; // 2d int array

            int iScale;
            double[] dArr1;
            float[,] fArr2;
            double[,] dArr2;
            try 
            {
                iScale = Operator.Convert<double, int>(dScale);
                dArr1 = Operator.Convert<int, double>(iArr1);
                fArr2 = Operator.Convert<int, float>(iArr2);
                dArr2 = Operator.Convert<int, double>(iArr2);
            }
            catch (Exception ex)
            {
                string msg = ex.Message; // avoid capture of ex itself
                throw new InvalidOperationException(msg);
            }
            Console.WriteLine("Conversion Functions Successful.");
            

            ////////////////////////////////
            // Checks Arithmetic
            ////////////////////////////////
            Console.WriteLine("\nArithmetic Function");
            int len = 100;

            // Random variables to operate on
            Rand rand = new Rand(123456);
            double[,] rdArr1 = rand.Double(len, len);
            double[,] rdArr2 = rand.Double(len, len);

            // Direct call 
            double[,] addArr2 = new double[len, len];
            double[,] subArr2 = new double[len, len];
            double[,] mulArr2 = new double[len, len];
            double[,] divArr2 = new double[len, len];
            int loops = 2000;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int idx = 0; idx < loops; idx++)
            {
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        addArr2[idx0, idx1] = rdArr1[idx0, idx1] + rdArr2[idx0, idx1];
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        subArr2[idx0, idx1] = rdArr1[idx0, idx1] - rdArr2[idx0, idx1];
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        mulArr2[idx0, idx1] = rdArr1[idx0, idx1] * rdArr2[idx0, idx1];
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        divArr2[idx0, idx1] = rdArr1[idx0, idx1] / rdArr2[idx0, idx1];
            }
            watch.Stop();
            Console.WriteLine("Direct time: {0}ms", watch.ElapsedMilliseconds);
            long dTics = watch.ElapsedTicks;

            // Operator calls
            double[,] opAddArr2 = new double[len, len];
            double[,] opSubArr2 = new double[len, len];
            double[,] opMulArr2 = new double[len, len];
            double[,] opDivArr2 = new double[len, len];
            watch.Restart();
            for (int idx = 0; idx < loops; idx++)
            {
                opAddArr2 = Operator.Add(rdArr1, rdArr2);
                opSubArr2 = Operator.Subtract(rdArr1, rdArr2);
                opMulArr2 = Operator.Multiply(rdArr1, rdArr2);
                opDivArr2 = Operator.Divide(rdArr1, rdArr2);
            }
            watch.Stop();
            Console.WriteLine("Operator time: {0}ms", watch.ElapsedMilliseconds);
            long oTics = watch.ElapsedTicks;

            // Checks for accuracy
            bool[,] eqAdd = Operator.Equal(addArr2, opAddArr2);
            bool[,] eqSub = Operator.Equal(subArr2, opSubArr2);
            bool[,] eqMul = Operator.Equal(mulArr2, opMulArr2);
            bool[,] eqDiv = Operator.Equal(divArr2, opDivArr2);

            int totAdd = Stats.Sum(Operator.Convert<bool,int>(eqAdd));
            int totSub = Stats.Sum(Operator.Convert<bool, int>(eqSub));
            int totMul = Stats.Sum(Operator.Convert<bool, int>(eqMul));
            int totDiv = Stats.Sum(Operator.Convert<bool, int>(eqDiv));

            int totLen = eqAdd.Length;
            passed = ((totAdd == totLen) & (totSub == totLen) & (totMul == totLen) & (totDiv == totLen));
            if (!passed)
                return false;
            Console.WriteLine("Evaluation within tolerance.");

            ////////////////////////////////////////////
            // Checks the expression function operations
            ////////////////////////////////////////////
            Console.WriteLine("\nExpression Function");

            float[,] rdArrF1 = Operator.Convert<double,float>(rdArr1);
            float[,] rdArrF2 = Operator.Convert<double, float>(rdArr2);
            float[,] addArrF = new float[len, len];
            float[,] subArrF = new float[len, len];
            float[,] mulArrF = new float[len, len];
            float[,] divArrF = new float[len, len];
            watch.Restart();
            for (int idx = 0; idx < loops; idx++)
            {
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        addArrF[idx0, idx1] = rdArrF1[idx0, idx1] + rdArrF2[idx0, idx1];
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        subArrF[idx0, idx1] = rdArrF1[idx0, idx1] - rdArrF2[idx0, idx1];
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        mulArrF[idx0, idx1] = rdArrF1[idx0, idx1] * rdArrF2[idx0, idx1];
                for (int idx0 = 0; idx0 < len; idx0++)
                    for (int idx1 = 0; idx1 < len; idx1++)
                        divArrF[idx0, idx1] = rdArrF1[idx0, idx1] / rdArrF2[idx0, idx1];
            }
            watch.Stop();
            Console.WriteLine("Direct time: {0}ms", watch.ElapsedMilliseconds);
            long dfTics = watch.ElapsedTicks;

            // Operator calls
            watch.Restart();
            float[,] opAddArrF = new float[len, len];
            float[,] opSubArrF = new float[len, len];
            float[,] opMulArrF = new float[len, len];
            float[,] opDivArrF = new float[len, len];
            watch.Restart();
            for (int idx = 0; idx < loops; idx++)
            {
                opAddArrF = Operator.Add(rdArrF1, rdArrF2);
                opSubArrF = Operator.Subtract(rdArrF1, rdArrF2);
                opMulArrF = Operator.Multiply(rdArrF1, rdArrF2);
                opDivArrF = Operator.Divide(rdArrF1, rdArrF2);
            }
            watch.Stop();
            Console.WriteLine("Operator time: {0}ms", watch.ElapsedMilliseconds);
            long ofTics = watch.ElapsedTicks;

            // Checks for accuracy
            eqAdd = Operator.Equal(addArrF, opAddArrF);
            eqSub = Operator.Equal(subArrF, opSubArrF);
            eqMul = Operator.Equal(mulArrF, opMulArrF);
            eqDiv = Operator.Equal(divArrF, opDivArrF);

            totAdd = Stats.Sum(Operator.Convert<bool, int>(eqAdd));
            totSub = Stats.Sum(Operator.Convert<bool, int>(eqSub));
            totMul = Stats.Sum(Operator.Convert<bool, int>(eqMul));
            totDiv = Stats.Sum(Operator.Convert<bool, int>(eqDiv));

            totLen = eqAdd.Length;
            passed = ((totAdd == totLen) & (totSub == totLen) & (totMul == totLen) & (totDiv == totLen));
            if (!passed)
                return false;
            Console.WriteLine("Evaluation within tolerance.");

            return true;
        }

        static bool WaitForKey(String message)
        {
            Console.Error.Write(message);
            while (!Console.KeyAvailable)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }
            Console.ReadKey();
            return true;
        }
    }
}
