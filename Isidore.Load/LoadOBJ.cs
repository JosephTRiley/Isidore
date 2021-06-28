using System.Collections.Generic;
using System.Linq;
using Isidore.Render;

namespace Isidore.Load
{
    /// <summary>
    /// A toolbox for loading Wavefront Object as a polymesh
    /// </summary>
    public class OBJ
    {
        /// <summary>
        /// Loads a Wavefront Object as a polymesh
        /// </summary>
        /// <param name="fileName"> File name of object file 
        /// to load </param>
        /// <returns> Polymesh </returns>
        public static Polyshape Load(string fileName)
        {
            string[] text = Text.Load(fileName);
            return Read(text);
        }

        /// <summary>
        /// Reads Wavefront Object data contained as an array of strings
        /// and returns a polymesh
        /// </summary>
        /// <param name="lines"> String array containing the 
        /// Object data </param>
        /// <returns> Polymesh </returns>
        public static Polyshape Read(string[] lines)
        {
            // Mesh list to return
            Polyshape meshes = new Polyshape();

            // Temporal list
            List<double[]> vList = new List<double[]>();
            List<double[]> vtList = new List<double[]>();
            List<double[]> vnList = new List<double[]>();
            List<int[]> fList = new List<int[]>();
            string name = null;

            // Book-keeping
            int fOffset = 1; // Facet list numbering offset
            string lastHeader = "";

            // Cycles through each line in the files
            for (int Idx = 0; Idx < lines.Length; Idx++)
            {
                // Skips if line is less than three characters
                if(lines[Idx].Length < 3)
                    continue;

                // Current line
                string line = lines[Idx];

                // Checks if there's a line break, if so, 
                // adds the next line
                if (line.EndsWith("\\"))
                {
                    line = line.TrimEnd('\\',' ');
                    line += lines[++Idx];
                }

                // Extracts the line header 
                string header = line.Substring(0, 2); // Line header

                // Makes a mesh if last header is a facet 
                // and this one isn't
                if(lastHeader == "f " && header!="f ")
                {
                    // Creates and adds the mesh to the list
                    Mesh thisMesh = new Mesh(fList, vList, vnList, vtList);
                    meshes.Add(thisMesh);
                    thisMesh.Name = name;

                    // Updating & resetting
                    fOffset += vList.Count;
                    vList = new List<double[]>();
                    vtList = new List<double[]>();
                    vnList = new List<double[]>();
                    fList = new List<int[]>();
                    name = null;
                }

                // Addresses each line
                switch (header)
                {
                    // Object name identifies a new mesh
                    case "o ": 
                        name = line.Remove(0, 2);
                        break;

                    // Vertex position data (should be first)
                    case "v ": 
                        ParseDoubleAndAdd(ref vList, line.Substring(2));
                        break;

                    // Vertex normal
                    case "vn": 
                        ParseDoubleAndAdd(ref vnList, line.Substring(3));
                        break;

                    // Vertex texture
                    case "vt":
                        ParseDoubleAndAdd(ref vtList, line.Substring(3));
                        break;

                    // Facet (Should be last in list)
                    case "f ":
                        // Replaces missing texture indices with 0s
                        line = line.Replace("//", "/0/");
                        // Parse to usually 3 (v), 9 (v,vt,vn) values
                        int[] ffull = line.Substring(2).Split(' ', '/').
                            Select(t => int.Parse(t)).ToArray();
                        // Finds the integers
                        int inc = ffull.Length / 3;
                        // Parses file and corrects the facet count
                        int[] f = new int[3];
                        for (int fIdx = 0; fIdx < 3; fIdx++)
                            f[fIdx] = ffull[fIdx * inc] - fOffset;
                        fList.Add(f);
                        break;

                    // Ignores all other headers
                }
                lastHeader = header;
            }

            // records the last mesh if there is no blank last line
            if(fList != null)
            {
                Mesh thisMesh = new Mesh(fList, vList, vnList, vtList);
                meshes.Add(thisMesh);
                thisMesh.Name = name;
            }

            return meshes;
        }

        /// <summary>
        /// Parses a string with double data into an array and adds it to 
        /// the list reference
        /// </summary>
        /// <param name="list"> List to add onto </param>
        /// <param name="line"> line to parse and add to the 
        /// list </param>
        /// <param name="delimiter"> Text used to delimit the 
        /// string </param>
        private static void ParseDoubleAndAdd(ref List<double[]> list, 
            string line, char delimiter = ' ')
        {
            double[] v = line.Split(delimiter).
                    Select(t => double.Parse(t)).ToArray();
            list.Add(v);
        }
    }
}