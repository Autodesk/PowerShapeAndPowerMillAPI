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
    /// Encapsulates a three-dimensional point and the permissible operations thereon.
    /// </summary>
    [Serializable]
    public class Point
    {
        #region "Fields"

        /// <summary>
        /// X coordinate.
        /// </summary>
        protected MM _x;

        /// <summary>
        /// Y coordinate.
        /// </summary>
        protected MM _y;

        /// <summary>
        /// Z coordinate.
        /// </summary>
        protected MM _z;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs a point at the origin of the co-ordinate system.
        /// </summary>
        public Point()
        {
        }

        /// <summary>
        /// Constructs a point at the specified position in the co-ordinate system.
        /// </summary>
        /// <param name="x">X co-ordinate.</param>
        /// <param name="y">Y co-ordinate.</param>
        /// <param name="z">Z co-ordinate</param>
        public Point(MM x, MM y, MM z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Constructs a point from an array of MM objects.
        /// </summary>
        /// <param name="pointArray">Array from which to create the point.</param>
        /// <exception cref="Exception">Thrown if input array does not contain exactly three coordinates.</exception>
        public Point(MM[] pointArray)
        {
            // Check that there are three elements in the array
            if (pointArray.Length != 3)
            {
                throw new Exception("Point constructor requires 3 coordinates");
            }

            _x = pointArray[0];
            _y = pointArray[1];
            _z = pointArray[2];
        }

        /// <summary>
        /// Constructs a point from an array of Doubles.
        /// </summary>
        /// <param name="pointArray">Array from which to create the point.</param>
        public Point(double[] pointArray)
        {
            try
            {
                _x = pointArray[0];
                _y = pointArray[1];
                _z = pointArray[2];
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Constructs a point from a single line of text and a delimiter.
        /// </summary>
        /// <param name="textLine">Line of text containing the delimited coordinates.</param>
        /// <param name="delimiter">Delimiter. If none is specified, a space is assumed.</param>
        /// <exception cref="Exception">Thrown if the input string does not contain exactly three coordinates or if the string cannot be parsed.</exception>
        public Point(string textLine, char delimiter = ' ')
        {
            // Split the text by the delimiter
            string[] textLineSplit = textLine.Split(delimiter);

            // We expect all three coordinates
            if (textLineSplit.Length != 3)
            {
                throw new Exception("Incorrect number of points found in line");
            }

            try
            {
                // Assign the values
                _x = Convert.ToDouble(textLineSplit[0]);
                _y = Convert.ToDouble(textLineSplit[1]);
                _z = Convert.ToDouble(textLineSplit[2]);
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing point values", ex);
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// X coordinate.
        /// </summary>
        public MM X
        {
            get { return _x; }

            set { _x = value; }
        }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public MM Y
        {
            get { return _y; }

            set { _y = value; }
        }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public MM Z
        {
            get { return _z; }

            set { _z = value; }
        }

        #endregion

        #region "Operators"

        /// <summary>
        /// Sums a Point and a Vector i.e displace the point by the vector.
        /// </summary>
        /// <param name="left">Point to sum.</param>
        /// <param name="right">Vector to sum.</param>
        /// <returns>Displaced point.</returns>
        public static Point operator +(Point left, Vector right)
        {
            return new Point(left.X + right.I, left.Y + right.J, left.Z + right.K);
        }

        /// <summary>
        /// Subtracts the specified right-hand point from the left, thereby rendering the vector from right to left.
        /// </summary>
        /// <param name="left">Left-hand point.</param>
        /// <param name="right">Right-hand point.</param>
        /// <returns>Vector from right to left.</returns>
        public static Vector operator -(Point left, Point right)
        {
            return new Vector(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        /// Subtracts the given Vector from the Point i.e displaces the point by the vector.
        /// </summary>
        /// <param name="left">Point from which to subtract vector.</param>
        /// <param name="right">Vector to subtract.</param>
        /// <returns></returns>
        public static Point operator -(Point left, Vector right)
        {
            return new Point(left.X - right.I, left.Y - right.J, left.Z - right.K);
        }

        /// <summary>
        /// Scales a point in all axes by the specified scalar value.
        /// </summary>
        /// <param name="left">Point to scale.</param>
        /// <param name="scalar">Scaling factor.</param>
        /// <returns>New point containing the result of the operation.</returns>
        public static Point operator *(Point left, double scalar)
        {
            return new Point(left.X * scalar, left.Y * scalar, left.Z * scalar);
        }

        /// <summary>
        /// Divides a point in all axes by the scalar value.
        /// </summary>
        /// <param name="left">Point to scale.</param>
        /// <param name="scalar">Scaling factor.</param>
        /// <returns>New point containing the result of the operation.</returns>
        public static Point operator /(Point left, double scalar)
        {
            return new Point(left.X / scalar, left.Y / scalar, left.Z / scalar);
        }

        /// <summary>
        /// Scales a point in all axes by the specified scalar value.
        /// </summary>
        /// <param name="scalar">Scaling factor.</param>
        /// <param name="right">Point to scale.</param>
        /// <returns>New point containing the result of the operation.</returns>
        public static Point operator *(double scalar, Point right)
        {
            return new Point(right.X * scalar, right.Y * scalar, right.Z * scalar);
        }

        /// <summary>
        /// Compares a Point to an Object and returns true if they are equal. Returns False if the specified Object
        /// is neither a Point nor a MM. Returns True if both Point and Object are null.
        /// </summary>
        /// <param name="left">Point to compare.</param>
        /// <param name="right">Object to compare.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool operator ==(Point left, object right)
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

            // Test for valid object.
            if (!(right is Point) && !(right is MM[]))
            {
                return false;
            }

            // Is it a Point..?
            if (right is Point)
            {
                var point = (Point) right;
                return left.X == point.X && left.Y == point.Y && left.Z == point.Z;
            }
            if (right is MM[])
            {
                // It should be a MM!
                MM[] rightMM = (MM[]) right;

                return left.X == rightMM[0] && left.Y == rightMM[1] && left.Z == rightMM[2];
            }
            return false;
        }

        /// <summary>
        /// Compares a Point to an Object and returns false if they are equal. Returns True if the specified Object
        /// is neither a Point nor a MM. Returns False if both Point and Object are null.
        /// </summary>
        /// <param name="left">Point to compare.</param>
        /// <param name="right">Object to compare.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool operator !=(Point left, object right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares a Point to an array of MM length objects, returning True if they are unequal.
        /// </summary>
        /// <param name="left">MM array.</param>
        /// <param name="right">Point.</param>
        /// <returns>True if operands are unequivalent; false otherwise.</returns>
        public static bool operator !=(MM[] left, Point right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares a Point to an array of MM length objects, returning True if they are equal.
        /// </summary>
        /// <param name="left">MM array.</param>
        /// <param name="right">Point.</param>
        /// <returns>True if operands are equivalent; false otherwise.</returns>
        public static bool operator ==(MM[] left, Point right)
        {
            Point leftPoint = new Point(left);

            return right == leftPoint;
        }

        #endregion

        #region "Overrides Operations"

        /// <summary>
        /// Returns a string representation of the Point in the form: X.XXX Y.YYY Z.ZZZ.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }

        /// <summary>
        /// Returns a string representation of the point with a custom delimiter between the three coordinates.
        /// If, for example, a comma were chosen as the delimeter, the resultant output would be of the form:
        /// X.XXX,Y.YYY,Z.ZZZ
        /// </summary>
        /// <param name="delimiter">Can be any string.</param>
        public string ToString(char[] delimiter)
        {
            return string.Format("{0}" + delimiter + "{1}" + delimiter + "{2}", X, Y, Z);
        }

        /// <summary>
        /// Returns true if the Point is equal to the specified object. If the object is not a point
        /// or is not equivalent to the Point, the operation returns False.
        /// </summary>
        /// <param name="obj">Object to comare against.</param>
        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }
            var _with1 = (Point) obj;
            if (X == _with1.X && Y == _with1.Y && Z == _with1.Z)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the Point is equal to the specified object with at least the specified precision. If the the object is not a point
        /// or is not equivalent to the Point, the operation returns False.
        /// </summary>
        /// <param name="obj">Object to comare against.</param>
        /// <param name="nDecPts">Number of decimal places to compare.</param>
        /// <returns>True if the operands are deemed equal.</returns>
        public bool Equals(object obj, int nDecPts)
        {
            if (!(obj is Point))
            {
                return false;
            }
            var _with2 = (Point) obj;
            if (X.Equals(_with2.X, nDecPts) && Y.Equals(_with2.Y, nDecPts) && Z.Equals(_with2.Z, nDecPts))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the HashCode for this Point.
        /// </summary>
        /// <returns>The HashCode for this Point.</returns>
        public override int GetHashCode()
        {
            //Perform field-by-field XOR of HashCodes
            return X.Value.GetHashCode() ^ Y.Value.GetHashCode() ^ Z.Value.GetHashCode();
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Rebases the Point from the specified workplane to the world.
        /// </summary>
        /// <param name="fromWorkplane">Workplane from which to rebase the point.</param>
        public void RebaseFromWorkplane(Workplane fromWorkplane)
        {
            MM x = _x;
            MM y = _y;
            MM z = _z;

            X = x * fromWorkplane.XAxis.I + y * fromWorkplane.YAxis.I + z * fromWorkplane.ZAxis.I;
            Y = x * fromWorkplane.XAxis.J + y * fromWorkplane.YAxis.J + z * fromWorkplane.ZAxis.J;
            Z = x * fromWorkplane.XAxis.K + y * fromWorkplane.YAxis.K + z * fromWorkplane.ZAxis.K;

            X += fromWorkplane.Origin.X;
            Y += fromWorkplane.Origin.Y;
            Z += fromWorkplane.Origin.Z;
        }

        /// <summary>
        /// Rebases Point from the world to the specified Workplane
        /// </summary>
        /// <param name="toWorkplane">Workplane to rebase the Point to.</param>
        public void RebaseToWorkplane(Workplane toWorkplane)
        {
            Vector fromOrigin = null;
            fromOrigin = this - toWorkplane.Origin;

            X = Vector.DotProduct(fromOrigin, toWorkplane.XAxis);
            Y = Vector.DotProduct(fromOrigin, toWorkplane.YAxis);
            Z = Vector.DotProduct(fromOrigin, toWorkplane.ZAxis);
        }

        /// <summary>
        /// Rotates Point by the specified angle in radians about the specified Vector.
        /// </summary>
        /// <param name="vectorToRotateAbout">Vector about which to rotate the Point.</param>
        /// <param name="rotationOrigin">Origin of coordinate system.</param>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutVector(Vector vectorToRotateAbout, Point rotationOrigin, Radian angle)
        {
            Vector vector = vectorToRotateAbout.Clone();
            vector.Normalize();

            Vector vectorToOrigin = rotationOrigin - new Point();

            // Move this point to the rotation origin
            Point thisPoint = Clone();
            thisPoint -= vectorToOrigin;

            Vector objPointVector = new Vector(thisPoint.X, thisPoint.Y, thisPoint.Z);
            double dp = Vector.DotProduct(objPointVector, vector);
            Vector r_p = vector * dp;
            Vector Vx = objPointVector - r_p;
            Vector N_rx = Vx * Math.Cos(angle);
            Vector xCn = Vector.CrossProduct(vector, Vx);
            Vector N_ry = xCn * Math.Sin(angle);
            Vector OutP = r_p + N_rx;
            OutP = OutP + N_ry;

            // Move the point back
            thisPoint = new Point(OutP.I, OutP.J, OutP.K);
            thisPoint += vectorToOrigin;

            _x = thisPoint.X;
            _y = thisPoint.Y;
            _z = thisPoint.Z;
        }

        /// <summary>
        /// Rotates Point by the specified angle in radians about the specified Point in the X axis.
        /// </summary>
        /// <param name="pointToRotateAbout">Point about which to rotate.</param>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutXAxis(Point pointToRotateAbout, Radian angle)
        {
            double cosAngle = 0;
            double sinAngle = 0;

            cosAngle = Math.Cos(angle);
            sinAngle = Math.Sin(angle);

            double yLength = 0;
            double zLength = 0;

            yLength = _y - pointToRotateAbout.Y;
            zLength = _z - pointToRotateAbout.Z;

            double newYLength = 0;
            double newZLength = 0;

            newYLength = yLength * cosAngle - zLength * sinAngle;
            newZLength = yLength * sinAngle + zLength * cosAngle;

            X = _x;
            Y = newYLength + pointToRotateAbout.Y;
            Z = newZLength + pointToRotateAbout.Z;
        }

        /// <summary>
        /// Rotates Point by the specified angle in radians about the specified Point in the Y axis.
        /// </summary>
        /// <param name="pointToRotateAbout">Point about which to rotate.</param>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutYAxis(Point pointToRotateAbout, Radian angle)
        {
            double cosAngle = 0;
            double sinAngle = 0;

            cosAngle = Math.Cos(angle);
            sinAngle = Math.Sin(angle);

            double zLength = 0;
            double xLength = 0;

            zLength = _z - pointToRotateAbout.Z;
            xLength = _x - pointToRotateAbout.X;

            double newZLength = 0;
            double newXLength = 0;

            newZLength = zLength * cosAngle - xLength * sinAngle;
            newXLength = zLength * sinAngle + xLength * cosAngle;

            Z = newZLength + pointToRotateAbout.Z;
            Y = _y;
            X = newXLength + pointToRotateAbout.X;
        }

        /// <summary>
        /// Rotates Point by the specified angle in radians about the specified Point in the Z axis.
        /// </summary>
        /// <param name="pointToRotateAbout">Point about which to rotate.</param>
        /// <param name="angle">Angle to rotate.</param>
        public void RotateAboutZAxis(Point pointToRotateAbout, Radian angle)
        {
            double cosAngle = 0;
            double sinAngle = 0;

            cosAngle = Math.Cos(angle);
            sinAngle = Math.Sin(angle);

            double xLength = 0;
            double yLength = 0;

            xLength = _x - pointToRotateAbout.X;
            yLength = _y - pointToRotateAbout.Y;

            double newXLength = 0;
            double newYLength = 0;

            newXLength = xLength * cosAngle - yLength * sinAngle;
            newYLength = xLength * sinAngle + yLength * cosAngle;

            X = newXLength + pointToRotateAbout.X;
            Y = newYLength + pointToRotateAbout.Y;
            Z = _z;
        }

        /// <summary>
        /// Rotates the Point by the specified Euler Angles.
        /// </summary>
        /// <param name="angle">Angles of rotation.</param>
        public void EulerRotation(Euler.Angles angle)
        {
            int i = 0;
            int t = 0;
            double[] M3 = new double[3];
            double[] M2 =
            {
                X,
                Y,
                Z
            };

            for (i = 0; i <= 2; i++)
            {
                M3[i] = 0;
                for (t = 0; t <= 2; t++)
                {
                    M3[i] += angle.Matrix[i, t] * M2[t];
                }
            }

            X = M3[0];
            Y = M3[1];
            Z = M3[2];
        }

        /// <summary>
        /// Returns the distance between this and another Point.
        /// </summary>
        /// <param name="secondPoint">Point from which to calculate distance.</param>
        /// <returns>Inter-point distance.</returns>
        public MM DistanceToPoint(Point secondPoint)
        {
            return (secondPoint - this).Magnitude;
        }

        /// <summary>
        /// Determines whether this Point lies on the straight line between two Points specified in the input arguments.
        /// </summary>
        /// <param name="lineStartPoint">First point.</param>
        /// <param name="lineEndPoint">Second point.</param>
        /// <returns>True if the points are colinear; false otherwise.</returns>
        public bool LiesOnLine(Point lineStartPoint, Point lineEndPoint)
        {
            // Create a line from the points
            Line line = new Line(lineStartPoint, lineEndPoint);

            // If point coordinates are not between line coordinates, can definitely return false
            if (_x < line.BoundingBox.MinX || _x > line.BoundingBox.MaxX || _y < line.BoundingBox.MinY ||
                (_y > line.BoundingBox.MaxY) |
                (_z < line.BoundingBox.MinZ) || _z > line.BoundingBox.MaxZ)
            {
                return false;
            }

            // USe cartesian equation of 3D line to determine whether it is solveable with current point coordinates
            Vector directionVector = new Vector(lineStartPoint, lineEndPoint);
            double xElement = (_x - lineStartPoint.X) / directionVector.I;
            double yElement = (_y - lineStartPoint.Y) / directionVector.J;
            double zElement = (_z - lineStartPoint.Z) / directionVector.K;

            // Return false if any of them are not equal
            if (xElement != yElement || yElement != zElement || zElement != xElement)
            {
                return false;
            }

            // If we have reached this stage, then the point does lie on the line
            return true;
        }

        /// <summary>
        /// Returns true if the point is inside the triangle, otherwise it returns false. It uses Barycentric coordinates of the point to return the result.
        /// </summary>
        /// <param name="vertex1">First triangle Vertex</param>
        /// <param name="vertex2">Second triangle Vertex</param>
        /// <param name="vertex3">Third triangle Vertex</param>
        /// <returns>True if point is within the triangle.</returns>
        public bool IsInsideTriangle(Point vertex1, Point vertex2, Point vertex3)
        {
            // Using barycentric coordinates a Point P is within the triangle (A, B, C)
            // For a detailed explanation of the algorithm see its source: http://blogs.msdn.com/b/rezanour/archive/2011/08/07/barycentric-coordinates-and-point-in-triangle-tests.aspx

            //Prepare our barycentric variables
            var u = vertex2 - vertex1;
            var v = vertex3 - vertex1;
            var w = this - vertex1;

            var vCrossW = Vector.CrossProduct(v, w);
            var vCrossU = Vector.CrossProduct(v, u);

            //Test sign of r
            if (Vector.DotProduct(vCrossW, vCrossU) < 0)
            {
                return false;
            }

            var uCrossW = Vector.CrossProduct(u, w);
            var uCrossV = Vector.CrossProduct(u, v);

            //Test sign of t
            if (Vector.DotProduct(uCrossW, uCrossV) < 0)
            {
                return false;
            }

            //At this point, we know that r And t And both > 0.
            //Therefore, as long as their sum Is <= 1, each must be less <= 1

            var denom = uCrossV.Magnitude;
            var r = vCrossW.Magnitude / denom;
            var t = uCrossW.Magnitude / denom;

            return r + t <= 1;
        }

        /// <summary>
        /// Returns true if a ray with given direction from the point intercepts the triangle, otherwise it returns false.
        /// Uses ray triangle intersection algorithm by Moller Haines.
        /// </summary>
        /// <param name="rayDirection">Direction of the ray</param>
        /// <param name="vertex1">First triangle Vertex</param>
        /// <param name="vertex2">Second triangle Vertex</param>
        /// <param name="vertex3">Third triangle Vertex</param>
        /// <returns>True if point is within the triangle.</returns>
        public bool IsRayInterceptTriangleTest(Vector rayDirection, Point vertex1, Point vertex2, Point vertex3)
        {
            var edge1 = new Vector(vertex1, vertex2);
            var edge2 = new Vector(vertex1, vertex3);

            //begin calculating determinant - also used to calculate U parameter
            var p = Vector.CrossProduct(rayDirection, edge2);

            //if determinant is near zero, ray lies in plane of triangle
            var det = Vector.DotProduct(edge1, p).Value;
            if (det > -double.Epsilon && det < double.Epsilon)
            {
                return false;
            }
            var invDet = 1 / det;

            //calculate distance from vertex1 to ray origin
            var r = new Vector(vertex1, this);

            //calculate U parameter and test bounds
            var u = Vector.DotProduct(r, p) * invDet;

            //The intersection lies outside of the triangle
            if (u < 0 || u > 1)
            {
                return false;
            }

            //Prepare to test v parameter
            var q = Vector.CrossProduct(r, edge1);

            // Calculate v parameter and test bound
            var v = Vector.DotProduct(rayDirection, q) * invDet;

            //The intersection lies outside of the triangle
            if (v < 0 || u + v > 1)
            {
                return false;
            }

            var intersectionDistance = Vector.DotProduct(edge2, q) * invDet;

            // Ray Intersection
            if (intersectionDistance >= 0)
            {
                return true;
            }

            // No hit
            return false;
        }

        /// <summary>
        /// This operation projects a point onto a Plane
        /// </summary>
        /// <param name="pointToProject">This is the point to project</param>
        /// <param name="projectionVector">This is the direction in which to project the point</param>
        /// <param name="pointOnPlane">This is any point on the plane</param>
        /// <param name="planeNormal">This is the plane normal</param>
        /// <returns>The point on the plane where the projected point hits</returns>
        public Point ProjectToPlane(Vector projectionVector, Point pointOnPlane, Vector planeNormal)
        {
            // As both the normal and projection vector are normalized this will give us the angle between them
            double dotProduct = Vector.DotProduct(projectionVector, planeNormal);

            // If the angle is zero then the projection misses the Plane
            if (Math.Abs(dotProduct) < 1E-07)
            {
                return null;
            }

            // Create a line from the projection point to the edge of the plane
            Vector projectionToPoint = pointOnPlane - this;

            // Then calculate the distance from the projection point to the plane down the projected vector
            // The dot product gives us the distance from the projection point to the plane (at right angles)
            double distance = 0;
            distance = Vector.DotProduct(projectionToPoint, planeNormal) / dotProduct;

            // Calculate projection point
            return this + projectionVector * distance;
        }

        #endregion

        #region "Clone Operation"

        /// <summary>
        /// Returns a clone of this Point.
        /// </summary>
        public Point Clone()
        {
            Point clonePoint = new Point(_x, _y, _z);
            return clonePoint;
        }

        #endregion
    }
}