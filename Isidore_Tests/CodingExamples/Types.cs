using System;

namespace Isidore_Tests
{
    /// <summary>
    /// This code provides coding examples that are common in programming
    /// that aren't commonly seen in scientific programming or scripting
    /// </summary>
    partial class CodingExamples
    {
        /// <summary>
        /// Example of typing and detecting
        /// </summary>
        public static void Types()
        {
            Console.WriteLine("Typing examples using derived classes");

            // declares type
            bType btype = new bType();
            dType dtype = new dType();
            d1Type d1type = new d1Type();

            Type tbtype = btype.GetType();
            Type tdtype = dtype.GetType();
            Type td1type = d1type.GetType();

            // Class types
            string tbStr = btype.GetType().Name;
            string tdStr = dtype.GetType().Name;
            string td1Str = d1type.GetType().Name;
            Console.WriteLine("Instance Types"); 
            Console.WriteLine("bType: " + tbStr + "\n dType: " + 
                tdStr + "\n dType1: " + td1Str);

            // Base class type
            string btbStr = tbtype.BaseType.Name;
            string btdStr = tdtype.BaseType.Name;
            string btd1Str = td1type.BaseType.Name;
            Console.WriteLine("Base Types");
            Console.WriteLine("Base bType: " + btbStr + "\nBase dType: " 
                + btdStr + "\nBase dType1: " + btd1Str);

            // Subclass indicator
            bool bSub = tbtype.IsSubclassOf(tbtype);
            bool dSub = tdtype.IsSubclassOf(tbtype);
            bool d1Sub = td1type.IsSubclassOf(td1type);
            Console.WriteLine("Subclass Indicator");
            Console.WriteLine("bType subclass of btype: " + bSub +
                "\ndType subclass of btype: " + dSub +
                "\nd1Type subclass of btype: " + d1Sub);

            // Assignable indicator
            bool bAss = tbtype.IsAssignableFrom(tbtype);
            bool dAss = tdtype.IsAssignableFrom(tbtype);
            bool d1Ass = td1type.IsAssignableFrom(tbtype);
            Console.WriteLine("Assignment Indicator");
            Console.WriteLine("bType assignable to btype: " + bSub +
                "\ndType assignable to btype: " + dSub +
                "\nd1Type assignable to btype: " + d1Sub);

            // Casting check
            var b2dCast = btype as dType;
            var b2d1Cast = btype as d1Type;
            var d2bCast = dtype as bType;
            var d12bCast = d1type as bType;
            Console.WriteLine("Casting check");
            Console.WriteLine("bType casts to dType: " + (b2dCast != null));
            Console.WriteLine("bType casts to d1Type: " + (b2d1Cast != null));
            Console.WriteLine("dType casts to bType: " + (d2bCast != null));
            Console.WriteLine("d1Type casts to bType: " + (d12bCast != null));

        }

        public class bType { }
        public class dType : bType { }
        public class d1Type : dType { }
    }
}
