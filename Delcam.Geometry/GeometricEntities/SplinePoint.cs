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
    /// SplinePoint inherits from Point. In addition to possessing location, a spline point also has direction and
    /// magnitude before and after the point.
    /// </summary>
    [Serializable]

    // TODO: Should this inherit from Point or not?
    public class SplinePoint : Point
    {
        #region " Fields "

        /// <summary>
        /// This is the direction of the Spline Curve into the point
        /// </summary>
        private Vector _directionBefore;

        /// <summary>
        /// This is the magnitude of the direction of the Spline Curve into the point
        /// It is kept separate from the direction to allow calculations to be more efficient
        /// </summary>
        private MM _magnitudeBefore;

        /// <summary>
        /// This is the direction of the Spline Curve out of the point
        /// </summary>
        private Vector _directionAfter;

        /// <summary>
        /// This is the magnitude of the direction of the Spline Curve out of the point
        /// It is kept separate from the direction to allow calculations to be more efficient
        /// </summary>
        private MM _magnitudeAfter;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Constructs a new SplinePoint about the specified point argument. Directions and magnitudes are unspecified.
        /// </summary>
        /// <param name="point">Point about which to base the new SplinePoint.</param>
        public SplinePoint(Point point)
        {
            _x = point.X;
            _y = point.Y;
            _z = point.Z;

            _directionBefore = new Vector();
            _magnitudeBefore = 0;
            _directionAfter = new Vector();
            _magnitudeAfter = 0;
        }

        /// <summary>
        /// Constructs a new SplinePoint with the specified position, directions and magnitudes.
        /// </summary>
        /// <param name="point">Position of the point.</param>
        /// <param name="directionBefore">Direction before the point.</param>
        /// <param name="magnitudeBefore">Magnitude before the point.</param>
        /// <param name="magnitudeAfter">Direction after the point.</param>
        /// <param name="directionAfter">Magnitude after the point.</param>
        public SplinePoint(
            Point point,
            Vector directionBefore = null,
            MM magnitudeBefore = new MM(),
            Vector directionAfter = null,
            MM magnitudeAfter = new MM())
        {
            _x = point.X;
            _y = point.Y;
            _z = point.Z;
            if (directionBefore == null)
            {
                directionBefore = new Vector(0.0, 0.0, 1.0);
            }
            _directionBefore = directionBefore;
            _magnitudeBefore = magnitudeBefore;
            if (directionAfter == null)
            {
                directionAfter = new Vector(0.0, 0.0, 1.0);
            }
            _directionAfter = directionAfter;
            _magnitudeAfter = magnitudeAfter;
        }

        /// <summary>
        /// Constructs a new SplinePoint with the specified position. If present, directions
        /// and magnitudes are inferred from the supplied control points.
        /// </summary>
        /// <param name="position">Position of the point.</param>
        /// <param name="controlPointBefore">Control point before position.</param>
        /// <param name="controlPointAfter">Control point after position.</param>
        public SplinePoint(Point position, Point controlPointBefore = null, Point controlPointAfter = null)
        {
            _x = position.X;
            _y = position.Y;
            _z = position.Z;
            if (controlPointBefore == null)
            {
                _directionBefore = new Vector();
                _magnitudeBefore = 0;
            }
            else
            {
                _directionBefore = position - controlPointBefore;
                _magnitudeBefore = _directionBefore.Magnitude;
                _directionBefore.Normalize();
            }
            if (controlPointAfter == null)
            {
                _directionAfter = new Vector();
                _magnitudeAfter = 0;
            }
            else
            {
                _directionAfter = controlPointAfter - position;
                _magnitudeAfter = _directionAfter.Magnitude;
                _directionAfter.Normalize();
            }
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Vector specifying direction prior to the point.
        /// </summary>
        public Vector DirectionBefore
        {
            get { return _directionBefore; }

            set { _directionBefore = value; }
        }

        /// <summary>
        /// Magnitude prior to the point.
        /// </summary>
        public MM MagnitudeBefore
        {
            get { return _magnitudeBefore; }

            set { _magnitudeBefore = value; }
        }

        /// <summary>
        /// Vector specifying direction after the point.
        /// </summary>
        public Vector DirectionAfter
        {
            get { return _directionAfter; }

            set { _directionAfter = value; }
        }

        /// <summary>
        /// Magnitude after the point.
        /// </summary>
        public MM MagnitudeAfter
        {
            get { return _magnitudeAfter; }

            set { _magnitudeAfter = value; }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a clone of the spline point.
        /// </summary>
        public SplinePoint Clone()
        {
            SplinePoint copiedSplinePoint = new SplinePoint(new Point(X, Y, Z));
            copiedSplinePoint.DirectionBefore = new Vector(DirectionBefore.I, DirectionBefore.J, DirectionBefore.K);
            copiedSplinePoint.DirectionAfter = new Vector(DirectionAfter.I, DirectionAfter.J, DirectionAfter.K);
            copiedSplinePoint.MagnitudeBefore = MagnitudeBefore;
            copiedSplinePoint.MagnitudeAfter = MagnitudeAfter;

            return copiedSplinePoint;
        }

        #endregion

        #region "Overrides Operations"

        public override bool Equals(Object obj)
        {
            SplinePoint p = obj as SplinePoint;
            if ((object) p == null)
            {
                return false;
            }

            return Equals(p);
        }

        public bool Equals(SplinePoint p)
        {
            // Return true if the fields match:
            return base.Equals(p) && DirectionBefore == p.DirectionBefore
                   && DirectionAfter == p.DirectionAfter
                   && MagnitudeBefore == p.MagnitudeBefore
                   && MagnitudeAfter == p.MagnitudeAfter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ DirectionBefore.GetHashCode() ^ DirectionAfter.GetHashCode() ^
                   MagnitudeAfter.GetHashCode() ^ MagnitudeBefore.GetHashCode();
        }

        public static bool operator ==(SplinePoint a, SplinePoint b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object) a == null || (object) b == null)
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(SplinePoint a, SplinePoint b)
        {
            return !(a == b);
        }

        #endregion
    }
}