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
using System.Linq;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Line object in PowerSHAPE.
    /// </summary>
    public class PSLine : PSWireframe, IPSMoveable, IPSRotateable, IPSMirrorable, IPSCuttable, IPSLimitable
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Line.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="id">The ID for the new line.</param>
        /// <param name="name">The name for the new line.</param>
        /// <remarks></remarks>
        internal PSLine(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a new line in PowerSHAPE between the start and end points.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="startPoint">The start point of the new line.</param>
        /// <param name="endPoint">The end point of the new line.</param>
        /// <remarks></remarks>
        internal PSLine(PSAutomation powerSHAPE, Point startPoint, Point endPoint) : base(powerSHAPE)
        {
            //Clear the list of CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            //Create the line
            _powerSHAPE.DoCommand("CREATE LINE SINGLE");
            var _with1 = startPoint;
            _powerSHAPE.DoCommand("ABS " + _with1.X.ToString("0.######") + " " + _with1.Y.ToString("0.######") + " " + _with1.Z.ToString("0.######"));
            var _with2 = endPoint;
            _powerSHAPE.DoCommand("ABS " + _with2.X.ToString("0.######") + " " + _with2.Y.ToString("0.######") + " " + _with2.Z.ToString("0.######"));
            _powerSHAPE.DoCommand("QUIT QUIT");

            //Now get its Id
            PSLine line = (PSLine) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = line.Id;
        }

        #endregion

        #region " Properties "

        internal const string LINE_IDENTIFIER = "LINE";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return LINE_IDENTIFIER; }
        }

        /// <summary>
        /// Defines the command that'll be used to rename the Line.
        /// </summary>
        internal override string RenameCommand
        {
            get { return "MODIFY"; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object.
        /// </summary>
        internal override int CompositeID
        {
            get { return 58 * 20000000 + _id; }
        }

        /// <summary>
        /// Overrides the NumberPoints property, as a line will always have 2 points.
        /// </summary>
        public override int NumberPoints
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the points on the Wireframe.
        /// </summary>
        public override List<Point> Points
        {
            get
            {
                List<Point> wireframePoints = new List<Point>();

                // Add start and end points to the list that will be returned
                wireframePoints.Add(StartPoint);
                wireframePoints.Add(EndPoint);

                return wireframePoints;
            }
        }

        /// <summary>
        /// Gets the bounding box of the line in PowerSHAPE.
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get
            {
                AbortIfDoesNotExist();

                // Add the start and end points to a collection
                List<Point> pointCollection = new List<Point>();
                pointCollection.Add(StartPoint);
                pointCollection.Add(EndPoint);

                // Find the minimum X, Y and Z coordinates from the start and end points
                Point minimum = new Point(pointCollection.Min(p => p.X),
                                          pointCollection.Min(p => p.Y),
                                          pointCollection.Min(p => p.Z));

                // Find the maximum X, Y and Z coordinates from the start and end points
                Point maximum = new Point(pointCollection.Max(p => p.X),
                                          pointCollection.Max(p => p.Y),
                                          pointCollection.Max(p => p.Z));

                // Create a BoundingBox from the minimum and maximum coordinates
                return new BoundingBox(minimum, maximum);
            }
        }

        /// <summary>
        /// Gets and sets the length of the line.
        /// </summary>
        public new MM Length
        {
            get { return base.Length; }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "LENGTH " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Sets the XY Angle Apparent.
        /// </summary>
        public Degree AngleXYApparent
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "XY", "APPARENT " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Sets the XY Angle Elevation.
        /// </summary>
        public double AngleXYElevation
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "XY", "ELEVATION " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Sets the YZ Angle Apparent.
        /// </summary>
        public Degree AngleYZApparent
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "YZ", "APPARENT " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Sets the YZ Angle Elevation.
        /// </summary>
        public double AngleYZElevation
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "YZ", "ELEVATION " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Sets the ZX Angle Apparent.
        /// </summary>
        public Degree AngleZXApparent
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "ZX", "APPARENT " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Sets the ZX Angle Elevation.
        /// </summary>
        public double AngleZXElevation
        {
            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "ZX", "ELEVATION " + value, "ACCEPT");
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the Line from PowerSHAPE and removes it from the Lines collection.
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Lines.Remove(this);
        }

        /// <summary>
        /// Reverses the direction of the line.
        /// </summary>
        public void Reverse()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("MODIFY", "REVERSE", "ACCEPT");
        }

        /// <summary>
        /// Extends one end of the line.
        /// </summary>
        /// <param name="lineEnd">To specify the end of the curve to be extended.</param>
        /// <param name="length">The extension length.</param>
        /// <param name="straightCurved">The straight curved.</param>
        /// <remarks></remarks>
        public void Extend(ExtensionEnd lineEnd, float length, ExtensionType straightCurved)
        {
            AddToSelection(true);

            // Carry out the extension operation
            _powerSHAPE.DoCommand("EDIT LIMIT POINT");
            _powerSHAPE.DoCommand("PROFILING " + straightCurved);
            _powerSHAPE.DoCommand("EDGE " + (int) lineEnd);
            _powerSHAPE.DoCommand("DISTANCE " + length);
            _powerSHAPE.DoCommand("EDIT LIMIT POINT OFF");

            // The line is renamed/gets a new id, so its database entry needs to be updated
            _id = _powerSHAPE.ActiveModel.SelectedItems[0].Id;
            _name = null;
        }

        /// <summary>
        /// As a line is always straight, the length between two points can be calculated geometrically.
        /// </summary>
        /// <param name="pointA">Point A.</param>
        /// <param name="pointB">Point B.</param>
        /// <exception cref="NotImplementedException">This feature should be calculated geometrically, and as such is not available in PSline</exception>
        /// <returns></returns>
        /// <remarks></remarks>
        public double GetLengthBetweenPoints(float pointA, float pointB)
        {
            throw new NotImplementedException(
                "This feature should be calculated geometrically, and as such is not available in PowerSHAPE");
        }

        #region " Edit Operations "

        /// <summary>
        /// Moves a single line by the relative distance between two absolute positions.
        /// </summary>
        /// <param name="moveOriginCoordinates">First of the two absolute positions.</param>
        /// <param name="pointToMoveTo">Second of the two absolute positions.</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation.</param>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// Moves a single line by a specified relative amount.
        /// </summary>
        /// <param name="moveVector">Relative amount by which the line will be moved.</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation.</param>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Rotates a single line by a specified angle around a specified axis.
        /// </summary>
        /// <param name="rotationAxis">Axis around which the line is are to be rotated.</param>
        /// <param name="rotationAngle">Angle by which the line is to be rotated.</param>
        /// <param name="copiesToCreate">Number of copies to create of the original line.</param>
        /// <param name="rotationOrigin">Origin of the rotation axis.</param>
        /// <returns>List of lines created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// Mirrors a single line in a specified plane.
        /// </summary>
        /// <param name="mirrorPlane">Plane about which to mirror the line.</param>
        /// <param name="mirrorPoint">Origin of the mirror plane.</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// Cuts a single line to produce two lines.
        /// </summary>
        /// <param name="pointAtWhichToCut">Point at which to cut the line. This point must lie on the line.</param>
        /// <exception cref="ApplicationException">Point does not lie on the line.</exception>
        /// <returns>A new line</returns>
        public PSEntity Cut(Point pointAtWhichToCut)
        {
            // Check that the point lies on the line
            if (pointAtWhichToCut.LiesOnLine(StartPoint, EndPoint) == false)
            {
                throw new ApplicationException("Point does not lie on the line");
            }

            // Cut line at defined point
            return PSEntityCutter.CutEntity(this, pointAtWhichToCut);
        }

        /// <summary>
        /// Limits a line by a specified entity.
        /// </summary>
        /// <param name="limitingEntity">The entity with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>The entities produced by the limiting operation.</returns>
        /// <remarks></remarks>
        public List<PSEntity> LimitToEntity(
            PSEntity limitingEntity,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true)
        {
            return PSEntityLimiter.LimitEntity(this, limitingEntity, limitingMode, keepOption, trimOption, finishOperation);
        }

        /// <summary>
        /// Limits a line by a specified list of entities.
        /// </summary>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>The entities produced by the limiting operation.</returns>
        /// <remarks></remarks>
        public List<PSEntity> LimitToEntities(
            List<PSEntity> limitingEntities,
            LimitingModes limitingMode = LimitingModes.LimitMode,
            LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne,
            LimitingTrimOptions trimOption = LimitingTrimOptions.LimitOne,
            bool finishOperation = true)
        {
            return PSEntityLimiter.LimitEntity(this, limitingEntities, limitingMode, keepOption, trimOption, finishOperation);
        }

        /// <summary>
        /// Limits using the dynamic cutter option.
        /// </summary>
        /// <param name="keepOption">Keep option, by default KeepOne.</param>
        /// <returns>The limited entity.</returns>
        /// <remarks></remarks>
        public PSEntity LimitUsingDynamicCutter(LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne)
        {
            return PSEntityLimiter.LimitEntityUsingDynamicCutter(this, keepOption);
        }

        #endregion

        #endregion
    }
}