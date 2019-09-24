// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents the base class for all collection classes for PowerMILL.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public abstract class PMEntitiesCollection<T> : PMCollection<T> where T : PMEntity
    {
        #region Fields

        /// <summary>
        /// PowerMILL Automation object
        /// </summary>
        protected PMAutomation _powerMILL;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMEntitiesCollection(PMAutomation powerMILL)
        {
            _powerMILL = powerMILL;
        }

        #endregion

        #region Operations

        /// <summary>
        /// Adds a new Entity to the List.
        /// </summary>
        public override void Add(T entity)
        {
            if (!Contains(entity))
            {
                base.Add(entity);
            }
        }

        /// <summary>
        /// Gets the last entity in the List.
        /// </summary>
        public T LastItem()
        {
            if (Count > 0)
            {
                return this[Count - 1];
            }
            return null;
        }

        /// <summary>
        /// Removes the entity at the specified index on the list and it also removes it from PowerMill.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public override void RemoveAt(int index)
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException("Current number of items is " + Count);
            }
            _powerMILL.DoCommand("DELETE " + this[index].Identifier + " '" + this[index].Name + "'");
            base.RemoveAt(index);
        }

        /// <summary>
        /// Removes the specified item from the list and from PowerMill.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if item is successfully removed; otherwise, false. This method also returns false if item was not found in the System.Collections.Generic.List(Of T).</returns>
        public new bool Remove(T entity)
        {
            if (Contains(entity))
            {
                _powerMILL.DoCommand("DELETE " + entity.Identifier + " '" + entity.Name + "'");
            }
            return base.Remove(entity);
        }

        /// <summary>
        /// Removes the specified item from the list and from PowerMill and applies 'NOQUIBBLE' option if 'useNoQuibble' is set to true.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <param name="useNoQuibble">The 'NOQUIBBLE' option is applied if set to true</param>
        public bool Remove(T entity, bool useNoQuibble)
        {
            if (Contains(entity))
            {
                if (useNoQuibble)
                {
                    _powerMILL.DoCommand("DELETE " + entity.Identifier + " '" + entity.Name + "' NOQUIBBLE");
                }
                else
                {
                    _powerMILL.DoCommand("DELETE " + entity.Identifier + " '" + entity.Name + "'");
                }
            }
            return base.Remove(entity);
        }

        /// <summary>
        /// Removes all entities in the list and in PowerMill.
        /// </summary>
        public override void Clear()
        {
            foreach (T entity in this)
            {
                _powerMILL.DoCommand("DELETE " + entity.Identifier + " '" + entity.Name + "'");
            }
            base.Clear();
        }

        /// <summary>
        /// Gets the entity with the specified name. If an entity with the specified names does not exist
        /// then Nothing is returned.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <returns>It may return Nothing if entity is not found.</returns>
        public T GetByName(string name)
        {
            // See if we can find an entity with the specified name
            foreach (T entity in this)
            {
                if (entity.Name == name)
                {
                    return entity;
                }
            }

            // None found, so return Nothing
            return null;
        }

        /// <summary>
        /// Get the next name that will be generated
        /// </summary>
        /// <param name="basePrefix">Optionnal prefix</param>
        /// <returns>Next entity name as a string</returns>
        public string GetNewEntityName(string basePrefix = null)
        {
            if (basePrefix != null)
            {
                if (Count > 0)
                {
                    if (GetByName(basePrefix) != null)
                    {
                        return _powerMILL
                            .DoCommandEx("PRINT PAR TERSE \"new_entity_name('" + this[0].Identifier + "','" + basePrefix + "')\"")
                            .ToString();
                    }
                    return basePrefix;
                }
                return basePrefix;
            }
            if (Count > 0)
            {
                return _powerMILL.DoCommandEx("PRINT PAR TERSE \"new_entity_name('" + this[0].Identifier + "')\"").ToString();
            }
            return "1";
        }

        /// <summary>
        /// Draws all entities
        /// </summary>
        public void DrawAll()
        {
            System.Reflection.FieldInfo[] fi = typeof(T).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            string identifier = fi[0].GetValue(null).ToString();
            if (identifier != "MODEL")
            {
                _powerMILL.DoCommand("DRAW " + identifier + " ALL");
            }
            else
            {
                _powerMILL.DoCommand("VIEW MODEL; SHADE NORMAL");
                _powerMILL.DoCommand("VIEW MODEL; WIREFRAME ON");
            }
        }

        /// <summary>
        /// Undraws all entities
        /// </summary>
        public void UndrawAll()
        {
            System.Reflection.FieldInfo[] fi = typeof(T).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            string identifier = fi[0].GetValue(null).ToString();
            _powerMILL.DoCommand("UNDRAW " + identifier + " ALL");
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the entity with the specified name.  If an entity with the specified names does not exist
        /// then Nothing is returned.
        /// </summary>
        public T this[string name]
        {
            get
            {
                foreach (var entity in this)
                {
                    if (((T) entity).Name == name)
                    {
                        return (T) entity;
                    }
                }
                return null;
            }
        }

        public T ActiveItem
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                string activeName =
                    _powerMILL.DoCommandEx("PRINT PAR terse \"entity('" + this[0].Identifier + "','').Name\"")
                              .ToString();
                if (string.IsNullOrEmpty(activeName))
                {
                    return null;
                }
                return this[activeName];
            }
        }

        #endregion
    }
    
}