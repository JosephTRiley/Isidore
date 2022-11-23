using System.IO;
using Isidore.Library;
using Isidore.Load;
using Isidore.Render;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests the mesh loader ray tracing of the voxel volume
    /// </summary>
    class OBJ_Read
    {
        public static bool Run()
        {
            // Retrieves text from Isidore.Library resource file
            // Note that the resource file was customized by hand
            string Rtext = Models.R3D_text();

            string[] lines = Text.Parse(Rtext);

            // Reads from Resources file
            Polyshape Rs = OBJ.Read(lines);

            // Loads from file
            string dname = new FileInfo(System.Windows.Forms.Application.
                ExecutablePath).DirectoryName;
            string fname = dname.Remove(dname.IndexOf("bin")) +
                    "Inputs\\Rhino3D Files\\PyramidTetrahedron.obj";
            Polyshape Pt = OBJ.Load(fname);

            // Loads a cube without texture information from file
            fname = dname.Remove(dname.IndexOf("bin")) +
                    "Inputs\\Rhino3D Files\\Cube_NoTexture.obj";
            Polyshape cubeNT = OBJ.Load(fname);

            return true;
        }
    }
}
