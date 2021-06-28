using System;

namespace Isidore.Maths
{
    public static partial class Distribution
    {
        /// <summary>
        /// Generates either an increasing or decreasing incremental 
        /// vector array
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="startValue"> First value </param>
        /// <param name="stopValue"> Second value</param>
        /// <param name="stepSize"> spacing between values </param>
        /// <returns> Incremental distribution </returns>
        public static T[] Increment<T>(T startValue, T stopValue, 
            T stepSize)
        {
            // Functions
            Func<int, T> ToT = Operator<int, T>.Convert;
            Func<T, int, T> dT = Operator<int, T>.Multiply;
            Func<T, T, T> plusT = Operator<T, T>.Add;

            // Swaps values based on step size direction
            if ((Operator.GreaterThan(startValue, stopValue) && 
                Operator.GreaterThan(stepSize, default(T))) ||
                (Operator.LessThan(startValue, stopValue) && 
                Operator.LessThan(stepSize, default(T))))
            {
                Function.Swap(ref startValue, ref stopValue);
            }

            // Number of points
            T LengthT = Operator.Divide<T>(Operator.Subtract<T>(
                stopValue, startValue), stepSize);
            int Length = Operator.Convert<T, int>(LengthT) + 1;

            // Increments either up or down
            T[] arr = new T[Length];
            for (int idx = 0; idx < Length; idx++)
            {
                T dValue = dT(stepSize, idx);
                arr[idx] = plusT(startValue, dValue);
            }

            return arr;
        }
    }
}
