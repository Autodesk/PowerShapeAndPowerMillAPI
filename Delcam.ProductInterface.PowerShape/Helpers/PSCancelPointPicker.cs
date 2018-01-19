// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Cancel point picker helper.
    /// </summary>
    /// <remarks></remarks>
    public class PSCancelPointPicker : ICancelPointPickCheck
    {
        private object _myLock = new object();

        private bool _IsCancelled;

        /// <summary>
        /// Sets IsCancelled flag to true.
        /// </summary>
        /// <remarks></remarks>
        public void CanelPointPicking()
        {
            lock (_myLock)
            {
                _IsCancelled = true;
            }
        }

        /// <summary>
        /// Returns IsCancelled flag state.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsCancelPointPick()
        {
            lock (_myLock)
            {
                return _IsCancelled;
            }
        }
    }
}