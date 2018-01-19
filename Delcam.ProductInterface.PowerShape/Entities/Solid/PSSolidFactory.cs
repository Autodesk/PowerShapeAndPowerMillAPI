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
    /// Creates instance solids based on a string identifying a solid's type in PowerSHAPE
    /// </summary>
    internal abstract class PSSolidFactory
    {
        /// <summary>
        /// Creates instance solids based on a string identifying a solid's type in PowerSHAPE
        /// </summary>
        /// <param name="powerSHAPE">PowerSHAPE instance</param>
        /// <param name="id">ID of the new solid</param>
        /// <param name="name">Name of the new solid</param>
        /// <returns>Solid with type requested</returns>
        /// <remarks></remarks>
        public static object CreateSolid(PSAutomation powerSHAPE, int id, string name)
        {
            // Get solid attributes
            //Dim id As String = baseSolid.Id
            //Dim name As String = baseSolid.Name

            switch (PSSolid.GetSolidType(powerSHAPE, id))
            {
                case SolidTypes.Block:
                    return new PSSolidBlock(powerSHAPE, id, name);
                case SolidTypes.Cone:
                    return new PSSolidCone(powerSHAPE, id, name);
                case SolidTypes.Cylinder:
                    return new PSSolidCylinder(powerSHAPE, id, name);
                case SolidTypes.Extrusion:
                    return new PSSolidExtrusion(powerSHAPE, id, name);
                case SolidTypes.Plane:
                    return new PSSolidPlane(powerSHAPE, id, name);
                case SolidTypes.Revolution:
                    return new PSSolidRevolution(powerSHAPE, id, name);
                case SolidTypes.Sphere:
                    return new PSSolidSphere(powerSHAPE, id, name);
                case SolidTypes.Spring:
                    return new PSSolidSpring(powerSHAPE, id, name);
                case SolidTypes.Torus:
                    return new PSSolidTorus(powerSHAPE, id, name);
            }

            // Solid type is not known, so return solid that was passed in
            return new PSSolid(powerSHAPE, id, name);
        }

        /// <summary>
        /// Creates instance solids based on a string identifying a solid's type in PowerSHAPE
        /// </summary>
        /// <param name="powerSHAPE">PowerSHAPE instance</param>
        /// <param name="id">ID of the new solid</param>
        /// <param name="name">Name of the new solid</param>
        /// <returns>Solid with type requested</returns>
        /// <remarks></remarks>
        public static object CreateSolid(PSAutomation powerSHAPE, int id, string name, string description)
        {
            // Get solid attributes
            //Dim id As String = baseSolid.Id
            //Dim name As String = baseSolid.Name

            switch (description)
            {
                case "Solid " + nameof(SolidTypes.Block):
                    return new PSSolidBlock(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Cone):
                    return new PSSolidCone(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Cylinder):
                    return new PSSolidCylinder(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Extrusion):
                    return new PSSolidExtrusion(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Plane):
                    return new PSSolidPlane(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Revolution):
                    return new PSSolidRevolution(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Sphere):
                    return new PSSolidSphere(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Spring):
                    return new PSSolidSpring(powerSHAPE, id, name);
                case "Solid " + nameof(SolidTypes.Torus):
                    return new PSSolidTorus(powerSHAPE, id, name);
            }

            // Solid type is not known, so return solid that was passed in
            return new PSSolid(powerSHAPE, id, name);
        }
    }
}