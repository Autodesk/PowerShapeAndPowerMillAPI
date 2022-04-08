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
    /// Captures a Point in PowerSHAPE
    /// </summary>
    public class PSPoint : PSEntity, IPSMoveable, IPSRotateable, IPSMirrorable
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initialises the Point
        /// </summary>
        internal PSPoint(PSAutomation powerSHAPE, int id, string name) : base(powerSHAPE, id, name)
        {
        }

        /// <summary>
        /// Creates a new Point
        /// </summary>
        internal PSPoint(PSAutomation powerSHAPE, Point point) : base(powerSHAPE)
        {
            // Create the point
            _powerSHAPE.DoCommand("CREATE POINT SINGLE",
                                  point.X.ToString("0.######") + " " + point.Y.ToString("0.######") + " " + point.Z.ToString("0.######"));

            _name = _powerSHAPE.ActiveModel.SelectedItems[0].Name;
            _id = _powerSHAPE.ReadIntValue(Identifier + "['" + Name + "'].ID");
        }

        #endregion

        #region " Properties "

        internal const string POINT_IDENTIFIER = "POINT";

        /// <summary>
        /// Gets the identifier PowerSHAPE uses to identify this type of entity
        /// </summary>
        internal override string Identifier
        {
            get { return POINT_IDENTIFIER; }
        }

        /// <summary>
        /// Gets the composite ID for use in communication with the ActiveDocument object
        /// </summary>
        internal override int CompositeID
        {
            get { return 49 * 20000000 + _id; }
        }

        /// <summary>
        /// Gets the command that'll be used to rename the Point
        /// </summary>
        internal override string RenameCommand
        {
            get { return "MODIFY"; }
        }

        /// <summary>
        /// Gets the bounding box of the point.  The bounding box of a point has the same minimum bound as maximum bound
        /// </summary>
        public override BoundingBox BoundingBox
        {
            get
            {
                Point position = ToPoint();
                return new BoundingBox(position.Clone(), position.Clone());
            }
        }

        /// <summary>
        /// Gets and sets the position of the point
        /// </summary>
        public Point Centre
        {
            get { return new Point(X, Y, Z); }
            set { MoveByVector(value - Centre, 0); }
        }

        /// <summary>
        /// Gets and sets the X coordinate of its relative position
        /// </summary>
        public MM X
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POSITION.X");
            }
            set
            {
                AbortIfDoesNotExist();
                AddToSelection(true);
                _powerSHAPE.Execute("MODIFY");
                _powerSHAPE.Execute("XPOS " + value.ToString("0.######"));
                _powerSHAPE.Execute("ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the Y coordinate of its relative position
        /// </summary>
        /// <history>
        /// Who  When        Why
        /// ---  ----        ---
        /// EKM  27/05/2009  Initial version
        /// EKM  24/07/2009  Changed to take Identifier
        /// </history>
        public MM Y
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POSITION.Y");
            }
            set
            {
                AbortIfDoesNotExist();
                AddToSelection(true);
                _powerSHAPE.Execute("MODIFY");
                _powerSHAPE.Execute("YPOS " + value.ToString("0.######"));
                _powerSHAPE.Execute("ACCEPT");
            }
        }

        /// <summary>
        /// Gets and sets the Z coordinate of its relative position
        /// </summary>
        public MM Z
        {
            get
            {
                AbortIfDoesNotExist();
                return _powerSHAPE.ReadDoubleValue(Identifier + "[ID " + _id + "].POSITION.Z");
            }
            set
            {
                AbortIfDoesNotExist();
                AddToSelection(true);
                _powerSHAPE.Execute("MODIFY");
                _powerSHAPE.Execute("ZPOS " + value.ToString("0.######"));
                _powerSHAPE.Execute("ACCEPT");
            }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Deletes the Point from PowerSHAPE and removes it from the Points collection
        /// </summary>
        public override void Delete()
        {
            _powerSHAPE.ActiveModel.Points.Remove(this);
        }

        /// <summary>
        /// Returns a string representation of the coordinates of the point
        /// </summary>
        /// <returns>String of coordinates the format "X Y Z"</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }

        /// <summary>
        /// Returns the Point as a Geometry Point
        /// </summary>
        /// <returns>A geometry point representation of this point</returns>
        public Point ToPoint()
        {
            return new Point(X, Y, Z);
        }

        /// <summary>
        /// Projects the Point onto the specified surface
        /// </summary>
        /// <param name="surface">The surface onto which the Point is to be projected</param>
        /// <returns>A boolean indicating whether the proection was successful</returns>
        public bool ProjectPointOntoSurface(PSSurface surface)
        {
            // Get original point coordinates
            Point originalCoordinates = ToPoint();

            // Select point and surface
            AddToSelection(true);
            surface.AddToSelection(false);

            // Carry out projection
            _powerSHAPE.ActiveModel.ClearCreatedItems();
            _powerSHAPE.DoCommand("EDIT PROJECTPOINT");

            // Check whether projection occurred
            if (ToPoint() == originalCoordinates)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the distance between this entity and another
        /// </summary>
        /// <param name="otherEntity">The function returns the distance between this entity and the otherEntity</param>
        /// <exception cref="Exception">Failed to determine distance between objects</exception>
        public MM DistanceTo(PSEntity otherEntity)
        {
            if (otherEntity is PSSurface)
            {
                return ((PSSurface) otherEntity).DistanceTo(this);
            }
            if (otherEntity is PSPoint)
            {
                return (ToPoint() - ((PSPoint) otherEntity).ToPoint()).Magnitude;
            }
            return base.DistanceTo(otherEntity);
        }

        /// <summary>
        /// Gets the nearest surface to this point as well as the nearest point on that surface
        /// </summary>
        /// <param name="nearestPoint">Returns the nearest point on the nearest surface. Or null if the nearest surface cannot be found</param>
        /// <param name="useWorldCoordinates">Whether or not to return the nearest point on the surface in world coordinates</param>
        /// <returns>The nearest surface or null if the nearest surface cannot be found</returns>
        public PSSurface GetNearestSurface(ref Point nearestPoint, bool useWorldCoordinates = false)
        {
            return _powerSHAPE.ActiveModel.GetNearestSurface(ToPoint(), ref nearestPoint, useWorldCoordinates);
        }

        #region " Edit Operations "

        /// <summary>
        /// This operation moves a single point by the relative distance between two absolute positions
        /// </summary>
        /// <param name="moveOriginCoordinates">The first of the two absolute positions</param>
        /// <param name="pointToMoveTo">The second of the two absolute positions</param>
        /// <param name="copiesToCreate">The number of copies that should be created by the operation</param>
        /// <returns>List of points created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> MoveBetweenPoints(Point moveOriginCoordinates, Point pointToMoveTo, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityBetweenPoints(this, moveOriginCoordinates, pointToMoveTo, copiesToCreate);
        }

        /// <summary>
        /// This operation moves a single point by a specified relative amount
        /// </summary>
        /// <param name="moveVector">Relative amount by which the point will be moved</param>
        /// <param name="copiesToCreate">Number of copies that should be created by the operation</param>
        /// <returns>A list of points created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> MoveByVector(Vector moveVector, int copiesToCreate)
        {
            return PSEntityMover.MoveEntityByVector(this, moveVector, copiesToCreate);
        }

        /// <summary>
        /// This operation mirrors a single point in a specified plane
        /// </summary>
        /// <param name="mirrorPlane">Plane about which to mirror the point</param>
        /// <param name="mirrorPoint">Origin of the mirror plane</param>
        public void Mirror(Planes mirrorPlane, Point mirrorPoint)
        {
            PSEntityMirrorer.MirrorEntity(this, mirrorPlane, mirrorPoint);
        }

        /// <summary>
        /// This operation rotates a single point by a specified angle around a specified axis
        /// </summary>
        /// <param name="rotationAxis">Axis around which the point is are to be rotated</param>
        /// <param name="rotationAngle">Angle by which the point is to be rotated</param>
        /// <param name="copiesToCreate">Number of copies to create of the original point</param>
        /// <param name="rotationOrigin">Origin of the rotation axis</param>
        /// <returns>List of points created by the operation.  If numberOfCopies is 0, an empty list is returned</returns>
        public List<PSEntity> Rotate(Axes rotationAxis, Degree rotationAngle, int copiesToCreate, Point rotationOrigin = null)
        {
            return PSEntityRotater.RotateEntity(this, rotationAxis, rotationAngle, copiesToCreate, rotationOrigin);
        }

        #endregion

        #endregion
    }
}