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
    /// Captures a Surface Torus object in PowerSHAPE
    /// </summary>
    public class PSSurfaceTorus : PSPrimitive
    {
        #region " Fields "

        #endregion

        #region " Properties "

        #endregion

        #region " Constructors "

        internal PSSurfaceTorus(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSurfaceTorus(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSurfaceTorus(PSAutomation powershape, Geometry.Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SURFACE TORUS");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSurfaceTorus newTorus = (PSSurfaceTorus) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newTorus.Id;
        }

        #endregion
    }
}