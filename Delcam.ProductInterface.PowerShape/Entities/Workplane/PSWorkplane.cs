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

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures an Workplane object in PowerSHAPE
    /// </summary>
    public class PSWorkplane : PSEntity, IPSMoveable, IPSRotateable, IPSMirrorable
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Workplane
        /// </summary>
        internal PSWorkplane(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Initialises the Workplane using the properties of a
        /// Autodesk.Geometry workplane
        /// </summary>
        internal PSWorkplane(PSAutomation powerSHAPE, Geometry.Workplane inputWorkplane) : base(powerSHAPE)
        {
            // If there is an active workplane then this will need to be active when the new workplanes
            // axes are being configured
            PSWorkplane previousWorkplane = _powerSHAPE.ActiveModel.ActiveWorkplane;

            _powerSHAPE.DoCommand("CREATE WORKPLANE SINGLE " + inputWorkplane.Origin);

            // Get the new workplane
            PSWorkplane newWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Activate the previous workplane
            _powerSHAPE.ActiveModel.ActiveWorkplane = previousWorkplane;

            // Set the axes values
            newWorkplane.XAxis = inputWorkplane.XAxis;
            newWorkplane.YAxis = inputWorkplane.YAxis;
            newWorkplane.ZAxis = inputWorkplane.ZAxis;

            // Get the id of the new workplane
            _name = newWorkplane.Name;
            _id = newWorkplane.Id;

            // Activate the new workplane
            _powerSHAPE.ActiveModel.ActiveWorkplane = newWorkplane;
        }

        /// <summary>
        /// Creates a workplane from three points
        /// </summary>
        internal PSWorkplane(
            PSAutomation powerSHAPE,
            Geometry.Point origin,
            Geometry.Point xAxisPoint,
            Geometry.Point yAxisPoint) : base(powerSHAPE)
        {
            _powerSHAPE.DoCommand("CREATE WORKPLANE THREEPOINTS",
                                  "X " + origin.X.ToString("0.######"),
                                  "Y " + origin.Y.ToString("0.######"),
                                  "Z " + origin.Z.ToString("0.######"),
                                  "X " + xAxisPoint.X.ToString("0.######"),
                                  "Y " + xAxisPoint.Y.ToString("0.######"),
                                  "Z " + xAxisPoint.Z.ToString("0.######"),
                                  "X " + yAxisPoint.X.ToString("0.######"),
                                  "Y " + yAxisPoint.Y.ToString("0.######"),
                                  "Z " + yAxisPoint.Z.ToString("0.######"));
            _powerSHAPE.DoCommand("ACCEPT");

            // Get the new workplane
            PSWorkplane newWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Get the id of the new workplane
            _name = newWorkplane.Name;
            _id = newWorkplane.Id;
        }

        /// <summary>
        /// Creates a workplane that doesn't need an origin
        /// </summary>
        internal PSWorkplane(
            PSAutomation powerSHAPE,
            PSEntity entity,
            NewWorkplaneInEntityPositions workplanePosition) : base(powerSHAPE)
        {
            // Select the entity
            entity.AddToSelection(true);

            // Create the workplane
            string position = "";
            switch (workplanePosition)
            {
                case NewWorkplaneInEntityPositions.Centre:
                    position = "SELECTIONCENTRE";
                    break;
                case NewWorkplaneInEntityPositions.Bottom:
                    position = "SELECTIONBOTTOM";
                    break;
                case NewWorkplaneInEntityPositions.Top:
                    position = "SELECTIONTOP";
                    break;
            }
            _powerSHAPE.DoCommand("CREATE WORKPLANE " + position);

            // Try to get workplane details
            try
            {
                PSWorkplane newWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];
                _name = newWorkplane.Name;
                _id = newWorkplane.Id;
            }
            catch
            {
                throw new ApplicationException("Failed to create Workplane");
            }
        }

        /// <summary>
        /// Creates a workplane aligned to an entity
        /// </summary>
        internal PSWorkplane(PSAutomation powerSHAPE, PSEntity entity, Geometry.Point origin) : base(powerSHAPE)
        {
            // Select the entity
            entity.AddToSelection(true);

            // Create the workplane
            _powerSHAPE.DoCommand("CREATE WORKPLANE NORMALSINGLE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Try to get workplane details
            try
            {
                PSWorkplane newWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];
                _name = newWorkplane.Name;
                _id = newWorkplane.Id;
            }
            catch
            {
                throw new ApplicationException("Failed to create Workplane");
            }
        }

        /// <summary>
        /// Creates a workplane aligned to the current view at the specified origin
        /// </summary>
        internal PSWorkplane(PSAutomation powerSHAPE, Geometry.Point origin) : base(powerSHAPE)
        {
            // Create the workplane
            _powerSHAPE.DoCommand("CREATE WORKPLANE VIEWSINGLE");
            _powerSHAPE.DoCommand(origin.ToString());

            // Try to get workplane details
            try
            {
                PSWorkplane newWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];
                _name = newWorkplane.Name;
                _id = newWorkplane.Id;
            }
            catch
            {
                throw new ApplicationException("Failed to create Workplane");
            }
        }

        #endregion

        #region " Properties "

        internal const string WORKPLANE_IDENTIFIER = "WORKPLANE";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return WORKPLANE_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 54 * 20000000 + _id; }
        }

        /// <summary>
        /// Overrides the BoundingBox property to return a box of volume 0 at the workplane origin
        /// </summary>
        public override Geometry.BoundingBox BoundingBox
        {
            get { return new Geometry.BoundingBox(Origin, Origin); }
        }

        /// <summary>
        /// Gets the active status of workplane.  To set a workplane active use the ActiveWorkplane
        /// property on the Model
        /// </summary>
        public bool IsActive
        {
            get { return _powerSHAPE.ReadBoolValue(Identifier + "['" + Name + "'].ACTIVE"); }
        }

        /// <summary>
        /// Defines the command that'll be used to rename the workplane
        /// </summary>
        internal override string RenameCommand
        {
            get { return "MODIFY"; }
        }

        /// <summary>
        /// Gets and sets the origin of the workplane
        /// </summary>
        public Geometry.Point Origin
        {
            get
            {
                double[] doubleArray = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].ORIGIN") as double[];
                Geometry.MM[] mmArray = new Geometry.MM[3];
                mmArray[0] = doubleArray[0];
                mmArray[1] = doubleArray[1];
                mmArray[2] = doubleArray[2];
                return new Geometry.Point(mmArray);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY",
                                      "X " + value.X.ToString("0.######"),
                                      "Y " + value.Y.ToString("0.######"),
                                      "Z " + value.Z.ToString("0.######"),
                                      "ACCEPT",
                                      "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets whether the workplane is locked
        /// </summary>
        public bool IsLocked
        {
            get { return _powerSHAPE.ReadBoolValue(Identifier + "[ID " + _id + "].LOCKED"); }

            set
            {
                if (value)
                {
                    _powerSHAPE.DoCommand("MODIFY LOCK ACCEPT");
                }
                else
                {
                    _powerSHAPE.DoCommand("MODIFY UNLOCK ACCEPT");
                }
            }
        }

        /// <summary>
        /// Gets and sets the Vector that represents the X Axis for the Workplane
        /// </summary>
        public Geometry.Vector XAxis
        {
            get
            {
                double[] doubleArray = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].XAXIS") as double[];
                Geometry.MM[] mmArray = new Geometry.MM[3];
                mmArray[0] = doubleArray[0];
                mmArray[1] = doubleArray[1];
                mmArray[2] = doubleArray[2];
                return new Geometry.Vector(mmArray);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "XAXIS DIRECTION");

                // Set all values of vector to be 1 to avoid creating a 0 0 0 vector direction at some point in the transformation
                _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");
                _powerSHAPE.DoCommand("X " + value.I.ToString("0.######"),
                                      "Y " + value.J.ToString("0.######"),
                                      "Z " + value.K.ToString("0.######"),
                                      "ACCEPT",
                                      "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the Vector that represents the Y Axis for the Workplane
        /// </summary>
        public Geometry.Vector YAxis
        {
            get
            {
                double[] doubleArray = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].YAXIS") as double[];
                Geometry.MM[] mmArray = new Geometry.MM[3];
                mmArray[0] = doubleArray[0];
                mmArray[1] = doubleArray[1];
                mmArray[2] = doubleArray[2];
                return new Geometry.Vector(mmArray);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "YAXIS DIRECTION");

                // Set all values of vector to be 1 to avoid creating a 0 0 0 vector direction at some point in the transformation
                _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");
                _powerSHAPE.DoCommand("X " + value.I.ToString("0.######"),
                                      "Y " + value.J.ToString("0.######"),
                                      "Z " + value.K.ToString("0.######"),
                                      "ACCEPT",
                                      "ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the Vector that represents the Z Axis for the Workplane
        /// </summary>
        public Geometry.Vector ZAxis
        {
            get
            {
                double[] doubleArray = _powerSHAPE.DoCommandEx(Identifier + "[ID " + _id + "].ZAXIS") as double[];
                Geometry.MM[] mmArray = new Geometry.MM[3];
                mmArray[0] = doubleArray[0];
                mmArray[1] = doubleArray[1];
                mmArray[2] = doubleArray[2];
                return new Geometry.Vector(mmArray);
            }

            set
            {
                AddToSelection(true);
                _powerSHAPE.DoCommand("MODIFY", "ZAXIS DIRECTION");

                // Set all values of vector to be 1 to avoid creating a 0 0 0 vector direction at some point in the transformation
                _powerSHAPE.DoCommand("X 1", "Y 1", "Z 1");
                _powerSHAPE.DoCommand("X " + value.I.ToString("0.######"),
                                      "Y " + value.J.ToString("0.######"),
                                      "Z " + value.K.ToString("0.######"),
                                      "ACCEPT",
                                      "ACCEPT");
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the Workplane from PowerSHAPE and removes it from the Workplanes collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Workplanes.Remove(this);
        }

        /// <summary>
        /// Gets a Geometry Workplane that represents the workplane
        /// </summary>
        /// <returns>The Geometry Workplane that represents the workplane</returns>
        public Geometry.Workplane ToWorkplane()
        {
            return new Geometry.Workplane(Origin, XAxis, YAxis, ZAxis);
        }

        /// <summary>
        /// Aligns the workplane to the current view in PowerSHAPE
        /// </summary>
        public void AlignToView()
        {
            // Select the workplane and align it to view

            // Align to view
            AddToSelection(true);
            _powerSHAPE.DoCommand("MODIFY", "VIEWALIGN", "ACCEPT");
        }

        #region " Edit Operations "

        /// <summary>
        /// Moves a single workplane by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        public List<PSEntity> MoveBetweenPoints(
            Geometry.Point moveOriginCoordinates,
            Geometry.Point pointToMoveTo,
            int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// Moves a single workplane by a specified relative amount
        /// </summary>
        /// <param name="moveVector">The relative amount by which the workplane will be moved</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        public List<PSEntity> MoveByVector(Geometry.Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// Rotates a single workplane by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">The axis around which the workplane is are to be rotated</param>
        /// <param name="rotationAngle">The angle by which the workplane is to be rotated</param>
        /// <param name="copiesToCreate">The number of copies to create of the original workplane</param>
        /// <param name="rotationOrigin">The origin of the rotation axis</param>
        /// <returns>A list of workplanes created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(
            Axes rotationAxis,
            Geometry.Degree rotationAngle,
            int copiesToCreate,
            Geometry.Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        /// <summary>
        /// Mirrors a single workplane in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">The plane about which to mirror the workplane</param>
        /// <param name="mirrorPoint">The origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Geometry.Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        #endregion

        #endregion
    }
}