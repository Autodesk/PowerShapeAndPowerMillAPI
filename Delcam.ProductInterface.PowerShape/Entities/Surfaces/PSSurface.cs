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
    /// Captures an Surface object in PowerSHAPE
    /// </summary>
    public class PSSurface : PSEntity, IPSMoveable, IPSRotateable, IPSMirrorable, IPSLimitable, IPSAddable, IPSIntersectable,
                             IPSSubtractable
    {
        #region " Fields "

        private SurfaceTypes? _surfaceType;
        private PSMaterial _material;
        private PSLateralsCollection _laterals;

        private PSLongitudinalsCollection _longitudinals;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connects to PowerSHAPE
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <remarks></remarks>
        internal PSSurface(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
            _laterals = new PSLateralsCollection(_powerSHAPE, this);
            _longitudinals = new PSLongitudinalsCollection(_powerSHAPE, this);
        }

        /// <summary>
        /// Initialises the Surface.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="id">The surface ID.</param>
        /// <param name="name">The surface name.</param>
        /// <remarks></remarks>
        internal PSSurface(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
            _laterals = new PSLateralsCollection(_powerSHAPE, this);
            _longitudinals = new PSLongitudinalsCollection(_powerSHAPE, this);
        }

        #endregion

        #region " Properties "

        internal const string SURFACE_IDENTIFIER = "SURFACE";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return SURFACE_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object.
        /// </summary>
        internal override int CompositeID
        {
            get { return 21 * 20000000 + _id; }
        }

        /// <summary>
        /// Gets the command that will be used to rename the Surface.
        /// </summary>
        internal override string RenameCommand
        {
            get
            {
                if ((Type != SurfaceTypes.PowerSurface) & (Type != SurfaceTypes.NURB))
                {
                    return "MODIFY";
                }
                return "RENAME";
            }
        }

        /// <summary>
        /// Gets the type of the Surface.
        /// </summary>
        public SurfaceTypes Type
        {
            get
            {
                AbortIfDoesNotExist();

                if (_surfaceType == null)
                {
                    _surfaceType = GetSurfaceType(_powerSHAPE, _id);
                }
                return (SurfaceTypes) _surfaceType;
            }
        }

        /// <summary>
        /// Gets and sets the Trim status of the Surface.
        /// </summary>
        public bool IsTrimmed
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].TRIMMED");
            }

            set
            {
                // Set trim command
                string command = "OFF";
                if (value)
                {
                    command = "ON";
                }

                // Add to selection
                AddToSelection(true);

                // Change trim status
                _powerSHAPE.DoCommand("TRIM " + command);
            }
        }

        /// <summary>
        /// Gets the Trim validity of the Surface.
        /// </summary>
        public bool IsTrimmingValid
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].TRIMMING_VALID");
            }
        }

        /// <summary>
        /// Gets the direction of the Surface.
        /// </summary>
        public Vector Direction
        {
            get
            {
                AbortIfDoesNotExist();
                double[] psDirection = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].DIRECTION") as double[];
                return new Vector(psDirection[0], psDirection[1], psDirection[2]);
            }
        }

        /// <summary>
        /// Gets the area of the Surface.
        /// </summary>
        public double Area
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].AREA");
            }
        }

        /// <summary>
        /// Gets the volume of the Surface.
        /// </summary>
        public virtual double Volume
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].VOLUME");
            }
        }

        /// <summary>
        /// Gets the primitive shape of the Surface.
        /// </summary>
        public virtual SurfaceShapes Shape
        {
            get
            {
                AbortIfDoesNotExist();
                string psShape = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].PRIMITIVE_TYPE");

                // Remove any spaces
                psShape = psShape.Replace(" ", "_");

                // Turn boxes into blocks
                if (psShape == "Box")
                {
                    psShape = "Block";
                }

                return (SurfaceShapes) Enum.Parse(typeof(SurfaceShapes), psShape, true);
            }
        }

        /// <summary>
        /// Gets the coordinates of the centre of gravity of the Surface.
        /// </summary>
        public Point CentreOfGravity
        {
            get
            {
                AbortIfDoesNotExist();
                double[] psCoords = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].COG") as double[];
                return new Point(psCoords[0], psCoords[1], psCoords[2]);
            }
        }

        /// <summary>
        /// Gets the number of laterals on the surface.
        /// </summary>
        public int NumberOfLaterals
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].NLATS");
            }
        }

        /// <summary>
        /// Gets the number of longitudinals on the surface.
        /// </summary>
        public int NumberOfLongitudinals
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].NLONS");
            }
        }

        /// <summary>
        /// Gets and sets whether the Surface laterals are closed.
        /// </summary>
        public bool IsClosedLaterals
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].LAT_CLOSED");
            }

            set
            {
                // Set command
                string command = "OPEN";
                if (value)
                {
                    command = "CLOSE";
                }

                // Select a lateral
                SelectSurfaceCurves(SurfaceCurveTypes.Lateral, new[] {1});

                // Carry out command
                _powerSHAPE.DoCommand(command);
            }
        }

        /// <summary>
        /// Gets and sets whether the Surface longitudinals are closed.
        /// </summary>
        public bool IsClosedLongitudinals
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].LON_CLOSED");
            }

            set
            {
                // Set command
                string command = "OPEN";
                if (value)
                {
                    command = "CLOSE";
                }

                // Select a longitudinal
                SelectSurfaceCurves(SurfaceCurveTypes.Longitudinal, new[] {1});

                // Carry out command
                _powerSHAPE.DoCommand(command);
            }
        }

        /// <summary>
        /// Gets whether the surface has a spine.
        /// </summary>
        public bool HasSpine
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].SPINE.EXISTS");
            }
        }

        /// <summary>
        /// Gets the number of boundaries on the surface.
        /// </summary>
        public int NumberOfBoundaries
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].BOUNDARIES");
            }
        }

        /// <summary>
        /// Gets the number of pCurves on the surface.
        /// </summary>
        public int NumberOfPCurves
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].PCURVES");
            }
        }

        /// <summary>
        /// Gets the list of laterals on the surface.
        /// </summary>
        public PSLateralsCollection Laterals
        {
            get
            {
                AbortIfDoesNotExist();

                // Populate list if it's empty
                if (_laterals.Count == 0)
                {
                    for (int i = 1; i <= NumberOfLaterals; i++)
                    {
                        PSLateral lateral = new PSLateral(_powerSHAPE, i.ToString(), this);
                        _laterals.Add(lateral);
                    }
                }

                return _laterals;
            }
        }

        /// <summary>
        /// Gets the list of longitudinals on the surface.
        /// </summary>
        public PSLongitudinalsCollection Longitudinals
        {
            get
            {
                AbortIfDoesNotExist();

                // Populate list if it's empty
                if (_longitudinals.Count == 0)
                {
                    for (int i = 1; i <= NumberOfLongitudinals; i++)
                    {
                        PSLongitudinal longitudinal = new PSLongitudinal(_powerSHAPE, i.ToString(), this);
                        _longitudinals.Add(longitudinal);
                    }
                }

                return _longitudinals;
            }
        }

        /// <summary>
        /// Gets a list of the PCurves on the surface.
        /// </summary>
        public List<PSpCurve> PCurves
        {
            get
            {
                AbortIfDoesNotExist();
                List<PSpCurve> allPCurves = new List<PSpCurve>();
                for (int i = 1; i <= NumberOfPCurves; i++)
                {
                    string pCurveName = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].PCURVE[" + i + "]");
                    allPCurves.Add(new PSpCurve(_powerSHAPE, this, pCurveName));
                }

                return allPCurves;
            }
        }

        /// <summary>
        /// Gets and Sets the material of the Surface in PowerSHAPE.
        /// </summary>
        public PSMaterial Material
        {
            get
            {
                // Check first that the material still exists
                if (_powerSHAPE.ActiveModel.Materials.Contains(_material))
                {
                    return _material;
                }
                return null;
            }

            set
            {
                _material = value;
                AddToSelection(true);
                _powerSHAPE.DoCommand("FORMAT MATERIAL", "CUSTOM", "SELECT " + _material.Name, "APPLY", "ACCEPT", "ACCEPT");
            }
        }

        #endregion

        #region " Static Property Methods "

        /// <summary>
        /// Gets the surface type, so removing the code from the property and removing the
        /// chance that the property can throw an exception.
        /// </summary>
        /// <param name="powerSHAPE">The current instance of PowerSHAPE.</param>
        /// <param name="id">The ID of the surface to interrogate.</param>
        /// <returns>The type of the surface as a SurfaceType.</returns>
        public static SurfaceTypes GetSurfaceType(PSAutomation powerSHAPE, int id)
        {
            string psType = powerSHAPE.ReadStringValue("SURFACE[ID " + id + "].TYPE");
            return (SurfaceTypes) Enum.Parse(typeof(SurfaceTypes), psType, true);
        }

        #endregion

        #region " Analysis "

        /// <summary>
        /// Gets the coordinates of a position on the surface defined by TU
        /// paremeters.
        /// </summary>
        /// <param name="t">The t parameter of the chosen surface position.</param>
        /// <param name="u">The u parameter of the chosen surface position.</param>
        /// <returns>A point representing the coordinates of the chosen position.</returns>
        public Point GetSurfacePoint(double t, double u)
        {
            double[] psPoint =
                _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].EVALUATE(" + t + ";" + u + ").POSITION") as double[];
            return new Point(psPoint[0], psPoint[1], psPoint[2]);
        }

        /// <summary>
        /// Gets the coordinates of a position on the surface defined byb TU parameters.
        /// </summary>
        /// <param name="TUParameters">A double array containing the TU parameters of the chosen surface position.</param>
        /// <returns>A point representing the coordinates of the chosen position.</returns>
        public Point GetSurfacePoint(double[] TUParameters)
        {
            return GetSurfacePoint(TUParameters[0], TUParameters[1]);
        }

        /// <summary>
        /// Gets the nearest TU parameters to an external point.
        /// </summary>
        /// <param name="referencePoint">Reference point that can lie anywhere in space.</param>
        /// <param name="guessT">Optional estimate T to speed up calculation times.</param>
        /// <param name="guessU">Optional estimate U to speed up calculation times.</param>
        /// <returns></returns>
        public double[] GetNearestTUParameters(Point referencePoint, float guessT = 0, float guessU = 0)
        {
            // Get the guess parameters string
            string guessParameters = null;
            if ((guessT != 0) & (guessU != 0))
            {
                guessParameters = ";" + guessT + ";" + guessU;
            }
            else
            {
                guessParameters = "";
            }

            double[] psParameters = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].NEAR(" +
                                                            referencePoint.X.ToString("0.######") +
                                                            "; " + referencePoint.Y.ToString("0.######") + "; " +
                                                            referencePoint.Z.ToString("0.######") + guessParameters + ")") as double[];

            // Remove the 3rd item in the array output from PowerSHAPE, which will always be 0
            Array.Resize(ref psParameters, 2);
            return psParameters;
        }

        /// <summary>
        /// Gets the normal of the Surface at the specified point (default is centre of surface).
        /// </summary>
        /// <param name="t">T coordinate of point on surface.</param>
        /// <param name="u">U coordinate of point on surface.</param>
        public Vector GetNormal(float t = 0, float u = 0)
        {
            AbortIfDoesNotExist();

            // Check whether user has entered tu parameters
            if (t == 0)
            {
                // Get tu parameters of approximate surface centre
                t = (float) (NumberOfLaterals / 2 + 0.5);
                u = (float) (NumberOfLongitudinals / 2 + 0.5);
            }

            double[] surfaceNormal =
                _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].EVALUATE(" + t + "; " + u + ").NORMAL") as double[];
            return new Vector(surfaceNormal[0], surfaceNormal[1], surfaceNormal[2]);
        }

        /// <summary>
        /// Gets the normal of the surface a the point nearest to the specified reference point
        /// </summary>
        /// <param name="referencePoint">The reference point to find the nearest point to</param>
        /// <returns>The normal of the surface a the point nearest to the specified reference point</returns>
        public Vector GetNearestNormal(Point referencePoint)
        {
            double[] result = GetNearestTUParameters(referencePoint);
            double[] psPoint =
                _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].EVALUATE(" + result[0] + ";" + result[1] +
                                        ").NORMAL") as double[];
            return new Vector(psPoint[0], psPoint[1], psPoint[2]);
        }

        /// <summary>
        /// Gets the distance between this entity and another
        /// </summary>
        /// <param name="otherEntity">The function returns the distance between this entity and the otherEntity</param>
        /// <exception cref="Exception">Failed to determine distance between objects</exception>
        public MM DistanceTo(PSEntity otherEntity)
        {
            if (otherEntity is PSPoint)
            {
                Point entityPoint = ((PSPoint) otherEntity).ToPoint();
                Point nearest = NearestPoint(entityPoint);
                return (nearest - entityPoint).Magnitude;
            }
            return base.DistanceTo(otherEntity);
        }

        /// <summary>
        /// Gets the nearest point on the surface compared to the specified point
        /// </summary>
        /// <param name="point">The function returns the distance between this specified point and the surface</param>
        /// <exception cref="Exception">Failed to determine nearest point</exception>
        public Point NearestPoint(Point point)
        {
            try
            {
                // Get the nearest TU values then evaluate the position
                double[] psParameters = GetNearestTUParameters(point);
                double[] nearestPosition = _powerSHAPE.DoCommandEx(
                    Identifier + "[ID " + _id + "].EVALUATE(" + psParameters[0] + "; " + psParameters[1] +
                    ").POSITION") as double[];

                return new Point(nearestPosition[0], nearestPosition[1], nearestPosition[2]);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine nearest point", ex);
            }
        }

        /// <summary>
        /// Gets the minimum curvature at a position defined by TU parameters.
        /// </summary>
        /// <param name="t">The t parameter of the chosen surface position.</param>
        /// <param name="u">The u parameter of the chosen surface position.</param>
        /// <returns>The minimum curvature at the chosen position.</returns>
        public double GetMinCurvature(double t, double u)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].EVALUATE(" + t + ";" + u + ").CURVATURE.MIN");
        }

        /// <summary>
        /// Gets the maximum curvature at a position defined by TU parameters.
        /// </summary>
        /// <param name="t">The t parameter of the chosen surface position.</param>
        /// <param name="u">The u parameter of the chosen surface position.</param>
        /// <returns>The maximum curvature at the chosen position.</returns>
        public double GetMaxCurvature(double t, double u)
        {
            return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].EVALUATE(" + t + ";" + u + ").CURVATURE.MAX");
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a new Surface using PowerSHAPE's automatic surfacing
        /// methods.  Any necessary wireframe must already be selected.
        /// </summary>
        /// <param name="powerSHAPE">Instance of PowerSHAPE.</param>
        /// <param name="automaticSurfacingMethod">AutomaticSurfacingMethods enum value for surface method.</param>
        /// <param name="automaticSurfaceType">AutomaticSurfaceTypes enum value for surface type.</param>
        /// .
        /// <param name="advancedSurfaceOptions">SurfaceAdvancedOptions object for advanced options.</param>
        /// <returns>New surface created by the operation</returns>
        internal static PSSurface CreateSmartSurface(
            PSAutomation powerSHAPE,
            AutomaticSurfacingMethods automaticSurfacingMethod,
            AutomaticSurfaceTypes automaticSurfaceType,
            SurfaceAdvancedOptions advancedSurfaceOptions = null)
        {
            if (powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                throw new Exception("No elements selected in PowerSHAPE");
            }

            // Set the method used to create the surface
            string surfacingMethod = "";
            switch (automaticSurfacingMethod)
            {
                case AutomaticSurfacingMethods.FromNetwork:
                    surfacingMethod = "NETWORK";
                    break;
                case AutomaticSurfacingMethods.FromSeparate:
                    surfacingMethod = "SEPARATE";
                    break;
                case AutomaticSurfacingMethods.Developable:
                    surfacingMethod = "DEVELOPABLE";
                    throw new Exception("Not yet implemented!");
                case AutomaticSurfacingMethods.DriveCurve:
                    surfacingMethod = "DRIVE";
                    break;
                case AutomaticSurfacingMethods.TwoRails:
                    surfacingMethod = "RAILS";
                    break;
                case AutomaticSurfacingMethods.PlaneOfBestFit:
                    surfacingMethod = "BESTFIT";
                    break;
                case AutomaticSurfacingMethods.FromTriangles:
                    surfacingMethod = "FROMTRIANGLES";
                    break;
                case AutomaticSurfacingMethods.NetworkOverTriangles:
                    surfacingMethod = "NETOVERTRIS";
                    throw new Exception("Not yet implemented!");
                case AutomaticSurfacingMethods.Fill:
                    surfacingMethod = "FILLIN";
                    break;
            }

            // Set whether the created surface is to be NURBS or a PowerSurface
            string surfaceType = "";
            switch (automaticSurfaceType)
            {
                case AutomaticSurfaceTypes.NURB:
                    surfaceType = "TIGHT";
                    break;
                case AutomaticSurfaceTypes.PowerSurface:
                    surfaceType = "EXACT";
                    break;
            }

            powerSHAPE.ActiveModel.ClearCreatedItems();

            PSSurface newSurface = null;

            if (advancedSurfaceOptions == null)
            {
                powerSHAPE.DoCommand("CREATE SURFACE AutoSurf", "METHOD " + surfacingMethod, surfaceType, "ACCEPT");

                try
                {
                    if (automaticSurfacingMethod == AutomaticSurfacingMethods.Fill)
                    {
                        newSurface = (PSSurface)powerSHAPE.ActiveModel.SelectedItems[0];
                    }
                    else
                    {
                        newSurface = (PSSurface)powerSHAPE.ActiveModel.CreatedItems[0];
                    }
                }
                catch
                {
                    throw new ApplicationException("Failed to create Surface");
                }
            }
            else
            {
                powerSHAPE.DoCommand("CREATE SURFACE AutoSurf", "METHOD " + surfacingMethod, surfaceType);
                powerSHAPE.DoCommand("Advanced");

                if (advancedSurfaceOptions.InteriorInterpolation.HasValue)
                {
                    if (advancedSurfaceOptions.InteriorInterpolation == AdvancedInteriorInterpolationMethods.Linear)
                    {
                        powerSHAPE.DoCommand("INTERPOLATION LINEAR");
                    }
                    else
                    {
                        powerSHAPE.DoCommand("INTERPOLATION TANGENTIAL");
                    }
                }

                if (advancedSurfaceOptions.EdgeMatchingOption.HasValue)
                {
                    string cmd = "";
                    switch (advancedSurfaceOptions.EdgeMatchingOption)
                    {
                        case AutomaticSurfaceOptions.None:
                            cmd = "Mode None";
                            break;
                        case AutomaticSurfaceOptions.ArcLength:
                            cmd = "Mode Arc";
                            break;
                        case AutomaticSurfaceOptions.Repoint:
                            cmd = "Mode Repoint";
                            break;
                        case AutomaticSurfaceOptions.TangentDirection:
                            cmd = "Mode Tangent";
                            break;
                        case AutomaticSurfaceOptions.WidthOfCurve:
                            cmd = "Mode Width";
                            break;
                        default:
                            cmd = "";
                            break;
                    }

                    if (!string.IsNullOrEmpty(cmd))
                    {
                        powerSHAPE.DoCommand(cmd);
                    }
                }

                if (advancedSurfaceOptions.CornerTolerance.HasValue)
                {
                    powerSHAPE.DoCommand(string.Format("Angtol {0}", advancedSurfaceOptions.CornerTolerance));
                }

                powerSHAPE.DoCommand("Apply");

                //creates surface prior to showing advanced options so need to get at this point.
                try
                {
                    if (automaticSurfacingMethod == AutomaticSurfacingMethods.Fill)
                    {
                        newSurface = (PSSurface) powerSHAPE.ActiveModel.SelectedItems[0];
                    }
                    else
                    {
                        newSurface = (PSSurface)powerSHAPE.ActiveModel.CreatedItems[0];
                    }
                }
                catch
                {
                    throw new ApplicationException("Failed to create Surface");
                }

                powerSHAPE.DoCommand("Accept");
                powerSHAPE.DoCommand("ACCEPT");
            }

            return newSurface;
        }

        /// <summary>
        /// Deletes the Surface from PowerSHAPE and removes it from the Surfaces collection.
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Surfaces.Remove(this);
        }

        /// <summary>
        /// Reverses the normal of the Surface.
        /// </summary>
        public void ReverseSurfaceNormal()
        {
            AbortIfDoesNotExist();

            // Select only this surface
            AddToSelection(true);

            // Carry out operation
            _powerSHAPE.DoCommand("REVERSE");
        }

        /// <summary>
        /// Extends one edge of the Surface.
        /// </summary>
        /// <param name="edge">The option for the surface extension.</param>
        /// <param name="length">The extension length.</param>
        /// <param name="straightCurved">The straight curved.</param>
        /// <remarks></remarks>
        public void Extend(ExtensionEdges edge, double length, ExtensionType straightCurved)
        {
            AddToSelection(true);

            _powerSHAPE.DoCommand("EDIT LIMIT POINT");
            _powerSHAPE.DoCommand("PROFILING " + straightCurved);
            _powerSHAPE.DoCommand("EDGE " + (int)edge);
            _powerSHAPE.DoCommand("DISTANCE " + length);
            _powerSHAPE.DoCommand("EDIT LIMIT POINT OFF");
        }

        /// <summary>
        /// Converts the indicated surface curves to be wireframe.  Curves can only
        /// be converted in one direction at once.
        /// </summary>
        public PSSurface ConvertSurfaceToPowerSurface()
        {
            // Check whether the surface is a NURBS surface
            string nurbsConfirmation = "";
            if ((Type == SurfaceTypes.NURB) | (Type == SurfaceTypes.BCP))
            {
                nurbsConfirmation = "YES";
            }

            // Convert to PowerSurface
            AddToSelection(true);
            _powerSHAPE.DoCommand("CONVERT GENERALSURFACE", nurbsConfirmation);

            // Set type to Nothing, so it re-interrogates PowerSHAPE
            _surfaceType = null;

            // Check that the surface type has been changed
            if (Type != SurfaceTypes.PowerSurface)
            {
                throw new ApplicationException("Failed to convert surface");
            }

            // Return it as a PSSurface as it is no longer a primitive
            // Cheat by duplicating it and deleting the original.  This will prevent any old references to 
            // the object as a PSSurfacePlane from trying to get the width and length properties
            PSSurface surface = (PSSurface) Duplicate();
            Delete();
            return surface;
        }

        /// <summary>
        /// Offsets the surface along its normal by the desired distance, returning
        /// a PowerSurface.  Because the type of the surface might have changed, this object no longer
        /// exists - any subsequent operations must be carried out on the returned surface.
        /// </summary>
        /// <param name="distanceToOffset">The distance to offset the surface.</param>
        /// <param name="cornerBehaviour">The desired behaviour off the offset surface at corners.</param>
        /// <param name="discontinuityBehaviour">The desired behaviour of the offset surface when the operation produces a discontinuity.</param>
        public void Offset(
            float distanceToOffset,
            OffsetCornerTypes cornerBehaviour,
            OffsetSplitAtDiscontinuity discontinuityBehaviour)
        {
            // Add surface to selection
            AddToSelection(true);

            // Start offset
            _powerSHAPE.DoCommand("EDIT OFFSET");

            // Carry out offset
            _powerSHAPE.DoCommand("NOKEEP");
            _powerSHAPE.DoCommand(cornerBehaviour.ToString());
            _powerSHAPE.DoCommand(discontinuityBehaviour.ToString());
            _powerSHAPE.DoCommand("DISTANCE " + distanceToOffset);

            // End offset
            _powerSHAPE.DoCommand("EDIT OFFSET OFF");
        }

        /// <summary>
        /// Scales a surface within PowerSHAPE, returning a PowerSurface.  Because the type of
        /// the surface might have changed, this object no longer exists - any subsequent operations must be
        /// carried out on the returned surface.
        /// </summary>
        /// <param name="offsetX">Scale value in X.</param>
        /// <param name="offsetY">Scale value in Y.</param>
        /// <param name="offsetZ">Scale value in Z.</param>
        /// <param name="scaleOrigin">Scale origin, workplane origin if none specified.</param>
        public PSSurface Scale(double offsetX, double offsetY, double offsetZ, Point scaleOrigin = null)
        {
            // Add surface to selection
            AddToSelection(true);

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

            // Ensure primitives will not be removed from the selection
            _powerSHAPE.DoCommand("REMOVEPRIMS NO");

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
                // Complete operation
                _powerSHAPE.DoCommand("APPLY");
                _powerSHAPE.DoCommand("DIMISS");
            }
            else
            {
                // Complete operation
                _powerSHAPE.DoCommand("ACCEPT");
                _powerSHAPE.DoCommand("CANCEL");
            }

            // Check that one surface has been updated
            if (_powerSHAPE.ActiveModel.UpdatedItems.Count == 0)
            {
                throw new ApplicationException("Scaling operation not completed correctly");
            }

            // Set id of current surface to null, forcing user to use the returned surface for subsequent operations
            int id = _id;
            string name = _name;

            _id = -1;
            _name = null;

            // If the surface was a primitive, it will now have been converted to a PowerSurface within PowerSHAPE.
            // Therefore, return a new surface so that the type is updated
            return new PSSurface(_powerSHAPE, id, name);
        }

        /// <summary>
        /// Contains the code to stitch a surface to other entities.
        /// </summary>
        /// <param name="entityToStitchTo">The target surface to stitch to.</param>
        /// <param name="surfaceCurveToStitch">The surface curve to stitch on the selected surface.</param>
        /// <param name="directionAcross">Match across seam option.</param>
        /// <param name="directionAlong">Match along seam option.</param>
        /// <param name="stitchingTolerance">This value is the size of the largest gap to stitch. If the stitch point has to move a distance greater than this value, then this point is not moved onto the target.</param>
        /// <param name="stitchType">This allows you to choose the type of the target from one of the following: Curves, Surfaces, Surface curves.</param>
        /// <remarks></remarks>
        private void StitchToEntity(
            PSEntity entityToStitchTo,
            PSSurfaceCurve surfaceCurveToStitch,
            StitchDirectionAcross directionAcross,
            StitchDirectionAlong directionAlong,
            float stitchingTolerance,
            StitchTypes stitchType)
        {
            // Select surface curve that will be stitched
            SelectSurfaceCurves(new[] {surfaceCurveToStitch});

            // Carry out stitching operation
            _powerSHAPE.DoCommand("STITCH");
            _powerSHAPE.DoCommand(stitchType.ToString());
            _powerSHAPE.DoCommand("DIRECTION ACROSS " + directionAcross);
            _powerSHAPE.DoCommand("DIRECTION ALONG " + directionAlong);
            _powerSHAPE.DoCommand("PRE_STITCH_TOL " + stitchingTolerance);
            entityToStitchTo.AddToSelection();
            _powerSHAPE.DoCommand("PREVIEW");
            _powerSHAPE.DoCommand("ACCEPT");
        }

        /// <summary>
        /// Stitches the surface to a curve or compcurve.
        /// </summary>
        /// <param name="curveToStitchTo">The curve to stitch to.</param>
        /// <param name="surfaceCurveToStitch">The surface curve to stitch on the selected surface.</param>
        /// <param name="directionAcross">Indicates whether the points should be matched across the stitch.</param>
        /// <param name="directionAlong">Indicates whether the points should be matched along the stitch.</param>
        /// <param name="stitchingTolerance">The tolerance to stitch to.</param>
        public void StitchToCurve(
            PSGenericCurve curveToStitchTo,
            PSSurfaceCurve surfaceCurveToStitch,
            StitchDirectionAcross directionAcross,
            StitchDirectionAlong directionAlong,
            float stitchingTolerance)
        {
            // Carry out stitching operation
            StitchToEntity(curveToStitchTo,
                           surfaceCurveToStitch,
                           directionAcross,
                           directionAlong,
                           stitchingTolerance,
                           StitchTypes.CURVES);
        }

        /// <summary>
        /// Stitches the surface to another surface.
        /// </summary>
        /// <param name="surfaceToStitchTo">The surface to stitch to.</param>
        /// <param name="surfaceCurveToStitch">The surface curve to stitch on the selected surface.</param>
        /// <param name="directionAcross">Indicates whether the points should be matched across the stitch.</param>
        /// <param name="directionAlong">Indicates whether the points should be matched along the stitch.</param>
        /// <param name="stitchingTolerance">The tolerance to stitch to.</param>
        public void StitchToSurface(
            PSSurface surfaceToStitchTo,
            PSSurfaceCurve surfaceCurveToStitch,
            StitchDirectionAcross directionAcross,
            StitchDirectionAlong directionAlong,
            float stitchingTolerance)
        {
            // Carry out stitching operation
            StitchToEntity(surfaceToStitchTo,
                           surfaceCurveToStitch,
                           directionAcross,
                           directionAlong,
                           stitchingTolerance,
                           StitchTypes.SURFACES);
        }

        /// <summary>
        /// Stitches the surface to a surface curve.
        /// </summary>
        /// <param name="surfaceCurveToStitchTo">The surface curve to stitch to.</param>
        /// <param name="surfaceCurveToStitch">The surface curve to stitch on the selected surface.</param>
        /// <param name="directionAcross">Indicates whether the points should be matched across the stitch.</param>
        /// <param name="directionAlong">Indicates whether the points should be matched along the stitch.</param>
        /// <param name="stitchingTolerance">The tolerance to stitch to.</param>
        public void StitchToSurfaceCurve(
            PSSurfaceCurve surfaceCurveToStitchTo,
            PSSurfaceCurve surfaceCurveToStitch,
            StitchDirectionAcross directionAcross,
            StitchDirectionAlong directionAlong,
            float stitchingTolerance)
        {
            // Carry out stitching operation
            StitchToEntity(surfaceCurveToStitchTo,
                           surfaceCurveToStitch,
                           directionAcross,
                           directionAlong,
                           stitchingTolerance,
                           StitchTypes.SURFACECURVES);
        }

        /// <summary>
        /// Appends another surface onto this surface, retaining this surface.
        /// </summary>
        /// <param name="surfacetoAppendTo">The surface to append onto this surface.</param>
        /// <param name="curveToAppend">The surface curve on this surface to use as the append point.</param>
        public void AppendToSurface(PSSurface surfacetoAppendTo, PSSurfaceCurve curveToAppend)
        {
            // Add surface curve to append to the selection
            curveToAppend.AddToSelection(true);

            // Carry out operation
            _powerSHAPE.DoCommand("APPEND");
            surfacetoAppendTo.AddToSelection(false);
            _powerSHAPE.DoCommand("ACCEPT");

            // Check that operation succeeded
            if (_powerSHAPE.ActiveModel.UpdatedItems.Count == 0)
            {
                throw new ApplicationException("Append operation was not successful");
            }

            // Delete now nonexistant surface
            surfacetoAppendTo.Delete();
        }

        #region " Edit Operations "

        /// <summary>
        /// Mirrors a single surface in a specified plane.
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the surface.</param>
        /// <param name="mirrorPoint">The origin of the mirror plane.</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// Moves a single surface by the relative distance between two absolute positions.
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions.</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// Moves a single surface by a specified relative amount.
        /// </summary>
        /// <param name="moveVector">The relative amount by which the surface will be moved.</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation.</param>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Rotates a single surface by a specified angle around a specified axis.
        /// </summary>
        /// <param name="rotationAxis">The axis around which the surface is are to be rotated.</param>
        /// <param name="rotationAngle">The angle by which the surface is to be rotated.</param>
        /// <param name="copiesToCreate">The number of copies to create of the original surface.</param>
        /// <param name="rotationOrigin">The origin of the rotation axis.</param>
        /// <returns>A list of surfaces created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// Limits a surface by a specified list of entities.
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
        /// Limits a surface by a specified list of entities.
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
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <returns>Limited entity.</returns>
        /// <remarks>To be used with PSEntityLimiter.NextSolution.</remarks>
        public PSEntity LimitUsingDynamicCutter(LimitingKeepOptions keepOption = LimitingKeepOptions.KeepOne)
        {
            return PSEntityLimiter.LimitEntityUsingDynamicCutter(this, keepOption);
        }

        #endregion

        #region " Surface Curves "

        /// <summary>
        /// Selects one or more curves on the Surface.
        /// </summary>
        /// <param name="surfaceCurveType">The surface direction option for a surface curve.</param>
        /// <param name="curveNumbers">The curves to select.</param>
        /// <remarks></remarks>
        public void SelectSurfaceCurves(SurfaceCurveTypes surfaceCurveType, int[] curveNumbers)
        {
            // Select the surface
            AddToSelection(true);

            // Create the list of curves
            string curveNumbersCommand = "";

            // Create the curveNumbers string for macro code
            foreach (int curveNumber in curveNumbers)
            {
                if (string.IsNullOrEmpty(curveNumbersCommand))
                {
                    curveNumbersCommand = curveNumber.ToString();
                }
                else
                {
                    curveNumbersCommand += " " + curveNumber;
                }
            }

            // Carry out operation
            _powerSHAPE.DoCommand(string.Format("SELECT_{0} {1}", surfaceCurveType.ToShortString(), curveNumbersCommand));
        }

        /// <summary>
        /// Selects one or more lateral and longitudinal curves on the Surface.
        /// </summary>
        /// <param name="lateralCurveNumbers">The lateral curves to select.</param>
        /// <param name="longitudinalCurveNumbers">The longitudinal curves to select.</param>
        /// <remarks></remarks>
        public void SelectLateralAndLongitudinalSurfaceCurves(int[] lateralCurveNumbers, int[] longitudinalCurveNumbers)
        {
            // Select the surface
            AddToSelection(true);

            // Create the list of curves
            string lateralCurveNumbersCommand = null;

            // Create the curveNumbers string for macro code
            foreach (int curveNumber in lateralCurveNumbers)
            {
                if (string.IsNullOrEmpty(lateralCurveNumbersCommand))
                {
                    lateralCurveNumbersCommand = curveNumber.ToString();
                }
                else
                {
                    lateralCurveNumbersCommand += " " + curveNumber;
                }
            }

            // Carry out operation
            _powerSHAPE.DoCommand(string.Format("SELECT_{0} {1}",
                                                SurfaceCurveTypes.Lateral.ToShortString(),
                                                lateralCurveNumbersCommand));

            // Create the list of curves
            string longitudinalCurveNumbersCommand = null;

            // Create the curveNumbers string for macro code
            foreach (int curveNumber in longitudinalCurveNumbers)
            {
                if (string.IsNullOrEmpty(longitudinalCurveNumbersCommand))
                {
                    longitudinalCurveNumbersCommand = curveNumber.ToString();
                }
                else
                {
                    longitudinalCurveNumbersCommand += " " + curveNumber;
                }
            }

            // Carry out operation
            _powerSHAPE.DoCommand(string.Format("SELECT_{0} {1}",
                                                SurfaceCurveTypes.Longitudinal.ToShortString(),
                                                longitudinalCurveNumbersCommand));
        }

        /// <summary>
        /// Selects one or more curves on the Surface.
        /// </summary>
        /// <param name="surfaceCurves">The specific surface curves to select.</param>
        /// <remarks>This operation can select any combination of laterals and longitudinals on one surface.</remarks>
        public void SelectSurfaceCurves(PSSurfaceCurve[] surfaceCurves)
        {
            // Create lists of laterals and longitudinals to select
            Dictionary<int, PSLateral> lateralsToSelect = new Dictionary<int, PSLateral>();
            Dictionary<int, PSLongitudinal> longitudinalsToSelect = new Dictionary<int, PSLongitudinal>();
            foreach (PSSurfaceCurve curveToSelect in surfaceCurves)
            {
                // Determine curve type
                if (curveToSelect is PSLateral)
                {
                    // Add to dictionary of laterals
                    lateralsToSelect.Add(Convert.ToInt32(curveToSelect.Name), (PSLateral) curveToSelect);
                }
                else
                {
                    // Add to dictionary of longitudinals
                    longitudinalsToSelect.Add(Convert.ToInt32(curveToSelect.Name), (PSLongitudinal) curveToSelect);
                }
            }

            // Select all curves
            SelectLateralAndLongitudinalSurfaceCurves(lateralsToSelect.Keys.ToArray(), longitudinalsToSelect.Keys.ToArray());
        }

        /// <summary>
        /// Adds a new surface curve at the point indicated.
        /// </summary>
        /// <param name="latOrLon">The surface direction option for a surface curve.</param>
        /// <param name="creationPoint">The creation point.</param>
        /// <remarks></remarks>
        public void AddSurfaceCurveByParameter(SurfaceCurveTypes latOrLon, float creationPoint)
        {
            if (this is PSPrimitive)
            {
                throw new Exception("Surface curves cannot be created for this type of surface primitive");
            }

            // Set up variables dependent on surface curve type
            int numberOfCurves = 0;
            switch (latOrLon)
            {
                case SurfaceCurveTypes.Lateral:
                    numberOfCurves = Laterals.Count;
                    break;
                case SurfaceCurveTypes.Longitudinal:
                    numberOfCurves = Longitudinals.Count;
                    break;
            }

            // Check that the creationPoint is valid
            if (creationPoint < 1)
            {
                throw new ApplicationException("Curve creation point must be greater than 1");
            }
            if (creationPoint > numberOfCurves)
            {
                throw new ApplicationException("Curve creation point is greater than the number of curve points");

                //creationPoint is an integer
            }
            if (creationPoint % 1 == 0)
            {
                throw new ApplicationException("Curve creation point cannot be on an existing curve");
            }

            // Insert curve
            AddToSelection(true);
            _powerSHAPE.DoCommand("ADD_CURVE");
            _powerSHAPE.DoCommand("TYPE " + latOrLon.ToLongString());
            _powerSHAPE.DoCommand("PARAMETERMETHOD", "PARAMETERMETHOD VALUE " + creationPoint);
            _powerSHAPE.DoCommand("APPLY", "DISMISS");

            // The internal IDs of the remaining curves need to be maintained, so their PowerSHAPE ids and names are
            // updated to match the changes that PowerSHAPE has made
            switch (latOrLon)
            {
                case SurfaceCurveTypes.Lateral:

                    // Insert the lateral into the correct place in the LateralsCollection
                    _laterals.Insert((int) Math.Floor(creationPoint),
                                     new PSLateral(_powerSHAPE, Math.Ceiling(creationPoint).ToString(), this));

                    // PowerSHAPE has changed the names and ids of the remaining laterals, so these need to be updated here
                    for (int i = (int) Math.Ceiling(creationPoint); i <= _laterals.Count - 1; i++)
                    {
                        _laterals[i].Name = (Convert.ToDouble(_laterals[i].Name) + 1).ToString();
                        _laterals[i].Id = Convert.ToInt32(_laterals[i].Id + 1);
                    }

                    break;
                case SurfaceCurveTypes.Longitudinal:

                    // Insert the longitudinal into the correct place in the LongitudinalsCollection
                    _longitudinals.Insert((int) Math.Floor(creationPoint),
                                          new PSLongitudinal(_powerSHAPE, Math.Ceiling(creationPoint).ToString(), this));

                    // PowerSHAPE has changed the names and ids of the remaining longitudinals, so these need to be updated here
                    for (int i = (int) Math.Ceiling(creationPoint); i <= _longitudinals.Count - 1; i++)
                    {
                        _longitudinals[i].Name = (Convert.ToDouble(_longitudinals[i].Name) + 1).ToString();
                        _longitudinals[i].Id = Convert.ToInt32(_longitudinals[i].Id + 1);
                    }

                    break;
            }
        }

        /// <summary>
        /// Converts the indicated surface curves to be wireframe.  Curves can only
        /// be converted in one direction at once.
        /// </summary>
        /// <param name="latOrLon">The SurfaceCurveTypes enum differentiates between a Lateral or a Longitudinal.</param>
        /// <param name="curveNumbers">This is the list of curve numbers to be converted to wireframe.</param>
        /// <returns>The compcurve(s) created by the conversion operation.</returns>
        public List<PSCompCurve> ConvertSurfaceCurvesToWireframe(SurfaceCurveTypes latOrLon, int[] curveNumbers)
        {
            // Set up variables dependent on surface curve type
            int numberOfCurves = 0;
            switch (latOrLon)
            {
                case SurfaceCurveTypes.Lateral:
                    numberOfCurves = NumberOfLaterals;
                    break;
                case SurfaceCurveTypes.Longitudinal:
                    numberOfCurves = NumberOfLongitudinals;
                    break;
            }

            foreach (int curve in curveNumbers)
            {
                // Check that the curve number is valid
                if (curve < 0)
                {
                    throw new ApplicationException("Curve creation point must be a positive integer");
                }
                if (curve > numberOfCurves)
                {
                    throw new ApplicationException("Curve creation point is greater than the number of curve points");
                }
            }

            // Select the surface
            AddToSelection(true);

            // Select surface curves
            SelectSurfaceCurves(latOrLon, curveNumbers);

            // Convert to wireframe
            _powerSHAPE.DoCommand("CONVERT WIREFRAME");

            // Check that items have been created
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                throw new ApplicationException("Failed to create any wireframe");
            }

            // Add created items to their appropriate collections and to the list to return
            List<PSCompCurve> createdCurves = new List<PSCompCurve>();
            foreach (PSCompCurve curve in _powerSHAPE.ActiveModel.CreatedItems)
            {
                createdCurves.Add(curve);
                _powerSHAPE.ActiveModel.CompCurves.Add(curve);
            }

            return createdCurves;
        }

        /// <summary>
        /// Swaps the laterals and longitudinals of a surface - laterals become longitudinals and longitudinals become laterals.
        /// </summary>
        public void SwapSurfaceCurves()
        {
            AbortIfDoesNotExist();

            // Select surface
            AddToSelection(true);

            // Swap curves
            _powerSHAPE.DoCommand("SWAP");

            // Clear the internal lists so that they are re-read when next queried
            _laterals = new PSLateralsCollection(_powerSHAPE, this);
            _longitudinals = new PSLongitudinalsCollection(_powerSHAPE, this);
        }

        /// <summary>
        /// Reverses the direction of the surface curves.
        /// </summary>
        /// <remarks></remarks>
        public void ReverseDirectionOfSurfaceCurves()
        {
            AbortIfDoesNotExist();

            // Add to selection
            AddToSelection(true);

            // Untrim surface to ensure all points available
            bool trimMethod = false;
            if (IsTrimmed)
            {
                trimMethod = true;
            }
            IsTrimmed = false;

            // Reverse curves
            SelectSurfaceCurves(SurfaceCurveTypes.Lateral, new[] {1});
            _powerSHAPE.DoCommand("REVERSE");
            SelectSurfaceCurves(SurfaceCurveTypes.Longitudinal, new[] {1});
            _powerSHAPE.DoCommand("REVERSE");

            // Restore trim status if necessary
            if (trimMethod)
            {
                IsTrimmed = true;
            }
        }

        #endregion

        #region " Boundaries and pCurves "

        /// <summary>
        /// Selects a boundary on the surface.  It is private because it doesn't switch off
        /// trim region editing after completion.
        /// </summary>
        /// <param name="index">The index of the boundary to be selected.</param>
        private void SelectBoundary(int index)
        {
            // Add the surface
            AddToSelection(true);

            // Check that the boundary exists
            if (index > NumberOfBoundaries)
            {
                throw new ApplicationException("Boundary does not exist on surface");
            }
            if (index < 1)
            {
                throw new ApplicationException("Boundary index must be 1 or higher");
            }

            // Switch PowerSHAPE into boundary trim region editing mode
            _powerSHAPE.TrimRegionEditingMode = TrimRegionEditingModes.BoundaryEditing;

            // Add the boundary
            _powerSHAPE.DoCommand("SELECT_CURVES " + index);
        }

        /// <summary>
        /// Operation deletes a boundary from the surface and then switches trim region editing off.
        /// </summary>
        /// <param name="index">The index of the boundary to delete.</param>
        public void DeleteBoundary(int index)
        {
            // Select the boundary
            SelectBoundary(index);

            // Delete the boundary
            _powerSHAPE.DoCommand("EXPLODE");

            // Switch trim region editing off
            _powerSHAPE.IsInTrimRegionEditing = false;
        }

        /// <summary>
        /// Converts a boundary on a surface into a composite curve.
        /// </summary>
        /// <param name="index">The index of the boundary to convert.</param>
        /// <returns>The compcurve created from the boundary.</returns>
        public PSCompCurve ConvertBoundaryToCompCurve(int index)
        {
            // Adds the boundary
            SelectBoundary(index);

            // Convert boundary to compcurve
            _powerSHAPE.DoCommand("CONVERT WIREFRAME");

            // Switch off trim region editing
            _powerSHAPE.IsInTrimRegionEditing = false;

            // Get the created compcurve
            PSCompCurve createdCompCurve = (PSCompCurve) _powerSHAPE.ActiveModel.CreatedItems[0];
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                throw new ApplicationException("No composite curve was created from the boundary");
            }
            if (_powerSHAPE.ActiveModel.CreatedItems.Count > 1)
            {
                throw new ApplicationException("More than one composite curve was created");
            }

            return createdCompCurve;
        }

        #endregion

        #endregion

        #region " Add/Intersect/Subtract Operations "

        /// <summary>
        /// Adds an entity to another entity.
        /// </summary>
        /// <param name="entityToAdd">Entity to be added.</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False.</param>
        /// <returns>Entity created from the addition.</returns>
        public PSEntity AddEntityToEntity(IPSAddable entityToAdd, bool copeWithCoincidentFaces = false)
        {
            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE ADDITION");

            // Select this item
            _powerSHAPE.DoCommand("PRIMARY ON");
            AddToSelection(false);

            if (copeWithCoincidentFaces)
            {
                _powerSHAPE.DoCommand("TOLERANCE ON");
            }
            else
            {
                _powerSHAPE.DoCommand("TOLERANCE OFF");
            }

            // Add addition solid to selection
            PSEntity entity = (PSEntity) entityToAdd;
            entity.AddToSelection(false);

            // Fill in options on the dialog and accept it to complete the operation.  The YES at the end is to handle the question about deleting the solid tree
            _powerSHAPE.DoCommand("KEEP OFF", "ACCEPT", "YES");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

                // Remove the lost solid
                entity.Delete();
                Delete();

                // Return the created Mesh
                return createdMesh;
            }

            // Remove deleted solid from Solids collection
            entity.Delete();

            // Return this item
            return this;
        }

        /// <summary>
        /// Adds entities to another entity.
        /// </summary>
        /// <param name="entitiesToAdd">Collection of entities to be added.</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False.</param>
        /// <returns>Entity created from the addition.</returns>
        public PSEntity AddEntitiesToEntity(IEnumerable<IPSAddable> entitiesToAdd, bool copeWithCoincidentFaces = false)
        {
            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE ADDITION");

            // Select this item
            _powerSHAPE.DoCommand("PRIMARY ON");
            AddToSelection(false);

            // Add entities to selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSEntity entity in entitiesToAdd)
            {
                entity.AddToSelection(false);
            }

            if (copeWithCoincidentFaces)
            {
                _powerSHAPE.DoCommand("TOLERANCE ON");
            }
            else
            {
                _powerSHAPE.DoCommand("TOLERANCE OFF");
            }

            // Fill in options on the dialog and accept it to complete the operation.  The YES at the end is to handle the question about deleting the solid tree
            _powerSHAPE.DoCommand("KEEP OFF", "ACCEPT", "YES");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

                // Remove the lost solids
                foreach (PSEntity entity in entitiesToAdd)
                {
                    entity.Delete();
                }
                Delete();

                // Return the created Mesh
                return createdMesh;
            }

            // Remove deleted solid from Solids collection
            foreach (PSEntity entity in entitiesToAdd)
            {
                entity.Delete();
            }

            // Return this item
            return this;
        }

        /// <summary>
        /// Intersects an entity with another entity.
        /// </summary>
        /// <param name="entityToIntersect">Entity to intersect.</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False.</param>
        /// <returns>Entity created from the intersection</returns>
        public PSEntity IntersectEntityWithEntity(IPSIntersectable entityToIntersect, bool copeWithCoincidentFaces = false)
        {
            // In order to force the dialog to appear, t.he command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE INTERSECTION");

            // Select this item
            _powerSHAPE.DoCommand("PRIMARY ON");
            AddToSelection(false);

            // Add addition solid to selection
            PSEntity entity = (PSEntity) entityToIntersect;
            entity.AddToSelection(false);

            if (copeWithCoincidentFaces)
            {
                _powerSHAPE.DoCommand("TOLERANCE ON");
            }
            else
            {
                _powerSHAPE.DoCommand("TOLERANCE OFF");
            }

            // Fill in options on the dialog and accept it to complete the operation.  The YES at the end is to handle the question about deleting the solid tree
            _powerSHAPE.DoCommand("KEEP OFF", "ACCEPT", "YES");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

                // Remove the lost solid
                entity.Delete();
                Delete();

                // Return the created Mesh
                return createdMesh;
            }

            // Remove deleted solid from Solids collection
            entity.Delete();

            // Return this item
            return this;
        }

        /// <summary>
        /// Intersects a collection of entities with another entity.
        /// </summary>
        /// <param name="entitiesToIntersect">Entities to intersect.</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False.</param>
        /// <returns>Entity created from the intersection</returns>
        public List<PSEntity> IntersectEntitiesWithEntity(
            IEnumerable<IPSIntersectable> entitiesToIntersect,
            bool copeWithCoincidentFaces = false)
        {
            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE INTERSECTION");

            // Select this item
            _powerSHAPE.DoCommand("PRIMARY ON");
            AddToSelection(false);

            // Add entities to selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSEntity entity in entitiesToIntersect)
            {
                entity.AddToSelection(false);
            }

            if (copeWithCoincidentFaces)
            {
                _powerSHAPE.DoCommand("TOLERANCE ON");
            }
            else
            {
                _powerSHAPE.DoCommand("TOLERANCE OFF");
            }

            // Fill in options on the dialog and accept it to complete the operation.  The YES at the end is to handle the question about deleting the solid tree
            _powerSHAPE.DoCommand("KEEP OFF", "ACCEPT", "YES");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

                // Remove the lost solids
                foreach (PSEntity entity in entitiesToIntersect)
                {
                    entity.Delete();
                }
                Delete();

                // Return the created Mesh
                return new List<PSEntity> {createdMesh};
            }

            // Remove deleted solid from Solids collection
            foreach (PSEntity entity in entitiesToIntersect)
            {
                entity.Delete();
            }

            // Return this item
            return new List<PSEntity> {this};
        }

        /// <summary>
        /// Subtracts an entity from another entity.
        /// </summary>
        /// <param name="entityToSubtract">Entity to subtract.</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False.</param>
        /// <returns>Entity created from the sub.traction.</returns>
        /// .
        public PSEntity SubtractEntityFromEntity(IPSSubtractable entityToSubtract, bool copeWithCoincidentFaces = false)
        {
            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE SUBTRACTION");

            // Select this item
            _powerSHAPE.DoCommand("PRIMARY ON");
            AddToSelection(false);

            // Add addition solid to selection
            PSEntity entity = (PSEntity) entityToSubtract;
            entity.AddToSelection(false);

            if (copeWithCoincidentFaces)
            {
                _powerSHAPE.DoCommand("TOLERANCE ON");
            }
            else
            {
                _powerSHAPE.DoCommand("TOLERANCE OFF");
            }

            // Fill in options on the dialog and accept it to complete the operation.  The YES at the end is to handle the question about deleting the solid tree
            _powerSHAPE.DoCommand("KEEP OFF", "ACCEPT", "YES");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

                // Remove the lost solid
                entity.Delete();
                Delete();

                // Return the created Mesh
                return createdMesh;
            }

            // Remove deleted solid from Solids collection
            entity.Delete();

            // Return this item
            return this;
        }

        /// <summary>
        /// Subtracts a collection of entities from another entity.
        /// </summary>
        /// <param name="entitiesToSubtract">Entities to subtract.</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False.</param>
        /// <returns>Entity created from the subtraction.</returns>
        public PSEntity SubtractEntitiesFromEntity(
            IEnumerable<IPSSubtractable> entitiesToSubtract,
            bool copeWithCoincidentFaces = false)
        {
            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE SUBTRACTION");

            // Select this item
            _powerSHAPE.DoCommand("PRIMARY ON");
            AddToSelection(false);

            // Add entities to selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            foreach (PSEntity entity in entitiesToSubtract)
            {
                entity.AddToSelection(false);
            }

            if (copeWithCoincidentFaces)
            {
                _powerSHAPE.DoCommand("TOLERANCE ON");
            }
            else
            {
                _powerSHAPE.DoCommand("TOLERANCE OFF");
            }

            // Fill in options on the dialog and accept it to complete the operation.  The YES at the end is to handle the question about deleting the solid tree
            _powerSHAPE.DoCommand("KEEP OFF", "ACCEPT", "YES");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

                // Remove the lost solids
                foreach (PSEntity entity in entitiesToSubtract)
                {
                    entity.Delete();
                }
                Delete();

                // Return the created Mesh
                return createdMesh;
            }

            // Remove deleted solid from Solids collection
            foreach (PSEntity entity in entitiesToSubtract)
            {
                entity.Delete();
            }

            // Return this item
            return this;
        }

        #endregion
    }

    #region "Inner Classes"

    /// <summary>
    /// Contains advanced surface options that can be used when creating surfaces.
    /// </summary>
    /// <remarks></remarks>
    public class SurfaceAdvancedOptions
    {
        #region "Properties"

        /// <summary>
        /// The edge matching option to control how PowerSHAPE adds extra surface points and curves to the initial network so that each point lies at the junction of a lateral and a longitudinal.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public AutomaticSurfaceOptions? EdgeMatchingOption { get; set; }

        /// <summary>
        /// The interior interpolation methods option for surface creation.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public AdvancedInteriorInterpolationMethods? InteriorInterpolation { get; set; }

        /// <summary>
        /// The corner tolerance.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public double? CornerTolerance { get; set; }

        #endregion

        //Public Sub New()
        //    EdgeMatchingOption = Nothing
        //    InteriorInterpolation = Nothing
        //    CornerTolerance = Nothing
        //End Sub

        /// <summary>
        /// Initialises a new instance of the PSSurface. SurfaceAdvancedOptions class.
        /// </summary>
        /// <param name="aEdgeMatchingOption">The edge matching option to control how PowerSHAPE adds extra surface points and curves to the initial network so that each point lies at the junction of a lateral and a longitudinal.</param>
        /// <param name="aInteriorInterPolation">The interior interpolation methods option for surface creation.</param>
        /// <param name="aCornerTolerance">The corner tolerance.</param>
        /// <remarks></remarks>
        public SurfaceAdvancedOptions(
            AutomaticSurfaceOptions? aEdgeMatchingOption = null,
            AdvancedInteriorInterpolationMethods? aInteriorInterPolation = null,
            double? aCornerTolerance = null)
        {
            EdgeMatchingOption = aEdgeMatchingOption;
            InteriorInterpolation = aInteriorInterPolation;
            CornerTolerance = aCornerTolerance;
        }
    }

    #endregion

    #region " Extensions "

    /// <summary>
    /// Contains all extension methods for the SurfaceCurveTypes enumeration.
    /// </summary>
    public static class SurfaceCurveTypesExtensions
    {
        /// <summary>
        /// Returns short forms of the surface curve type for insertion into macro code.
        /// </summary>
        /// <param name="inputSurfaceCurveType">The surface curve type to convert to short string.</param>
        /// <returns>The short string to the input surface type (e.g. Lateral will be 'LATS').</returns>
        /// <remarks></remarks>
        public static string ToShortString(this SurfaceCurveTypes inputSurfaceCurveType)
        {
            return inputSurfaceCurveType == SurfaceCurveTypes.Lateral ? "LATS" : "LONS";
        }

        /// <summary>
        /// Returns long forms of the surface curve type for insertion into macro code.
        /// </summary>
        /// <param name="inputSurfaceCurveType">The surface curve type to convert to long string.</param>
        /// <returns>Long surface type name (e.g. Lateral will be 'LATERAL').</returns>
        /// <remarks></remarks>
        public static string ToLongString(this SurfaceCurveTypes inputSurfaceCurveType)
        {
            return inputSurfaceCurveType == SurfaceCurveTypes.Lateral ? "LATERAL" : "LONGITUDINAL";
        }
    }

    #endregion
}