// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.FileSystem;
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
    public class CurveTest : GenericCurveTest<PSCurve>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Curves;
                _genericCurvesCollection = (PSGenericCurvesCollection<PSCurve>) _powerSHAPECollection;
            }
            catch (Exception e)
            {
                Assert.Fail("_entityCollection could not be set to Active Model's CurvesCollection\n\n" + e.Message);
            }
        }

        #region Curve Tests

        #region Properties

        /// <summary>
        /// Test for Type
        /// </summary>
        [Test]
        public void CurveTypeTest()
        {
            // Get bezier curve
            var bezierCurve = (PSCurve) ImportAndGetEntity(TestFiles.SINGLE_CURVE);

            // Check type
            Assert.AreEqual(CurveTypes.Bezier, bezierCurve.Type, "Did not return bezier type");

            // Delete curve
            bezierCurve.Delete();

            // Get g2 curve
            //PSCurve g2Curve = (PSCurve)ImportAndGetEntity(TestFiles.SINGLE_CURVE_G2);

            //// Check type
            //Assert.AreEqual(PSCurve.CurveTypes.g2, g2Curve.Type, "Did not return g2 type");

            //// Delete curve
            //g2Curve.Delete();
            // TODO: PowerSHAPE does not currently return curve type g2

            // Get bezier curve
            var bSplineCurve = (PSCurve) ImportAndGetEntity(TestFiles.SINGLE_CURVE_BSPLINE);

            // Check type
            Assert.AreEqual(CurveTypes.bSpline, bSplineCurve.Type, "Did not return b-spline type");
        }

        #endregion

        #endregion

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void CurveIdTest()
        {
            IdTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void CurveIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_CURVE, "CURVE");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void CurveEqualsTest()
        {
            Equals(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void CurveExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_CURVE);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddCurveToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void BlankCurveTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void CurveBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_CURVE, new Point(-137.5, 25.285373, 0), new Point(-102, 78.099911, 0));
        }

        [Test]
        public void CopyCurveTest()
        {
            DuplicateTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void DeleteCurveTest()
        {
            DeleteTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void CurveLevelTest()
        {
            LevelTest(TestFiles.SINGLE_CURVE, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void CurveLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_CURVE, 1);
        }

        [Test]
        public void LimitCurveWithEntitiesTest()
        {
            LimitToEntitiesTest(TestFiles.SINGLE_CURVE, TestFiles.CURVE_LIMITERS, LimitingModes.IntersectCurveMode);
        }

        [Test]
        public void OffsetCurveTest()
        {
            OffsetTest(TestFiles.CURVE_TO_OFFSET, new BoundingBox(-99.868817, -68.230014, 37.095345, 65.389255, 0, 0));
        }

        [Test]
        public void MirrorCurveTest()
        {
            // Setup parameters
            var mirrorPoint = new Point(-102, 73.5, 0);
            var initialEndPoint = new Point(-137.5, 26, 0);

            // Carry out operation
            MirrorTest(TestFiles.SINGLE_CURVE, mirrorPoint);

            // Get point that should now be on mirror point
            var mirroredCurve = _powerSHAPE.ActiveModel.Curves[0];
            Assert.AreEqual(mirrorPoint, mirroredCurve.StartPoint, "Curve was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((mirrorPoint.X - initialEndPoint.X) * 2.0,
                            mirroredCurve.EndPoint.X - initialEndPoint.X,
                            "Curve was not mirrored in correct plane");
        }

        [Test]
        public void MoveCurveTest()
        {
            MoveTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void MoveCurveCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void MoveCurveDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_CURVE, "CentreOfGravity");
        }

        [Test]
        public void CurveNameTest()
        {
            NameTest(TestFiles.SINGLE_CURVE, "SingleCurve", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("curve[NewName].EXISTS"));
        }

        [Test]
        public void RemoveCurveFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void RotateCurveTest()
        {
            RotateTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void RotateCurveCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// Test the distance between two curves
        /// </summary>
        [Test]
        public void MinimumDistanceCurveCurve()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Curves[0];
            var entity2 = _powerSHAPE.ActiveModel.Curves[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 8.12466236356738);
        }

        /// <summary>
        /// Test the distance between a curve and a line
        /// </summary>
        [Test]
        public void MinimumDistanceCurveLine()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Curves[0];
            var entity2 = _powerSHAPE.ActiveModel.Lines[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 27.1941782323838);
        }

        #endregion

        #region Wireframe Tests

        #region Properties

        /// <summary>
        /// A test for Centre of Gravity
        /// </summary>
        [Test]
        public void CurveCentreOfGravityTest()
        {
            // Check centre of gravity
            CentreOfGravityTest(TestFiles.SINGLE_CURVE, new Point(-122.294347, 53.006734, 0));
        }

        /// <summary>
        /// A test for Length
        /// </summary>
        [Test]
        public void CurveLengthTest()
        {
            LengthTest(TestFiles.SINGLE_CURVE, 95.495200);
        }

        /// <summary>
        /// A test for Points
        /// </summary>
        [Test]
        public void CurvePointsTest()
        {
            PointsTest(TestFiles.SINGLE_CURVE, 5);
        }

        /// <summary>
        /// A test for StartPoint
        /// </summary>
        [Test]
        public void CurveStartPointTest()
        {
            // Check start point
            StartPointTest(TestFiles.SINGLE_CURVE, new Point(-102, 73.5, 0));
        }

        /// <summary>
        /// A test for EndPoint
        /// </summary>
        [Test]
        public void CurveEndPointTest()
        {
            // Check end point
            EndPointTest(TestFiles.SINGLE_CURVE, new Point(-137.5, 26, 0));
        }

        /// <summary>
        /// Test for NumberPoints
        /// </summary>
        [Test]
        public void CurveNumberPointsTest()
        {
            // Get curve
            var curve = (PSCurve) ImportAndGetEntity(TestFiles.SINGLE_CURVE);

            // Check number of points
            Assert.AreEqual(5, curve.NumberPoints, "Returned incorrect number of points");
        }

        #endregion

        #endregion

        #region GenericCurve Tests

        #region Properties

        /// <summary>
        /// A test for Area
        /// </summary>
        [Test]
        public void CurveAreaTest()
        {
            // Check area
            AreaTest(TestFiles.SINGLE_CURVE, 405.464119);
        }

        /// <summary>
        /// A test for IsClosed
        /// </summary>
        [Test]
        public void CurveIsClosedTest()
        {
            // Check IsClosed property
            IsClosedTest(TestFiles.SINGLE_CURVE);
        }

        #endregion

        #region Operations

        /// <summary>
        /// A test for DeletePoints
        /// </summary>
        [Test]
        public void CurveDeletePointsTest()
        {
            DeletePointsTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for Extend
        /// </summary>
        [Test]
        public void CurveExtendTest()
        {
            if (_powerSHAPE.Version > new Version("15.1"))
            {
                ExtendTest(TestFiles.SINGLE_CURVE,
                           new Point(-145.845138, 31.272691, 0),
                           new Point(-152.505482, 38.731901, 0));
            }
            else
            {
                ExtendTest(TestFiles.SINGLE_CURVE,
                           new Point(-145.876438, 31.411324, 0),
                           new Point(-152.323192, 39.055888, 0));
            }
        }

        /// <summary>
        /// Test for moving a point on a curve
        /// </summary>
        [Test]
        public void EditCurvePointTest()
        {
            MoveCurvePointTest(TestFiles.CURVE_TO_FREE);
        }

        /// <summary>
        /// A test for FreeTangentsAndMagnitudes
        /// </summary>
        [Test]
        public void FreeCurveSelectedMagnitudesDirectionsTest()
        {
            FreeSelectedMagnitudesDirectionsTest(TestFiles.CURVE_TO_FREE, 23.7);
        }

        /// <summary>
        /// A test for FreeTangentsAndMagnitudes
        /// </summary>
        [Test]
        public void FreeCurveAllMagnitudesDirectionsTest()
        {
            FreeAllMagnitudesDirectionsTest(TestFiles.CURVE_TO_FREE, 23.7);
        }

        /// <summary>
        /// A test for GetLengthBetweenPoints
        /// </summary>
        [Test]
        public void GetLengthBetweenCurvePointsTest()
        {
            GetLengthBetweenPointsTest(TestFiles.SINGLE_CURVE, 24.088779);
        }

        /// <summary>
        /// A test for GetPointEntryAzimuthAngle
        /// </summary>
        [Test]
        public void GetCurvePointEntryAzimuthAngleTest()
        {
            GetPointEntryAzimuthAngleTest(TestFiles.CURVE_TO_FREE, 273.204018);
        }

        /// <summary>
        /// A test for GetPointEntryElevationAngle
        /// </summary>
        [Test]
        public void GetCurvePointEntryElevationAngleTest()
        {
            GetPointEntryElevationAngleTest(TestFiles.CURVE_TO_FREE, 0);
        }

        /// <summary>
        /// A test for GetPointEntryMagnitude
        /// </summary>
        [Test]
        public void GetCurvePointEntryMagnitudeTest()
        {
            GetPointEntryMagnitudeTest(TestFiles.CURVE_TO_FREE, 39.359826);
        }

        /// <summary>
        /// A test for GetPointEntryTangent
        /// </summary>
        [Test]
        public void GetCurvePointEntryTangentTest()
        {
            GetPointEntryTangentTest(TestFiles.CURVE_TO_FREE, new Vector(0.055892, -0.998437, 0));
        }

        /// <summary>
        /// A test for GetPointExitAzimuthAngle
        /// </summary>
        [Test]
        public void GetCurvePointExitAzimuthAngleTest()
        {
            GetPointExitAzimuthAngleTest(TestFiles.CURVE_TO_FREE, 340.709954);
        }

        /// <summary>
        /// A test for GetPointExitElevationAngle
        /// </summary>
        [Test]
        public void GetCurvePointExitElevationAngleTest()
        {
            GetPointExitElevationAngleTest(TestFiles.CURVE_TO_FREE, 0);
        }

        /// <summary>
        /// A test for GetPointExitMagnitude
        /// </summary>
        [Test]
        public void GetCurvePointExitMagnitudeTest()
        {
            GetPointExitMagnitudeTest(TestFiles.CURVE_TO_FREE, 63.568860);
        }

        /// <summary>
        /// A test for GetPointExitTangent
        /// </summary>
        [Test]
        public void GetCurvePointExitTangentTest()
        {
            GetPointExitTangentTest(TestFiles.CURVE_TO_FREE, new Vector(0.943858, -0.330350, 0));
        }

        /// <summary>
        /// A test for MergeAndSpline
        /// </summary>
        [Test]
        public void MergeAndSplineCurveTest()
        {
            MergeAndSplineTest(TestFiles.CURVE_TO_FREE);
        }

        /// <summary>
        /// A test for RenumberPoints
        /// </summary>
        [Test]
        public void RenumberCurvePointsTest()
        {
            RenumberPointsTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for Repoint
        /// </summary>
        [Test]
        public void RepointCurveTest()
        {
            RepointTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for Reverse
        /// </summary>
        [Test]
        public void ReverseCurveTest()
        {
            ReverseTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for Reverse
        /// </summary>
        [Test]
        public void ScaleCurveTest()
        {
            ScaleTest(TestFiles.SINGLE_CURVE);
        }

        /// <summary>
        /// A test for ProjectOntoSurface
        /// </summary>
        [Test]
        public void ProjectCurveOntoSurfaceTest()
        {
            ProjectOntoSurfaceTest(TestFiles.CURVE_TO_PROJECT,
                                   TestFiles.SURFACE_TO_WRAP_ONTO,
                                   new Point(-75, 55.42, 40),
                                   new Point(-67.250588, 38.936667, 38.437641));
        }

        /// <summary>
        /// A test for ProjectOntoPlane using the Z axis
        /// </summary>
        [Test]
        public void ProjectOntoPlaneZ()
        {
            ProjectOntoPlaneTest(TestFiles.CURVE_TO_PROJECT, Planes.XY, 10.3);
        }

        /// <summary>
        /// A test for ProjectOntoPlane using the X axis
        /// </summary>
        [Test]
        public void ProjectOntoPlaneX()
        {
            ProjectOntoPlaneTest(TestFiles.CURVE_TO_PROJECT, Planes.YZ, 9.2);
        }

        /// <summary>
        /// A test for ProjectOntoPlane using the Y axis
        /// </summary>
        [Test]
        public void ProjectOntoPlaneY()
        {
            ProjectOntoPlaneTest(TestFiles.CURVE_TO_PROJECT, Planes.ZX, 12.6);
        }

        [Test]
        public void InsertPointBetweenPointsTest_Curve()
        {
            InsertPointBetweenPointsTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void InsertPointRelativeToCurvePointTest_Curve()
        {
            InsertPointRelativeToCurvePointTest(TestFiles.SINGLE_CURVE);
        }

        [Test]
        public void InsertPointByProximityTest_Curve()
        {
            var curve = (PSCurve) ImportAndGetEntity(TestFiles.SINGLE_CURVE);

            var numPointsBefore = curve.NumberPoints;
            curve.InsertPointByProximity(new Point(-40, 50, 0));

            // Test that a point has been added
            Assert.AreEqual(curve.NumberPoints, numPointsBefore + 1, "Expected a point to be added.");
        }

        [Test]
        public void InsertPointsAtPlaneIntersections_Curve()
        {
            var curve = (PSCurve) ImportAndGetEntity(TestFiles.SINGLE_CURVE);

            var numPointsBefore = curve.NumberPoints;

            var firstPoint = curve.Points[0].Clone();
            var secondPoint = curve.Points[1].Clone();
            var thirdPoint = curve.Points[2].Clone();
            var fourthPoint = curve.Points[3].Clone();
            var fifthPoint = curve.Points[4].Clone();
            curve.InsertPointsAtPlaneIntersections(Planes.YZ, -130);

            // Test that 3 points have been added
            Assert.AreEqual(curve.NumberPoints, numPointsBefore + 3, "Expected a point to be added.");

            // Test that the first has been inserted between first and second points
            Assert.AreEqual(firstPoint.X,
                            curve.Points[0].X,
                            "Expected first new point to be inserted between first and second.");
            Assert.AreEqual(firstPoint.Y,
                            curve.Points[0].Y,
                            "Expected first new point to be inserted between first and second.");
            Assert.AreEqual(firstPoint.Z,
                            curve.Points[0].Z,
                            "Expected first new point to be inserted between first and second.");
            Assert.AreEqual(secondPoint.X,
                            curve.Points[2].X,
                            "Expected first new point to be inserted between first and second.");
            Assert.AreEqual(secondPoint.Y,
                            curve.Points[2].Y,
                            "Expected first new point to be inserted between first and second.");
            Assert.AreEqual(secondPoint.Z,
                            curve.Points[2].Z,
                            "Expected first new point to be inserted between first and second.");

            // Test that the second has been inserted between second and third points
            Assert.AreEqual(secondPoint.X,
                            curve.Points[2].X,
                            "Expected second new point to be inserted between second and third.");
            Assert.AreEqual(secondPoint.Y,
                            curve.Points[2].Y,
                            "Expected second new point to be inserted between second and third.");
            Assert.AreEqual(secondPoint.Z,
                            curve.Points[2].Z,
                            "Expected second new point to be inserted between second and third.");
            Assert.AreEqual(thirdPoint.X,
                            curve.Points[4].X,
                            "Expected second new point to be inserted between second and third.");
            Assert.AreEqual(thirdPoint.Y,
                            curve.Points[4].Y,
                            "Expected second new point to be inserted between second and third.");
            Assert.AreEqual(thirdPoint.Z,
                            curve.Points[4].Z,
                            "Expected second new point to be inserted between second and third.");

            // Test that the third has been inserted between fourth and fifth points
            Assert.AreEqual(fourthPoint.X,
                            curve.Points[5].X,
                            "Expected third new point to be inserted between fourth and fifth.");
            Assert.AreEqual(fourthPoint.Y,
                            curve.Points[5].Y,
                            "Expected third new point to be inserted between fourth and fifth.");
            Assert.AreEqual(fourthPoint.Z,
                            curve.Points[5].Z,
                            "Expected third new point to be inserted between fourth and fifth.");
            Assert.AreEqual(fifthPoint.X,
                            curve.Points[7].X,
                            "Expected third new point to be inserted between fourth and fifth.");
            Assert.AreEqual(fifthPoint.Y,
                            curve.Points[7].Y,
                            "Expected third new point to be inserted between fourth and fifth.");
            Assert.AreEqual(fifthPoint.Z,
                            curve.Points[7].Z,
                            "Expected third new point to be inserted between fourth and fifth.");
        }

        #endregion

        #endregion
    }
}