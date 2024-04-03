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
    /// Represents the collection of Setup objects in the Active PowerMILL Project.
    /// </summary>
    public class PMSetupsCollection : PMEntitiesCollection<PMSetup>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMSetupsCollection(PMAutomation powerMILL) : base(powerMILL)
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
            if (_powerMILL.Version.Major >= 2019)
            {
                foreach (string name in ReadSetups())
                {
                    Add(new PMSetup(_powerMILL, name));
                }
            }
        }

        /// <summary>
        /// Gets the list of the names of all the Setups in PowerMILL.
        /// </summary>
        /// <returns>The list of the names of all the Setups in PowerMILL.</returns>
        internal List<string> ReadSetups()
        {
            var names = new List<string>();
            var SetupsXml = _powerMILL.GetPowerMillParameterXML("extract(folder('" + PMSetup.SETUP_IDENTIFIER + "'),'name')").GetElementsByTagName("string");
            foreach (XmlNode SetupNode in SetupsXml)
            {
                names.Add(SetupNode.InnerText);
            }
            return names;
        }

        #endregion
    }
}