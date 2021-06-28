using System;
using System.Collections.Generic;

namespace Isidore_Tests
{
    /// <summary>
    /// This code provides coding examples that are common in programming
    /// that aren't commonly seen in scientific programming or scripting
    /// </summary>
    partial class CodingExamples
    {
        /// <summary>
        /// Example of bit shifting
        /// </summary>
        public static void Castings()
        {
            // Some classes
            BaseClass bClass0 = new BaseClass();
            DerivedClass dClass0 = new DerivedClass();
            dClass0.dNum = double.NegativeInfinity;

            // Add to a base class list
            List<BaseClass> bList = new List<BaseClass>();
            bList.Add(bClass0);
            bList.Add(dClass0);

            // Extract derived class from list
            DerivedClass dClass1 = (DerivedClass)bList[1];

            // Holder class
            HolderClass hClass = new HolderClass();
            hClass.Class = dClass0;

            // Now retrieves full class
            DerivedClass rClass = (DerivedClass)hClass.Class;

            // This is the same operation using classes that are cloneable
            iBaseClass ibClass0 = new iBaseClass();
            iDerivedClass idClass0 = new iDerivedClass();
            idClass0.dNum = double.NegativeInfinity;

            // Add to a base class list
            List<iBaseClass> ibList = new List<iBaseClass>();
            ibList.Add(ibClass0);
            ibList.Add(idClass0);

            // Extract derived class from list
            iDerivedClass idClass1 = (iDerivedClass)ibList[1];

            // Holder class
            iHolderClass ihClass = new iHolderClass();
            ihClass.Class = idClass0;

            // Now retrieves full class
            iDerivedClass irClass = (iDerivedClass)ihClass.Class;

            // This is for the actual structures
            idHolderClass idhClass = new idHolderClass();

            iHolderClass ihClass1 = idhClass;
            idHolderClass idhClass1 = (idHolderClass)ihClass1;

        }

        private class BaseClass
        {
            public double bNum = double.PositiveInfinity;
            public double[] bArr = null;
        }

        private class DerivedClass:BaseClass
        {
            public double dNum = double.PositiveInfinity;
        }

        private class HolderClass
        {
            public BaseClass Class = null;
        }

        private class iBaseClass : ICloneable
        {
            public double bNum = double.PositiveInfinity;
            public double[] bArr = null;

            object ICloneable.Clone()
            {
                return MemberwiseClone() as iBaseClass;
            }
        }

        private class iDerivedClass:iBaseClass
        {
            public double dNum = double.PositiveInfinity;
        }

        private class iHolderClass : ICloneable
        {
            public iBaseClass Class = null;

            object ICloneable.Clone()
            {
                return MemberwiseClone() as iHolderClass;
            }
        }

        private class idHolderClass:iHolderClass
        {
            public int[] iArrr = null;
        }
    }
}
