using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore.Render
{
    /// <summary>
    /// Texture material interface.
    /// </summary>
    public interface IMaterialTexture
    {
        /// <summary>
        /// Returns a material value at coordinates U,V
        /// </summary>
        /// <param name="U"> U coordinate </param>
        /// <param name="V"> V coordinate </param>
        /// <returns> Material value </returns>
        Material getMaterial(double U, double V);
        //double GetVal(double U, double V, double Idx);
    }

    /// <summary>
    /// Texture material class.  Uses a material list and an array of IDs to
    /// determine the physical characteristics of the texture
    /// </summary>
    public class MaterialTexture:Texture<double>,IMaterialTexture
    {
        private MapTexture<int> matID;
        /// <summary>
        /// Material classes used with the texturing
        /// </summary>
        public Materials materials;

        /// <summary>
        /// Constructor
        /// </summary>
        public MaterialTexture()
        {
            matID = null;
            materials = null;
            //isAvailable = false;
        }

        /// <summary>
        /// Map texture identification number
        /// </summary>
        public MapTexture<int> MatID 
        { 
            get { return matID; } 
            set { matID = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MatID"> Material ID </param>
        /// <param name="MaterialList"> List of materials </param>
        public MaterialTexture(MapTexture<int> MatID, Materials MaterialList)
        {
            matID = new MapTexture<int>(MatID);
            materials = new Materials();
            MaterialList.ForEach(mat => materials.Add(mat));
            //isAvailable = true;
        }

        /// <summary>
        /// Material texture copy constructor
        /// </summary>
        /// <param name="matText0"></param>
        public MaterialTexture(MaterialTexture matText0)
            : this(matText0.matID,matText0.materials)
        {
        }

        /// <summary>
        /// Returns the material corresponding to the material ID at
        /// the specified UV coordinates
        /// </summary>
        /// <param name="U"> U coordinate </param>
        /// <param name="V"> V coordinate </param>
        /// <returns> local material </returns>
        public Material getMaterial(double U, double V)
        {
            int MatID = matID.GetVal(U,V);
            return materials[MatID];
        }

        /// <summary>
        /// Returns the zero index value of the material located at
        /// the specified UV coordinates
        /// </summary>
        /// <param name="U"> U coordinate </param>
        /// <param name="V"> V coordinate </param>
        /// <returns> local material's first value </returns>
        public override double GetVal(double U, double V)
        {
            Material mat = getMaterial(U, V);
            return mat.GetVal(0);
        }

        /// <summary>
        /// Returns the index value of the material located at
        /// the specified UV coordinates
        /// </summary>
        /// <param name="U"> U coordinate </param>
        /// <param name="V"> V coordinate </param>
        /// <param name="Idx"> Material index </param>
        /// <returns> Material value located at UV and the specified index </returns>
        public override double GetVal(double U, double V, double Idx)
        {
            Material mat = getMaterial(U, V);
            return mat.GetVal(Idx);
        }
    }
}
