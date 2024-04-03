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
    /// Represents the toolpath factory
    /// </summary>
    /// <remarks></remarks>
    internal class PMToolpathEntityFactory
    {
        /// <summary>
        /// Creates a new toolpath based on its strategy.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        /// <returns>The new tool instance</returns>
        internal static PMToolpath CreateEntity(PMAutomation powerMILL, string name)
        {
            switch (
                        powerMILL.GetPowerMillEntityParameter("toolpath", name, "strategy").Trim())
            {
                case "raster":
                    return new PMToolpathRasterFinishing(powerMILL, name);
                case "radial":
                    return new PMToolpathRadialFinishing(powerMILL, name);
                case "spiral":
                    return new PMToolpathSpiralFinishing(powerMILL, name);
                case "pattern":
                    return new PMToolpathPatternFinishing(powerMILL, name);
                case "com_pattern":
                    return new PMToolpathCommittedPattern(powerMILL, name);
                case "com_boundary":
                    return new PMToolpathCommittedBoundary(powerMILL, name);
                case "constantz":
                    return new PMToolpathConstantZFinishing(powerMILL, name);
                case "offset_3d":
                    return new PMToolpath3DOffsetFinishing(powerMILL, name);
                case "pencil_corner":
                    return new PMToolpathPencilCornerFinishing(powerMILL, name);
                case "stitch_corner":
                    return new PMToolpathStitchCornerFinishing(powerMILL, name);
                case "automatic_corner":
                    return new PMToolpathCornerFinishing(powerMILL, name);
                case "along_corner":
                    return new PMToolpathAlongCornerFinishing(powerMILL, name);
                case "multi_pencil_corner":
                    return new PMToolpathMultiPencilCornerFinishing(powerMILL, name);
                case "rotary":
                    return new PMToolpathRotaryFinishing(powerMILL, name);
                case "point_projection":
                    return new PMToolpathPointProjectionFinishing(powerMILL, name);
                case "line_projection":
                    return new PMToolpathLineProjectionFinishing(powerMILL, name);
                case "plane_projection":
                    return new PMToolpathPlaneProjectionFinishing(powerMILL, name);
                case "curve_projection":
                    return new PMToolpathCurveProjectionFinishing(powerMILL, name);
                case "profile":
                    return new PMToolpathProfileFinishing(powerMILL, name);
                case "opti_constz":
                    return new PMToolpathOptimisedConstantZFinishing(powerMILL, name);
                case "inter_constz":
                    return new PMToolpathSteepAndShallowFinishing(powerMILL, name);
                case "swarf":
                    return new PMToolpathSwarfMachining(powerMILL, name);
                case "surface_proj":
                    return new PMToolpathSurfaceProjectionFinishing(powerMILL, name);
                case "embedded":
                    return new PMToolpathEmbeddedPatternFinishing(powerMILL, name);
                case "raster_area_clear":
                    return new PMToolpathRasterAreaClearance(powerMILL, name);
                case "offset_area_clear":
                    return new PMToolpathOffsetAreaClearance(powerMILL, name);
                case "profile_area_clear":
                    return new PMToolpathProfileAreaClearance(powerMILL, name);
                case "drill":
                    return new PMToolpathDrilling(powerMILL, name);
                case "wireframe_swarf":
                    return new PMToolpathWireframeSwarfMachining(powerMILL, name);
                case "raster_flat":
                    return new PMToolpathRasterFlatFinishing(powerMILL, name);
                case "offset_flat":
                    return new PMToolpathOffsetFlatFinishing(powerMILL, name);
                case "plunge":
                    return new PMToolpathPlungeMilling(powerMILL, name);
                case "parametric_offset":
                    return new PMToolpathParametricOffsetFinishing(powerMILL, name);
                case "surface_machine":
                    return new PMToolpathSurfaceFinishing(powerMILL, name);
                case "port_area_clear":
                    return new PMToolpathPortAreaClearance(powerMILL, name);
                case "port_plunge":
                    return new PMToolpathPortPlungeFinishing(powerMILL, name);
                case "port_spiral":
                    return new PMToolpathPortSpiralFinishing(powerMILL, name);
                case "method":
                    return new PMToolpathMethod(powerMILL, name);
                case "blisk":
                    return new PMToolpathBliskAreaClearance(powerMILL, name);
                case "blisk_hub":
                    return new PMToolpathHubFinishing(powerMILL, name);
                case "blisk_blade":
                    return new PMToolpathBladeFinishing(powerMILL, name);
                case "disc_profile":
                    return new PMToolpathDiscProfileFinishing(powerMILL, name);
                case "curve_profile":
                    return new PMToolpathCurveProfile(powerMILL, name);
                case "curve_area_clear":
                    return new PMToolpathCurveAreaClearance(powerMILL, name);
                case "face":
                    return new PMToolpathFaceMilling(powerMILL, name);
                case "chamfer":
                    return new PMToolpathChamferMilling(powerMILL, name);
                case "wireframe_profile":
                    return new PMToolpathWireframeProfileMachining(powerMILL, name);
                case "corner_clear":
                    return new PMToolpathCornerClearance(powerMILL, name);
                case "edge_break":
                    return new PMToolpathEdgeBreak(powerMILL, name);
                case "flowline":
                    return new PMToolpathFlowlineFinishing(powerMILL, name);
                case "parametric_spiral":
                    return new PMToolpathParametricSpiralFinishing(powerMILL, name);
                case "adaptive_area_clear":
                    return new PMToolpathAdaptiveAreaClearance(powerMILL, name);
                case "rib":
                    return new PMToolpathRibMachining(powerMILL, name);
                case "blade":
                    return new PMToolpathBladeMachining(powerMILL, name);
                case "feature_face":
                    return new PMToolpathFeatureFaceMachining(powerMILL, name);
                case "feature_chamfer":
                    return new PMToolpathFeatureChamferMachining(powerMILL, name);
                default:
                    return new PMToolpath(powerMILL, name);
            }
        }
    }
}