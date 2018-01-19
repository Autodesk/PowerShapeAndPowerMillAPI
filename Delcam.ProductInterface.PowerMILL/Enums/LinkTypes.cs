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
    /// The type of link moves to connect consecutive cutting passes for links shorter than the Short/Long threshold value.
    /// </summary>
    public enum LinkTypes
    {
        /// <summary>
        /// The tool moves to the safe area before making the link move.
        /// </summary>
        Safe,

        /// <summary>
        /// The tool descends at rapid feed from the safe area to the specified incremental distance above the target point,
        /// and then plunges the remaining distance. If you have a multi-axis toolpath, the tool retracts a set distance along
        /// the tool axis and then retracts to the safe area.
        /// It comes down from the safe area and completes the approach along the tool axis.
        /// The distance moved along the tool axis is controlled by the Plunge Distance on the Z Heights
        /// </summary>
        Incremental,

        /// <summary>
        /// The links traverse clear of the part by the specified incremental distance,
        /// descend at rapid feed to the specified incremental distance above the contact point,
        /// and then plunge the remaining distance
        /// </summary>
        Skim,

        /// <summary>
        /// The links follow the surface of the triangulated model, and so prevent gouging.
        /// </summary>
        OnSurface,

        /// <summary>
        /// The links traverse clear of the part by an amount,
        /// so that with the specified <see cref="Stepdown"/> the vertical descent will reach the next contact point
        /// </summary>
        Stepdown,

        /// <summary>
        /// Straight-line moves are used.
        /// These straight moves between raster paths could cause gouging over curved areas of the model.
        /// </summary>
        Straight,

        /// <summary>
        /// The tool moves are circular.
        /// </summary>
        CircularArc
    }
}