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
    /// Represents the coolant options in PowerMill.
    /// PowerMILL turns the coolant off at the end of a toolpath.
    /// </summary>
    public enum CoolantOptions
    {
        /// <summary>
        /// There is no coolant output.
        /// </summary>
        None,

        /// <summary>
        /// Turns the coolant on.
        /// </summary>
        Standard,

        /// <summary>
        /// Turns the coolant to flood.
        /// </summary>
        Flood,

        /// <summary>
        /// Turns the coolant to mist.
        /// </summary>
        Mist,

        /// <summary>
        /// Turns the coolant to tap.
        /// </summary>
        Tap,

        /// <summary>
        /// Turns the coolant to air blast.
        /// </summary>
        Air,

        /// <summary>
        /// Turns the coolant to through spindle.
        /// </summary>
        Through,

        /// <summary>
        /// Enables two coolant codes.
        /// </summary>
        Double
    }
}