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
    /// Captures a Surface Plane object in PowerSHAPE
    /// </summary>
    public class PSSurfacePlane : PSPrimitive
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
                _powerSHAPE.DoCommand("SURFEDITS");
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
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("WIDTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets the volume of the Plane, which is 0
        /// </summary>
        public override double Volume
        {
            get { return 0; }
        }

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor gets a plane already created in PowerSHAPE
        /// </summary>
        internal PSSurfacePlane(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        /// <summary>
        /// This constructor gets a plane already created in PowerSHAPE
        /// </summary>
        internal PSSurfacePlane(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// This constructor creates a plane at a specified origin
        /// </summary>
        internal PSSurfacePlane(PSAutomation powershape, Point origin, Planes principalPlane) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Set the principal plane
            _powerSHAPE.SetActivePlane(principalPlane);

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SURFACE PLANE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSurfacePlane newPlane = (PSSurfacePlane) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newPlane.Id;
        }

        /// <summary>
        /// This constructor creates a plane at a specified origin and sets is length and width properties
        /// </summary>
        internal PSSurfacePlane(PSAutomation powershape, Point origin, Planes principalPlane, MM length, MM width) : this(
            powershape,
            origin,
            principalPlane)
        {
            // Alter attributes
            Length = length;
            Width = width;
        }

        #endregion
    }
}