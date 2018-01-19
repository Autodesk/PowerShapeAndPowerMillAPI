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
    /// Captures a collection of Longitudinals in a project
    /// </summary>
    /// <remarks></remarks>
    public class PSLongitudinalsCollection : PSSurfaceCurvesCollection<PSLongitudinal>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSLongitudinalsCollection(PSAutomation powerSHAPE, PSSurface parentSurface) : base(powerSHAPE, parentSurface)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Composite Curve
        /// </summary>
        internal override string Identifier
        {
            get { return "SURFACE['" + _parentSurface.Id + "'].LONGITUDINAL"; }
        }

        #endregion

        #region " Operations "

        #endregion
    }
}