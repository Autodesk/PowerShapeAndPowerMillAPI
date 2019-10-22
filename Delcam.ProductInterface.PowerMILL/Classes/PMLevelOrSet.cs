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
    /// Captures a Level or Set object in PowerMILL.
    /// </summary>
    public class PMLevelOrSet : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMLevelOrSet(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMLevelOrSet(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the visibility of the level or set in PowerMill.
        /// </summary>
        public bool Visible
        {
            set
            {
                if (value)
                {
                    PowerMill.DoCommand("DRAW LEVEL '" + Name + "'");
                }
                else
                {
                    PowerMill.DoCommand("UNDRAW LEVEL '" + Name + "'");
                }
            }
        }

        internal static string LEVEL_OR_SET_IDENTIFIER = "LEVEL";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return LEVEL_OR_SET_IDENTIFIER; }
        }

        /// <summary>
        /// BoundingBox property is not valid for this type.
        /// </summary>
        public override Geometry.BoundingBox BoundingBox
        {
            get { throw new Exception("Property not valid for " + GetType()); }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes Level or Set. It also updates PowerMill.
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            PowerMill.ActiveProject.LevelsAndSets.Remove(this);
        }

        /// <summary>
        /// Select all from Level or Set
        /// </summary>
        /// <remarks></remarks>
        public void SelectAll()
        {
            PowerMill.DoCommand("EDIT LEVEL '" + Name + "' SELECT ALL");
        }

        /// <summary>
        /// Select wireframe from Level or Set
        /// </summary>
        /// <remarks></remarks>
        public void SelectWireframe()
        {
            PowerMill.DoCommand("EDIT LEVEL '" + Name + "' SELECT WIREFRAME");
        }

        /// <summary>
        /// Select surfaces from Level or Set
        /// </summary>
        /// <remarks></remarks>
        public void SelectSurfaces()
        {
            PowerMill.DoCommand("EDIT LEVEL '" + Name + "' SELECT SURFACE");
        }

        #endregion
    }
}