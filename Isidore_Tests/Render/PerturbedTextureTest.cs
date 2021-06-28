using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Isidore.Maths;
using Isidore.Render;
using Isidore.Matlab;

namespace Isidore_Tests
{
    /// <summary>
    /// Tests procedural textures and noise functions
    /// </summary>
    class PerturbedTextureTest
    {
        /// <summary>
        /// Tests that perturbed texture is working as expected
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            Console.WriteLine("\n-Starting Perturbed Texture Function Check");

            Stopwatch watch = new Stopwatch();

            ///////////////////////////////////////////////////////////////////
            // 1: Checks perturbation multiplier
            ///////////////////////////////////////////////////////////////////

            //// Billboard
            //var surface = new Billboard(new Point(-1, -1, 10),
            //    new Normal(0, 0, -1), new Vector(0, 1, 0), 3, 2);
            var surface = new Isidore.Render.Plane(new Point(0, 0, 10),
                new Normal(0, 0, -1), new Vector(0, 1, 0));

            // Orthonormal projector located on the origin
            var proj = new RectangleProjector(140, 120, 0.01, 0.01, 0, 0);
            //var proj = new RectangleProjector(1000, 500, 
            //    0.0025, 0.0025, 0, 0);
            var pos0 = proj.Pos0;
            var pos1 = proj.Pos1;
            int len0 = pos0.Length;
            int len1 = pos1.Length;
   
            // Perturbation parameters
            var perturbMult = Enumerable.Range(0, 101).
                Select(p => (double)p / 100).ToArray();
            var plen = perturbMult.Length;

            // Makes a perturb texture
            // Source texture
            var noiseFunc1 = new PerlinNoiseFunction(1234, 10);
            var shift1 = new Vector(new double[] { 12.3, 23.4, 34.5, 45.6 });
            var noise1 = new fBmNoise(noiseFunc1, 0.125, 512, 1.0 / 3.0, 2,
                    shift1, 1);
            var mapTexture = new ProceduralTexture(noise1);

            // Perturbation texture
            var noiseFunc2 = new PerlinNoiseFunction(-1234, 10);
            var shift2 = new Vector(new double[] { 112.3, 123.4, 134.5, 145.6 });
            //var noise2 = new fBmNoise(noiseFunc2, 0.125, 512, 1.0 / 3.0, 2,
            //        shift2, 1);
            var noise2 = new PerlinTurbulenceNoise(noiseFunc2, 0.125, 512,
                false, 2, shift2);
            var pTexture = new ProceduralTexture(noise2);

            // Perturbed Texture
            var pVec = new Vector(0, 1, 0);
            var perturbedTexture = new PerturbedTexture(mapTexture, 
                pTexture, pVec);
            var textVal = new TextureValue(perturbedTexture, true);
            var matStack = new MaterialStack(textVal);

            // Scene
            var scene = new Scene();
            scene.Projectors.Add(proj);
            scene.Bodies.Add(surface);
            surface.Materials.Add(matStack);

            // Perturbation multiplier loop
            var textArr1 = new double[len0, len1, plen];
            for (int pidx = 0; pidx < plen; pidx++)
            {
                // Updates the perturbation texture
                //noise2 = new fBmNoise(noiseFunc2, 0.125, 512, 1.0 / 3.0, 2,
                //    shift2, perturbMult[pidx]);
                noise2 = new PerlinTurbulenceNoise(noiseFunc2, 0.125, 512,
                    false, 2, shift1, perturbMult[pidx]);
                pTexture.noise = noise2;

                watch.Reset();
                watch.Start();
                scene.AdvanceToTime(0, true);
                watch.Stop();
                Console.WriteLine(
                    "Perturbed Texture step {0}/{1} render time: {2}ms",
                    pidx, plen - 1, watch.ElapsedMilliseconds);

                // Data extraction
                for (int idx0 = 0; idx0 < len0; idx0++)
                    for (int idx1 = 0; idx1 < len1; idx1++)
                    {
                        var iData = proj.Ray(idx0, idx1).Rays[0].IntersectData;
                        if (iData.Hit)
                            textArr1[idx0, idx1, pidx] =
                                iData.GetPropertyData<double, Scalar>("Value");
                    }
            }

            ///////////////////////////////////////////////////////////////////
            // 2: Checks perturbation multiplier
            ///////////////////////////////////////////////////////////////////

            // Sets the plane 10m in front of the camera, 
            // moving at 1 m/s along the X-axis
            var trans1 = Transform.Translate(0, 0, 10);
            var trans2 = Transform.Translate(2, 0, 10);
            var tline = new KeyFrameTrans();
            tline.AddKeys(trans1, 0);
            tline.AddKeys(trans2, 2);
            surface.TransformTimeLine = tline;

            // Samples every 1/20 sec for 2 sec
            var perturbTime = Enumerable.Range(0, 101).
                Select(x => x / 50.0).ToArray();
            var len2 = perturbTime.Length;

            // Cycles through each time step
            var textArr2 = new double[len0, len1, len2];
            var textArr3 = new double[len0, len1, len2];
            for (int aidx = 0; aidx < 2; aidx++)
            {
                noise2.absoluteValueNoise = aidx == 0 ? false : true;
                for (int idx = 0; idx < perturbTime.Length; idx++)
                {
                    // Runs time stepRetrieves the ray
                    watch.Reset();
                    watch.Start();
                    scene.AdvanceToTime(perturbTime[idx], true);
                    watch.Stop();
                    Console.WriteLine(
                        "Shifted Perturbed Texture step {0}/{1}, render time: {2}ms",
                        idx, plen - 1, watch.ElapsedMilliseconds);

                    // Data extraction
                    for (int idx0 = 0; idx0 < len0; idx0++)
                        for (int idx1 = 0; idx1 < len1; idx1++)
                        {
                            var iData = 
                                proj.Ray(idx0, idx1).Rays[0].IntersectData;
                            if (iData.Hit)
                            {
                                if (aidx == 0)
                                    textArr2[idx0, idx1, idx] =
                                        iData.GetPropertyData<double, 
                                        Scalar>("Value");
                                else
                                    textArr3[idx0, idx1, idx] =
                                        iData.GetPropertyData<double, 
                                        Scalar>("Value");
                            }
                        }
                }
            }

            //////////////////////////////////////
            // MatLab display
            /////////////////////////////////////
            // Finds output directory location
            String fname = new FileInfo(
                System.Windows.Forms.Application.ExecutablePath).DirectoryName;
            String matStr = fname.Remove(fname.IndexOf("bin")) + 
                "OutputData\\Render\\ProceduralTexture";
            // Opens secession, moves to MatLab processing area
            MLApp.MLApp matlab = new MLApp.MLApp();
            String resStr = matlab.Execute("cd('" + matStr + "');");
            resStr = matlab.Execute("clear;");
            MatLab.Put(matlab, "textArr1", textArr1);
            MatLab.Put(matlab, "textArr2", textArr2);
            MatLab.Put(matlab, "textArr3", textArr3);
            MatLab.Put(matlab, "perturbMult", perturbMult);
            MatLab.Put(matlab, "perturbTime", perturbTime);
            matlab.PutWorkspaceData("pos0", "base", proj.Pos0);
            matlab.PutWorkspaceData("pos1", "base", proj.Pos1);
            matlab.Execute("PerturbedTextureCheck;");

            Console.WriteLine("\n Perturbed Texture Check Passed");

            return true;
        }
    }
}
