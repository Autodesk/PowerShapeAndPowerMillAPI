// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents the factory for tools.
    /// </summary>
    internal class PMToolEntityFactory
    {
        /// <summary>
        /// Creates a new tool based on its type.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        /// <returns>The new tool instance</returns>
        internal static PMTool CreateEntity(PMAutomation powerMILL, string name)
        {
            var result = powerMILL.GetPowerMillEntityParameter("tool", name, "type").Trim();
            switch (result)
            {
                case "end_mill":
                    return new PMToolEndMill(powerMILL, name);
                case "ball_nosed":
                    return new PMToolBallNosed(powerMILL, name);
                case "tip_radiused":
                    return new PMToolTipRadiused(powerMILL, name);
                case "taper_spherical":
                    return new PMToolTaperedSpherical(powerMILL, name);
                case "taper_tipped":
                    return new PMToolTaperedTipped(powerMILL, name);
                case "drill":
                    return new PMToolDrill(powerMILL, name);
                case "tipped_disc":
                    return new PMToolTippedDisc(powerMILL, name);
                case "off_centre_tip_rad":
                    return new PMToolOffCentreTipRadiused(powerMILL, name);
                case "tap":
                    return new PMToolTap(powerMILL, name);
                case "thread_mill":
                    return new PMToolThreadMill(powerMILL, name);
                case "form":
                    return new PMToolForm(powerMILL, name);
                case "routing":
                    return new PMToolRouting(powerMILL, name);
                case "barrel":
                    return new PMToolBarrel(powerMILL, name);
                case "dovetail":
                    return new PMToolDovetail(powerMILL, name);
                case "turn_profiling":
                    return new PMToolProfilingTurning(powerMILL, name);
                case "turn_grooving":
                    return new PMToolGroovingTurning(powerMILL, name);
                default:
                    return new PMTool(powerMILL, name);
            }
        }
    }
}