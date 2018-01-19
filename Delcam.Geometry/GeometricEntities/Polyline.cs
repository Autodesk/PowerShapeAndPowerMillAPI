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
    /// Encapsulates a Polyline.
    /// A Polyline comprises a collection of points, each connected by a straight line.
    /// If marked as closed, the start and end points are also joined by a line;
    /// it is not necessary to duplicate the start point at the end of the list in order to close the shape.
    /// </summary>
    [Serializable]
    public class Polyline : List<Point>
    {
        #region "Fields"

        /// <summary>
        /// This indicates whether the polyline should be treated as a closed polyline or not.
        /// This removes the need to hold an extra end point that is the same as the start point.
        /// It also makes it less complex to work with.  If you moved the start point then in order
        /// to ensure the curve stayed closed you would also need to move the end point
        /// </summary>
        private bool _isClosed;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an empty PolyLine.
        /// </summary>
        public Polyline()
        {
            _isClosed = false;
        }

        /// <summary>
        /// Constructs a PolyLine with the specified list of points.
        /// </summary>
        /// <param name="points">Points with which to initialise the Polyline.</param>
        public Polyline(List<Point> points)
        {
            AddRange(points);
            _isClosed = false;

            CleanUpPolylineClosure();
        }

        /// <summary>
        /// Constructs a Polyline, initialises the list of points to the specified value and, if specified, closes the line.
        /// </summary>
        /// <param name="points">List of points with which to initialise the Polyline.</param>
        /// <param name="isClosed">If True, the Polyline is closed. False otherwise.</param>
        public Polyline(List<Point> points, bool isClosed)
        {
            AddRange(points);
            _isClosed = isClosed;

            CleanUpPolylineClosure();
        }

        /// <summary>
        /// Constructs a Polyline from a SplineCurve, fitting it to the specified tolerance.
        /// </summary>
        /// <param name="splineCurve">SplineCurve from which to create the Polyline.</param>
        /// <param name="tolerance">Square tolerance within which to fit the spline curve.</param>
        public Polyline(Spline splineCurve, double tolerance)
        {
            // Create a new array for the points. NOTE: Spline points will contain magnitude and direction, to maintain this curve
            // we will add control points into the array below.
            Point[] C = new Point[-1 + 1];
            C = new Point[1];
            C[0] = new Point(splineCurve[0].X, splineCurve[0].Y, splineCurve[0].Z);

            for (int i = 1; i <= splineCurve.Count - 1; i++)
            {
                // Increase the arry size by 3 (2 control points and next point)
                Array.Resize(ref C, C.Length + 3);

                // Create the control points for current point on the curve and the previous one. 
                Point CP1 = splineCurve[i - 1] + splineCurve[i - 1].DirectionAfter * splineCurve[i - 1].MagnitudeAfter;
                Point CP2 = splineCurve[i] - splineCurve[i].DirectionBefore * splineCurve[i].MagnitudeBefore;

                // Add the control points to the arry
                C[C.Length - 3] = new Point(CP1.X, CP1.Y, CP1.Z);
                C[C.Length - 2] = new Point(CP2.X, CP2.Y, CP2.Z);

                // Add the current point into the new array.
                C[C.Length - 1] = new Point(splineCurve[i].X, splineCurve[i].Y, splineCurve[i].Z);
            }

            // Gets the array of points needed to create the poly line
            Point[] Pts = CurveFunctions.PolygoniseCubicCurve(C, tolerance);
            Clear();

            // Creates the new poly line.
            for (int i = 0; i <= Pts.Length - 1; i++)
            {
                Add(Pts[i]);
            }
        }

        /// <summary>
        /// Constructs a Polyline and initialises it from an ASCII file.
        /// Each line of the file comprises the three coordinates of each point seperated by spaces. <br></br>
        /// I.e. <br></br><br></br>
        /// A.AAA B.BBB C.CCC <br></br>
        /// D.DDD E.EEE F.FFF <br></br>
        /// ... <br></br>
        /// etc.
        /// </summary>
        /// <param name="asciiFile">ASCII file from which to read the Polyline data.</param>
        public Polyline(File asciiFile)
        {
            if (asciiFile.Exists == false)
            {
                return;
            }

            foreach (string textLine in asciiFile.ReadTextLines())
            {
                Point point = new Point(textLine, ' ');

                if (point != null)
                {
                    Add(point);
                }
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// BoundingBox of the Polyline. Nothing if the Polyline contains no points.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }
                double minX = this[0].X;
                double maxX = this[0].X;
                double minY = this[0].Y;
                double maxY = this[0].Y;
                double minZ = this[0].Z;
                double maxZ = this[0].Z;

                foreach (Point point in this)
                {
                    if (point.X.Value < minX)
                    {
                        minX = point.X;
                    }
                    if (point.X.Value > maxX)
                    {
                        maxX = point.X;
                    }
                    if (point.Y.Value < minY)
                    {
                        minY = point.Y;
                    }
                    if (point.Y.Value > maxY)
                    {
                        maxY = point.Y;
                    }
                    if (point.Z.Value < minZ)
                    {
                        minZ = point.Z;
                    }
                    if (point.Z.Value > maxZ)
                    {
                        maxZ = point.Z;
                    }
                }

                return new BoundingBox(minX, maxX, minY, maxY, minZ, maxZ);
            }
        }

        /// <summary>
        /// Length of the Polyline.
        /// </summary>
        public MM Length
        {
            get
            {
                MM polylineLength = 0.0;

                // Create vector between each point and add its magnitude to the combined length
                for (int index = 0; index <= Count - 2; index++)
                {
                    polylineLength += (this[index] - this[index + 1]).Magnitude;
                }

                // If the Polyline is closed then add the distance from the last point to the first too
                if (_isClosed & (Count > 1))
                {
                    polylineLength += (this[Count - 1] - this[0]).Magnitude;
                }

                return polylineLength;
            }
        }

        /// <summary>
        /// List of distances between consecutive points. The 1st element of the list will be zero because it is the first point.
        /// The 2nd element of the list will be the distance between the 2nd point of the polyline and the 1st point. In generic terms, LengthMap
        /// will contain Point(n) - Point(n-1).
        /// </summary>
        public List<MM> LengthMap
        {
            get
            {
                var polylineLengthMap = new List<MM>();
                var index = 0;
                double distance = 0;

                while (index <= Count - 1)
                {
                    if (index == 0)
                    {
                        polylineLengthMap.Add(0);
                    }
                    else
                    {
                        distance = distance + (this[index] - this[index - 1]).Magnitude;
                        polylineLengthMap.Add(distance);
                    }
                    index = index + 1;
                }

                return polylineLengthMap;
            }
        }

        /// <summary>
        /// Centre point of the Polyline.
        /// </summary>
        public Point Centre
        {
            get
            {
                int numberOfPoints = Count;
                if (numberOfPoints == 0)
                {
                    return null;
                }
                Point centrePoint = new Point(0, 0, 0);

                foreach (Point p in this)
                {
                    centrePoint.X += p.X;
                    centrePoint.Y += p.Y;
                    centrePoint.Z += p.Z;
                }

                centrePoint.X = centrePoint.X / (double) numberOfPoints;
                centrePoint.Y = centrePoint.Y / (double) numberOfPoints;
                centrePoint.Z = centrePoint.Z / (double) numberOfPoints;

                return centrePoint;
            }
        }

        /// <summary>
        /// Vector normal to the centre of the Polyline.
        /// </summary>
        public Vector Normal
        {
            get
            {
                bool blnClosed = (this[0] - this[Count - 1]).Magnitude.Value < 1E-06;
                Vector vecNormal = new Vector();
                Point ptCentre = Centre;
                for (int i = 0; i <= Count - 2; i++)
                {
                    Vector vecNext = new Vector();
                    vecNext = Vector.CrossProduct(this[i] - ptCentre, this[i + 1] - ptCentre);
                    vecNext.Normalize();
                    vecNormal += vecNext;
                }
                vecNormal.Normalize();
                return vecNormal;
            }
        }

        /// <summary>
        /// True if the Polyline is closed; false otherwise.
        /// </summary>
        public bool IsClosed
        {
            get { return _isClosed; }

            set { _isClosed = value; }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Extracts Polylines from a DUCT picture file and returns them as a list.
        /// </summary>
        /// <param name="file">DUCT picture file.</param>
        /// <returns>List of polylines.</returns>
        public static List<Polyline> ReadFromDUCTPictureFile(File file)
        {
            List<Polyline> polylines = new List<Polyline>();

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
                            Polyline polyline = new Polyline(spline, 0.01);
                            polylines.Add(polyline);
                        }
                        else
                        {
                            // Continuing the last section
                            Polyline polyline = new Polyline(spline, 0.01);

                            // Add all points apart from the first one
                            for (int i = 1; i <= polyline.Count - 1; i++)
                            {
                                polylines[polylines.Count - 1].Add(polyline[i]);
                            }
                        }
                        pointIndex += instructions[instructionIndex][1];
                    }
                    else if ((instructions[instructionIndex][0] & 512) == 512)
                    {
                        // Conic arcs
                        Spline intermediateCurve = new Spline();
                        Spline spline = new Spline();
                        int pixelIndex = 0;
                        if ((instructions[instructionIndex][0] & 24) == 24)
                        {
                            pixelIndex = -1;
                        }
                        while (pixelIndex < instructions[instructionIndex][1])
                        {
                            intermediateCurve.Add(new SplinePoint(points[pointIndex + pixelIndex]));

                            // If there are three points on the curve
                            if (intermediateCurve.Count == 3)
                            {
                                intermediateCurve.FreeTangentsAndMagnitudes();
                                if (spline.Count != 0)
                                {
                                    // If the spline curve already has points then dont add the first one, just set the output tangent and magnitude 
                                    spline.Last().MagnitudeAfter = intermediateCurve[0].MagnitudeAfter;
                                    spline.Last().DirectionAfter = intermediateCurve[0].DirectionAfter;
                                }
                                else
                                {
                                    // else add first point
                                    spline.Add(intermediateCurve[0]);
                                }

                                // add second and third point
                                spline.Add(intermediateCurve[1]);
                                spline.Add(intermediateCurve[2]);
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
                            Polyline polyline = new Polyline(spline, 0.01);
                            polylines.Add(polyline);
                        }
                        else
                        {
                            // Continuing the last section
                            Polyline polyline = new Polyline(spline, 0.01);

                            // Add all points apart from the first one
                            for (int i = 1; i <= polyline.Count - 1; i++)
                            {
                                polylines[polylines.Count - 1].Add(polyline[i]);
                            }
                        }
                        pointIndex += instructions[instructionIndex][1];
                    }
                    else
                    {
                        // Polylines
                        if ((instructions[instructionIndex][0] & 24) == 0)
                        {
                            // Starting a new section
                            Polyline polyline =
                                new Polyline(
                                    new List<Point>(points.GetRange(pointIndex, instructions[instructionIndex][1]).ToArray()));
                            polylines.Add(polyline);
                        }
                        else
                        {
                            // Continuing the last section
                            polylines[polylines.Count - 1].AddRange(
                                points.GetRange(pointIndex, instructions[instructionIndex][1]).ToArray());
                        }
                        pointIndex += instructions[instructionIndex][1];
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

            return polylines;
        }

        /// <summary>
        /// Writes this Polyline to the specified DUCT picture file.
        /// The file is always written in MM, as that is the unit of measure of the Polyline.
        /// </summary>
        /// <param name="file">DUCT file to be written.</param>
        public void WriteToDUCTPictureFile(File file)
        {
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
                if (Count > 1000000)
                {
                    integerLine += " " + GetField(4, 3) + GetField(3, 3) + GetField(-1, 11) + GetField(1, 11) +
                                   GetField(Count, 11) + GetField(-2, 6) + GetField(2, 6) + GetField(0, 6) + GetField(0, 6) +
                                   GetField(0, 6);
                }
                else
                {
                    integerLine += " " + GetField(4, 6) + GetField(3, 6) + GetField(-1, 6) + GetField(1, 6) + GetField(Count, 6) +
                                   GetField(-2, 6) + GetField(2, 6) + GetField(0, 6) + GetField(0, 6) + GetField(0, 6);
                }
                pictureData.Append(integerLine + Constants.vbNewLine);

                // Write size data
                string sizeLine = "";
                sizeLine += " " + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) +
                            GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) + GetField(0, "0.0000", 10) +
                            GetField(0, "0.0000", 10);
                pictureData.Append(sizeLine + Constants.vbNewLine);

                // Write instruction codes
                int instructionCode = GetInstructionCode(1, 0, 0, 0, 0, 0);
                string instructionCodeString = "";
                if (Count > 1000000)
                {
                    instructionCodeString = " " + GetField(instructionCode, 11) + " " + GetField(Count, 11) + Constants.vbNewLine;
                    pictureData.Append(instructionCodeString);
                }
                else
                {
                    instructionCodeString = " " + GetField(instructionCode, 11) + " " + GetField(Count, 6) + Constants.vbNewLine;
                    pictureData.Append(instructionCodeString);
                }

                // Write polyline data
                string pointCode = null;
                for (int p = 0; p <= Count - 1; p++)
                {
                    //Use System.Globalization.CultureInfo.InvariantCulture
                    pointCode = string.Format(CultureInfo.InvariantCulture,
                                              " {0,14:E} {1,14:E} {2,14:E}",
                                              this[p].X,
                                              this[p].Y,
                                              this[p].Z);
                    pictureData.Append(pointCode + Constants.vbNewLine);
                }

                // If it is closed the write the first point again
                if (_isClosed & (Count > 0))
                {
                    pointCode = string.Format(CultureInfo.InvariantCulture,
                                              " {0,14:E} {1,14:E} {2,14:E}",
                                              this[0].X,
                                              this[0].Y,
                                              this[0].Z);
                    pictureData.Append(pointCode + Constants.vbNewLine);
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
        /// Writes the Polyline to the specified ascii file.
        /// Each point on the Polyline is written as a seperate line in the file,
        /// each coordinate being delimited from the next with a space.
        /// </summary>
        /// <param name="asciiFile">ASCII file to to be written.</param>
        public void WriteToASCIIFile(File asciiFile)
        {
            // Delete the file if it already exists
            asciiFile.Delete();

            StringBuilder textLines = new StringBuilder();

            foreach (Point point in this)
            {
                textLines.AppendLine(point.X.ToString() + " " + point.Y.ToString() + " " + point.Z.ToString());
            }

            // If the polyline is closed the write the first point again
            if (_isClosed & (Count > 0))
            {
                textLines.AppendLine(this[0].X.ToString() + " " + this[0].Y.ToString() + " " + this[0].Z.ToString());
            }

            asciiFile.WriteText(textLines.ToString());
        }

        /// <summary>
        /// Returns to the caller the index of the next point after the one specified.
        /// If the specified index is greater or equal to the end of the line, the start index is returned.
        /// </summary>
        /// <param name="index">Index prior to the one that is to be returned.</param>
        public int NextPointIndex(int index)
        {
            // Increment the index
            int nextIndex = index + 1;

            // Handle wraparound
            if (nextIndex >= Count)
            {
                nextIndex = 0;
            }

            // Return next index
            return nextIndex;
        }

        /// <summary>
        /// Returns to the caller the index of the point on the Polyline that is nearest to that specified in the single input argument.
        /// </summary>
        /// <param name="testPoint">Point for which to find the nearest point on the Polyline.</param>
        /// <returns>Index of the nearest Polyline point to that specified.</returns>
        /// <exception cref="Exception">Thrown if the PolyLine contains no points.</exception>
        public int IndexOfNearestPoint(Point testPoint)
        {
            if (Count == 0)
            {
                throw new Exception("The polyline does not contain any points");
            }

            // Start with the nearest distance as being the largest possible distance
            MM nearestDistance = double.PositiveInfinity;
            int nearestIndex = 0;

            int index = 0;
            foreach (Point polylinePoint in this)
            {
                // Find out how far this point is from the test point
                MM thisDistance = (testPoint - polylinePoint).Magnitude;

                // If it is the nearest one so far then
                if (thisDistance < nearestDistance)
                {
                    // Set this as the nearest point
                    nearestDistance = thisDistance;
                    nearestIndex = index;
                }
                index += 1;
            }

            // Return the nearest index
            return nearestIndex;
        }

        /// <summary>
        /// Returns a PointCloud of all points on the PolyLine.
        /// </summary>
        public PointCloud ToPointCloud()
        {
            // Create a point cloud from the points and return it
            PointCloud pointCloud = new PointCloud();

            foreach (Point point in this)
            {
                pointCloud.Add(point.Clone());
            }

            return pointCloud;
        }

        /// <summary>
        /// Ensures that no connecting line exceeds the specified span length.
        /// If such a line exists, a point(s) is inserted and the line split.
        /// Linearity is preserved.
        /// </summary>
        /// <param name="maximumSpanLength">Maximum permissible span length.</param>
        /// <remarks></remarks>
        public void Densify(double maximumSpanLength)
        {
            int currentMax = Count - 1;
            var newPoints = new Point[currentMax];

            newPoints[0] = this[0];
            var numberOfPoints = 1;

            for (int p = 1; p < Count; p++)
            {
                var v1 = this[p] - this[p - 1];
                double vectorLength = v1.Magnitude;

                // If the length of the line connecting two consecutive points is greater than max
                if (vectorLength > maximumSpanLength)
                {
                    // Densify between these two points
                    int numberToInsert = Convert.ToInt32(Math.Floor(vectorLength / maximumSpanLength) - 1);

                    // Calculate actual spacing such that new points will be evenly distributed
                    double actualSpacing = vectorLength / (1 + numberToInsert);

                    v1.Normalize();
                    Point tempEndPoint = this[p - 1];

                    // Make sure we have enough space to store new points
                    if (numberOfPoints + numberToInsert > currentMax)
                    {
                        Array.Resize(ref newPoints, currentMax + numberToInsert + 1); // Include existing last point
                        currentMax += numberToInsert;
                    }

                    // Add incremental points to the array
                    // Last one should end up the same with this[p]
                    for (int i = 1; i <= numberToInsert; i++)
                    {
                        Vector v2 = v1 * actualSpacing;
                        tempEndPoint = tempEndPoint + v2;
                        newPoints[numberOfPoints] = tempEndPoint;
                        numberOfPoints += 1;
                    }
                }
                newPoints[numberOfPoints] = this[p];
                numberOfPoints += 1;
            }
            Array.Resize(ref newPoints, numberOfPoints);

            Clear();

            AddRange(newPoints);
        }

        /// <summary>
        /// Repoints the curve creating the specified number of points.
        /// </summary>
        /// <param name="numberOfPoints">The number of new points that are added to the curve and evenly distributed between the start and end points.</param>
        public void Repoint(int numberOfPoints)
        {
            var inputPoints = new Point[numberOfPoints];

            // Get distance between points for them to be equidistant
            double distanceBetweenPoints = (double) Length / (numberOfPoints - 1);

            // Initialize 1st and last point
            inputPoints[0] = this.First();
            inputPoints[numberOfPoints - 1] = this.Last();

            var index1 = 1;
            while (index1 <= numberOfPoints - 2)
            {
                double distanceTillPoint = distanceBetweenPoints * index1;
                var index2 = 0;

                while (index2 <= Count - 1)
                {
                    if (LengthMap[index2] >= distanceTillPoint)
                    {
                        // If Me(index2) is at expected distanceTillPoint keep it
                        if (Math.Abs(distanceTillPoint - LengthMap[index2].Value) < double.Epsilon)
                        {
                            inputPoints[index1] = this[index2];
                            break;
                        }

                        // If Me(index2) is not at expected distanceTillPoint then add to the previous point the needed distance
                        // to be at distanceTillPoint from the previous point
                        double proportionOfThePreviousPointDistance = (distanceTillPoint - LengthMap[index2 - 1]) /
                                                                      (LengthMap[index2] - LengthMap[index2 - 1]);
                        inputPoints[index1] = this[index2 - 1] +
                                              proportionOfThePreviousPointDistance * (this[index2] - this[index2 - 1]);
                        break;
                    }
                    index2 = index2 + 1;
                }
                index1 = index1 + 1;
            }

            Clear();
            foreach (Point objPoint in inputPoints)
            {
                Add(objPoint);
            }
        }

        #endregion

        #region "Private Operations"

        /// <summary>
        /// This sub ensures that the Polyline does not contain an unnecessary duplication of the
        /// first point to make the Polyline closed.  If it finds a duplicate it removes it and marks
        /// the Polyline closed.  Otherwise the Polyline stays unaffected.
        /// </summary>
        private void CleanUpPolylineClosure()
        {
            // Check for a duplicate start and end point
            if (Count > 1)
            {
                if (this[0] == this[Count - 1])
                {
                    // Found one so remove it and mark the polyline as closed
                    RemoveAt(Count - 1);
                    _isClosed = true;
                }
            }
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

        #region "Transform Operations"

        /// <summary>
        /// Scales the Polyline about the origin by the specified scaling factor. All points are scaled.
        /// </summary>
        /// <param name="scalar">Factor by which to scale.</param>
        public void Scale(double scalar)
        {
            Scale(new Point(0.0, 0.0, 0.0), scalar);
        }

        /// <summary>
        /// Scales the Polyline about the specified origin and by the specified scaling factor. All points are scaled.
        /// </summary>
        /// <param name="origin">Origin about which to scale.</param>
        /// <param name="scalar">Factor by which to scale.</param>
        public void Scale(Point origin, double scalar)
        {
            Scale(origin, scalar, scalar, scalar);
        }

        /// <summary>
        /// Scales the Polyline about the origin using the specified scaling factors. All points are scaled.
        /// </summary>
        /// <param name="scalarX">Factor by which to scale in X.</param>
        /// <param name="scalarY">Factor by which to scale in Y.</param>
        /// <param name="scalarZ">Factor by which to scale in Z.</param>
        public void Scale(double scalarX, double scalarY, double scalarZ)
        {
            Scale(new Point(0.0, 0.0, 0.0), scalarX, scalarY, scalarZ);
        }

        /// <summary>
        /// Scales the Polyline about the specified origin and using the specified scaling factors. All points are scaled.
        /// </summary>
        /// <param name="origin">Origin about which to scale.</param>
        /// <param name="scalarX">Factor by which to scale in X.</param>
        /// <param name="scalarY">Factor by which to scale in Y.</param>
        /// <param name="scalarZ">Factor by which to scale in Z.</param>
        public void Scale(Point origin, double scalarX, double scalarY, double scalarZ)
        {
            // Move the polyline so the specified origin is at the polyline's origin
            Vector offset = new Vector(-origin.X, -origin.Y, -origin.Z);
            Move(offset);

            // Scale the points
            foreach (Point point in this)
            {
                point.X *= scalarX;
                point.Y *= scalarY;
                point.Z *= scalarZ;
            }

            // Move the polyline back
            Move(-1 * offset);
        }

        /// <summary>
        /// Moves the polyline by the specified Vector. All points are displaced.
        /// </summary>
        /// <param name="offset">Vector specifying offset.</param>
        public void Move(Vector offset)
        {
            for (int i = 0; i <= Count - 1; i++)
            {
                this[i] = this[i] + offset;
            }
        }

        /// <summary>
        /// Rotates the entire polyline by the specified angle about the X axis and the specified origin within the ActiveWorkplane - or the world if none is set.
        /// </summary>
        /// <param name="origin">Origin about which to rotate the line.</param>
        /// <param name="angle">Number of radians to rotate.</param>
        public void RotateAboutXAxis(Point origin, Radian angle)
        {
            foreach (Point objPoint in this)
            {
                objPoint.RotateAboutXAxis(origin, angle);
            }
        }

        /// <summary>
        /// Rotates the entire polyline by the specified angle about the Y axis and the specified origin within the ActiveWorkplane - or the world if none is set.
        /// </summary>
        /// <param name="origin">Origin about which to rotate the line.</param>
        /// <param name="angle">Number of radians to rotate.</param>
        public void RotateAboutYAxis(Point origin, Radian angle)
        {
            foreach (Point objPoint in this)
            {
                objPoint.RotateAboutYAxis(origin, angle);
            }
        }

        /// <summary>
        /// Rotates the entire polyline by the specified angle about the Z axis and the specified origin within the ActiveWorkplane - or the world if none is set.
        /// </summary>
        /// <param name="origin">Origin about which to rotate the line.</param>
        /// <param name="angle">Number of radians to rotate.</param>
        public void RotateAboutZAxis(Point origin, Radian angle)
        {
            foreach (Point objPoint in this)
            {
                objPoint.RotateAboutZAxis(origin, angle);
            }
        }

        /// <summary>
        /// Rebases the polyline from the world to the specified workplane.
        /// </summary>
        /// <param name="toWorkplane">Destination workplane.</param>
        public void RebaseToWorkplane(Workplane toWorkplane)
        {
            foreach (Point objPoint in this)
            {
                objPoint.RebaseToWorkplane(toWorkplane);
            }
        }

        /// <summary>
        /// Rebases the polyline from the specified workplane to the world.
        /// </summary>
        /// <param name="fromWorkplane">Workplane from which the line is to be rebased.</param>
        public void RebaseFromWorkplane(Workplane fromWorkplane)
        {
            foreach (Point objPoint in this)
            {
                objPoint.RebaseFromWorkplane(fromWorkplane);
            }
        }

        #endregion

        #region "Clone Operation"

        /// <summary>
        /// Returns a clone of this Polyline.
        /// </summary>
        public Polyline Clone()
        {
            Polyline clonePolyline = new Polyline();
            foreach (Point point in this)
            {
                clonePolyline.Add(point.Clone());
            }
            return clonePolyline;
        }

        #endregion
    }
}