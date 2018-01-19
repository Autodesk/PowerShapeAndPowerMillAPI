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
    /// Allows to add an entity to another entity.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSAddable
    {
        /// <summary>
        /// Add two entities together.
        /// </summary>
        /// <param name="entityToAdd">Entity to be added.</param>
        /// <param name="copeWithCoincidentFaces">If it should cope with coincident faces.</param>
        /// <returns>Entity created from the addition.</returns>
        /// <remarks></remarks>
        PSEntity AddEntityToEntity(IPSAddable entityToAdd, bool copeWithCoincidentFaces = false);

        /// <summary>
        /// Add a group of entities to another entity.
        /// </summary>
        /// <param name="entitiesToAdd">Entities to be added.</param>
        /// <param name="copeWithCoincidentFaces">If it should cope with coincident faces.</param>
        /// <returns>Entity created from the addition.</returns>
        /// <remarks></remarks>
        PSEntity AddEntitiesToEntity(IEnumerable<IPSAddable> entitiesToAdd, bool copeWithCoincidentFaces = false);
    }
}