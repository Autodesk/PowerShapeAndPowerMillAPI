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
    /// Creates instance surfaces based on a string identifying a surface's type in PowerSHAPE
    /// </summary>
    internal abstract class PSSurfaceFactory
    {
        /// <summary>
        /// Creates instance surfaces based on a string identifying a surface's type in PowerSHAPE
        /// </summary>
        /// <param name="powerSHAPE">PowerSHAPE instance to create the surface in</param>
        /// <param name="id">ID for the new surface</param>
        /// <param name="name">Name for the new surface</param>
        /// <returns>New surface created by the operation</returns>
        public static object CreateSurface(PSAutomation powerSHAPE, int id, string name)
        {
            // Get surface attributes
            //Dim id As String = baseSurface.Id
            //Dim name As String = baseSurface.Name

            switch (PSSurface.GetSurfaceType(powerSHAPE, id))
            {
                case SurfaceTypes.Block:
                    return new PSSurfaceBlock(powerSHAPE, id, name);
                case SurfaceTypes.Cone:
                    return new PSSurfaceCone(powerSHAPE, id, name);
                case SurfaceTypes.Cylinder:
                    return new PSSurfaceCylinder(powerSHAPE, id, name);
                case SurfaceTypes.Extrusion:
                    return new PSSurfaceExtrusion(powerSHAPE, id, name);
                case SurfaceTypes.Plane:
                    return new PSSurfacePlane(powerSHAPE, id, name);
                case SurfaceTypes.Revolution:
                    return new PSSurfaceRevolution(powerSHAPE, id, name);
                case SurfaceTypes.Sphere:
                    return new PSSurfaceSphere(powerSHAPE, id, name);
                case SurfaceTypes.Spring:
                    return new PSSurfaceSpring(powerSHAPE, id, name);
                case SurfaceTypes.Torus:
                    return new PSSurfaceTorus(powerSHAPE, id, name);
            }

            // Surface is not a primitive, so return surface that was passed in
            return new PSSurface(powerSHAPE, id, name);
        }

        /// <summary>
        /// Creates instance surfaces based on a string identifying a surface's type in PowerSHAPE
        /// </summary>
        /// <param name="powerSHAPE">PowerSHAPE instance to create the surface in</param>
        /// <param name="id">ID for the new surface</param>
        /// <param name="name">Name for the new surface</param>
        /// <returns>New surface created by the operation</returns>
        public static object CreateSurface(PSAutomation powerSHAPE, int id, string name, string description)
        {
            // Get surface attributes
            //Dim id As String = baseSurface.Id
            //Dim name As String = baseSurface.Name

            switch (description)
            {
                case "Surface " + nameof(SurfaceTypes.Block):
                    return new PSSurfaceBlock(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Cone):
                    return new PSSurfaceCone(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Cylinder):
                    return new PSSurfaceCylinder(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Extrusion):
                    return new PSSurfaceExtrusion(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Plane):
                    return new PSSurfacePlane(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Revolution):
                    return new PSSurfaceRevolution(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Sphere):
                    return new PSSurfaceSphere(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Spring):
                    return new PSSurfaceSpring(powerSHAPE, id, name);
                case "Surface " + nameof(SurfaceTypes.Torus):
                    return new PSSurfaceTorus(powerSHAPE, id, name);
            }

            // Surface is not a primitive, so return surface that was passed in
            return new PSSurface(powerSHAPE, id, name);
        }
    }
}