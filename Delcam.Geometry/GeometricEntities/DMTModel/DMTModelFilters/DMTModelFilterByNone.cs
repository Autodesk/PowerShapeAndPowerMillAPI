// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.Geometry
{
    /// <summary>
    /// Allows to read a DMTModel without filtering it.
    /// </summary>
    internal class DMTModelFilterByNone : IDMTModelFilter
    {
        /// <summary>
        /// Checks if a triangle can be added to the DMTModel.
        /// </summary>
        /// <param name="vertex1">The 1st triangle vertex.</param>
        /// <param name="vertex2">The 2nd triangle vertex.</param>
        /// <param name="vertex3">The 3rd triangle vertex.</param>
        /// <returns>Returns always true.</returns>
        public bool CanAddTriangle(Point vertex1, Point vertex2, Point vertex3)
        {
            // This class won't filter the DMTModel, so always true
            return true;
        }
    }
}