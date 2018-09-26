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
    /// Captures a Setup object in PowerMILL.
    /// </summary>
    public class PMSetup : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMSetup(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMSetup(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string SETUP_IDENTIFIER = "SETUP";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return SETUP_IDENTIFIER; }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes Setup. It also updates PowerMill.
        /// </summary>
        public override void Delete()
        {
            PowerMill.ActiveProject.Setups.Remove(this);
        }

        #endregion
    }
}