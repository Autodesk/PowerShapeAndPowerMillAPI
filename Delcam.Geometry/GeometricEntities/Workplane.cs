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
using System.IO;
using System.Linq;
using Autodesk.Geometry.Euler;
using Autodesk.Matrix;
using File = Autodesk.FileSystem.File;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates a Workplane. A workplane consists of three vectors that describe the orientation of a
    /// model in respect to the world.
    /// </summary>
    [Serializable]
    public class Workplane
    {
        #region "Fields"

        /// <summary>
        /// Origin of the Workplane.
        /// </summary>
        private Point _origin;

        /// <summary>
        /// This is the X Axis of the Workplane
        /// </summary>
        private Vector _xAxis;

        /// <summary>
        /// This is the Y Axis of the Workplane
        /// </summary>
        private Vector _yAxis;

        /// <summary>
        /// This is the Z Axis of the Workplane
        /// </summary>
        private Vector _zAxis;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructor a workplane to match the orientation of the world.
        /// </summary>
        public Workplane()
        {
            _origin = new Point(0.0, 0.0, 0.0);

            // Create three unit vectors that match the orientation of the world
            _xAxis = new Vector(1.0, 0.0, 0.0);
            _yAxis = new Vector(0.0, 1.0, 0.0);
            _zAxis = new Vector(0.0, 0.0, 1.0);
        }

        /// <summary>
        /// Constructs a workplane with orientation relative to the world using the specified values.
        /// </summary>
        /// <param name="origin">Origin of the coordinate system.</param>
        /// <param name="xAxis">Vector representing the X axis.</param>
        /// <param name="yAxis">Vector representing the Y axis.</param>
        /// <param name="zAxis">Vector representing the Z axis.</param>
        public Workplane(Point origin, Vector xAxis, Vector yAxis, Vector zAxis)
        {
            CheckIfVectorsArePerpendicular(xAxis, yAxis);
            CheckIfVectorsArePerpendicular(xAxis, zAxis);
            CheckIfVectorsArePerpendicular(yAxis, zAxis);

            _origin = origin;

            _xAxis = xAxis;
            _yAxis = yAxis;
            _zAxis = zAxis;
        }

        /// <summary>
        /// Constructs workplane from two vectors and point
        /// </summary>
        /// <param name="origin">Origin of the coordinate system.</param>
        /// <param name="xAxis">Vector representing the X axis.</param>
        /// <param name="yAxis">Vector representing the Y axis.</param>
        public Workplane(Point origin, Vector xAxis, Vector yAxis)
        {
            CheckIfVectorsArePerpendicular(xAxis, yAxis);

            _origin = origin;

            _xAxis = xAxis;
            _yAxis = yAxis;
            _zAxis = Vector.CrossProduct(xAxis, yAxis);
        }

        /// <summary>
        /// Check if vectors are roughly perpendicular. Throw exception if they are not.
        /// </summary>
        /// <param name="xAxis">Vector representing the first axis.</param>
        /// <param name="yAxis">Vector representing the second axis.</param>
        private void CheckIfVectorsArePerpendicular(Vector xAxis, Vector yAxis)
        {
            if (Math.Abs(Vector.DotProduct(xAxis, yAxis)) > 0.0001)
            {
                throw new ArgumentException("Vectors are not perpendicular");
            }
        }

        /// <summary>
        /// Creates workplane from MAT, a TRX or a MATRIX file.
        /// </summary>
        /// <param name="file">The file to create the workplane from.</param>
        public Workplane(File file)
        {
            Tuple<double[,], double[]> rotationMatrixAndTranslation;
            if (IsAMatFile(file))
            {
                rotationMatrixAndTranslation = GetRotationMatrixAndTranslationFromMatFile(file);
            }
            else if (IsATrxFile(file))
            {
                rotationMatrixAndTranslation = GetRotationMatrixAndTranslationFromTrxFile(file);
            }
            else if (IsAMatrixFile(file))
            {
                rotationMatrixAndTranslation = GetRotationMatrixAndTranslationFromMatrixFile(file);
            }
            else
            {
                throw new ArgumentException("File type is invalid, a MAT file, a TRX file or a MATRIX file is expected.");
            }

            // Get the origin of the workplane
            _origin = new Point(rotationMatrixAndTranslation.Item2[0],
                                rotationMatrixAndTranslation.Item2[1],
                                rotationMatrixAndTranslation.Item2[2]);

            // Get the x, y and z axis of the workplane
            _xAxis = new Vector(rotationMatrixAndTranslation.Item1[0, 0],
                                rotationMatrixAndTranslation.Item1[1, 0],
                                rotationMatrixAndTranslation.Item1[2, 0]);
            _yAxis = new Vector(rotationMatrixAndTranslation.Item1[0, 1],
                                rotationMatrixAndTranslation.Item1[1, 1],
                                rotationMatrixAndTranslation.Item1[2, 1]);
            _zAxis = new Vector(rotationMatrixAndTranslation.Item1[0, 2],
                                rotationMatrixAndTranslation.Item1[1, 2],
                                rotationMatrixAndTranslation.Item1[2, 2]);
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Workplane origin.
        /// </summary>
        public Point Origin
        {
            get { return _origin; }

            set { _origin = value; }
        }

        /// <summary>
        /// Vector representing the workplane in X.
        /// </summary>
        public Vector XAxis
        {
            get { return _xAxis; }

            set { _xAxis = value; }
        }

        /// <summary>
        /// Vector representing the workplane in Y.
        /// </summary>
        public Vector YAxis
        {
            get { return _yAxis; }

            set { _yAxis = value; }
        }

        /// <summary>
        /// Vector representing the workplane in Z.
        /// </summary>
        public Vector ZAxis
        {
            get { return _zAxis; }

            set { _zAxis = value; }
        }

        #endregion

        #region "Transform Operations"

        /// <summary>
        /// Moves the workplane by the specified Vector.
        /// </summary>
        /// <param name="offset">Vector by which to offset workplane.</param>
        public void Move(Vector offset)
        {
            _origin += offset;
        }

        /// <summary>
        /// Rotates workplane by the specified angle about the X axis and the specified origin.
        /// </summary>
        /// <param name="origin">Origin of rotation.</param>
        /// <param name="angle">Angle through which to rotate.</param>
        public void RotateAboutXAxis(Point origin, Radian angle)
        {
            // Rotate the origin
            _origin.RotateAboutXAxis(origin, angle);

            // Rotate the Axes
            _xAxis.RotateAboutXAxis(angle);
            _yAxis.RotateAboutXAxis(angle);
            _zAxis.RotateAboutXAxis(angle);
        }

        /// <summary>
        /// Rotates workplane by the specified angle about the Y axis and the specified origin.
        /// </summary>
        /// <param name="origin">Origin of rotation.</param>
        /// <param name="angle">Angle through which to rotate.</param>
        public void RotateAboutYAxis(Point origin, Radian angle)
        {
            // Rotate the origin
            _origin.RotateAboutYAxis(origin, angle);

            // Rotate the Axes
            _xAxis.RotateAboutYAxis(angle);
            _yAxis.RotateAboutYAxis(angle);
            _zAxis.RotateAboutYAxis(angle);
        }

        /// <summary>
        /// Rotates workplane by the specified angle about the Z axis and the specified origin.
        /// </summary>
        /// <param name="origin">Origin of rotation.</param>
        /// <param name="angle">Angle through which to rotate.</param>
        public void RotateAboutZAxis(Point origin, Radian angle)
        {
            // Rotate the origin
            _origin.RotateAboutZAxis(origin, angle);

            // Rotate the Axes
            _xAxis.RotateAboutZAxis(angle);
            _yAxis.RotateAboutZAxis(angle);
            _zAxis.RotateAboutZAxis(angle);
        }

        /// <summary>
        /// Gets Transformation Matrix from workplane.
        /// </summary>
        /// <returns>Returns the Transformation Matrix.</returns>
        public double[,] GetTransformationMatrix()
        {
            var rotationAndTranslation = GetRotationMatrixAndTranslationFromWorkplaneAxes();
            double[,] transformatioMatrix = Matrix.Matrix.Identity(4);

            var rows = rotationAndTranslation.Item1.GetLength(0);
            var columns = rotationAndTranslation.Item1.GetLength(1);

            // Copy rotation matrix
            for (int i = 0; i <= columns - 1; i++)
            {
                for (int j = 0; j <= rows - 1; j++)
                {
                    transformatioMatrix[i, j] = rotationAndTranslation.Item1[i, j];
                }
            }

            // Copy translation vector
            transformatioMatrix[0, 3] = rotationAndTranslation.Item2[0];
            transformatioMatrix[1, 3] = rotationAndTranslation.Item2[1];
            transformatioMatrix[2, 3] = rotationAndTranslation.Item2[2];

            return transformatioMatrix;
        }

        #endregion

        #region " Copy Operation "

        /// <summary>
        /// Returns a clone of this Workplane.
        /// </summary>
        public Workplane Clone()
        {
            Workplane cloneWorkplane = new Workplane(_origin.Clone(), XAxis.Clone(), YAxis.Clone(), ZAxis.Clone());
            return cloneWorkplane;
        }

        #endregion

        #region "Export Operations"

        /// <summary>
        /// Writes workplane to MAT file.
        /// </summary>
        /// <param name="matFile">MAT file path.</param>
        public void WriteToMat(File matFile)
        {
            var rotationAndTranslation = GetRotationMatrixAndTranslationFromWorkplaneAxes();
            WriteRotationMatrixAndTranslationToMatFile(matFile, rotationAndTranslation.Item1, rotationAndTranslation.Item2);
        }

        /// <summary>
        /// Writes workplane to TRX file.
        /// </summary>
        /// <param name="trxFile">TRX file path.</param>
        public void WriteToTrx(File trxFile)
        {
            var rotationAndTranslation = GetRotationMatrixAndTranslationFromWorkplaneAxes();
            WriteRotationMatrixAndTranslationToTrxFile(trxFile, rotationAndTranslation.Item1, rotationAndTranslation.Item2);
        }

        /// <summary>
        /// Writes workplane to a MATRIX file.
        /// </summary>
        /// <param name="matrixFile">The file path to the '.matrix' file.</param>
        public void WriteToMatrix(File matrixFile)
        {
            matrixFile.Delete();

            // The expected format of the transformation matrix is the following:
            //xAxis.i|yAxis.i|zAxis.i|x
            //xAxis.j|yAxis.j|zAxis.j|y
            //xAxis.k|yAxis.k|zAxis.k|z
            //0|0|0|1

            // The 1st column is the new x vector relative to World
            // The 2nd column is the new y vector relative to World
            // The 3rd column is the new z vector relative to World
            // The 4th column is the new origin position relative to World

            string firstTranformationMatrixLine = $"{XAxis.I}|{YAxis.I}|{ZAxis.I}|{Origin.X}";
            string secondTranformationMatrixLine = $"{XAxis.J}|{YAxis.J}|{ZAxis.J}|{Origin.Y}";
            string thirdTranformationMatrixLine = $"{XAxis.K}|{YAxis.K}|{ZAxis.K}|{Origin.Z}";
            string fourthTranformationMatrixLine = "0|0|0|1";

            var lines = new List<string>(6);
            lines.Add(firstTranformationMatrixLine);
            lines.Add(secondTranformationMatrixLine);
            lines.Add(thirdTranformationMatrixLine);
            lines.Add(fourthTranformationMatrixLine);

            matrixFile.WriteTextLines(lines);
        }

        #endregion

        #region "Implementation"

        /// <summary>
        /// Creates a transformation matrix from a MAT file.
        /// </summary>
        /// <param name="matFile">The MAT file.</param>
        /// <returns>The transformation matrix.</returns>
        private Tuple<double[,], double[]> GetRotationMatrixAndTranslationFromMatFile(File matFile)
        {
            StreamReader myFile = new StreamReader(matFile.Path);
            double xRotation = 0;
            double yRotation = 0;
            double zRotation = 0;
            var translation = new double[3];
            bool invert = false;

            double rotationInDegrees;

            while (myFile.EndOfStream == false)
            {
                string myLine = myFile.ReadLine();
                string[] splitLine = myLine.Split();
                var lineValues = splitLine.Where(x => !string.IsNullOrEmpty(x)).ToList();

                // PowerInspect writes the MAT files using InvariantCulture

                if (lineValues.Any())
                {
                    switch (lineValues[0])
                    {
                        case "R":
                            switch (lineValues[1])
                            {
                                case "X":
                                    rotationInDegrees = Convert.ToDouble(lineValues[2], CultureInfo.InvariantCulture);
                                    xRotation = rotationInDegrees * Math.PI / 180;

                                    break;
                                case "Y":
                                    rotationInDegrees = Convert.ToDouble(lineValues[2], CultureInfo.InvariantCulture);
                                    yRotation = rotationInDegrees * Math.PI / 180;

                                    break;
                                case "Z":
                                    rotationInDegrees = Convert.ToDouble(lineValues[2], CultureInfo.InvariantCulture);
                                    zRotation = rotationInDegrees * Math.PI / 180;

                                    break;
                            }
                            break;
                        case "T":
                            translation[0] = Convert.ToDouble(lineValues[1], CultureInfo.InvariantCulture);
                            translation[1] = Convert.ToDouble(lineValues[2], CultureInfo.InvariantCulture);
                            translation[2] = Convert.ToDouble(lineValues[3], CultureInfo.InvariantCulture);
                            break;
                        case "I":
                            invert = true;

                            break;
                    }
                }
            }
            myFile.Close();

            // PowerInspect uses the convention XYZ extrinsic (which means 1st rotate around x, then y, then z regarding to the world)
            double[,] rotationMatrix = new Angles(xRotation, yRotation, zRotation, Conventions.XYZ).Matrix;

            if (invert)
            {
                //Invert the Rotation matrix and invert the Translation vector.
                //This is equivalent to transpose the Rotation Matrix(Rt) and for the translation vector is equivalent to multiply it by -Rt
                //(http://www.cg.info.hiroshima-cu.ac.jp/~miyazaki/knowledge/teche53.html)
                rotationMatrix = rotationMatrix.Transpose();
                var negativeRotationMatrix = rotationMatrix.Multiply(-1);
                translation = negativeRotationMatrix.Multiply(translation);

                translation[0] = translation[0];
                translation[1] = translation[1];
                translation[2] = translation[2];
            }

            return new Tuple<double[,], double[]>(rotationMatrix, translation);
        }

        /// <summary>
        /// Creates a transformation matrix from a TRX file.
        /// </summary>
        /// <param name="trxFile">The TRX file.</param>
        /// <returns>The transformation matrix.</returns>
        private Tuple<double[,], double[]> GetRotationMatrixAndTranslationFromTrxFile(File trxFile)
        {
            var lines = trxFile.ReadTextLines();

            double xRotation = 0;
            double yRotation = 0;
            double zRotation = 0;
            double xTranslation = 0;
            double yTranslation = 0;
            double zTranslation = 0;

            foreach (var line in lines)
            {
                var lineInvariant = line.ToLowerInvariant();
                string[] lineSplit = lineInvariant.Split(':');
                double lineValue;
                try
                {
                    lineValue = Convert.ToDouble(lineSplit.Last());
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        String.Format("Invalid TRX file. No numeric value found in this TRX line: {0}", line),
                        ex);
                    throw;
                }

                var lineIdentifier = lineSplit.First();
                if (lineIdentifier.Contains("rot_x"))
                {
                    xRotation = lineValue;
                }
                else if (lineIdentifier.Contains("rot_y"))
                {
                    yRotation = lineValue;
                }
                else if (lineIdentifier.Contains("rot_z"))
                {
                    zRotation = lineValue;
                }
                else if (lineIdentifier.Contains("shift_x"))
                {
                    xTranslation = lineValue;
                }
                else if (lineIdentifier.Contains("shift_y"))
                {
                    yTranslation = lineValue;
                }
                else if (lineIdentifier.Contains("shift_z"))
                {
                    zTranslation = lineValue;
                }
                else
                {
                    throw new ArgumentException(
                        String.Format("Invalid TRX File. TRX line should be rotation or shift in X,Y or Z: {0}", line));
                }
            }

            double[,] rotationmatrix = new Angles(xRotation * Math.PI / 180.0,
                                                  yRotation * Math.PI / 180.0,
                                                  zRotation * Math.PI / 180.0,
                                                  Conventions.XYZ).Matrix;
            double[] translation = {xTranslation, yTranslation, zTranslation};
            return new Tuple<double[,], double[]>(rotationmatrix, translation);
        }

        /// <summary>
        /// Gets rotation and transformation matrix from a MATRIX file.
        /// </summary>
        /// <param name="matrixFile">The MATRIX file.</param>
        /// <returns>The transformation matrix.</returns>
        private Tuple<double[,], double[]> GetRotationMatrixAndTranslationFromMatrixFile(File matrixFile)
        {
            int NUMBER_OF_ROWS_IN_TRANSFORMATION_MATRIX = 4;
            int NUMBER_OF_COLUMNS_IN_TRANSFORMATION_MATRIX = 4;
            int NUMBER_OF_ROWS_IN_ROTATION_MATRIX = 3;
            int NUMBER_OF_COLUMNS_IN_ROTATION_MATRIX = 3;
            int NUMBER_OF_ELEMENTS_IN_TRANSLATION_VECTOR = 3;

            var rows = matrixFile.ReadTextLines();

            if (rows.Count() != NUMBER_OF_ROWS_IN_TRANSFORMATION_MATRIX)
            {
                throw new ArgumentException(
                    $"Failed to load 'matrix' file. The file should have {NUMBER_OF_ROWS_IN_TRANSFORMATION_MATRIX} lines. The first line should be xAxis.i|yAxis.i|zAxis.i|x. The second line should be xAxis.j|yAxis.j|zAxis.j|y. The third line should be xAxis.k|yAxis.k|zAxis.k|z");
            }

            double[,] rotationMatrix = new double[NUMBER_OF_ROWS_IN_ROTATION_MATRIX, NUMBER_OF_COLUMNS_IN_ROTATION_MATRIX];
            double[] translation = new double[NUMBER_OF_ELEMENTS_IN_TRANSLATION_VECTOR];

            // The expected format of the transformation matrix is the following:
            //xAxis.i|yAxis.i|zAxis.i|x
            //xAxis.j|yAxis.j|zAxis.j|y
            //xAxis.k|yAxis.k|zAxis.k|z
            //0|0|0|1

            // The 1st column is the new x vector relative to World
            // The 2nd column is the new y vector relative to World
            // The 3rd column is the new z vector relative to World
            // The 4th column is the new origin position relative to World
            for (int l = 0; l < NUMBER_OF_ROWS_IN_ROTATION_MATRIX; l++)
            {
                var rowInvariant = rows[l].ToLowerInvariant();
                var rowItems = rowInvariant.Split('|');

                if (rowItems.Count() != NUMBER_OF_COLUMNS_IN_TRANSFORMATION_MATRIX)
                {
                    throw new ArgumentException(
                        $"Failed to load 'matrix' file. The file should have per line {NUMBER_OF_COLUMNS_IN_TRANSFORMATION_MATRIX} numeric values separated by '|'.");
                }

                for (int c = 0; c < NUMBER_OF_COLUMNS_IN_TRANSFORMATION_MATRIX; c++)
                {
                    double rowValue;
                    try
                    {
                        rowValue = Convert.ToDouble(rowItems[c]);
                        if (c == 3)
                        {
                            // In the 4th column are the translation values
                            translation[l] = rowValue;
                        }
                        else
                        {
                            rotationMatrix[l, c] = rowValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException($"Invalid MATRIX file. No numeric value found in : {rowItems[c]}", ex);
                    }
                }
            }

            return new Tuple<double[,], double[]>(rotationMatrix, translation);
        }

        private Tuple<double[,], double[]> GetRotationMatrixAndTranslationFromWorkplaneAxes()
        {
            // Start with the identity matrix
            double[,] rotationMatrix = Matrix.Matrix.Identity(3);
            var translation = new double[3];

            // Fill the x-axis in the Transformation Matrix
            rotationMatrix[0, 0] = _xAxis.I;
            rotationMatrix[1, 0] = _xAxis.J;
            rotationMatrix[2, 0] = _xAxis.K;

            // Fill the y-axis in the Transformation Matrix
            rotationMatrix[0, 1] = _yAxis.I;
            rotationMatrix[1, 1] = _yAxis.J;
            rotationMatrix[2, 1] = _yAxis.K;

            // Fill the z-axis in the Transformation Matrix
            rotationMatrix[0, 2] = _zAxis.I;
            rotationMatrix[1, 2] = _zAxis.J;
            rotationMatrix[2, 2] = _zAxis.K;

            // Fill the translation in the Transformation Matrix
            translation[0] = _origin.X;
            translation[1] = _origin.Y;
            translation[2] = _origin.Z;

            return new Tuple<double[,], double[]>(rotationMatrix, translation);
        }

        /// <summary>
        /// Writes the transformation matrix to a mat file.
        /// </summary>
        /// <param name="matFile">The MAT file to write.</param>
        /// <param name="rotationMatrix">The rotation matrix.</param>
        /// <param name="translation">The translation matrix.</param>
        private void WriteRotationMatrixAndTranslationToMatFile(File matFile, double[,] rotationMatrix, double[] translation)
        {
            // Delete the file if a previous one exists
            if (System.IO.File.Exists(matFile.Path))
            {
                System.IO.File.Delete(matFile.Path);
            }

            string xLine = null;
            string yLine = null;
            var zLine = "";

            // PowerInspect writes the MAT files using Invariant culture.
            string transLine = "   T   " + Convert.ToString(translation[0], CultureInfo.InvariantCulture) + " " +
                               Convert.ToString(translation[1], CultureInfo.InvariantCulture) + " " +
                               Convert.ToString(translation[2], CultureInfo.InvariantCulture);

            var rotations = Angles.GetRotationsFromMatrixXYZ(rotationMatrix);

            zLine = "   R Z " + Convert.ToString(rotations[0].Value, CultureInfo.InvariantCulture);
            yLine = "   R Y " + Convert.ToString(rotations[1].Value, CultureInfo.InvariantCulture);
            xLine = "   R X " + Convert.ToString(rotations[2].Value, CultureInfo.InvariantCulture);
            var lines = new List<string>();
            lines.Add(xLine);
            lines.Add(yLine);
            lines.Add(zLine);
            lines.Add(transLine);
            lines.Add("*");
            lines.Add("");

            // create a new text document
            System.IO.File.WriteAllLines(matFile.Path, lines);
        }

        /// <summary>
        /// Writes the transformation matrix to a TRX file.
        /// </summary>
        /// <param name="trxFile">The TRX file to write.</param>
        /// <param name="rotationMatrix">The rotation matrix.</param>
        /// <param name="translation">The translation matrix.</param>
        private void WriteRotationMatrixAndTranslationToTrxFile(
            File trxFile,
            double[,] rotationMatrix,
            double[] translation)
        {
            trxFile.Delete();

            Degree[] rotations = Angles.GetRotationsFromMatrixXYZ(rotationMatrix);
            string xRotationLine = "ROT_X: " +
                                   (rotations[2].Value >= 0.0
                                       ? rotations[2].Value.ToString("+0.000")
                                       : rotations[2].Value.ToString("0.000"));
            string yRotationLine = "ROT_Y: " +
                                   (rotations[1].Value >= 0.0
                                       ? rotations[1].Value.ToString("+0.000")
                                       : rotations[1].Value.ToString("0.000"));
            string zRotationLine = "ROT_Z: " +
                                   (rotations[0].Value >= 0.0
                                       ? rotations[0].Value.ToString("+0.000")
                                       : rotations[0].Value.ToString("0.000"));
            string xTranslationLine = "SHIFT_X: " +
                                      (translation[0] >= 0.0
                                          ? translation[0].ToString("+0.000")
                                          : translation[0].ToString("0.000"));
            string yTranslationLine = "SHIFT_Y: " +
                                      (translation[1] >= 0.0
                                          ? translation[1].ToString("+0.000")
                                          : translation[1].ToString("0.000"));
            string zTranslationLine = "SHIFT_Z: " +
                                      (translation[0] >= 0.0
                                          ? translation[2].ToString("+0.000")
                                          : translation[2].ToString("0.000"));

            var lines = new List<string>(6);
            lines.Add(xRotationLine);
            lines.Add(yRotationLine);
            lines.Add(zRotationLine);
            lines.Add(xTranslationLine);
            lines.Add(yTranslationLine);
            lines.Add(zTranslationLine);

            trxFile.WriteTextLines(lines);
        }

        /// <summary>
        /// Checks if it is a MAT file.
        /// </summary>
        /// <param name="file">The file to be tested.</param>
        /// <returns>True if it is a MAT file, false otherwise.</returns>
        private bool IsAMatFile(File file)
        {
            return file.Extension.ToLowerInvariant() == "mat";
        }

        /// <summary>
        /// Checks if it is a TRX file.
        /// </summary>
        /// <param name="file">The file to be tested.</param>
        /// <returns>True if it is a TRX file, false otherwise.</returns>
        private bool IsATrxFile(File file)
        {
            return file.Extension.ToLowerInvariant() == "trx";
        }

        /// <summary>
        /// Checks if it is a Matrix file. A file ending with '.matrix' is a custom file format created to be able to load a workplane from a file describing a transformation matrix.
        /// </summary>
        /// <param name="file">File with the transformation matrix.</param>
        /// <returns>True if it is a Matrix file, false otherwise.</returns>
        private bool IsAMatrixFile(File file)
        {
            return file.Extension.ToLowerInvariant() == "matrix";
        }

        #endregion
    }
}