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
    /// Direction in which to define the incremental move.
    /// </summary>
    public enum PointApproach
    {
        /// <summary>
        /// Move along tool axis.
        /// </summary>
        ToolAxis,

        /// <summary>
        /// Move along contact normal.
        /// </summary>
        Normal,

        /// <summary>
        /// Move along contact normal.
        /// </summary>
        Tangent,

        /// <summary>
        /// Move along contact normal.
        /// </summary>
        Radial
    }
}