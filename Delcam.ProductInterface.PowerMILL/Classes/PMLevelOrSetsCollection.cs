// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents a collection of Level or Set objects in the Active PowerMILL Project.
    /// </summary>
    public class PMLevelOrSetsCollection : PMEntitiesCollection<PMLevelOrSet>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMLevelOrSetsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the items in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadLevelsAndSets())
            {
                Add(new PMLevelOrSet(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets a list of the names of all the Level and sets in PowerMILL.
        /// </summary>
        /// <returns>The list of the names of all the Levels and sets in PowerMILL.</returns>
        internal List<string> ReadLevelsAndSets()
        {
            List<string> names = new List<string>();
            foreach (var level in _powerMILL.PowerMILLProject.Levels)
            {
                names.Add(level.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a new Level with the specified name.
        /// </summary>
        /// <param name="name">The name of the level.</param>
        /// <returns></returns>
        public PMLevelOrSet CreateLevel(string name)
        {
            // Check to make sure a level by this name does not already exists
            if (this[name] != null)
            {
                return null;
            }

            // Create the level
            _powerMILL.DoCommand("CREATE LEVEL ; LEVEL");

            // Get the new level
            var newLevel = (PMLevelOrSet) _powerMILL.ActiveProject.CreatedItems(typeof(PMLevelOrSet))[0];
            if (!string.IsNullOrEmpty(name))
            {
                newLevel.Name = name;
            }
            Add(newLevel);
            return newLevel;
        }

        /// <summary>
        /// Creates a new set with the specified name.
        /// </summary>
        /// <param name="name">The name of the set.</param>
        /// <returns></returns>
        public PMLevelOrSet CreateSet(string name)
        {
            // Check to make sure a set by this name does not already exists
            if (this[name] != null)
            {
                return null;
            }

            // Create the set
            _powerMILL.DoCommand("CREATE LEVEL ; MODELCOMPSET");

            // Get the new set
            var item = (PMLevelOrSet) _powerMILL.ActiveProject.CreatedItems(typeof(PMLevelOrSet))[0];
            if (!string.IsNullOrEmpty(name))
            {
                item.Name = name;
            }
            Add(item);
            return item;
        }
        #endregion
    }
}