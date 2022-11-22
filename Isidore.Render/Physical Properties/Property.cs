using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

// Property classes are used to record any type of data in a single 
// base format.  Special attention must be paid to cloning.
// This code was developed in a Properties solution file in
// D:\Work\Dissertation\Research\TestCode\List Copying\Properties

namespace Isidore.Render
{
    /// <summary>
    /// The property class is used to represent physical properties
    /// associated with render rays and is used in conjunction
    /// with the Material class.
    /// </summary>
    public abstract class Property : ICloneable
    {
        #region Fields & Properties

        ///// <summary>
        ///// Property's data value
        ///// </summary>
        //public virtual object Value { get; set; }

        /// <summary>
        /// Property's data units
        /// </summary>
        public virtual string Units { get; set; }

        #endregion Fields & Properties
        #region Methods

        /// <summary>
        /// Returns the value of the field or property 
        /// matching the name
        /// </summary>
        /// <typeparam name="T"> Value type </typeparam>
        /// <param name="name"> name of field or property to return </param>
        /// <returns> The data contained in the entry </returns>
        virtual public T GetData<T>(string name)
        {
            // Checks to see if the value is a field
            FieldInfo finfo = GetType().GetField(name);
            if (finfo != null)
            {
                object value = finfo.GetValue(this);
                return (T)value;
            }

            // Checks to see if the values is a property
            PropertyInfo pinfo = GetType().GetProperty(name);
            if (pinfo != null)
            {
                object value = pinfo.GetValue(this);
                return (T)value;
            }

            return default(T);
        }



        /// <summary>
        /// Deep-copy (Non-referenced) clone
        /// </summary>
        /// <returns> Cloned copy </returns>
        public Property Clone()
        {
            return CloneImp();
        }

        /// <summary>
        /// Deep-copy (Non-referenced) clone casted as an object class
        /// </summary>
        /// <returns> Object class clone </returns>
        object ICloneable.Clone()
        {
            return CloneImp();
        }

        /// <summary>
        /// Clone implementation. Uses MemberwiseClone to clone, and 
        /// inheriting classes will implement the cloning of
        /// specific data types 
        /// </summary>
        /// <returns> Clone copy </returns>
        protected virtual Property CloneImp()
        {
            // Shallow copy
            Property newCopy = (Property)MemberwiseClone();
            // Deep copy
            if (Units != null)
                newCopy.Units = (string)Units.Clone();

            return newCopy;
        }

        #endregion Methods
    }

    /// <summary>
    /// Properties is a list of property instances
    /// </summary>
    public class Properties : List<Property>
    {
        /// <summary>
        /// Adds a property to a list.  If a property already has this type,
        /// an exception will be thrown.
        /// </summary>
        /// <param name="property"> Property to add to the list </param>
        new public void Add(Property property)
        {
            Type type = property.GetType();
            if (this.Any(prop => prop.GetType() == type))
                throw new InvalidOperationException(
                    String.Format("Properties already has a entry for type {0}", type));
            else
               base.Add(property);
        }

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        public Properties Clone()
        {

            Properties newList = new Properties();

            ForEach(n => newList.Add(n.Clone()));

            return newList;
        }
    }
}