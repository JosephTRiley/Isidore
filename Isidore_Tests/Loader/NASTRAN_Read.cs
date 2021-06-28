using System;
using System.IO;
using Isidore.Load;

namespace Isidore_Tests
{
    class NASTRAN_Read
    {

        public static bool Run()
        {
            // Checks from x32 or x64;
            int check = IntPtr.Size;

            // Relative path to dat file
            String relPath = "\\..\\..\\";
            if (check == 8) // in x64, there's an additional directory
                relPath += "..\\";

            // File Location
            String path = Directory.GetCurrentDirectory();
            String fileName = path + relPath + "Inputs\\NASTRAN Files\\Sphere-000.dat";

            // Loads data
            Data.NAS geomData = Load.NAS(fileName);

            // MatLab Exchange
            MLApp.MLApp matlab = new MLApp.MLApp();
            String strAppDir = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String res = matlab.Execute("clear;");
            res = matlab.Execute("cd('" + strAppDir + "');");
            matlab.PutWorkspaceData("gridID", "base", geomData.Grid.ID);
            matlab.PutWorkspaceData("gridPos", "base", geomData.Grid.Position);
            matlab.PutWorkspaceData("nodesID", "base", geomData.Node.ID);
            matlab.PutWorkspaceData("nodeModelID", "base", geomData.Node.ModelID);
            matlab.PutWorkspaceData("nodeVertexID", "base", geomData.Node.Vertices);
            matlab.Execute("figure;plot3(gridPos(:,1),gridPos(:,2),gridPos(:,3),'.')");
            matlab.Execute("save NASTRAN_Read.mat");

            return true;
        }
    }
}
