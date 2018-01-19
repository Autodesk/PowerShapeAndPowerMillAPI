// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Represents a window in PowerShape.
    /// </summary>
    /// <remarks></remarks>
    public class PSWindow : PSDatabaseEntity
    {
        #region " Fields "

        /// <summary>
        /// The number of the window in the PowerSHAPE instance.  As others are deleted, this changes.
        /// </summary>
        private int _number;

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor creates a Window with the specified number.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="number">The window number.</param>
        /// <remarks></remarks>
        internal PSWindow(PSAutomation powerSHAPE, int number) : base(powerSHAPE)
        {
            // Set the window number, which is synonymous with the window name
            _number = number;
            _name = _number.ToString();
            _id = _powerSHAPE.ReadIntValue(Identifier + "['" + Name + "'].ID");

            // Reset other window IDs because they may have been changed
            if (_powerSHAPE.Windows.Count != 0)
            {
                foreach (PSWindow existingWindow in _powerSHAPE.Windows)
                {
                    existingWindow.Id = _powerSHAPE.ReadIntValue(Identifier + "[" + existingWindow.Name + "].ID");
                }
            }
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify a window.
        /// </summary>
        internal override string Identifier
        {
            get { return "WINDOW"; }
        }

        /// <summary>
        /// As the name command "is not implemented for this type of entity", this property
        /// gets and sets the number of the window.
        /// </summary>
        public override string Name
        {
            get { return _name; }
            set { throw new NotImplementedException("Cannot set name of window in PowerSHAPE"); }
        }

        /// <summary>
        /// Gets the type of the window.
        /// </summary>
        /// <returns>The window type - drawing or model.</returns>
        public WindowTypes Type
        {
            get
            {
                string psWindowType = _powerSHAPE.ReadStringValue(Identifier + "[" + _number + "].TYPE");
                return (WindowTypes) Enum.Parse(typeof(WindowTypes), psWindowType, true);
            }
        }

        /// <summary>
        /// Gets the name of the loaded model or drawing.
        /// </summary>
        public string AttachedProcessName
        {
            get
            {
                if (Type == WindowTypes.MODEL)
                {
                    return _powerSHAPE.ReadStringValue(Identifier + "[" + _number + "].MODEL");
                }
                return _powerSHAPE.ReadStringValue(Identifier + "[" + _number + "].DRAWING");
            }
        }

        /// <summary>
        /// Gets whether the Window is currently active.
        /// </summary>
        public bool IsActive
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "['" + Name + "'].SELECTED") == 1; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Brings the current window to the front.
        /// </summary>
        public void MakeActive()
        {
            // Bring window to the front
            _powerSHAPE.DoCommand("Sel_Window @" + _number + " Front");
        }

        /// <summary>
        /// Checks whether the Window exists.
        /// </summary>
        public override bool Exists
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[" + _number + "].EXISTS") == 1; }
        }

        #endregion
    }
}