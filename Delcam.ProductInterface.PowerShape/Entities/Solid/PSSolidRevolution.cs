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
    /// Captures a solid revolution in PowerSHAPE
    /// </summary>
    public class PSSolidRevolution : PSSolid
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the angle of the revolution
        /// </summary>
        public Degree Angle
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].ANGLE"); }
            set
            {
                // Add the revolution to the selection
                AddToSelection();

                // Make the angle adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("ANGLE " + value, "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSolidRevolution(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidRevolution(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidRevolution(
            PSAutomation powershape,
            Planes principalPlane,
            PSWireframe[] wireframeToExtrude) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Add the wireframe to the selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSWireframe wireframe in wireframeToExtrude)
            {
                wireframe.AddToSelection(false);
            }

            // Create a revolution using the principal plane and wireframe specified
            _powerSHAPE.SetActivePlane(principalPlane);
            _powerSHAPE.DoCommand("CREATE SOLID REVOLUTION");

            // Get created revolution id
            PSSolidRevolution newRevolution = (PSSolidRevolution) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newRevolution.Id;
        }

        #endregion
    }
}