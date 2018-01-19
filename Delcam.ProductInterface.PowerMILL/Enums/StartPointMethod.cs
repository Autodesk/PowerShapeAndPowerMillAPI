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
    /// Defines how the start point is set.
    /// </summary>
    public enum StartPointMethod
    {
        /// <summary>
        /// The start point is positioned independently of all other properties at the point defined by the Coordinates field.
        /// </summary>
        Absolute,

        /// <summary>
        /// The start point is at Safe Z above the block centre:
        /// </summary>
        BlockCentre,

        /// <summary>
        /// The start point is positioned at the specified distance from the first point in the toolpath, measured along its tool axis.
        /// </summary>
        FirstPoint,

        /// <summary>
        /// The start point is at Safe Z above the first point in the toolpath. For multi-axis toolpaths,
        /// the start point is set by considering a point that is the specified distance from the first point on the toolpath,
        /// measured along the tool axis, and projecting this onto the Safe Z area.
        /// </summary>
        FirstPointSafe
    }
}