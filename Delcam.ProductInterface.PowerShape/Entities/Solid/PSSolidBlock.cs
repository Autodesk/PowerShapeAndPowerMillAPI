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
    /// Captures a solid block in PowerSHAPE
    /// </summary>
    public class PSSolidBlock : PSSolid
    {
        #region " Fields "

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and Sets the height of the Block
        /// </summary>
        public MM Height
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].HEIGHT"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the height adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("HEIGHT " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the length of the Block
        /// </summary>
        public MM Length
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the length adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("LENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets the width of the Block
        /// </summary>
        public MM Width
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].WIDTH"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the width adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("WIDTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets draft angle 1 of the Block
        /// </summary>
        public Degree DraftAngle1
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].ANGLE1"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the draft angle adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("ANGLE1 " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets draft angle 2 of the Block
        /// </summary>
        public Degree DraftAngle2
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].ANGLE2"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the draft angle adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("ANGLE2 " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets draft angle 3 of the Block
        /// </summary>
        public Degree DraftAngle3
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].ANGLE3"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the draft angle adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("ANGLE3 " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and Sets draft angle 4 of the Block
        /// </summary>
        public Degree DraftAngle4
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].ANGLE4"); }
            set
            {
                // Add the block to the selection
                AddToSelection(true);

                // Make the draft angle adjustment
                _powerSHAPE.DoCommand("MODIFY");
                _powerSHAPE.DoCommand("ANGLE4 " + value, "ACCEPT");
            }
        }

        #endregion

        #region " Constructors "

        internal PSSolidBlock(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        internal PSSolidBlock(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        internal PSSolidBlock(PSAutomation powershape, Point origin) : base(powershape)
        {
            // Clear CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            // Create a plane at the point specified
            _powerSHAPE.DoCommand("CREATE SOLID BLOCK");
            _powerSHAPE.DoCommand(origin.ToString());

            // Get created plane id
            PSEntity newBlock = _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = newBlock.Id;
        }

        #endregion
    }
}