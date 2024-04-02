// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents a drilling toolpath strategy in PowerMill.
    /// </summary>
    /// <remarks></remarks>
    public class PMToolpathDrilling : PMToolpath
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMToolpathDrilling(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMToolpathDrilling(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        public DrillCycleTypes CycleType
        {
            get
            {
                var drillType =
                        PowerMill.GetPowerMillEntityParameter("toolpath", Name, "drill.type").Trim();
                switch (drillType)
                {
                    case "single_peck":
                        return DrillCycleTypes.SinglePeck;
                    case "deep_drill":
                        return DrillCycleTypes.DeepDrill;
                    case "break_chip":
                        return DrillCycleTypes.BreakChip;
                    case "tap":
                        return DrillCycleTypes.Tap;
                    case "bore_1":
                        return DrillCycleTypes.Ream;
                    case "bore_2":
                        return DrillCycleTypes.CounterBore;
                    case "bore_3":
                    case "bore_4":
                    case "bore_5":
                        return DrillCycleTypes.Bore;
                    case "helical":
                        return DrillCycleTypes.Helical;
                    case "tap_2":
                        return DrillCycleTypes.RigidTapping;
                    case "deep_2":
                        return DrillCycleTypes.DeepDrill;
                    case "heli_2":
                        return DrillCycleTypes.ReverseHelical;
                    case "heli_cw":
                        return DrillCycleTypes.HelicalClockwise;
                    case "heli_2_cw":
                        return DrillCycleTypes.ReverseHelicalClockwise;
                    case "profile":
                        return DrillCycleTypes.Profile;
                    case "profile_cw":
                        return DrillCycleTypes.ProfileClockwise;
                    case "tap_cw":
                        return DrillCycleTypes.TapClockwise;
                    case "tap_2_cw":
                        return DrillCycleTypes.RigidTapping;
                    case "fine":
                        return DrillCycleTypes.FineBoring;
                    case "thread_mill":
                        return DrillCycleTypes.ThreadMill;
                    case "external_thread":
                        return DrillCycleTypes.ExternalThread;
                    default:
                        throw new Exception("Unsupported cycle type - " + drillType);
                }
            }
            set
            {
                IsActive = true;
                switch (value)
                {
                    case DrillCycleTypes.BreakChip:
                        PowerMill.DoCommand("EDIT DRILL TYPE BREAK_CHIP");
                        break;
                    case DrillCycleTypes.CounterBore:
                        PowerMill.DoCommand("EDIT DRILL TYPE BORE_2");
                        break;
                    case DrillCycleTypes.DeepDrill:
                        PowerMill.DoCommand("EDIT DRILL TYPE DEEP_DRILL");
                        break;
                    case DrillCycleTypes.ExternalThread:
                        PowerMill.DoCommand("EDIT DRILL TYPE EXT_THREAD EDIT DRILL DEPTH HOLE",
                                            "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.FineBoring:
                        PowerMill.DoCommand("EDIT DRILL TYPE FINE", "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.Helical:
                        PowerMill.DoCommand("EDIT DRILL TYPE HELICAL", "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.Profile:
                        PowerMill.DoCommand("EDIT DRILL TYPE CIRCULAR", "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.Ream:
                        PowerMill.DoCommand("EDIT DRILL TYPE BORE_1");
                        break;
                    case DrillCycleTypes.RigidTapping:
                        PowerMill.DoCommand("EDIT DRILL TYPE TAP_2", "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.SinglePeck:
                        PowerMill.DoCommand("EDIT DRILL TYPE DRILL");
                        break;
                    case DrillCycleTypes.Tap:
                        PowerMill.DoCommand("EDIT DRILL TYPE TAP", "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.ThreadMill:
                        PowerMill.DoCommand("EDIT DRILL TYPE THREAD");
                        break;
                    case DrillCycleTypes.Bore:
                        PowerMill.DoCommand("EDIT DRILL TYPE BORE_3");
                        break;
                    case DrillCycleTypes.ReverseHelical:
                        PowerMill.DoCommand("EDIT DRILL TYPE HELICAL_2", "EDIT DRILL CLOCKWISE OFF");
                        break;
                    case DrillCycleTypes.HelicalClockwise:
                        PowerMill.DoCommand("EDIT DRILL TYPE HELICAL", "EDIT DRILL CLOCKWISE ON");
                        break;
                    case DrillCycleTypes.ReverseHelicalClockwise:
                        PowerMill.DoCommand("EDIT DRILL TYPE HELICAL_2", "EDIT DRILL CLOCKWISE ON");
                        break;
                    case DrillCycleTypes.ProfileClockwise:
                        PowerMill.DoCommand("EDIT DRILL TYPE PROFILE", "EDIT DRILL CLOCKWISE ON");
                        break;
                    case DrillCycleTypes.TapClockwise:
                        PowerMill.DoCommand("EDIT DRILL TYPE TAP", "EDIT DRILL CLOCKWISE ON");
                        break;
                }
            }
        }

        #endregion
    }
}