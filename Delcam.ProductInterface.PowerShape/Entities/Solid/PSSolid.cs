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
using Microsoft.VisualBasic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures an Solid object in PowerSHAPE
    /// </summary>
    public class PSSolid : PSEntity, IPSMoveable, IPSRotateable, IPSMirrorable, IPSOffsetable, IPSScalable, IPSAddable,
                           IPSIntersectable, IPSSubtractable, IPSLimitable
    {
        #region " Fields "

        private SolidTypes _solidType;

        private PSMaterial _material;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connects to PowerSHAPE
        /// </summary>
        internal PSSolid(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        /// <summary>
        /// Initialises the Solid
        /// </summary>
        internal PSSolid(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a Solid from a mesh
        /// </summary>
        internal PSSolid(PSAutomation powerSHAPE, PSMesh meshToConvert) : base(powerSHAPE)
        {
            // Select the mesh to convert
            meshToConvert.AddToSelection(true);

            // Convert mesh to solid
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.DoCommand("CREATE SOLID SURFACES", "NO");

            // Check one solid was created
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                throw new ApplicationException("No solid created");
            }
            if (_powerSHAPE.ActiveModel.SelectedItems.Count != 1)
            {
                _powerSHAPE.ActiveModel.Undo();
                throw new ApplicationException("More than one solid created");
            }

            PSSolid createdSolid = (PSSolid) _powerSHAPE.ActiveModel.SelectedItems[0];
            _id = createdSolid.Id;
            _name = createdSolid.Name;

            // Remove converted mesh
            _powerSHAPE.ActiveModel.Meshes.Remove(meshToConvert);
        }

        /// <summary>
        /// Creates a Solid from a drive curve and a number of sections
        /// </summary>
        /// <param name="powerSHAPE">The instance of PowerSHAPE being used</param>
        /// <param name="driveCurve">The drive curve along which to create the solid</param>
        /// <param name="sections">The sections through which to create the solid</param>
        /// <remarks></remarks>
        internal PSSolid(PSAutomation powerSHAPE, PSGenericCurve driveCurve, List<PSGenericCurve> sections) : base(powerSHAPE)
        {
            // Create the solid
            // Clear selection
            powerSHAPE.ActiveModel.ClearSelectedItems();

            // Create the solid
            _powerSHAPE.DoCommand("CREATE SOLID DRIVECURVE", "SECTIONS");

            // Select the sections
            foreach (PSWireframe section in sections)
            {
                section.AddToSelection();
            }

            // Select the drive curve
            _powerSHAPE.DoCommand("DRIVECURVE");
            driveCurve.AddToSelection();

            // Create the solid
            _powerSHAPE.DoCommand("FINISH");

            PSSolid createdSolid = (PSSolid) _powerSHAPE.ActiveModel.CreatedItems[0];
            _id = createdSolid.Id;
            _name = createdSolid.Name;
        }

        #endregion

        #region " Properties "

        internal const string SOLID_IDENTIFIER = "SOLID";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return SOLID_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 40 * 20000000 + _id; }
        }

        /// <summary>
        /// Gets and sets the level the solid is on
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        public override PSLevel Level
        {
            get
            {
                AbortIfDoesNotExist();

                // If the solid is on level 0 it reports the level as "Solid is a ghost" so need to handle that
                string levelString = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].LEVEL");
                if (levelString == "Solid is a ghost")
                {
                    return _powerSHAPE.ActiveModel.Levels[0];
                }
                return _powerSHAPE.ActiveModel.Levels[int.Parse(levelString)];
            }
            set { base.Level = value; }
        }

        /// <summary>
        /// Gets and sets whether the Solid is the active solid
        /// </summary>
        public bool IsActive
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].ACTIVE");
            }

            set
            {
                // Add to selection
                AddToSelection(true);

                if (value)
                {
                    _powerSHAPE.DoCommand("MODIFY MODIFY ACTIVATE ACCEPT");
                }
                else
                {
                    _powerSHAPE.DoCommand("MODIFY MODIFY DEACTIVATE ACCEPT");
                }
            }
        }

        /// <summary>
        /// Get and sets solid name in PowerShape and in Custom Core entity.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Name
        {
            get
            {
                if (_name == null)
                {
                    AbortIfDoesNotExist();
                    _name = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].NAME");
                }

                return _name;
            }

            set
            {
                // Do the rename
                AddToSelection(true);
                _powerSHAPE.DoCommand("TREEEDIT", "MODIFY NAME " + value);

                // Set the name
                _name = value;
            }
        }

        /// <summary>
        /// Gets the representation number of the Solid
        /// </summary>
        public double RepresentationNumber
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].REP_NUMBER"); }
        }

        /// <summary>
        /// Gets the volume of the Solid
        /// </summary>
        public virtual double Volume
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].VOLUME"); }
        }

        /// <summary>
        /// Gets the area of the surfaces in the Solid
        /// </summary>
        public double Area
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].AREA"); }
        }

        /// <summary>
        /// Gets the coordinates of the centre of gravity of the Solid
        /// </summary>
        public Point CentreOfGravity
        {
            get
            {
                double[] psCoords = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].COG") as double[];
                return new Point(psCoords[0], psCoords[1], psCoords[2]);
            }
        }

        /// <summary>
        /// Gets the moment of inertia of the Solid
        /// </summary>
        public double[] MomentOfInertia
        {
            get
            {
                double[] psMOI = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].MOI") as double[];
                return new[]
                {
                    psMOI[0],
                    psMOI[1],
                    psMOI[2]
                };
            }
        }

        /// <summary>
        /// Gets the mean thickness of the Solid
        /// </summary>
        public double Thickness
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].MEAN_THICKNESS"); }
        }

        /// <summary>
        /// Gets whether the Solid is hidden
        /// </summary>
        public bool IsHidden
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].HIDE");
            }
        }

        /// <summary>
        /// Gets whether the Solid is watertight
        /// </summary>
        public bool IsWatertight
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].WATERTIGHT");
            }
        }

        /// <summary>
        /// Gets whether the Solid boundaries are valid
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
        /// Gets and sets whether the solid is a version 8 or parasolid
        /// </summary>
        public SolidVersions Version
        {
            get
            {
                AbortIfDoesNotExist();

                if (_powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].V8"))
                {
                    return SolidVersions.Version8;
                }
                return SolidVersions.Parasolid;
            }
            set
            {
                // Check that conversion is necessary
                if (Version == value)
                {
                    throw new ApplicationException("Solid version is already " + value + ".  Cannot carry out conversion");
                }

                // Get currently selected items
                List<PSEntity> previouslySelected = new List<PSEntity>();
                foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
                {
                    previouslySelected.Add(selectedEntity);
                }

                // Carry out conversion operation
                AddToSelection(true);
                if (value == SolidVersions.Parasolid)
                {
                    _powerSHAPE.DoCommand("CONVERT SOLIDTOPARASOLID");
                }
                else
                {
                    _powerSHAPE.DoCommand("CONVERT PARASOLIDTOSOLID");
                }

                // Restore previous selection
                _powerSHAPE.ActiveModel.ClearSelectedItems();
                foreach (PSEntity entityToSelect in previouslySelected)
                {
                    entityToSelect.AddToSelection(false);
                }
            }
        }

        /// <summary>
        /// Gets the type of the Solid
        /// </summary>
        public SolidTypes Type
        {
            get
            {
                string psType = _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].TYPE");
                _solidType = (SolidTypes) Enum.Parse(typeof(SolidTypes), psType, true);
                return _solidType;
            }
        }

        /// <summary>
        /// Gets whether the solid is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].CONNECTED");
            }
        }

        /// <summary>
        /// Gets the number of surfaces in the solid
        /// </summary>
        public int NumberOfSurfaces
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].SURFACES");
            }
        }

        /// <summary>
        /// Gets the number of features in the solid, not including sub-branches
        /// </summary>
        public int NumberOfFeatures
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].FEATURES");
            }
        }

        /// <summary>
        /// Gets the number of features in the solid, including sub-branches
        /// </summary>
        public int NumberOfFeaturesInclSubBranches
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].FEATURES.ALL");
            }
        }

        /// <summary>
        /// Gets the number of features in the solid, not including sub-branches
        /// </summary>
        public int NumberOfSelectedFeatures
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].FEATURES.SELECTED");
            }
        }

        /// <summary>
        /// Gets the number of error-suppressed features in the solid's tree
        /// </summary>
        public int NumberOfErrorSuppressedFeatures
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].ERROR");
            }
        }

        /// <summary>
        /// Gets the number of features that were replayed during the last tree redefinition
        /// </summary>
        public string NumberOfRedefinedFeatures
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadStringValue(Identifier + "[ID " + _id + "].REDEFINED");
            }
        }

        /// <summary>
        /// Gets the number of group features in the solid, not including sub-branches
        /// </summary>
        public int NumberOfGroupFeatures
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].GROUPS");
            }
        }

        /// <summary>
        /// Gets the number of group features in the solid, including sub-branches
        /// </summary>
        public int NumberOfGroupFeaturesInclSubBranches
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].GROUPS.ALL");
            }
        }

        /// <summary>
        /// Gets whether the solid is closed
        /// </summary>
        public bool IsClosed
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].CLOSED");
            }
        }

        /// <summary>
        /// Gets whether the solid is a ghost
        /// </summary>
        public bool IsGhost
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].GHOST");
            }
        }

        /// <summary>
        /// Gets the number of linked half-edges in the solid
        /// </summary>
        public int NumberLinkedHalfEdges
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].NLINKS");
            }
        }

        /// <summary>
        /// Gets the number of unlinked half-edges in the solid
        /// </summary>
        public int NumberUnlinkedHalfEdges
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].NLINKS_OPEN");
            }
        }

        /// <summary>
        /// Gets the number of vertices along edges in the solid
        /// </summary>
        public int NumberVerticesAlongEdges
        {
            get
            {
                AbortIfDoesNotExist();

                return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].EDGE_VERTICES");
            }
        }

        /// <summary>
        /// Gets the average edge tolerance of the Solid
        /// </summary>
        public double AverageEdgeTolerance
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].AVERAGE_EDGE_TOL"); }
        }

        /// <summary>
        /// Gets the maximum edge tolerance of the Solid
        /// </summary>
        public double MaximumEdgeTolerance
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].MAXIMUM_EDGE_TOL"); }
        }

        /// <summary>
        /// Gets the number of analytic surfaces in the Solid
        /// </summary>
        public int NumberAnalyticSurfaces
        {
            get { return _powerSHAPE.ReadIntValue(Identifier + "[ID " + _id + "].ANALYTIC_SURFS"); }
        }

        /// <summary>
        /// Gets the tolerance to which the Solid's half edges are known to link
        /// </summary>
        public double HalfEdgeTolerance
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].TOLERANCE"); }
        }

        /// <summary>
        /// Gets and Sets the material of the Solid in PowerSHAPE
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

        #region " Operations "

        /// <summary>
        /// Deletes the Solid from PowerSHAPE and removes it from the Solids collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Solids.Remove(this);
        }

        /// <summary>
        /// Returns a DMT Model of the solid
        /// </summary>
        /// <returns>A DMT Model of the solid</returns>
        public DMTModel ToDMTModel()
        {
            // Write to a DMT file
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("dmt");
            WriteToDMTFile(tempFile);

            // Read in the model
            var model = DMTModelReader.ReadFile(tempFile);

            // Delete the file
            tempFile.Delete();

            // Return the model
            return model;
        }

        /// <summary>
        /// Exports the solid to the selected STL file
        /// </summary>
        /// <param name="file">The STL file name to export to</param>
        public void WriteToSTLFile(FileSystem.File file)
        {
            if (file.Extension.ToLower() == "stl")
            {
                file.Delete();
                AddToSelection(true);
                _powerSHAPE.DoCommand("FILE EXPORT '" + file.Path + "'");
            }
            else
            {
                throw new Exception("The selected file name is not a valid STL file name");
            }
        }

        /// <summary>
        /// Exports the solid to the selected DMT file
        /// </summary>
        /// <param name="file">The DMT file name to export to</param>
        public void WriteToDMTFile(FileSystem.File file)
        {
            if (file.Extension.ToLower() == "dmt")
            {
                file.Delete();
                AddToSelection(true);
                _powerSHAPE.DoCommand("FILE EXPORT '" + file.Path + "'");
            }
            else
            {
                throw new Exception("The selected file name is not a valid DMT file name");
            }
        }

        /// <summary>
        /// Splits the solid with a surface
        /// </summary>
        /// <param name="surfaceForSplit">The surface with which to split the solid</param>
        /// <returns>A list of the solid(s) created by the operation</returns>
        public List<PSSolid> SplitSolidWithSurface(PSSurface surfaceForSplit)
        {
            // Ensure the solid is currently active
            IsActive = true;

            // Select the surface
            surfaceForSplit.AddToSelection(true);

            // Split solid with surface
            _powerSHAPE.DoCommand("CREATE FEATURE SPLITTREE");

            // Check that the operation has worked
            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                throw new ApplicationException("The solid was not split with this surface");
            }

            // Remove the surface from the collection
            surfaceForSplit.Delete();

            // Select all solids
            _powerSHAPE.ActiveModel.Solids.AddToSelection(true);

            // Get all created solids
            List<PSSolid> createdSolids = new List<PSSolid>();
            foreach (PSSolid createdSolid in _powerSHAPE.ActiveModel.SelectedItems.Cast<PSSolid>().ToList())
            {
                if (_powerSHAPE.ActiveModel.Solids.Contains(createdSolid) == false)
                {
                    _powerSHAPE.ActiveModel.Solids.Add(createdSolid);
                    createdSolids.Add(createdSolid);
                }
            }

            return createdSolids;
        }

        /// <summary>
        /// Applies a defined fillet to edges manually selected by the user
        /// </summary>
        /// <param name="radius">The desired radius of the fillet</param>
        public void FilletEdges(MM radius)
        {
            // Ensure current Solid is active
            IsActive = true;

            // Get original volume
            float initialVolume = (float) Volume;

            // Raise filleting dialog
            _powerSHAPE.DoCommand("CREATE FEATURE FILLET");
            _powerSHAPE.DoCommand("RADIUS " + radius.ToString("0.######"));

            // Prompt user to select edges
            Interaction.MsgBox("Pick the edges to be filleted, and then click OK");

            // Carry out operation
            _powerSHAPE.DoCommand("APPLY");

            // Check that filleting has been completed successfully
            if (Volume == initialVolume)
            {
                throw new ApplicationException("Subtraction has not been completed successfully");
            }

            // Complete filleting
            _powerSHAPE.DoCommand("DISMISS");
        }

        /// <summary>
        /// Gets the solid type, so removing the code from the property and removing the
        /// chance that the property can throw an exception
        /// </summary>
        /// <param name="powerSHAPE">The current instance of PowerSHAPE</param>
        /// <param name="id">The ID of the solid to interrogate</param>
        /// <returns>The type of the solid as a SolidType</returns>
        public static SolidTypes GetSolidType(PSAutomation powerSHAPE, int id)
        {
            string psType = powerSHAPE.ReadStringValue("SOLID[ID " + id + "].TYPE");
            return (SolidTypes) Enum.Parse(typeof(SolidTypes), psType, true);
        }

        /// <summary>
        /// Reverses the normal of the Surface
        /// </summary>
        public void ReverseNormal()
        {
            AbortIfDoesNotExist();

            // Select only this surface
            AddToSelection(true);

            // Carry out operation
            _powerSHAPE.DoCommand("REVERSE");
        }

        #region " Edit Operations "

        /// <summary>
        /// Limits a solid by a specified list of entities
        /// </summary>
        /// <param name="limitingEntity">The entity with which to perform the limiting operation</param>
        /// <param name="limitingMode">The mode in which to carry out the operation</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit</param>
        /// <param name="trimOption">Whether to trim one or all of the entities</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>List of solids created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
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
        /// Limits a solid by a specified list of entities.
        /// </summary>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation.</param>
        /// <param name="limitingMode">The mode in which to carry out the operation.</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit.</param>
        /// <param name="trimOption">Whether to trim one or all of the entities.</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>List of solids created by the operation.  If numberOfCopies is 0, an empty list is returned.</returns>
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
        /// This operation mirrors a single solid in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the solid</param>
        /// <param name="mirrorPoint">The origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// This operation moves a single solid by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>List of solids created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// This operation moves a single solid by a specified relative amount
        /// </summary>
        /// <param name="moveVector">The relative amount by which the solid will be moved</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>List of solids created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// This operation rotates a single solid by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">The axis around which the solid is are to be rotated</param>
        /// <param name="rotationAngle">The angle by which the solid is to be rotated</param>
        /// <param name="copiesToCreate">The number of copies to create of the original solid</param>
        /// <param name="rotationOrigin">The origin of the rotation axis</param>
        /// <returns>A list of solids created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// This operation offsets a single solid by a specified distance
        /// </summary>
        /// <param name="offsetDistance">The distance by which to offset the solid</param>
        /// <param name="copiesToCreate">The number of copies to be created from the operation</param>
        /// <returns>A list of offset solids</returns>
        public List<PSEntity> Offset(MM offsetDistance, int copiesToCreate)
        {
            return PSEntityOffseter.OffsetEntity(this, offsetDistance, copiesToCreate);
        }

        /// <summary>
        /// Scales the solid the same amount in each axis
        /// </summary>
        /// <param name="scaleFactor">Scale factor for the axes</param>
        /// <param name="lockX">Whether to use the X scale or not</param>
        /// <param name="lockY">Whether to use the Y scale or not</param>
        /// <param name="lockZ">Whether to use the Z scale or not</param>
        /// <param name="scaleOrigin">Origin for the scale operation</param>
        public void ScaleUniform(double scaleFactor, bool lockX, bool lockY, bool lockZ, Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleUniform(this, scaleFactor, lockX, lockY, lockZ, scaleOrigin);
        }

        /// <summary>
        /// Scales the solid a different amount in each axis
        /// </summary>
        /// <param name="scaleFactorX">Scale factor for the X axis</param>
        /// <param name="scaleFactorY">Scale factor for the Y axis</param>
        /// <param name="scaleFactorZ">Scale factor for the Z axis</param>
        /// <param name="lockX">Whether to use the X scale or not</param>
        /// <param name="lockY">Whether to use the Y scale or not</param>
        /// <param name="lockZ">Whether to use the Z scale or not</param>
        /// <param name="scaleOrigin">Origin for the scale operation</param>
        public void ScaleNonUniform(
            double scaleFactorX,
            double scaleFactorY,
            double scaleFactorZ,
            bool lockX,
            bool lockY,
            bool lockZ,
            Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleNonUniform(this, scaleFactorX, scaleFactorY, scaleFactorZ, lockX, lockY, lockZ, scaleOrigin);
        }

        /// <summary>
        /// Scales the solid to a projected volume
        /// </summary>
        /// <param name="newVolume">Volume to scale to</param>
        /// <param name="lockX">Whether to use the X scale or not</param>
        /// <param name="lockY">Whether to use the Y scale or not</param>
        /// <param name="lockZ">Whether to use the Z scale or not</param>
        /// <param name="scaleOrigin">Origin for the scale operation</param>
        public void ScaleProjectedVolume(double newVolume, bool lockX, bool lockY, bool lockZ, Point scaleOrigin = null)
        {
            PSEntityScaler.ScaleProjectedVolume(this, newVolume, lockX, lockY, lockZ, scaleOrigin);
        }

        #endregion

        #region " Add/Intersect/Subtract Operations "

        /// <summary>
        /// Adds an entity to another entity
        /// </summary>
        /// <param name="entityToAdd">Entity to be added</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False</param>
        /// <returns>Entity created from the addition</returns>
        public PSEntity AddEntityToEntity(IPSAddable entityToAdd, bool copeWithCoincidentFaces = false)
        {
            // Ensure current Solid is active
            IsActive = true;

            // If the entity is a solid then we need to ensure that the other entity is the same version as this one
            if (entityToAdd is PSSolid)
            {
                PSSolid solidToAdd = null;
                solidToAdd = (PSSolid) entityToAdd;
                if (solidToAdd.Version != Version)
                {
                    solidToAdd.Version = Version;
                }
            }

            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE ADDITION");

            // Add addition solid to selection
            PSEntity entity = (PSEntity) entityToAdd;
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
                _powerSHAPE.ActiveModel.Add(createdMesh);

                // Remove the lost solids
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
        /// Adds entities to another entity
        /// </summary>
        /// <param name="entitiesToAdd">Collection of entities to be added</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False</param>
        /// <returns>Entity created from the addition</returns>
        public PSEntity AddEntitiesToEntity(IEnumerable<IPSAddable> entitiesToAdd, bool copeWithCoincidentFaces = false)
        {
            // Ensure current Solid is active
            IsActive = true;

            // If any of the entities are a solid then we need to ensure that the other entity is the same version as this one
            foreach (PSEntity entity in entitiesToAdd)
            {
                if (entity is PSSolid)
                {
                    PSSolid solid = null;
                    solid = (PSSolid) entity;
                    if (solid.Version != Version)
                    {
                        solid.Version = Version;
                    }
                }
            }

            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE ADDITION");

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
                _powerSHAPE.ActiveModel.Add(createdMesh);

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
        /// Intersects an entity with another entity
        /// </summary>
        /// <param name="entityToIntersect">Entity to intersect</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False</param>
        /// <returns>Entity created from the intersection</returns>
        public PSEntity IntersectEntityWithEntity(IPSIntersectable entityToIntersect, bool copeWithCoincidentFaces = false)
        {
            // Ensure current Solid is active
            IsActive = true;

            // If the entity is a solid then we need to ensure that the other entity is the same version as this one
            if (entityToIntersect is PSSolid)
            {
                PSSolid solidToAdd = null;
                solidToAdd = (PSSolid) entityToIntersect;
                if (solidToAdd.Version != Version)
                {
                    solidToAdd.Version = Version;
                }
            }

            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE INTERSECTION");

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
                _powerSHAPE.ActiveModel.Add(createdMesh);

                // Remove the lost solids
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
        /// Intersects a collection of entities with another entity
        /// </summary>
        /// <param name="entitiesToIntersect">Entities to intersect</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False</param>
        /// <returns>Entity created from the intersection</returns>
        public List<PSEntity> IntersectEntitiesWithEntity(
            IEnumerable<IPSIntersectable> entitiesToIntersect,
            bool copeWithCoincidentFaces = false)
        {
            // Ensure current Solid is active
            IsActive = true;

            // If any of the entities are a solid then we need to ensure that the other entity is the same version as this one
            foreach (PSEntity entity in entitiesToIntersect)
            {
                if (entity is PSSolid)
                {
                    PSSolid solid = null;
                    solid = (PSSolid) entity;
                    if (solid.Version != Version)
                    {
                        solid.Version = Version;
                    }
                }
            }

            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE INTERSECTION");

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
                if (_powerSHAPE.ActiveModel.SelectedItems.Count == 1)
                {
                    // Could be that something happened.  See if any of the entities to delete are gone
                    PSEntity possibleCreated = _powerSHAPE.ActiveModel.SelectedItems[0];
                    bool itemDeleted = false;
                    foreach (PSEntity entity in entitiesToIntersect)
                    {
                        if (entity.Exists == false)
                        {
                            itemDeleted = true;
                            entity.Delete();
                        }
                    }
                    if (itemDeleted)
                    {
                        Id = possibleCreated.Id;
                        Name = possibleCreated.Name;
                        List<PSEntity> createdItems = new List<PSEntity>();
                        createdItems.Add(this);

                        // Check to see if any other solids got created
                        // Select all the solids
                        _powerSHAPE.DoCommand("QUICK QUICKSELECTSOLIDONLY");

                        // See if there are any that aren't in the list and if there are then add them
                        foreach (PSSolid solid in _powerSHAPE.ActiveModel.SelectedItems)
                        {
                            if (_powerSHAPE.ActiveModel.Solids.Contains(solid) == false)
                            {
                                _powerSHAPE.ActiveModel.Solids.Add(solid);
                                createdItems.Add(solid);
                            }
                        }

                        // Just select the resultant solid
                        AddToSelection(true);
                        return createdItems;
                    }
                    return null;
                }
                return null;
            }
            if (ReferenceEquals(_powerSHAPE.ActiveModel.CreatedItems[0].GetType(), typeof(PSMesh)))
            {
                // Check whether a mesh has been created (occurs if PowerSHAPE was adding a mesh to a solid)
                PSMesh createdMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];
                _powerSHAPE.ActiveModel.Add(createdMesh);

                // Remove the lost solids
                foreach (PSEntity entity in entitiesToIntersect)
                {
                    if (entity.Exists == false)
                    {
                        entity.Delete();
                    }
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
        /// Subtracts an entity from another entity
        /// </summary>
        /// <param name="entityToSubtract">Entity to subtract</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False</param>
        /// <returns>Entity created from the subtraction</returns>
        public PSEntity SubtractEntityFromEntity(IPSSubtractable entityToSubtract, bool copeWithCoincidentFaces = false)
        {
            // Ensure current Solid is active
            IsActive = true;

            // If the entity is a solid then we need to ensure that the other entity is the same version as this one
            if (entityToSubtract is PSSolid)
            {
                PSSolid solidToAdd = null;
                solidToAdd = (PSSolid) entityToSubtract;
                if (solidToAdd.Version != Version)
                {
                    solidToAdd.Version = Version;
                }
            }

            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE SUBTRACTION");

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
                _powerSHAPE.ActiveModel.Add(createdMesh);

                // Remove the lost solids
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
        /// Subtracts a collection of entities from another entity
        /// </summary>
        /// <param name="entitiesToSubtract">Entities to subtract</param>
        /// <param name="copeWithCoincidentFaces">Optional boolean defaults to False</param>
        /// <returns>Entity created from the subtraction</returns>
        public PSEntity SubtractEntitiesFromEntity(
            IEnumerable<IPSSubtractable> entitiesToSubtract,
            bool copeWithCoincidentFaces = false)
        {
            // Ensure current Solid is active
            IsActive = true;

            // If any of the entities are a solid then we need to ensure that the other entity is the same version as this one
            foreach (PSEntity entity in entitiesToSubtract)
            {
                if (entity is PSSolid)
                {
                    PSSolid solid = null;
                    solid = (PSSolid) entity;
                    if (solid.Version != Version)
                    {
                        solid.Version = Version;
                    }
                }
            }

            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
            _powerSHAPE.ActiveModel.ClearSelectedItems();
            _powerSHAPE.DoCommand("CREATE FEATURE SUBTRACTION");

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
                _powerSHAPE.ActiveModel.Add(createdMesh);

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

        #endregion
    }
}