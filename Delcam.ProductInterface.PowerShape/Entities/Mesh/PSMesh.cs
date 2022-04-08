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
    /// Captures a Mesh object in PowerSHAPE
    /// </summary>
    public class PSMesh : PSEntity, IPSMoveable, IPSRotateable, IPSMirrorable, IPSOffsetable, IPSLimitable, IPSAddable,
                          IPSIntersectable, IPSSubtractable, IPSScalable
    {
        #region " Fields "

        /// <summary>
        /// Material that is currently applied to the Mesh
        /// </summary>
        private PSMaterial _material;

        /// <summary>
        /// Indicates whether or not we are in a sculpting mode
        /// </summary>
        private bool _isSculpting;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Mesh.
        /// </summary>
        /// <param name="powerSHAPE">This is the PowerSHAPE Automation object.</param>
        /// <param name="id">The ID of the new model.</param>
        /// <param name="name">The name of the new model.</param>
        /// <remarks></remarks>
        internal PSMesh(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a new Mesh from a DMT Model.
        /// </summary>
        /// <param name="powerSHAPE">This is the PowerSHAPE Automation object.</param>
        /// <param name="model">The DMT model from which to create the model.</param>
        internal PSMesh(PSAutomation powerSHAPE, DMTModel model) : base(powerSHAPE)
        {
            // Write the DMT to a temporary file
            FileSystem.File tempFile = FileSystem.File.CreateTemporaryFile("dmt");
            DMTModelWriter.WriteFile(model, tempFile);

            // Import the DMT into PowerSHAPE
            _powerSHAPE.DoCommand("FILE IMPORT '" + tempFile.Path + "'");

            // Delete the temporary file
            tempFile.Delete();

            PSMesh newMesh = (PSMesh) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Set the Id
            _id = newMesh.Id;
        }

        #endregion

        #region " Properties "

        internal const string MESH_IDENTIFIER = "SYMBOL";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return MESH_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 60 * 20000000 + _id; }
        }

        /// <summary>
        /// Gets and Sets the material of the Mesh in PowerSHAPE
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

        /// <summary>
        /// Gets and Sets the centre of the Mesh in PowerSHAPE based on the active Workplane
        /// </summary>
        public Point Centre
        {
            get
            {
                // Get the DMT for the Mesh
                DMTModel model = ToDMTModel();

                // If we are in anything other than the active workplane then set the active workplane of
                // the dmt file
                if (_powerSHAPE.ActiveModel.ActiveWorkplane != null)
                {
                    model.RebaseToWorkplane(_powerSHAPE.ActiveModel.ActiveWorkplane.ToWorkplane());
                }

                // Get the centre of the DMT
                return model.BoundingBox.VolumetricCentre;
            }

// Move the model to the specified value

            set { MoveByVector(value - Centre, 0); }
        }

        /// <summary>
        /// Gets the area of the Mesh
        /// </summary>
        public double Area
        {
            get { return _powerSHAPE.ReadDoubleValue(Identifier + "['" + Name + "'].AREA"); }
        }

        /// <summary>
        /// Gets the number of triangles in the mesh
        /// </summary>
        /// <returns></returns>
        public int NumberOfTriangles
        {
            get
            {
                string symbolDef = _powerSHAPE.ReadStringValue(Identifier + "['" + Name + "'].SYMBOL_DEF");
                return _powerSHAPE.ReadIntValue("SYMBOL_DEF['" + symbolDef + "'].NUMBER_TRIANGLES");
            }
        }

        /// <summary>
        /// Gets the number of nodes in the mesh
        /// </summary>
        /// <returns></returns>
        public int NumberOfNodes
        {
            get
            {
                string symbolDef = _powerSHAPE.ReadStringValue(Identifier + "['" + Name + "'].SYMBOL_DEF");
                return _powerSHAPE.ReadIntValue("SYMBOL_DEF['" + symbolDef + "'].NUMBER_NODES");
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the Mesh from PowerSHAPE and removes it from the Meshes collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Meshes.Remove(this);
        }

        /// <summary>
        /// Adds draft angles to the mesh down to the origin of the current workplane
        /// </summary>
        /// <param name="draftAngle">The draft angle to use</param>
        /// <param name="createWallsOnly">Whether or not to only add the draft walls</param>
        public void CreateDraft(double draftAngle, bool createWallsOnly)
        {
            // Select the model
            AddToSelection(true);

            // Apply the draft
            string yesNo = null;
            if (createWallsOnly)
            {
                yesNo = "ON";
            }
            else
            {
                yesNo = "OFF";
            }
            _powerSHAPE.DoCommand("DRAFTWALL", "ANGLE " + draftAngle, "WALLSONLY " + yesNo, "COPY OFF", "ACCEPT");
        }

        /// <summary>
        /// Creates a boundary curve object
        /// </summary>
        /// <param name="location">Optional location defaulted to 1</param>
        /// <param name="smoothness">Optional smoothness defaulted to 0</param>
        /// <returns></returns>
        public List<PSCompCurve> CreateBoundary(int location = 1, int smoothness = 0)
        {
            AddToSelection(true);

            _powerSHAPE.DoCommand("CREATE CURVE FROM_MESH_BOUNDARIES");

            // Set parameters
            _powerSHAPE.DoCommand("LOCATION " + location);
            _powerSHAPE.DoCommand("SMOOTHNESS " + smoothness);

            _powerSHAPE.DoCommand("ACCEPT");

            if (_powerSHAPE.ActiveModel.CreatedItems.Count == 0)
            {
                throw new Exception("Failed to create boundary");
            }

            List<PSCompCurve> boundaries = new List<PSCompCurve>();

            foreach (PSCompCurve boundary in _powerSHAPE.ActiveModel.CreatedItems)
            {
                _powerSHAPE.ActiveModel.Add(boundary);
                boundaries.Add(boundary);
            }

            return boundaries;
        }

        /// <summary>
        /// Appends the specified Mesh to this Mesh. It will leave a single
        /// Mesh, deleting the specified one.
        /// </summary>
        /// <param name="meshToAppend">The mesh to be appended to this mesh</param>
        public void Append(PSMesh meshToAppend)
        {
            // Append the two meshes
            AddToSelection(true);
            meshToAppend.AddToSelection();
            _powerSHAPE.DoCommand("TRIAPPEND");

            // This creates a brand new mesh (the original two are now deleted)
            // Remove the specified one from the list of meshes
            _powerSHAPE.ActiveModel.Meshes.Remove(meshToAppend);

            // Get the new one
            PSMesh objNewMesh = (PSMesh) _powerSHAPE.ActiveModel.SelectedItems[0];

            // This mesh is now deleted so put the new one into this one so it appears to the user
            // as though the mesh is the original one
            _id = objNewMesh.Id;

            // Set the name of the new mesh to be the name this one was before it got deleted
            objNewMesh.Name = _name;
        }

        /// <summary>
        /// Splits the mesh into seperate meshes.  If the mesh cannot be divided further than itself then
        /// only one mesh (the start mesh) is returned.  Otherwise all the new created meshes are returned
        /// and this mesh fails to exist
        /// </summary>
        /// <returns>
        /// If the mesh cannot be divided further than itself then only one mesh (the start mesh) is
        /// returned.  Otherwise all the new created meshes are returned and this mesh fails to exist
        /// </returns>
        public List<PSMesh> Split()
        {
            // Select the mesh
            AddToSelection(true);

            // Split it
            _powerSHAPE.DoCommand("TRIDIVIDE");
            _powerSHAPE.DoCommand("YES");

            // Get the selected models
            List<PSEntity> selectedMeshes = _powerSHAPE.ActiveModel.SelectedItems;

            // If only one mesh is selected then there was nothing to divide into
            // Otherwise we need to delete the current item from the list of meshes and add the new ones
            List<PSMesh> returnedMeshes = new List<PSMesh>();
            if (selectedMeshes.Count == 1)
            {
                returnedMeshes.Add((PSMesh) selectedMeshes[0]);
            }
            else
            {
                Delete();
                foreach (PSMesh dividedMesh in selectedMeshes)
                {
                    _powerSHAPE.ActiveModel.Add(dividedMesh);
                    returnedMeshes.Add(dividedMesh);
                }
            }

            // Return the meshes
            return returnedMeshes;
        }

        /// <summary>
        /// Stitches the triangle mesh based on the specified values.
        /// </summary>
        /// <param name="iterationMode">The iteration mode is either Single or Multipass.</param>
        /// <param name="startTolerance">Default 0.1.</param>
        /// <param name="endTolerance">Default 0.1.</param>
        /// <param name="numberOfSteps">Default 10.</param>
        /// <param name="stitchFullMesh">If true, executes 'EXTRACTBOUNDARY' turning it off and executes 'REMOVECOINCIDENT' command according with 'removeCoincidentNodes' option.</param>
        /// <param name="removeCoincidentNodes">Executes 'REMOVECOINCIDENT' command turning it on or off.</param>
        /// <remarks></remarks>
        public void StitchTriangles(
            TriangleStitchIterationModes iterationMode,
            double startTolerance = 0.1,
            double endTolerance = 0.1,
            uint numberOfSteps = 10,
            bool stitchFullMesh = false,
            bool removeCoincidentNodes = false)
        {
            AddToSelection(true);

            _powerSHAPE.DoCommand("FIX STITCH");

            if (iterationMode == TriangleStitchIterationModes.SinglePass)
            {
                _powerSHAPE.DoCommand("ITERATION SINGLE");
            }
            else
            {
                _powerSHAPE.DoCommand("ITERATION MULTI");
            }

            if (stitchFullMesh)
            {
                _powerSHAPE.DoCommand("EXTRACTBOUNDARY OFF");
                if (removeCoincidentNodes)
                {
                    _powerSHAPE.DoCommand("REMOVECOINCIDENT ON");
                }
                else
                {
                    _powerSHAPE.DoCommand("REMOVECOINCIDENT OFF");
                }
            }
            else
            {
                _powerSHAPE.DoCommand("EXTRACTBOUNDARY ON");
            }

            _powerSHAPE.DoCommand("TOLERANCE START " + startTolerance.ToString("0.######"),
                                  "TOLERANCE END " + endTolerance.ToString("0.######"),
                                  "TOLERANCE STEPS " + numberOfSteps,
                                  "ACCEPT");
        }

        /// <summary>
        /// Allows the user to select a hole on the mesh and fill it
        /// </summary>
        public void FillHole()
        {
            // Pick a point that will define where the hole is
            Point pickedPoint = _powerSHAPE.PickPoint();

            // Create a CompCurve around the hole
            _powerSHAPE.DoCommand("CREATE CURVE COMPCURVE");
            _powerSHAPE.DoCommand(pickedPoint.X.ToString("0.######") + " " + pickedPoint.Y.ToString("0.######") + " " + pickedPoint.Z.ToString("0.######"));
            _powerSHAPE.DoCommand("FASTFORWARDS");
            _powerSHAPE.DoCommand("SAVE");

            PSCompCurve createdCompCurve = (PSCompCurve) _powerSHAPE.ActiveModel.CreatedItems[0];
            createdCompCurve.AddToSelection(true);

            // The curve must be closed for this to work
            if (createdCompCurve.IsClosed)
            {
                _powerSHAPE.DialogsOff();

                // Create a fillin surface from the curve
                PSSurface fillinSurface = _powerSHAPE.ActiveModel.Surfaces.CreateFillInSurface(createdCompCurve);
                PSMesh fillinMesh = _powerSHAPE.ActiveModel.Meshes.CreateMeshFromSurface(fillinSurface);
                fillinSurface.Delete();
                fillinMesh.AddToSelection(true);

                // Append the fillin mesh to this mesh
                Append(fillinMesh);

                // Delete the curve that was created
                createdCompCurve.Delete();

                _powerSHAPE.DialogsOn();
            }
            else
            {
                // Delete the curve that was created and throw an exception
                createdCompCurve.Delete();
                _powerSHAPE.DialogsOn();
                throw new Exception("Unable to create closed curve");
            }
        }

        /// <summary>
        /// Starts the interactive sculpting tool. It is finished
        /// or aborted using the ApplyScult and CancelSculpt operations
        /// </summary>
        /// <param name="sculptType">This is type of sculpt to apply</param>
        /// <param name="brushDiameter">This is the diameter of the brush</param>
        /// <param name="brushStrength">This is the strength of the sculpt</param>
        /// <param name="brushSmoothness">This is the smoothness of the sculpt</param>
        /// <param name="limitDeviation">This determines whether or not to limit the amount the sculpt will change the mesh</param>
        /// <param name="maximumDeviation">The maximum distance the sculpt will change the mesh by</param>
        /// <param name="refineTriangles">This determines whether or not to refine the triangles</param>
        /// <param name="refineTolerance">In conjunction with refineTriangles specifies the tolerance for refinement</param>
        public void StartSculpt(
            SculptTypes sculptType,
            float brushDiameter,
            float brushStrength,
            float brushSmoothness,
            bool limitDeviation = false,
            double maximumDeviation = 1.0,
            bool refineTriangles = false,
            float refineTolerance = 10.0f)
        {
            // Check to see if we are already sculpting
            if (_isSculpting == false)
            {
                // Select the mesh and raise the form
                AddToSelection(true);
                _powerSHAPE.DoCommand("EDIT_TRIANGLES");
            }

            // Set the scult type, brush shape and general parameters
            _powerSHAPE.DoCommand("STRATEGY " + sculptType.ToString().ToUpper(),
                                  "OPTION DIAMETER",
                                  "CHANGE_OPT " + brushDiameter,
                                  "OPTION STRENGTH",
                                  "CHANGE_OPT " + brushStrength,
                                  "OPTION SMOOTHNESS",
                                  "CHANGE_OPT " + brushSmoothness);

            // Limit deviation
            if (limitDeviation)
            {
                _powerSHAPE.DoCommand("LIMDEV ON", "SET_MAX_DEV " + maximumDeviation);
            }
            else
            {
                _powerSHAPE.DoCommand("LIMDEV OFF");
            }

            // Refine triangles
            if (refineTriangles)
            {
                _powerSHAPE.DoCommand("REFINE ON", "REF_TOL " + refineTolerance);
            }
            else
            {
                _powerSHAPE.DoCommand("REFINE OFF");
            }

            _isSculpting = true;
        }

        /// <summary>
        /// Accepts the last StartSculpt to the mesh
        /// </summary>
        public void AcceptSculpt()
        {
            if (_isSculpting)
            {
                _powerSHAPE.DoCommand("ACCEPT");
                _isSculpting = false;
            }
        }

        /// <summary>
        /// Cancels the last StartSculpt
        /// </summary>
        public void CancelSculpt()
        {
            if (_isSculpting)
            {
                _powerSHAPE.DoCommand("CANCEL");
                _isSculpting = false;
            }
        }

        /// <summary>
        /// Detects and fixes the faults in the mesh
        /// </summary>
        /// <param name="fixOption">This option determines the type of fix/operation the mesh doctor will do. If 'All' is selected it detects and fixes all types of faults, otherwise the Mesh Doctor identifies and fixes the select fault type. The option 'Status' gives the status of the faults.</param>
        /// <param name="repairType">This option allows to choose between an automation repair and an expand superfault option.</param>
        public void RunMeshDoctor(MeshDoctorFixOptions fixOption, MeshDoctorRepairTypes repairType)
        {
            AddToSelection(true);

            switch (fixOption)
            {
                case MeshDoctorFixOptions.All:
                    _powerSHAPE.DoCommand("Fix MeshDoctor");
                    break;
                case MeshDoctorFixOptions.RestrictBadConnection:
                    _powerSHAPE.DoCommand("Fix MeshDoctor Restrict BadConnection");
                    break;
                case MeshDoctorFixOptions.RestrictBoundary:
                    _powerSHAPE.DoCommand("Fix MeshDoctor Restrict Boundary");
                    break;
                case MeshDoctorFixOptions.RestrictAspect:
                    _powerSHAPE.DoCommand("Fix MeshDoctor Restrict Aspect");
                    break;
                case MeshDoctorFixOptions.RestrictSmall:
                    _powerSHAPE.DoCommand("Fix MeshDoctor Restrict Small");
                    break;
                case MeshDoctorFixOptions.RestrictIntersect:
                    _powerSHAPE.DoCommand("Fix MeshDoctor Restrict Intersect");
                    break;
                case MeshDoctorFixOptions.RestrictOverlap:
                    _powerSHAPE.DoCommand("Fix MeshDoctor Restrict Overlap");
                    break;
                case MeshDoctorFixOptions.Status:
                    _powerSHAPE.DoCommand("Fix MeshDoctorStatus");

                    break;
            }

            switch (repairType)
            {
                case MeshDoctorRepairTypes.AttemptAutomaticRepair:
                    _powerSHAPE.DoCommand("Repair Attempt automatic repair");
                    break;
                case MeshDoctorRepairTypes.ExpandSuperfault:
                    _powerSHAPE.DoCommand("Repair Expand superfault");

                    break;
            }

            _powerSHAPE.DoCommand("APPLY");
            _powerSHAPE.DoCommand("ACCEPT");
            _powerSHAPE.DoCommand("FINISH");
        }

        /// <summary>
        /// Returns a DMT Model of the mesh
        /// </summary>
        /// <returns>A DMT Model of the Mesh</returns>
        public DMTModel ToDMTModel()
        {
            // Select the mesh
            AddToSelection(true);

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
        /// Exports the mesh to the selected STL file
        /// </summary>
        /// <param name="file">The STL file name to export to</param>
        public void WriteToSTLFile(FileSystem.File file)
        {
            if (file.Extension.ToLower() == "stl")
            {
                file.Delete();
                AddToSelection(true);
                _powerSHAPE.ActiveModel.Export(file, ExportItemsOptions.Selected, ExportWorkplanes.Active);
            }
            else
            {
                throw new Exception("The selected file name is not a valid STL file name");
            }
        }

        /// <summary>
        /// Exports the mesh to the selected DMT file
        /// </summary>
        /// <param name="file">The DMT file name to export to</param>
        public void WriteToDMTFile(FileSystem.File file)
        {
            if (file.Extension.ToLower() == "dmt")
            {
                file.Delete();
                AddToSelection(true);
                _powerSHAPE.ActiveModel.Export(file, ExportItemsOptions.Selected, ExportWorkplanes.Active);
            }
            else
            {
                throw new Exception("The selected file name is not a valid DMT file name");
            }
        }

        #region " Edit Operations "

        /// <summary>
        /// Limits a mesh by a specified list of entities
        /// </summary>
        /// <param name="limitingEntity">The entity with which to perform the limiting operation</param>
        /// <param name="limitingMode">The mode in which to carry out the operation</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit</param>
        /// <param name="trimOption">Whether to trim one or all of the entities</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>A list containing the limited entity</returns>
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
        /// This operation limits a mesh by a specified list of entities
        /// </summary>
        /// <param name="limitingEntities">The entities with which to perform the limiting operation</param>
        /// <param name="limitingMode">The mode in which to carry out the operation</param>
        /// <param name="keepOption">Whether to keep one or both sides of the limit</param>
        /// <param name="trimOption">Whether to trim one or all of the entities</param>
        /// <param name="finishOperation">If true, turns edit selection off.</param>
        /// <returns>A list containing the limited entity</returns>
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
        /// This operation mirrors a single mesh in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the mesh</param>
        /// <param name="mirrorPoint">The origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// This operation moves a single mesh by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>A list containing the entity moved</returns>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// This operation moves a single mesh by a specified relative amount
        /// </summary>
        /// <param name="moveVector">The relative amount by which the mesh will be moved</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>A list containing the entity moved</returns>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// This operation rotates a single mesh by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">The axis around which the mesh is are to be rotated</param>
        /// <param name="rotationAngle">The angle by which the mesh is to be rotated</param>
        /// <param name="copiesToCreate">The number of copies to create of the original mesh</param>
        /// <param name="rotationOrigin">The origin of the rotation axis</param>
        /// <returns>A list of meshs created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// This operation offsets a single mesh by a specified distance
        /// </summary>
        /// <param name="offsetDistance">The distance by which to offset the mesh</param>
        /// <param name="copiesToCreate">The number of copies to be created from the operation</param>
        /// <returns>A list of offset meshes</returns>
        public List<PSEntity> Offset(MM offsetDistance, int copiesToCreate)
        {
            return PSEntityOffseter.OffsetEntity(this, offsetDistance, copiesToCreate);
        }

        /// <summary>
        /// Reduces the Mesh maintaining the specified tolerance
        /// </summary>
        /// <param name="tolerace">Tolerance to maintain during reduction</param>
        /// <param name="limitTriangleEdgeLength">Whether or not to limit the length of edges</param>
        /// <param name="maximumEdgeLength">The maximum edge length to reduce to.  Only applies if limitTriangleEdgeLength is true</param>
        public void ReduceToTolerance(double tolerace, bool limitTriangleEdgeLength, MM maximumEdgeLength)
        {
            // Select the mesh
            AddToSelection(true);

            // Then reduce it
            _powerSHAPE.DoCommand("REDUCE", "REDUCETO TOLERANCE", "TOLERANCE " + tolerace.ToString("0.######"));

            if (limitTriangleEdgeLength)
            {
                _powerSHAPE.DoCommand("MAXEDGELENGTH ON", "MAXEDGELENGTH " + maximumEdgeLength.ToString("0.######"));
            }
            else
            {
                _powerSHAPE.DoCommand("MAXEDGELENGTH OFF");
            }

            _powerSHAPE.DoCommand("ACCEPT");
        }

        /// <summary>
        /// Reduces the Mesh by the specified percentage
        /// </summary>
        /// <param name="percentage">Percentage by which to reduce the mesh</param>
        public void ReduceByPercentage(double percentage)
        {
            // Select the mesh
            AddToSelection(true);

            // Then reduce it
            _powerSHAPE.DoCommand("REDUCE", "REDUCETO PERCENTAGE", "PERCENTAGE " + percentage.ToString("0.######"));

            _powerSHAPE.DoCommand("ACCEPT");
        }

        /// <summary>
        /// Scales the mesh a different amount in each axis
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
        /// Scales the mesh to a projected volume
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

        /// <summary>
        /// Scales the mesh the same amount in each axis
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

        #endregion

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

            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                return null;
            }

            // Remove the lost solids
            entity.Delete();

            // Return this Mesh
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

            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                return null;
            }

            // Remove the lost solids
            foreach (PSEntity entity in entitiesToAdd)
            {
                entity.Delete();
            }

            // Return this Mesh
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
            // In order to force the dialog to appear, the command is sent before doing the selection of the entity
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

            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                return null;
            }

            // Remove the lost solids
            entity.Delete();

            // Return this Mesh
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

            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                return null;
            }

            // Remove the lost solids
            foreach (PSEntity entity in entitiesToIntersect)
            {
                entity.Delete();
            }

            // Return this Mesh
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

            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                return null;
            }

            // Remove the lost solids
            entity.Delete();

            // Return this Mesh
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

            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                return null;
            }

            // Remove the lost solids
            foreach (PSEntity entity in entitiesToSubtract)
            {
                entity.Delete();
            }

            // Return this Mesh
            return this;
        }

        #endregion
    }
}