// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Defines a three-dimensional bounding box.
    /// </summary>
    [Serializable]
    public class BoundingBox
    {
        #region " Fields "

        /// <summary>
        /// This is the minimum point of the bounding box
        /// </summary>
        Point _minimumBounds;

        /// <summary>
        /// This is the maximum point of the bounding box
        /// </summary>
        Point _maximumBounds;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Constructs a maximally sized bounding box.
        /// </summary>
        public BoundingBox()
        {
            _minimumBounds = new Point(double.MaxValue, double.MaxValue, double.MaxValue);
            _maximumBounds = new Point(double.MinValue, double.MinValue, double.MinValue);
        }

        /// <summary>
        /// Constructs a bounding box with the specified dimensions.
        /// </summary>
        /// <param name="minX">Minimum value in X.</param>
        /// <param name="maxX">Maximum value in X.</param>
        /// <param name="minY">Minimum value in Y.</param>
        /// <param name="maxY">Maximum value in Y.</param>
        /// <param name="minZ">Minimum value in Z.</param>
        /// <param name="maxZ">Maximum value in Z.</param>
        public BoundingBox(MM minX, MM maxX, MM minY, MM maxY, MM minZ, MM maxZ)
        {
            _minimumBounds = new Point(minX, minY, minZ);
            _maximumBounds = new Point(maxX, maxY, maxZ);
        }

        /// <summary>
        /// Constructs a bounding box with the specified dimensions.
        /// </summary>
        /// <param name="minimumBounds">Minimum values in X, Y and Z.</param>
        /// <param name="maximumBounds">Maximum values in X, Y and Z.</param>
        public BoundingBox(Point minimumBounds, Point maximumBounds)
        {
            _minimumBounds = minimumBounds;
            _maximumBounds = maximumBounds;
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Minimum X, Y and Z bounds of the bounding box.
        /// </summary>
        public Point MinimumBounds
        {
            get { return _minimumBounds; }
        }

        /// <summary>
        /// Maximum X, Y and Z bounds of the bounding box.
        /// </summary>
        public Point MaximumBounds
        {
            get { return _maximumBounds; }
        }

        /// <summary>
        /// Minimum boundary in X.
        /// </summary>
        public MM MinX
        {
            get { return _minimumBounds.X; }

            set { _minimumBounds.X = value; }
        }

        /// <summary>
        /// Maximum boundary in X.
        /// </summary>
        public MM MaxX
        {
            get { return _maximumBounds.X; }

            set { _maximumBounds.X = value; }
        }

        /// <summary>
        /// Minimum boundary in Y.
        /// </summary>
        public MM MinY
        {
            get { return _minimumBounds.Y; }

            set { _minimumBounds.Y = value; }
        }

        /// <summary>
        /// Maximum boundary in Y.
        /// </summary>
        public MM MaxY
        {
            get { return _maximumBounds.Y; }

            set { _maximumBounds.Y = value; }
        }

        /// <summary>
        /// Minimum boundary in Z.
        /// </summary>
        public MM MinZ
        {
            get { return _minimumBounds.Z; }

            set { _minimumBounds.Z = value; }
        }

        /// <summary>
        /// Maximum boundary in Z.
        /// </summary>
        public MM MaxZ
        {
            get { return _maximumBounds.Z; }

            set { _maximumBounds.Z = value; }
        }

        /// <summary>
        /// Boundary extent in X.
        /// </summary>
        public MM XSize
        {
            get { return _maximumBounds.X - _minimumBounds.X; }
        }

        /// <summary>
        /// Boundary extent in Y.
        /// </summary>
        public MM YSize
        {
            get { return _maximumBounds.Y - _minimumBounds.Y; }
        }

        /// <summary>
        /// Boundary extent in Z.
        /// </summary>
        public MM ZSize
        {
            get { return _maximumBounds.Z - _minimumBounds.Z; }
        }

        /// <summary>
        /// Volumetric centre of the bounding box.
        /// </summary>
        public Point VolumetricCentre
        {
            get { return _minimumBounds + (_maximumBounds - _minimumBounds) / 2; }
        }

        /// <summary>
        /// Volume of bounding box.
        /// </summary>
        public double Volume
        {
            get { return XSize * YSize * ZSize; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Test whether this item is the same as the passed object
        /// </summary>
        /// <param name="obj">Item to compare against</param>
        public override bool Equals(object obj)
        {
            if (obj is BoundingBox)
            {
                BoundingBox other = (BoundingBox) obj;
                return MinX == other.MinX && MaxX == other.MaxX && MinY == other.MinY && MaxY == other.MaxY &&
                       MinZ == other.MinZ &&
                       MaxZ == other.MaxZ;
            }

            return false;
        }

        /// <summary>
        /// Get the bounding box around two boxes.
        /// </summary>
        /// <param name="box1"></param>
        /// <param name="box2"></param>
        /// <returns>Returns the bounding box around two bounding boxes. If either box is Nothing, the other will be returned. If both boxes are Nothing, Nothing will be returned.</returns>
        /// <remarks></remarks>
        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            //If either box is null return the other (which automatically returns null if both are null)
            if (box1 == null)
            {
                return box2;
            }
            if (box2 == null)
            {
                return box1;
            }

            double minX = Math.Min(box1.MinX.Value, box2.MinX.Value);
            double minY = Math.Min(box1.MinY.Value, box2.MinY.Value);
            double minZ = Math.Min(box1.MinZ.Value, box2.MinZ.Value);
            double maxX = Math.Max(box1.MaxX.Value, box2.MaxX.Value);
            double maxY = Math.Max(box1.MaxY.Value, box2.MaxY.Value);
            double maxZ = Math.Max(box1.MaxZ.Value, box2.MaxZ.Value);
            return new BoundingBox(minX, maxX, minY, maxY, minZ, maxZ);
        }

        /// <summary>
        /// Returns a string representation of the BoundingBox
        /// </summary>
        public override string ToString()
        {
            return string.Format("X {0} to {1}, Y {2} to {3}, Z {4} to {5}", MinX, MaxX, MinY, MaxY, MinZ, MaxZ);
        }

        #endregion

        #region " Clone Operation "

        /// <summary>
        /// Clones of the current BoundingBox.
        /// </summary>
        public BoundingBox Clone()
        {
            BoundingBox clonedBox = new BoundingBox(_minimumBounds.Clone(), _maximumBounds.Clone());
            return clonedBox;
        }

        #endregion
    }
}