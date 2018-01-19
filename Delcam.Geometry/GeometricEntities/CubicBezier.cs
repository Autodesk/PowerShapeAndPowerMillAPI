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

namespace Autodesk.Geometry
{
    /// <summary>
    /// Most important case in the family of bezier curves. Consists of four control points,
    /// the first and last of which are incident on the curve.
    /// </summary>
    /// <remarks></remarks>
    public class CubicBezier
    {
        #region "Constructors"

        /// <summary>
        /// Constructs a CubicBezier object with the specified parameters.
        /// </summary>
        /// <param name="startPoint">Curve starts here.</param>
        /// <param name="endPoint">Curve ends here.</param>
        /// <param name="controlPoint1">First control point.</param>
        /// <param name="controlPoint2">Second control point.</param>
        /// <remarks></remarks>
        public CubicBezier(Point startPoint, Point endPoint, Point controlPoint1, Point controlPoint2)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            StartControlPoint = controlPoint1;
            EndControlPoint = controlPoint2;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Start point.
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// End point.
        /// </summary>
        public Point EndPoint { get; set; }

        /// <summary>
        /// First control point.
        /// </summary>
        public Point StartControlPoint { get; set; }

        /// <summary>
        /// Second control point.
        /// </summary>
        public Point EndControlPoint { get; set; }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a point on the curve in t where 0 &lt;= t &lt;= 1
        /// </summary>
        /// <param name="t">Relative time parameter at which to calculate point.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if t does not satisfy the condition: 0 &lt;= t &lt;= 1.</exception>
        public Point Position(double t)
        {
            if ((t < 0) | (t > 1))
            {
                throw new ArgumentOutOfRangeException("t must satisfy the condition 0 <= t <= 1.");
            }

            Point term1 = Math.Pow(1 - t, 3) * StartPoint;
            Point term2 = 3 * Math.Pow(1 - t, 2) * t * StartControlPoint;
            Point term3 = 3 * (1 - t) * Math.Pow(t, 2) * EndControlPoint;
            Point term4 = Math.Pow(t, 3) * EndPoint;

            Point pos = new Point();
            pos.X = term1.X + term2.X + term3.X + term4.X;
            pos.Y = term1.Y + term2.Y + term3.Y + term4.Y;
            pos.Z = term1.Z + term2.Z + term3.Z + term4.Z;
            return pos;
        }

        /// <summary>
        /// Returns the tangent at a position on the Bezier Curve in t where 0 &lt;= t &lt;= 1.
        /// The tangent is normalized by default.
        /// </summary>
        /// <param name="t">Relative time parameter specifying position at which tangent is to be calculated.</param>
        /// <param name="normalize">If True, tangent will be normalised. False otherwise.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if t does not satisfy the condition: 0 &lt;= t &lt;= 1.</exception>
        public Vector Tangent(double t, bool normalize = true)
        {
            if ((t < 0) | (t > 1))
            {
                throw new ArgumentOutOfRangeException("t must satisfy the condition 0 <= t <= 1.");
            }

            Vector term1 = 3 * Math.Pow(1 - t, 2) * (StartControlPoint - StartPoint);
            Vector term2 = 6 * (1 - t) * t * (EndControlPoint - StartControlPoint);
            Vector term3 = 3 * Math.Pow(t, 2) * (EndPoint - EndControlPoint);

            Vector calculatedTangent = term1 + term2 + term3;
            if (normalize)
            {
                calculatedTangent.Normalize();
            }
            return calculatedTangent;
        }

        /// <summary>
        /// Returns vector representing radius of curvature at the specified position on the curve in t where 0 &lt;= t &lt;= 1.
        /// </summary>
        /// <param name="t">Relative time parameter specifying position at which tangent is to be calculated.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if t does not satisfy the condition: 0 &lt;= t &lt;= 1.</exception>
        public Vector Curvature(double t)
        {
            if ((t < 0) | (t > 1))
            {
                throw new ArgumentOutOfRangeException("t must satisfy the condition 0 <= t <= 1.");
            }

            Point p0 = StartPoint;
            Point p1 = StartControlPoint;
            Point p2 = EndControlPoint;
            Point p3 = EndPoint;

            double termX = 6 * (1 - t) * (p2.X - 2.0 * p1.X + p0.X) + 6 * t * (p3.X - 2.0 * p2.X + p1.X);
            double termY = 6 * (1 - t) * (p2.Y - 2.0 * p1.Y + p0.Y) + 6 * t * (p3.Y - 2.0 * p2.Y + p1.Y);
            double termZ = 6 * (1 - t) * (p2.Z - 2.0 * p1.Z + p0.Z) + 6 * t * (p3.Z - 2.0 * p2.Z + p1.Z);

            Vector calculatedCurvature = new Vector(termX, termY, termZ);
            calculatedCurvature.Normalize();
            return calculatedCurvature;
        }

        /// <summary>
        /// Calculates the value of t, where 0 &lt;= t &lt;= 1, at which occurs the point on the
        /// bezier curve closest to a given point in space.
        /// </summary>
        /// <param name="pointInSpace">Point in space at which to determine the bezier closest approach. </param>
        /// <param name="nIterationsList">List of iteration coefficients.</param>
        /// <param name="distanceToNearestPosition">Actual distance from curve at t to point in space. </param>
        /// <returns>Value of t at closest approach.</returns>
        /// <remarks>
        /// Algorithm progressively reduces granularity of result by first identifying a span t/n of closest approach,
        /// where n is nIterationList[0], and then refining the outcome with successive decomposition phases, the number of which is the
        /// number of integers in nIterationList.
        /// </remarks>
        public double FindNearestPositionOnSpan(
            ref Point pointInSpace,
            ref List<int> nIterationsList,
            out double distanceToNearestPosition)
        {
            // Get the distance from the point in space to the start of the Bezier Curve
            Vector startVector = pointInSpace - StartPoint;
            double startDistance = startVector.Magnitude;

            distanceToNearestPosition = 0;

            // Get the distance from the point in space to the end of the Bezier Curve
            Vector endVector = pointInSpace - EndPoint;
            double endDistance = endVector.Magnitude;

            double initialMinimumDistance = 0;
            double initialNearestT = 0;

            // intialise the nearest position to either the start or the end of the curve
            if (startDistance < endDistance)
            {
                initialMinimumDistance = startDistance;
                initialNearestT = 0;
            }
            else
            {
                initialMinimumDistance = endDistance;
                initialNearestT = 1;
            }

            // iteratively find the nearest point on the curve
            double distance = 0;
            int i = 0;
            double t = 0;
            double currentMinimumDistance = initialMinimumDistance;
            double currentNearestT = initialNearestT;

            // If the distance from the start or the end point is 0 then we can stop searching. Otherwise we find the nearest position on the span

            if (!(currentMinimumDistance == 0))
            {
                double firstT = 0;
                double lastT = 1;

                foreach (int nSteps in nIterationsList)
                {
                    double iterationMinimumDistance = initialMinimumDistance;
                    double iterationMinimumT = initialNearestT;

                    double dT = (lastT - firstT) / nSteps;

                    for (i = 0; i <= nSteps - 1; i++)
                    {
                        t = firstT + i * dT;

                        Point currentPoint = Position(t);

                        Vector currentVector = pointInSpace - currentPoint;
                        distance = currentVector.Magnitude;

                        if (distance < iterationMinimumDistance)
                        {
                            iterationMinimumDistance = distance;
                            iterationMinimumT = t;
                        }
                    }

                    if (iterationMinimumDistance < currentMinimumDistance)
                    {
                        currentMinimumDistance = iterationMinimumDistance;
                        currentNearestT = iterationMinimumT;
                    }

                    if (currentNearestT == 0)
                    {
                        firstT = 0;
                        lastT = 0 + dT;
                    }
                    else if (currentNearestT == 1)
                    {
                        firstT = 1 - dT;
                        lastT = 1;
                    }
                    else
                    {
                        firstT = currentNearestT - dT;
                        lastT = currentNearestT + dT;
                    }
                }
            }

            distanceToNearestPosition = currentMinimumDistance;

            return currentNearestT;
        }

        /// <summary>
        /// Determines whether the Bezier curve is straight.
        /// Occurs when all points along it are colinear.
        /// </summary>
        /// <param name="isAllowOutsideRange">If True, control points do not have to fall between start and end points.</param>
        public bool IsSpanStraight(bool isAllowOutsideRange = true)
        {
            Point c0 = StartPoint;
            Point c1 = StartControlPoint;
            Point c2 = EndControlPoint;
            Point c3 = EndPoint;

            bool isC1Colinear = IsPointsColinear(ref c0, ref c3, ref c1, isAllowOutsideRange);
            bool isC2Colinear = IsPointsColinear(ref c3, ref c0, ref c2, isAllowOutsideRange);

            bool isStraight = false;
            if (isC1Colinear & isC2Colinear)
            {
                isStraight = true;
            }

            return isStraight;
        }

        /// <summary>
        /// Checks whether a point is colinear with two other points
        /// </summary>
        private bool IsPointsColinear(ref Point p1, ref Point p2, ref Point testPoint, bool isAllowOutsideRange = true)
        {
            Vector v1 = p2 - p1;
            Vector v2 = testPoint - p1;

            v1.Normalize();
            v2.Normalize();

            bool isColinear = false;
            if ((v1.I == v2.I) & (v1.J == v2.J) & (v1.K == v2.K))
            {
                isColinear = true;
            }
            else if ((v1.I == -v2.I) & (v1.J == -v2.J) & (v1.K == -v2.K) & isAllowOutsideRange)
            {
                isColinear = true;
            }

            return isColinear;
        }

        /// <summary>
        /// The curvature ratio is distance(C0--->C1--->C2--->C3)/distance(C0---->C3)
        /// A bezier curve is always within the boundary of its control points
        /// A curvature ratio of 1 corresponds to a straight line
        /// A large curvature ratio means that the bezier curve has a large curvature
        /// </summary>
        internal double GetCurvatureRatio()
        {
            bool isSpanStraight = IsSpanStraight(true);

            if (isSpanStraight)
            {
                return 1;
            }
            Point c0 = StartPoint;
            Point c1 = StartControlPoint;
            Point c2 = EndControlPoint;
            Point c3 = EndPoint;

            // I use double because this will be used to help approximate splines with linear segments
            // Hence bezier curves could be short and rounding errors significant
            double shortDistance = (c3 - c0).Magnitude;
            double longDistance = (c3 - c2).Magnitude + (c2 - c1).Magnitude + (c1 - c0).Magnitude;

            double curvatureRatio = longDistance / shortDistance;

            return curvatureRatio;
        }

        /// <summary>
        /// Approximates the length of a span as
        /// (distance(C0--->C3) + distance(C0--->C1 + C1--->C2 + C2--->C3))/2
        /// This is accurate for splines that are almost linear
        /// </summary>
        internal double ApproximateSpanLengthUsingCurvatureRatio()
        {
            double curvatureRatio = GetCurvatureRatio();

            double approximateSpanLength = (EndPoint - StartPoint).Magnitude * (1 + (curvatureRatio - 1) / 2);
            return approximateSpanLength;
        }

        #endregion
    }
}