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
    /// Indicates the safety of various parts of the toolpath.
    /// </summary>
    public enum ToolpathSafety
    {
        /// <summary>
        /// Safty unchecked
        /// </summary>
        NotChecked,

        /// <summary>
        /// Tool does not gouge .
        /// </summary>
        Safe,

        /// <summary>
        /// Tool gouges
        /// </summary>
        Collides
    }
}