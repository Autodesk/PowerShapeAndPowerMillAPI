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
    public class InvalidPowerMillParameterException : Exception
    {
        private readonly Version _version;
        private readonly string _parameterName;

        public InvalidPowerMillParameterException(Version version, string parameterName)
        {
            _version = version;
            _parameterName = parameterName;
        }

        public override string Message
        {
            get { return string.Format("PowerMill Version {0} does not have parameter named: {1}", _version, _parameterName); }
        }
    }
}