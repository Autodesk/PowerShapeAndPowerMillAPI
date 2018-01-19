// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Encapsulates data used to report the progress of the Macro.
    /// </summary>
    public class MacroStepEventArgs
    {
        #region " Fields "

        /// <summary>
        /// This is the index of the last run step
        /// </summary>
        private int _index;

        /// <summary>
        /// This is the total number of steps to be executed
        /// </summary>
        private int _totalSteps;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the attributes of the EventArgs to the specified values.
        /// </summary>
        /// <param name="index">Start index.</param>
        /// <param name="totalSteps">Total number of steps.</param>
        public MacroStepEventArgs(int index, int totalSteps)
        {
            _index = index;
            _totalSteps = totalSteps;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Current step index in the Macro.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Total number of steps in the Macro.
        /// </summary>
        public int TotalSteps
        {
            get { return _totalSteps; }
        }

        /// <summary>
        /// Percentage complete: 0.0 - 100.0.
        /// </summary>
        public double Percentage
        {
            get { return _index / Convert.ToDouble(_totalSteps) * 100.0; }
        }

        #endregion
    }
}