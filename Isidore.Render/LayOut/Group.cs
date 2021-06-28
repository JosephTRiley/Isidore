using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isidore
{
    /// <summary>
    /// A Group is a 
    /// </summary>
    public class Group
    {
        private Objs objects;

        public Point AnchorPoint = new Point();
        public tPath TransformPath;

        public Objs Objects { get { return objects; } set { objects = value; } }

        public Group()
        {
            Vector Zero = new Vector(0);
            Vector One = new Vector(1);
            TransformPath = new tPath();
        }

        public Transform atTime(double Time)
        {
            return TransformPath.atTime(Time);
        }

    }

    public class Groups : List<Group>
    {
    }

}
