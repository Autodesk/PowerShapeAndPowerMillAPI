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

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents the Collection of Group objects in the Active PowerMILL Project.
    /// </summary>
    public class PMGroupsCollection : PMEntitiesCollection<PMGroup>
    {
        #region Constructors

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMGroupsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the groups in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadGroups())
            {
                Add(new PMGroup(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets a list of the names of all the Groups in PowerMILL
        /// </summary>
        /// <returns>The list of the names of all the Groups in PowerMILL</returns>
        internal List<string> ReadGroups()
        {
            string[] items = null;
            List<string> names = new List<string>();
            items = _powerMILL.DoCommandEx("PRINT ENTITY GROUP")
                              .ToString()
                              .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i <= items.Length - 1; i++)
            {
                string name = "";
                var _with1 = items[i];
                int index = _with1.IndexOf("'") + 1;
                if (index > 0)
                {
                    name = _with1.Substring(index, _with1.LastIndexOf("'") - index);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }
            }
            return names;
        }

        #endregion
    }
}