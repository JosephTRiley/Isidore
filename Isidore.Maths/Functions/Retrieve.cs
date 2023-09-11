using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security.Policy;

namespace Isidore.Maths
{
    /// <summary>
    /// Evaluates every element of an array, returning the result
    /// </summary>
    public class Retrieve<T>
    {
        public static Tout Value<Tout>(T inst, string[] fieldNames)
        {
            {
                // Retrieved value
                object value = inst;

                // Descends through the member levels
                for (int idx = 0; idx < fieldNames.Length; idx++)
                {
                    // Retrieves type, field, and property info
                    Type type = value.GetType();
                    FieldInfo finfo = type.GetField(fieldNames[idx]);
                    PropertyInfo pinfo = type.GetProperty(fieldNames[idx]);
                    if (finfo == null && pinfo == null)
                        return default(Tout);
                    if (finfo != null)
                        value = finfo.GetValue(value);
                    else
                        value = pinfo.GetValue(value);
                }

                return (Tout)value;
            }
        }

        public static Tout Value<Tout>(T inst, string fieldName)
        {
            // Splits fieldname string into an array of field strings
            string[] fieldNames = fieldName.Split('.');

            return Value<Tout>(inst, fieldNames);
        }

        public static Tout[] Value<Tout>(T[] inst, string fieldName) 
        {
            // Splits fieldname string into an array of field strings
            string[] fieldNames = fieldName.Split('.');

            // Cycles through array
            Tout[] iValue = new Tout[inst.Length];
            for (int idx = 0; idx < inst.Length; idx++)
                iValue[idx] = Value<Tout>(inst[idx], fieldNames);

            return iValue;
        }

        public static Tout[,] Value<Tout>(T[,] inst, string fieldName)
        {
            // Splits fieldname string into an array of field strings
            string[] fieldNames = fieldName.Split('.');

            // Cycles through array
            int[] size = new int[] { inst.GetLength(0), inst.GetLength(1) };
            Tout[,] iValue = new Tout[size[0], size[1]];
            for (int idx0 = 0; idx0 < size[0]; idx0++)
                for (int idx1 = 0; idx1 < size[1]; idx1++)
                    iValue[idx0, idx1] = 
                        Value<Tout>(inst[idx0, idx1], fieldNames);

            return iValue;
        }
    }
}
