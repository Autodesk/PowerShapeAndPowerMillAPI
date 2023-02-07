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
using System.Linq;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a Setup object in PowerMILL.
    /// </summary>
    public class PMSetup : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMSetup(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMSetup(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string SETUP_IDENTIFIER = "SETUP";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return SETUP_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the list of all the toolpaths that are in this Setup.
        /// </summary>
        public List<PMToolpath> Toolpaths
        {
            get
            {
                List<PMToolpath> result = new List<PMToolpath>();

                // Get the list of toolpaths that are in the Setup
                var toolpathsGUIDs = PowerMill.DoCommandEx($"PRINT FOLDER 'Setup\\{Name}'").ToString();
                // Remove by replacement all CRs
                toolpathsGUIDs = toolpathsGUIDs.Replace(((char)13).ToString(), string.Empty);
                // Remove by replacement all prefixes
                toolpathsGUIDs = toolpathsGUIDs.Replace($"Setup\\{Name}\\\\", string.Empty);
                // Split the collection by NL
                var splitToolpathGUIDs = toolpathsGUIDs.Split(new[]{(char)10},StringSplitOptions.RemoveEmptyEntries);
                // If no toolpaths were contained within the setup, return the empty list
                if (splitToolpathGUIDs.Length == 0) return result;
                // Convert to GUIDs to resolve parsing concerns
                var targetGUIDs = splitToolpathGUIDs.Select(x => new Guid(x));

                // Use the retrieved GUIDs to select from the session's complete toolpath collection.
                // Iterating in this manor will order the resultant list by the order in which the 
                // toolpaths appear in the setup
                try
                {
                    var setupToolpaths = targetGUIDs
                        .Select(targetGUID => PowerMill.ActiveProject.Toolpaths
                            .First(toolpath => toolpath.ID == targetGUID));
                    result = setupToolpaths.ToList();
                }
                catch (InvalidOperationException e)
                {
                    throw new Exception(
                        "Unable to correlate the toolpaths contained within the setup to those of the active project." +
                        "Consider using the Refresh method on the active project prior to access");
                }
                return result;
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes Setup. It also updates PowerMill.
        /// </summary>
        public override void Delete()
        {
            PowerMill.ActiveProject.Setups.Remove(this);
        }

        #endregion
    }
}