// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Base class for all PowerShape Product Interface collections which items are not PSEntities.
    /// It controls the addition of items to PowerShape collections.
    /// </summary>
    /// <typeparam name="T">The type of the item. It can be a PSEntity, a PSLevel, etc.</typeparam>
    /// <remarks></remarks>
    public abstract class PSCollection<T> : List<T>
    {
    }
}