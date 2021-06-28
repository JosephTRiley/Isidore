using System;
using System.Linq;
using Isidore.Maths;

namespace Isidore_Tests
{
    class PointTest
    {
        public static bool Run()
        {
            
            /////// Reduce Component

            // Checks null call
            Point Pnull = new Point();
            Ray Rnull = new Ray();

            // Checked component dimensional reduction and increase
            Point P0 = new Point(-13.0, 9.0, 12.0);
            Point Pyz = (Point)P0.Clone();
            Point Pxz = (Point)P0.Clone();
            Point Pxy = P0.CopyReduceComponent(2);

            // Removal
            Pyz.ReduceComponent(0);
            Pxz.ReduceComponent(1);

            // Check
            bool isGood1 = Operator.Equal(Pyz.Comp, new double[] { 9.0, 12.0 }).All(all => all = true);
            if(isGood1)
                isGood1 = Operator.Equal(Pxz.Comp, new double[] { -13.0, 12.0 }).All(all => all = true);
            if (isGood1)
                isGood1 = Operator.Equal(Pxy.Comp, new double[] { -13.0, 9.0 }).All(all => all = true);

            if(isGood1)
                Console.WriteLine("Point.ReduceComponent Succeeded");
            else
                Console.WriteLine("Point.ReduceComponent Check Failed");

            // Perspective Projection check
            Point P1 = new Point(10.0, 20.0, 30.0);
            Transform ppTrans = Transform.Perspective();
            Point Ppp = P1.CopyTransform(ppTrans);
            Point aPpp = Ppp.CopyTransform(ppTrans,true);

            // Perspective Check
            double ang = 30 * Math.PI / 180;
            double near = 1.0;
            double lim = Math.Tan(ang) * near;
            Transform pTrans = Transform.Perspective(-lim, lim, -lim, lim, near, 100.0);
            Point Pp = P1.CopyTransform(pTrans);

            double[] truth1 = new double[] { 10.0 / 30.0, 20.0 / 30.0, 1.0 };
            double[] truth2 = new double[] { (1/3)/lim, (2/3)/lim}; // Only comparing the first two values
            double[] comp = new double[2];
            Array.Copy(Pp.Comp, 0, comp, 0, 2);
            bool isGood2 = Operator.Equal(Ppp.Comp, truth1).All(all => all = true);
            if(isGood2)
                isGood2 = Operator.Equal(comp, truth2).All(all => all = true);

            if (isGood2)
                Console.WriteLine("Point.Perspective Transforms Succeeded");
            else
                Console.WriteLine("Point.Perspective Check Failed");

            // Rigid body transform for converting from local to global
            var Pl = new Point(1, 2, 3);
            var origin = new Point(10, 6, 2);
            var ang0 = Math.PI / 4;
            var ang1 = Math.PI / 6;
            var axis0 = new Vector(Math.Sin(ang0), 0, Math.Sin(ang0));
            var axis1 = new Vector(Math.Sin(ang0) * Math.Sin(-ang1), Math.Cos(ang1), 
                Math.Sin(ang0) * Math.Sin(ang1));
            Transform lTrans = Transform.LocalCoord(origin, axis0, axis1);
            var Pg = Pl.CopyTransform(lTrans); 

            // Checks if deep copy cloning works
            Point P2 = P0.Clone();
            P2.Comp[2] += 2;
            double[] diff = Operator.Subtract(P2.Comp, P0.Comp);
            bool isGood3 = diff[0] == 0 && diff[1] == 0 && diff[2] == 2;
            if (isGood3)
                Console.WriteLine("Point.Clone Check Succeeded");
            else
                Console.WriteLine("Point.Clone Check Failed");

            return isGood1 && isGood2 && isGood3;
            
        }
    }
}
