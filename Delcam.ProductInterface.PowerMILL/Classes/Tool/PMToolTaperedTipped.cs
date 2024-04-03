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
    /// Represents a tapered tipped tool in PowerMill.
    /// </summary>
    /// <remarks></remarks>
    public class PMToolTaperedTipped : PMTool
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMToolTaperedTipped(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMToolTaperedTipped(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the tip radius of a Tapered Tipped tool.
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

        /// <summary>
        /// Gets and sets the taper height of a Tapered Tipped tool.
        /// </summary>
        public MM TaperHeight
        {
            get
            {
                return
                    Convert.ToDouble(
                        PowerMill.GetPowerMillEntityParameter("tool", Name, "taperheight").Trim());
            }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" TAPERHEIGHT \"" + value + "\"", "TOOL ACCEPT"); }
        }

        /// <summary>
        /// Gets and sets the taper diameter of a Tapered Tipped tool.
        /// </summary>
        public MM TaperDiameter
        {
            get { return (Diameter / 2.0 - TaperHeight * Math.Tan(((Radian) TaperAngle).Value)) * 2.0; }
            set { PowerMill.DoCommand("EDIT TOOL \"" + Name + "\" TAPERDIAMETER \"" + value + "\"", "TOOL ACCEPT"); }
        }

        #endregion
    }
}