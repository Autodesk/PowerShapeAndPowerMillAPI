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
    /// Encapsulates a three dimensional vector and operations thereon.
    /// </summary>
    [Serializable]
    public class Vector
    {
        #region " Fields "

        /// <summary>
        /// Vector i component.
        /// </summary>
        protected MM _i;

        /// <summary>
        /// Vector j component.
        /// </summary>
        protected MM _j;

        /// <summary>
        /// Vector k component.
        /// </summary>
        protected MM _k;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Constructs a null vector.
        /// </summary>
        public Vector()
        {
            _i = 0.0;
            _j = 0.0;
            _k = 0.0;
        }

        /// <summary>
        /// Constructs a vector initialised to the specified magnitudes.
        /// </summary>
        /// <param name="i">Magnitude in i.</param>
        /// <param name="j">Magnitude in j.</param>
        /// <param name="k">Magnitude in k.</param>
        public Vector(MM i, MM j, MM k)
        {
            _i = i;
            _j = j;
            _k = k;
        }

        /// <summary>
        /// Constructs a vector between the specified start and end points.
        /// </summary>
        /// <param name="startPoint">Vector start point.</param>
        /// <param name="endPoint">Vector end point.</param>
        public Vector(Point startPoint, Point endPoint)
        {
            _i = endPoint.X - startPoint.X;
            _j = endPoint.Y - startPoint.Y;
            _k = endPoint.Z - startPoint.Z;
        }

        /// <summary>
        /// Constructor initialises the Vector using an array of MM objects.
        /// </summary>
        /// <param name="componentArray">Array of MM objects from which to create the vector.</param>
        /// <exception cref="Exception">Thrown if input array does not contain three coordinates.</exception>
        public Vector(MM[] componentArray)
        {
            // Check that there are three elements in the array
            if (componentArray.Length != 3)
            {
                throw new Exception("Vector constructor requires 3 coordinates");
            }

            _i = componentArray[0];
            _j = componentArray[1];
            _k = componentArray[2];
        }

        /// <summary>
        /// Constructs a Vector from a single line of text, the magnitudes of which are demarked by the specified delimeter.
        /// </summary>
        /// <param name="textLine">Line of text containing the components and separated by the delimiter.</param>
        /// <param name="delimiter">Delimiter - defaults to a single space.</param>
        /// <exception cref="Exception">Thrown if the string does not contain exactly three components or cannot be parsed.</exception>
        public Vector(string textLine, char delimiter = ' ')
        {
            // Split the text by the delimiter
            string[] textLineSplit = textLine.Split(delimiter);

            // We expect all three components
            if (textLineSplit.Length != 3)
            {
                throw new Exception("Incorrect number of components found in line");
            }

            try
            {
                // Assign the values
                _i = Convert.ToDouble(textLineSplit[0]);
                _j = Convert.ToDouble(textLineSplit[1]);
                _k = Convert.ToDouble(textLineSplit[2]);
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing component values", ex);
            }
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Magnitude of the vector in the first axis (X)
        /// </summary>
        public MM I
        {
            get { return _i; }

            set { _i = value; }
        }

        /// <summary>
        /// Magnitude of the vector in the second axis (Y)
        /// </summary>
        public MM J
        {
            get { return _j; }
            set { _j = value; }
        }

        /// <summary>
        /// Magnitude of the vector in the third axis (Z)
        /// </summary>
        public MM K
        {
            get { return _k; }
            set { _k = value; }
        }

        /// <summary>
        /// Magnitude of the vector.
        /// </summary>
        public MM Magnitude
        {
            get { return Math.Sqrt(MagnitudeSquare); }
        }

        /// <summary>
        /// Magnitude square of the vector.
        /// </summary>
        public MM MagnitudeSquare
        {
            get { return _i * _i + _j * _j + _k * _k; }
        }

        #endregion

        #region " Operators "

        /// <summary>
        /// Performs vector addition on its two operands.
        /// </summary>
        /// <param name="left">First vector to add.</param>
        /// <param name="right">Second vector to add.</param>
        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.I + right.I, left.J + right.J, left.K + right.K);
        }

        /// <summary>
        /// Subtracts the right-hand of the two specified vectors from the left.
        /// </summary>
        /// <param name="left">Vector from which to subtract (minuend).</param>
        /// <param name="right">Vector to subtract (subtrahend).</param>
        public static Vector operator -(Vector left, Vector right)
        {
            return new Vector(left.I - right.I, left.J - right.J, left.K - right.K);
        }

        /// <summary>
        /// Multiplies a Vector by a scalar.
        /// </summary>
        /// <param name="left">Vector to multiply.</param>
        /// <param name="scalar">Scalar value with which to multiply vector.</param>
        public static Vector operator *(Vector left, double scalar)
        {
            return new Vector(left.I * scalar, left.J * scalar, left.K * scalar);
        }

        /// <summary>
        /// Multiplies a Vector by a scalar.
        /// </summary>
        /// <param name="scalar">Scalar value with which to multiply vector.</param>
        /// <param name="right">Vector to multiply.</param>
        public static Vector operator *(double scalar, Vector right)
        {
            return new Vector(right.I * scalar, right.J * scalar, right.K * scalar);
        }

        /// <summary>
        /// Divides a vector by a scalar value.
        /// </summary>
        /// <param name="left">Vector to be divided.</param>
        /// <param name="scalar">Dividing scalar.</param>
        public static Vector operator /(Vector left, double scalar)
        {
            return new Vector(left.I / scalar, left.J / scalar, left.K / scalar);
        }

        /// <summary>
        /// Calculates the dot product of the two specified vectors.
        /// If both vectors are unit vectors, the dot product will be the cosine of the angle between them.
        /// If only b is a unit vector, the dot product will yield the signed magnitude of the projection
        /// of a on b.
        /// </summary>
        /// <param name="a">First of two vectors.</param>
        /// <param name="b">Second of two vectors.</param>
        /// <returns>Scalar result of the operation.</returns>
        public static MM DotProduct(Vector a, Vector b)
        {
            return a.I * b.I + a.J * b.J + a.K * b.K;
        }

        /// <summary>
        /// Calculates the cross product of the two specified vectors.
        /// The resultant vector is perpendicular to both a and b, the two vectors used to produce it,
        /// with vector a corresponding to the X axis and b the Y.
        /// </summary>
        /// <param name="a">First input vector.</param>
        /// <param name="b">Second input vector.</param>
        /// <returns>Vector result of the operation.</returns>
        public static Vector CrossProduct(Vector a, Vector b)
        {
            double ax = 0;
            double ay = 0;
            double az = 0;
            double bx = 0;
            double by = 0;
            double bz = 0;

            ax = a.I;
            ay = a.J;
            az = a.K;

            bx = b.I;
            by = b.J;
            bz = b.K;

            double i = 0;
            double j = 0;
            double k = 0;

            i = ay * bz - by * az;
            j = az * bx - bz * ax;
            k = ax * by - bx * ay;

            return new Vector(i, j, k);
        }

        /// <summary>
        /// Returns true if the two specified vectors are not equal; false otherwise.
        /// </summary>
        /// <param name="left">First of two vectors.</param>
        /// <param name="right">Second of two vectors.</param>
        public static bool operator !=(Vector left, Vector right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns true if the two specified vectors are equal; false otherwise.
        /// </summary>
        /// <param name="left">First of two vectors.</param>
        /// <param name="right">Second of two vectors.</param>
        public static bool operator ==(Vector left, Vector right)
        {
            // Test for null.
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null))
            {
                return false;
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }

            return Convert.ToBoolean(left.I == right.I && left.J == right.J && left.K == right.K);
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Returns the angle between this and a second vector.
        /// </summary>
        /// <param name="otherVector">Vector with which to measure angle.</param>
        public Radian AngleBetween(Vector otherVector)
        {
            Vector firstVector = Clone();
            firstVector.Normalize();
            Vector secondVector = otherVector.Clone();
            secondVector.Normalize();

            double dotProduct = DotProduct(firstVector, secondVector);
            if (dotProduct < -1)
            {
                dotProduct = -1;
            }
            else if (dotProduct > 1)
            {
                dotProduct = 1;
            }

            return Math.Acos(dotProduct);
        }

        /// <summary>
        /// Returns the angle from this vector to another.
        /// </summary>
        /// <param name="otherVector">Second vector against which to measure angle.</param>
        public Radian AngleTo(Vector otherVector)
        {
            double angleABC = Math.Atan2(otherVector.J, otherVector.I) - Math.Atan2(_j, _i);

            if (angleABC < -1E-06)
            {
                angleABC += 2.0 * Math.PI;
            }

            return angleABC;
        }

        /// <summary>
        /// Normalises this vector.
        /// </summary>
        /// <exception cref="Exception">Thrown if this is of zero length: a zero length vector cannot be converted to a unit vector.</exception>
        public void Normalize()
        {
            try
            {
                MM magnitude = Magnitude;
                I = _i / magnitude.Value;
                J = _j / magnitude.Value;
                K = _k / magnitude.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("Zero length vector cannot be converted to unit vector.");
            }
        }

        /// <summary>
        /// Rebases this vector from a specified workplane to the world.
        /// </summary>
        /// <param name="fromWorkplane">Workplane from which to rebase vector.</param>
        public void RebaseFromWorkplane(Workplane fromWorkplane)
        {
            Vector rebasedVector = new Vector();

            double i = _i;
            double j = _j;
            double k = _k;

            rebasedVector.I = i * fromWorkplane.XAxis.I + j * fromWorkplane.YAxis.I + k * fromWorkplane.ZAxis.I;
            rebasedVector.J = i * fromWorkplane.XAxis.J + j * fromWorkplane.YAxis.J + k * fromWorkplane.ZAxis.J;
            rebasedVector.K = i * fromWorkplane.XAxis.K + j * fromWorkplane.YAxis.K + k * fromWorkplane.ZAxis.K;

            _i = rebasedVector.I;
            _j = rebasedVector.J;
            _k = rebasedVector.K;
        }

        /// <summary>
        /// Rebases this vector to the specified workplane.
        /// </summary>
        /// <param name="toWorkplane">Workplane to rebase to.</param>
        public void RebaseToWorkplane(Workplane toWorkplane)
        {
            Vector rebasedVector = new Vector();

            rebasedVector.I = DotProduct(this, toWorkplane.XAxis);
            rebasedVector.J = DotProduct(this, toWorkplane.YAxis);
            rebasedVector.K = DotProduct(this, toWorkplane.ZAxis);

            _i = rebasedVector.I;
            _j = rebasedVector.J;
            _k = rebasedVector.K;
        }

        /// <summary>
        /// Rotates this Vector by the specified angle in radians about the specified vector.
        /// </summary>
        /// <param name="vectorToRotateAbout">Vector to rotate about.</param>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutVector(Vector vectorToRotateAbout, Radian angle)
        {
            Vector vector = vectorToRotateAbout.Clone();
            vector.Normalize();

            Vector objPointVector = new Vector(_i, _j, _k);
            double dp = DotProduct(objPointVector, vector);
            Vector r_p = vector * dp;
            Vector Vx = objPointVector - r_p;
            Vector N_rx = Vx * Math.Cos(angle);
            Vector xCn = CrossProduct(vector, Vx);
            Vector N_ry = xCn * Math.Sin(angle);
            Vector OutP = r_p + N_rx;
            OutP = OutP + N_ry;

            _i = OutP.I;
            _j = OutP.J;
            _k = OutP.K;
        }

        /// <summary>
        /// Rotates this Vector by the specified angle in Radians about X.
        /// </summary>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutXAxis(Radian angle)
        {
            double cosAngle = 0;
            double sinAngle = 0;

            cosAngle = Math.Cos(angle);
            sinAngle = Math.Sin(angle);

            double yLength = 0;
            double zLength = 0;

            yLength = _j;
            zLength = _k;

            double newYLength = 0;
            double newZLength = 0;

            newYLength = yLength * cosAngle - zLength * sinAngle;
            newZLength = yLength * sinAngle + zLength * cosAngle;

            _i = _i;
            _j = newYLength;
            _k = newZLength;
        }

        /// <summary>
        /// Rotates this Vector by the specified angle in Radians about Y.
        /// </summary>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutYAxis(Radian angle)
        {
            double cosAngle = 0;
            double sinAngle = 0;

            cosAngle = Math.Cos(angle);
            sinAngle = Math.Sin(angle);

            double zLength = 0;
            double xLength = 0;

            zLength = _k;
            xLength = _i;

            double newZLength = 0;
            double newXLength = 0;

            newZLength = zLength * cosAngle - xLength * sinAngle;
            newXLength = zLength * sinAngle + xLength * cosAngle;

            _i = newXLength;
            _j = _j;
            _k = newZLength;
        }

        /// <summary>
        /// Rotates this Vector by the specified angle in Radians about Z.
        /// </summary>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutZAxis(Radian angle)
        {
            double cosAngle = 0;
            double sinAngle = 0;

            cosAngle = Math.Cos(angle);
            sinAngle = Math.Sin(angle);

            double xLength = 0;
            double yLength = 0;

            xLength = _i;
            yLength = _j;

            double dblNewXLength = 0;
            double dblNewYLength = 0;

            dblNewXLength = xLength * cosAngle - yLength * sinAngle;
            dblNewYLength = xLength * sinAngle + yLength * cosAngle;

            _i = dblNewXLength;
            _j = dblNewYLength;
            _k = _k;
        }

        /// <summary>
        /// Rotates the Vector by the specified Euler Angles.
        /// </summary>
        /// <param name="eulerAngles">Angles to rotate.</param>
        public void EulerRotation(Euler.Angles eulerAngles)
        {
            int i = 0;
            int t = 0;

            double[] M3 = new double[3];
            double[] M2 =
            {
                I,
                J,
                K
            };

            for (i = 0; i <= 2; i++)
            {
                M3[i] = 0;
                for (t = 0; t <= 2; t++)
                {
                    M3[i] += eulerAngles.Matrix[i, t] * M2[t];
                }
            }

            _i = M3[0];
            _j = M3[1];
            _k = M3[2];
        }

        /// <summary>
        /// Sets the xVector parameter to be the size of this in the X axis and the yVector parameter to be the size of this in the Y axis.
        /// </summary>
        /// <param name="xVector">Vector to populate as X.</param>
        /// <param name="yVector">Vector to populate as Y.</param>
        public void GetXYVectors(ref Vector xVector, ref Vector yVector)
        {
            //Normalize this vector
            Vector zVector = Clone();
            zVector.Normalize();

            //Dont quite understand the rest...
            Vector x = null;
            Vector y = null;

            if ((Math.Abs(Convert.ToDouble(zVector.K)) >= Math.Abs(Convert.ToDouble(zVector.I))) &
                (Math.Abs(Convert.ToDouble(zVector.K)) >= Math.Abs(Convert.ToDouble(zVector.J))))
            {
                x = new Vector(1, 0, -1.0 * zVector.J / zVector.K);
            }
            else if ((Math.Abs(Convert.ToDouble(zVector.J)) >= Math.Abs(Convert.ToDouble(zVector.I))) &
                     (Math.Abs(Convert.ToDouble(zVector.J)) >= Math.Abs(Convert.ToDouble(zVector.K))))
            {
                x = new Vector(1, -1.0 * zVector.K / zVector.J, 0);
            }
            else if (zVector.I != 0)
            {
                x = new Vector(-1.0 * zVector.K / zVector.I, 0, 1);
            }

            x.Normalize();
            y = CrossProduct(zVector, x);

            y.Normalize();
            x = CrossProduct(y, zVector);

            xVector = x;
            yVector = y;
        }

        /// <summary>
        /// Returns True if this Vector is equal to the specified object. I.e. all three axes are equivalent.
        /// </summary>
        /// <param name="obj">Object to compare against.</param>
        /// <returns>True if vector and object are equivalent; false otherwwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }
            var _with1 = (Vector) obj;
            if (I == _with1.I && J == _with1.J && K == _with1.K)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the Vector is equal to the specified object with at least the specified precision. If the the object is not a Vector
        /// or is not equivalent to the Vector, the operation returns False.
        /// </summary>
        /// <param name="obj">Object to comare against.</param>
        /// <param name="nDecPts">Number of decimal places to compare.</param>
        /// <returns>True if the operands are deemed equal.</returns>
        public bool Equals(object obj, int nDecPts)
        {
            if (!(obj is Vector))
            {
                return false;
            }
            var _with2 = (Vector) obj;
            if (I.Equals(_with2.I, nDecPts) && J.Equals(_with2.J, nDecPts) && K.Equals(_with2.K, nDecPts))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a string representation of the Vector of the form: I.IIII J.JJJ K.KKK
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", I, J, K);
        }

        #endregion

        #region " Copy Operation "

        /// <summary>
        /// Returns a clone of this Vector.
        /// </summary>
        public Vector Clone()
        {
            Vector cloneVector = new Vector(_i, _j, _k);
            return cloneVector;
        }

        #endregion
    }
}