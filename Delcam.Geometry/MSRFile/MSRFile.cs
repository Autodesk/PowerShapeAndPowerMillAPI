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
using System.Text;
using System.Threading;
using Autodesk.FileSystem;
using Autodesk.Geometry.Euler;
using Microsoft.VisualBasic;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Performs read and write operations on MSR files.
    /// </summary>
    [Serializable]
    public class MSRFile : File
    {
        //TODO: SHould this inherit from file?

        #region " Point Data "

        /// <summary>
        /// Encapsulates data for each point.
        /// </summary>
        [Serializable]
        public class PointData
        {
            #region " Fields "

            private int _pointNumber;
            private Point _nominalSurfacePoint;
            private Vector _surfaceNormal;
            private double _surfaceOffset;
            private double _upperTolerance;
            private double _lowerTolerance;
            private Point _probeCentre;
            private MM _probeRadius;

            private Point _measuredSurfacePoint;

            #endregion

            #region " Constructors "

            /// <summary>
            /// Constructs a PointData object with default values.
            /// </summary>
            public PointData()
            {
                _nominalSurfacePoint = new Point();
                _surfaceNormal = new Vector();
                _surfaceOffset = 0.0;
                _upperTolerance = 0.0;
                _lowerTolerance = 0.0;
                _probeCentre = new Point();
                _probeRadius = 0.0;

                _measuredSurfacePoint = null;
            }

            /// <summary>
            /// Constructs a PointData object with the specified values.
            /// </summary>
            /// <param name="nominalSurfacePoint">Nominal point on the surface to be probed.</param>
            /// <param name="surfaceNormal">Vector normal to the surface at nominalSurfacePoint.</param>
            /// <param name="surfaceOffset">Expected offset of the point from the ideal.</param>
            /// <param name="upperTolerance">Upper permissible offset tolerance.</param>
            /// <param name="lowerTolerance">Lower permissible offset tolerance.</param>
            /// <param name="weight">Relative importance of this point in the model.</param>
            /// <param name="probeCentre">Probe centre on measurement.</param>
            /// <param name="probeRadius">Probe radius in millimetres.</param>
            /// <remarks></remarks>
            public PointData(
                Point nominalSurfacePoint,
                Vector surfaceNormal,
                double surfaceOffset,
                double upperTolerance,
                double lowerTolerance,
                double weight,
                Point probeCentre,
                MM probeRadius)
            {
                _nominalSurfacePoint = nominalSurfacePoint;
                _surfaceNormal = surfaceNormal;
                _surfaceOffset = surfaceOffset;
                _upperTolerance = upperTolerance;
                _lowerTolerance = lowerTolerance;
                _probeCentre = probeCentre;
                _probeRadius = probeRadius;

                //Calculate the surface point
                CalculateMeasuredSurfacePoint();
            }

            #endregion

            #region " Operations "

            /// <summary>
            /// This operation applies the specified EulerAngles to the PointData
            /// </summary>
            internal void ApplyMatrix(Angles objMatrix)
            {
                _nominalSurfacePoint.EulerRotation(objMatrix);
                _surfaceNormal.EulerRotation(objMatrix);
                _probeCentre.EulerRotation(objMatrix);
            }

            /// <summary>
            /// This operation calculates the MeasuredSurfacePoint from the probe data.  Before running this
            /// operation the data should be rotated by the EulerAngles if appropriate
            /// </summary>
            internal void CalculateMeasuredSurfacePoint()
            {
                _measuredSurfacePoint = new Point(_probeCentre.X - _surfaceNormal.I * _probeRadius,
                                                  _probeCentre.Y - _surfaceNormal.J * _probeRadius,
                                                  _probeCentre.Z - _surfaceNormal.K * _probeRadius);
            }

            #endregion

            #region " Properties "

            /// <summary>
            /// Number of the point in the file.
            /// </summary>
            public int PointNumber
            {
                get { return _pointNumber; }
                set { _pointNumber = value; }
            }

            /// <summary>
            /// Nominal position of the PointData.
            /// </summary>
            public Point NominalSurfacePoint
            {
                get { return _nominalSurfacePoint; }
                set { _nominalSurfacePoint = value; }
            }

            /// <summary>
            /// Normal to the surface at the nominal point.
            /// </summary>
            public Vector SurfaceNormal
            {
                get { return _surfaceNormal; }
                set { _surfaceNormal = value; }
            }

            /// <summary>
            /// Expected manufactured offset from the nominal point.
            /// </summary>
            public double SurfaceOffset
            {
                get { return _surfaceOffset; }
                set { _surfaceOffset = value; }
            }

            /// <summary>
            /// Upper permissible tolerance.
            /// </summary>
            public double UpperTolerance
            {
                get { return _upperTolerance; }
                set { _upperTolerance = value; }
            }

            /// <summary>
            /// Lower permissible tolerance.
            /// </summary>
            public double LowerTolerance
            {
                get { return _lowerTolerance; }
                set { _lowerTolerance = value; }
            }

            /// <summary>
            /// Probe centre.
            /// </summary>
            public Point ProbeCentre
            {
                get { return _probeCentre; }
                set { _probeCentre = value; }
            }

            /// <summary>
            /// Probe radius.
            /// </summary>
            public MM ProbeRadius
            {
                get { return _probeRadius; }
                set { _probeRadius = value; }
            }

            /// <summary>
            /// Position of point as probed.
            /// </summary>
            public Point MeasuredSurfacePoint
            {
                get
                {
                    if (_measuredSurfacePoint == null)
                    {
                        CalculateMeasuredSurfacePoint();
                    }
                    return _measuredSurfacePoint;
                }
            }

            /// <summary>
            /// Distance between measured surface point and nominal surface point. A more accurate value is provided by the VariationProjectedToSurfaceNormal property.
            /// </summary>
            public MM Variation
            {
                get { return (MeasuredSurfacePoint - NominalSurfacePoint).Magnitude; }
            }

            /// <summary>
            /// Distance between measured surface point and nominal surface point projected onto the surface normal to give the variation and ignoring cosine error.
            /// </summary>
            public MM VariationProjectedToSurfaceNormal
            {
                get { return Vector.DotProduct(MeasuredSurfacePoint - NominalSurfacePoint, SurfaceNormal); }
            }

            #endregion
        }

        #endregion

        #region " Fields "

        private List<PointData> _points;

        private List<string> _headerLines;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Constructs an MSRFile object and populates it with points from the specified file.
        /// </summary>
        /// <param name="filePath">Path to the .msr file.</param>
        /// <param name="anglesInDegrees">Specifies whether the angles in a G330 line are in degrees (default) or radians</param>
        /// .
        public MSRFile(string filePath, bool anglesInDegrees = true) : base(filePath)
        {
            // Bespoke work #121.
            // File parsing now culture independent (MXA).
            // Prior to processing MSR file, change current culture to invariant to cope with culture specific disparities.
            // Store current culture so it can be reinstated.
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                // Read in the file
                ReadFile(anglesInDegrees);
            }
            catch (Exception ex)
            {
                throw ex;

                //We don't process it here, but include the try block to ensure finally statement and reinstatement of culture.
            }
            finally
            {
                // Always swap the original culture back!
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Constructs a new file with the specified path and filename and copies
        /// to it the specified list of points.
        /// </summary>
        /// <param name="filePathToCreate">Path to the new file.</param>
        /// <param name="pointData">List of points to be included in the file.</param>
        public MSRFile(string filePathToCreate, List<PointData> pointData) : base(filePathToCreate)
        {
            // Get points
            _points = pointData;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Reads point data from the specified file and stores it in the object.
        /// </summary>
        /// <param name="anglesInDegrees">Specifies whether the angles in a G330 line are in degrees or radians (default).</param>
        /// <exception cref="Exception">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// Multiple control characters found for a single point.<br></br>Point has no corresponding G801 line in MSR file.<br></br>
        /// Missing G800 line in MSR file after parsing points.<br></br>No points found in MSR file.
        /// </exception>
        private void ReadFile(bool anglesInDegrees)
        {
            //Read the lines in from the file
            List<string> lines = null;
            lines = ReadTextLines();

            Angles matrix = null;

            _points = new List<PointData>();
            _headerLines = new List<string>();

            bool isStillInHeader = true;
            bool endHit = false;

            for (int lineIndex = 0; lineIndex <= lines.Count - 1; lineIndex++)
            {
                string line = null;
                line = lines[lineIndex].Trim();

                PointData point = null;

                // If the line starts with START then we must not have read any previous starts
                if (line.StartsWith("START") && _points.Count > 0)
                {
                    throw new Exception(string.Format("MSR File '{0}' contains a start line after the first point data.", Path));
                }
                // Log if we hit an end line
                if (line.StartsWith("END"))
                {
                    endHit = true;
                }
                else if (line.StartsWith("G330"))
                {
                    // Check we aren't receiving this after hitting END
                    if (endHit)
                    {
                        throw new Exception(string.Format("MSR File '{0}' contains point data after and END line.", Path));
                    }

                    // G330 line specifies the orientation of the machine for and following points
                    isStillInHeader = false;

                    //Read in the euler angles
                    Radian a = default(Radian);
                    Radian b = default(Radian);
                    Radian c = default(Radian);

                    string characters = "";
                    char lastControlChar = '~';
                    for (int charIndex = 4; charIndex <= line.Length - 1; charIndex++)
                    {
                        if (IsControlChar(line[charIndex]))
                        {
                            switch (lastControlChar)
                            {
                                case 'A':
                                    if (anglesInDegrees)
                                    {
                                        a = (Degree) Convert.ToDouble(characters);
                                    }
                                    else
                                    {
                                        a = Convert.ToDouble(characters);
                                    }
                                    break;
                                case 'B':
                                    if (anglesInDegrees)
                                    {
                                        b = (Degree) Convert.ToDouble(characters);
                                    }
                                    else
                                    {
                                        b = Convert.ToDouble(characters);
                                    }
                                    break;
                                case 'C':
                                    if (anglesInDegrees)
                                    {
                                        c = (Degree) Convert.ToDouble(characters);
                                    }
                                    else
                                    {
                                        c = Convert.ToDouble(characters);
                                    }
                                    break;
                            }
                            lastControlChar = line[charIndex];
                            characters = "";
                        }
                        else if (line[charIndex] != ' ')
                        {
                            characters += line[charIndex];
                        }
                    }
                    matrix = new Angles(a, b, c, Conventions.XYZ);
                }
                else if (line.StartsWith("G800"))
                {
                    // Check we aren't receiving this after hitting END
                    if (endHit)
                    {
                        throw new Exception(string.Format("MSR File '{0}' contains point data after and END line.", Path));
                    }

                    // G800 line is the nominal point and is followed by the G801 line which is the measured point
                    isStillInHeader = false;

                    //Parse the characters in the line
                    point = new PointData();

                    string characters = "";
                    char lastControlChar = '~';
                    var controlCharactersFound = new List<string>();
                    for (int charIndex = 4; charIndex <= line.Length - 1; charIndex++)
                    {
                        if (IsControlChar(line[charIndex]))
                        {
                            if (controlCharactersFound.Contains(line[charIndex].ToString()))
                            {
                                throw new Exception(
                                    string.Format(
                                        "Control character '{0}' found multiple times for point number {1} in MSR File '{2}'",
                                        line[charIndex],
                                        _points.Count + 1,
                                        Path));
                            }
                            controlCharactersFound.Add(line[charIndex].ToString());
                            StoreValue800(ref point, lastControlChar, characters);
                            lastControlChar = line[charIndex];
                            characters = "";
                        }
                        else if (line[charIndex] != ' ')
                        {
                            characters += line[charIndex];
                        }
                    }
                    StoreValue800(ref point, lastControlChar, characters);

                    line = lines[lineIndex + 1].Trim();
                    if (line.StartsWith("G801"))
                    {
                        characters = "";
                        lastControlChar = '~';
                        controlCharactersFound = new List<string>();
                        for (int intChar = 4; intChar <= line.Length - 1; intChar++)
                        {
                            if (IsControlChar(line[intChar]))
                            {
                                if (controlCharactersFound.Contains(line[intChar].ToString()))
                                {
                                    throw new Exception(
                                        string.Format(
                                            "Control character '{0}' found multiple times for point number {1} in MSR File '{2}'",
                                            line[intChar],
                                            _points.Count + 1,
                                            Path));
                                }
                                controlCharactersFound.Add(line[intChar].ToString());
                                StoreValue801(ref point, lastControlChar, characters);
                                lastControlChar = line[intChar];
                                characters = "";
                            }
                            else if (line[intChar] != ' ')
                            {
                                characters += line[intChar];
                            }
                        }
                        StoreValue801(ref point, lastControlChar, characters);

                        //Store the point
                        if (matrix != null)
                        {
                            point.ApplyMatrix(matrix);
                        }
                        point.CalculateMeasuredSurfacePoint();
                        _points.Add(point);

                        lineIndex += 1;
                    }
                    else
                    {
                        // Throw an exception as all 800 lines must be followed by an 801 line
                        throw new Exception(string.Format("Point number {0} has no G801 line in MSR File '{1}'",
                                                          point.PointNumber,
                                                          Path));
                    }
                }
                else if (line.StartsWith("G801"))
                {
                    // We have lost a line here so throw an exception
                    throw new Exception(string.Format("Missing G800 line after reading {0} points in MSR File '{1}'",
                                                      _points.Count,
                                                      Path));
                }
                else if (isStillInHeader)
                {
                    _headerLines.Add(lines[lineIndex]);
                }
            }

            // Throw an exception if no points were found in the file
            if (_points.Count == 0)
            {
                throw new Exception(string.Format("No points found in MSR File '{0}'", Path));
            }
        }

        /// <summary>
        /// This operation determines if the character is a control character
        /// </summary>
        private bool IsControlChar(char character)
        {
            switch (character)
            {
                case 'N':
                case 'X':
                case 'Y':
                case 'Z':
                case 'I':
                case 'J':
                case 'K':
                case 'O':
                case 'U':
                case 'L':
                case 'W':
                case 'R':
                case 'A':
                case 'B':
                case 'C':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// This operation stores the G800 line values in the PointData object passed in
        /// </summary>
        private void StoreValue800(ref PointData point, char controlChar, string chars)
        {
            switch (controlChar)
            {
                case 'N':
                    point.PointNumber = Convert.ToInt32(chars);
                    break;
                case 'X':
                    point.NominalSurfacePoint.X = Convert.ToDouble(chars);
                    break;
                case 'Y':
                    point.NominalSurfacePoint.Y = Convert.ToDouble(chars);
                    break;
                case 'Z':
                    point.NominalSurfacePoint.Z = Convert.ToDouble(chars);
                    break;
                case 'I':
                    point.SurfaceNormal.I = Convert.ToDouble(chars);
                    break;
                case 'J':
                    point.SurfaceNormal.J = Convert.ToDouble(chars);
                    break;
                case 'K':
                    point.SurfaceNormal.K = Convert.ToDouble(chars);
                    break;
                case 'O':
                    point.SurfaceOffset = Convert.ToDouble(chars);
                    break;
                case 'U':
                    point.UpperTolerance = Convert.ToDouble(chars);
                    break;
                case 'L':
                    point.LowerTolerance = Convert.ToDouble(chars);
                    break;
            }
        }

        /// <summary>
        /// This operation stores the G801 line values in the PointData object passed in
        /// </summary>
        private void StoreValue801(ref PointData point, char controlChar, string chars)
        {
            switch (controlChar)
            {
                case 'N':
                    int number = Convert.ToInt32(chars);
                    if (number != point.PointNumber)
                    {
                        throw new Exception(
                            string.Format("Point number for G801 does not match G800 for point number {0} in MSR File '{1}'",
                                          point.PointNumber,
                                          Path));
                    }
                    break;
                case 'X':
                    point.ProbeCentre.X = Convert.ToDouble(chars);
                    break;
                case 'Y':
                    point.ProbeCentre.Y = Convert.ToDouble(chars);
                    break;
                case 'Z':
                    point.ProbeCentre.Z = Convert.ToDouble(chars);
                    break;
                case 'R':
                    point.ProbeRadius = Convert.ToDouble(chars);
                    break;
            }
        }

        /// <summary>
        /// Writes detransformed data (see ToString) to the specified file.
        /// </summary>
        /// <param name="filePath">Path and filename of destination file.</param>
        public void WriteFile(string filePath)
        {
            MSRFile fileToSave = new MSRFile(filePath, Points);
            fileToSave.WriteText(ToString());
        }

        /// <summary>
        /// Returns to the caller a string containing the contents of this object.<br></br><br></br> E.g.<br></br><br></br>
        /// START<br></br>
        /// G800 N1 X-86.807 Y-7.998 Z242.112 I0.894 J-0.241 K0.377 O0 U0.1 L-0.1<br></br>
        /// G801 N1 X-85.466 Y-8.36 Z242.678 R1.5
        /// </summary>
        /// <returns>The file's PointData as a string</returns>
        public override string ToString()
        {
            StringBuilder outputString = new StringBuilder();
            outputString.Append("START");
            int numberOfPoints = Points.Count;
            for (int i = 1; i <= numberOfPoints; i++)
            {
                outputString.Append(Constants.vbCrLf + "G800 N" + i);
                outputString.Append(" X" + Math.Round(Points[i - 1].NominalSurfacePoint.X.Value, 4));
                outputString.Append(" Y" + Math.Round(Points[i - 1].NominalSurfacePoint.Y.Value, 4));
                outputString.Append(" Z" + Math.Round(Points[i - 1].NominalSurfacePoint.Z.Value, 4));
                outputString.Append(" I" + Math.Round(Points[i - 1].SurfaceNormal.I.Value, 4));
                outputString.Append(" J" + Math.Round(Points[i - 1].SurfaceNormal.J.Value, 4));
                outputString.Append(" K" + Math.Round(Points[i - 1].SurfaceNormal.K.Value, 4));
                outputString.Append(" O" + Math.Round(Points[i - 1].SurfaceOffset, 4));
                outputString.Append(" U" + Math.Round(Points[i - 1].UpperTolerance, 4));
                outputString.Append(" L" + Math.Round(Points[i - 1].LowerTolerance, 4));

                outputString.Append(Constants.vbCrLf + "G801 N" + i);
                outputString.Append(" X" + Math.Round(Points[i - 1].ProbeCentre.X.Value, 4));
                outputString.Append(" Y" + Math.Round(Points[i - 1].ProbeCentre.Y.Value, 4));
                outputString.Append(" Z" + Math.Round(Points[i - 1].ProbeCentre.Z.Value, 4));
                outputString.Append(" R" + Math.Round(Points[i - 1].ProbeRadius.Value, 4));
            }
            outputString.Append(Constants.vbCrLf + "END");

            return outputString.ToString();
        }

        #region "Operators"

        /// <summary>
        /// Appends two MSR files
        /// </summary>
        /// <param name="firstMSRFile">First MSR file to append.</param>
        /// <param name="secondMSRFile">Second MSR file to append.</param>
        /// <returns>List of the combined pointData.</returns>
        public static List<PointData> operator +(MSRFile firstMSRFile, MSRFile secondMSRFile)
        {
            // Create new list of points from two original lists
            List<PointData> outputPoints = firstMSRFile.Points;
            outputPoints.AddRange(secondMSRFile.Points);

            return outputPoints;
        }

        #endregion

        #endregion

        #region " Properties "

        /// <summary>
        /// File's point data.
        /// </summary>
        public List<PointData> Points
        {
            get { return _points; }
            set { _points = value; }
        }

        /// <summary>
        /// File headers.
        /// </summary>
        public List<string> HeaderLines
        {
            get { return _headerLines; }
            set { _headerLines = value; }
        }

        #endregion
    }
}