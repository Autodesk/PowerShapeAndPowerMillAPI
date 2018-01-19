// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Allows entities rotation.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSRotateable
    {
        /// <summary>
        /// Rotates an entity arc by a specified angle around a specified axis.
        /// </summary>
        /// <param name="rotationAxis">The axis around which the arc is are to be rotated.</param>
        /// <param name="rotationAngle">The angle by which the arc is to be rotated.</param>
        /// <param name="copiesToCreate">The number of copies to create of the original arc.</param>
        /// <param name="rotationOrigin">The origin of the rotation axis.</param>
        /// <returns>A list of entities created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        /// <remarks></remarks>
        List<PSEntity> Rotate(
            Axes rotationAxis,
            Geometry.Degree rotationAngle,
            int copiesToCreate,
            Geometry.Point rotationOrigin = null);
    }
}