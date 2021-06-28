namespace Isidore.Load
{
    public partial class Data
    {
        /// <summary>
        /// Data structure containing NASTRAN data
        /// </summary>
        public struct NAS
        {
            /// <summary>
            /// NASTRAN grid point data
            /// </summary>
            public Grid Grid;
            /// <summary>
            /// NASTRAN node patch data
            /// </summary>
            public Node Node;
        }

        /// <summary>
        /// NASTRAN grid point data structure
        /// </summary>
        public struct Grid
        {
            /// <summary>
            /// Identification number
            /// </summary>
            public int[] ID;
            /// <summary>
            /// Location in Cartesian space
            /// </summary>
            public double[,] Position;
        }

        /// <summary>
        /// NASTRAN node patch data
        /// </summary>
        public struct Node
        {
            /// <summary>
            /// Identification number of model patch is a member of
            /// </summary>
            public int[] ModelID;
            /// <summary>
            /// Identification number
            /// </summary>
            public int[] ID;
            /// <summary>
            /// ID number of grid points that make up each facet patch
            /// </summary>
            public int[,] Vertices;
            /// <summary>
            /// List of model names listed in ModelID 
            /// (May have duplicate names)
            /// </summary>
            public string[] ModelList;
        }
    }
}
