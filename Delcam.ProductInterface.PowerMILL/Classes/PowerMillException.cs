// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.ProductInterface.PowerMILL
{
    public class PowerMillException : Exception
    {
        private string _reason { get; }
        private Version _version { get; }

        public PowerMillException(Version version, string message)
        {
            _version = version;
            _reason = message;
        }

        public override string Message
        {
            get { return string.Format("PowerMill Version {0} Error: {1}", _version, _reason); }
        }
    }
}