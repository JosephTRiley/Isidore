using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    public class Loader
    {
        public static bool Run()
        {
            Console.WriteLine("\n*******************");
            Console.WriteLine("\n Loader Check");
            Console.WriteLine("\n*******************");

            // error monitors
            List<bool> passed = new List<bool>();

            // Maths Tests
            passed.Add(OBJ_Read.Run());
            //passed.Add(Rhino3D_Read.Run());
            passed.Add(NASTRAN_Read.Run());

            bool passedAll = true;
            foreach (bool pass in passed)
                if (!pass) passedAll = false;

            return passedAll;
        }
    }
}
