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
    /// <summary>
    /// Captures a Group object in PowerMILL.
    /// </summary>
    public class PMGroup : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMGroup(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMGroup(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string GROUP_IDENTIFIER = "GROUP";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return GROUP_IDENTIFIER; }
        }

        /// <summary>
        /// BoundingBox property is not valid for this type
        /// </summary>
        public override Geometry.BoundingBox BoundingBox
        {
            get { throw new Exception("Property not valid for " + GetType()); }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes Group from active project and from PowerMill.
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            PowerMill.ActiveProject.Groups.Remove(this);
        }

        #endregion
    }
}