// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents a boundary at the extents of the block in PowerMILL.
    /// </summary>
    public class PMBoundaryBlock : PMBoundary
    {
        #region Constructors

        /// <summary>
        /// Creates a new PMBoundary instance.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMBoundaryBlock(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Creates a new PMBoundary instance.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMBoundaryBlock(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion
    }
}