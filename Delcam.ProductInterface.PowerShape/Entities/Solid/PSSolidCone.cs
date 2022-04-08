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
    /// Captures a solid cone in PowerSHAPE
    /// </summary>
    public class PSSolidCone : PSSolid
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the length of the cone
        /// </summary>
        public MM Length
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH"); }
            set
            {
                // Add the cone to the selection
                AddToSelection();

                // Make the length adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("LENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the top radius of the cone
        /// </summary>
        public MM TopRadius
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].TOP_RADIUS"); }

            set
            {
                // Add the cone to the selection
                AddToSelection();

                // Change the top radius
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("TOP_RADIUS " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the base radius of the cone
        /// </summary>
        public MM BaseRadius
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].BASE_RADIUS"); }

            set
            {
                // Add the cone to the selection
                AddToSelection();

                // Change the base radius
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("BASE_RADIUS " + value.ToString("0.######"), "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSolidCone(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidCone(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidCone(PSAutomation powershape, Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SOLID CONE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSolidCone newCone = (PSSolidCone) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newCone.Id;
        }

        #endregion
    }
}