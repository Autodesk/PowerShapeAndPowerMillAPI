// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Curve object in PowerSHAPE
    /// </summary>
    public class PSpCurve : PSWireframe
    {
        #region " Fields "

        private PSSurface _surface;

        private Polyline _polyLine;

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return "PCURVE"; }
        }

        /// <summary>
        /// This operation checks to see if the pCurve exists in PowerSHAPE
        /// </summary>
        public override bool Exists
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "['" + Name + "'].EXISTS") == 1; }
        }

        /// <summary>
        /// As all PCurve evaluation operations in PowerSHAPE need to use the name, this cannot
        /// be derived from the id
        /// </summary>
        /// <exception cref="ApplicationException">Cannot set the name of a pCurve</exception>
        public override string Name
        {
            get { return base.Name; }
            set { throw new ApplicationException("Cannot set the name of a pCurve"); }
        }

        /// <summary>
        /// Gets the bounding box of the internal polyline
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get { return _polyLine.BoundingBox; }
        }

        /// <summary>
        /// Gets and Sets the level of the parent surface
        /// </summary>
        public override PSLevel Level
        {
            get { return _surface.Level; }
            set { _surface.Level = value; }
        }

        /// <summary>
        /// Gets and Sets the level number of the parent surface
        /// </summary>
        public override uint LevelNumber
        {
            get { return _surface.LevelNumber; }
            set { _surface.LevelNumber = value; }
        }

        /// <summary>
        /// Because many properties of the pCurve are not availble through the PowerSHAPE macro code,
        /// a spline curve is used to emulate the pCurve behind the scenes
        /// </summary>
        private Polyline PolyLine
        {
            get { return _polyLine; }
            set { _polyLine = value; }
        }

        /// <summary>
        /// Returns the parent surface of the pCurve
        /// </summary>
        public PSSurface Surface
        {
            get { return _surface; }
        }

        /// <summary>
        /// Gets the number of points on the pCurve - overriden because the pCurve cannot be
        /// interrogated using its id
        /// </summary>
        public override int NumberPoints
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "['" + Name + "'].NUMBER"); }
        }

        /// <summary>
        /// Gets the points on the pCurve - overriden because the pCurve cannot be
        /// interrogated in PowerSHAPE using its id
        /// </summary>
        public override List<Point> Points
        {
            get
            {
                AbortIfDoesNotExist();

                List<Point> pCurvePoints = new List<Point>();
                int numberPoints = NumberPoints;
                for (int index = 1; index <= numberPoints; index++)
                {
                    double[] coordinates = null;
                    coordinates = _powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].POINT[" + index + "]") as double[];
                    Point point = new Point(coordinates[0], coordinates[1], coordinates[2]);
                    pCurvePoints.Add(point);
                }

                return pCurvePoints;
            }
        }

        /// <summary>
        /// Gets the start point of the pCurve, overriden from Wireframe because can't interrogate
        /// a pCurve using its id
        /// </summary>
        public override Point StartPoint
        {
            get
            {
                double[] startPointFromPowerSHAPE = null;
                startPointFromPowerSHAPE = _powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].START") as double[];

                return new Point(startPointFromPowerSHAPE);
            }
        }

        /// <summary>
        /// Gets the end point of the pCurve, overriden from Wireframe because can't interrogate
        /// a pCurve using its id
        /// </summary>
        public override Point EndPoint
        {
            get
            {
                double[] endPointFromPowerSHAPE = null;
                endPointFromPowerSHAPE = _powerSHAPE.DoCommandEx(Identifier + "['" + Name + "'].END") as double[];

                return new Point(endPointFromPowerSHAPE);
            }
        }

        /// <summary>
        /// Returns the length of the backend polyline
        /// </summary>
        public override MM Length
        {
            get { return _polyLine.Length; }
        }

        /// <summary>
        /// Gets the volumetric centre of the backend polyline's bounding box
        /// </summary>
        public override Point CentreOfGravity
        {
            get { return _polyLine.BoundingBox.VolumetricCentre; }
        }

        /// <summary>
        /// Gets and sets whether the pCurve is closed or not
        /// </summary>
        public bool IsClosed
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "['" + Name + "'].CLOSED");
            }

            set
            {
                AddToSelection(true);
                if (value)
                {
                    _powerSHAPE.DoCommand("CLOSE");
                }
                else
                {
                    _powerSHAPE.DoCommand("OPEN");
                }
            }
        }

        /// <summary>
        /// Gets whether the pCurve is on the edge of its parent surface
        /// </summary>
        public bool IsOnEdge
        {
            get { return _powerSHAPE.ReadBoolValue(Identifier + "['" + Name + "'].EDGE"); }
        }

        /// <summary>
        /// Gets whether the pCurve is used in a boundary of its parent surface
        /// </summary>
        public bool IsInBoundary
        {
            get { return _powerSHAPE.ReadBoolValue(Identifier + "['" + Name + "'].IN_BOUNDARY"); }
        }

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor connects to PowerSHAPE
        /// </summary>
        internal PSpCurve(PSAutomation powerSHAPE, PSSurface surface, string name) : base(powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
            _surface = surface;

            // A pCurve can't be evaluated in PowerSHAPE using its ID, so the ID is set from its name
            _name = name;
            _id = _powerSHAPE.ReadIntValue(Identifier + "['" + name + "'].ID");

            // Because many properties of the pCurve are not available through the PowerSHAPE interface, a polyline
            // is created internally to emulate it
            _polyLine = new Polyline(Points);
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the pCurve from its surface
        /// </summary>
        /// <exception cref="NotImplementedException">Cannot currently delete a pCurve through the Automation Interface</exception>
        public override void Delete()
        {
        }

        #endregion
    }
}