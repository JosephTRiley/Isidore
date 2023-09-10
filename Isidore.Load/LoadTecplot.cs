using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions; // For using Regex

namespace Isidore.Load
{

    /// <summary>
    /// Provides means for passing data
    /// </summary>
    public partial class Data
    {
        /// <summary>
        /// Structure for storing Tecplot 3D data
        /// </summary>
        public struct Tecplot
        {
            /// <summary>
            /// Plot title
            /// </summary>
            public string Title;

            /// <summary>
            /// Variables names
            /// </summary>
            public string[] Variables;

            /// <summary>
            /// Ant auxiliary data included in the file header
            /// </summary>
            public string[] AuxiliaryData;

            /// <summary>
            /// 3D data contained in the file
            /// </summary>
            public double[][, ,] Data;
        }
    }

    /// <summary>
    /// A toolbox for loading data from files into memory
    /// </summary>
    public partial class Load
    {
        /// <summary>
        /// Tecplot full file name
        /// </summary>
        private static StreamReader textFile;

        /// <summary>
        /// Current line being parsed
        /// </summary>
        private static string thisLine;

        /// <summary>
        /// Regular expression for identifying and parsing any double quotes
        /// </summary>
        private static Regex reg = new Regex("\"([^\"]*)\""); 

        /// <summary>
        /// Processes a tecplot ASCII file and returns the data as a 
        /// 3D array.  Additional data is also contained in the 
        /// outputted structure
        /// </summary>
        /// <param name="fileStr"> Tecplot full file name </param>
        /// <returns> Tecplot data structure </returns>
        public static Data.Tecplot Tecplot(string fileStr)
        {
            
            // Checks for file
            if (!File.Exists(fileStr))
                throw new Exception("File does not exist.");

            // Opens file for reading
            textFile = File.OpenText(fileStr);

            // Parses Header information
            Tuple<string, List<string>, List<string>, int[], bool> items = TecplotHeader();
            string title = items.Item1;
            List<string> variables = items.Item2;
            List<string> auxList = items.Item3;
            int[] dataDims = items.Item4;
            bool blockFormat = items.Item5;

            // Parses data by format
            double[][,,] data = null;
            if (blockFormat)
                data = assembleTecplotBlock(dataDims, variables.Count);
            else
                data = assembleTecplotPoint(dataDims, variables.Count);

            // Builds and returns Tecplot structure
            Data.Tecplot tplot = new Data.Tecplot();
            tplot.Title = items.Item1;
            tplot.Variables = items.Item2.ToArray();
            tplot.AuxiliaryData = items.Item3.ToArray();
            tplot.Data = data;

            return tplot;
        }

        /// <summary>
        /// Processes 3D point formatted Tecplot data.
        /// Assumes stream is already pointed to data.
        /// </summary>
        /// <param name="dimSize"> Array size in each dimension </param>
        /// <param name="valNum"> Number of variables </param>
        /// <returns> 3D data array </returns>
        public static double[][,,] assembleTecplotBlock(int[] dimSize, 
            int valNum)
        {
            // Makes new data array
            double[][,,] data = new double[valNum][,,];
            for(int idx=0; idx<valNum; idx++)
                data[idx] = new double[dimSize[0],dimSize[1],dimSize[2]];

            // Indexers and counters
            int totEl = dimSize[0]*dimSize[1]*dimSize[2]; // Total elements
            int cnt = 0; // Data indices counter
            int inc2 = dimSize[0]*dimSize[1]; // Third axis increment value
            
            // Parses data
            while(!textFile.EndOfStream)
            {
                thisLine = textFile.ReadLine();
                string[] theseStr = thisLine.Split(' ');
                for(int idx = 0; idx < theseStr.Length; idx++)
                {
                    // Checks for empties
                    if(!String.IsNullOrEmpty(theseStr[idx]))
                    {
                        // Current data locations
                        int idx0 = cnt % dimSize[0]; // i
                        int idx1 = (cnt/dimSize[0]) % dimSize[1]; // j
                        int idx2 = (cnt/inc2) % dimSize[2]; // k
                        int idxV = cnt/totEl; // variable

                        // Assigns data
                        double thisVal = double.Parse(theseStr[idx]);
                        data[idxV][idx0, idx1, idx2] = thisVal;
                    
                        // Increment counters
                        cnt ++;
                    }
                }
                
            }

            return data;
        }

        /// <summary>
        /// Processes 3D point formatted Tecplot data
        /// </summary>
        /// <param name="dimSize"> Array size in each dimension </param>
        /// <param name="valNum"> Number of variables </param>
        /// <returns> 3D data array </returns>
        public static double[][, ,] assembleTecplotPoint(int[] dimSize, 
            int valNum)
        {
            throw new NotImplementedException();
        }

       
        /// <summary>
        /// Reads the header of a Tecplot file.  Returning the plot's 
        /// title, variable names, auxiliary information, data size 
        /// in each dimension, and a block format tag 
        /// </summary>
        /// <returns> A tuple containing the data outlined in the 
        /// summary </returns>
        public static Tuple<string, List<string>, 
            List<string>, int[], bool> TecplotHeader()
        {

            // Output Data
            // Plot title
            string titleStr = null;
            // Variable list
            List<string> varList = new List<string>();
            // Auxiliary data list
            List<string> auxDataList = new List<string>();
            // Assumes a 3D array;
            int[] arrSize = new int[3];
            // Identifies the data format as either 
            // point (false) or block (true) 
            bool blockFormat = false; 

            // Cycles through every line of the header data (only, we hope)
            bool searching = true;
            string auxStr = "auxdata";
            // Looks for Zone identifier containing format data
            bool hitZone = false;

            while(searching)
            {
                // Reads the next line and converts it to lower case
                thisLine = textFile.ReadLine();

                // If there is a blank line, a line starting DT=, 
                // then we've entered the data section
                if(String.IsNullOrEmpty(thisLine) || 
                    thisLine.ToLower().Contains("dt="))
                {
                    searching = false;
                }

                // Writes auxdata text to an list
                if (thisLine.ToLower().Contains(auxStr))
                {
                    string str = (string)thisLine.Clone();
                    int idx = str.IndexOf(auxStr, 
                        StringComparison.OrdinalIgnoreCase);
                    auxDataList.Add(
                        str.Substring(idx + auxStr.Length + 1));
                }
                else
                {
                    // Plot title
                    if (thisLine.ToLower().Contains("title"))
                    {
                        // Parses first line since least one variable 
                        // name will be there
                        MatchCollection matches = reg.Matches(thisLine);
                        titleStr = matches[0].ToString().Replace("\"", "");
                    }

                    // Variable names
                    if (thisLine.ToLower().Contains("variables"))
                    {
                        varList = retrieveVars();
                    }

                    // Looks for zone data information
                    if(thisLine.StartsWith("zone", 
                        StringComparison.OrdinalIgnoreCase))
                        hitZone = true;

                    // I size
                    if (hitZone && thisLine.ToLower().Contains(" i="))
                    {
                        arrSize[0] = retrieveInt(thisLine, "i=", ",");
                    }

                    // J size
                    if (hitZone && thisLine.ToLower().Contains(" j="))
                    {
                        arrSize[1] = retrieveInt(thisLine, "j=", ",");
                    }

                    // K size
                    if (hitZone && thisLine.ToLower().Contains(" k="))
                    {
                        arrSize[2] = retrieveInt(thisLine, "k=", ",");
                    }

                    // Block identifier
                    if (hitZone && thisLine.ToLower().Contains("block"))
                        blockFormat = true;
                }
            }

            return Tuple.Create(titleStr, varList, auxDataList, arrSize, 
                blockFormat);
        }

        /// <summary>
        /// Returns an integer bound by the two markers
        /// </summary>
        /// <param name="str"> Text line to process </param>
        /// <param name="mark0"> Left bounding marker </param>
        /// <param name="mark1"> Right bounding marker </param>
        /// <returns> Integer bound by the markers </returns>
        private static int retrieveInt(string str, string mark0, 
            string mark1)
        {
            // Finds the position of the first and last markers
            int idx = str.IndexOf(mark0, 
                StringComparison.OrdinalIgnoreCase) + mark0.Length;
            string thisStr = str.Substring(idx);
            idx = thisStr.IndexOf(mark1, 
                StringComparison.OrdinalIgnoreCase);
            thisStr = thisStr.Substring(0, idx);

            return int.Parse(thisStr);
        }

        /// <summary>
        /// Returns all variable names from a Tecplot file
        /// </summary>
        /// <returns> All variable names </returns>
        private static List<string> retrieveVars()
        {

            List<string> varList = new List<string>();

            // Parses first line since least one variable 
            // name will be there
            MatchCollection matches = reg.Matches(thisLine);
            // Adds to varList
            foreach (object item in matches)
            {
                string varStr = item.ToString().Replace("\"", "");
                varList.Add(varStr);
            }

            // Checks subsequent lines
            // Looks for a double quote on the next line
            while (textFile.Peek() == 34)
            {
                thisLine = textFile.ReadLine();
                matches = reg.Matches(thisLine);
                foreach (object item in matches)
                    varList.Add(item.ToString().Replace("\"", ""));
            }


            return varList;
        }

        /// <summary>
        /// Reads the header of a Tecplot file.  Returning the plot's 
        /// title, variable names, auxiliary information, data size in 
        /// each dimension, and a block format tag 
        /// </summary>
        /// <param name="fileStr"> Tecplot full file name </param>
        /// <returns> A tuple containing the data outlined in the 
        /// summary </returns>
        public static Tuple<string, List<string>, List<string>, int[], bool> 
            TecplotHeader(string fileStr)
        {
            // Checks for file
            if (!File.Exists(fileStr))
                throw new Exception("File does not exist.");

            // Opens file for reading
            textFile = File.OpenText(fileStr);

            Tuple<string, List<string>, List<string>, int[], bool> header = TecplotHeader();

            textFile.Close();

            return header;
        }
    }
}
