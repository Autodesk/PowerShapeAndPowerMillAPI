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
    /// Select how to determine the height from which the ramp descends
    /// </summary>
    public enum RampHeight
    {
        /// <summary>
        /// The height above the cleared stock for the start of the ramp.
        /// </summary>
        IncrementalRamp,

        /// <summary>
        /// The start of the ramp is at the same height as the end of the previous toolpath segment,
        /// provided that this is higher than the toolpath segment by the amount specified in the Height field.
        /// </summary>
        Segment,

        /// <summary>
        /// The start of the ramp is at the same height as the end of the previous toolpath segment plus the height specified in the Height field.
        /// </summary>
        SegmentIncremental
    }
}