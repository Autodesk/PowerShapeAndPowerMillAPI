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
    /// Captures a solid extrusion in PowerSHAPE
    /// </summary>
    public class PSSolidExtrusion : PSSolid
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the length of the extrusion
        /// </summary>
        public MM Length
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH"); }
            set
            {
                // Add the extrusion to the selection
                AddToSelection();

                // Make the length adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("LENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the negative length of the extrusion
        /// </summary>
        public MM NegativeLength
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].NEGLENGTH"); }
            set
            {
                // Add the extrusion to the selection
                AddToSelection();

                // Make the length adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("NEGLENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the draft angle of the extrusion
        /// </summary>
        public Degree DraftAngle
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].ANGLE"); }
            set
            {
                // Add the extrusion to the selection
                AddToSelection();

                // Make the draft angle adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("ANGLE " + value, "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSolidExtrusion(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidExtrusion(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidExtrusion(
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

            // Create an extrusion using the principal plane and wireframe specified
            _powerSHAPE.SetActivePlane(principalPlane);
            _powerSHAPE.DoCommand("CREATE SOLID EXTRUSION", "ACCEPT");

            // Get created plane id
            PSSolidExtrusion newExtrusion = (PSSolidExtrusion) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newExtrusion.Id;
        }

        #endregion
    }
}