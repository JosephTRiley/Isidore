using System;
using System.IO;
using System.Diagnostics;
using Isidore.Render;
using Isidore.Maths;
using Isidore.Matlab;
using Isidore.ImgProcess;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests ray tracing a sphere surface
    /// </summary>
    class MatTraceReflectance
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Stopwatch watch = new Stopwatch();

            // one meter sphere
            Isidore.Render.Sphere sphere = new Isidore.Render.Sphere(new
                Isidore.Maths.Point(0, 0, 0), 0.5);

            // Attaches the sphere to a polyshape
            Polyshape shape = new Isidore.Render.Polyshape(sphere);

            // Adds straight reflectance material layer to the sphere
            //Reflector reflectance = new Reflector(0.5/Math.PI); // Creates instance
            //MaterialStack reflMats = new MaterialStack(reflectance);
            //sphere.Materials.Add(reflMats);

            // Adds a spectral reflectance material layer to the polyshape
            Reflectance SpecReflect = new Reflectance(new double[]
                { 0.0, 1.0e-6, 10.0e-6 }, new double[] { 1.0 / Math.PI, 2.0 / Math.PI, 11.0 / Math.PI });
            Reflector sReflectance = new Reflector(SpecReflect);
            MaterialStack sReflMats = new MaterialStack(sReflectance);
            shape.Materials.Add(sReflMats);

            // Adds a temperature property via PropertyValue to the polyshape
            Temperature temperature = new Temperature(1000);
            PropertyValue surfaceTemp = new PropertyValue(temperature);
            MaterialStack tempMats = new MaterialStack(surfaceTemp);
            shape.Materials.Add(tempMats);

            // Orthonormal projector located -10m from the shape
            RectangleProjector proj = new RectangleProjector(120, 140, 0.01, 0.01, 0, 0);
            proj.TransformTimeLine = new KeyFrameTrans(Transform.Translate(new double[] { 0, 0, -10 }));

            // Adds a wavelength
            Wavelength wlen = new Wavelength(1.5e-6);
            foreach (RenderRay ray in proj.LocalRays)
                ray.Properties.Add(wlen);

            //  Adds the projector & shape to the scene
            Scene scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(shape);

            // Renders at t=0.0
            watch.Start();

            scene.AdvanceToTime(0.0);

            watch.Stop();
            Console.WriteLine("Material Trace Reflectance Render 1: {0}ms", watch.ElapsedMilliseconds);

            // Checks if AdvanceToTime is able to reject same times (via Item)
            sphere.AdvanceToTime(0.0);

            // Retrieves data
            int len0 = proj.Pos0.Length;
            int len1 = proj.Pos1.Length;
            double[,] x = new double[len0, len1];
            double[,] y = new double[len0, len1];
            int[,] intImg = new int[len0, len1];
            double[,] cosIncImg = new double[len0, len1];
            double[,] depthImg = new double[len0, len1];
            double[,] uImg = new double[len0, len1];
            double[,] vImg = new double[len0, len1];
            double[,] rImg = new double[len0, len1];
            double[,] tImg = new double[len0, len1];

            string rType = typeof(Reflectance).ToString();
            string tType = typeof(Temperature).ToString();
            // Cycles through ray tree to get, casting lets us fill in the blanks if not a map ray
            for (int idx0 = 0; idx0 < len0; idx0++)
                for (int idx1 = 0; idx1 < len1; idx1++)
                {
                    // Retrieves the ray tree for this pixel
                    RayTree thisTree = proj.Ray(idx0, idx1);
                    // And the ray (Could do just this step, was checking above)
                    RenderRay thisRay = proj.Ray(idx0, idx1).Rays[0];

                    // Records position (World space)
                    x[idx0, idx1] = thisRay.Origin.Comp[0];
                    y[idx0, idx1] = thisRay.Origin.Comp[1];

                    // Checks to see if the ray has hit
                    if (thisRay.IntersectData.Hit)
                    {
                        intImg[idx0, idx1] = 1;
                        depthImg[idx0, idx1] = thisRay.IntersectData.Travel;

                        ShapeSpecificData sData = thisRay.IntersectData.BodySpecificData
                            as ShapeSpecificData;
                        if (sData != null)
                        {
                            cosIncImg[idx0, idx1] = sData.CosIncAng;
                            uImg[idx0, idx1] = sData.U;
                            vImg[idx0, idx1] = sData.V;
                        }

                        // Properties extraction 
                        for (int idx = 0; idx < thisRay.IntersectData.Properties.Count; idx++)
                        {
                            // Extracts type
                            string pType = thisRay.IntersectData.Properties[idx].GetType().ToString();

                            // Checks type & Extracts
                            if (pType == rType)
                            {
                                Reflectance thisReflect = (Reflectance)thisRay.IntersectData.Properties[idx];
                                rImg[idx0, idx1] = thisReflect.Coefficient[0];
                            }
                            else if(pType==tType)
                            {
                                Temperature thisTemp = (Temperature)thisRay.IntersectData.Properties[idx];
                                tImg[idx0, idx1] = thisTemp.Value;
                            }

                            // This is another option
                            Temperature getTemp = (Temperature)thisRay.IntersectData.GetProperty<Temperature>();
                            tImg[idx0, idx1] = getTemp.Value;
                        }
                        
                    }
                }

            // This is another way of getting the data our of properties without having to loop
            double[,] tImg1 = proj.GetPropertyData<double, Temperature>("Value");

            // And this retrieves the property tree & data
            Property[,][] tTree = proj.GetPropertyTree<Temperature>();
            double[,][] tTreeData = proj.GetPropertyDataTree<double, Temperature>("Value");

            // This is what happens when you use GetIntersectValue with properties
            Properties[,] pImg = proj.GetIntersectValue<Properties>("Properties");
            int tlen0 = proj.Pos0.Length;
            int tlen1 = proj.Pos1.Length;
            double[,] tImg2 = new double[tlen0, tlen1];
            for (int idx0 = 0; idx0 < tlen0; idx0++)
                for (int idx1 = 0; idx1 < tlen1; idx1++)
                {
                    Properties pCell = pImg[idx0, idx1];
                    for (int pidx = 0; pidx < pCell.Count; pidx++)
                        if (pCell[pidx].GetType() == typeof(Temperature))
                        {
                            Temperature tProp = (Temperature)pCell[pidx];
                            tImg2[idx0, idx1] = tProp.Value;
                        }
                }


            // Displays data
            Figure fig = new Figure();
            fig.Disp(rImg, "Reflectance Map");

            // MatLab processing
            // Finds output directory location
            String fname = new FileInfo(System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + "OutputData\\Render\\MatTrace";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            // Outputs data
            MatLab.Put(matlab, "inter", intImg);
            MatLab.Put(matlab, "depth", depthImg);
            MatLab.Put(matlab, "cosIncImg", cosIncImg);
            MatLab.Put(matlab, "reflect", rImg);
            MatLab.Put(matlab, "temp", tImg);
            MatLab.Put(matlab, "temp1", tImg1);
            MatLab.Put(matlab, "temp2", tImg2);
            MatLab.Put(matlab, "u", uImg);
            MatLab.Put(matlab, "v", vImg);
            MatLab.Put(matlab, "x", x);
            MatLab.Put(matlab, "y", y);
            MatLab.Put(matlab, "pos0", proj.Pos0);
            MatLab.Put(matlab, "pos1", proj.Pos1);
            resStr = matlab.Execute("MatTraceReflectCheck");

            return true;
        }
    }
}
