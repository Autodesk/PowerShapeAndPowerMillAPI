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

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates a straight line.
    /// </summary>
    [Serializable]
    public class Line
    {
        #region "Fields"

        /// <summary>
        /// This is the start point of the line
        /// </summary>
        private Point _startPoint;

        /// <summary>
        /// This is the end point of the line
        /// </summary>
        private Point _endPoint;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs a Line between the specified start and end points.
        /// </summary>
        /// <param name="startPoint">Start point.</param>
        /// <param name="endPoint">End point.</param>
        public Line(Point startPoint, Point endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Line starts here.
        /// </summary>
        public Point StartPoint
        {
            get { return _startPoint; }

            set { _startPoint = value; }
        }

        /// <summary>
        /// Line ends here.
        /// </summary>
        public Point EndPoint
        {
            get { return _endPoint; }

            set { _endPoint = value; }
        }

        /// <summary>
        /// Centre of the line.
        /// </summary>
        public Point MidPoint
        {
            get
            {
                // Calculate the vector from the start point to the end point
                Vector lineVector = _endPoint - _startPoint;

                // Halve it to get the vector to the mid point
                lineVector /= 2.0;

                // Return the mid point
                return _startPoint + lineVector;
            }
        }

        /// <summary>
        /// Returns the bounding box of the line.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                // Add the start and end points to a collection
                List<Point> pointCollection = new List<Point>();
                pointCollection.Add(StartPoint);
                pointCollection.Add(EndPoint);

                // Find the minimum X, Y and Z coordinates from the start and end points
                Point minimum = new Point(pointCollection.Min(point => point.X),
                                          pointCollection.Min(point => point.Y),
                                          pointCollection.Min(point => point.Z));

                // Find the maximum X, Y and Z coordinates from the start and end points
                Point maximum = new Point(pointCollection.Max(point => point.X),
                                          pointCollection.Max(point => point.Y),
                                          pointCollection.Max(point => point.Z));

                // Create a BoundingBox from the minimum and maximum coordinates
                return new BoundingBox(minimum, maximum);
            }
        }

        /// <summary>
        /// Determins whether two line segments intersect.
        /// </summary>
        /// <param name="secondLine"></param>
        /// <returns></returns>
        public bool Intersects(Line secondLine)
        {
            Vector u1 = EndPoint - StartPoint;
            Vector u2 = secondLine.EndPoint - secondLine.StartPoint;
            Vector w = secondLine.StartPoint - StartPoint; // vector from s2 to s1

            if (u1.MagnitudeSquare == 0.0 || u2.MagnitudeSquare == 0.0)
            {
                throw new ArgumentException("Cannot check line with 0 length.");
            }

            if (w.MagnitudeSquare == 0.0)
            {
                return true; // Overlapped start point.
            }

            // Check coplanar
            if (Vector.DotProduct(u1, Vector.CrossProduct(w, u2)) != 0.0)
            {
                return false;
            }

            // Compute coefficient of intersection point, in parametric equation P(s) = P0 + s * u;
            // Equate two lines with both 0 <= s <= 1, and results the following
            var s1 = Vector.DotProduct(Vector.CrossProduct(w, u2), Vector.CrossProduct(u1, u2)) /
                     Vector.CrossProduct(u1, u2).MagnitudeSquare;
            var s2 = Vector.DotProduct(Vector.CrossProduct(u1, w), Vector.CrossProduct(u2, u1)) /
                     Vector.CrossProduct(u2, u1).MagnitudeSquare;

            if (s1 >= 0.0 && s1 <= 1.0 && s2 >= 0.0 && s2 <= 1.0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}