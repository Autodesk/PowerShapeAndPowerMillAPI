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
    /// Allows the offset of an entity.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSOffsetable
    {
        /// <summary>
        /// Offsets a single entity by a specified distance.
        /// </summary>
        /// <param name="offsetDistance">Distance by which to offset the curve.</param>
        /// <param name="copiesToCreate">Number of copies to be created from the operation.</param>
        /// <returns>A list of the copied entities that have been created.  If numberOfCopies was 0, an empty list is returned.</returns>
        /// <remarks></remarks>
        List<PSEntity> Offset(Geometry.MM offsetDistance, int copiesToCreate);
    }
}