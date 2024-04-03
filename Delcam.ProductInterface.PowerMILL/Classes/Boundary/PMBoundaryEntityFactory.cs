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
    /// Represents the factory for boundaries.
    /// </summary>
    internal class PMBoundaryEntityFactory
    {
        /// <summary>
        /// Creates a new boundary based on its type.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The name for the boundary entity.</param>
        /// <returns>The created boundary.</returns>
        /// <remarks></remarks>
        internal static PMBoundary CreateEntity(PMAutomation powerMILL, string name)
        {
            switch (
                powerMILL.GetPowerMillEntityParameter("boundary", name ,"type").Trim())
            {
                case "block":
                    return new PMBoundaryBlock(powerMILL, name);
                case "rest":
                    return new PMBoundaryRest(powerMILL, name);
                case "selected":
                    return new PMBoundarySelectedSurface(powerMILL, name);
                case "shallow":
                    return new PMBoundaryShallow(powerMILL, name);
                case "silhouette":
                    return new PMBoundarySilhouette(powerMILL, name);
                case "collision":
                    return new PMBoundaryCollisionSafe(powerMILL, name);
                case "stockmodel_rest":
                    return new PMBoundaryStockModelRest(powerMILL, name);
                case "contact_point":
                    return new PMBoundaryContactPoint(powerMILL, name);
                case "contact_conv":
                    return new PMBoundaryContactConversion(powerMILL, name);
                case "boolean_operation":
                    return new PMBoundaryBooleanOperation(powerMILL, name);
                case "user":
                    return new PMBoundaryUserDefined(powerMILL, name);
                default:
                    throw new Exception("Failed to determine boundary type");
            }
        }
    }
}