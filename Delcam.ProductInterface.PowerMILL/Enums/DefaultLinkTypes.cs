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
    /// Type of link moves
    /// </summary>
    public enum DefaultLinkTypes
    {
        /// <summary>
        /// The tool descends at rapid feed from the safe area to the specified incremental distance above the target point,
        /// and then plunges the remaining distance. If you have a multi-axis toolpath,
        /// the tool retracts a set distance along the tool axis and then retracts to the safe area.
        /// It comes down from the safe area and completes the approach along the tool axis
        /// </summary>
        Incremental,

        /// <summary>
        /// The tool moves to the safe area before making the link move.
        /// </summary>
        Safe,

        /// <summary>
        /// The links traverse clear of the part by the specified incremental distance,
        /// descend at rapid feed to the specified incremental distance above the contact point,
        /// and then plunge the remaining distance.
        /// </summary>
        Skim
    }
}