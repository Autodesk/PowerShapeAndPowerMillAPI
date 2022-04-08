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
    /// Captures a solid plane in PowerSHAPE
    /// </summary>
    public class PSSolidPlane : PSSolid
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the length of the Plane
        /// </summary>
        public MM Length
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH");
            }
            set
            {
                // Add the block to the selection
                AddToSelection();

                // Make the length adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("LENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the width of the Plane
        /// </summary>
        public MM Width
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].WIDTH");
            }
            set
            {
                // Add the block to the selection
                AddToSelection();

                // Make the width adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("WIDTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSolidPlane(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidPlane(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidPlane(PSAutomation powershape, Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SOLID PLANE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSolidPlane newPlane = (PSSolidPlane) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newPlane.Id;
        }

        #endregion
    }
}