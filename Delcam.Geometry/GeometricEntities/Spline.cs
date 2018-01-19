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
using System.Globalization;
using System.Linq;
using System.Text;
using Autodesk.FileSystem;
using Microsoft.VisualBasic;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates a Spline Curve.
    /// </summary>
    /// <remarks>
    /// A spline curve is similar to a Polyline except every point on the curve has a before and after
    /// distance and magnitude value used to control the direction of the curve through the point.
    /// </remarks>
    [Serializable]
    public class Spline : List<SplinePoint>
    {
        #region "Constructors"

        /// <summary>
        /// Constructs an empty curve.
        /// </summary>
        public Spline()
        {
        }

        /// <summary>
        /// Constructs a best-fit spline curve through the specified ordered points.
        /// </summary>
        /// <param name="orderedBestFitPoints">Ordered list of points to fit.</param>
        /// <param name="autoFree">If true, joins will be smoothed; otherwise points are added to the spline with straight lines.</param>
        public Spline(List<Point> orderedBestFitPoints, bool autoFree = true)
        {
            foreach (Point myPoint in orderedBestFitPoints)
            {
                AddPointToEndOfSpline(myPoint, false);
            }

            if (autoFree)
            {
                FreeTangentsAndMagnitudes();
            }
        }

        /// <summary>
        /// Constructs a new spline curve from a list of spline points.
        /// </summary>
        /// <param name="orderedSplinePoints">Spline points through which to create the curve.</param>
        public Spline(List<SplinePoint> orderedSplinePoints)
        {
            AddRange(orderedSplinePoints);
        }

        /// <summary>
        /// Constructs a new spline curve from an array of spline points
        /// </summary>
        /// <param name="orderedSplinePoints">The spline points through which to create a curve</param>
        public Spline(SplinePoint[] orderedSplinePoints)
        {
            AddRange(orderedSplinePoints);
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Control point for the specified span and index.
        /// </summary>
        /// <param name="spanIndex">Zero-based span index.</param>
        /// <param name="controlPointIndex">Zero-based control point index.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown in either of the following two cases:<br></br><br></br>
        /// The specified span index does not identify a valid span in the spline. i.e. 0 &gt; spanIndex or spanIndex - 1 &gt;= number of spans.<br></br>
        /// The control point index fails to meet the condition: 0 &gt;= controlPointIndex &lt;= 3.
        /// </exception>
        public Point GetSpanControlPoint(int spanIndex, int controlPointIndex)
        {
            // Special handling for spanIndex = Me.Count() - 1 and controlPointIndex = 0
            // This is so that a user can loop through the points in a spline without producing an error on the last point
            if ((spanIndex == Count - 1) & (controlPointIndex == 0))
            {
                spanIndex = spanIndex - 1;
                controlPointIndex = 3;
            }

            if ((spanIndex >= Count - 1) | (spanIndex < 0))
            {
                throw new IndexOutOfRangeException("The specified span index is out of range.");
            }
            if ((controlPointIndex < 0) | (controlPointIndex > 3))
            {
                throw new IndexOutOfRangeException("The control point index must be between 0 and 3.");
            }

            if (controlPointIndex == 0)
            {
                return this[spanIndex];
            }
            if (controlPointIndex == 1)
            {
                return this[spanIndex] + this[spanIndex].DirectionAfter * this[spanIndex].MagnitudeAfter;
            }
            if (controlPointIndex == 2)
            {
                return this[spanIndex + 1] - this[spanIndex + 1].DirectionBefore * this[spanIndex + 1].MagnitudeBefore;
            }
            if (controlPointIndex == 3)
            {
                return this[spanIndex + 1];
            }

            throw new Exception("Failed to get control point.");
        }

        public void SetSpanControlPoint(Point value, int spanIndex, int controlPointIndex)
        {
            if ((spanIndex >= Count - 1) | (spanIndex < 0))
            {
                throw new IndexOutOfRangeException("The specified span index is out of range.");
            }
            if ((controlPointIndex < 0) | (controlPointIndex > 3))
            {
                throw new IndexOutOfRangeException("The control point index must be between 0 and 3.");
            }

            if (controlPointIndex == 0)
            {
                Point controlPointAfter = this[spanIndex] + this[spanIndex].DirectionAfter * this[spanIndex].MagnitudeAfter;
                Vector newDirectionAfter = controlPointAfter - value;
                MM newMagnitudeAfter = newDirectionAfter.Magnitude;
                newDirectionAfter.Normalize();
                Point controlPointBefore = this[spanIndex] - this[spanIndex].DirectionBefore * this[spanIndex].MagnitudeBefore;
                Vector newDirectionBefore = value - controlPointBefore;
                MM newMagnitudeBefore = newDirectionBefore.Magnitude;
                newDirectionBefore.Normalize();
                this[spanIndex] = new SplinePoint(value,
                                                  newDirectionBefore,
                                                  newMagnitudeBefore,
                                                  newDirectionAfter,
                                                  newMagnitudeAfter);
            }
            else if (controlPointIndex == 1)
            {
                this[spanIndex].DirectionAfter = value - this[spanIndex];
                this[spanIndex].MagnitudeAfter = this[spanIndex].DirectionAfter.Magnitude;
                this[spanIndex].DirectionAfter.Normalize();
            }
            else if (controlPointIndex == 2)
            {
                this[spanIndex + 1].DirectionBefore = this[spanIndex + 1] - value;
                this[spanIndex + 1].MagnitudeBefore = this[spanIndex + 1].DirectionBefore.Magnitude;
                this[spanIndex + 1].DirectionBefore.Normalize();
            }
            else if (controlPointIndex == 3)
            {
                Point controlPointAfter = this[spanIndex + 1] +
                                          this[spanIndex + 1].DirectionAfter * this[spanIndex + 1].MagnitudeAfter;
                Vector newDirectionAfter = controlPointAfter - value;
                MM newMagnitudeAfter = newDirectionAfter.Magnitude;
                newDirectionAfter.Normalize();
                Point controlPointBefore = this[spanIndex + 1] -
                                           this[spanIndex + 1].DirectionBefore * this[spanIndex + 1].MagnitudeBefore;
                Vector newDirectionBefore = value - controlPointBefore;
                MM newMagnitudeBefore = newDirectionBefore.Magnitude;
                newDirectionBefore.Normalize();
                this[spanIndex + 1] = new SplinePoint(value,
                                                      newDirectionBefore,
                                                      newMagnitudeBefore,
                                                      newDirectionAfter,
                                                      newMagnitudeAfter);
            }
        }

        /// <summary>
        /// Control points of the specified span.
        /// </summary>
        /// <param name="spanIndex">Zero-based span index.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if the specified span index does not identify a valid span in the spline.
        /// i.e. 0 &gt; spanIndex or spanIndex - 1 &gt;= number of spans.
        /// </exception>
        public List<Point> GetSpanControlPoints(int spanIndex)
        {
            if ((spanIndex >= Count - 1) | (spanIndex < 0))
            {
                throw new IndexOutOfRangeException("The specified span index is out of range.");
            }

            List<Point> controlPoints = new List<Point>();
            controlPoints.Add(this[spanIndex]);
            controlPoints.Add(this[spanIndex] + this[spanIndex].DirectionAfter * this[spanIndex].MagnitudeAfter);
            controlPoints.Add(this[spanIndex + 1] - this[spanIndex + 1].DirectionBefore * this[spanIndex + 1].MagnitudeBefore);
            controlPoints.Add(this[spanIndex + 1]);

            return controlPoints;
        }

        public void SetSpanControlPoints(List<Point> value, int spanIndex)
        {
            if ((spanIndex >= Count - 1) | (spanIndex < 0))
            {
                throw new IndexOutOfRangeException("The specified span index is out of range.");
            }

            SetSpanControlPoint(value[0], spanIndex, 0);
            SetSpanControlPoint(value[1], spanIndex, 1);
            SetSpanControlPoint(value[2], spanIndex, 2);
            SetSpanControlPoint(value[3], spanIndex, 3);
        }

        /// <summary>
        /// The specified span as a CubicBezier.
        /// </summary>
        /// <param name="spanIndex">Zero-based span index.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if the specified span index does not identify a valid span in the spline.
        /// i.e. 0 &gt; spanIndex or spanIndex - 1 &gt;= number of spans.
        /// </exception>
        public CubicBezier GetBezierCurve(int spanIndex)
        {
            if ((spanIndex >= Count - 1) | (spanIndex < 0))
            {
                throw new IndexOutOfRangeException("The specified span index is out of range.");
            }

            var controlPoint1 = this[spanIndex] + this[spanIndex].DirectionAfter * this[spanIndex].MagnitudeAfter;
            var controlPoint2 = this[spanIndex + 1] - this[spanIndex + 1].DirectionBefore * this[spanIndex + 1].MagnitudeBefore;

            return new CubicBezier(this[spanIndex], this[spanIndex + 1], controlPoint1, controlPoint2);
        }

        /// <summary>
        /// True if spline is closed; false otherwise.
        /// </summary>
        /// <remarks>The spline is closed if the distance from its first point to its last is zero.</remarks>
        /// <exception cref="Exception">Thrown if the setting of this property fails to either open or close the spline.</exception>
        public bool IsClosed
        {
            // Get whether the spline is open or closed
            get { return IsSplineClosed(); }
            set
            {
                // Get whether the spline is open or closed
                bool amIClosed = IsSplineClosed();

                if (amIClosed & value)
                {
                    // The spline is Closed
                    // The user wants the spline to be Closed
                    // Do nothing
                }
                else if (amIClosed & (value == false))
                {
                    // The spline is Closed
                    // The user wants the spline to be Open
                    // Open the spline

                    // Delete the last point on the spline
                    RemoveAt(this.Count() - 1);
                }
                else if ((amIClosed == false) & (value == false))
                {
                    // The spline is open
                    // The user wants the spline to be open
                    // Do nothing
                }
                else if ((amIClosed == false) & value)
                {
                    // The spline is open
                    // The user wants the spline to be Closed
                    // Close the spline

                    // Get the start point of the spline
                    SplinePoint startPoint = this[0].Clone();

                    // Insert a new point that is coincedent with the start point of the spline
                    Add(startPoint);

                    // Set the direction after and the magnitude after the new end point to be the same as that of the first point
                    this[this.Count() - 1].DirectionAfter = this[0].DirectionAfter;
                    this[this.Count() - 1].MagnitudeAfter = this[0].MagnitudeAfter;

                    // Set the direction before the new last point to equal the direction after the start point
                    this[this.Count() - 1].DirectionBefore = this[0].DirectionAfter;

                    // Free the magnitudes of the final span
                    FreeChordMagnitudes(this.Count() - 2);
                }

                // Check that the curve is opened or closed correctly
                amIClosed = IsSplineClosed();
                if (!(amIClosed == value))
                {
                    if (value)
                    {
                        throw new Exception("Error. Failed to open the spline");
                    }
                    throw new Exception("Error. Failed to close the spline");
                }
            }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a clone of this spline.
        /// </summary>
        public Spline Clone()
        {
            Spline copiedSpline = new Spline();
            for (int i = 0; i <= this.Count() - 1; i++)
            {
                SplinePoint sp = this[i].Clone();
                copiedSpline.Add(sp);
            }

            return copiedSpline;
        }

        /// <summary>
        /// Adds a point to the end of this spline.
        /// </summary>
        /// <param name="point">Point to be added to the end of the spline.</param>
        /// <param name="autoFree">If true (default), join will be smoothed; otherwise point is added to the spline with a straight line.</param>
        public void AddPointToEndOfSpline(Point point, bool autoFree = true)
        {
            Add(new SplinePoint(point));

            if (autoFree)
            {
                // Adding this point will affect the directions of the previous two points, and therefore
                // the magnitudes of the previous three points. Update accordingly.
                if (Count > 1)
                {
                    if (Count > 2)
                    {
                        FreePointDirections(Count - 3);
                    }
                    FreePointDirections(Count - 2);
                    FreePointDirections(Count - 1);

                    if (Count > 3)
                    {
                        FreeChordMagnitudes(Count - 4);
                    }
                    if (Count > 2)
                    {
                        FreeChordMagnitudes(Count - 3);
                    }
                    FreeChordMagnitudes(Count - 2);
                }
            }
            else
            {
                // Extend the curve with a straight line
                if (this.Count() > 1)
                {
                    SetSpanControlPoint(point, this.Count() - 2, 1);
                    SetSpanControlPoint(GetSpanControlPoint(this.Count() - 2, 0), this.Count() - 2, 2);
                }
            }
        }

        /// <summary>
        /// Inserts the specified point into this spline.
        /// If inserted in the middle, then no control points in the existing spline are changed.
        /// If inserted at the beginning or end of the spline, the join is smoothed without altering the shape of the original spline.
        /// </summary>
        /// <param name="pointIndex">
        /// Index of the point to be added. An index of zero adds the point to the front of the spline.
        /// An index equal to or exceeding the maximum current point index will add the point to the end of the line.
        /// </param>
        /// <param name="point">Point to insert.</param>
        /// <param name="pointProximityTolerance">Optional tolerance. Prevents a point from being added in too close a proximity to a neighbouring point.</param>
        /// <returns>Sets to True if the operation succeeded; false otherwise.</returns>
        public bool InsertPoint(int pointIndex, Point point, double pointProximityTolerance = 0)
        {
            Point point1 = new Point();
            Point point2 = new Point();

            Vector vector1 = new Vector();
            Vector vector2 = new Vector();

            double distance1 = 0;
            double distance2 = 0;

            bool isSuccess = true;

            if (pointProximityTolerance > 0)
            {
                if (pointIndex > 0)
                {
                    if (pointIndex < Count)
                    {
                        point1 = GetSpanControlPoint(pointIndex - 1, 0);
                    }
                    else
                    {
                        point1 = GetSpanControlPoint(Count - 2, 3);
                    }
                    vector1 = point - point1;
                    distance1 = vector1.Magnitude;
                    if (distance1 < pointProximityTolerance)
                    {
                        isSuccess = false;
                    }
                }

                if (pointIndex < this.Count())
                {
                    if (pointIndex == 0)
                    {
                        point2 = GetSpanControlPoint(0, 0);
                    }
                    else
                    {
                        point2 = GetSpanControlPoint(pointIndex - 1, 3);
                    }

                    vector2 = point - point2;
                    distance2 = vector2.Magnitude;
                    if (distance2 < pointProximityTolerance)
                    {
                        isSuccess = false;
                    }
                }
            }

            if (isSuccess)
            {
                if (pointIndex > this.Count() - 1)
                {
                    AddPointToEndOfSpline(point, false);

                    if (Count > 2)
                    {
                        SetSpanControlPoint(GetSpanControlPoint(Count - 3, 3) +
                                            (GetSpanControlPoint(Count - 3, 3) - GetSpanControlPoint(Count - 3, 2)),
                                            Count - 2,
                                            1);
                    }

                    FreePointMagnitudes(Count - 1);
                    FreePointDirections(Count - 1);
                }
                else
                {
                    Insert(pointIndex, new SplinePoint(new Point(point.X, point.Y, point.Z)));

                    if (pointIndex == 0)
                    {
                        SetSpanControlPoint(GetSpanControlPoint(1, 0) + (GetSpanControlPoint(1, 0) - GetSpanControlPoint(1, 1)),
                                            0,
                                            2);
                    }

                    FreePointDirections(pointIndex);
                    FreePointMagnitudes(pointIndex);
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Splits the Bezier curve span at the specified index at the point infered from the specified t parameter.
        /// </summary>
        /// <param name="spanIndex">Index of the span.</param>
        /// <param name="t">Value of t on which to split where 0 &gt;= t &lt;= 1.</param>
        /// <param name="pointProximityTolerance">Optional tolerance. Prevents a point from being added in too close a proximity to a neighbouring point.</param>
        /// <returns>True if the operation succeeds; false otherwise.</returns>
        public bool SplitSpan(int spanIndex, double t, double pointProximityTolerance = 0)
        {
            // Special handling for spanIndex = Me.Count() - 1 and t = 0
            // This is so that a user can loop through the points in a spline without producing an error on the last point
            if ((spanIndex == Count - 1) & (t == 0))
            {
                spanIndex = spanIndex - 1;
                t = 1;
            }

            // Get the control points for the span
            Point c1 = GetSpanControlPoint(spanIndex, 0);
            Point c2 = GetSpanControlPoint(spanIndex, 1);
            Point c3 = GetSpanControlPoint(spanIndex, 2);
            Point c4 = GetSpanControlPoint(spanIndex, 3);

            // Next check if the control points are colinear
            bool isSpanStraight = GetBezierCurve(spanIndex).IsSpanStraight(true);

            bool isSuccess = false;

            if (isSpanStraight == false)
            {
                // We calculate the split position and the new control points by construction

                // The vectors vd join control points c1--->c2--->c3--->c4
                Vector vd1 = c2 - c1;
                Vector vd2 = c3 - c2;
                Vector vd3 = c4 - c3;

                // Points d lie on the vectors that join the control points
                Point d1 = c1 + t * vd1;
                Point d2 = c2 + t * vd2;
                Point d3 = c3 + t * vd3;

                // The vectors ve join control points d1---->d2--->d3
                Vector ve1 = d2 - d1;
                Vector ve2 = d3 - d2;

                // Points e lie on the vectors that join the control points d
                Point e1 = d1 + t * ve1;
                Point e2 = d2 + t * ve2;

                // The vector vf joins control points e1 and e2
                Vector vf1 = e2 - e1;

                // The split position, vf, lies on vector vf
                Point f1 = e1 + t * vf1;

                // Create the new point
                isSuccess = InsertPoint(spanIndex + 1, f1, pointProximityTolerance);
                if (isSuccess)
                {
                    SetSpanControlPoint(d1, spanIndex, 1);
                    SetSpanControlPoint(e1, spanIndex, 2);

                    SetSpanControlPoint(e2, spanIndex + 1, 1);
                    SetSpanControlPoint(d3, spanIndex + 1, 2);
                }
            }
            else
            {
                // The previous method breaks down if the span is straight because e1 = e2 = 0
                // Instead we will calculate the split position by linear extrapolation between c1 and c4
                Point f1 = c1 + t * (c4 - c1);

                // Create the new point
                isSuccess = InsertPoint(spanIndex + 1, f1, pointProximityTolerance);
                if (isSuccess)
                {
                    SetSpanControlPoint(f1, spanIndex, 1);
                    SetSpanControlPoint(c1, spanIndex, 2);

                    SetSpanControlPoint(c3, spanIndex + 1, 1);
                    SetSpanControlPoint(f1, spanIndex + 1, 2);
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Used to specify endedness for extension operations.
        /// </summary>
        public enum SplineExtensionEnd
        {
            StartOfSpline,
            EndOfSpline
        }

        /// <summary>
        /// Applies a linear extension to either the start or to the end of this spline.
        /// </summary>
        /// <param name="extensionDistance">Distance by which to extend the spline.</param>
        /// <param name="selectEndOfSpline">Indicates either the start or the end of the line.</param>
        /// <param name="pointProximityTolerance">Optional tolerance. Prevents a point from being added in too close a proximity to a neighbouring point.</param>
        /// <returns>True if the operation succeeds; false otherwise.</returns>
        public bool LinearExtension(
            double extensionDistance,
            SplineExtensionEnd selectEndOfSpline,
            double pointProximityTolerance = 0)
        {
            Vector v = new Vector();

            var isSuccess = false;

            if (selectEndOfSpline == SplineExtensionEnd.StartOfSpline)
            {
                v = GetSpanControlPoint(0, 0) - GetSpanControlPoint(0, 1);
                v.Normalize();

                Point newPoint = GetSpanControlPoint(0, 0) + extensionDistance * v;
                isSuccess = InsertPoint(0, newPoint, pointProximityTolerance);

                // Make the extension straight.
                if (isSuccess)
                {
                    SetSpanControlPoint(GetSpanControlPoint(0, 3), 0, 1);
                    SetSpanControlPoint(GetSpanControlPoint(0, 0), 0, 2);
                }
            }
            else if (selectEndOfSpline == SplineExtensionEnd.EndOfSpline)
            {
                int nSpans = this.Count() - 1;
                v = GetSpanControlPoint(nSpans - 1, 3) - GetSpanControlPoint(nSpans - 1, 2);
                v.Normalize();

                Point newPoint = GetSpanControlPoint(nSpans - 1, 3) + extensionDistance * v;
                isSuccess = InsertPoint(nSpans + 1, newPoint, pointProximityTolerance);

                // Make the extension straight.
                if (isSuccess)
                {
                    SetSpanControlPoint(GetSpanControlPoint(nSpans, 3), nSpans, 1);
                    SetSpanControlPoint(GetSpanControlPoint(nSpans, 0), nSpans, 2);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// Writes this spline to the specified DUCT picture file.
        /// The file is written in units of MM as this is the unit of points that constitute the line.
        /// </summary>
        /// <param name="file">File to write.</param>
        /// <exception cref="Exception">Thrown if the curve possesses fewer than two points.</exception>
        public void WriteToDUCTPictureFile(File file)
        {
            if (Count < 2)
            {
                throw new Exception("Cannot output a curve with fewer than 2 points");
            }

            try
            {
                // Delete the file if it already exists
                file.Delete();

                StringBuilder pictureData = new StringBuilder();

                // Write header
                string dateTimeString = null;
                var _with1 = DateAndTime.Now;
                dateTimeString = _with1.Day + " " + DateAndTime.MonthName(_with1.Month, true).ToUpper() + " " + _with1.Year +
                                 " " +
                                 _with1.Hour + "." + _with1.Minute + "." + _with1.Second;
                pictureData.Append("   DuctPicture PICTURE FILE  " + file.Name + "  " + dateTimeString + Constants.vbNewLine);

                // Write part line
                string unitsString = "";
                unitsString = "MM";
                pictureData.Append(" PART:                     PowerMILL MADE IN     " + unitsString + Constants.vbNewLine);

                // Write special marker
                pictureData.Append(" *" + Constants.vbNewLine);

                // Write integer data
                string integerLine = "";
                if ((Count - 1) * 3 > 1000000)
                {
                    integerLine += " " + GetField(4, 3) + GetField(3, 3) + GetField(-1, 11) + GetField(2, 11) +
                                   GetField((Count - 1) * 3 + 1, 11) + GetField(0, 6) + GetField(2, 6) + GetField(0, 6) +
                                   GetField(0, 6) + GetField(0, 6);
                }
                else
                {
                    integerLine += " " + GetField(4, 6) + GetField(3, 6) + GetField(-1, 6) + GetField(2, 6) +
                                   GetField((Count - 1) * 3 + 1, 6) + GetField(0, 6) + GetField(2, 6) + GetField(0, 6) +
                                   GetField(0, 6) + GetField(0, 6);
                }
                pictureData.Append(integerLine + Constants.vbNewLine);

                // Write size data
                string sizeLine = "";
                sizeLine += " " + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) +
                            GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) +
                            GetField(0, "0.0000", 10);
                pictureData.Append(sizeLine + Constants.vbNewLine);

                // Write instruction codes
                if (Count * 3 > 1000000)
                {
                    pictureData.Append(" " + GetField(2048, 11) + " " + GetField(1, 11) + Constants.vbNewLine);
                }
                else
                {
                    pictureData.Append(" " + GetField(2048, 11) + " " + GetField(1, 6) + Constants.vbNewLine);
                }

                int instructionCode = GetInstructionCode(1, 2, 0, 0, 3, 0);
                string instructionCodeString = "";
                if ((Count - 1) * 3 > 1000000)
                {
                    instructionCodeString = " " + GetField(instructionCode, 11) + " " + GetField((Count - 1) * 3, 11) +
                                            Constants.vbNewLine;
                    pictureData.Append(instructionCodeString);
                }
                else
                {
                    instructionCodeString = " " + GetField(instructionCode, 11) + " " + GetField((Count - 1) * 3, 6) +
                                            Constants.vbNewLine;
                    pictureData.Append(instructionCodeString);
                }

                // Write spline data
                string pointCode = null;
                for (int p = 0; p <= Count - 1; p++)
                {
                    if (p > 0)
                    {
                        // Insert the before point
                        Point before = this[p] - this[p].DirectionBefore * this[p].MagnitudeBefore;
                        pointCode = string.Format(CultureInfo.InvariantCulture,
                                                  " {0,14:E} {1,14:E} {2,14:E}",
                                                  before.X,
                                                  before.Y,
                                                  before.Z);
                        pictureData.Append(pointCode + Constants.vbNewLine);
                    }

                    //Use System.Globalization.CultureInfo.InvariantCulture
                    pointCode = string.Format(CultureInfo.InvariantCulture,
                                              " {0,14:E} {1,14:E} {2,14:E}",
                                              this[p].X,
                                              this[p].Y,
                                              this[p].Z);
                    pictureData.Append(pointCode + Constants.vbNewLine);
                    if (p < Count - 1)
                    {
                        // Insert the after point
                        Point after = this[p] + this[p].DirectionAfter * this[p].MagnitudeAfter;
                        pointCode = string.Format(CultureInfo.InvariantCulture,
                                                  " {0,14:E} {1,14:E} {2,14:E}",
                                                  after.X,
                                                  after.Y,
                                                  after.Z);
                        pictureData.Append(pointCode + Constants.vbNewLine);
                    }
                }

                // Write the file
                file.WriteText(pictureData.ToString(), false, Encoding.ASCII);
            }
            catch (Exception ex)
            {
                file.Delete();
                throw;
            }
        }

        /// <summary>
        /// Frees the tangent directions and magnitudes through all points on the curve.
        /// </summary>
        public void FreeTangentsAndMagnitudes()
        {
            FreeCurveDirections();
            FreeCurveMagnitudes();
        }

        /// <summary>
        /// Frees the tangent directions through all points on the curve.
        /// </summary>
        public void FreeCurveDirections()
        {
            //Implements the algorithm in dgkDuctCurve::Process()
            if (Count < 2)
            {
                return;
            }

            for (int i = 0; i <= Count - 1; i++)
            {
                FreePointDirections(i);
            }
        }

        /// <summary>
        /// Frees the entry and exit direction of a point
        /// </summary>
        /// <param name="pointIndex">Index of the point to free.</param>
        public void FreePointDirections(int pointIndex)
        {
            if (Count == 2)
            {
                Vector chord = this[1] - this[0];
                chord.Normalize();
                this[0].DirectionAfter = chord;
                this[0].DirectionBefore = chord;
                this[0].MagnitudeAfter = 1;
                this[0].MagnitudeBefore = 1;
                this[1].DirectionAfter = chord;
                this[1].DirectionBefore = chord;
                this[1].MagnitudeAfter = 1;
                this[1].MagnitudeBefore = 1;
                return;
            }

            Vector d = null;
            Vector dB = null;
            Vector dA = null;
            Vector cB = null;
            Vector c2B = null;
            Vector cA = null;
            Vector c2A = null;
            double wB = 0;
            double wA = 0;

            // If a spline is closed then we will create dummy points to force curvature continuity across the start/end of the spline
            // This function will create the dummy points if they are required
            bool wasSplineInitiallyClosed = IsSplineClosed();
            bool werePointsAddedToStart = false;
            bool werePointsAddedToEnd = false;
            AddDummyPointsToClosedSpline(ref pointIndex,
                                         ref wasSplineInitiallyClosed,
                                         ref werePointsAddedToStart,
                                         ref werePointsAddedToEnd);

            // AMO copied the following code from dgkDuctCurve::Process()
            if (pointIndex == 0)
            {
                cA = this[pointIndex + 1] - this[pointIndex];
                cA.Normalize();
                c2A = this[pointIndex + 2] - this[pointIndex + 1];
                c2A.Normalize();
                dA = cA + c2A;
                dA.Normalize();

                d = cA * Vector.DotProduct(dA, cA) * 2 - dA;
            }
            else if (pointIndex == Count - 1)
            {
                cB = this[pointIndex] - this[pointIndex - 1];
                cB.Normalize();
                c2B = this[pointIndex - 1] - this[pointIndex - 2];
                c2B.Normalize();
                dB = c2B + cB;
                dB.Normalize();

                d = cB * Vector.DotProduct(dB, cB) * 2 - dB;
            }
            else
            {
                cB = this[pointIndex] - this[pointIndex - 1];
                cB.Normalize();
                cA = this[pointIndex + 1] - this[pointIndex];
                cA.Normalize();
                d = cB + cA;
                d.Normalize();

                double r = (cB - cA).Magnitude;
                r = Math.Sqrt(Math.Max(4 - r * r, 0));
                r = Math.Sqrt(Math.Max(2 - r, 0));

                if ((pointIndex == 1) & (Count == 3))
                {
                    wB = 1;
                    wA = 1;
                }
                else if (pointIndex == 1)
                {
                    wB = r;

                    c2A = this[pointIndex + 2] - this[pointIndex + 1];
                    c2A.Normalize();
                    dA = cA + c2A;
                    dA.Normalize();
                    wA = (cA - dA).Magnitude;
                }
                else if (pointIndex == Count - 2)
                {
                    c2B = this[pointIndex - 1] - this[pointIndex - 2];
                    c2B.Normalize();
                    dB = cB + c2B;
                    dB.Normalize();
                    wB = (cB - dB).Magnitude;

                    wA = r;
                }
                else
                {
                    c2B = this[pointIndex - 1] - this[pointIndex - 2];
                    c2B.Normalize();
                    dB = cB + c2B;
                    dB.Normalize();
                    wB = (cB - dB).Magnitude;

                    c2A = this[pointIndex + 2] - this[pointIndex + 1];
                    c2A.Normalize();
                    dA = cA + c2A;
                    dA.Normalize();
                    wA = (cA - dA).Magnitude;
                }

                double sumdif = wA + wB;
                double den = sumdif + 0.1 / ((1 + 10000.0 * sumdif) * (1 + 10000.0 * sumdif));
                if (wA >= wB)
                {
                    wB = 0.5 + 0.5 * ((wA - wB) / den) * ((wA - wB) / den);
                    wA = 1 - wB;
                }
                else
                {
                    wA = 0.5 + 0.5 * ((wA - wB) / den) * ((wA - wB) / den);
                    wB = 1 - wA;
                }

                d = cB * wB + cA * wA;
            }
            d.Normalize();

            this[pointIndex].DirectionBefore = d;
            this[pointIndex].DirectionAfter = d;

            // ^^ End of the code from that was copied from dgkDuctCurve::Process()

            // If a spline is closed then we will have created dummy points to force curvature continuity across the start/end of the spline
            // This function will remove the dummy points if they were created
            RemoveDummyPointsFromClosedSpline(wasSplineInitiallyClosed, werePointsAddedToStart, werePointsAddedToEnd);
        }

        /// <summary>
        /// Frees the magnitudes through all points on the curve.
        /// </summary>
        public void FreeCurveMagnitudes()
        {
            for (int i = 0; i <= Count - 2; i++)
            {
                FreeChordMagnitudes(i);
            }
        }

        /// <summary>
        /// Frees chord magnitudes for the specified span.
        /// </summary>
        /// <param name="spanIndex">Index of span within this spline.</param>
        public void FreeChordMagnitudes(int spanIndex)
        {
            double magnitudeBefore = 0;
            double magnitudeAfter = 0;
            double chordMagnitude = 0;

            Vector chord = this[spanIndex + 1] - this[spanIndex];
            chordMagnitude = chord.Magnitude;
            chord.Normalize();

            double cosineAfter = Vector.DotProduct(this[spanIndex].DirectionAfter, chord);
            double cosineBefore = Vector.DotProduct(this[spanIndex + 1].DirectionBefore, chord);

            double sineBefore = Math.Max(Math.Sqrt(Math.Abs(1 - cosineBefore * cosineBefore)), 0);
            double sineAfter = Math.Max(Math.Sqrt(Math.Abs(1 - cosineAfter * cosineAfter)), 0);

            if ((cosineAfter > 0) & (cosineBefore > 0) & (sineAfter + sineBefore < 0.01))
            {
                magnitudeAfter = chordMagnitude;
                magnitudeBefore = chordMagnitude;
            }
            else
            {
                double var = 2 - cosineAfter * cosineBefore - sineAfter * sineBefore;
                double dena = 1 + cosineBefore * var;
                magnitudeAfter = 2 * chordMagnitude / Math.Max(dena, 0.6667);
                double denb = 1 + cosineAfter * var;
                magnitudeBefore = 2 * chordMagnitude / Math.Max(denb, 0.6667);
            }
            this[spanIndex].MagnitudeAfter = magnitudeAfter / 3.0;
            this[spanIndex + 1].MagnitudeBefore = magnitudeBefore / 3.0;
        }

        /// <summary>
        /// Frees magnitude for the specified point. This is a variation of the FreeChordMagnitudes() method.
        /// </summary>
        /// <param name="pointIndex">Index of the point to be freed.</param>
        public void FreePointMagnitudes(int pointIndex)
        {
            // Note that going through point-by-point will produces slightly different results than chord-by-chord.
            // This is because:
            // 1) Magnitude before of point N depends on value of magnitude after of point N-1.
            // 2) Freeing magnitudes chord-by-chord sets these two magnitudes at the same time.
            // 3) Freeing magnitudes point-by-point sets these two magnitudes at different times.

            // If a spline is closed then we will create dummy points to force curvature continuity across the start/end of the spline
            // This function will create the dummy points if they are required
            bool wasSplineInitiallyClosed = IsSplineClosed();
            bool werePointsAddedToStart = false;
            bool werePointsAddedToEnd = false;
            AddDummyPointsToClosedSpline(ref pointIndex,
                                         ref wasSplineInitiallyClosed,
                                         ref werePointsAddedToStart,
                                         ref werePointsAddedToEnd);

            bool doFreeMagnitudeAfter = pointIndex < Count - 1;
            bool doFreeMagnitudeBefore = pointIndex > 0;

            if (doFreeMagnitudeBefore)
            {
                double magnitudeBefore = 0;

                Vector chordBefore = this[pointIndex] - this[pointIndex - 1];
                double chordBeforeMagnitude = chordBefore.Magnitude;
                chordBefore.Normalize();

                double cosineBefore = Vector.DotProduct(this[pointIndex].DirectionBefore, chordBefore);
                double sineBefore = Math.Max(Math.Sqrt(Math.Abs(1 - cosineBefore * cosineBefore)), 0);

                double cosineAfterPreviousPoint = Vector.DotProduct(this[pointIndex - 1].DirectionAfter, chordBefore);
                double sineAfterPreviousPoint =
                    Math.Max(Math.Sqrt(Math.Abs(1 - cosineAfterPreviousPoint * cosineAfterPreviousPoint)), 0);

                if ((cosineAfterPreviousPoint > 0) & (cosineBefore > 0) & (cosineAfterPreviousPoint + sineBefore < 0.01))
                {
                    magnitudeBefore = chordBeforeMagnitude;
                }
                else
                {
                    double var = 2 - cosineAfterPreviousPoint * cosineBefore - sineAfterPreviousPoint * sineBefore;
                    double denb = 1 + cosineAfterPreviousPoint * var;
                    magnitudeBefore = 2 * chordBeforeMagnitude / Math.Max(denb, 0.6667);
                }

                this[pointIndex].MagnitudeBefore = magnitudeBefore / 3.0;
            }

            if (doFreeMagnitudeAfter)
            {
                double magnitudeAfter = 0;

                Vector chordAfter = this[pointIndex + 1] - this[pointIndex];
                double chordAfterMagnitude = chordAfter.Magnitude;
                chordAfter.Normalize();

                double cosineAfter = Vector.DotProduct(this[pointIndex].DirectionAfter, chordAfter);
                double sineAfter = Math.Max(Math.Sqrt(Math.Abs(1 - cosineAfter * cosineAfter)), 0);

                double cosineBeforeNextPoint = Vector.DotProduct(this[pointIndex + 1].DirectionBefore, chordAfter);
                double sineBeforeNextPoint = Math.Max(Math.Sqrt(Math.Abs(1 - cosineBeforeNextPoint * cosineBeforeNextPoint)), 0);

                if ((cosineAfter > 0) & (cosineBeforeNextPoint > 0) & (sineAfter + sineBeforeNextPoint < 0.01))
                {
                    magnitudeAfter = chordAfterMagnitude;
                }
                else
                {
                    double var = 2 - cosineAfter * cosineBeforeNextPoint - sineAfter * sineBeforeNextPoint;
                    double dena = 1 + cosineBeforeNextPoint * var;
                    magnitudeAfter = 2 * chordAfterMagnitude / Math.Max(dena, 0.6667);
                }

                this[pointIndex].MagnitudeAfter = magnitudeAfter / 3.0;
            }

            // If a spline is closed then we will have created dummy points to force curvature continuity across the start/end of the spline
            // This function will remove the dummy points if they were created
            RemoveDummyPointsFromClosedSpline(wasSplineInitiallyClosed, werePointsAddedToStart, werePointsAddedToEnd);
        }

        /// <summary>
        /// If a spline is closed then we will create dummy points to force curvature continuity across the start/end of the spline
        /// This function will create the dummy points if they are required
        /// </summary>
        /// <param name="pointIndex"> If dummy points are added to the beginning of the spline then point index will be increased accordingly </param>
        /// <param name="wasSplineInitiallyClosed"> We will track whether the spline was closed before the points were added </param>
        /// <param name="werePointsAddedToStart"> This will return true if dummy points were added to the start of the spline </param>
        /// <param name="werePointsAddedToEnd">  This will return true if dummy points were added to the end of the spline  </param>
        /// <remarks></remarks>
        private void AddDummyPointsToClosedSpline(
            ref int pointIndex,
            ref bool wasSplineInitiallyClosed,
            ref bool werePointsAddedToStart,
            ref bool werePointsAddedToEnd)
        {
            wasSplineInitiallyClosed = IsSplineClosed();
            werePointsAddedToStart = false;
            werePointsAddedToEnd = false;

            if (wasSplineInitiallyClosed & (Count > 3))
            {
                // Synchornise the start and end point (which are at the same coordinates)
                this[Count - 1].MagnitudeAfter = this[0].MagnitudeAfter;
                this[Count - 1].DirectionAfter = this[0].DirectionAfter;
                this[0].MagnitudeBefore = this[Count - 1].MagnitudeBefore;
                this[0].DirectionBefore = this[Count - 1].DirectionBefore;

                if (pointIndex > Count - 2)
                {
                    // Create a copy of point 1 and point 2 at the end of the spline
                    // This ensures tangency continuity accross the last point

                    // Copy point 1 and add it to the end of the spline
                    SplinePoint point1 = this[1].Clone();
                    Add(point1);

                    // Copy point 2 and add it to the end of the spline
                    SplinePoint point2 = this[2].Clone();
                    Add(point2);

                    werePointsAddedToEnd = true;
                }
                else if (pointIndex < 2)
                {
                    // We create a copy of points n - 2 and n - 3 and add them to the beginning of the spline
                    // This ensures tangency continuity accross the first point

                    // Copy point n - 2 and add it to the beginning of the spline
                    // Copy point n - 3 and add it to the beginning of the spline
                    SplinePoint pointNMinus2 = this[Count - 2].Clone();
                    SplinePoint pointNMinus3 = this[Count - 3].Clone();
                    Insert(0, pointNMinus2);
                    Insert(0, pointNMinus3);

                    // Increase the point index by two to compensate for the additional points
                    pointIndex = pointIndex + 2;

                    werePointsAddedToStart = true;
                }
            }
        }

        /// <summary>
        /// If a spline is closed then we will create dummy points to force curvature continuity across the start/end of the spline
        /// This function will create the dummy points if they are required
        /// </summary>
        /// <param name="wasSplineInitiallyClosed"> If dummy points were added to a closed spline then it will not be closed any more. This prameter states whether the spline was closed before the dummy points were added </param>
        /// <param name="werePointsAddedToStart"> True if dummy points were added to the start of the spline</param>
        /// <param name="werePointsAddedToEnd"> True if dummy points were added to the end of the spline </param>
        /// <remarks></remarks>
        private void RemoveDummyPointsFromClosedSpline(
            bool wasSplineInitiallyClosed,
            bool werePointsAddedToStart,
            bool werePointsAddedToEnd)
        {
            // If the spline was initially closed then we might have added dummy points
            // If we added dummy points then we will remove them now
            if (wasSplineInitiallyClosed & (Count > 3))
            {
                if (werePointsAddedToEnd)
                {
                    // We created a copy of point 1 and point 2 at the end of the spline
                    RemoveAt(Count - 1);
                    RemoveAt(Count - 1);
                }
                else if (werePointsAddedToStart)
                {
                    // We created a copy of points n - 2 and n - 3 at the begining of the spline
                    RemoveAt(0);
                    RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Approximates the length between the two specified points along the path of the spline.
        /// </summary>
        /// <param name="chunkingTolerance">Measure of acceptable error.</param>
        /// <param name="pointProximityTolerance">Prevents points from being considered in the calculation if they are in too close a proximity to one another.</param>
        /// <param name="startPoint">Point on curve at which to commence length calculation.</param>
        /// <param name="endPoint">Point on curve at which to end length calculation.</param>
        /// <returns>Calculated length.</returns>
        public double GetLengthBetweenPoints(
            double chunkingTolerance,
            double pointProximityTolerance,
            int startPoint = 0,
            int endPoint = 0)
        {
            // The method approximates the curve by splitting it into linear segments
            // As a guide I use a chunking tolerance of (PowerSHAPE general tolerance)^2

            // Split the region of interest of the spline into approximately linear spans
            List<double> originalSpanIndexList = new List<double>();
            Spline chunkedSpline = ChunkSpline(ref originalSpanIndexList,
                                               chunkingTolerance,
                                               pointProximityTolerance,
                                               startPoint,
                                               endPoint);

            // Calculate the length of the span in the region of interest
            double length = 0;
            for (int i = 0; i <= chunkedSpline.Count - 2; i++)
            {
                if ((originalSpanIndexList[i] >= startPoint) & (originalSpanIndexList[i] < endPoint))
                {
                    length = length + (chunkedSpline.GetSpanControlPoint(i, 0) - chunkedSpline.GetSpanControlPoint(i, 3))
                             .Magnitude;
                }
            }

            return length;
        }

        /// <summary>
        /// Private function to check whether a spline is closed.
        /// This exists to be used by the Property Me.Closed() which can get or set whether the spline is closed
        /// </summary>
        private bool IsSplineClosed()
        {
            // The spline is closed if the distance from the first point of the spline to the last point of the spline is zero
            bool isClosed = this[0].DistanceToPoint(this[Count - 1]) == 0;

            return isClosed;
        }

        /// <summary>
        /// Create a linear approximation of a spline
        /// This is used to calculate the length of a spline quickly and accurately
        /// doubles are used instead of doubles to reduce the chance of rounding errors
        /// </summary>
        public Spline ChunkSpline(
            ref List<double> originalSpanIndexList,
            double chunkingTolerance,
            double pointProximityTolerance,
            int startPoint = 0,
            int endPoint = 0)
        {
            originalSpanIndexList = new List<double>();

            int i = 0;
            if ((endPoint == 0) | (endPoint > Count - 1))
            {
                endPoint = Count - 1;
            }

            // Initialise the chunked spline as a copy of the original spline
            Spline chunkedSpline = Clone();

            for (i = 0; i <= chunkedSpline.Count - 1; i++)
            {
                originalSpanIndexList.Add(i);
            }

            i = startPoint;
            while (i < chunkedSpline.Count - (Count - endPoint))
            {
                double curvatureRatio = chunkedSpline.GetBezierCurve(i).GetCurvatureRatio();
                if (curvatureRatio > 1 + chunkingTolerance)
                {
                    bool isSuccess = chunkedSpline.SplitSpan(i, 0.5, pointProximityTolerance);

                    if (isSuccess == false)
                    {
                        i = i + 1;
                    }
                    else
                    {
                        originalSpanIndexList.Insert(i + 1, (originalSpanIndexList[i] + originalSpanIndexList[i + 1]) / 2);
                    }
                }
                else
                {
                    i = i + 1;
                }
            }

            return chunkedSpline;
        }

        /// <summary>
        /// If closed, reorders this spline so that it has a new start point.
        /// </summary>
        /// <param name="newStartIndex">Index of the point to set as start.</param>
        /// <exception cref="Exception">Thrown if newStartIndex is out of range or the spline is open.</exception>
        public void ReorderClosedSpline(int newStartIndex)
        {
            bool amIClosed = IsSplineClosed();
            if (amIClosed)
            {
                int nPoints = this.Count();
                if ((newStartIndex == nPoints - 1) | (newStartIndex == 0))
                {
                    // Do nothing
                }
                else if (newStartIndex >= nPoints)
                {
                    throw new Exception(
                        "Error. The requested start point exceeds the number of points in Spline.ChangeStartPoint(...)");
                }
                else
                {
                    // Synchornise the original start and end points (which are at the same coordinates)
                    this[Count - 1].MagnitudeAfter = this[0].MagnitudeAfter;
                    this[Count - 1].DirectionAfter = this[0].DirectionAfter;
                    this[0].MagnitudeBefore = this[Count - 1].MagnitudeBefore;
                    this[0].DirectionBefore = this[Count - 1].DirectionBefore;

                    // Make a copy off the new start point
                    SplinePoint newP0 = this[newStartIndex].Clone();

                    // Remove the last point, which is a duplicate of the first point
                    RemoveAt(this.Count() - 1);

                    // reorder the spline
                    int i = 0;
                    for (i = 0; i <= newStartIndex - 1; i++)
                    {
                        Add(this[0]);
                        RemoveAt(0);
                    }

                    // Add a new duplicate last point to the spline
                    Add(newP0);
                }
            }
            else
            {
                // The spline is open
                throw new Exception("Error. Cannot change the start point of an open spline in Spline.ChangeStartPoint(...)");
            }
        }

        /// <summary>
        /// Reverses the curve
        /// </summary>
        public void Reverse()
        {
            // Let the points get reversed by the base method
            base.Reverse();

            // Now reverse the direction of all the magnitues
            foreach (SplinePoint point in this)
            {
                Vector tempVector = point.DirectionBefore.Clone();
                point.DirectionBefore = point.DirectionAfter * -1;
                point.DirectionAfter = tempVector * -1;
            }
        }

        /// <summary>
        /// Reverses the curve between the points specified
        /// </summary>
        /// <param name="index">The point to start the reversing from</param>
        /// <param name="count">The number of points to reverse</param>
        public void Reverse(int index, int count)
        {
            // Let the points get reversed by the base method
            base.Reverse(index, count);

            // Now reverse the direction of all the magnitues
            for (int i = index; i <= index + count - 1; i++)
            {
                SplinePoint point = this[i];
                Vector tempVector = point.DirectionBefore.Clone();
                point.DirectionBefore = point.DirectionAfter * -1;
                point.DirectionAfter = tempVector * -1;
            }
        }

        #endregion

        #region "Shared"

        /// <summary>
        /// Evaluates the specific point on a cubic bezier curve.
        /// </summary>
        /// <param name="p0">Control point 1 of the bezier curve.</param>
        /// <param name="p1">Control point 2 of the bezier curve.</param>
        /// <param name="p2">Control point 3 of the bezier curve.</param>
        /// <param name="p3">Control point 4 of the bezier curve.</param>
        /// <param name="t">Position of the points needs to be evaluated on Cubic Bezier curver. Ranges from 0 to 1.</param>
        /// <returns></returns>
        public static Point CubicBezier(Point p0, Point p1, Point p2, Point p3, double t)
        {
            // Mathematical equation to compute point on cubic bezier curve: 
            // Ref: https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Cubic_B.C3.A9zier_curves
            // B(t) = (1-t)^3 P0 + 3(1-t)^2 P1 + 3(1-t)t^2 P2 + t^3 P3, 0 <= t <= 1

            if (t > 1 || t < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(t), "t must be in range from 0 to 1.");
            }

            Point a = new Point();
            Point b = new Point();
            Point c = new Point();
            Point p = new Point();

            c.X = 3.0 * (p1.X - p0.X);
            c.Y = 3.0 * (p1.Y - p0.Y);
            c.Z = 3.0 * (p1.Z - p0.Z);
            b.X = 3.0 * (p2.X - p1.X) - c.X;
            b.Y = 3.0 * (p2.Y - p1.Y) - c.Y;
            b.Z = 3.0 * (p2.Z - p1.Z) - c.Z;
            a.X = p3.X - p0.X - c.X - b.X;
            a.Y = p3.Y - p0.Y - c.Y - b.Y;
            a.Z = p3.Z - p0.Z - c.Z - b.Z;

            p.X = a.X * Math.Pow(t, 3) + b.X * Math.Pow(t, 2) + c.X * t + p0.X;
            p.Y = a.Y * Math.Pow(t, 3) + b.Y * Math.Pow(t, 2) + c.Y * t + p0.Y;
            p.Z = a.Z * Math.Pow(t, 3) + b.Z * Math.Pow(t, 2) + c.Z * t + p0.Z;

            return p;
        }

        /// <summary>
        /// Extracts spline curves from a DUCT picture file and returns them to the caller.
        /// </summary>
        /// <param name="file">Path to the DUCT picture file.</param>
        /// <returns>A list of spline curves.</returns>
        public static List<Spline> ReadFromDUCTPictureFile(File file)
        {
            List<Spline> splineCurves = new List<Spline>();

            string CurCult = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");

                string strBuff = "";
                using (
                    System.IO.FileStream FS = new System.IO.FileStream(file.Path,
                                                                       System.IO.FileMode.Open,
                                                                       System.IO.FileAccess.Read))
                {
                    using (System.IO.StreamReader SR = new System.IO.StreamReader(FS))
                    {
                        strBuff = SR.ReadToEnd();
                        SR.Close();
                    }
                    FS.Close();
                }
                strBuff = strBuff.Replace(Strings.Chr(9).ToString(), "");
                strBuff = strBuff.Replace(Strings.Chr(13).ToString(), "");
                string[] strSpl = strBuff.Split(Strings.Chr(10));

                List<List<int>> instructions = new List<List<int>>();
                List<Point> points = new List<Point>();

                // Check to see if the file is in inches or mm
                bool isInInches = strSpl[1].Trim().ToLower().EndsWith("inches");

                // Ignore the header lines just concentrate on the juicy bits
                for (int lineIndex = 5; lineIndex <= strSpl.Length - 1; lineIndex++)
                {
                    // Collect up all the numeric values on the line
                    List<double> elementsOnLine = new List<double>();
                    foreach (string stringElement in strSpl[lineIndex].Split(' '))
                    {
                        double value = 0;
                        if (double.TryParse(stringElement,
                                            NumberStyles.Any,
                                            CultureInfo.InvariantCulture,
                                            out value))
                        {
                            elementsOnLine.Add(value);
                        }
                    }
                    if (elementsOnLine.Count == 2)
                    {
                        // If there are two values then it is an instruction line
                        instructions.Add(new List<int>
                        {
                            Convert.ToInt32(elementsOnLine[0]),
                            Convert.ToInt32(elementsOnLine[1])
                        });
                    }
                    else if (elementsOnLine.Count >= 3)
                    {
                        // Otherwise it is a point data line
                        if (isInInches)
                        {
                            points.Add(new Point(elementsOnLine[0] * 25.4, elementsOnLine[1] * 25.4, elementsOnLine[2] * 25.4));
                        }
                        else
                        {
                            points.Add(new Point(elementsOnLine[0], elementsOnLine[1], elementsOnLine[2]));
                        }
                    }
                }

                // So we have all the data, now we need to populate the polyline and spline lists

                // This keeps track of which point we are looking at for the current instruction
                int pointIndex = 0;

                for (int instructionIndex = 0; instructionIndex <= instructions.Count - 1; instructionIndex++)
                {
                    if ((instructions[instructionIndex][0] & 1024) == 1024)
                    {
                        // Bezier curves
                        Spline spline = new Spline();
                        int pixelIndex = 1;
                        if ((instructions[instructionIndex][0] & 24) == 24)
                        {
                            pixelIndex = -1;
                        }
                        while (pixelIndex < instructions[instructionIndex][1])
                            if (pixelIndex == -1)
                            {
                                // First pixel only has tangency out
                                Vector directionOut = points[pointIndex + pixelIndex + 1] - points[pointIndex + pixelIndex];
                                double distanceOut = directionOut.Magnitude;
                                directionOut.Normalize();
                                spline.Add(new SplinePoint(points[pointIndex + pixelIndex],
                                                           directionOut,
                                                           distanceOut,
                                                           directionOut,
                                                           distanceOut));
                                pixelIndex += 3;
                            }
                            else if (instructions[instructionIndex][1] - pixelIndex == 1)
                            {
                                // Last pixel only has tangency in
                                Vector directionIn = points[pointIndex + pixelIndex] - points[pointIndex + pixelIndex - 1];
                                double distanceIn = directionIn.Magnitude;
                                directionIn.Normalize();
                                spline.Add(new SplinePoint(points[pointIndex + pixelIndex],
                                                           directionIn,
                                                           distanceIn,
                                                           directionIn,
                                                           distanceIn));
                                pixelIndex += 3;
                            }
                            else
                            {
                                // Pixel has tangency in and out
                                Vector directionIn = points[pointIndex + pixelIndex] - points[pointIndex + pixelIndex - 1];
                                double distanceIn = directionIn.Magnitude;
                                directionIn.Normalize();
                                Vector directionOut = points[pointIndex + pixelIndex + 1] - points[pointIndex + pixelIndex];
                                double distanceOut = directionOut.Magnitude;
                                directionOut.Normalize();
                                spline.Add(new SplinePoint(points[pointIndex + pixelIndex],
                                                           directionIn,
                                                           distanceIn,
                                                           directionOut,
                                                           distanceOut));
                                pixelIndex += 3;
                            }
                        if ((instructions[instructionIndex][0] & 24) == 0)
                        {
                            // Starting a new section
                            splineCurves.Add(spline);
                        }
                        else
                        {
                            // Continuing the last section
                            // Set the last point of the last section to work like the first point of this section
                            splineCurves[splineCurves.Count - 1][splineCurves[splineCurves.Count - 1].Count - 1].DirectionAfter =
                                spline[0].DirectionBefore;
                            splineCurves[splineCurves.Count - 1][splineCurves[splineCurves.Count - 1].Count - 1].MagnitudeAfter =
                                spline[0].MagnitudeBefore;

                            // Now add this section apart from the first point
                            for (int i = 1; i <= spline.Count - 1; i++)
                            {
                                splineCurves[splineCurves.Count - 1].Add(new SplinePoint(spline[i],
                                                                                         spline[i].DirectionBefore,
                                                                                         spline[i].MagnitudeBefore,
                                                                                         spline[i].DirectionAfter,
                                                                                         spline[i].MagnitudeAfter));
                            }
                        }
                        pointIndex += instructions[instructionIndex][1];
                    }
                    else if ((instructions[instructionIndex][0] & 512) == 512)
                    {
                        // Conic arcs
                        Spline intermediateCurve = new Spline();
                        Spline splineCurve = new Spline();
                        int pixelIndex = 0;
                        if ((instructions[instructionIndex][0] & 24) == 24)
                        {
                            pixelIndex = -1;
                        }
                        while (pixelIndex < instructions[instructionIndex][1])
                        {
                            intermediateCurve.Add(new SplinePoint(points[pixelIndex + pointIndex]));

                            // If there are three points on the curve
                            if (intermediateCurve.Count == 3)
                            {
                                intermediateCurve.FreeTangentsAndMagnitudes();
                                if (splineCurve.Count != 0)
                                {
                                    // If the spline curve already has points then dont add the first one, just set the output tangent and magnitude 
                                    splineCurve.Last().MagnitudeAfter = intermediateCurve[0].MagnitudeAfter;
                                    splineCurve.Last().DirectionAfter = intermediateCurve[0].DirectionAfter;
                                }
                                else
                                {
                                    // else add first point
                                    splineCurve.Add(intermediateCurve[0]);
                                }

                                // add second and third point
                                splineCurve.Add(intermediateCurve[1]);
                                splineCurve.Add(intermediateCurve[2]);
                                var p = intermediateCurve[2];

                                // reset intermediate curve and add last point
                                intermediateCurve.Clear();
                                intermediateCurve.Add(p.Clone());
                            }

                            pixelIndex += 1;
                        }
                        if ((instructions[instructionIndex][0] & 24) == 0)
                        {
                            // Starting a new section
                            splineCurves.Add(splineCurve);
                        }
                        else
                        {
                            // Continuing the last section
                            splineCurves[splineCurves.Count - 1][splineCurves[splineCurves.Count - 1].Count - 1].DirectionAfter =
                                splineCurve[0].DirectionAfter;
                            splineCurves[splineCurves.Count - 1][splineCurves[splineCurves.Count - 1].Count - 1].MagnitudeAfter =
                                splineCurve[0].MagnitudeAfter;

                            // Add this spline curve to the last one
                            for (int i = 1; i <= splineCurve.Count - 1; i++)
                            {
                                splineCurves[splineCurves.Count - 1].Add(new SplinePoint(splineCurve[i],
                                                                                         splineCurve[i].DirectionBefore,
                                                                                         splineCurve[i].MagnitudeBefore,
                                                                                         splineCurve[i].DirectionAfter,
                                                                                         splineCurve[i].MagnitudeAfter));
                            }
                        }
                        pointIndex += instructions[instructionIndex][1];
                    }
                    else
                    {
                        // Polylines
                        if (instructions[instructionIndex][1] == 1)
                        {
                            Spline splineCurve = new Spline();
                            splineCurve.Add(new SplinePoint(points[pointIndex]));
                            splineCurves.Add(splineCurve);
                            pointIndex += 1;
                        }
                        else
                        {
                            // Get the polyline
                            Polyline polyline =
                                new Polyline(points.GetRange(pointIndex, instructions[instructionIndex][1]).ToList());

                            // Turn it into a spline curve
                            Spline splineCurve = new Spline();
                            for (int i = 0; i <= polyline.Count - 1; i++)
                            {
                                if (i == 0)
                                {
                                    // Only need to sort direction and magnitude out
                                    Vector dirOut = polyline[i + 1] - polyline[i];
                                    dirOut.Normalize();
                                    splineCurve.Add(new SplinePoint(polyline[i], dirOut, 1, dirOut, 1));
                                }
                                else if (i == polyline.Count - 1)
                                {
                                    // Only need to sort direction and magnitude in
                                    Vector dirIn = polyline[i] - polyline[i - 1];
                                    dirIn.Normalize();
                                    splineCurve.Add(new SplinePoint(polyline[i], dirIn, 1, dirIn, 1));
                                }
                                else
                                {
                                    // Need to sort direction in and out
                                    Vector dirIn = polyline[i] - polyline[i - 1];
                                    dirIn.Normalize();
                                    Vector dirOut = polyline[i + 1] - polyline[i];
                                    dirOut.Normalize();
                                    splineCurve.Add(new SplinePoint(polyline[i], dirIn, 1, dirOut, 1));
                                }
                            }
                            if ((instructions[instructionIndex][0] & 24) == 0)
                            {
                                // Starting a new section
                                splineCurves.Add(splineCurve);
                            }
                            else
                            {
                                // Continuing the last section
                                // Point the last point to the first one here
                                Vector dir = splineCurve[0] - splineCurves.Last().Last();
                                splineCurves[splineCurves.Count - 1][splineCurves[splineCurves.Count - 1].Count - 1]
                                    .DirectionAfter = dir;
                                splineCurves[splineCurves.Count - 1][splineCurves[splineCurves.Count - 1].Count - 1]
                                    .MagnitudeAfter = 1;

                                // Point the first point of this curve to be the same
                                splineCurve[0].DirectionBefore = dir;
                                splineCurve[0].MagnitudeBefore = 1;

                                // Add this spline curve to the last one
                                for (int i = 0; i <= splineCurve.Count - 1; i++)
                                {
                                    splineCurves[splineCurves.Count - 1].Add(new SplinePoint(splineCurve[i],
                                                                                             splineCurve[i].DirectionBefore,
                                                                                             splineCurve[i].MagnitudeBefore,
                                                                                             splineCurve[i].DirectionAfter,
                                                                                             splineCurve[i].MagnitudeAfter));
                                }
                            }
                            pointIndex += instructions[instructionIndex][1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(CurCult);
            }

            return splineCurves;
        }

        private static int GetInstructionCode(int IPCOL, int IARC, int IIMRK, int IJRMK, int IUPDN, int IDASH)
        {
            //TODO: Comment this code

            return IPCOL * 2048 + IARC * 512 + IIMRK * 64 + IJRMK * 32 + IUPDN * 8 + IDASH * 1;
        }

        private static string GetField(int Value, int FieldLength)
        {
            //TODO: Comment this code

            string tmp = Value.ToString("0");
            tmp = Strings.Space(FieldLength - tmp.Length) + tmp;
            return tmp;
        }

        private static string GetField(double Value, string FormatCode, int FieldLength)
        {
            //TODO: Comment this code

            //Use System.Globalization.CultureInfo.InvariantCulture
            string tmp = Value.ToString(FormatCode, CultureInfo.InvariantCulture);
            tmp = Strings.Space(FieldLength - tmp.Length) + tmp;
            return tmp;
        }

        #endregion
    }
}