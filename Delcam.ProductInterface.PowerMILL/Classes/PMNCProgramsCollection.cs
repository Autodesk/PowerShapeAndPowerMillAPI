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
    /// Collection of NCProgram objects in the Active PowerMILL Project.
    /// </summary>
    public class PMNCProgramsCollection : PMEntitiesCollection<PMNCProgram>
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMNCProgramsCollection(PMAutomation powerMILL) : base(powerMILL)
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
            foreach (string name in ReadNCPrograms())
            {
                Add(new PMNCProgram(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets the list of names of all NC Programs in PowerMILL.
        /// </summary>
        internal List<string> ReadNCPrograms()
        {
            List<string> names = new List<string>();
            foreach (var ncProgram in _powerMILL.PowerMILLProject.NCPrograms)
            {
                names.Add(ncProgram.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates a new NC program with the specified name.
        /// </summary>
        /// <param name="name">The name for the new NC Program</param>
        public PMNCProgram CreateNCProgram(string name)
        {
            // Create the NC Program
            _powerMILL.DoCommand("CREATE NCPROGRAM ;");

            // Get the new program
            var newProgram = (PMNCProgram) _powerMILL.ActiveProject.CreatedItems(typeof(PMNCProgram))[0];
            if (!string.IsNullOrEmpty(name))
            {
                newProgram.Name = name;
            }
            Add(newProgram);
            return newProgram;
        }

        #endregion
    }
}