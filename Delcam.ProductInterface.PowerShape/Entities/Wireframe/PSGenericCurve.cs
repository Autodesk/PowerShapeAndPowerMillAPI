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
    /// Base class for curves in PowerSHAPE.
    /// </summary>
    public abstract class PSGenericCurve : PSWireframe, IPSMoveable, IPSRotateable, IPSMirrorable, IPSLimitable
    {
        #region " Constructors "

        /// <summary>
        /// Connects to PowerSHAPE.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <remarks></remarks>
        internal PSGenericCurve(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
        }

        /// <summary>
        /// Connects to PowerSHAPE and a specific entity using its name
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="id">The ID of the new curve.</param>
        /// <param name="name">The name of the new curve.</param>
        /// <remarks></remarks>
        internal PSGenericCurve(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
            _powerSHAPE = powerSHAPE;
            _id = id;
            _name = name;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets and sets whether the curve is closed or not.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].CLOSED");
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
        /// Gets the area of the curve.  If it's closed, the area is the enclosed space.  If it's open, the area
        /// is the enclosed space were it to be closed with a straight line.  If the wireframe is not on a plane,
        /// the area is calculated supposing that it has been projected onto the principal plane.
        /// </summary>
        public double Area
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].AREA"); }
        }

        /// <summary>
        /// Gets the bounding box of the GenericCurve in PowerSHAPE.
        /// </summary>
        /// <returns>A BoundingBox object representing the bounding box in PowerSHAPE.</returns>
        public override BoundingBox BoundingBox
        {
            get
            {
                AbortIfDoesNotExist();
                double[] minSize = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].MIN_RANGE") as double[];
                double[] maxSize = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].MAX_RANGE") as double[];

                return new BoundingBox(minSize[0], maxSize[0], minSize[1], maxSize[1], minSize[2], maxSize[2]);
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Frees the magnitudes and directions of the entire wireframe.
        /// </summary>
        public void FreeMagnitudesAndDirections()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("FREEMAGDIRS");
        }

        /// <summary>
        /// Frees the magnitudes and directions of the selected points on the wireframe.
        /// </summary>
        /// <param name="wireframePoints">The points to edit the tangents and magnitudes. Index for the points is zero based.</param>
        public void FreeMagnitudesDirections(int[] wireframePoints)
        {
            SelectCurvePoints(wireframePoints);
            _powerSHAPE.DoCommand("FREEMAGDIRS");
        }

        /// <summary>
        /// Reverses the curve.
        /// </summary>
        public void Reverse()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("REVERSE_CURVE");
        }

        /// <summary>
        /// Renumbers the points from the specified start point.
        /// </summary>
        /// <param name="intStartPoint">The new start point for the curve. Index for the points is zero based.</param>
        /// <exception cref="Exception">Only closed curves can be renumbered.</exception>
        /// <remarks>Curves may be renumbered. One reason for this may be to open a surface at a different location.</remarks>
        public void RenumberCurvePoints(int intStartPoint)
        {
            // Only closed curves can be renumbered
            if (IsClosed)
            {
                SelectCurvePoints(new[] {intStartPoint});
                _powerSHAPE.DoCommand("RENUMBER");
            }
            else
            {
                throw new Exception("Only closed curves can be renumbered");
            }
        }

        /// <summary>
        /// Selects the specified points.
        /// </summary>
        /// <param name="pointsIndeces">The point indexes in PowerShape. Index for the points is zero based.</param>
        /// <exception cref="Exception">Point is not defined.</exception>
        public void SelectCurvePoints(int[] pointsIndeces)
        {
            AbortIfDoesNotExist();
            string command = "";
            foreach (int pointIndex in pointsIndeces)
            {
                var index = pointIndex + 1;
                if (index < 1 || index > NumberPoints)
                {
                    throw new Exception("Point is not defined");
                }
                command += index + " ";
            }
            command.Trim();

            AddToSelection(true);

            _powerSHAPE.DoCommand("SELECT_POINTS " + command);
        }

        /// <summary>
        /// Deletes the specified points.
        /// </summary>
        /// <param name="pointsIndeces">The point indexes in PowerShape. Index for the points is zero based.</param>
        public void DeleteCurvePoints(int[] pointsIndeces)
        {
            SelectCurvePoints(pointsIndeces);
            _powerSHAPE.DoCommand("DELETE_POINT");
        }

        /// <summary>
        /// Repoints the curve between the specified points creating the specified number of points.
        /// </summary>
        /// <param name="startPoint">The index of the start point. Index for the points is zero based.</param>
        /// <param name="endPoint">The index of the end point. Index for the points is zero based.</param>
        /// <param name="numberOfPoints">The number of new points that are added to the curve and evenly distributed between the start and end points.</param>
        /// <param name="cornerBehaviour">You can select points which you don't want to remove. These points are marked as corners.</param>
        public void RepointCurveBetweenPoints(
            int startPoint,
            int endPoint,
            int numberOfPoints,
            CornerBehaviours cornerBehaviour = CornerBehaviours.SelectCorners)
        {
            AddToSelection(true);

            startPoint = startPoint + 1;
            endPoint = endPoint + 1;

            if (startPoint < 1 || startPoint > NumberPoints)
            {
                throw new Exception("Start Point is not defined");
            }
            if (endPoint < 1 || endPoint > NumberPoints)
            {
                throw new Exception("End Point is not defined");
            }
            if (numberOfPoints < 2)
            {
                throw new Exception("Number of points must be 2 or more");
            }

            string corners = "";
            switch (cornerBehaviour)
            {
                case CornerBehaviours.RemoveAllCorners:
                    corners = "NONE";
                    break;
                case CornerBehaviours.KeepAllDiscontinuities:
                    corners = "ALL";
                    break;
                case CornerBehaviours.SelectCorners:
                    corners = "SELECT";
                    break;
            }
            _powerSHAPE.DoCommand("REPOINT_CURVE",
                                  "START " + startPoint,
                                  "END " + endPoint,
                                  "NUMBER " + numberOfPoints,
                                  "CORNERS " + corners,
                                  "ACCEPT");
        }

        /// <summary>
        /// Extends one end of the curve.
        /// </summary>
        /// <param name="curveEnd">The curve end to be extended.</param>
        /// <param name="length">The length to extend by.</param>
        /// <param name="straightCurved">The extension type.</param>
        /// <remarks></remarks>
        public void Extend(ExtensionEnd curveEnd, float length, ExtensionType straightCurved)
        {
            AddToSelection(true);

            // Carry out the extension operation
            _powerSHAPE.DoCommand("EDIT LIMIT POINT");
            _powerSHAPE.DoCommand("PROFILING " + straightCurved);
            _powerSHAPE.DoCommand("EDGE " + (int) curveEnd);
            _powerSHAPE.DoCommand("DISTANCE " + length);
            _powerSHAPE.DoCommand("EDIT LIMIT POINT OFF");

            // The curve is renamed/gets a new id, so its database entry needs to be updated
            _id = _powerSHAPE.ActiveModel.SelectedItems[0].Id;
            _name = null;
        }

        /// <summary>
        /// Scales a curve within PowerSHAPE.
        /// </summary>
        /// <param name="offsetX">Scale value in X.</param>
        /// <param name="offsetY">Scale value in Y.</param>
        /// <param name="offsetZ">Scale value in Z.</param>
        /// <param name="scaleOrigin">Scale origin, workplane origin if none specified.</param>
        /// <exception cref="ApplicationException">Scaling operation not completed correctly.</exception>
        public void Scale(double offsetX, double offsetY, double offsetZ, Point scaleOrigin = null)
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

            // Unlock all axes
            _powerSHAPE.DoCommand("LOCK X OFF");
            _powerSHAPE.DoCommand("LOCK Y OFF");
            _powerSHAPE.DoCommand("LOCK Z OFF");

            // Enter scaling values
            _powerSHAPE.DoCommand("X " + offsetX.ToString("0.######"));
            _powerSHAPE.DoCommand("Y " + offsetY.ToString("0.######"));
            _powerSHAPE.DoCommand("Z " + offsetZ.ToString("0.######"));

            // Complete operation
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

            // Check that curve has been scaled
            if (_powerSHAPE.ActiveModel.UpdatedItems.Count == 0)
            {
                throw new ApplicationException("Scaling operation not completed correctly");
            }
        }

        /// <summary>
        /// Merges and splines the curve.
        /// </summary>
        public void MergeAndSpline()
        {
            // Add the trimming entity to the selection
            AddToSelection(true);

            _powerSHAPE.DoCommand("MERGE_CURVE");

            // Point this item at the newly created curve object
            _id = _powerSHAPE.ActiveModel.SelectedItems[0].Id;
            _name = _powerSHAPE.ActiveModel.SelectedItems[0].Name;
        }

        /// <summary>
        /// Gets the length of curve between two key points.
        /// </summary>
        /// <param name="pointA">A single representing the chosen start keypoint on the curve. Index starts from zero.</param>
        /// <param name="pointB">A single representing the chosen end keypoint on the curve. Index starts from zero.</param>
        public MM GetLengthBetweenTwoPoints(double pointA, double pointB)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].LENGTH_BETWEEN(" + (pointA + 1) + ";" +
                                               (pointB + 1) + ")");
        }

        /// <summary>
        /// Projects the curve onto the specified plane at the specified position on the plane.
        /// </summary>
        /// <param name="plane">The plane onto which to project the curve.</param>
        /// <param name="positionOnPlane">The position on the plane at which to project the curve.</param>
        public void ProjectOntoPlane(Planes plane, MM positionOnPlane)
        {
            // Determine the axis
            string axis = "";
            switch (plane)
            {
                case Planes.XY:
                    axis = "Z";
                    break;
                case Planes.ZX:
                    axis = "Y";
                    break;
                case Planes.YZ:
                    axis = "X";
                    break;
            }

            // Select the curve
            AddToSelection(true);

            // Do the projection
            _powerSHAPE.DoCommand("EDIT PROJECT",
                                  "ALONG_" + axis,
                                  "DISTANCE " + positionOnPlane.ToString("0.######"),
                                  "KEEP_COPY OFF",
                                  "ACCEPT");
        }

        /// <summary>
        /// Creates a single spline from a generic curve.
        /// </summary>
        public Spline ToSpline()
        {
            // Select the curve
            AddToSelection(true);

            // Write to a pic file
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("pic");
            PSAutomation.ActiveModel.Export(tempFile, ExportItemsOptions.Selected);

            // Geth the data from the pic file
            List<Spline> splineList = Spline.ReadFromDUCTPictureFile(tempFile);

            // create a single spline from the data
            Spline mySpline = new Spline();
            foreach (Spline s in splineList)
            {
                foreach (SplinePoint sp in s)
                {
                    mySpline.Add(sp);
                }
            }

            // Delete the file
            tempFile.Delete();

            // Return the model
            return mySpline;
        }

        /// <summary>
        /// Creates a single polyline from a generic curve.
        /// </summary>
        public Polyline ToPolyline()
        {
            // Select the curve
            AddToSelection(true);

            // Write to a pic file
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("pic");
            PSAutomation.ActiveModel.Export(tempFile, ExportItemsOptions.Selected);

            // Geth the data from the pic file
            List<Polyline> polyLineList = Polyline.ReadFromDUCTPictureFile(tempFile);

            // create a single spline from the data
            Polyline myPolyLine = new Polyline();
            foreach (Polyline s in polyLineList)
            {
                foreach (Point sp in s)
                {
                    myPolyLine.Add(sp);
                }
            }

            // Delete the file
            tempFile.Delete();

            // Return the model
            return myPolyLine;
        }

        #region " Edit Operations "

        /// <summary>
        /// Moves a selected Point on a curve by a direction vector.
        /// </summary>
        /// <param name="pointNumber">Starts from zero.</param>
        /// <param name="direction">Vector.</param>
        public void EditPositionOfPointRelative(int pointNumber, Vector direction)
        {
            AddToSelection();
            SelectCurvePoints(new[] {pointNumber});
            _powerSHAPE.DoCommand("EDIT SUBEDITS ON");
            _powerSHAPE.DoCommand("EDIT MOVE");

            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("X " + direction.I.ToString("0.######"),
                                      "Y " + direction.J.ToString("0.######"),
                                      "Z " + direction.K.ToString("0.######"));
            }
            else
            {
                _powerSHAPE.DoCommand("" + direction.I.ToString("0.######") + " " + direction.J.ToString("0.######") + " " + direction.K.ToString("0.######"));
            }

            _powerSHAPE.DoCommand("APPLY", "DISMISS", "EDIT SUBEDITS OFF");

            AddToSelection();
        }

        /// <summary>
        /// Moves a curve point to a new point position.
        /// </summary>
        /// <param name="pointNumber">The index of the point to move. Index starts from zero.</param>
        /// <param name="newPoint">The new position.</param>
        public void EditPositionOfPointAbsolute(int pointNumber, Point newPoint)
        {
            Point oldPoint = Point(pointNumber);

            //uses index position

            Vector direction = newPoint - oldPoint;

            EditPositionOfPointRelative(pointNumber, direction);
        }

        /// <summary>
        /// Mirrors a single curve in a specified plane.
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the curve.</param>
        /// <param name="mirrorPoint">The origin of the mirror plane.</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// Moves a single curve by the relative distance between two absolute positions.
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions.</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions.</param>
        /// <param name="copiesToCreate">The number of co.pies that should be created by the operation.</param>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// Moves a single curve by a specified relative amount.
        /// </summary>
        /// <param name="moveVector">The relative amount by which the curve will be moved.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Rotates a single curve by a specified .angle around a specified axis.
        /// </summary>
        /// <param name="rotationAxis">The axis around which the curve is are to be rotated.</param>
        /// <param name="rotationAngle">The angle by which the curve is to be rotated.</param>
        /// <param name="copiesToCreate">The number of copies to create of the original curve.</param>
        /// <param name="rotationOrigin">The origin of the rotation axis.</param>
        /// <returns>A list of curves created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// Limits a curve by a specified list of entities.
        /// </summary>
        /// <param name="limitingEntity">The entity with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
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
        /// Limits a curve by a specified list of entities.
        /// </summary>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
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
        /// <param name="keepOption">The limiting keep option to apply.</param>
        /// <returns>Limited entity.</returns>
        /// <remarks>To be used with PSEntityLimiter.NextSolution.</remarks>
        public PSEntity LimitUsingDynamicCutter(LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne)
        {
            return PSEntityLimiter.LimitEntityUsingDynamicCutter(this, keepOption);
        }

        /// <summary>
        /// Inserts a new point.
        /// </summary>
        /// <param name="position">Position for new point. Index starts at zero.</param>
        /// <exception cref="Exception">Position out of range, or failed to insert point.</exception>
        public void InsertPointAtPositionBetweenPoints(double position)
        {
            if ((position < 0.0) | (position > NumberPoints - 1))
            {
                throw new Exception("Position must be between 0.0 and " + (NumberPoints - 1) + ".0.");
            }

            AddToSelection(true);

            int numberOfPointsBefore = NumberPoints;

            _powerSHAPE.DoCommand("ADD_POINT ParameterMethod");
            _powerSHAPE.DoCommand("ParameterMethod Value " + (position + 1));
            _powerSHAPE.DoCommand("APPLY");
            _powerSHAPE.DoCommand("DISMISS");

            if (NumberPoints != numberOfPointsBefore + 1)
            {
                throw new Exception("Failed to insert point.");
            }
        }

        /// <summary>
        /// Inserts a new point by proximal point.
        /// </summary>
        /// <param name="proximalPoint">Proximal position for point.</param>
        /// <exception cref="Exception">Failed to insert point.</exception>
        public void InsertPointByProximity(Point proximalPoint)
        {
            AddToSelection(true);

            int numberOfPointsBefore = NumberPoints;

            _powerSHAPE.DoCommand("ADD_POINT NearestPoint");
            _powerSHAPE.DoCommand("NearestPoint X " + proximalPoint.X.ToString("0.######"));
            _powerSHAPE.DoCommand("NearestPoint Y " + proximalPoint.Y.ToString("0.######"));
            _powerSHAPE.DoCommand("NearestPoint Z " + proximalPoint.Z.ToString("0.######"));
            _powerSHAPE.DoCommand("APPLY");
            _powerSHAPE.DoCommand("DISMISS");

            if (NumberPoints != numberOfPointsBefore + 1)
            {
                throw new Exception("Failed to insert point.");
            }
        }

        /// <summary>
        /// Inserts a new relative point.
        /// </summary>
        /// <param name="referencePointIndex">Reference point. Index starting from zero</param>
        /// <param name="distance">Distance from reference point.</param>
        /// <exception cref="Exception">Failed to insert point.</exception>
        public void InsertPointRelativeToPointOnCurve(int referencePointIndex, double distance)
        {
            AddToSelection(true);

            SelectCurvePoints(new[] {referencePointIndex});

            int numberOfPointsBefore = NumberPoints;

            _powerSHAPE.DoCommand("ADD_POINT DistanceMethod");
            _powerSHAPE.DoCommand("DistanceMethod Distance " + distance);
            _powerSHAPE.DoCommand("APPLY");
            _powerSHAPE.DoCommand("DISMISS");

            if (NumberPoints != numberOfPointsBefore + 1)
            {
                throw new Exception("Failed to insert point.");
            }
        }

        /// <summary>
        /// Inserts a new point at an offset from the plane intersection.
        /// </summary>
        /// <param name="plane">Plane intersecting curve.</param>
        /// <param name="offset">Value to offset.</param>
        /// <exception cref="Exception">Failed to insert points.</exception>
        public void InsertPointsAtPlaneIntersections(Planes plane, double offset)
        {
            _powerSHAPE.SetActivePlane(plane);

            AddToSelection(true);

            int numberOfPointsBefore = NumberPoints;

            _powerSHAPE.DoCommand("ADD_POINT cueWpIntersect");
            _powerSHAPE.DoCommand("cueWpIntersect offset " + offset);
            _powerSHAPE.DoCommand("APPLY");
            _powerSHAPE.DoCommand("DISMISS");

            if (NumberPoints == numberOfPointsBefore)
            {
                throw new Exception("Failed to insert points.");
            }
        }

        #endregion

        #region " Tangency Interrogations "

        /// <summary>
        /// Entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which entry tangency is required. Index starts from zero.</param>
        public Vector GetEntryTangentOfPoint(int pointNumber)
        {
            var result = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) + "].ENTRY_TANGENT");
            double[] entryTangent = result as double[];
            return new Vector(entryTangent[0], entryTangent[1], entryTangent[2]);
        }

        /// <summary>
        /// Gets the exit tangent of the desired point.
        /// </summary>
        /// .
        /// <param name="pointNumber">Point at which exit tangency is required. Index starts from zero.</param>
        public Vector GetExitTangentOfPoint(int pointNumber)
        {
            double[] exitTangent =
                _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) +
                                        "].EXIT_TANGENT") as double[];
            if (exitTangent == null)
            {
                return new Vector();
            }
            return new Vector(exitTangent[0], exitTangent[1], exitTangent[2]);
        }

        /// <summary>
        /// Gets the magnitude of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which entry tangency magnitude is require. Index starts from zero.</param>
        public double GetEntryMagnitudeOfPoint(int pointNumber)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) + "].ENTRY_MAGNITUDE");
        }

        /// <summary>
        /// Gets the magnitude of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which exit tangency magnitude is required. Index starts from zero.</param>
        public double GetExitMagnitudeOfPoint(int pointNumber)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) + "].EXIT_MAGNITUDE");
        }

        /// <summary>
        /// Gets the azimuth angle of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which the entry tangency azimuth angle is required. Index starts from zero.</param>
        public double GetEntryAzimuthAngleOfPoint(int pointNumber)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) +
                                               "].ENTRY_TANGENT.AZIMUTH");
        }

        /// <summary>
        /// Gets the elevation angle of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which the exit tangency elevation angle is required. Index starts from zero.</param>
        public double GetEntryElevationAngleOfPoint(int pointNumber)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) +
                                               "].ENTRY_TANGENT.ELEVATION");
        }

        /// <summary>
        /// Gets the azimuth angle of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which the exit tangency azimuth angle is required. Index starts from zero.</param>
        public double GetExitAzimuthAngleOfPoint(int pointNumber)
        {
            return _powerSHAPE.ReadDoubleValue(
                Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) + "].EXIT_TANGENT.AZIMUTH");
        }

        /// <summary>
        /// Gets the elevation angle of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">Point at which the exit tangency elevation angle is required. Index starts from zero.</param>
        public double GetExitElevationAngleOfPoint(int pointNumber)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POINT[" + (pointNumber + 1) +
                                               "].EXIT_TANGENT.ELEVATION");
        }

        /// <summary>
        /// Edits the entry and exit tangencies at a defined point.
        /// </summary>
        /// <param name="pointNumber">Point at which to carry out the operation. Index starts from zero.</param>
        /// <param name="entryTangent">Desired entry tangent. If this is not to be changed, pass in the current entry tangent.</param>
        /// <param name="exitTangent">Desired exit tangent. If this is not to be changed, pass in the current exit tangent.</param>
        public void EditCurvePointTangency(int pointNumber, Vector entryTangent, Vector exitTangent)
        {
            //' Select point
            SelectCurvePoints(new[] {pointNumber});

            //' Raise tangency editor form
            _powerSHAPE.DoCommand("TANGENT");

            //' Edit entry tangency if necessary
            if (entryTangent != GetEntryTangentOfPoint(pointNumber))
            {
                _powerSHAPE.DoCommand("TAN_BEFORE");
                _powerSHAPE.DoCommand("TAN_DIR DIRECTION");

                //' Start by setting all coodinates to be 1, so that there is never an instance where all are 0
                _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");

                //' Set tangent
                _powerSHAPE.DoCommand("X " + entryTangent.I.ToString("0.######"),
                                      "Y " + entryTangent.J.ToString("0.######"),
                                      "Z " + entryTangent.K.ToString("0.######"));
                _powerSHAPE.DoCommand("ACCEPT");
            }

            //' Edit exit tangency if necessary
            if (exitTangent != GetExitTangentOfPoint(pointNumber))
            {
                _powerSHAPE.DoCommand("TAN_AFTER");
                _powerSHAPE.DoCommand("TAN_DIR DIRECTION");

                //' Start by setting all coodinates to be 1, so that there is never an instance where all are 0
                _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");

                //' Set tangent
                _powerSHAPE.DoCommand("X " + exitTangent.I.ToString("0.######"),
                                      "Y " + exitTangent.J.ToString("0.######"),
                                      "Z " + exitTangent.K.ToString("0.######"));
                _powerSHAPE.DoCommand("ACCEPT");
            }

            //' Close form
            _powerSHAPE.DoCommand("ACCEPT");
        }

        /// <summary>
        /// Edits the entry and exit tangency magnitudes at a defined point.
        /// </summary>
        /// <param name="pointNumber">Point at which to carry out the operation. Index starts from zero.</param>
        /// <param name="entryMagnitude">Desired entry tangent. If this is not to be changed, pass in the current entry tangent.</param>
        /// <param name="exitMagnitude">Desired exit tangent. If this is not to be changed, pass in the current exit tangent.</param>
        public void EditCurvePointTangencyMagnitude(int pointNumber, double entryMagnitude, double exitMagnitude)
        {
            //' Select point
            SelectCurvePoints(new[] {pointNumber});

            //' Raise tangency editor form
            _powerSHAPE.DoCommand("TANGENT");

            //' Edit entry tangency if necessary
            if (entryMagnitude != GetEntryMagnitudeOfPoint(pointNumber))
            {
                _powerSHAPE.DoCommand("MAG_BEFORE");
                _powerSHAPE.DoCommand("MAGNITUDE " + entryMagnitude);
            }

            //' Edit exit tangency if necessary
            if (exitMagnitude != GetExitMagnitudeOfPoint(pointNumber))
            {
                _powerSHAPE.DoCommand("MAG_AFTER");
                _powerSHAPE.DoCommand("MAGNITUDE " + exitMagnitude);
            }

            //' Close form
            _powerSHAPE.DoCommand("ACCEPT");
        }

        #endregion

        #region "Dependencies Operations"

        /// <summary>
        /// Deletes any dependencies.
        /// Useful when trying to duplicate a curve which is dependent on a mesh.
        /// It must be added to the selection for this to work!
        /// </summary>
        public virtual void DeleteDependencies()
        {
            _powerSHAPE.DoCommand("DELETE_DEPENDENCIES");
        }

        #endregion

        #endregion
    }
}