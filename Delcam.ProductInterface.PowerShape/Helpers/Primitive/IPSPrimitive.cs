// **********************************************************************
// *         © COPYRIGHT 2024 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Security.Cryptography;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Allows an entity to be accessed a Primitive
    /// </summary>
    /// <remarks></remarks>
    public interface IPSPrimitive
    {
        /// <summary>
        /// Gets the origin of the primitive
        /// </summary>
        Geometry.Point Origin { get; }

        /// <summary>
        /// Gets the X axis of the primitive
        /// </summary>
        Geometry.Vector XAxis { get; }

        /// <summary>
        /// Gets the Y axis of the primitive
        /// </summary>
        Geometry.Vector YAxis { get; }

        /// <summary>
        /// Gets the Z axis of the primitive
        /// </summary>
        Geometry.Vector ZAxis { get; }

    }
}