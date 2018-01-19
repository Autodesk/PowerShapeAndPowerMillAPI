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
    /// Allows an entity to be moved.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSMoveable
    {
        /// <summary>
        /// Moves an entity by a specified relative amount.
        /// </summary>
        /// <param name="moveVector">Relative amount by which the entity will be moved.</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation.</param>
        /// <returns>A list of the copied entities that have been created.  If numberOfCopies was 0, an empty list is returned.</returns>
        /// <remarks></remarks>
        List<PSEntity> MoveByVector(Geometry.Vector moveVector, int copiesToCreate);

        /// <summary>
        /// Moves an entity by the relative distance between two absolute positions.
        /// </summary>
        /// <param name="moveOriginCoordinates">First of the two absolute positions.</param>
        /// <param name="pointToMoveTo">Second of the two absolute positions.</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation.</param>
        /// <returns>A list of the copied entities that have been created.  If numberOfCopies was 0, an empty list is returned.</returns>
        /// <remarks></remarks>
        List<PSEntity> MoveBetweenPoints(Geometry.Point moveOriginCoordinates, Geometry.Point pointToMoveTo, int copiesToCreate);
    }
}