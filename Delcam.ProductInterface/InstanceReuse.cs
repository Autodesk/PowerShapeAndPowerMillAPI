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
    /// Specifies how an executable instance is to be obtained.
    /// </summary>
    public enum InstanceReuse
    {
        /// <summary>
        /// Will use an existing running instance
        /// </summary>
        UseExistingInstance,

        /// <summary>
        /// Will create a new instance alongside any existing instances
        /// </summary>
        CreateNewInstance,

        /// <summary>
        /// Will close any existing instances and open a new instance
        /// </summary>
        CreateSingleInstance
    }
}