// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for ArcTest and is intended
    /// to contain all ArcTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class GenericCurveTest<T> : WireframeTest<T> where T : PSGenericCurve
    {
        #region Fields

        protected PSGenericCurvesCollection<T> _genericCurvesCollection;

        #endregion

        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            _genericCurvesCollection = (PSGenericCurvesCollection<T>) _powerSHAPECollection;
        }

        #region GenericCurve Tests

        #region Properties

        /// <summary>
        /// Test for Area
        /// </summary>
        public void AreaTest(string genericCurveFile, double expectedArea)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check area
            Assert.AreEqual(expectedArea, genericCurve.Area, "Area is not as expected");
        }

        /// <summary>
        /// Test for EndPoint
        /// </summary>
        public void EndPointTest(string genericCurveFile, Point expectedEndPoint)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check end point
            Assert.AreEqual(expectedEndPoint, genericCurve.EndPoint, "EndPoint is not as expected");
        }

        /// <summary>
        /// Test for IsClosed
        /// </summary>
        public void IsClosedTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check that curve is defined as being open
            Assert.IsFalse(genericCurve.IsClosed, "Curve incorrectly indicated as being open");

            // Close curve
            genericCurve.IsClosed = true;

            // Check that curve is defined as being closed
            Assert.IsTrue(genericCurve.IsClosed, "Curve incorrectly indicated as being closed");
        }

        /// <summary>
        /// Test for StartPoint
        /// </summary>
        public void StartPointTest(string genericCurveFile, Point expectedStartPoint)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check start point
            Assert.AreEqual(expectedStartPoint, genericCurve.StartPoint, "StartPoint is not as expected");
        }

        #endregion

        #region Operations

        /// <summary>
        /// Test for DeletePoints
        /// </summary>
        public void DeletePointsTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Get initial start and end points
            var initialStartPoint = genericCurve.StartPoint;
            var initialEndPoint = genericCurve.EndPoint;

            // Delete points
            genericCurve.DeleteCurvePoints(new[] {1, 2, 3});

            // Check remaining points
            Assert.AreEqual(2, genericCurve.NumberPoints, "Incorrect number of points remaining");
            Assert.AreEqual(initialStartPoint, genericCurve.StartPoint, "Start point has changed");
            Assert.AreEqual(initialEndPoint, genericCurve.EndPoint, "End point has changed");
        }

        /// <summary>
        /// Test for Extend
        /// </summary>
        public void ExtendTest(
            string genericCurveFile,
            Point expectedCurvedExtensionEnd,
            Point expectedLinearExtensionEnd)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Get initial length
            var initialLength = genericCurve.Length.Value;

            // Check curved curve extension
            genericCurve.Extend(ExtensionEnd.TWO, 10F, ExtensionType.Curvature);

            // Check remaining points
            Assert.IsTrue(genericCurve.Length > initialLength, "Curvature extension failed");
            Assert.AreEqual(expectedCurvedExtensionEnd,
                            genericCurve.EndPoint,
                            "Curvature extension extended to the incorrect point");

            // Update initial length
            initialLength = genericCurve.Length.Value;

            // Check straight curve extension
            genericCurve.Extend(ExtensionEnd.TWO, 10F, ExtensionType.Linear);

            // Check remaining points
            Assert.IsTrue(genericCurve.Length > initialLength, "Straight extension failed");
            Assert.AreEqual(expectedLinearExtensionEnd,
                            genericCurve.EndPoint,
                            "Linear extension extended to the incorrect point");
        }

        /// <summary>
        /// Test for FreeTangentsMagnitudes
        /// </summary>
        public void FreeSelectedMagnitudesDirectionsTest(string genericCurveFile, double expectedPoint2)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);
            var initialPoint2ExitMagnitude = genericCurve.GetExitMagnitudeOfPoint(1);
            var initialPoint4ExitMagnitude = genericCurve.GetExitMagnitudeOfPoint(3);

            // Free select tangents/magnitudes
            genericCurve.FreeMagnitudesDirections(new[] {1, 2});

            // Check Point 2 tangency
            Assert.AreNotEqual(initialPoint2ExitMagnitude,
                               genericCurve.GetExitMagnitudeOfPoint(1),
                               "The operation has not been carried out on Point 2");
            Assert.AreEqual(Math.Round(expectedPoint2, 0),
                            Math.Round(genericCurve.GetExitMagnitudeOfPoint(1), 0),
                            "Point 2 was not altered correctly");
            Assert.AreEqual(initialPoint4ExitMagnitude,
                            genericCurve.GetExitMagnitudeOfPoint(3),
                            "The operation has not been limited to the desired points");
        }

        /// <summary>
        /// Test for FreeTangentsAndMagnitudes
        /// </summary>
        public void FreeAllMagnitudesDirectionsTest(string genericCurveFile, double expectedPoint2)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);
            var initialPoint2ExitMagnitude = genericCurve.GetExitMagnitudeOfPoint(1);

            // Free select tangents/magnitudes
            genericCurve.FreeMagnitudesAndDirections();

            // Check Point 2 tangency
            Assert.AreNotEqual(initialPoint2ExitMagnitude,
                               genericCurve.GetExitMagnitudeOfPoint(1),
                               "The operation has not been carried out");
            Assert.AreEqual(Math.Round(expectedPoint2, 1),
                            Math.Round(genericCurve.GetExitMagnitudeOfPoint(1), 1),
                            "Point 2 was not altered correctly");
        }

        /// <summary>
        /// Test for GetLengthBetweenPoints
        /// </summary>
        public void GetLengthBetweenPointsTest(string genericCurveFile, double expectedLength)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check distance
            Assert.AreEqual(expectedLength,
                            Math.Round(genericCurve.GetLengthBetweenTwoPoints(1, 2).Value, 6),
                            "The returned length is incorrect");
        }

        /// <summary>
        /// Test for GetPointEntryAzimuthAngle
        /// </summary>
        public void GetPointEntryAzimuthAngleTest(string genericCurveFile, double expectedAngle)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check angle
            Assert.AreEqual(expectedAngle,
                            Math.Round(genericCurve.GetEntryAzimuthAngleOfPoint(1), 6),
                            "The returned entry azimuth angle is incorrect");
        }

        /// <summary>
        /// Test for GetPointEntryElevationAngle
        /// </summary>
        public void GetPointEntryElevationAngleTest(string genericCurveFile, double expectedAngle)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check angle
            Assert.AreEqual(expectedAngle,
                            Math.Round(genericCurve.GetEntryElevationAngleOfPoint(1), 6),
                            "The returned entry azimuth angle is incorrect");
        }

        /// <summary>
        /// Test for GetPointEntryMagnitude
        /// </summary>
        public void GetPointEntryMagnitudeTest(string genericCurveFile, double expectedMagnitude)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check magnitude
            Assert.IsTrue(expectedMagnitude == Math.Round(genericCurve.GetEntryMagnitudeOfPoint(1), 6),
                          "The returned entry magnitude is incorrect");
        }

        /// <summary>
        /// Test for GetPointEntryTangent
        /// </summary>
        public void GetPointEntryTangentTest(string genericCurveFile, Vector expectedTangent)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check tangent
            Assert.AreEqual(expectedTangent,
                            genericCurve.GetEntryTangentOfPoint(1),
                            "The returned entry tangent is incorrect");
        }

        /// <summary>
        /// Test for GetPointExitAzimuthAngle
        /// </summary>
        public void GetPointExitAzimuthAngleTest(string genericCurveFile, double expectedAngle)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check angle
            Assert.AreEqual(expectedAngle,
                            Math.Round(genericCurve.GetExitAzimuthAngleOfPoint(1), 6),
                            "The returned exit azimuth angle is incorrect");
        }

        /// <summary>
        /// Test for GetPointExitElevationAngle
        /// </summary>
        public void GetPointExitElevationAngleTest(string genericCurveFile, double expectedAngle)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check angle
            Assert.AreEqual(expectedAngle,
                            Math.Round(genericCurve.GetExitElevationAngleOfPoint(2), 6),
                            "The returned exit azimuth angle is incorrect");
        }

        /// <summary>
        /// Test for GetPointExitMagnitude
        /// </summary>
        public void GetPointExitMagnitudeTest(string genericCurveFile, double expectedMagnitude)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check magnitude
            Assert.AreEqual(expectedMagnitude,
                            Math.Round(genericCurve.GetExitMagnitudeOfPoint(1), 6),
                            "The returned exit magnitude is incorrect");
        }

        /// <summary>
        /// Test for GetPointExitTangent
        /// </summary>
        public void GetPointExitTangentTest(string genericCurveFile, Vector expectedTangent)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Check tangent
            Assert.AreEqual(expectedTangent,
                            genericCurve.GetExitTangentOfPoint(1),
                            "The returned exit tangent is incorrect");
        }

        /// <summary>
        /// Test for MergeAndSpline
        /// </summary>
        public void MergeAndSplineTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Get initial number of points
            var initialNumberPoints = genericCurve.NumberPoints;

            // Merge and spline curve
            genericCurve.MergeAndSpline();

            // Check resultant number of points
            Assert.AreNotEqual(initialNumberPoints,
                               genericCurve.NumberPoints,
                               "The merge/spline operation was not carried out on the curve");
        }

        public void MoveCurvePointTest(string genericCurveFile)
        {
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            var initialStartPoint = genericCurve.Point(0);

            var dir = new Vector(10.0, 20.0, 30.0);

            genericCurve.EditPositionOfPointRelative(0, dir);

            var newPoint = genericCurve.Point(0);

            var nDir = newPoint - initialStartPoint;

            Assert.AreEqual(dir, nDir, "Vector direction not the same");
        }

        /// <summary>
        /// Test for RenumberPoints
        /// </summary>
        public void RenumberPointsTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Close curve
            genericCurve.IsClosed = true;

            // Get initial position of start point
            var initialStartPoint = genericCurve.StartPoint;

            // Renumber points
            genericCurve.RenumberCurvePoints(0);

            // Check resultant start point
            Assert.AreEqual(initialStartPoint, genericCurve.StartPoint, "The start point has not been changed");
        }

        /// <summary>
        /// Test for Repoint
        /// </summary>
        public void RepointTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Repoint curve
            genericCurve.RepointCurveBetweenPoints(0, 2, 5, CornerBehaviours.KeepAllDiscontinuities);

            // Check resultant number of points
            Assert.AreEqual(7, genericCurve.NumberPoints, "Curve was not repointed");
        }

        /// <summary>
        /// Test for toleranced Repoint
        /// </summary>
        public void RepointTolerancedTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve)ImportAndGetEntity(genericCurveFile);

            // Repoint curve
            genericCurve.RepointCurveBetweenPoints(0, 4, 0.1f, CornerBehaviours.KeepAllDiscontinuities);

            // Check resultant number of points
            Assert.AreEqual(13, genericCurve.NumberPoints, "Curve was not repointed");
        }

        /// <summary>
        /// Test for Reverse
        /// </summary>
        public void ReverseTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Get initial curve endpoint
            var initialEndPoint = genericCurve.EndPoint;

            // Reverse curve
            genericCurve.Reverse();

            // Check that the start and end point have been switched
            Assert.AreEqual(initialEndPoint, genericCurve.StartPoint, "Curve was not reversed");
        }

        /// <summary>
        /// Test for Scale
        /// </summary>
        public void ScaleTest(string genericCurveFile)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            //Setup parameters
            var scaleVector = new Vector(2, 2, 0);
            var expectedStartPoint = new Point(genericCurve.StartPoint.X * scaleVector.I,
                                               genericCurve.StartPoint.Y * scaleVector.J,
                                               genericCurve.StartPoint.Z * scaleVector.K);

            // Scale curve
            genericCurve.Scale(scaleVector.I.Value, scaleVector.J.Value, scaleVector.K.Value, new Point());

            // Check that the start and end point have been switched
            Assert.AreEqual(expectedStartPoint, genericCurve.StartPoint, "Curve was not scaled correctly");
        }

        /// <summary>
        /// Test for ProjectOntoSurface
        /// </summary>
        public void ProjectOntoSurfaceTest(
            string genericCurveFile,
            string surfaceFile,
            Point expectedStartPoint,
            Point expectedEndPoint)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Get surface
            var surfaceToProjectOnto = (PSSurface) ImportAndGetEntity(surfaceFile);

            // Setup principal axis
            _powerSHAPE.SetActivePlane(Planes.XY);

            // Project curve
            var projections = genericCurve.ProjectOntoSurface(surfaceToProjectOnto,
                                                              ProjectionType.AlongPrincipalAxis,
                                                              false);

            // Check the new start and end points
            Assert.IsTrue(projections != null && projections.Count == 1, "Curve was not projected correctly");
            Assert.IsTrue(expectedStartPoint.Equals(projections[0].StartPoint, 2), "Curve was not projected correctly");
            Assert.IsTrue(expectedEndPoint.Equals(projections[0].EndPoint, 2), "Curve was not projected correctly");
        }

        /// <summary>
        /// Test for ProjectOntoPlane
        /// </summary>
        public void ProjectOntoPlaneTest(string genericCurveFile, Planes plane, MM position)
        {
            // Get curve
            var genericCurve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            // Project curve
            genericCurve.ProjectOntoPlane(plane, position);

            // Check the curve is flat in the projected plane and projected to the correction position
            switch (plane)
            {
                case Planes.XY:
                    Assert.AreEqual((MM) 0.0, genericCurve.BoundingBox.ZSize);
                    Assert.AreEqual(position, genericCurve.BoundingBox.MinZ);
                    break;
                case Planes.YZ:
                    Assert.AreEqual((MM) 0.0, genericCurve.BoundingBox.XSize);
                    Assert.AreEqual(position, genericCurve.BoundingBox.MinX);
                    break;
                case Planes.ZX:
                    Assert.AreEqual((MM) 0.0, genericCurve.BoundingBox.YSize);
                    Assert.AreEqual(position, genericCurve.BoundingBox.MinY);
                    break;
            }
        }

        public void InsertPointBetweenPointsTest(string genericCurveFile)
        {
            var curve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            Assert.Throws(typeof(Exception),
                          delegate { curve.InsertPointAtPositionBetweenPoints(curve.NumberPoints - 0.9); },
                          "'Out of range' exception was expected.");

            Assert.Throws(typeof(Exception),
                          delegate { curve.InsertPointAtPositionBetweenPoints(-0.1); },
                          "'Out of range' exception was expected.");

            var numPointsBefore = curve.NumberPoints;
            var secondPoint = curve.Points[1].Clone();
            var thirdPoint = curve.Points[2].Clone();
            curve.InsertPointAtPositionBetweenPoints(1.5);

            // Test that a point has been added
            Assert.AreEqual(curve.NumberPoints, numPointsBefore + 1, "Expected a point to be added.");

            // Test that it has been inserted between first and second points
            Assert.AreEqual(secondPoint.X, curve.Points[1].X, "Expected second point to be unchanged.");
            Assert.AreEqual(secondPoint.Y, curve.Points[1].Y, "Expected second point to be unchanged.");
            Assert.AreEqual(secondPoint.Z, curve.Points[1].Z, "Expected second point to be unchanged.");
            Assert.AreEqual(thirdPoint.X, curve.Points[3].X, "Expected new point to be inserted before third.");
            Assert.AreEqual(thirdPoint.Y, curve.Points[3].Y, "Expected new point to be inserted before third.");
            Assert.AreEqual(thirdPoint.Z, curve.Points[3].Z, "Expected new point to be inserted before third.");
        }

        public void InsertPointByProximityTest(string genericCurveFile)
        {
        }

        public void InsertPointRelativeToCurvePointTest(string genericCurveFile)
        {
            var curve = (PSGenericCurve) ImportAndGetEntity(genericCurveFile);

            Assert.Throws(typeof(Exception),
                          delegate { curve.InsertPointRelativeToPointOnCurve(-1, 0.5); },
                          "'Out of range' exception was expected.");

            Assert.Throws(typeof(Exception),
                          delegate { curve.InsertPointRelativeToPointOnCurve(curve.NumberPoints, 0.5); },
                          "'Out of range' exception was expected.");

            var numPointsBefore = curve.NumberPoints;
            var secondPoint = curve.Points[1].Clone();
            var thirdPoint = curve.Points[2].Clone();
            curve.InsertPointRelativeToPointOnCurve(1, 5.0);

            // Test that a point has been added
            Assert.AreEqual(curve.NumberPoints, numPointsBefore + 1, "Expected a point to be added.");

            // Test that it has been inserted between first and second points
            Assert.AreEqual(secondPoint.X,
                            curve.Points[1].X,
                            "Expected second point to be inserted between second and third.");
            Assert.AreEqual(secondPoint.Y,
                            curve.Points[1].Y,
                            "Expected second point to be inserted between second and third.");
            Assert.AreEqual(secondPoint.Z,
                            curve.Points[1].Z,
                            "Expected second point to be inserted between second and third.");
            Assert.AreEqual(thirdPoint.X,
                            curve.Points[3].X,
                            "Expected second point to be inserted between second and third.");
            Assert.AreEqual(thirdPoint.Y,
                            curve.Points[3].Y,
                            "Expected second point to be inserted between second and third.");
            Assert.AreEqual(thirdPoint.Z,
                            curve.Points[3].Z,
                            "Expected second point to be inserted between second and third.");
        }

        /// <summary>
        /// Test for extracting a polyline from a DUCT file
        /// </summary>
        public void ExtractPolylineFromDuctTest(string genericCurveFile)
        {
            _powerSHAPE.ArcFitPicFiles = true;
            var curve = (PSGenericCurve) ImportAndGetEntity(TestFiles.COMPCURVE_DUCT);

            // Create a polyline from the curve
            var polyline = curve.ToPolyline();

            // Check some of the points
            Assert.AreEqual(new Point("-161.245954753894 -42.8180519258823 0"),
                            polyline[50],
                            "Point not in place expected.");
            Assert.AreEqual(new Point("-152.77639 -18.802062 0"),
                            polyline[100],
                            "Point not in place expected.");
            Assert.AreEqual(new Point("-128.4143 -28.92773 0"),
                            polyline[116],
                            "Point not in place expected.");
        }

        /// <summary>
        /// Test for extracting a spline from a DUCT file
        /// </summary>
        public void ExtractSplineFromDuctTest(string genericCurveFile)
        {
            _powerSHAPE.ArcFitPicFiles = true;
            var curve = (PSGenericCurve) ImportAndGetEntity(TestFiles.COMPCURVE_DUCT);

            // Create a spline from the curve
            var spline = curve.ToSpline();

            // Check some of the points
            Assert.AreEqual(new Point("-162.8286 -28.31586 0"),
                            spline[10],
                            "Point not in place expected.");
            Assert.AreEqual(new Point("-157.6683 -18.91881 0"),
                            spline[20],
                            "Point not in place expected.");
            Assert.AreEqual(new Point("-128.4143 -28.92773 0"),
                            spline[34],
                            "Point not in place expected.");
        }

        #endregion

        #endregion
    }
}