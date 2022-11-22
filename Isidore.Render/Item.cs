using System;
using System.Collections.Generic;
using Isidore.Maths;

namespace Isidore.Render
{
    /// <summary>
    /// An item is anything that exists in 3D space other than a point, vector, 
    /// or ray
    /// </summary>
    public abstract class Item : ICloneable
    {
        # region Fields & Properties

        /// <summary>
        /// Switch to allow an item to be used. If off, the object doesn't 
        /// interact
        /// </summary>
        public bool On = true;

        /// <summary>
        /// Unique Identification number, specific to instance of the child 
        /// class.  If -1, then the scene will assign a value when 
        /// advancing time.  If there are duplicates, then the second instance
        /// on the list will be reassigned the next open integer.
        /// </summary>
        public int ID = -1; 

        /// <summary>
        /// Non-unique identification name
        /// </summary>
        public string Name = "";

        /// <summary>
        /// A user defined description of the item
        /// </summary>
        public string Description = "";
        

        private KeyFrameTrans transformTimeLine = new KeyFrameTrans();

        /// <summary>
        /// Animation transform for this item
        /// </summary>
        public virtual KeyFrameTrans TransformTimeLine
        {
            get { return transformTimeLine; }
            set { transformTimeLine = value; }
        }

        /// <summary>
        /// Sets or retrieves the items's current time state
        /// </summary>
        public double CurrentTime 
        { 
            // Item's time is stored in the animated transform key frame
            get { return TransformTimeLine.CurrentTime; }
            // Setting CurrentTime will call SetToTime method
            //set { AdvanceToTime(value); }
        }

        /// <summary>
        /// Provides the transformation matrix to cast from local to world 
        /// space at the current time.
        /// </summary>
        public Transform Local2World { get { return TransformTimeLine.CurrentValue; } }

        # endregion Fields & Properties
        # region Constructors

        /// <summary>
        /// Item Constructor
        /// </summary>
        public Item()
        {
            On = true;
            ID = 0;
            Name = null;
            TransformTimeLine = new KeyFrameTrans();
            //this.CurrentTime = 0.0;
        }

        # endregion Constructors
        # region Methods

        /// <summary>
        /// Sets the instance's states to time "now"
        /// </summary>
        /// <param name="now"> Time to set  the state of the object to </param>
        /// <param name="force"> Forces AdvanceToTime to run even if the time 
        /// is the same </param>
        public virtual void AdvanceToTime(double now=0, bool force = false)
        {
            // Checks to see if time is different
            if (!force && now == CurrentTime)
                return;
            // This also updates this.CurrentTime which references TimeLine
            TransformTimeLine.InterpolateToTime(now);
        }

        /// <summary>
        /// Deep-copy (Non-referenced) clone
        /// </summary>
        /// <returns> Cloned copy </returns>
        public Item Clone()
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
        protected virtual Item CloneImp()
        {
            // Shallow copy
            Item newCopy = (Item)MemberwiseClone();

            // Deep copy
            DeepCopyOverride(ref newCopy);

            // If the current time has been set, then this should set
            // the interpolated members in the copy
            if(!double.IsNaN(CurrentTime))
                newCopy.AdvanceToTime(CurrentTime, true);

            return newCopy;
        }

        /// <summary>
        /// Implements deep copies of members that would
        /// otherwise be shallow copied.
        /// </summary>
        /// <param name="copy"> Clone copy </param>
        protected virtual void DeepCopyOverride(ref Item copy)
        {
            // KeyFrameTrans TransformTimeLine
            if (TransformTimeLine != null)
                copy.TransformTimeLine = TransformTimeLine.Clone();
        }

        # endregion Methods
    }

    /// <summary>
    /// Items is a list of item instances
    /// </summary>
    public class Items : List<Item>
    {
        /// <summary>
        /// Maximum value of the array
        /// (This is the default value of item.IDs)
        /// </summary>
        private int maxVal = -1;

        /// <summary>
        /// Adds an object to the list.  Automatically checks indices
        /// for null value (-1) or duplicates.
        /// </summary>
        /// <param name="item"> The object to add to the list. </param>
        new public void Add(Item item)
        {
            // Checks & corrects the ID
            // If ID > maxVal, updates maxID
            if (item.ID > maxVal)
            {
                maxVal = item.ID;
            }
            else
            {
                // If null (-1), then increments & assigns maxVal
                if (item.ID == -1)
                    item.ID = ++maxVal;
                // Otherwise Checks for duplicates
                else
                    //int[] idArr = this.Select(tag => tag.ID).ToArray();
                    for (int idx = 0; idx < Count; idx++)
                        if (item.ID == this[idx].ID)
                        {
                            item.ID = ++maxVal;
                            continue;
                        }
            }

            // Adds to list
            base.Add(item);
        }

        /// <summary>
        /// Clones this instance by performing a deep copy
        /// </summary>
        /// <returns> Clone copy of this instance </returns>
        public Items Clone()
        {

            Items newList = new Items();

            ForEach(n => newList.Add(n.Clone()));

            return newList;
        }
    }
}
