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

namespace Autodesk.Geometry.GeometricEntities
{
    /// <summary>
    /// Represents a non-primitive surface defined by a network of curves.
    /// </summary>
    public class Surface
    {
        #region Fields

        private List<Spline> _laterals;
        private List<Spline> _longitudinals;

        #endregion

        #region Construtors

        /// <summary>
        /// Creates a surface with an empty list of laterals and longitudinals.
        /// </summary>
        private Surface()
        {
            _laterals = new List<Spline>();
            _longitudinals = new List<Spline>();
        }

        /// <summary>
        /// Creates a surface from the surface curves provided. The other set of surface curves is calculated by joining the network.
        /// </summary>
        /// <param name="surfaceCurves">The laterals or longitudinals of the surface.</param>
        /// <param name="areLaterals">True if the curves are the laterals, false if they are the longitudinals of the surface.</param>
        /// <exception cref="System.ArgumentException">Thrown when the provided surface curves don't have the same number of points.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the number of surface curves is below 2.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when surface curves are null.</exception>
        public Surface(List<Spline> surfaceCurves, bool areLaterals) : this()
        {
            if (surfaceCurves == null)
            {
                throw new ArgumentNullException("Surface curves cannot be null.");
            }

            if (surfaceCurves.Count < 2)
            {
                throw new ArgumentException("To create a surface at least two curves are needed.");
            }

            if (!HaveTheSameNumberOfPoints(surfaceCurves))
            {
                throw new ArgumentException(
                    "To create a surface the provided surface curves must have the same number of points.");
            }

            if (areLaterals)
            {
                _laterals.AddRange(surfaceCurves);
                var perpendiculars = CreatePerpendiculars(_laterals);
                _longitudinals.AddRange(perpendiculars);
            }
            else
            {
                _longitudinals.AddRange(surfaceCurves);
                var perpendiculars = CreatePerpendiculars(_longitudinals);
                _laterals.AddRange(perpendiculars);
            }
        }

        /// <summary>
        /// Creates a surface from the laterals and longitudinals provided.
        /// </summary>
        /// <param name="laterals">The lateral curves.</param>
        /// <param name="longitudinals">The longitudinal curves.</param>
        /// <exception cref="System.ArgumentException">Thrown when the provided surface curves don't have the same number of points.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the coincident nodes constraint of a network is not met by the provided laterals and longitudinals.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the number of laterals is below 2.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the number of longitudinals is below 2.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when laterals or longitudinals are null.</exception>
        public Surface(List<Spline> laterals, List<Spline> longitudinals) : this()
        {
            if (laterals == null)
            {
                throw new ArgumentNullException("Surface curves cannot be null.");
            }

            if (longitudinals == null)
            {
                throw new ArgumentNullException("Surface curves cannot be null.");
            }

            if (laterals.Count < 2)
            {
                throw new ArgumentException("To create a surface at least two laterals are needed.");
            }

            if (longitudinals.Count < 2)
            {
                throw new ArgumentException("To create a surface at least two longitudinals are needed.");
            }

            if (!HaveTheSameNumberOfPoints(laterals, longitudinals))
            {
                throw new ArgumentException("To create a surface the surface curves must have the same number of points.");
            }

            if (!AreLateralPointsPositionsCoincidentWithLongitudinalPointsPositions(laterals, longitudinals))
            {
                throw new ArgumentException(
                    "Coincident nodes constraint of a network is not met by the provided laterals and longitudinals.");
            }

            _laterals.AddRange(laterals);
            _longitudinals.AddRange(longitudinals);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of laterals.
        /// </summary>
        public List<Spline> Laterals => _laterals;

        /// <summary>
        /// Gets the list of longitudinals.
        /// </summary>
        public List<Spline> Longitudinals => _longitudinals;

        #endregion

        #region Operations

        /// <summary>
        /// Gets the normal of the Surface at the specified point.
        /// </summary>
        /// <param name="lateralIndex">Lateral index of point on surface.</param>
        /// <param name="longitudinalIndex">Longitudinal index of point on surface.</param>
        /// <returns>The normal vector at the specified point. </returns>
        public Vector GetNormal(int lateralIndex, int longitudinalIndex)
        {
            var lateralVector = _laterals[lateralIndex][longitudinalIndex].DirectionAfter != new Vector(0,0,0) ? _laterals[lateralIndex][longitudinalIndex].DirectionAfter : _laterals[lateralIndex][longitudinalIndex].DirectionBefore;
            var longitudinalVector = _longitudinals[longitudinalIndex][lateralIndex].DirectionAfter != new Vector(0,0,0) ? _longitudinals[longitudinalIndex][lateralIndex].DirectionAfter : _longitudinals[longitudinalIndex][lateralIndex].DirectionBefore;
            var normal = Vector.CrossProduct(longitudinalVector, lateralVector);
            normal.Normalize();

            return normal;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Checks if the provided surface curves have the same number of curves.
        /// </summary>
        /// <param name="laterals">The lateral curves.</param>
        /// <param name="longitudinals">The longitudinal curves.</param>
        /// <returns>True if the number of points is the same, false otherwise.</returns>
        private bool HaveTheSameNumberOfPoints(List<Spline> laterals, List<Spline> longitudinals)
        {
            if (!HaveTheSameNumberOfPoints(laterals))
            {
                return false;
            }

            if (!HaveTheSameNumberOfPoints(longitudinals))
            {
                return false;
            }

            if (laterals.Count != longitudinals[0].Count)
            {
                return false;
            }

            if (laterals[0].Count != longitudinals.Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the provided surface curves have the same number of curves.
        /// </summary>
        /// <param name="surfaceCurves">The surface curves.</param>
        /// <returns>True if the number of points is the same, false otherwise.</returns>
        private bool HaveTheSameNumberOfPoints(List<Spline> surfaceCurves)
        {
            if (surfaceCurves.GroupBy(x => x.Count()).Count() > 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the points for laterals are coincident with the longitudinal ones.
        /// </summary>
        /// <param name="laterals">The lateral curves.</param>
        /// <param name="longitudinals">The longitudinal curves.</param>
        /// <returns>True if points are coincident, false otherwise.</returns>
        private bool AreLateralPointsPositionsCoincidentWithLongitudinalPointsPositions(
            List<Spline> laterals,
            List<Spline> longitudinals)
        {
            for (int i = 0; i < laterals.Count; i++)
            {
                for (int j = 0; j < longitudinals.Count; j++)
                {
                    if (longitudinals[j][i].X != laterals[i][j].X ||
                        longitudinals[j][i].Y != laterals[i][j].Y ||
                        longitudinals[j][i].Z != laterals[i][j].Z)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates the perpendicular curves to surface curves.
        /// </summary>
        /// <param name="curves"></param>
        /// <returns> The perpendicular curves.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the provided surface curves don't have the same number of points.</exception>
        private List<Spline> CreatePerpendiculars(List<Spline> curves)
        {
            if (!HaveTheSameNumberOfPoints(curves))
            {
                throw new ArgumentException("Surface curves must have the same number of points.");
            }

            var numberOfPoints = curves.First().Count;
            if (curves.First().IsClosed)
            {
                // When a curve is closed an extra point is added to the end of the spline that is the clone of the 1st point. 
                // The number of perpendiculars curves that we want to create should be equal to the number of points of the curve. 
                // For a open curve the spline has the same number of points, but for a closed curve it has an additional one.
                numberOfPoints -= 1;
            }

            var perpendiculars = new List<Spline>();
            for (int i = 0; i <= numberOfPoints - 1; i++)
            {
                var perpendicularPoints = new List<Point>();
                for (int j = 0; j <= curves.Count - 1; j++)
                {
                    SplinePoint mySplinePoint = curves[j][i];
                    SplinePoint newSplinePoint = new SplinePoint(new Point(mySplinePoint.X, mySplinePoint.Y, mySplinePoint.Z));
                    newSplinePoint.DirectionAfter = mySplinePoint.DirectionAfter;
                    newSplinePoint.DirectionBefore = mySplinePoint.DirectionBefore;
                    newSplinePoint.MagnitudeAfter = mySplinePoint.MagnitudeAfter;
                    newSplinePoint.MagnitudeBefore = mySplinePoint.MagnitudeBefore;
                    perpendicularPoints.Add(newSplinePoint);
                }

                var perpendicular = new Spline(perpendicularPoints);
                perpendicular.FreeTangentsAndMagnitudes();
                perpendiculars.Add(perpendicular);
            }

            return perpendiculars;
        }

        #endregion
    }
}