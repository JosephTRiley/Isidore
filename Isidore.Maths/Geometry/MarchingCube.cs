using System;
using System.Collections.Generic;

namespace Isidore.Maths
{
    /// <summary>
    /// Implementation of Marching Cube class of algorithms for
    /// representing implicit surfaces by discrete values. This
    /// code follows Jules Bloomenthal's approach popular in
    /// implicit surfaces texts.
    /// </summary>
    public class MarchingCube
    {
        // This is

        #region Fields

        // This code uses 3-space point grids which uses a different
        // ordering scheme than a typical native 3D approach.  
        // Here's the key and a 3-space display
        // 3S  3D     
        // --  --     
        //  0  0      
        //  1  4      
        //  2  2      
        //  3  6     
        //  4  1      
        //  5  5      
        //  6  3      
        //  7  7      
        //           
        //
        //      vertex definition   edge definition     face definition
        //           6 ________ 7          _____6__          ________
        //            /|       /|        7/|       /|       /|       /|
        //          /  |     /  |       /  |     /5 |     /  | 6-T /  |
        //        /____|__ /    |     /__4_|__ /    10  /____|__ /3-F |
        //      2|     |  |3    |    |    11  |     |  |     |  | 2-R |
        //       |    4|__|_____|5   |     |__|__2__|  | 4-L |__|_____|
        //       |    /   |    /     8   3/   9    /   |    /1-N|    /
        //       |  /     |  /       |  /     |  /1    |  / 5-B |  /
        //       |/_______|/     z   |/___0___|/       |/_______|/
        //      0          1  y|/_x  

        /// <summary>
        /// Uses either a Marching Cube or Marching Tetrahedral polygonizer
        /// Generally, the tetrahedral method has superior fitting.
        /// Currently only tetra is supported
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Indicates marching cube polygonizer mode
            /// </summary>
            Cube = 1,
            /// <summary>
            /// Indicates marching tetrahedral polygonizer mode
            /// </summary>
            Tetra = 2
        };

        /// <summary>
        /// Provides the corner indices for all six marching tetrahedrons
        /// </summary>
        public readonly static int[][] tetraIdxArr = new int[][]
        {
            new int[] { 0, 2, 1, 4 },
            new int[] { 3, 2, 4, 1 },
            new int[] { 3, 2, 6, 4 },
            new int[] { 3, 1, 4, 5 },
            new int[] { 3, 4, 6, 5 },
            new int[] { 3, 6, 7, 5 },
        };

        /// <summary>
        /// Holds the 256 facet combinations possible with a marching cube
        /// algorithm.  CubeTable is used to populate the table
        /// </summary>
        protected int[][] cubeTable;

        /// <summary>
        /// Reference to point grid containing data apply scalar 
        /// surface to
        /// </summary>
        public PointGrid pointGrid;

        /// <summary>
        /// Facets created by marching
        /// </summary>
        public List<int[]> facets;

        /// <summary>
        /// Vertices created by marching
        /// </summary>
        public List<Point> vertices;

        /// <summary>
        /// Edge descriptions relating each vertices to points 
        /// in the point grid
        /// </summary>
        public List<int[]> edges;

        # endregion Fields
        # region Constructor

        /// <summary>
        /// Constructor automatically applies marching cube using the
        /// supplied threshold.  Alternatively, Polygonize can be called
        /// directly.  The mode determines the method: Mode.Cube = standard
        /// marching cube, Mode.Tetra = tetrahedral marching cube.
        /// CAUTION: Cube has ambiguities, use Tetra whenever possible.
        /// </summary>
        /// <param name="data"> processed point grid data </param>
        /// <param name="threshold"> Threshold indicating the scalar 
        /// field </param>
        /// <param name="mode"> Marching cube mode: Cube, Tetra </param>
        public MarchingCube(ref PointGrid data, double threshold = 0.0, 
            Mode mode = Mode.Tetra)
        {
            pointGrid = data;
            Polygonize(threshold, mode);
        }

        # endregion Constructor
        # region methods
        /// <summary>
        /// Polygonizes (i.e. meshes) the point grid with the supplied
        /// threshold. The mode determines the method: Mode.Cube = standard
        /// marching cube, Mode.Tetra = tetrahedral marching cube.
        /// </summary>
        /// <param name="threshold"> Threshold indicating the scalar 
        /// field </param>
        /// <param name="mode"> Marching cube mode: Cube, Tetra </param>
        public void Polygonize(double threshold = 0.0, 
            Mode mode = Mode.Tetra)
        {
            // Populates cube table
            if (mode == Mode.Cube)
                this.cubeTable = (int[][])CubeTable().Clone();

            // Initializes polygon lists
            // Facet List
            facets = new List<int[]>();
            // Vertices List
            vertices = new List<Point>();
            // Edge List (for removing redundant vertices)
            edges = new List<int[]>(); 

            // Applies threshold to all points
            pointGrid.ApplyThreshold(threshold);

            // For each cube, produces an array of vertices and facets
            // Total number of cubes
            int len = pointGrid.Cubes.GetLength(0);

            if (mode == Mode.Tetra) // marching tetrahedrons
                for (int idx = 0; idx < len; idx++)
                    TetraMarch(idx, threshold);
            else // marching cubes
                for (int idx = 0; idx < len; idx++)
                    CubeMarch(idx, threshold);
        }

        /// <summary>
        /// Returns the vertex and facet lists that describes the
        /// facet mesh
        /// </summary>
        /// <returns> Tuple containing: 1) vertex list, 2) 
        /// facet list </returns>
        public Tuple<double[,], int[,]> getFacetMesh()
        {
            double[,] V = new double[vertices.Count, 3]; // Vertex list
            int[,] F = new int[facets.Count, 3]; // Facet list

            // Vertex list
            for (int vIdx = 0; vIdx < vertices.Count; vIdx++)
                for (int pIdx = 0; pIdx < 3; pIdx++)
                    V[vIdx, pIdx] = vertices[vIdx].Comp[pIdx];

            for (int fIdx = 0; fIdx < facets.Count; fIdx++)
                for (int pIdx = 0; pIdx < 3; pIdx++)
                    F[fIdx, pIdx] = facets[fIdx][pIdx];

            return new Tuple<double[,], int[,]>(V, F);
        }

        /// <summary>
        /// Applies tetrahedral marching to a cube and updates
        /// the facet and vertices lists
        /// </summary>
        /// <param name="cubeIndex"> Indices in cube list </param>
        /// <param name="threshold"> Threshold for scalar 
        /// surface mesh </param>
        public void TetraMarch(int cubeIndex, double threshold)
        {
            // Corner points corresponding to points at cubeIndex
            GridPoint[] cornersPts = pointGrid.getCorners(cubeIndex);
            // This is passed to Edge2Vert
            int[] ptsIdx = pointGrid.getCornersIDs(cubeIndex);

            // Sets 

            // Cycles through all six possible tetrahedron per cube
            for (int tIdx = 0; tIdx < 6; tIdx++) // Each tetrahedral
            {
                // reference to the 4 tetrahedrons' corners 
                int[] corns = tetraIdxArr[tIdx];

                // Tags points that are above threshold via "above"
                // Increments "caseIndex" specifies which 
                // configuration we have 
                bool[] above = new bool[4]; // Positive value tag
                int caseIndex = 0; // Tetrahedral case index
                int iStep = 8; // Index steps from 8 to 1
                // For each tetrahedral corner
                for (int cIdx = 0; cIdx < 4; cIdx++)
                {
                    // Sets above to the point's Above value then
                    // if point's value is greater than threshold, 
                    // then adds to index
                    if (above[cIdx] = cornersPts[corns[cIdx]].Above)
                        caseIndex += iStep;
                    iStep >>= 1; // Shifts right (decreasing)
                }

                // Finds where a edge passes through a threshold, 
                // passes to Edge2Vert to record the vertices 
                // and edge point
                // Each tetrahedral has six sides any can hold a vertex
                int[] vertIndex = new int[6]; 
                if (above[0] != above[1]) // 1st & 2nd points
                    vertIndex[0] = Edge2Vert(ptsIdx[corns[0]], 
                        ptsIdx[corns[1]]);
                if (above[0] != above[2]) // 1st & 3rd points
                    vertIndex[1] = Edge2Vert(ptsIdx[corns[0]], 
                        ptsIdx[corns[2]]);
                if (above[0] != above[3]) // 1st & 4th points
                    vertIndex[2] = Edge2Vert(ptsIdx[corns[0]], 
                        ptsIdx[corns[3]]);
                if (above[1] != above[2]) // 2nd & 3rd points
                    vertIndex[3] = Edge2Vert(ptsIdx[corns[1]], 
                        ptsIdx[corns[2]]);
                if (above[1] != above[3]) // 2nd & 4th points
                    vertIndex[4] = Edge2Vert(ptsIdx[corns[1]], 
                        ptsIdx[corns[3]]);
                if (above[2] != above[3]) // 3rd & 4th points
                    vertIndex[5] = Edge2Vert(ptsIdx[corns[2]], 
                        ptsIdx[corns[3]]);

                // Builds this tetrahedral's facet(s) 
                // from the vertex indices
                // Only 14 cases because full & empty calls 
                // have no facets
                switch(caseIndex)
                {
                    case 1:
                        facets.Add(new int[] { vertIndex[4],
                            vertIndex[5], vertIndex[2] });
                        break;
                    case 2:
                        facets.Add(new int[] { vertIndex[1],
                            vertIndex[5], vertIndex[3] });
                        break;
                    case 3:
                        facets.Add(new int[] { vertIndex[2],
                            vertIndex[4], vertIndex[3] });
                        facets.Add(new int[] { vertIndex[2],
                            vertIndex[3], vertIndex[1] });
                        break;
                    case 4:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[3], vertIndex[4] });
                        break;
                    case 5:
                        facets.Add(new int[] { vertIndex[2],
                            vertIndex[0], vertIndex[3] });
                        facets.Add(new int[] { vertIndex[2],
                            vertIndex[3], vertIndex[5] });
                        break;
                    case 6:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[1], vertIndex[5] });
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[5], vertIndex[4] });
                        break;
                    case 7:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[1], vertIndex[2] });
                        break;
                    case 8:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[2], vertIndex[1] });
                        break;
                    case 9:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[4], vertIndex[5] });
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[5], vertIndex[1] });
                        break;
                    case 10:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[2], vertIndex[5] });
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[5], vertIndex[3] });
                        break;
                    case 11:
                        facets.Add(new int[] { vertIndex[0],
                            vertIndex[4], vertIndex[3] });
                        break;
                    case 12:
                        facets.Add(new int[] { vertIndex[2],
                            vertIndex[1], vertIndex[3] });
                        facets.Add(new int[] { vertIndex[2],
                            vertIndex[3], vertIndex[4] });
                        break;
                    case 13:
                        facets.Add(new int[] { vertIndex[5],
                            vertIndex[1], vertIndex[3] });
                        break;
                    case 14:
                        facets.Add(new int[] { vertIndex[4],
                            vertIndex[2], vertIndex[5] });
                        break;
                }

            }
        }

        /// <summary>
        /// Applies a marching to a cube without decomposition and updates
        /// the facet and vertices lists
        /// </summary>
        /// <param name="cubeIndex"> Indices in cube list </param>
        /// <param name="threshold"> Threshold for scalar surface
        /// mesh </param>
        public void CubeMarch(int cubeIndex, double threshold)
        {
            // Applies threshold to all points

            // Corner points corresponding to points at cubeIndex
            GridPoint[] cornersPts = pointGrid.getCorners(cubeIndex);
            // This is passed to Edge2Vert
            int[] ptsIdx = pointGrid.getCornersIDs(cubeIndex);

            // Finds cube table index by checking each point for threshold
            // crossing in the same order the table was created
            int tableIndex = 0;
            for (int idx = 0; idx < 8; idx++) // For each point
                if (cornersPts[idx].Above) // Above threshold
                    tableIndex += 1 << idx; // Add to index

            // Cube table returns the edges that cross the threshold 
            int[] edgeList = cubeTable[tableIndex];

            // Builds facets using the edge table
            int vert0 = -1; // Anchor vertex that all facets share
            int vert1 = -1; // Previous facet's new vertex
            int cnt = 1; // Need to count to three for facet writing
            // For each edge in this cube table entry
            for (int idx = 0; idx < edgeList.Length; idx++)
            {   
                // Finds the vertex corresponding to this edge
                int vert2 = Edge2Vert(ptsIdx[corner1[edgeList[idx]]], 
                    ptsIdx[corner2[edgeList[idx]]]);

                // Need to bank three vertices for a facet,
                // Otherwise, record the anchor
                if(cnt > 2)
                    facets.Add(new int[] { vert0, vert1, vert2 });
                else
                    vert0 = vert1; // Shifts vertices over
                
                // Shifts vertices over
                vert1 = vert2;

                cnt++; // Increments
            }
        }

        /// <summary>
        /// Finds the vertex corresponding to the edge passing
        /// through the threshold, looks for it on the vertices
        /// list via the edge endpoint list.  Adds the vertex
        /// and edge points to the lists if it's a new point.
        /// Returns the vertex index in the vertices list.
        /// </summary>
        /// <param name="end0"> Edge first end point </param>
        /// <param name="end1"> Edge second end point </param>
        /// <returns> Edge ID </returns>
        public int Edge2Vert(int end0, int end1)
        {
            // Makes sure the values are sorted
            if(end0 > end1)
                Function.Swap(ref end0, ref end1);
            // Checks to see if the vertex is already in the vertices list
            // This is about as fast as a for loop
            int vertIdx = 
                edges.FindIndex(x => (x[0] == end0 && x[1] == end1));
            //int vertIdx = -1; // Bypasses vertex reduction
            // If -1, then it's a new point so add to list
            if (vertIdx == -1)
            {
                // Averages right down the middle 
                // (no weighting for binary values)
                Point vertex = 0.5 * (pointGrid.Points[end0] + 
                    pointGrid.Points[end1]);
                vertices.Add(vertex); // Adds to vertex list
                edges.Add(new int[] { end0, end1 }); // Adds to edge list
                vertIdx = vertices.Count - 1; // Updates vertex index
            }

            return vertIdx;
        }

        /// <summary>
        /// Populates a marching cube table with all 256 possible
        /// facet orientation.  The only drawback is that it doesn't
        /// separate out facets.  This needs to be fixed someday.
        /// </summary>
        /// <returns> Returns 256 possible facet cubes </returns>
        public static int[][] CubeTable()
        {
            // Cube table
            int[][] table = new int[256][];
            for (int idx = 0; idx < 256; idx++) // Table index loop
            {
                // Cube table edge list entry
                List<int> edgeList = new List<int>();

                // Edge completed processing flag
                bool[] processed = new bool[12];

                // Determines which corners are above threshold for 
                // this cube table entry (all off - all on)
                bool[] above = new bool[8];
                for (int cIdx = 0; cIdx < 8; cIdx++)
                    above[cIdx] = (idx >> cIdx & 1) > 0;

                // For each edge in a cube
                for (int eIdx = 0; eIdx < 12; eIdx++)
                {
                    // Processes when there is a threshold crossing 
                    // and hasn't yet been process
                    if (!processed[eIdx] && (
                        above[corner1[eIdx]] != above[corner2[eIdx]]))
                    {
                        
                        // Selects a face based on the corner value
                        int face = above[corner1[eIdx]] ? 
                            rightFace[eIdx] : leftFace[eIdx];

                        // Goes through the edges of the face, 
                        // adding them to the list
                        int startIdx = eIdx; // Starting edge index 
                        int edgeIdx = eIdx; // current edge index
                        bool searching = true;
                        while (searching)
                        {
                            // Finds the next clockwise 
                            // edge index on this face
                            edgeIdx = NextClockWiseEdge(edgeIdx, face);

                            // If there's a threshold crossing,
                            // add its ID to the entry
                            if (above[corner1[edgeIdx]] != 
                                above[corner2[edgeIdx]])
                            {
                                // Adds edge ID to this entry list
                                edgeList.Add(edgeIdx);

                                // Checks to see if this is where we started
                                // If so, steps out of search loop to look 
                                // for unprocessed edges.  Otherwise, 
                                // grab the next face and continue search
                                if (edgeIdx == startIdx)
                                    searching = false;
                                else
                                    face = AdjoiningFace(edgeIdx, face);
                            }

                            // Marks the edge as processed
                            processed[edgeIdx] = true;
                        }
                    }
                }

                // Reverses order (to match convention of Last is First)
                edgeList.Reverse();

                // Converts list to array and copies 
                // it to this cube table entry
                table[idx] = edgeList.ToArray();
            }

            return table;
        }

        /// <summary>
        /// Returns the next clockwise edge from a given edge and face
        /// </summary>
        /// <param name="edge"> Edge integer corresponding to EO 
        /// value </param>
        /// <param name="face"> Face integer corresponding to FO 
        /// value </param>
        /// <returns> Next clockwise edge integer corresponding to EO
        /// value </returns>
        private static int NextClockWiseEdge(int edge, int face)
        {
            // Converts to EO operation
            EO edgeIndex = (EO)edge;
            FO faceIndex = (FO)face;
            EO nextEdge = EO.BF; // Have to provided an initial state
            // Uses switch case and conditional operators
            switch (edgeIndex)
            {
                case (EO.LB):
                    nextEdge = (faceIndex == FO.L) ? EO.LF : EO.BN;
                    break;
                case (EO.LT):
                    nextEdge = (faceIndex == FO.L) ? EO.LN : EO.TF;
                    break;
                case (EO.LN):
                    nextEdge = (faceIndex == FO.L) ? EO.LB : EO.TN;
                    break;
                case (EO.LF):
                    nextEdge = (faceIndex == FO.L) ? EO.LT : EO.BF;
                    break;
                case (EO.RB):
                    nextEdge = (faceIndex == FO.R) ? EO.RN : EO.BF;
                    break;
                case (EO.RT):
                    nextEdge = (faceIndex == FO.R) ? EO.RF : EO.TN;
                    break;
                case (EO.RN):
                    nextEdge = (faceIndex == FO.R) ? EO.RT : EO.BN;
                    break;
                case (EO.RF):
                    nextEdge = (faceIndex == FO.R) ? EO.RB : EO.TF;
                    break;
                case (EO.BN):
                    nextEdge = (faceIndex == FO.B) ? EO.RB : EO.LN;
                    break;
                case (EO.BF):
                    nextEdge = (faceIndex == FO.B) ? EO.LB : EO.RF;
                    break;
                case (EO.TN):
                    nextEdge = (faceIndex == FO.T) ? EO.LT : EO.RN;
                    break;
                case (EO.TF):
                    nextEdge = (faceIndex == FO.T) ? EO.RT : EO.LF;
                    break;
            }
            return (int)nextEdge;
        }

        /// <summary>
        /// Face order
        /// 0: left direction: -x, -i
        ///	1: right direction: +x, +i
        /// 2: bottom direction: -y, -j
        /// 3: top direction: +y, +j
        /// 4: near direction: -z, -k
        /// 5: far direction:+z, +k
        /// </summary>
        private enum FO { L = 0, R = 1, B = 2, T = 3, N = 4, F = 5 };

        /// <summary>
        /// Edge order:
        /// left bottom edge
        /// left top edge
        /// left near edge
        /// left far edge
        /// right bottom edge
        /// right top edge
        /// right near edge
        /// right far edge
        /// bottom near edge
        /// bottom far edge
        /// top near edge
        /// top far edge
        /// </summary>
        private enum EO
        {
            LB = 0, LT = 1, LN = 2, LF = 3, RB = 4, RT = 5, RN = 6,
            RF = 7, BN = 8, BF = 9, TN = 10, TF = 11
        };

        /// <summary>
        /// Corner order: 
        /// 0: left bottom near corner
        /// 1: left bottom far corner
        /// 2: left top near corner
        /// 3: left top far corner
        /// 4: right bottom near corner
        /// 5: right bottom far corner
        /// 6: right top near corner
        ///	7: right top far corner
        /// </summary>
        private enum CO
        {
            LBN = 0, LBF = 1, LTN = 2, LTF = 3, RBN = 4,
            RBF = 5, RTN = 6, RTF = 7
        };

        /// <summary>
        /// Face on left when going Corner1 to Corner2
        /// </summary>
        public readonly static int[] leftFace = new int[] {(int)FO.B,
            (int)FO.L, (int)FO.L, (int)FO.F, (int)FO.R, (int)FO.T,
            (int)FO.N, (int)FO.R, (int)FO.N, (int)FO.B, (int)FO.T,
            (int)FO.F};

        /// <summary>
        /// Face on right when going Corner1 to Corner2
        /// </summary>
        public readonly static int[] rightFace = new int[] { (int)FO.L,
            (int)FO.T, (int)FO.N, (int)FO.L, (int)FO.B, (int)FO.R,
            (int)FO.R, (int)FO.F, (int)FO.B, (int)FO.F, (int)FO.N,
            (int)FO.T };

        /// <summary>
        /// First corner for each edge
        /// </summary>
        public readonly static int[] corner1 = new int[] 
        { 0, 2, 0, 4, 1, 3, 1, 5, 0, 4, 2, 6 };

        /// <summary>
        /// Second corner arrangement list
        /// </summary>
        public readonly static int[] corner2 = new int[] 
        { 4, 6, 2, 6, 5, 7, 3, 7, 1, 5, 3, 7 };

        /// <summary>
        /// Returns the face that adjoins with the input face via
        /// the common edge
        /// </summary>
        /// <param name="edge"> Common edge shared by both faces </param>
        /// <param name="face"> Adjacent face </param>
        /// <returns> Adjoining face index </returns>
        private static int AdjoiningFace(int edge, int face)
        {
            // Gets left possible face 
            int otherFace = leftFace[edge];
            // If the left face is the same, 
            // the "other" face must be on the left
            if (face == otherFace)
                return rightFace[edge];
            else
                return otherFace;
        }

        # endregion Methods
    }
}