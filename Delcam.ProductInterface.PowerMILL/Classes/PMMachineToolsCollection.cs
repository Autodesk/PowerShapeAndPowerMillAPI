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
    /// Collection of Machine Tool objects in the Active PowerMILL Project.
    /// </summary>
    public class PMMachineToolsCollection : PMEntitiesCollection<PMMachineTool>
    {
        #region Constructors

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        public PMMachineToolsCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the list of Machine Tools according with PowerMill.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadMachineTools())
            {
                Add(new PMMachineTool(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets the list of names of all machine tools in PowerMILL.
        /// </summary>
        /// <returns>The list of names of all tools in PowerMILL.</returns>
        internal List<string> ReadMachineTools()
        {
            List<string> names = new List<string>();
            foreach (var machineTool in _powerMILL.PowerMILLProject.MachineTools)
            {
                names.Add(machineTool.Name);
            }
            return names;
        }

        #endregion
    }
}