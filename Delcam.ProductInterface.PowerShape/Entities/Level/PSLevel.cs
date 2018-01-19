// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Level in PowerSHAPE.
    /// </summary>
    public class PSLevel
    {
        #region " Fields "

        /// <summary>
        /// The PowerSHAPE Automation object.
        /// </summary>
        protected PSAutomation _powerSHAPE;

        /// <summary>
        /// The number of the level.
        /// </summary>
        protected int _number;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises a Level with the specified number identifier.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="number">The level number.</param>
        internal PSLevel(PSAutomation powerSHAPE, int number)
        {
            _powerSHAPE = powerSHAPE;
            _number = number;
        }

        /// <summary>
        /// Creates a new Level.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="number">The level number.</param>
        /// <param name="name">The level name.</param>
        internal PSLevel(PSAutomation powerSHAPE, int number, string name)
        {
            //Check that the number is valid
            if (number < 5 || number > 997)
            {
                throw new Exception("New Levels may only be numbered between 5 and 997");
            }

            _powerSHAPE = powerSHAPE;
            _number = number;

            //See if the level is already being used
            if (IsUsed)
            {
                throw new Exception("The specified level number is already in use");
            }

            //Create the level (set the name and activate it)
            Name = name;
            IsActive = true;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the number of the level.
        /// </summary>
        public int Number
        {
            get { return _number; }
        }

        /// <summary>
        /// Gets and sets the name of the level.  The format of the string should be of the format: [Group : ]Name.
        /// </summary>
        public string Name
        {
            get { return _powerSHAPE.ReadStringValue("LEVEL[" + _number + "].NAME"); }
            set { _powerSHAPE.DoCommand("LEVEL RENAME " + _number + " " + value); }
        }

        /// <summary>
        /// Gets whether the level is currently in use or not.
        /// </summary>
        public bool IsUsed
        {
            get { return _powerSHAPE.ReadIntValue("LEVEL[" + _number + "].USED") == 1; }
        }

        /// <summary>
        /// Gets and sets whether a level is active or not.
        /// </summary>
        public bool IsActive
        {
            get { return _powerSHAPE.ReadIntValue("LEVEL[" + _number + "].ACTIVE") == 1; }
            set
            {
                if (value)
                {
                    _powerSHAPE.DoCommand("TOOLBAR LEVEL ON " + _number);
                }
                else
                {
                    _powerSHAPE.DoCommand("TOOLBAR LEVEL OFF " + _number);
                }
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Overrides the equals function to compare Levels. Compares levels by number.
        /// </summary>
        /// <param name="obj">The other level to compare to.</param>
        /// <returns>True if numbers match.</returns>
        /// <remarks></remarks>
        public bool Equals(object obj)
        {
            if (obj is PSLevel)
            {
                return _number == ((PSLevel) obj).Number;
            }
            return false;
        }

        /// <summary>
        /// This operation checks to see if a Level exists.
        /// </summary>
        internal bool Exists()
        {
            if ((IsUsed == false) & string.IsNullOrEmpty(Name))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds all entities on this level to the selection.
        /// </summary>
        /// <param name="keepPreviousSelection">If true keeps the previous selection in PowerShape.</param>
        public void AddContentsToSelection(bool keepPreviousSelection = false)
        {
            // Get current selection for restoration if required
            List<PSEntity> previouslySelected = new List<PSEntity>();
            if (keepPreviousSelection)
            {
                foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
                {
                    previouslySelected.Add(selectedEntity);
                }
            }

            // Add entities to selection
            _powerSHAPE.DoCommand("FILTERBUTTON FILTERITEMS");
            _powerSHAPE.DoCommand("ALLTYPES", "ALLSTYLES", "ALLLEVELS", "INVERTLEVEL");
            _powerSHAPE.DoCommand("SELECTLEVEL " + Number, "ALL", "ACCEPT");
            _powerSHAPE.DoCommand("EVERYTHING PARTIALBOX");

            // Restore previous selection, if required
            foreach (PSEntity selectedEntity in previouslySelected)
            {
                selectedEntity.AddToSelection(false);
            }
        }

        /// <summary>
        /// Deletes all entities on this level.
        /// </summary>
        public void Clear()
        {
            AddContentsToSelection(false);

            foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
            {
                selectedEntity.Delete();
            }
        }

        #endregion
    }
}