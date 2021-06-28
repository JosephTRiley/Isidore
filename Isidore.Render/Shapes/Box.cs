using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore
{
    class Box : Surface
    {

        # region Member

        public Transform localSpace;
        protected Plane[] side = new Plane[6];

        # endregion

        # region Constructor

        public Box(Point LowerCorner, Point UpperCorner)
        {
            // Planes bounding the box
            side[0] = new Plane(LowerCorner, Vector.Down);
            side[1] = new Plane(UpperCorner, Vector.Up);
            side[2] = new Plane(LowerCorner, Vector.Left);
            side[3] = new Plane(UpperCorner, Vector.Right);
            side[4] = new Plane(LowerCorner, Vector.Back);
            side[5] = new Plane(UpperCorner, Vector.Forward);



            for (int idx = 0; idx < 3; idx++)
            {
                if (diff.xyz(idx) < 0)
                {

                }
            }
        }

        # endregion
    }
}
