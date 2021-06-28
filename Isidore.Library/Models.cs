using Isidore.Render;
using Isidore.Library.Properties;
using Isidore.Maths;
using Isidore.Load;

namespace Isidore.Library
{
    /// <summary>
    /// Model library
    /// </summary>
    public class Models
    {
        /// <summary>
        /// Mesh model of a cube
        /// </summary>
        /// <param name="scalingComponents"> Scaling components </param>
        /// <returns> Cube Mesh </returns>
        public static Mesh Cube(double[] scalingComponents = null)
        {
            double[] comp = scalingComponents ?? 
                new double[] { 1.0, 1.0, 1.0 };
            string[] str = Text.Parse(Resources.Cube);
            return StringObj2Mesh(str, comp);
        }

        /// <summary>
        /// Mesh model of a pyramid
        /// </summary>
        /// <param name="scalingComponents"> Scaling components </param>
        /// <returns> Cube Mesh </returns>
        public static Mesh Pyramid(double[] scalingComponents = null)
        {
            double[] comp = scalingComponents ??
                new double[] { 1.0, 1.0, 1.0 };
            string[] str = Text.Parse(Resources.Pyramid);
            return StringObj2Mesh(str, comp);
        }

        /// <summary>
        /// Mesh model of a 3D "R"
        /// </summary>
        /// <param name="scalingComponents"> Scaling components </param>
        /// <returns> "R" Mesh </returns>
        public static Mesh R(double[] scalingComponents = null)
        {
            double[] comp = scalingComponents ??
                new double[] { 1.0, 1.0, 1.0 };
            string[] str = Text.Parse(Resources.R3D);
            return StringObj2Mesh(str, comp);
        }

        /// <summary>
        /// Mesh model of a Toyota Hilux
        /// </summary>
        /// <returns> Hilux Mesh </returns>
        public static Polyshape Hilux()
        {
            string[] str = Text.Parse(Resources.TruckBroad);
            return OBJ.Read(str);
        }

        /// <summary>
        /// Returns the Wavefront object ASCII text data of the
        /// 3D "R" model named R3D.obj.
        /// </summary>
        /// <returns> Single string containing the R3D model </returns>
        public static string R3D_text()
        {
            return Resources.R3D;
        }

        /// <summary>
        /// Converts a Wavefront Object model definition contained in a 
        /// string array to a Mesh object.
        /// </summary>
        /// <param name="textArray"> String array containing the model 
        /// definition </param>
        /// <param name="ScaleComp"> Scaling Components </param>
        /// <returns> Mesh object </returns>
        protected static Mesh StringObj2Mesh(string[] textArray, 
            double[] ScaleComp)
        {
            Transform scale = Transform.Scale(ScaleComp);
            Polyshape meshes = OBJ.Read(textArray);
            Mesh mesh = (Mesh)meshes.Shapes[0];
            mesh.LocalTransform(scale);
            return mesh;
        }
    }
}
