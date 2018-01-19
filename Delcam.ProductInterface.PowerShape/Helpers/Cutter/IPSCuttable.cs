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
    /// Allows an entity to be divided into two entities.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSCuttable
    {
        /// <summary>
        /// Cuts an entity.
        /// </summary>
        /// <param name="pointAtWhichToCut">The point used to cut the entity.</param>
        /// <returns>The new created entity.</returns>
        /// <remarks></remarks>
        PSEntity Cut(Geometry.Point pointAtWhichToCut);
    }
}