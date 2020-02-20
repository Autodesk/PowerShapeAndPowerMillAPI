// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Linq;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a collection of laterals in a project
    /// </summary>
    /// <remarks></remarks>
    public class PSLateralsCollection : PSSurfaceCurvesCollection<PSLateral>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSLateralsCollection(PSAutomation powerSHAPE, PSSurface parentSurface) : base(powerSHAPE, parentSurface)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Composite Curve
        /// </summary>
        internal override string Identifier
        {
            get { return "SURFACE['" + _parentSurface.Id + "'].Lateral"; }
        }

        #endregion

        #region " Operations "

        public override void AddToSelection(bool emptySelectionFirst = false)
        {
            if (emptySelectionFirst)
            {
                _powerSHAPE.ActiveModel.ClearSelectedItems();
            }

            _parentSurface.SelectSurfaceCurves(SurfaceCurveTypes.Lateral,
                                               _parentSurface.Longitudinals.Select(x => int.Parse(x.Name)).ToArray<int>());
        }

        #endregion
    }
}