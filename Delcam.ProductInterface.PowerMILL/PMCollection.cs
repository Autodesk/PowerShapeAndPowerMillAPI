// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections;
using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Base class for all PowerMill Product Interface collections which items are not PMEntities.
    /// It controls the addition of items to PowerMill collections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks></remarks>
    public class PMCollection<T> : IList<T>
    {
        #region Fields

        /// <summary>
        /// The list used to implement the interface IList.
        /// </summary>
        protected List<T> _list;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        internal PMCollection()
        {
            _list = new List<T>();
        }

        #endregion

        #region IList Implementation

        /// <summary>
        /// Returns an enumerator of items.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerator<T> IEnumerable_GetEnumerator()
        {
            // Implement IEnumerable method
            return _list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return IEnumerable_GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator of items.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerator GetEnumerator()
        {
            // Implement IEnumerable method
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Adds a new item to the List.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks></remarks>
        public virtual void Add(T item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// Removes all entities in the list.
        /// </summary>
        public virtual void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Determines whether the indicated item is contained within the collection.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>A boolean indicating the item's presence in the collection.</returns>
        /// <remarks></remarks>
        public bool Contains(T item)
        {
            // Cycle through every entity to see whether it matches the indicated entity
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies the contents of the collection to the specified list, starting at the specified index.
        /// </summary>
        /// <param name="array">The destination of the elements copied.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <remarks></remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Call the underlying list method
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>A boolean indicating whether the item has been removed.</returns>
        /// <remarks></remarks>
        public virtual bool Remove(T item)
        {
            //Return _list.Remove(item)
            return _list.Remove(item);
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            // Implement List property
            get { return _list.Count; }
        }

        /// <summary>
        /// PSCollection is not readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Index of item in the list.
        /// </summary>
        /// <param name="item">The item to get the index from.</param>
        /// <returns>The index of the item.</returns>
        /// <remarks></remarks>
        public virtual int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Inserts the specified item into the desired position within the collection.
        /// </summary>
        /// <param name="index">The position into which the item is to be inserted.</param>
        /// <param name="item">The entity to be inserted.</param>
        /// <remarks></remarks>
        public void Insert(int index, T item)
        {
            // Implement List method
            _list.Insert(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <remarks></remarks>
        public virtual void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Gets and sets the desired indexed item.
        /// </summary>
        /// <param name="index">The index of the desired item.</param>
        /// <value>The desired item.</value>
        /// <returns>The desired item.</returns>
        /// <remarks></remarks>
        public virtual T this[int index]
        {
            // Use List property
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion
    }
}