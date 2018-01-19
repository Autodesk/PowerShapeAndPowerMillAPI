// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents a tapping tool in PowerMill.
    /// </summary>
    /// <remarks></remarks>
    public class PMToolTap : PMTool
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMToolTap(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMToolTap(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the pitch of a Thread Mill or Tapping tool
        /// </summary>
        public MM Pitch
        {
            set
            {
                if (PowerMill.Version < new Version("18.0"))
                {
                    throw new Exception("Pitch is not a valid property for Tapping tools before PowerMILL 2015");
                }
                PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" TAP_PITCH \"" + value + "\"", "TOOL ACCEPT");
            }
        }

        #endregion
    }
}