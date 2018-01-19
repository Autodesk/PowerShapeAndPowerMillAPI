// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Collection of Windows in CustomCore.
    /// </summary>
    /// <remarks></remarks>
    public class PSWindowsCollection : PSDatabaseEntitiesCollection<PSWindow>
    {
        #region " Constructors "

        /// <summary>
        /// This constructor initialises the model list object.
        /// </summary>
        /// <param name="powerSHAPE">The Automation object for PowerSHAPE.</param>
        internal PSWindowsCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Window.
        /// </summary>
        internal override string Identifier
        {
            get { return "WINDOW"; }
        }

        /// <summary>
        /// Returns true if there is any window opened in PowerShape and false otherwise.
        /// </summary>
        internal bool HasWindowsOpen
        {
            get { return _powerSHAPE.ReadIntValue("WINDOW.NUMBER") != 0; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Gets the desired named window from the the collection.
        /// </summary>
        /// <returns>The desired window.</returns>
        /// <value>The name of the desired window (which is synonymous with its number).</value>
        public PSWindow this[string name]
        {
            // Use List property
            get
            {
                foreach (PSWindow window in this)
                {
                    if (window.Name == name)
                    {
                        return window;
                    }
                }

                // Window is not in collection
                return null;
            }
            set { this[Convert.ToInt32(name)] = value; }
        }

        /// <summary>
        /// Removes the window at the specified index from the collection and from PowerShape.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <remarks></remarks>
        public new void RemoveAt(int index)
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException("Current number of items is " + Count);
            }

            //Close all windows associated with the selected model
            PSWindow window = this[index];
            Remove(window);
        }

        /// <summary>
        /// Removes the window with the specified name from the collection and from PowerShape.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        /// <returns>True if it succeeds.</returns>
        /// <remarks></remarks>
        public bool Remove(string name)
        {
            var window = this.FirstOrDefault(w => w.Name == name);
            if (window != null)
            {
                Remove(window);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the specified window.
        /// </summary>
        /// <param name="window">The window to delete.</param>
        /// <returns>True if it succeeds.</returns>
        /// <remarks></remarks>
        public bool Remove(PSWindow window)
        {
            if (window.Exists)
            {
                // Close window
                window.MakeActive();
                PSModel activeModel = _powerSHAPE.ActiveModel;
                _powerSHAPE.DoCommand("CANCEL");
                _powerSHAPE.DoCommand("FILE CLOSE SELECTED YES");
                _powerSHAPE.Models.Remove(activeModel);
            }

            if (Contains(window))
            {
                return base.Remove(window);
            }

            return false;
        }

        /// <summary>
        /// Closes and removes all windows in the list.
        /// </summary>
        public override void Clear()
        {
            // Close all open windows
            int numberOfWindows = Count;
            for (int i = numberOfWindows - 1; i >= 0; i += -1)
            {
                // Remove window from collection
                Remove(this[i]);
            }
            base.Clear();

            // Check that all windows have been removed from the collection
            if (Count != 0)
            {
                throw new ApplicationException("Not all windows have been removed from collection");
            }

            // Check that all windows have been closed
            if (HasWindowsOpen)
            {
                throw new ApplicationException("Not all windows have been closed");
            }
        }

        #endregion
    }
}