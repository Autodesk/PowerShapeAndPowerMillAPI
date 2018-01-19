// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Diagnostics;

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Allows CopyCAD to signal to PowerSHAPE that it is busy.
    /// </summary>
    public class CommandBusyFlag
    {
        // CONVERTED FROM Unmanaged C++ by Parmi.
        //
        // A simple class to allow CopyCAD to signal to PowerSHAPE when it is busy.
        //
        // To set the busy flag:
        //
        // truCCBusyFlag::set();
        //
        // to unset:
        //
        // truCCBusyFlag::unset();
        //
        // to check it:
        //
        // truCCBusyFlag::is_set();
        //
        // If truCCBusyFlag::set() is called from CopyCAD, then calling
        // truCCBusyFlag::is_set() in PowerSHAPE will return utTRUE.
        //
        // ----------------------------------------------------------------------------
        // (c) Copyright 2000 Delcam International plc
        // ----------------------------------------------------------------------------
        //
        // History.
        // DICC  Who When     What
        // ----- --- -------- ---------------------------------------------------------
        // 70863 psd 15/05/08 Written.
        //-----------------------------------------------------------------------------

        private string BUSY_FLAG = "";
        private string THE_SET_FLAG = "Y";
        private string THE_UNSET_FLAG = "N";

        private string THE_INVALID_FLAG = "blah";

        //Shared Memory Class 
        private SharedMemory NamedSHM = new SharedMemory();

        /// <summary>
        /// Creates the flag object and maps into this process shared memory of the corresponding name.
        /// If it does not already exist then it is created.
        /// </summary>
        /// <param name="flag">Flag name and the name of the shared memory to be mapped/created.</param>
        /// <remarks></remarks>
        public CommandBusyFlag(string flag)
        {
            BUSY_FLAG = flag;
            if (string.IsNullOrEmpty(BUSY_FLAG))
            {
                Debug.Assert(false, "Flag name not set");
            }

            NamedSHM.Open(BUSY_FLAG, 2);
        }

        /// <summary>
        /// Returns True if the flag has been set; False otherwise.
        /// </summary>
        public object is_set()
        {
            string flag = THE_INVALID_FLAG;
            flag = NamedSHM.Peek();

            if (flag == THE_SET_FLAG)
            {
                return true;
            }
            if (flag == THE_UNSET_FLAG)
            {
                return false;
            }
            Debug.Assert(false, "The flag wasn't set!");
            return false;
        }

        /// <summary>
        /// Resets the flag.
        /// </summary>
        public void unset_flag()
        {
            NamedSHM.Put(THE_UNSET_FLAG);
        }

        /// <summary>
        /// Sets the flag.
        /// </summary>
        public void set_flag()
        {
            NamedSHM.Put(THE_SET_FLAG);
        }
    }
}