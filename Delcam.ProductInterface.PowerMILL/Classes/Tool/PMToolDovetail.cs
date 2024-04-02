// **********************************************************************
// *         © COPYRIGHT 2024 Autodesk, Inc.All Rights Reserved         *
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
    /// Represents a Dovetail tool in PowerMILL.
    /// </summary>
    public class PMToolDovetail : PMTool
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMToolDovetail(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMToolDovetail(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the upper tip radius of a Dovetail tool.
        /// </summary>
        public MM UpperTipRadius
        {
            get
            {
                return
                    Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("tool", Name, "UpperTipRadius").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" UPPER TIPRADIUS \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the tip radius of a Dovetail tool.
        /// </summary>
        public MM TipRadius
        {
            get
            {
                return
                    Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("tool", Name, "tipradius").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" TIPRADIUS \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the taper angle of a Tapered Tipped tool.
        /// </summary>
        public Degree TaperAngle
        {
            get
            {
                return
                    Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("tool", Name, "taperangle").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" TAPERANGLE \"" + value + "\"", "TOOL ACCEPT"); }
        }

        #endregion
    }
}