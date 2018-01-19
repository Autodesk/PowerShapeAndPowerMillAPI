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
    /// Captures a wire frame model in PowerSHAPE
    /// </summary>
    /// <remarks></remarks>
    public abstract class PSWireframe : PSEntity
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connects to PowerSHAPE
        /// </summary>
        internal PSWireframe(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        /// <summary>
        /// Connects to PowerSHAPE and a specific entity using its name
        /// </summary>
        internal PSWireframe(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
            _powerSHAPE = powerSHAPE;
            _id = id;
            _name = name;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the start point of the wireframe
        /// </summary>
        public virtual Point StartPoint
        {
            get
            {
                double[] psStartPoint = null;
                psStartPoint = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].START") as double[];

                return new Point(psStartPoint);
            }
        }

        /// <summary>
        /// Gets the end point of the wireframe
        /// </summary>
        public virtual Point EndPoint
        {
            get
            {
                double[] psEndPoint = null;
                psEndPoint = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].END") as double[];

                return new Point(psEndPoint);
            }
        }

        /// <summary>
        /// Gets the number of points in the wireframe
        /// </summary>
        public virtual int NumberPoints
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].NUMBER"); }
        }

        /// <summary>
        /// Gets the points on the Wireframe
        /// </summary>
        public virtual List<Point> Points
        {
            get
            {
                AbortIfDoesNotExist();

                List<Point> wireframePoints = new List<Point>();
                for (int index = 1; index <= NumberPoints; index++)
                {
                    double[] coordinates = null;
                    coordinates = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].POINT[" + index + "]") as double[];
                    Point point = new Point(coordinates[0], coordinates[1], coordinates[2]);
                    wireframePoints.Add(point);
                }

                return wireframePoints;
            }
        }

        /// <summary>
        /// Gets the point at the specified zero based index
        /// </summary>
        /// <param name="index">The index of the required point</param>
        public virtual Point Point(int index)
        {
            AbortIfDoesNotExist();
            if (index >= NumberPoints)
            {
                throw new IndexOutOfRangeException();
            }

            double[] coordinates = null;
            coordinates = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].POINT[" + (index + 1) + "]") as double[];
            return new Point(coordinates[0], coordinates[1], coordinates[2]);
        }

        /// <summary>
        /// Gets the length of the wireframe
        /// </summary>
        public virtual MM Length
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH");
            }
        }

        /// <summary>
        /// Gets the centre of gravity of the wireframe
        /// </summary>
        public virtual Point CentreOfGravity
        {
            get
            {
                double[] psCOG = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].COG") as double[];
                return new Point(psCOG[0], psCOG[1], psCOG[2]);
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Projects a Curve onto a surface.
        /// </summary>
        /// <param name="surface">The surface to wrap onto.</param>
        /// <param name="projectionType">The type of projection to be carried out.</param>
        /// <param name="keepOriginal">If true, it deletes the original entity.</param>
        /// <remarks>The old CompCurve is removed, and this object's ID set to be the new ID.</remarks>
        /// <returns>List of entities created by the operation.</returns>
        public virtual List<PSCompCurve> ProjectOntoSurface(PSEntity surface, ProjectionType projectionType, bool keepOriginal)
        {
            // Add necessary components to the selection
            AddToSelection(true);
            surface.AddToSelection(false);

            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.DoCommand("CREATE CURVE PROJECT");

            // Select correct projection type
            switch (projectionType)
            {
                case ProjectionType.AlongPrincipalAxis:
                    _powerSHAPE.DoCommand("PRINCIPAL");
                    break;
                case ProjectionType.AlongSurfaceNormal:
                    _powerSHAPE.DoCommand("NORMAL");
                    break;
                case ProjectionType.ThroughSurface:
                    _powerSHAPE.DoCommand("THRU");
                    break;
            }

            _powerSHAPE.DoCommand("ACCEPT");

            List<PSCompCurve> newEntities = new List<PSCompCurve>();
            foreach (PSCompCurve newEntity in _powerSHAPE.ActiveModel.CreatedItems)
            {
                _powerSHAPE.ActiveModel.Add(newEntity);
                newEntities.Add(newEntity);
            }

            if (keepOriginal == false)
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("DELETE");
            }

            return newEntities;
        }

        #endregion
    }
}