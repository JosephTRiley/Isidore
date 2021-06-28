using System;
using System.Collections.Generic;
using System.Linq;
using Isidore.Maths;

namespace Isidore.Render
{
    /// <summary>
    /// Represents an axis-aligned octree.  Since this is an octree,
    /// there are no spatial transforms.  
    /// </summary>
    public class MeshOctree: ICloneable
    {
        #region Fields & Properties

        /// <summary>
        /// Component mesh octboxes
        /// </summary>
        protected internal List<MeshOctBox> meshoctboxes;

        #endregion Fields & Properties
        #region Constructors

        /// <summary>
        /// Constructor using a Mesh.  maxFacetCount sets the threshold
        /// for when to trigger a subdivision.  maxTreeDepth supersedes
        /// maxFacetCount.
        /// </summary>
        /// <param name="mesh"> Mesh to apply the octree to </param>
        /// <param name="maxFacetCount"> Maximum number of facets per 
        /// octbox </param>
        /// <param name="maxTreeDepth"> Maximum tree depth </param>
        public MeshOctree(Mesh mesh, int maxFacetCount = 20,
            int maxTreeDepth = 4):this(mesh.GlobalVertices, mesh.Facets,
                maxFacetCount, maxTreeDepth)
        { }

        /// <summary>
        /// Constructor using Mesh components.  maxFacetCount sets the 
        /// threshold for when to trigger a subdivision.  maxTreeDepth 
        /// supersedes maxFacetCount.
        /// </summary>
        /// <param name="vertices"> Mesh vertices list </param>
        /// <param name="facets"> Mesh facet list </param>
        /// <param name="maxFacetCount"> Maximum number of facets per 
        /// octbox </param>
        /// <param name="maxTreeDepth"> Maximum tree depth </param>
        public MeshOctree(Vertices vertices, List<int[]> facets, 
            int maxFacetCount = 25, int maxTreeDepth = 4)
        {
            // First octbox has and ID of 0
            meshoctboxes = new List<MeshOctBox>();
            meshoctboxes.Add(new MeshOctBox(vertices, new int[] { 0 }));
            meshoctboxes[0].FacetOverlap = 
                Enumerable.Range(0, facets.Count).ToList();

            // Subdivides based on overlapping facet count
            for(int idx = 0; idx < meshoctboxes.Count; idx++)
            {
                // If the number of facets for this octbox is above the max
                // and that the tree depth is below the maximum depth
                if(meshoctboxes[idx].FacetOverlap.Count > maxFacetCount &&
                    meshoctboxes[idx].Index.Length < maxTreeDepth)
                {
                    // Subdivides the current mesh octbox
                    meshoctboxes[idx].Subdivide();

                    // Processes each child box
                    foreach (MeshOctBox box in meshoctboxes[idx].ChildBoxes)
                    {
                        // Finds overlapping facets
                        box.FindFacetOverlap(vertices, facets, 
                            meshoctboxes[idx].FacetOverlap);

                        // Adds to mesh octree
                        meshoctboxes.Add(box);
                    }
                }
            }
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Returns a list of which mesh octboxes are currently on
        /// </summary>
        /// <returns> On status of each octbox </returns>
        public bool[] IsOn()
        {
            bool[] isOn = new bool[meshoctboxes.Count];
            for (int idx = 0; idx < meshoctboxes.Count; idx++)
                isOn[idx] = meshoctboxes[idx].On;
            return isOn;
        }

        /// <summary>
        /// Returns a list of octbox intersection structures for matching
        /// the OctBox list of this MeshOctree
        /// </summary>
        /// <param name="ray"> Ray to intersect </param>
        /// <returns> List of octbox intersect structure corresponding to 
        /// the boxes in the mesh octbox list </returns>
        public List<OctBoxIntersect> Intersect(Ray ray)
        {
            List<OctBoxIntersect> octree = new List<OctBoxIntersect>();

            // This records whether the box is on, since we will be flipping
            // those not intersected to Off
            bool[] realOn = IsOn();

            for (int idx = 0; idx < meshoctboxes.Count; idx++)
            {
                OctBoxIntersect oData = meshoctboxes[idx].Intersect(ray);
                octree.Add(oData);
                // If this box is not hit, then turns off all children so
                // they are not traced
                if (!oData.Hit)
                    meshoctboxes[idx].On = false;
            }

            // Resets the boxes On flags to their actual setting
            for (int idx = 0; idx < meshoctboxes.Count; idx++)
                meshoctboxes[idx].On = realOn[idx];

            return octree;
        }

        /// <summary>
        /// Deep-copy (Non-referenced) clone
        /// </summary>
        /// <returns> Cloned copy </returns>
        public MeshOctree Clone()
        {
            return CloneImp();
        }

        /// <summary>
        /// Deep-copy (Non-referenced) clone casted as an object class
        /// </summary>
        /// <returns> Object class clone </returns>
        object ICloneable.Clone()
        {
            return CloneImp();
        }

        /// <summary>
        /// Clone implementation. Uses MemberwiseClone to clone, and 
        /// inheriting classes will implement the cloning of
        /// specific data types 
        /// </summary>
        /// <returns> Clone copy </returns>
        protected virtual MeshOctree CloneImp()
        {
            // Shallow copy
            var newCopy = (MeshOctree)MemberwiseClone();

            // Deep copy
            DeepCopyOverride(ref newCopy);
            
            return newCopy;
        }

        /// <summary>
        /// Implements deep copies of members that would
        /// otherwise be shallow copied.
        /// </summary>
        /// <param name="copy"> Clone copy </param>
        protected virtual void DeepCopyOverride(ref MeshOctree copy)
        {
            // protected internal List<MeshOctBox> mesh octboxes;
            if (meshoctboxes != null)
                copy.meshoctboxes = meshoctboxes.Select(c => c.Clone()).ToList();
        }

        #endregion Methods
    }
}
