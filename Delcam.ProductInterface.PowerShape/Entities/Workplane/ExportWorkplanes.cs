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
    /// Used to select the workplane when exporting from PowerSHAPE.
    /// </summary>
    public enum ExportWorkplanes
    {
        /// <summary>
        /// Allows the user to export using the World.
        /// </summary>
        World,

        /// <summary>
        /// Allows the user to export using the active workplane.
        /// </summary>
        Active
    }
}