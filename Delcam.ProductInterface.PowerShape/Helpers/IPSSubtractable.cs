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
    /// Allows an entity to be subtracted.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSSubtractable
    {
        /// <summary>
        /// Subtracts an entity from another entity.
        /// </summary>
        /// <param name="entityToSubtract">Entity to subtract.</param>
        /// <param name="copeWithCoincidentFaces">Cope with coincident faces option.</param>
        /// <returns>Entity created from the subtraction.</returns>
        /// <remarks></remarks>
        PSEntity SubtractEntityFromEntity(IPSSubtractable entityToSubtract, bool copeWithCoincidentFaces = false);

        /// <summary>
        /// Subtracts a collection of entities from another entity.
        /// </summary>
        /// <param name="entitiesToSubtract">Entities to subtract.</param>
        /// <param name="copeWithCoincidentFaces">Cope with coincident faces option.</param>
        /// <returns>Entity created from the subtraction.</returns>
        /// <remarks></remarks>
        PSEntity SubtractEntitiesFromEntity(
            IEnumerable<IPSSubtractable> entitiesToSubtract,
            bool copeWithCoincidentFaces = false);
    }
}