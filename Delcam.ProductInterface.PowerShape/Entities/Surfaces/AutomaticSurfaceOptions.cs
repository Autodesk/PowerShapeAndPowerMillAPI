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
    /// Edge matching options to control how PowerSHAPE adds extra surface points and curves to the initial network so that each point lies at the junction of a lateral and a longitudinal.
    /// </summary>
    /// <remarks></remarks>
    public enum AutomaticSurfaceOptions
    {
        None,
        TangentDirection,
        WidthOfCurve,
        ArcLength,
        Repoint,
        BSpline
    }
}