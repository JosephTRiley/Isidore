using System.Collections.Generic;
using Isidore.Maths;

namespace Isidore.Render
{
    /// <summary>
    /// Vertex is a position, direction, and barycentric coordinate
    /// used with 
    /// </summary>
    public class Vertex
    {
        #region Fields & Properties

        /// <summary>
        /// Vertex position
        /// </summary>
        public Point Position;

        /// <summary>
        /// Vertex normal
        /// </summary>
        public Normal Normal;

        /// <summary>
        /// Barycentric UV coordinates
        /// </summary>
        public double[] UV;

        #endregion Fields & Properties
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Position"> Vertex position </param>
        /// <param name="Normal"> Vertex normal </param>
        /// <param name="UV"> Vertex barycentric (UV) coordinates </param>
        public Vertex(Point Position = null, Normal Normal = null, 
            double[] UV = null)
        {
            this.Position = Position ?? Point.Zero(3);
            this.Normal = Normal ?? Normal.Unit(3, 2);
            this.UV = UV;
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Transforms the vertex components
        /// </summary>
        /// <param name="trans"> Transformation instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        public void Transform(Transform trans, bool inverse = false)
        {
            Position.Transform(trans, inverse);
            Normal.Transform(trans, inverse);
        }

        /// <summary>
        /// Transforms a copy of this vertex
        /// </summary>
        /// <param name="trans"> Transformation instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        /// <returns> Copy of this vertex in m-space </returns>
        public Vertex CopyTransform(Transform trans, bool inverse = false)
        {
            Vertex newVert = Clone();
            newVert.Transform(trans, inverse);
            return newVert;
        }

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        public Vertex Clone()
        {
            if (UV == null)
                return new Vertex(Position.Clone(), Normal.Clone());
            else
                return new Vertex(Position.Clone(), Normal.Clone(),
                    (double[])UV.Clone());
        }

        #endregion Methods

    }

    /// <summary>
    /// List of Vertices
    /// </summary>
    public class Vertices : List<Vertex>
    {
        /// <summary>
        /// Transforms the vertex list
        /// </summary>
        /// <param name="trans"> Transformation instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        public void Transform(Transform trans, bool inverse = false)
        {
            ForEach(vert => vert.Transform(trans, inverse));
        }

        /// <summary>
        /// Transforms a copy of this vertex list
        /// </summary>
        /// <param name="trans"> Transformation instance </param>
        /// <param name="inverse"> Switch for using the inverse 
        /// transform </param>
        /// <returns> Copy of this vertex list in m-space </returns>
        public Vertices CopyTransform(Transform trans, bool inverse = false)
        {
            Vertices newVert = Clone();
            newVert.Transform(trans, inverse);
            return newVert;
        }

        /// <summary>
        /// Clones the list
        /// </summary>
        /// <returns> Clone of list </returns>
        public Vertices Clone()
        {
            // Makes a new list using the default constructor
            Vertices verts = new Vertices();

            // Adds a clone of each list member
            ForEach(vert => verts.Add(vert.Clone()));

            return verts;
        }
    }
}
