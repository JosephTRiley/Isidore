using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer
{
    // Groups will combine surfaces, primative, sprites, etc. into one unit
    public class Group
    {
        Surfaces Surfaces;
        
        public Group()
        {
            Surfaces = new Surfaces();
        }
    }

    public class Groups : List<Group>
    {

    }
}
