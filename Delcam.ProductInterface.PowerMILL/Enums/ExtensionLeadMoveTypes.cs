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
    /// a <see cref="PMLead"/>'s move type to be used before each cutting move
    /// </summary>
    public enum ExtensionLeadMoveTypes
    {
        /// <summary>
        /// There are no leads in or out applied to the toolpath. This is the default option.
        /// </summary>
        None,

        /// <summary>
        /// This inserts a straight move at the end of each cutting move. The direction of the move is determined by the Angle.
        /// </summary>
        Straight,

        /// <summary>
        /// This inserts a tangential arc move at the end of each cutting path.
        /// This arc lies in the plane defined by the toolpath's tangent direction and the surface normal.
        /// </summary>
        SurfaceNormalArc,

        /// <summary>
        /// This inserts a tangential arc move at the end of each cutting path. This arc lies in a plane containing the tangent direction and the tool axis.
        /// </summary>
        VerticalArc,

        /// <summary>
        /// This inserts a tangential arc move at the end of each cutting path.
        /// This arc lies in the plane containing the tool axis and the tangent direction,
        /// rotated by 90DegreesSymbol about the tangent direction.
        /// </summary>
        HorizontalArc,

        /// <summary>
        /// </summary>
        HorizontalArcLeft,

        /// <summary>
        /// </summary>
        HorizontalArcRight,

        /// <summary>
        /// This inserts a straight tangential move at the end of each cutting move.
        /// </summary>
        ExtendedMove,

        /// <summary>
        /// This inserts a horizontal move at the end of each cutting move.
        /// </summary>
        Boxed
    }
}