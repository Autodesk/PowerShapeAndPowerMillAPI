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
    public class CompCurveTest : GenericCurveTest<PSCompCurve>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.CompCurves;
                _genericCurvesCollection = (PSGenericCurvesCollection<PSCompCurve>) _powerSHAPECollection;
            }
            catch (Exception e)
            {
                Assert.Fail("_entityCollection could not be set to Active Model's CompCurvesCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void CompCurveIdTest()
        {
            IdTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void CompCurveIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_COMPCURVE, "COMPCURVE");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void CompCurveEqualsTest()
        {
            Equals(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void CompCurveExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_COMPCURVE);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddCompCurveToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void BlankCompCurveTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void CompCurveBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_COMPCURVE, new Point(-66.121722, 21, 0), new Point(-22, 40.118679, 0));
        }

        [Test]
        public void CopyCompCurveTest()
        {
            DuplicateTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void DeleteCompCurveTest()
        {
            DeleteTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void CompCurveLevelTest()
        {
            LevelTest(TestFiles.SINGLE_COMPCURVE, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void CompCurveLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_COMPCURVE, 1);
        }

        [Test]
        public void LimitCompCurveWithEntitiesTest()
        {
            LimitToEntitiesTest(TestFiles.SINGLE_COMPCURVE,
                                TestFiles.COMPCURVE_LIMITERS,
                                LimitingModes.IntersectCurveMode);
        }

        [Test]
        public void LimitCompCurveToEntitiesFailureTest()
        {
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.Import(new File(TestFiles.THREE_COMPCURVES));
            var curve1 = activeModel.CompCurves[0];
            var curve2 = activeModel.CompCurves[1];
            var result = curve1.LimitToEntity(curve2);

            Assert.IsEmpty(result);
        }

        [Test]
        public void MirrorCompCurveTest()
        {
            // Setup parameters
            var mirrorPoint = new Point(-22, 34, 0);
            var initialEndPoint = new Point(-63, 21, 0);

            // Carry out operation
            MirrorTest(TestFiles.SINGLE_COMPCURVE, mirrorPoint);

            // Get point that should now be on mirror point
            var mirroredCompCurve = _powerSHAPE.ActiveModel.CompCurves[0];
            Assert.AreEqual(mirrorPoint, mirroredCompCurve.StartPoint, "CompCurve was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((mirrorPoint.X - initialEndPoint.X) * 2.0,
                            mirroredCompCurve.EndPoint.X - initialEndPoint.X,
                            "CompCurve was not mirrored in correct plane");
        }

        [Test]
        public void OffsetCompCurveTest()
        {
            OffsetTest(TestFiles.COMPCURVE_TO_OFFSET, new BoundingBox(-99.868817, -68.230014, 37.095345, 65.389255, 0, 0));
        }

        [Test]
        public void MoveCompCurveTest()
        {
            MoveTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void MoveCompCurveCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void MoveCompCurveDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_COMPCURVE, "CentreOfGravity");
        }

        [Test]
        public void CompCurveNameTest()
        {
            NameTest(TestFiles.SINGLE_COMPCURVE, "SingleCompCurve", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("compcurve[NewName].EXISTS"));
        }

        [Test]
        public void RemoveCompCurveFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void RotateCompCurveTest()
        {
            RotateTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void RotateCompCurveCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// Test the distance between two composite curves
        /// </summary>
        [Test]
        public void MinimumDistanceCompCurveCompCurve()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.CompCurves[0];
            var entity2 = _powerSHAPE.ActiveModel.CompCurves[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 8.124662363567424);
        }

        /// <summary>
        /// Test the distance between a compcurve and a curve
        /// </summary>
        [Test]
        public void MinimumDistanceCompCurveCurve()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.CompCurves[0];
            var entity2 = _powerSHAPE.ActiveModel.Curves[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 69.479413151339458);
        }

        #endregion

        #region Wireframe Tests

        #region Properties

        /// <summary>
        /// A test for Centre of Gravity
        /// </summary>
        [Test]
        public void CompCurveCentreOfGravityTest()
        {
            // Check bounding box
            CentreOfGravityTest(TestFiles.SINGLE_COMPCURVE, new Point(-46.378983, 32.107367, 0));
        }

        /// <summary>
        /// A test for Length
        /// </summary>
        [Test]
        public void CompCurveLengthTest()
        {
            LengthTest(TestFiles.SINGLE_COMPCURVE, 59.895426);
        }

        /// <summary>
        /// A test for Points
        /// </summary>
        [Test]
        public void CompCurvePointsTest()
        {
            PointsTest(TestFiles.SINGLE_COMPCURVE, 5);
        }

        #endregion

        #endregion

        #region GenericCurve Tests

        #region Properties

        /// <summary>
        /// A test for Area
        /// </summary>
        [Test]
        public void CompCurveAreaTest()
        {
            // Check area
            AreaTest(TestFiles.SINGLE_COMPCURVE, 466.075826);
        }

        /// <summary>
        /// A test for EndPoint
        /// </summary>
        [Test]
        public void CompCurveEndPointTest()
        {
            // Check end point
            EndPointTest(TestFiles.SINGLE_COMPCURVE, new Point(-63, 21, 0));
        }

        /// <summary>
        /// A test for IsClosed
        /// </summary>
        [Test]
        public void CompCurveIsClosedTest()
        {
            // Check IsClosed property
            IsClosedTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for StartPoint
        /// </summary>
        [Test]
        public void CompCurveStartPointTest()
        {
            // Check start point
            StartPointTest(TestFiles.SINGLE_COMPCURVE, new Point(-22, 34, 0));
        }

        #endregion

        #region Operations

        /// <summary>
        /// A test for DeletePoints
        /// </summary>
        [Test]
        public void CompCurveDeletePointsTest()
        {
            DeletePointsTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for Extend
        /// </summary>
        [Test]
        public void CompCurveExtendTest()
        {
            if (_powerSHAPE.Version > new Version("15.1"))
            {
                ExtendTest(TestFiles.SINGLE_COMPCURVE,
                           new Point(-55.613702, 14.3507, 0),
                           new Point(-47.038746, 9.205786, 0));
            }
            else
            {
                ExtendTest(TestFiles.SINGLE_COMPCURVE,
                           new Point(-57.042781, 13.860087, 0),
                           new Point(-66.887732, 15.614207, 0));
            }
        }

        /// <summary>
        /// A test for FreeTangentsAndMagnitudes
        /// </summary>
        [Test]
        public void FreeCompCurveSelectedMagnitudesDirectionsTest()
        {
            FreeSelectedMagnitudesDirectionsTest(TestFiles.COMPCURVE_TO_FREE, 19.2);
        }

        /// <summary>
        /// A test for FreeTangentsAndMagnitudes
        /// </summary>
        [Test]
        public void FreeCompCurveAllMagnitudesDirectionsTest()
        {
            FreeAllMagnitudesDirectionsTest(TestFiles.COMPCURVE_TO_FREE, 19.2);
        }

        /// <summary>
        /// A test for GetLengthBetweenPoints
        /// </summary>
        [Test]
        public void GetLengthBetweenCompCurvePointsTest()
        {
            GetLengthBetweenPointsTest(TestFiles.SINGLE_COMPCURVE, 19.175008);
        }

        /// <summary>
        /// A test for GetPointEntryAzimuthAngle
        /// </summary>
        [Test]
        public void GetCompCurvePointEntryAzimuthAngleTest()
        {
            GetPointEntryAzimuthAngleTest(TestFiles.COMPCURVE_TO_FREE, 167.781684);
        }

        /// <summary>
        /// A test for GetPointEntryElevationAngle
        /// </summary>
        [Test]
        public void GetCompCurvePointEntryElevationAngleTest()
        {
            GetPointEntryElevationAngleTest(TestFiles.COMPCURVE_TO_FREE, 0);
        }

        /// <summary>
        /// A test for GetPointEntryMagnitude
        /// </summary>
        [Test]
        public void GetCompCurvePointEntryMagnitudeTest()
        {
            GetPointEntryMagnitudeTest(TestFiles.COMPCURVE_TO_FREE, 16.506620);
        }

        /// <summary>
        /// A test for GetPointEntryTangent
        /// </summary>
        [Test]
        public void GetCompCurvePointEntryTangentTest()
        {
            GetPointEntryTangentTest(TestFiles.COMPCURVE_TO_FREE, new Vector(-0.977348, 0.211637, 0));
        }

        /// <summary>
        /// A test for GetPointExitAzimuthAngle
        /// </summary>
        [Test]
        public void GetCompCurvePointExitAzimuthAngleTest()
        {
            GetPointExitAzimuthAngleTest(TestFiles.COMPCURVE_TO_FREE, 230.710593);
        }

        /// <summary>
        /// A test for GetPointExitElevationAngle
        /// </summary>
        [Test]
        public void GetCompCurvePointExitElevationAngleTest()
        {
            GetPointExitElevationAngleTest(TestFiles.COMPCURVE_TO_FREE, 0);
        }

        /// <summary>
        /// A test for GetPointExitMagnitude
        /// </summary>
        [Test]
        public void GetCompCurvePointExitMagnitudeTest()
        {
            GetPointExitMagnitudeTest(TestFiles.COMPCURVE_TO_FREE, 21.319006);
        }

        /// <summary>
        /// A test for GetPointExitTangent
        /// </summary>
        [Test]
        public void GetCompCurvePointExitTangentTest()
        {
            GetPointExitTangentTest(TestFiles.COMPCURVE_TO_FREE, new Vector(-0.633238, -0.773957, 0));
        }

        /// <summary>
        /// A test for MergeAndSpline
        /// </summary>
        [Test]
        public void MergeAndSplineCompCurveTest()
        {
            MergeAndSplineTest(TestFiles.COMPCURVE_TO_FREE);
        }

        /// <summary>
        /// A test for RenumberPoints
        /// </summary>
        [Test]
        public void RenumberCompCurvePointsTest()
        {
            RenumberPointsTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for Repoint
        /// </summary>
        [Test]
        public void RepointCompCurveTest()
        {
            RepointTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for Reverse
        /// </summary>
        [Test]
        public void ReverseCompCurveTest()
        {
            ReverseTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for Reverse
        /// </summary>
        [Test]
        public void ScaleCompCurveTest()
        {
            ScaleTest(TestFiles.SINGLE_COMPCURVE);
        }

        /// <summary>
        /// A test for ProjectOntoSurface
        /// </summary>
        [Test]
        public void ProjectCompCurveOntoSurfaceTest()
        {
            ProjectOntoSurfaceTest(TestFiles.COMPCURVE_TO_PROJECT,
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
            ProjectOntoPlaneTest(TestFiles.COMPCURVE_TO_PROJECT, Planes.XY, 10.3);
        }

        /// <summary>
        /// A test for ProjectOntoPlane using the X axis
        /// </summary>
        [Test]
        public void ProjectOntoPlaneX()
        {
            ProjectOntoPlaneTest(TestFiles.COMPCURVE_TO_PROJECT, Planes.YZ, 9.2);
        }

        /// <summary>
        /// A test for ProjectOntoPlane using the Y axis
        /// </summary>
        [Test]
        public void ProjectOntoPlaneY()
        {
            ProjectOntoPlaneTest(TestFiles.COMPCURVE_TO_PROJECT, Planes.ZX, 12.6);
        }

        [Test]
        public void InsertPointBetweenPointsTest_CompCurve()
        {
            InsertPointBetweenPointsTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void InsertPointRelativeToCurvePointTest_CompCurve()
        {
            InsertPointRelativeToCurvePointTest(TestFiles.SINGLE_COMPCURVE);
        }

        [Test]
        public void InsertPointByProximityTest_CompCurve()
        {
            var curve = (PSCompCurve) ImportAndGetEntity(TestFiles.SINGLE_COMPCURVE);

            var numPointsBefore = curve.NumberPoints;
            curve.InsertPointByProximity(new Point(-40, 50, 0));

            // Test that a point has been added
            Assert.AreEqual(curve.NumberPoints, numPointsBefore + 1, "Expected a point to be added.");
        }

        [Test]
        public void InsertPointsAtPlaneIntersections_CompCurve()
        {
            var curve = (PSCompCurve) ImportAndGetEntity(TestFiles.SINGLE_COMPCURVE);

            var numPointsBefore = curve.NumberPoints;

            var firstPoint = curve.Points[0].Clone();
            var secondPoint = curve.Points[1].Clone();
            var thirdPoint = curve.Points[2].Clone();
            var fourthPoint = curve.Points[3].Clone();
            curve.InsertPointsAtPlaneIntersections(Planes.ZX, 37.0);

            // Test that 2 points have been added
            Assert.AreEqual(curve.NumberPoints, numPointsBefore + 2, "Expected a point to be added.");

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

            // Test that the second has been inserted between third and fourth points
            Assert.AreEqual(thirdPoint.X,
                            curve.Points[3].X,
                            "Expected second new point to be inserted between third and fourth.");
            Assert.AreEqual(thirdPoint.Y,
                            curve.Points[3].Y,
                            "Expected second new point to be inserted between third and fourth.");
            Assert.AreEqual(thirdPoint.Z,
                            curve.Points[3].Z,
                            "Expected second new point to be inserted between third and fourth.");
            Assert.AreEqual(fourthPoint.X,
                            curve.Points[5].X,
                            "Expected second new point to be inserted between third and fourth.");
            Assert.AreEqual(fourthPoint.Y,
                            curve.Points[5].Y,
                            "Expected second new point to be inserted between third and fourth.");
            Assert.AreEqual(fourthPoint.Z,
                            curve.Points[5].Z,
                            "Expected second new point to be inserted between third and fourth.");
        }

        /// <summary>
        /// A test for extracting polylines from a DUCT file
        /// </summary>
        [Test]
        public void ExtractPolylineFromDuctFileTest()
        {
            ExtractPolylineFromDuctTest(TestFiles.COMPCURVE_TO_FREE);
        }

        /// <summary>
        /// A test for extracting conic arcs from a DUCT file
        /// </summary>
        [Test]
        public void ExtractSplineFromDuctFileTest()
        {
            ExtractSplineFromDuctTest(TestFiles.COMPCURVE_TO_FREE);
        }

        #endregion

        #endregion

        #region CompCurve Tests

        #region Properties

        /// <summary>
        /// Test for NumberPoints
        /// </summary>
        [Test]
        public void CompCurveNumberPointsTest()
        {
            // Get curve
            var compCurve = (PSCompCurve) ImportAndGetEntity(TestFiles.SINGLE_COMPCURVE);

            // Check number of points
            Assert.AreEqual(5, compCurve.NumberPoints, "Returned incorrect number of points");
        }

        /// <summary>
        /// Test for NumberCurveItems
        /// </summary>
        [Test]
        public void CompCurveNumberCurveItemsTest()
        {
            // Get curve
            var compCurve = (PSCompCurve) ImportAndGetEntity(TestFiles.COMPCURVE_MULTIPLE_ITEMS);

            // Check number of items
            Assert.AreEqual(2, compCurve.NumberCurveItems, "Returned incorrect number of curve items");
        }

        #endregion

        #endregion
    }
}