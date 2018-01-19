// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.ProductInterface.PowerMILL;

namespace Autodesk.ProductInterface
{
    internal class PMEntityFactory
    {
        /// <summary>
        /// This operation creates instance objects based on an item's identifier in PowerMill
        /// </summary>
        public static PMEntity CreateEntity(PMAutomation powerMill, string identifier, string name)
        {
            if (identifier == PMBoundary.BOUNDARY_IDENTIFIER)
            {
                return PMBoundaryEntityFactory.CreateEntity(powerMill, name);
            }
            if (identifier == PMFeatureSet.FEATURESET_IDENTIFIER)
            {
                return new PMFeatureSet(powerMill, name);
            }
            if (identifier == PMGroup.GROUP_IDENTIFIER)
            {
                return new PMGroup(powerMill, name);
            }
            if (identifier == PMLevelOrSet.LEVEL_OR_SET_IDENTIFIER)
            {
                return new PMLevelOrSet(powerMill, name);
            }
            if (identifier == PMModel.MODEL_IDENTIFIER)
            {
                return new PMModel(powerMill, name);
            }
            if (identifier == PMNCProgram.NC_PROGRAM_IDENTIFIER)
            {
                return new PMNCProgram(powerMill, name);
            }
            if (identifier == PMPattern.PATTERN_IDENTIFIER)
            {
                return new PMPattern(powerMill, name);
            }
            if (identifier == PMStockModel.STOCKMODEL_IDENTIFIER)
            {
                return new PMStockModel(powerMill, name);
            }
            if (identifier == PMToolpath.TOOLPATH_IDENTIFIER)
            {
                return PMToolpathEntityFactory.CreateEntity(powerMill, name);
            }
            if (identifier == PMTool.TOOL_IDENTIFIER)
            {
                return PMToolEntityFactory.CreateEntity(powerMill, name);
            }
            if (identifier == PMWorkplane.WORKPLANE_IDENTIFIER)
            {
                return new PMWorkplane(powerMill, name);
            }
            if (identifier == PMMachineTool.MACHINE_TOOL_IDENTIFIER)
            {
                return new PMMachineTool(powerMill, name);
            }
            return null;
        }
    }
}