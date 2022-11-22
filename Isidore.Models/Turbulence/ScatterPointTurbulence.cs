using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isidore.Maths;

namespace Isidore.Models
{
    /// <summary>
    /// The ScatterPointTurbulence class calculates the turbulence
    /// magnitude in a point in global space by interpolating between
    /// turbulence points.
    /// </summary>
    public class ScatterPointTurbulence
    {
        #region  Fields & Properties

        /// <summary>
        /// The limiting distance a turbulence point influences a 
        /// point location 
        /// </summary>
        public double influenceRange { get; set; }

        /// <summary>
        /// The maximum number of turbulence points that can influence
        /// a point location
        /// </summary>
        public int maxPtValues { get; set; }

        /// <summary>
        /// The list of turbulence points used in this scatter point
        /// turbulence
        /// </summary>
        public List<TurbulencePointWFS> Pts
        {
            get { return pts; }
            set
            {
                pts = value;
                // Rebuilds the K-D tree
                List<Point> ppts = new List<Point>();
                for (int idx = 0; idx < pts.Count; idx++)
                    ppts.Add(pts[idx]);
                kdTree = new KDTree(ppts);
            }
        }
        private List<TurbulencePointWFS> pts;

        /// <summary>
        /// The K-D tree for the list of turbulence points
        /// </summary>
        public KDTree KDTree { get { return kdTree; } }
        private KDTree kdTree;

        #endregion  Fields & Properties
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pts"> List of turbulence points </param>
        /// <param name="influenceRange"> The distance a turbulence point 
        /// influences a location </param>
        /// <param name="maxPtVals"> The maximum number of turbulence points
        /// that can influence a location </param>
        public ScatterPointTurbulence(List<TurbulencePointWFS> pts, 
            double influenceRange = 0.01, int maxPtVals = 10)
        {
            Pts = pts;
            this.influenceRange = influenceRange;
            maxPtValues = maxPtVals;
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Retrieves the turbulence magnitude values
        /// at the given locations
        /// </summary>
        /// <param name="pos"> Location points </param>
        /// <param name="now"> Current time </param>
        /// <returns> Turbulence values </returns>
        public double[] GetVal(List<Point> pos, double now)
        {
            double[] val = new double[pos.Count];
            for (int idx = 0; idx < pos.Count; idx++)
                val[idx] = GetVal(pos[idx], now);
            // This worked in C# but intermediately crashes in MATLAB
            //Parallel.For(0, pos.Count, idx =>
            //{
            //    val[idx] = GetVal(pos[idx], now);
            //});

            return val;
        }

        /// <summary>
        /// Retrieves the turbulence magnitude value
        /// at the given location
        /// </summary>
        /// <param name="pos"> Location point </param>
        /// <param name="now"> Current time </param>
        /// <returns> Turbulence value </returns>
        public double GetVal(Point pos, double now)
        {
            double val=0;

            // Finds points within the influence range only if range > 0
            Tuple<int[], double[]> inRng = new Tuple<int[], double[]>(new int[0], new double[0]);
            if(influenceRange>0)
                inRng = kdTree.LocateNear(pos, influenceRange, maxPtValues);


            // If there are no points within the range, finds closest point
            // Otherwise calculates the turbulence magnitude
            if (inRng.Item1.Length > 0)
            {
                int[] ind = inRng.Item1;
                double[] dist = inRng.Item2;

                // Equals a point when dist=0
                if (dist[0] == 0)
                {
                    val = pts[ind[0]].GetVal(pos, now);
                }
                else
                {
                    // Inverse distances
                    double[] idist = new double[dist.Length];
                    double idistSum = 0;
                    for (int idx = 0; idx < dist.Length; idx++)
                    {
                        idist[idx] = 1.0 / dist[idx];
                        idistSum += idist[idx];
                    }
                    // Fractional influences values
                    for (int idx = 0; idx < dist.Length; idx++)
                    {
                        double frac = idist[idx] / idistSum;
                        val += frac * pts[ind[idx]].GetVal(pos, now);
                    }
                }
            }
            else
            {
                Tuple<int, double> closest = kdTree.Nearest(pos);
                val = pts[closest.Item1].GetVal(pos, now);
            }

            return val;
        }

        #endregion Methods
    }
}
