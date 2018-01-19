// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.ProductInterface.Properties;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represent a Lead extension in <see cref="PMToolpath"/>.
    /// </summary>
    public class PMLeadExtension : PMLead
    {
        public PMLeadExtension(LeadTypes leadInExtension, PMToolpath toolpath) : base(leadInExtension, toolpath)
        {
        }

        /// <summary>
        /// The type of move to use after each cutting move
        /// </summary>
        public new ExtensionLeadMoveTypes MoveType
        {
            get
            {
                var result = Toolpath.GetParameter(GetPropertyFullName(Resources.ExtensionLeadMoveTypes));
                var resourceset = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture,
                                                                           false,
                                                                           true);
                return Extensions.EnumExtensions.GetEnumByResourceValue<ExtensionLeadMoveTypes>(result, resourceset);
            }
            set
            {
                Toolpath.SetParameter(GetPropertyFullName(Resources.ExtensionLeadMoveTypes),
                                      Resources.ResourceManager.GetString(value.ToString()));
            }
        }
    }
}