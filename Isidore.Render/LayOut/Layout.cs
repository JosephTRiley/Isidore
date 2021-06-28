using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore
{
    // This version only supports Z-buffer

    /// <summary>
    /// Layout is used to construct scene for an animation
    /// </summary>
    public class Layout
    {
        public Camera Camera;
        public Lasers Lasers;
        public Surfaces Surfaces;
        public Groups AniGroups;
        public double[] sampleTimes;
        public double maxSimTime = 600;

        public Layout()
        {
            sampleTimes = new double[]{0};
            Camera = new Camera();
            Lasers = new Lasers();
            Surfaces = new Surfaces();
            AniGroups = new Groups();
        }

        public void Update(double now)
        {
            Camera.Update(now);
            Lasers.ForEach(laser => laser.Update(now));
            Surfaces.ForEach(surf => surf.Update(now));
        }

        /// <summary>
        /// Prepares the surface name list corresponding tho the ID number
        /// </summary>
        /// <returns> String array of surface names for the layout </returns>
        public string[] getIDList()
        {
            // Creates ID list
            string[] IDList = new string[Surfaces.Count + 1];
            IDList[0] = "0: Miss"; // zero value is a miss
            for (int idx = 0; idx < Surfaces.Count; idx++)
            {
                int cnt = idx + 1;
                IDList[cnt] = cnt.ToString() + ": " + Surfaces[idx].Name;
            }
            return IDList;
        }
    }
}
