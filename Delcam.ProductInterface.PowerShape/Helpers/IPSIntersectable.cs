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
    /// Allows intersectable behaviour between entities.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSIntersectable
    {
        /// <summary>
        /// Intersects an entity with another entity.
        /// </summary>
        /// <param name="entityToIntersect">The entity to intersect.</param>
        /// <param name="copeWithCoincidentFaces">Cope with coincident faces value.</param>
        /// <returns>The entity created from the intersection.</returns>
        /// <remarks></remarks>
        PSEntity IntersectEntityWithEntity(IPSIntersectable entityToIntersect, bool copeWithCoincidentFaces = false);

        /// <summary>
        /// Intersects a collection of entities with another entity.
        /// </summary>
        /// <param name="entitiesToIntersect">The entity to intersect.</param>
        /// <param name="copeWithCoincidentFaces">Cope with coincident faces value.</param>
        /// <returns>The entity created from the intersection.</returns>
        /// <remarks></remarks>
        List<PSEntity> IntersectEntitiesWithEntity(
            IEnumerable<IPSIntersectable> entitiesToIntersect,
            bool copeWithCoincidentFaces = false);
    }
}