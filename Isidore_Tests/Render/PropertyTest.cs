using System;
using System.Collections.Generic;
using System.Linq;
using Isidore.Render;

namespace Isidore_Tests
{
    /// <summary>
    /// Test that property cloning is working as expected
    /// </summary>
    class PropertyTest
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            ///////////////////////////////////////////////////////////////////
            // Every property in Isidore.Render
            ///////////////////////////////////////////////////////////////////

            // Irradiance
            Irradiance irr = new Irradiance(192.0);

            // Spectral Irradiance
            SpectralIrradiance specIrr = new SpectralIrradiance(
                new Spectrum<double, double>(new double[] { 1.0e-6, 2.0e-6 },
                new double[] { 1.0, 2.0 }));

            // Reflectance
            Reflectance reflect = new Reflectance(new double[] { 0.5e-6, 0.75e-6 },
                new double[] { 0.25, 0.5 });

            // Refractive Index
            RefractiveIndex refract = new RefractiveIndex(1.5);
            RefractiveIndex specRefract = new RefractiveIndex(
                new double[] { 3e-6, 5e-6 }, new double[] { 1.3, 1.6 });

            // Temperature
            Temperature temp = new Temperature();
            temp.Value = 300;

            // Wavelength
            Wavelength wavelen = new Wavelength();
            wavelen.Value = new double[] { 0.9e-6 };
            Wavelength wlenArr = new Wavelength(new double[] 
            { 0.5e-6, 0.6e-6, 0.7e-6 });

            // Makes a new list
            List<Property> list = new List<Property>();
            list.Add(irr);
            list.Add(specIrr);
            list.Add(reflect);
            list.Add(refract);
            list.Add(specRefract);
            list.Add(temp);
            list.Add(wavelen);
            list.Add(wlenArr);

            ///////////////////////////////////////////////////////////////////
            // Method 1 for copying a new list
            ///////////////////////////////////////////////////////////////////
            List<Property> newList1 = new List<Property>();
            for (int idx = 0; idx < list.Count; idx++)
                newList1.Add(list[idx].Clone());

            ///////////////////////////////////////////////////////////////////
            // Method 2 
            ///////////////////////////////////////////////////////////////////
            List<Property> newList2 = list.Select(n => n.Clone()).ToList();

            ///////////////////////////////////////////////////////////////////
            // Using the Properties list class
            ///////////////////////////////////////////////////////////////////
            // These are copies of the children
            Wavelength copyWlen = wavelen.Clone();
            copyWlen.Value[0] = 2e-6;

            Temperature copyTemp = temp.Clone();
            copyTemp.Value = 32;
            copyTemp.Units = "Celsius";

            // Properties list class
            Properties pList1 = new Properties();
            pList1.Add(copyTemp.Clone());
            pList1.Add(copyWlen.Clone());

            Properties pList2 = pList1.Clone();

            ///////////////////////////////////////////////////////////////////
            // Actual use in Isidore Render
            ///////////////////////////////////////////////////////////////////
            Properties props = new Properties();
            props.Add(irr);
            props.Add(specIrr);
            props.Add(reflect);
            props.Add(refract);
            props.Add(temp);
            props.Add(wavelen);

            IntersectData iData = new IntersectData();
            iData.Properties = props;

            IntersectData iData2 = new IntersectData();
            iData2.Properties = props;

            IntersectData cData = iData.Clone();
            IntersectData cData2 = iData2.Clone();

            var place = cData2.Properties.FindIndex(
                p => p.GetType() == typeof(RefractiveIndex));
            cData2.Properties[place] = specRefract;

            place = cData2.Properties.FindIndex(
                p => p.GetType() == typeof(Wavelength));
            cData2.Properties[place] = wlenArr;

            Console.WriteLine("Finished Property Check");

            return true;
        }
    }
}
