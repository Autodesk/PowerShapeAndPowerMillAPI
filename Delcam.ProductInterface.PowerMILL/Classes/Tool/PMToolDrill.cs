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
    /// Represents a drill tool in PowerMILL.
    /// </summary>
    public class PMToolDrill : PMTool
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMToolDrill(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMToolDrill(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the taper angle of a Tapered Tipped or Drill tool
        /// </summary>
        public Degree TaperAngle
        {
            get
            {
                return
                    Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("tool", Name, "drillangle").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" TAPERANGLE \"" + value + "\"", "TOOL ACCEPT"); }
        }

        #endregion
    }
}