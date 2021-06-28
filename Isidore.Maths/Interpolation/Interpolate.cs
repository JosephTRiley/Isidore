using System;
using System.Linq;

namespace Isidore.Maths
{
    /// <summary>
    /// Provides methods for interpolation.  Current option is Linear.
    /// </summary>
    public partial class Interpolate
    {

        /// <summary>
        /// Places a value in a sorted array of value and provides 
        /// the unit factor that locates it between two points.
        /// The output will be the lower bound with idx+1 being the upper 
        /// bound.
        /// If a point is outside the bounds of the array, 
        /// unitFactor will be either Positive or Negative Infinity.
        /// </summary>
        /// /// <param name="idxPt"> Interpolation point </param>
        /// <param name="arrPts"> Vector time/location point array </param>
        /// <returns> Tuple containing the 0) Lower bounding index, 
        /// 1) Relative position between pt[Idx] and pt[Idx+1] </returns>
        public static Tuple<int, double> PlaceVal(double idxPt, 
            double[] arrPts)
        {
            int idx = 0;
            double unitfactor;
            if(idxPt <= arrPts[0])
                unitfactor = double.NegativeInfinity;
            else if(idxPt >= arrPts.Last())
            {
                unitfactor = double.PositiveInfinity;
                idx = arrPts.Length -1;
            }
            else
            {
                idx = arrPts.Count(n => n <= idxPt) - 1;
                unitfactor = (1 - (arrPts[idx+1] - idxPt) / 
                             (arrPts[idx+1] - arrPts[idx]));
            }
            return Tuple.Create<int,double>(idx,unitfactor);
        }

        /// <summary>
        /// Provided a value, returns the index of the sorted array that is 
        /// the lower bounding value.  A return of -1 identifies the value 
        /// as less than the first value and a value of -2 identifies the 
        /// value as being larger than the final value
        /// </summary>
        /// <param name="pt"> Point to place </param>
        /// <param name="arrPts"> Vector time/location point array </param>
        /// <returns> Index of the lower bounding index </returns>
        public static int Place(double pt, double[] arrPts)
        {
            if (pt <= arrPts[0])
                return -1;
            else if (pt >= arrPts.Last())
                return -2;
            else
                return arrPts.Count(n => n <= pt) - 1;
        }
    }
}
