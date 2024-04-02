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
using System.Xml;

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
            List<string> names = new List<string>();
            var groupsXml = _powerMILL.GetPowerMillParameterXML("extract(folder('GROUP'),'name')").GetElementsByTagName("string");
            foreach (XmlNode groupNode in groupsXml)
            {
                names.Add(groupNode.InnerText);
            }
            return names;
        }

        #endregion
    }
}