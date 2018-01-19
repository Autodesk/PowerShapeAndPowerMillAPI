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
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').origin.x\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').origin.y\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').origin.z\"")
                                     .ToString()
                                     .Trim()));
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
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Xaxis.x\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Xaxis.y\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Xaxis.z\"")
                                     .ToString()
                                     .Trim()));
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
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Yaxis.x\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Yaxis.y\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Yaxis.z\"")
                                     .ToString()
                                     .Trim()));
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
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Zaxis.x\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Zaxis.y\"")
                                     .ToString()
                                     .Trim()),
                        Convert.ToDouble(
                            PowerMill.DoCommandEx("PRINT PAR terse \"entity('workplane', '" + Name + "').Zaxis.z\"")
                                     .ToString()
                                     .Trim()));
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