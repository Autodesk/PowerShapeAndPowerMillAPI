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
    /// Captures a solid torus in PowerSHAPE
    /// </summary>
    public class PSSolidTorus : PSSolidPrimitive
    {
        #region " Fields "

        #endregion

        #region " Properties "

        #endregion

        #region " Constructors "

        internal PSSolidTorus(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidTorus(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidTorus(PSAutomation powershape, Geometry.Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SOLID TORUS");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSolidTorus newTorus = (PSSolidTorus) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newTorus.Id;
        }

        #endregion
    }
}