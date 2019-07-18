﻿// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents the collection of FeatureGroup objects in the Active PowerMILL Project.
    /// </summary>
    public class PMFeatureGroupsCollection : PMEntitiesCollection<PMFeatureGroup>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMFeatureGroupsCollection(PMAutomation powerMILL) : base(powerMILL)
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
            if (_powerMILL.Version.Major >= 2018)
            {
                foreach (string name in ReadFeatureGroups())
                {
                    Add(new PMFeatureGroup(_powerMILL, name));
                }
            }
        }

        /// <summary>
        /// Gets the list of the names of all the FeatureGroups in PowerMILL.
        /// </summary>
        /// <returns>The list of the names of all the FeatureGroups in PowerMILL.</returns>
        internal List<string> ReadFeatureGroups()
        {
            string[] items;
            var names = new List<string>();

            items = ((string) _powerMILL.DoCommandEx("PRINT ENTITY " + PMFeatureGroup.FEATUREGROUP_IDENTIFIER)).Split(
                new[] {"\r\n"},
                StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < items.Length; i++)
            {
                var name = "";
                var index = items[i].IndexOf("'") + 1;
                if (index > 0)
                {
                    name = items[i].Substring(index, items[i].LastIndexOf("'") - index);
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }

            return names;
        }

        /// <summary>
        /// Undraw all FeatureGroups.
        /// </summary>        
        public void UndrawAll()
        {
            _powerMILL.DoCommand("UNDRAW FEATUREGROUP ALL");
        }

        #endregion
    }
}