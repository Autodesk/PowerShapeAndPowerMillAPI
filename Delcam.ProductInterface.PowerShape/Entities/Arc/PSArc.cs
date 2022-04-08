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
    /// Captures an Arc object in PowerSHAPE
    /// </summary>
    public class PSArc : PSWireframe, IPSMoveable, IPSRotateable, IPSMirrorable, IPSLimitable, IPSCuttable, IPSOffsetable
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Arc
        /// </summary>
        internal PSArc(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a full Arc (circle)
        /// </summary>
        internal PSArc(PSAutomation powerSHAPE, Point centre, Point startPoint, MM radius) : this(
            powerSHAPE,
            centre,
            startPoint,
            startPoint,
            radius,
            360.0f)
        {
        }

        /// <summary>
        /// Creates an Arc based on a start point, radius and span
        /// </summary>
        internal PSArc(PSAutomation powerSHAPE, Point centre, Point startPoint, MM radius, Degree span) : this(
            powerSHAPE,
            centre,
            startPoint,
            startPoint,
            radius,
            (float) span)
        {
        }

        /// <summary>
        /// Creates an Arc based on a start point, end point and radius
        /// </summary>
        internal PSArc(PSAutomation powerSHAPE, Point centre, Point startPoint, Point endPoint) : this(
            powerSHAPE,
            centre,
            startPoint,
            endPoint,
            0,
            0f)
        {
        }

        /// <summary>
        /// Creates an Arc based on all parameters
        /// </summary>
        private PSArc(
            PSAutomation powerSHAPE,
            Point centre,
            Point startPoint,
            Point endPoint,
            MM radius,
            float span) : base(powerSHAPE)
        {
            //Clear the list of CreatedItems
            _powerSHAPE.ActiveModel.ClearCreatedItems();

            //Create the line
            _powerSHAPE.DoCommand("CREATE ARC SWEPT",
                                  centre.X.ToString("0.######") + " " + centre.Y.ToString("0.######") + " " + centre.Z.ToString("0.######"),
                                  "ABS " + startPoint.X.ToString("0.######") + " " + startPoint.Y.ToString("0.######") + " " +
                                  startPoint.Z.ToString("0.######"),
                                  "ABS " + endPoint.X.ToString("0.######") + " " + endPoint.Y.ToString("0.######") + " " + endPoint.Z.ToString("0.######"),
                                  "QUIT QUIT");

            // Get its id
            PSArc newArc = (PSArc) _powerSHAPE.ActiveModel.CreatedItems[0];
            _name = newArc.Name;
            _id = newArc.Id;

            // Set the radius and span
            if (radius != null)
            {
                Radius = radius;
            }

            if (span != null)
            {
                Span = span;
            }
        }

        #endregion

        #region " Properties "

        internal const string ARC_IDENTIFIER = "ARC";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return ARC_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the command that'll be used to rename the Arc
        /// </summary>
        internal override string RenameCommand
        {
            get { return "MODIFY"; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 52 * 20000000 + _id; }
        }

        /// <summary>
        /// Gets and sets the Radius of the Arc
        /// </summary>
        public MM Radius
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].RADIUS");
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "DIMENSION " + value.ToString("0.######"), "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the span of the Arc in Degrees
        /// </summary>
        public Degree Span
        {
            get
            {
                AbortIfDoesNotExist();

                //' Check whether version is less than 10.2.22, as macro code changes for Span after this
                //If _powerSHAPE.Version < New Version(10, 2) Then
                //    Return CType(_powerSHAPE.DoCommandEx(Identifier & "[ID " & _id & "].SPAN"), Geometry.Degree)
                //End If
                double spanAngle = _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].SPAN_ANGLE");
                return spanAngle;
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "SPAN " + value, "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the centre of the Arc
        /// </summary>
        public Point Centre
        {
            get
            {
                AbortIfDoesNotExist();
                double[] point = null;
                point = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].CENTRE") as double[];
                return new Point(point[0], point[1], point[2]);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY",
                                      "CENTRE X " + value.X.ToString("0.######"),
                                      "CENTRE Y " + value.Y.ToString("0.######"),
                                      "CENTRE Z " + value.Z.ToString("0.######"),
                                      "ACCEPT");
            }
        }

        /// <summary>
        /// Gets the Centre of Gravity of the arc
        /// </summary>
        public override Point CentreOfGravity
        {
// Get COG from bounding box details

            get
            {
                return new Point((BoundingBox.MaxX + BoundingBox.MinX) / 2.0,
                                 (BoundingBox.MaxY + BoundingBox.MinY) / 2.0,
                                 (BoundingBox.MaxZ + BoundingBox.MinZ) / 2.0);
            }
        }

        /// <summary>
        /// Gets and sets the Through point of the Arc (approximately the mid point of the Arc)
        /// </summary>
        public Point Through
        {
            get
            {
                AbortIfDoesNotExist();
                double[] point = null;
                point = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].MID") as double[];
                return new Point(point[0], point[1], point[2]);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY",
                                      "THROUGH X " + value.X.ToString("0.######"),
                                      "THROUGH Y " + value.Y.ToString("0.######"),
                                      "THROUGH Z " + value.Z.ToString("0.######"),
                                      "ACCEPT");
            }
        }

        /// <summary>
        /// Gets the midpoint of the Arc
        /// </summary>
        public Point MidPoint
        {
            get
            {
                AbortIfDoesNotExist();
                double[] point = null;
                point = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].MID") as double[];
                return new Point(point[0], point[1], point[2]);
            }
        }

        /// <summary>
        /// Gets the points on the Arc
        /// </summary>
        public override List<Point> Points
        {
            get
            {
                List<Point> arcPoints = new List<Point>();

                // Add start and end points to the list that will be returned
                arcPoints.Add(StartPoint);
                arcPoints.Add(EndPoint);

                return arcPoints;
            }
        }

        /// <summary>
        /// Overrides the NumberPoints property, as an arc will always have 2 points
        /// </summary>
        /// <returns>Always returns 2 as arcs have two points</returns>
        public override int NumberPoints
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets and sets the start angle of the Arc in Degrees
        /// </summary>
        /// <returns>Angle in degrees</returns>
        public Degree StartAngle
        {
            get
            {
                AbortIfDoesNotExist();

                double psStartAngle = _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].START_ANGLE");
                return psStartAngle;
            }
        }

        /// <summary>
        /// Gets and sets the end angle of the Arc in Degrees
        /// </summary>
        /// <returns>Angle in degrees</returns>
        public Degree EndAngle
        {
            get
            {
                AbortIfDoesNotExist();

                double psEndAngle = _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].END_ANGLE");
                return psEndAngle;
            }
        }

        /// <summary>
        /// Gets the vector of the normal to the arc plane
        /// </summary>
        /// <returns>Vector of the normal to the arc plane</returns>
        public Vector CentreNormal
        {
            get
            {
                // Create vector from centre to startpoint
                Vector centreToStart = new Vector(Centre, StartPoint);

                // Create vector from centre to endpoint
                Vector centreToEnd = new Vector(Centre, EndPoint);

                // Return cross product of two vectors
                return Vector.CrossProduct(centreToStart, centreToEnd);
            }
        }

        #endregion

        #region " Operations "

        // TODO: Write GetPointOnArc function (with angleFromStartPoint as a parameter)

        /// <summary>
        /// Deletes the Arc from PowerSHAPE and removes it from the Arcs collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Arcs.Remove(this);
        }

        /// <summary>
        /// Reverses the direction of the Arc
        /// </summary>
        public void Reverse()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("MODIFY", "REVERSE", "ACCEPT");
        }

        /// <summary>
        /// Completes the Arc (equivalent of setting the span to 360 degrees)
        /// </summary>
        public void Full()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("MODIFY", "FULL", "ACCEPT");
        }

        /// <summary>
        /// Scales an arc within PowerSHAPE, returning a curve.  This deletes the current arc.
        /// </summary>
        /// <param name="offsetX">Scale value in X</param>
        /// <param name="offsetY">Scale value in Y</param>
        /// <param name="offsetZ">Scale value in Z</param>
        /// <param name="scaleOrigin">Scale origin, workplane origin if none specified</param>
        /// <returns>Created arc</returns>
        public PSCurve ScaleToCurve(double offsetX, double offsetY, double offsetZ, Point scaleOrigin = null)
        {
            // Open dialog
            _powerSHAPE.DoCommand("EDIT SCALE");
            _powerSHAPE.DoCommand("NOKEEP");

            // If a different scale origin has been defined, set it within PowerSHAPE
            if (scaleOrigin != null)
            {
                _powerSHAPE.DoCommand("SCALEORIGIN");
                _powerSHAPE.DoCommand(scaleOrigin.X.ToString("0.######") + " " + scaleOrigin.Y.ToString("0.######") + " " + scaleOrigin.Z.ToString("0.######"));
            }

            // Set the scaling to be non-uniform, so allowing each component to be edited individually
            _powerSHAPE.DoCommand("NONUNIFORM");

            // Ensure arcs will not be removed from the selection
            _powerSHAPE.DoCommand("REMOVEARCS NO");

            // Unlock all axes
            _powerSHAPE.DoCommand("LOCK X OFF");
            _powerSHAPE.DoCommand("LOCK Y OFF");
            _powerSHAPE.DoCommand("LOCK Z OFF");

            // Enter scaling values
            _powerSHAPE.DoCommand("X " + offsetX.ToString("0.######"));
            _powerSHAPE.DoCommand("Y " + offsetY.ToString("0.######"));
            _powerSHAPE.DoCommand("Z " + offsetZ.ToString("0.######"));

            if (_powerSHAPE.Version.Major > 11)
            {
                _powerSHAPE.DoCommand("APPLY");
                _powerSHAPE.DoCommand("DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand("ACCEPT");
                _powerSHAPE.DoCommand("CANCEL");
            }

            // Check that one curve is selected
            if (_powerSHAPE.Version >= new Version("13.2"))
            {
                if ((_powerSHAPE.ActiveModel.SelectedItems.Count == 0) | Exists)
                {
                    throw new ApplicationException("Scaling operation not completed correctly");
                }
            }
            else
            {
                if ((_powerSHAPE.ActiveModel.CreatedItems.Count == 0) | Exists)
                {
                    throw new ApplicationException("Scaling operation not completed correctly");
                }
            }

            // Delete the current arc
            Delete();

            // Add the created curve
            PSCurve createdCurve = null;

            if (_powerSHAPE.Version >= new Version("13.2"))
            {
                createdCurve = (PSCurve) _powerSHAPE.ActiveModel.SelectedItems[0];
            }
            else
            {
                createdCurve = (PSCurve) _powerSHAPE.ActiveModel.CreatedItems[0];
            }

            _powerSHAPE.ActiveModel.Curves.Add(createdCurve);

            // Return created curve
            return createdCurve;
        }

        #region " Edit Operations "

        /// <summary>
        /// Moves a single arc by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">First of the two absolute positions</param>
        /// <param name="pointToMoveTo">Second of the two absolute positions</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation</param>
        /// <returns>A list of arcs created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// Moves a single arc by a specified relative amount
        /// </summary>
        /// <param name="moveVector">The relative amount by which the arc will be moved</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>A list of arcs created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Rotates a single arc by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">The axis around which the arc is are to be rotated</param>
        /// <param name="rotationAngle">The angle by which the arc is to be rotated</param>
        /// <param name="copiesToCreate">The number of copies to create of the original arc</param>
        /// <param name="rotationOrigin">The origin of the rotation axis</param>
        /// <returns>A list of arcs created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// This operation offsets a single arc by a specified distance
        /// </summary>
        /// <param name="offsetDistance">The distance by which to offset the mesh</param>
        /// <param name="copiesToCreate">The number of copies to be created from the operation</param>
        /// <returns>A list of offset arcs</returns>
        public List<PSEntity> Offset(MM offsetDistance, int copiesToCreate)
        {
            return PSEntityOffseter.OffsetEntity(this, offsetDistance, copiesToCreate);
        }

        /// <summary>
        /// Mirrors a single arc in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the arc</param>
        /// <param name="mirrorPoint">The origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// Limits an arc by a specified list of entities
        /// </summary>
        /// <param name="limitingEntity">The entity with which to perform the limiting operation</param>
        /// <param name="limitingMode">The mode in which to carry out the operation</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit</param>
        /// <param name="trimOption">Whether to trim one or all of the entities</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>A list of limited entities</returns>
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
        /// Limits an arc by a specified list of entities.
        /// </summary>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>A list of limited entities.</returns>
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
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <returns>Limited entity</returns>
        /// <remarks>To be used with PSEntityLimiter.NextSolution</remarks>
        public PSEntity LimitUsingDynamicCutter(LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne)
        {
            return PSEntityLimiter.LimitEntityUsingDynamicCutter(this, keepOption);
        }

        /// <summary>
        /// Cuts an arc at a specified point
        /// </summary>
        /// <param name="pointAtWhichToCut">The point at which to cut the arc - this must lie on the arc</param>
        /// <returns>The cut piece of the original arc</returns>
        public PSEntity Cut(Point pointAtWhichToCut)
        {
            // Check that point lies on arc
            // TODO: Write this functionality

            // Carry out operation
            PSArc newArc = (PSArc) PSEntityCutter.CutEntity(this, pointAtWhichToCut);

            // Return arc
            return newArc;
        }

        #endregion

        #endregion
    }
}