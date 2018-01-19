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
    /// The orientation of the links.
    /// </summary>
    public enum MoveDirection
    {
        /// <summary>
        /// The retract and approach moves have the same orientation as the tool axis.
        /// </summary>
        ToolAxis,

        /// <summary>
        /// The retract and approach moves are normal to the contact point (or normal to the Leads In/Out). This can be particularly useful for tipped disc cutters where moving along the tool axis may not be possible (for example, when undercutting).
        /// </summary>
        Normal,

        /// <summary>
        /// The retract and approach moves are tangential to the toolpath.
        /// </summary>
        Tangent,

        /// <summary>
        /// The retract and approach moves are perpendicular to both the tool axis and the tangent direction of the toolpath.
        /// </summary>
        Radial
    }
}