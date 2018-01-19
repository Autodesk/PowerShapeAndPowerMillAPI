// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Specifies the location of the tool at the end of the toolpath.
    /// </summary>
    public enum EndPointMethod
    {
        /// <summary>
        /// Use an absolute position.
        /// </summary>
        Absolute,

        /// <summary>
        /// The end point is at Safe Z above the block centre:
        /// </summary>
        BlockCentre,

        /// <summary>
        /// Position end point at specified distance above the end of the last segment or leadout in the toolpath.
        /// </summary>
        LastPoint,

        /// <summary>
        /// Position end point on the safe area above the end of the last segment or leadout in the toolpath.
        /// </summary>
        LastPointSafe
    }
}