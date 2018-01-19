// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Creates instance objects based on a string identifying an items type in PowerSHAPE.
    /// </summary>
    internal abstract class PSEntityFactory
    {
        /// <summary>
        /// Creates instance objects based on a string identifying an item's type in PowerSHAPE.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="type">The identifier PowerSHAPE uses to identify this type of entity.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="name">The name of the entity.</param>
        /// <returns>The new entity.</returns>
        internal static object CreateEntity(PSAutomation powerSHAPE, string type, int id, string name)
        {
            PSEntity entity = null;

            switch (type.ToUpper())
            {
                case PSAnnotation.ANNOTATION_IDENTIFIER:
                    entity = new PSAnnotation(powerSHAPE, id, name);
                    break;
                case PSArc.ARC_IDENTIFIER:
                    entity = new PSArc(powerSHAPE, id, name);
                    break;
                case PSCompCurve.COMPCURVE_IDENTIFIER:
                case "COMPOSITE CURVE":
                    entity = new PSCompCurve(powerSHAPE, id, name);
                    break;
                case PSCurve.CURVE_IDENTIFIER:
                    entity = new PSCurve(powerSHAPE, id, name);
                    break;
                case PSElectrode.ELECTRODE_IDENTIFIER:
                    entity = new PSElectrode(powerSHAPE, id, name);
                    break;
                case PSLine.LINE_IDENTIFIER:
                    entity = new PSLine(powerSHAPE, id, name);
                    break;
                case PSPoint.POINT_IDENTIFIER:
                    entity = new PSPoint(powerSHAPE, id, name);
                    break;
                case PSSolid.SOLID_IDENTIFIER:
                    entity = (PSEntity) PSSolidFactory.CreateSolid(powerSHAPE, id, name);
                    break;
                case PSMesh.MESH_IDENTIFIER:
                    entity = new PSMesh(powerSHAPE, id, name);
                    break;
                case PSSurface.SURFACE_IDENTIFIER:
                    entity = (PSEntity) PSSurfaceFactory.CreateSurface(powerSHAPE, id, name);
                    break;
                case PSWorkplane.WORKPLANE_IDENTIFIER:
                    entity = new PSWorkplane(powerSHAPE, id, name);
                    break;
            }

            return entity;
        }

        /// <summary>
        /// Creates instance objects based on a string identifying an item's type in PowerSHAPE.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="type">The identifier PowerSHAPE uses to identify this type of entity.</param>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="name">The name of the entity.</param>
        /// <returns>The new entity.</returns>
        internal static object CreateEntity(
            PSAutomation powerSHAPE,
            int type,
            int id,
            string name,
            string description,
            string level)
        {
            PSEntity entity = null;

            switch (type)
            {
                case 61:
                    entity = new PSAnnotation(powerSHAPE, id, name);
                    break;
                case 52:
                    entity = new PSArc(powerSHAPE, id, name);
                    break;
                case 59:
                    entity = new PSCompCurve(powerSHAPE, id, name);
                    break;
                case 53:
                    entity = new PSCurve(powerSHAPE, id, name);
                    break;
                case 58:
                    entity = new PSLine(powerSHAPE, id, name);
                    break;
                case 49:
                    entity = new PSPoint(powerSHAPE, id, name);
                    break;
                case 40:
                    entity = (PSEntity) PSSolidFactory.CreateSolid(powerSHAPE, id, name, description);
                    break;
                case 60:
                    switch (description)
                    {
                        case "Mesh":
                            entity = new PSMesh(powerSHAPE, id, name);
                            break;
                        case "Point":
                            entity = new PSPoint(powerSHAPE, id, name);
                            break;
                        case "Electrode":
                            entity = new PSElectrode(powerSHAPE, id, name);
                            break;
                    }
                    break;
                case 21:
                    entity = (PSEntity) PSSurfaceFactory.CreateSurface(powerSHAPE, id, name, description);
                    break;
                case 54:
                    entity = new PSWorkplane(powerSHAPE, id, name);
                    break;
            }

            if (entity != null)
            {
                entity._levelNumber = int.Parse(level);
            }

            return entity;
        }

        /// <summary>
        /// Creates instance objects based on a string identifying an item's type in PowerSHAPE.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="compositeID">The composite identifier of the entity.</param>
        /// <returns>The new entity.</returns>
        internal static object CreateEntity(PSAutomation powerSHAPE, int compositeID)
        {
            int typeID = compositeID / 20000000;
            switch (typeID)
            {
                case 61:
                    return new PSAnnotation(powerSHAPE, compositeID - 61 * 20000000, null);
                case 52:
                    return new PSArc(powerSHAPE, compositeID - 52 * 20000000, null);
                case 59:
                    return new PSCompCurve(powerSHAPE, compositeID - 59 * 20000000, null);
                case 53:
                    return new PSCurve(powerSHAPE, compositeID - 53 * 20000000, null);
                case 58:
                    return new PSLine(powerSHAPE, compositeID - 58 * 20000000, null);
                case 49:
                    return new PSPoint(powerSHAPE, compositeID - 49 * 20000000, null);
                case 40:
                    return PSSolidFactory.CreateSolid(powerSHAPE, compositeID - 40 * 20000000, null);
                case 60:
                    if (powerSHAPE.ReadBoolValue("POINT[ID " + (compositeID - 60 * 20000000) + "].EXISTS"))
                    {
                        return new PSPoint(powerSHAPE, compositeID - 60 * 20000000, null);
                    }
                    else if (powerSHAPE.ReadBoolValue("ELECTRODE[ID " + (compositeID - 60 * 20000000) + "].EXISTS"))
                    {
                        return new PSElectrode(powerSHAPE, compositeID - 60 * 20000000, null);
                    }
                    else
                    {
                        return new PSMesh(powerSHAPE, compositeID - 60 * 20000000, null);
                    }
                    break;
                case 21:
                    return PSSurfaceFactory.CreateSurface(powerSHAPE, compositeID - 21 * 20000000, null);
                case 54:
                    return new PSWorkplane(powerSHAPE, compositeID - 54 * 20000000, null);
            }

            return null;
        }
    }
}