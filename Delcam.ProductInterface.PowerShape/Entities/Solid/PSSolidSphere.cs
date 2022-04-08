// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a solid sphere in PowerSHAPE
    /// </summary>
    public class PSSolidSphere : PSSolid
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the radius of the sphere in MM
        /// </summary>
        public MM Radius
        {
            get { return _powerSHAPE.ReadDoubleValue("SOLID['" + Name + "'].RADIUS"); }

            set
            {
                // Add to selection
                AddToSelection(true);

                _powerSHAPE.DoCommand("MODIFY", "CHANGE_DIMENSION RADIUS", "DIMENSION " + value.ToString("0.######"), "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSolidSphere(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidSphere(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidSphere(PSAutomation powershape, Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SOLID SPHERE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSolidSphere newSphere = (PSSolidSphere) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newSphere.Id;
        }

        #endregion
    }
}