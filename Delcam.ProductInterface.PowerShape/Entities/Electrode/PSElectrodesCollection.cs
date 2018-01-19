// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a collection of Electrodes in a Project
    /// </summary>
    public class PSElectrodesCollection : PSEntitiesCollection<PSElectrode>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSElectrodesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting an Electrode
        /// </summary>
        internal override string Identifier
        {
            get { return "ELECTRODE"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Adds all of the electrodes to the selection.
        /// </summary>
        /// <param name="emptySelectionFirst">If true it will empty PowerShape selection first.</param>
        public override void AddToSelection(bool emptySelectionFirst = false)
        {
            if (emptySelectionFirst)
            {
                _powerSHAPE.ActiveModel.ClearSelectedItems();
            }

            ForEach(x => x.AddToSelection());
        }

        #endregion
    }
}