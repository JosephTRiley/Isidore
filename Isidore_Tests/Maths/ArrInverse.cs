using System;
using Isidore.Maths;

namespace Isidore_Tests
{
    public class ArrInverse
    {
        public static bool Run()
        {
            double threshErr = 0.0001; // Acceptable error (1 = 100%)

            Console.WriteLine("Array Inverse Check");

            // Checks 4x4 inverse
            double[,] dArr = { { 9, 1, 9, 5 }, { 2, 3, 2, 8 }, { 9, 5, 9, 2 }, { 6, 9, 9, 4 } };
            double[,] idArr = Arr.Inverse(dArr);
            double[,] itArr = { {-0.245614035087719, 0.185758513931889, 0.537667698658411, -0.333333333333333},
                                {-0.210526315789474, 0.083591331269350, 0.191950464396285, 0.000000000000000},
                                {0.350877192982456,-0.256965944272446,-0.515995872033024,0.333333333333333},
                                {0.052631578947368,0.111455108359133,-0.077399380804954,0}};

            double[] perDiff = new double[idArr.Length];
            int len = idArr.GetLength(0);
            for (int idx0 = 0; idx0 < len; idx0++)
                for(int idx1 = 0; idx1<idArr.GetLength(1); idx1++)
                {
                    int tag = idx1 + idx0*len;
                    perDiff[tag] = Aids.perErr(idArr[idx0,idx1], itArr[idx0,idx1]);
                    if( perDiff[tag] > threshErr)
                    {
                        Console.WriteLine("Array Inverse Check Failed");
                        return false;
                    }
                }

            // Checks 2x2
            dArr = new double[,] { { 9, 9 }, { 8, 6 } };
            idArr = Arr.Inverse(dArr);
            itArr = new double[,] { { -3.0 / 9.0, 0.5 }, { 4.0 / 9.0, -0.5 } };

            perDiff = new double[idArr.Length];
            len = idArr.GetLength(0);
            for (int idx0 = 0; idx0 < len; idx0++)
                for (int idx1 = 0; idx1 < idArr.GetLength(1); idx1++)
                {
                    int tag = idx1 + idx0 * len;
                    perDiff[tag] = Aids.perErr(idArr[idx0, idx1], itArr[idx0, idx1]);
                    if (perDiff[tag] > threshErr)
                    {
                        Console.WriteLine("Array Inverse Check Failed");
                        return false;
                    }
                }

            // Returns true
            return true;
        }


    }
}
