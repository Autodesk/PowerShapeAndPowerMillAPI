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
using Autodesk.Extensions;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Base class for Surface Curves.
    /// </summary>
    public abstract class PSSurfaceCurve : PSWireframe
    {
        #region " Fields "

        /// <summary>
        /// The parent surface of the Surface Curve.
        /// </summary>
        /// <remarks></remarks>
        protected PSSurface _surface;

        /// <summary>
        /// The internal ID of the Surface Curve.
        /// </summary>
        /// <remarks></remarks>
        protected Guid _internalId;

        #endregion

        #region " Properties "

        /// <summary>
        /// Overrides Exists() from DatabaseEntity as a surface curve must be analysed using its
        /// name. If the surface curve has been deleted, its name will now be 0 and always return False
        /// </summary>
        /// <returns>Boolean indicating whether this surface curve is on the specified surface.</returns>
        public override bool Exists
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[" + Name + "].EXISTS") == 1; }
        }

        /// <summary>
        /// Gets/Sets the unique internal ID of the surface curve.
        /// </summary>
        /// <remarks>
        /// Because PowerSHAPE recalculates the ID of the surface curve every time,
        /// a unique identifier is needed within this system to determine whether the curve still exists etc.
        /// </remarks>
        internal Guid InternalId
        {
            get { return _internalId; }
            set { _internalId = value; }
        }

        /// <summary>
        /// Gets/Sets the level of the surface curve by getting/setting the level
        /// of its parent surface.
        /// </summary>
        public override PSLevel Level
        {
            get { return _surface.Level; }
            set { _surface.Level = value; }
        }

        /// <summary>
        /// Gets/Sets the name of the lateral or longitudinal.
        /// </summary>
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// BoundingBox property is not valid for this type
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get { throw new Exception("Property not valid for " + GetType()); }
        }

        /// <summary>
        /// Gets the length of the surface curve - overriden because the curve cannot be
        /// interrogated using its id.
        /// </summary>
        public override MM Length
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[" + Name + "].LENGTH"); }
        }

        /// <summary>
        /// Gets the number of points on the surface curve - overriden because the curve cannot be
        /// interrogated using its id.
        /// </summary>
        public override int NumberPoints
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[" + Name + "].NUMBER"); }
        }

        /// <summary>
        /// Gets the centre of gravity of the surface curve - overriden because the curve cannot be
        /// interrogated using its id.
        /// </summary>
        public override Point CentreOfGravity
        {
            get
            {
                double[] cogFromPowerSHAPE = null;
                cogFromPowerSHAPE = _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].COG") as double[];

                return new Point(cogFromPowerSHAPE);
            }
        }

        /// <summary>
        /// Gets the points on the surface curve.
        /// </summary>
        public override List<Point> Points
        {
            get
            {
                AbortIfDoesNotExist();

                List<Point> surfaceCurvePoints = new List<Point>();
                for (int index = 1; index <= NumberPoints; index++)
                {
                    double[] coordinates = null;
                    coordinates = _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + index + "]") as double[];
                    Point point = new Point(coordinates[0], coordinates[1], coordinates[2]);
                    surfaceCurvePoints.Add(point);
                }

                return surfaceCurvePoints;
            }
        }

        /// <summary>
        /// Gets the start point of the surface curve - overriden because the curve cannot be
        /// interrogated using its id.
        /// </summary>
        public override Point StartPoint
        {
            get
            {
                double[] startPointFromPowerSHAPE = null;
                startPointFromPowerSHAPE = _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].START") as double[];

                return new Point(startPointFromPowerSHAPE);
            }
        }

        /// <summary>
        /// Gets the end point of the surface curve - overriden because the curve cannot be
        /// interrogated using its id.
        /// </summary>
        public override Point EndPoint
        {
            get
            {
                double[] endPointFromPowerSHAPE = null;
                endPointFromPowerSHAPE = _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].END") as double[];

                return new Point(endPointFromPowerSHAPE);
            }
        }

        /// <summary>
        /// Gets the short name of the surface curve for use in macros.
        /// </summary>
        internal abstract string ShortType { get; }

        /// <summary>
        /// Always returns 0, as a surface curve can only ever be opaque.
        /// </summary>
        public override double Transparency
        {
            get { return 0; }
            set
            {
                if (value != 0)
                {
                    throw new ApplicationException("Surface curve transparency can only ever be 1");
                }
            }
        }

        /// <summary>
        /// Gets the parent surface of the surface curve.
        /// </summary>
        public PSSurface ParentSurface
        {
            get { return _surface; }
        }

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connects to PowerShape.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="surface">The parent surface.</param>
        /// <remarks></remarks>
        internal PSSurfaceCurve(PSAutomation powerSHAPE, PSSurface surface) : base(powerSHAPE)
        {
            _powerSHAPE = powerSHAPE;
            _surface = surface;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Overrides Equals() from DatabaseEntity as the surface curve's name/ID can be changed since
        /// the surface curve was initialised. Therefore its unique internal ID is used for comparison.
        /// </summary>
        /// <param name="obj">The other PSSurfaceCurve to compare to.</param>
        /// <returns>Returns a boolean indicating whether the two objects are equal.</returns>
        /// <remarks></remarks>
        public override bool Equals(object obj)
        {
            if (obj is PSSurfaceCurve)
            {
                return (_internalId == ((PSSurfaceCurve) obj).InternalId) & (Identifier == ((PSSurfaceCurve) obj).Identifier);
            }
            return false;
        }

        /// <summary>
        /// Deletes the surface curve from the surface. It is always overriden by the Lateral and Longitudinal classes
        /// as the other surface curve IDs will have to be updated afterward.
        /// </summary>
        public override void Delete()
        {
            // Add to selection
            AddToSelection(true);

            // Carry out operation
            _powerSHAPE.DoCommand("DELETE_CURVE");

            // Set the name to be 0 to identify that this curve has been deleted
            _name = "0";
        }

        /// <summary>
        /// Calls the parent surface's AddSurfaceCurveToSelection operation.
        /// </summary>
        /// <param name="emptySelectionFirst">Determines whether previous surface curve additions are to be disregarded.</param>
        public override void AddToSelection(bool emptySelectionFirst = false)
        {
            // Add surface to selection. If set to empty selection first, only need to add the
            // surface.  If adding surface curves to a selection already containing surface curves,
            // check that the surface is the only item selected
            if (emptySelectionFirst)
            {
                _surface.AddToSelection(true);
            }
            else
            {
                if ((_powerSHAPE.ActiveModel.SelectedItems.Contains(_surface) == false) |
                    (_powerSHAPE.ActiveModel.SelectedItems.Count != 1))
                {
                    _surface.AddToSelection(true);
                }
            }

            // Check that the surface curve exists
            if (Exists == false)
            {
                throw new ApplicationException("Surface curve does not exist");
            }

            // Add surface curve
            _powerSHAPE.DoCommand("SELECT_" + ShortType + "S " + _name);
        }

        /// <summary>
        /// Gets the length of curve between two key points.
        /// </summary>
        /// <param name="pointA">Single representing the chosen start keypoint on the curve. Index starts from zero.</param>
        /// <param name="pointB">Single representing the chosen end keypoint on the curve. Index starts from zero.</param>
        public MM GetLengthBetweenTwoPoints(float pointA, float pointB)
        {
            return (MM) _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].LENGTH_BETWEEN(" + (pointA + 1) + ";" +
                                                (pointB + 1) +
                                                ")");
        }

        /// <summary>
        /// Offsets the surface curve by the specified amount.
        /// </summary>
        /// <param name="distanceToOffset">Distance to offset the curve.</param>
        /// <param name="discontinuityBehaviour">Determines whether the curve should be split at discontinuities.</param>
        /// <param name="applyCurveSmoothing">If true, activates the 'SMOOTH ON' option in PowerShape. If false, activates 'SMOOTH OFF'.</param>
        /// <remarks></remarks>
        public void Offset(float distanceToOffset, OffsetSplitAtDiscontinuity discontinuityBehaviour, bool applyCurveSmoothing)
        {
            // Add surface curve
            AddToSelection(true);

            // Carry out operation
            if (applyCurveSmoothing)
            {
                _powerSHAPE.DoCommand("SMOOTH ON");
            }
            else
            {
                _powerSHAPE.DoCommand("SMOOTH OFF");
            }
            _powerSHAPE.DoCommand("EDIT SUBEDITS ON");
            _powerSHAPE.DoCommand("EDIT OFFSET");
            _powerSHAPE.DoCommand(discontinuityBehaviour.ToString());
            _powerSHAPE.DoCommand("DISTANCE " + distanceToOffset);
            _powerSHAPE.DoCommand("EDIT OFFSET OFF");
            _powerSHAPE.DoCommand("EDIT SUBEDITS OFF");
        }

        /// <summary>
        /// Moves the indicated surface points by a vector.
        /// </summary>
        /// <param name="moveVector">Vector along which to move the points.</param>
        /// <param name="pointNumbersToMove">Points to move. Index starts from zero.</param>
        public void MoveSurfacePointsAlongVector(Vector moveVector, int[] pointNumbersToMove)
        {
            // Select the points to move
            SelectCurvePoints(pointNumbersToMove);

            // Move the points
            _powerSHAPE.DoCommand("EDIT MOVE");

            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("X " + moveVector.I.ToString("0.######"),
                                      "Y " + moveVector.J.ToString("0.######"),
                                      "Z " + moveVector.K.ToString("0.######"));
                _powerSHAPE.DoCommand("APPLY", "DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand(moveVector.ToString());
                _powerSHAPE.DoCommand("EDIT MOVE OFF");
            }
        }

        /// <summary>
        /// Returns a composite curve of the surface curve.
        /// </summary>
        /// <returns>A new composite curve.</returns>
        public PSCompCurve CreateCompositeCurve()
        {
            // Select the surface curve
            AddToSelection(true);

            // Create the compcurve
            _powerSHAPE.DoCommand("CONVERT WIREFRAME");

            // Get the create curve
            PSCompCurve createdCurve = (PSCompCurve) _powerSHAPE.ActiveModel.CreatedItems[0];
            _powerSHAPE.ActiveModel.Add(createdCurve);
            return createdCurve;
        }

        #region " Point Operations "

        /// <summary>
        /// Selects the indicated points on the surface curve.
        /// </summary>
        /// <param name="pointsToSelect">Point numbers to be selected. Index for the points is zero based.</param>
        public void SelectCurvePoints(int[] pointsToSelect)
        {
            // Add surface curve
            AddToSelection(true);

            // Get point numbers to select
            string pointNumbers = (pointsToSelect[0] + 1).ToString();
            if (pointsToSelect.Count() > 1)
            {
                for (int i = 1; i <= pointsToSelect.Count() - 1; i++)
                {
                    pointNumbers += " " + (pointsToSelect[i] + 1);
                }
            }

            // Select points
            _powerSHAPE.DoCommand("SELECT_POINTS " + pointNumbers);
        }

        /// <summary>
        /// Offsets the indicated points on the surface curve.
        /// </summary>
        /// <param name="distanceToOffset">Distance to offset points.</param>
        /// <param name="discontinuityBehaviour">OffsetSplitAtDiscontinuity object for handling SPLITON/OFF.</param>
        /// <param name="applyCurveSmoothing">Boolean to apply curve smoothing.</param>
        /// <param name="pointNumbersToOffset">Array of points to offset. Index starts from zero.</param>
        /// <remarks></remarks>
        public void OffsetPointsOnCurve(
            float distanceToOffset,
            OffsetSplitAtDiscontinuity discontinuityBehaviour,
            bool applyCurveSmoothing,
            int[] pointNumbersToOffset)
        {
            // Add surface curve points
            SelectCurvePoints(pointNumbersToOffset);

            // Carry out operation
            if (applyCurveSmoothing)
            {
                _powerSHAPE.DoCommand("SMOOTH ON");
            }
            else
            {
                _powerSHAPE.DoCommand("SMOOTH OFF");
            }
            _powerSHAPE.DoCommand("EDIT SUBEDITS ON");
            _powerSHAPE.DoCommand("EDIT OFFSET");
            _powerSHAPE.DoCommand(discontinuityBehaviour.ToString());
            _powerSHAPE.DoCommand("DISTANCE " + distanceToOffset);
            _powerSHAPE.DoCommand("EDIT OFFSET OFF");
            _powerSHAPE.DoCommand("EDIT SUBEDITS OFF");
        }

        #endregion

        #region " Tangency Interrogations "

        /// <summary>
        /// Gets the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which entry tangency is required. Index starts from zero.</param>
        /// <returns>Entry tangent vector.</returns>
        public Vector GetEntryTangentOfPoint(int pointNumber)
        {
            double[] entryTangent =
                _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_TANGENT") as double[];
            return new Vector(entryTangent[0], entryTangent[1], entryTangent[2]);
        }

        /// <summary>
        /// Gets the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which exit tangency is required. Index starts from zero.</param>
        /// <returns>Exit tangent vector.</returns>
        public Vector GetExitTangentOfPoint(int pointNumber)
        {
            double[] exitTangent =
                _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_TANGENT") as double[];
            return new Vector(exitTangent[0], exitTangent[1], exitTangent[2]);
        }

        /// <summary>
        /// Gets the magnitude of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which entry tangency magnitude is required. Index starts from zero.</param>
        /// <returns>Entry tangent magnitude.</returns>
        public double GetEntryMagnitudeOfPoint(int pointNumber)
        {
            double entryMagnitude = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_MAGNITUDE");
            return entryMagnitude;
        }

        /// <summary>
        /// Gets the magnitude of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which exit tangency magnitude is required. Index starts from zero.</param>
        /// <returns>Exit tangent magnitude.</returns>
        public double GetExitMagnitudeOfPoint(int pointNumber)
        {
            double exitMagnitude = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_MAGNITUDE");
            return exitMagnitude;
        }

        /// <summary>
        /// Gets the azimuth angle of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the entry tangency azimuth angle is required. Index starts from zero.</param>
        /// <returns>Entry azimuth.</returns>
        public double GetEntryAzimuthAngleOfPoint(int pointNumber)
        {
            double entryAzimuth = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_TANGENT.AZIMUTH");
            return entryAzimuth;
        }

        /// <summary>
        /// Gets the elevation angle of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the exit tangency elevation angle is required. Index starts from zero.</param>
        /// <returns>Entry elevation.</returns>
        public double GetEntryElevationAngleOfPoint(int pointNumber)
        {
            double entryElevation = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_TANGENT.ELEVATION");
            return entryElevation;
        }

        /// <summary>
        /// Gets the azimuth angle of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the exit tangency azimuth angle is required. Index starts from zero.</param>
        /// <returns>Exit azimuth.</returns>
        public double GetExitAzimuthAngleOfPOint(int pointNumber)
        {
            double exitAzimuth = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_TANGENT.AZIMUTH");
            return exitAzimuth;
        }

        /// <summary>
        /// Gets the elevation angle of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the exit tangency elevation angle is required. Index starts from zero.</param>
        /// <returns>Exit elevation.</returns>
        public double GetExitElevationAngleOfPoint(int pointNumber)
        {
            double exitElevation = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_TANGENT.ELEVATION");
            return exitElevation;
        }

        /// <summary>
        /// Gets the entry normal of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which entry normal is required. Index starts from zero.</param>
        /// <returns>Entry normal vector.</returns>
        public Vector GetEntryNormalOfPoint(int pointNumber)
        {
            double[] entryNormal =
                _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_NORMAL") as double[];
            return new Vector(entryNormal[0], entryNormal[1], entryNormal[2]);
        }

        /// <summary>
        /// Gets the exit normal of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which exit normal is required. Index starts from zero.</param>
        /// <returns>Exit normal vector.</returns>
        public Vector GetExitNormalOfPoint(int pointNumber)
        {
            double[] exitNormal =
                _powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_NORMAL") as double[];
            return new Vector(exitNormal[0], exitNormal[1], exitNormal[2]);
        }

        /// <summary>
        /// Gets the flare angle of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the entry tangency flare angle is required. Index starts from zero.</param>
        /// <returns>Entry flare.</returns>
        public double GetEntryFlareAngleOfPoint(int pointNumber)
        {
            double entryFlare = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_TANGENT.FLARE");
            return entryFlare;
        }

        /// <summary>
        /// Gets the twist angle of the entry tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the exit tangency twist angle is required. Index starts from zero.</param>
        /// <returns>Entry twist.</returns>
        public double GetEntryTwistAngleOfPoint(int pointNumber)
        {
            double entryTwist = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].ENTRY_TANGENT.TWIST");
            return entryTwist;
        }

        /// <summary>
        /// Gets the flare angle of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the exit tangency flare angle is required. Index starts from zero.</param>
        /// <returns>Exit flare.</returns>
        public double GetExitFlareAngleOfPoint(int pointNumber)
        {
            double exitFlare = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_TANGENT.FLARE");
            return exitFlare;
        }

        /// <summary>
        /// Gets the twist angle of the exit tangent of the desired point.
        /// </summary>
        /// <param name="pointNumber">The point at which the exit tangency twist angle is required. Index starts from zero.</param>
        /// <returns>Exit twist.</returns>
        public double GetExitTwistAngleOfPoint(int pointNumber)
        {
            double exitTwist = (double)_powerSHAPE.DoCommandEx(Identifier + "[" + Name + "].POINT[" + (pointNumber + 1) + "].EXIT_TANGENT.TWIST");
            return exitTwist;
        }

        #endregion

        #region "Edit Tangency Operations"

        /// <summary>
        /// Edits the after tangent at the selected point.
        /// </summary>
        /// <param name="pointNumber">The number of the point to select. Index starts from zero.</param>
        /// <param name="afterTangentVector">The control point for the after tangent.</param>
        /// <remarks>The tangents at the selected points is set along the curve.</remarks>
        public void EditAfterTangencyOfPoint(int pointNumber, Vector afterTangentVector)
        {
            SelectCurvePoints(new[] {pointNumber});

            _powerSHAPE.DoCommand("TANGENT");
            _powerSHAPE.DoCommand("DIR_ALONG");

            _powerSHAPE.DoCommand("TAN_AFTER");
            _powerSHAPE.DoCommand("TAN_DIR DIRECTION");

            //' Start by setting all coodinates to be 1, so that there is never an instance where all are 0
            _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");

            //' Set tangent
            _powerSHAPE.DoCommand("X " + afterTangentVector.I.ToString("0.######"),
                                  "Y " + afterTangentVector.J.ToString("0.######"),
                                  "Z " + afterTangentVector.K.ToString("0.######"));

            _powerSHAPE.DoCommand("ACCEPT");
            _powerSHAPE.DoCommand("ACCEPT");
        }

        /// <summary>
        /// Edits the before tangent at the selected point.
        /// </summary>
        /// <param name="pointNumber">The number of the point to select. Index starts from zero.</param>
        /// <param name="beforeTangentVector">The control point for the before tangent.</param>
        /// <remarks>The tangents at the selected points is set along the curve.</remarks>
        public void EditBeforeTangencyOfPoint(int pointNumber, Vector beforeTangentVector)
        {
            SelectCurvePoints(new[] {pointNumber});

            _powerSHAPE.DoCommand("TANGENT");
            _powerSHAPE.DoCommand("DIR_ALONG");

            _powerSHAPE.DoCommand("TAN_BEFORE");
            _powerSHAPE.DoCommand("TAN_DIR DIRECTION");

            //' Start by setting all coodinates to be 1, so that there is never an instance where all are 0
            _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");

            //' Set tangent
            _powerSHAPE.DoCommand("X " + beforeTangentVector.I.ToString("0.######"),
                                  "Y " + beforeTangentVector.J.ToString("0.######"),
                                  "Z " + beforeTangentVector.K.ToString("0.######"));

            _powerSHAPE.DoCommand("ACCEPT");
            _powerSHAPE.DoCommand("ACCEPT");
        }

        /// <summary>
        /// Edits the entry and exit tangencies at a defined point.
        /// </summary>
        /// <param name="pointNumber">The point at which to carry out the operation. Index starts from zero.</param>
        /// <param name="entryTangent">The desired entry tangent. If this is not to be changed, pass in the current entry tangent.</param>
        /// <param name="exitTangent">The desired exit tangent. If this is not to be changed, pass in the current exit tangent.</param>
        public void EditTangencyOfPoint(int pointNumber, Vector entryTangent, Vector exitTangent)
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
        /// Frees the magnitudes and directions of the entire wireframe.
        /// </summary>
        public void FreeMagnitudesAndDirections()
        {
            AddToSelection(true);
            _powerSHAPE.DoCommand("FREEMAGDIRS");
        }

        #endregion

        #region " Edit Operations "

        /// <summary>
        /// Moves a single surfacecurve by the relative distance between two absolute positions.
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions.</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions.</param>
        public void Move(Point moveOriginCoordinates, Point pointToMoveTo)
        {
            // Because the code is different, a surface curve does not implement IMoveable
            // Select surface curve
            AddToSelection(true);

            // Move curve
            _powerSHAPE.DoCommand("EDIT SUBEDITS ON");
            _powerSHAPE.DoCommand("EDIT MOVE");
            _powerSHAPE.DoCommand("MOVEORIGIN", moveOriginCoordinates.ToString());
            _powerSHAPE.DoCommand("ABS " + pointToMoveTo);
            _powerSHAPE.DoCommand("EDIT MOVE OFF");
        }

        /// <summary>
        /// Moves a single surfacecurve by a specified relative amount.
        /// </summary>
        /// <param name="moveVector">The relative amount by which the surfacecurve will be moved.</param>
        public void Move(Vector moveVector)
        {
            // Because the code is different, a surface curve does not implement IMoveable
            // Select surface curve
            AddToSelection(true);

            // Move curve
            _powerSHAPE.DoCommand("EDIT SUBEDITS ON");
            _powerSHAPE.DoCommand("EDIT MOVE");

            // Do different code if PowerSHAPE version is higher
            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("X " + moveVector.I.ToString("0.######"),
                                      "Y " + moveVector.J.ToString("0.######"),
                                      "Z " + moveVector.K.ToString("0.######"));
                _powerSHAPE.DoCommand("APPLY", "DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand(moveVector.I.ToString("0.######") + " " + moveVector.J.ToString("0.######") + " " + moveVector.K.ToString("0.######"));
                _powerSHAPE.DoCommand("CANCEL");
            }

            _powerSHAPE.DoCommand("EDIT MOVE OFF");
        }

        /// <summary>
        /// Rotates a single surface curve by a specified angle around a specified axis.
        /// </summary>
        /// <param name="rotationAxis">The axis around which the surface curve is are to be rotated.</param>
        /// <param name="rotationAngle">The angle by which the surface curve is to be rotated.</param>
        /// <param name="rotationOrigin">The origin of the rotation axis.</param>
        public void Rotate(Axes rotationAxis, Degree rotationAngle, Point rotationOrigin = null)
        {
            // Because the code is different, the surface curve does not implement IRotateable
            // Carry out operation
            _powerSHAPE.DoCommand("EDIT SUBEDITS ON");
            _powerSHAPE.DoCommand("EDIT ROTATE");

            // If a different rotation origin has been defined, set it within PowerSHAPE
            if (rotationOrigin != null)
            {
                _powerSHAPE.DoCommand("AXIS");
                _powerSHAPE.DoCommand(rotationOrigin.ToString());
            }

            // Set the active plane, which determines the axis of rotation
            _powerSHAPE.SetActivePlane(rotationAxis.AxisToPlane());

            // Enter rotation angle
            _powerSHAPE.DoCommand("ANGLE " + rotationAngle);
            if (_powerSHAPE.Version >= new Version("11.2"))
            {
                _powerSHAPE.DoCommand("APPLY", "DISMISS");
            }
            else
            {
                _powerSHAPE.DoCommand("CANCEL");
            }
        }

        #endregion

        #endregion
    }
}