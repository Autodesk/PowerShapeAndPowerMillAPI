// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Abstract base class that all list classes are based on.
    /// </summary>
    /// <typeparam name="T">The type of the PSEntity.</typeparam>
    /// <remarks></remarks>
    public abstract class PSEntitiesCollection<T> : PSDatabaseEntitiesCollection<T> where T : PSEntity
    {
        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSEntitiesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Gets the desired indexed entity from the the collection.
        /// </summary>
        /// <returns>The desired entity.</returns>
        /// <value>The index of the desired entity.</value>
        public T this[string name]
        {
            // Use List property
            get
            {
                foreach (T listItem in this)
                {
                    if (listItem.Name == name)
                    {
                        return listItem;
                    }
                }

                throw new ApplicationException("Collection does not contain an item with name '" + name + "'");
            }

            set
            {
                foreach (T listItem in this)
                {
                    if (listItem.Name == name)
                    {
                        this[IndexOf(listItem)] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the entity at the specified index from PowerShape and from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public new void RemoveAt(int index)
        {
            // Check that the index is within range
            if (index >= Count)
            {
                throw new IndexOutOfRangeException("Current number of items is " + Count);
            }

            // Remove item from collection and delete it from PowerSHAPE
            Remove(this[index]);
        }

        /// <summary>
        /// Removes the specified item from PowerShape and from the collection.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if entity was removed from the collection.</returns>
        public new bool Remove(T entity)
        {
            // Get entity's name
            string entityName = entity.Name;

            // Delete the item from PowerSHAPE
            if (entity.Exists)
            {
                entity.AddToSelection(true);
                _powerSHAPE.DoCommand("DELETE");
            }

            // Check whether entity exists and its name matches that to be removed
            return base.Remove(entity);
        }

        /// <summary>
        /// Removes the specified item by name from PowerShape and from the collection.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <returns>True if entity was removed from the collection.</returns>
        public override bool Remove(string entityName)
        {
            // Get entity from its name
            T entity = GetByName(entityName);

            // Delete the item from PowerSHAPE
            if (entity.Exists)
            {
                entity.AddToSelection(true);
                _powerSHAPE.DoCommand("DELETE");
            }

            // Check whether entity exists and its name matches that to be removed
            return base.Remove(entity);
        }

        /// <summary>
        /// Removes all entities in the list.
        /// </summary>
        public override void Clear()
        {
            // Get initial number of items in collection
            int initialCollectionCount = Count;

            // Loop through items, deleting and removing each from the collection
            for (int i = 0; i <= initialCollectionCount - 1; i++)
            {
                // Get last entity
                PSEntity currentEntity = this[Count - 1];

                // Remove entity from collection
                Remove((T) currentEntity);
            }
        }

        /// <summary>
        /// Adds all of the current entity type to the selection.
        /// </summary>
        /// <param name="emptySelectionFirst">If true it will empty PowerShape selection first.</param>
        public virtual void AddToSelection(bool emptySelectionFirst = false)
        {
            // If adding to current selection, get its components to restore them later
            List<PSEntity> previousSelection = new List<PSEntity>();
            if (emptySelectionFirst == false)
            {
                foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
                {
                    previousSelection.Add(selectedEntity);
                }
            }

            // Raise the Filter Selection dialog and select the entities
            _powerSHAPE.DoCommand("FILTERITEMS", "ALLTYPES", "INVERTTYPE");
            _powerSHAPE.DoCommand("SELECTTYPE " + Identifier);
            _powerSHAPE.DoCommand("ALL", "ACCEPT");
            _powerSHAPE.DoCommand("EVERYTHING PARTIALBOX");

            // Restore the previously selected entities
            foreach (PSEntity selectedEntity in previousSelection)
            {
                selectedEntity.AddToSelection();
            }
        }

        /// <summary>
        /// Removes all of the current entity type from the selection
        /// </summary>
        public virtual void RemoveFromSelection()
        {
            // Iterate through the currently selected items, removing all of the current entity
            foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
            {
                if (selectedEntity is T)
                {
                    selectedEntity.RemoveFromSelection();
                }
            }
        }

        #endregion
    }
}