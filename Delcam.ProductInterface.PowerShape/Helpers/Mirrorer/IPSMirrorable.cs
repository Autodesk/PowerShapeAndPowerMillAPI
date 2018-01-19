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
    /// Allows an entity to be mirrored.
    /// </summary>
    /// <remarks></remarks>
    public interface IPSMirrorable
    {
        /// <summary>
        /// Mirrors an entity in a specified plane.
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror.</param>
        /// <param name="mirrorPoint">The origin of the mirror plane.</param>
        /// <remarks></remarks>
        void Mirror(Planes mirrorPlane, Geometry.Point mirrorPoint);
    }
}