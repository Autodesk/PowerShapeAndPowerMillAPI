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
    /// Captures a Surface Spring object in PowerSHAPE
    /// </summary>
    public class PSSurfaceSpring : PSPrimitive
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the top radius of the spring
        /// </summary>
        public MM TopRadius
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].TOP_RADIUS"); }

            set
            {
                // Add the spring to the selection
                AddToSelection();

                // Change the top radius
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("TOP_RADIUS " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the base radius of the spring
        /// </summary>
        public MM BaseRadius
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].BASE_RADIUS"); }

            set
            {
                // Add the spring to the selection
                AddToSelection();

                // Change the base radius
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("BASE_RADIUS " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the height of the spring
        /// </summary>
        public MM Height
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].HEIGHT"); }
            set
            {
                // Add the speing to the selection
                AddToSelection();

                // Make the height adjustment
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("HEIGHT " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the pitch of the spring
        /// </summary>
        public double Pitch
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].PITCH"); }
            set
            {
                // Add the spring to the selection
                AddToSelection();

                // Make the height adjustment
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("PITCH " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the turns of the spring
        /// </summary>
        public double Turns
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].TURNS"); }
            set
            {
                // Add the spring to the selection
                AddToSelection();

                // Make the turns adjustment
                _powerSHAPE.DoCommand("SURFEDITS");
                _powerSHAPE.DoCommand("TURNS " + value, "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSurfaceSpring(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSurfaceSpring(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSurfaceSpring(PSAutomation powershape, Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SURFACE SPRING");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSSurfaceSpring newSpring = (PSSurfaceSpring) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newSpring.Id;
        }

        #endregion
    }
}