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
    /// Used when exporting options from PowerSHAPE.
    /// </summary>
    public enum ExportItemsOptions
    {
        /// <summary>
        /// Exports all items that are in the model regardless of selection or visibility.
        /// </summary>
        All,

        /// <summary>
        /// Exports only those items that are selected.  If no items are selection an exception shall be raised.
        /// </summary>
        Selected,

        /// <summary>
        /// Exports only those items that are visible.
        /// </summary>
        Visible
    }
}