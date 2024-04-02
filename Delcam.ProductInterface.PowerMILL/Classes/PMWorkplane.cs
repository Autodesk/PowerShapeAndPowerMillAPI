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
    /// Captures the a Workplane in PowerMILL.
    /// </summary>
    public class PMWorkplane : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMWorkplane(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMWorkplane(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string WORKPLANE_IDENTIFIER = "WORKPLANE";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return WORKPLANE_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the Origin of Workplane.
        /// </summary>
        public Geometry.Point Origin
        {
            get
            {
                return
                    new Geometry.Point(
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "origin.x").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "origin.y").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "origin.y").Trim())
                        );
            }
        }

        /// <summary>
        /// Gets the XAxis vector.
        /// </summary>
        public Geometry.Vector XAxisVector
        {
            get
            {
                return
                    new Geometry.Vector(
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Xaxis.x").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Xaxis.y").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Xaxis.z").Trim())
                        );
            }
        }

        /// <summary>
        /// Gets the YAxis vector.
        /// </summary>
        public Geometry.Vector YAxisVector
        {
            get
            {
                return
                    new Geometry.Vector(
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Yaxis.x").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Yaxis.y").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Yaxis.z").Trim())
                        );
            }
        }

        /// <summary>
        /// Gets the ZAxis vector.
        /// </summary>
        public Geometry.Vector ZAxisVector
        {
            get
            {
                return
                    new Geometry.Vector(
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Zaxis.x").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Zaxis.y").Trim()),
                        Convert.ToDouble(
                            PowerMill.GetPowerMillEntityParameter("workplane", Name, "Zaxis.z").Trim())
                        );
            }
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
        /// Deletes the workplane from the active project and from PowerMill.
        /// </summary>
        /// <remarks></remarks>
        public override void Delete()
        {
            PowerMill.ActiveProject.Workplanes.Remove(this);
        }

        /// <summary>
        /// Returns a Workplane representation of this PMWorkplane
        /// </summary>
        public Geometry.Workplane ToWorkplane()
        {
            return new Geometry.Workplane(Origin, XAxisVector, YAxisVector, ZAxisVector);
        }

        #endregion
    }
}