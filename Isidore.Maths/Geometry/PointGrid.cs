using System;
using System.Collections.Generic;
using System.Linq;

namespace Isidore.Maths
{

    /// <summary>
    /// PointGrid constructs a grid lattice of GridPoints.  The Point will 
    /// exist in the same N-dimensional space as the length of the 
    /// resolution.  E.G. a 2D grid could have a resolution of 3x4, 
    /// where a 2D plane of points in 3D space would have a 
    /// resolution of 3x4x1.
    /// </summary>
    [Serializable]
    public class PointGrid //: List<GridPoint>
    {
        # region Fields

        /// <summary>
        /// Grid rank (number of dimensions)
        /// </summary>
        protected int rank;

        /// <summary>
        /// Total number of points in the grid
        /// </summary>
        protected int totCnt;

        /// <summary>
        /// Points per cube
        /// </summary>
        protected int ptsPerCube;

        /// <summary>
        /// Number of points in each dimension
        /// </summary>
        private int[] resolution;

        /// <summary>
        /// Number of points in each dimension
        /// </summary>
        public int[] Resolution
        {
            get
            {
                return resolution;
            }
        }

        /// <summary>
        /// Points elements within the grid lattice
        /// </summary>
        public List<GridPoint> Points;

        /// <summary>
        /// List of cubes defined by the index of each point in the array
        /// </summary>
        public int[,] Cubes;

        # endregion Fields
        # region Constructors

        /// <summary>
        /// PointGrid Constructor
        /// </summary>
        /// <param name="Resolution"> Number of elements within 
        /// each dimension </param>
        /// <param name="LowerCorner"> Lower corner point's 
        /// location </param>
        /// <param name="Separation"> Separation of each point 
        /// in each axis </param>
        public PointGrid(int[] Resolution = null, 
            double[] LowerCorner = null, double[] Separation = null)
        {
            Points = new List<GridPoint>();

            // Data processing (Don't need to clone since throwing away)
            int[] tmpRes;
            double[] lowCorn, sep;
            tmpRes = Resolution ?? new int[] { 5, 5, 5 };
            lowCorn = LowerCorner ?? new double[] { -4, -4, -4 }; 
            sep = Separation ?? new double[] { 1, 1, 1 };

            // Clones tmpRes
            resolution = tmpRes.Clone() as int[];

            // Grid  related data
            // Rank of point cloud
            rank = this.resolution.Length;
            // Finds total count by multiplying each resolution element
            totCnt = resolution.Aggregate(1, 
                (total, len) => total = total * len);

            // Checks that all inputs agree
            if(lowCorn.Length != rank || sep.Length != rank)
                throw new System.ArgumentException(
                    "All inputs must have the same number of values.", 
                    "PointGrid");

            //////////////
            // Points
            //////////////

            // Finds the unique locations along each axis
            double[][] axisPos = new double[rank][];
            // Increments through rank
            for (int rIdx = 0; rIdx < rank; rIdx++) 
            {
                axisPos[rIdx] = new double[resolution[rIdx]];
                for(int iIdx = 0; iIdx < resolution[rIdx]; iIdx++)
                    axisPos[rIdx][iIdx] = lowCorn[rIdx] + sep[rIdx]*iIdx;
            }

            // Assigns data to list
            for(int idx=0;idx<totCnt;idx++)
            {
                // Maps array to index and position
                int[] thisIdx = new int[rank]; // Index
                double[] thisPos = new double[rank]; // Position
                int runSeg = 1; // Provides a running segment size
                for(int dIdx=0; dIdx<rank; dIdx++)
                {
                    // Defines index
                    thisIdx[dIdx] = (idx/runSeg) % resolution[dIdx];
                    // Assigns position based on the index
                    thisPos[dIdx] = axisPos[dIdx][thisIdx[dIdx]];
                    runSeg *= resolution[dIdx];
                }

                // Makes local grid point
                GridPoint thisPt = new GridPoint(thisPos,thisIdx,true);

                // Adds point, index, and on to list
                Points.Add(thisPt);
            }

            // Identifies surface points (outer most points)
            this.ResetSurfacePts();

            //////////////
            // Cubes
            //////////////

            // Total number of cubes
            int[] cRes = Operator.Subtract(resolution, 1);
            int totCubes = cRes.Aggregate(1, 
                (total, len) => total = total * len);

            // Total corners per cube
            ptsPerCube = 1 << rank; // Total number of corner points

            Cubes = new int[totCubes, ptsPerCube];

            // Indices of lower points that all the remaining points 
            // will be based on
            int scalar = 1; // Multiplicative after modulus
            int divisor = 1; // Divisor before modulus
            for (int rIdx = 0; rIdx < rank; rIdx++ ) // Each dimension
            {
                for (int cIdx = 0; cIdx < totCubes; cIdx++) // Each cube
                    Cubes[cIdx, 0] += scalar * ((cIdx / divisor) 
                        % cRes[rIdx]);
                scalar *= cRes[rIdx] + 1;
                divisor *= cRes[rIdx];
            }

            // Fills in the remaining points 
            // position shifts per dimension
            int[] shiftArr = new int[ptsPerCube]; 
            shiftArr[0] = 1;
            for (int rIdx = 1; rIdx < rank; rIdx++) // Each dimension
                shiftArr[rIdx] = shiftArr[rIdx - 1] * resolution[rIdx-1];

            // Point shift tags
            int[,] shiftTag = new int[ptsPerCube, rank];
            for (int pIdx = 0; pIdx < ptsPerCube; pIdx++)
                for (int rIdx = 0; rIdx < rank; rIdx++) // each dimension
                    shiftTag[pIdx, rIdx] = pIdx >> rIdx & 1; // Adjacent pts

            // Records cube corner points
            for (int cIdx = 0; cIdx < totCubes; cIdx++) // Each cube
                for (int pIdx = 1; pIdx < ptsPerCube; pIdx++) // each point
                {
                    Cubes[cIdx, pIdx] = Cubes[cIdx, 0];
                    for (int rIdx = 0; rIdx < rank; rIdx++) // Each dimension
                        Cubes[cIdx, pIdx] += shiftArr[rIdx] * 
                            shiftTag[pIdx, rIdx];
                }
        }
        # endregion Constructors
        # region Methods

        /// <summary>
        /// Returns a reference to the point located at
        /// the inputted indices
        /// </summary>
        /// <param name="index"> N-dimensional indices </param>
        /// <returns> Grid point at that position </returns>
        public GridPoint getPoint(int[] index)
        {
            int linIdx = Function.LinearIndex(index, resolution);
            return Points[linIdx];
        }

        /// <summary>
        /// Retrieves a reference to the corner points for the cube index
        /// </summary>
        /// <param name="cubeIndex"> Index of the cube in "Cubes" </param>
        /// <returns> All points of the cube </returns>
        public GridPoint[] getCorners(int cubeIndex)
        {
            int len = Cubes.GetLength(1);
            GridPoint[] corners = new GridPoint[len];
            for(int cIdx=0;cIdx<len;cIdx++)
                corners[cIdx] = Points[Cubes[cubeIndex, cIdx]];
            return corners;
        }

        /// <summary>
        /// Retrieves the indices of the grid points that 
        /// make up the requested cube
        /// </summary>
        /// <param name="cubeIndex"> Index of the cube in "Cubes" </param>
        /// <returns> All indices of the cube  </returns>
        public int[] getCornersIDs(int cubeIndex)
        {
            int len = Cubes.GetLength(1);
            int[] corners = new int[len];
            for (int cIdx = 0; cIdx < len; cIdx++)
                corners[cIdx] = Cubes[cubeIndex, cIdx];
            return corners;
        }

        /// <summary>
        /// Transforms the points in this point grid using a square matrix
        /// </summary>
        /// <param name="trans"> Transform instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        public void Transform(Transform trans, bool inverse = false)
        {
            for (int idx = 0; idx < this.Points.Count; idx++)
                Points[idx].Transform(trans, inverse);
        }

        /// <summary>
        /// Applied a transform matrix to a copy of this point grid
        /// </summary>
        /// <param name="trans"> Transform instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        /// <returns> Copy of this point grid in m-space </returns>
        public PointGrid CopyTransform(Transform trans, 
            bool inverse = false)
        {
            PointGrid newPG = (PointGrid)this.Clone();
            newPG.Transform(trans, inverse);
            return newPG;
        }

        /// <summary>
        /// Returns a clone of this point grid by performing a deep copy.
        /// </summary>
        /// <returns> Copy of point grid </returns>
        public PointGrid Clone()
        {
            PointGrid newGrid = new PointGrid();
            newGrid.resolution = (int[])resolution.Clone();
            newGrid.Points = Points.Select(pt => pt.Clone()).ToList();
            newGrid.Cubes = (int[,])Cubes.Clone();
            return newGrid;
        }

        /// <summary>
        /// Converts the position, index, and On values for the grid points 
        /// in the point grid into single, multi-dimensional arrays.
        /// </summary>
        /// <returns> A Tuple containing: 1) Positions, 2) Indices, 
        /// 3) Above switch, 4) On Surface, 4) Value </returns>
        public Tuple<double[,], int[,], bool[], bool[], double[]> 
            ConvertToArrays()
        {
            int len = this.Points.Count;
            int dims = this.Points[0].Comp.Length;

            double[,] pts = new double[len, dims];
            int[,] idxs = new int[len, dims];
            bool[] above = new bool[len];
            bool[] surf = new bool[len];
            double[] vals = new double[len];

            // Maximum number of associated cubes
            int maxCubes = 1 << dims; // Right shifts to get 2^dims

            // Cycles through each point and adds it to the array
            for (int lIdx = 0; lIdx < len; lIdx++)
            {
                // Dimensions
                for (int dIdx = 0; dIdx < dims; dIdx++)
                {
                    pts[lIdx, dIdx] = Points[lIdx].Comp[dIdx];
                    idxs[lIdx, dIdx] = Points[lIdx].Index[dIdx];
                }
                above[lIdx] = Points[lIdx].Above;
                surf[lIdx] = Points[lIdx].OnSurface;
                vals[lIdx] = Points[lIdx].Value;
            }

            return new Tuple<double[,], int[,], bool[], bool[], double[]>
                (pts, idxs, above, surf, vals);
        }

        /// <summary>
        /// Sets the Above field for all points to true
        /// </summary>
        public void SetPtsInside()
        {
            Points.ForEach(pt => pt.above = true);
        }

        /// <summary>
        /// Sets the Above field for all points to false
        /// </summary>
        public void SetPtsOutside()
        {
            Points.ForEach(pt => pt.above = false);
        }

        /// <summary>
        /// Resets all OnSurface tags to the bounding surface of the
        /// full grid
        /// </summary>
        public void ResetSurfacePts()
        {
            Points.ForEach(pt => pt.OnSurface = outsideSurfacePt(pt));
        }

        /// <summary>
        /// Determines if the point is an outer point on the grid 
        /// based on its Tag
        /// </summary>
        /// <param name="gridPt"> Grid point to evaluate </param>
        /// <returns> Outer border determination </returns>
        private bool outsideSurfacePt(GridPoint gridPt)
        {
            // Lower bound
            if (Are.Any(gridPt.Index, i => i == 0))
                return true;
            // Upper bound
            else
                // Checks each dimension and compares to the resolution
                for (int idx = 0; idx < gridPt.Comp.Length; idx++)
                    if (gridPt.Index[idx] == this.Resolution[idx] - 1)
                        return true;
            // At this point, it's not a grid border point
            return false;
        }

        /// <summary>
        /// Identifies surface points. i.e. points that have at
        /// least one adjacent point either missing or switched off
        /// </summary>
        public void IdentifySurfacePts()
        {
            // Book-keeping
            int totCnt = this.Points.Count; // Total number of points
            int rank = this.resolution.Length; // Rank of point cloud
            // Adjacent points offset
            int[] shift = new int[rank];
            shift[0] = 1; // First dimension is always +/-1
            for (int idx = 1; idx < shift.Length; idx++)
                shift[idx] = shift[idx - 1] * this.resolution[idx - 1];

            // Reset all points
            //this.ResetSurfacePts();

            // Cycles through each point
            for (int idx = 0; idx < Points.Count; idx++)
            {
                // 1) If a point is off, updates tag, moves on
                if (!Points[idx].Above)
                {
                    Points[idx].OnSurface = false;
                    continue; // Steps to next point
                }

                // All points past here are Above threshold

                // The OnSurface calculation goes like this
                // 1) Already checked the point is above threshold
                // 2) If already marked as a surface, then it moves on
                // 3) Checks adjacent points in each axis. If any adjacent
                // Point is below threshold, then the point is a surface 
                // point. Because the exterior grid points are marked 
                // as surface points at creation, the point indexing
                // is greatly simplified
                int aIdx = 0;
                while (aIdx < rank && !Points[idx].OnSurface)
                {
                    Points[idx].OnSurface = 
                        !Points[idx - shift[aIdx]].Above || 
                        !Points[idx + shift[aIdx]].Above;
                    aIdx++;
                }
            }
        }

        /// <summary>
        /// Applies the threshold to every point in the grid.  Above 
        /// threshold will set Above=true, otherwise Above=false
        /// </summary>
        /// <param name="thresold"> Threshold to apply </param>
        public void ApplyThreshold(double thresold)
        {
            // Cycles through each point via lambda expression
            Points.ForEach(pt => pt.ApplyThreshold(thresold));
        }

        /// <summary>
        /// Assigns an array of values to the point grid.  
        /// The value array must be the same length as the 
        /// number of points.
        /// </summary>
        /// <param name="values"> Array of values </param>
        public void CopyValues(double[] values)
        {
            // Checks that are as many values as points
            if (values.Length != Points.Count)
                throw new System.ArgumentException(
                    "The number of values must match the number of points.", 
                    "PointGrid");
            // Assigns values to each point
            for (int idx = 0; idx < Points.Count; idx++)
                Points[idx].Value = values[idx];
        }

        # endregion Methods
    }

    ///////////////////////////////////////
    /////////// GridPoint /////////////////
    ///////////////////////////////////////

    /// <summary>
    /// GridPoint is used within PointGrid to represent elements 
    /// within a lattice.
    /// GridPoint is a child of Point with two additional fields
    /// </summary>
    [Serializable]
    public class GridPoint : Point
    {
        #region Fields & Properties

        /// <summary>
        /// Identifies points that are above threshold, 
        /// which will be within the facet surface.
        /// </summary>
        internal bool above;

        /// <summary>
        /// Identifies points that are above threshold, 
        /// which will be within the facet surface.
        /// </summary>
        public bool Above { get { return above; } }

        /// <summary>
        /// Identifies the point and being an outside point in the grid.
        /// (Defaults to false.)
        /// </summary>
        public bool OnSurface;

        /// <summary>
        /// Index of this grid point within the grid lattice
        /// </summary>
        protected int[] index;

        /// <summary>
        /// Index of this grid point within the grid lattice
        /// </summary>
        public int[] Index { get { return index; } }

        /// <summary>
        /// Value of the point
        /// </summary>
        public double Value { get; set; }

        # endregion Fields & Properties
        # region Constructors

        /// <summary>
        /// GridPoint Constructor
        /// </summary>
        /// <param name="pos"> Component position location </param>
        /// <param name="indx"> Index array of this point within the 
        /// grid lattice </param>
        /// <param name="above"> Boolean switch for indicating a point 
        /// as above threshold </param>
        /// <param name="value"> Boolean switch tag </param>
        public GridPoint(double[] pos = null, int[] indx = null, 
            bool above = true, double value = 0) : base(pos)
        {
            // Additional fields
            this.above = above; // Above marker
            Value = value; // point values
            index = indx ?? new int[pos.Length];

            // Checks that all inputs agree
            if (pos.Length != index.Length)
                throw new System.ArgumentException(
                    "All inputs must have the same number of values.", 
                    "GridPoint");
        }

        # endregion Constructor
        # region Methods

        /// <summary>
        /// Returns a clone of this grid point by performing a deep copy.
        /// </summary>
        /// <returns> Grid point copy </returns>
        new public GridPoint Clone()
        {
            return new GridPoint((double[])Comp.Clone(), 
                (int[])Index.Clone(), Above, Value);
        }

        /// <summary>
        /// If the point value > threshold, the point is tags as Above=true
        /// </summary>
        /// <param name="threshold"> Threshold to apply </param>
        public void ApplyThreshold(double threshold)
        {
            if (Value > threshold)
                above = true;
            else
                above = false;
        }

        # endregion Methods
    }
}
