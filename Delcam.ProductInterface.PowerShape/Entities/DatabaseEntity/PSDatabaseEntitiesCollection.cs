// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Linq;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Base class for curve collections.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class PSDatabaseEntitiesCollection<T> : List<T> where T : PSDatabaseEntity
    {
        #region " Fields "

        /// <summary>
        /// The PowerSHAPE Automation object.
        /// </summary>
        protected PSAutomation _powerSHAPE;

        /// <summary>
        /// The DatabaseEntitiesCollection as a list and is used in place of 'Me'.
        /// </summary>
        //protected List<T> _list;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        internal PSDatabaseEntitiesCollection(PSAutomation powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the entity with the specified name. If an entity with the specified names does not exist
        /// then Nothing is returned.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <returns>It may return Nothing if entity is not found.</returns>
        public T GetByName(string name)
        {
            // See if we can find an entity with the specified name
            foreach (PSDatabaseEntity entity in this)
            {
                if (entity.Name == name)
                {
                    return (T) entity;
                }
            }

            // None found, so return Nothing
            return null;
        }

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity for
        /// selection operations.
        /// </summary>
        internal abstract string Identifier { get; }

        #endregion

        #region " DatabaseEntitiesCollection Operations "

        /// <summary>
        /// Adds a new DatabaseEntity to the List.
        /// </summary>
        /// <param name="databaseEntity">The entity to add.</param>
        /// <remarks></remarks>
        internal void Add(T databaseEntity)
        {
            if (Contains(databaseEntity) == false)
            {
                base.Add(databaseEntity);
            }
        }

        /// <summary>
        /// Adds a new DatabaseEntity to the List.
        /// </summary>
        /// <param name="databaseEntity">The entity to add.</param>
        /// <remarks></remarks>
        internal void AddNoChecks(T databaseEntity)
        {
            Add(databaseEntity);
        }

        /// <summary>
        /// Removes the item with the specified name.
        /// </summary>
        /// <param name="name">The name of the entity to remove.</param>
        /// <returns>True if item is successfully removed; otherwise, false.</returns>
        /// <remarks></remarks>
        public virtual bool Remove(string name)
        {
            var entity = this.FirstOrDefault(x => x.Name == name);
            if (entity != null)
            {
                Remove(entity);
                return true;
            }
            return false;
        }

        public virtual void Clear()
        {
            base.Clear();
        }

        #endregion
    }
}