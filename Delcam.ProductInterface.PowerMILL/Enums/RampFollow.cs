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
    /// Direction of <see cref="PMRamp"/>.
    /// </summary>
    public enum RampFollow
    {
        /// <summary>
        /// The ramp moves follow the profile of the toolpath.
        /// </summary>
        ToolPath = 0,

        /// <summary>
        /// The ramp moves are normal to the cutting direction at that point.
        /// If the requested line cannot be fitted into the area, then the Toolpath option is used automatically.
        /// </summary>
        Line = 1,

        /// <summary>
        /// The ramp moves are circular. If the requested circle cannot be fitted into the area, then the Line method is used automatically.
        /// </summary>
        Circle = 2
    }
}