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
    /// This is a test class for SurfaceTest and is intended
    /// to contain all SurfaceTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SurfaceTest : EntityTest<PSSurface>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Surfaces;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's SurfacesCollection\n\n" +
                            e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void SurfaceIdTest()
        {
            IdTest(TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void SurfaceIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_SURFACE, "SURFACE");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void SurfaceEqualsTest()
        {
            Equals(TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void SurfaceExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddSurfaceToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_LINE, TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void BlankSurfaceTest()
        {
            BlankTest(TestFiles.THREE_LINES, TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void SurfaceBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_SURFACE, new Point(-89, 12, 0), new Point(-39, 62, 0));
        }

        [Test]
        public void CopySurfaceTest()
        {
            DuplicateTest(TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void DeleteSurfaceTest()
        {
            DeleteTest(TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void SurfaceLevelTest()
        {
            LevelTest(TestFiles.SINGLE_SURFACE, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void SurfaceLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_SURFACE, 1);
        }

        [Test]
        public void LimitSurfaceToEntitiesTest()
        {
            LimitToEntitiesTest(TestFiles.SINGLE_SURFACE, TestFiles.SURFACE_LIMITERS, LimitingModes.LimitMode);
        }

        [Test]
        public void MirrorSurfaceTest()
        {
            // Carry out operation
            var mirrorPoint = new Point(-39, 12, 0);
            var initialEndPoint = new Point(-89, 12, 0);

            MirrorTest(TestFiles.SINGLE_SURFACE, mirrorPoint);

            // Get point that should now be on mirror point
            var mirroredSurface = _powerSHAPE.ActiveModel.Surfaces[0];
            Assert.AreEqual(mirrorPoint,
                            mirroredSurface.Laterals[0].EndPoint,
                            "Surface was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((mirrorPoint.X - initialEndPoint.X) * 2.0,
                            mirroredSurface.Laterals[0].StartPoint.X - initialEndPoint.X,
                            "Arc was not mirrored in correct plane");
        }

        [Test]
        public void MoveSurfaceTest()
        {
            MoveTest(TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void MoveSurfaceCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void MoveSurfaceDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_SURFACE, "CentreOfGravity");
        }

        [Test]
        public void SurfaceNameTest()
        {
            NameTest(TestFiles.SINGLE_SURFACE, "SingleSurface", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("surface[NewName].EXISTS"));
        }

        [Test]
        public void RemoveSurfaceFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_LINE, TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void RotateSurfaceTest()
        {
            RotateTest(TestFiles.SINGLE_SURFACE);
        }

        [Test]
        public void RotateSurfaceCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// Test the nearest point
        /// </summary>
        [Test]
        public void NearestPoint()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Surfaces[0];
            var entity2 = _powerSHAPE.ActiveModel.Points[0];

            // Test the distance
            Assert.AreEqual(new Point(-222, 58, 0), entity1.NearestPoint(entity2.ToPoint()));
        }

        /// <summary>
        /// Test the distance between a surface and a point
        /// </summary>
        [Test]
        public void MinimumDistanceSurfacePoint()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Surfaces[0];
            var entity2 = _powerSHAPE.ActiveModel.Points[0];

            // Test the distance
            Assert.AreEqual((MM) 354.290840976732, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test the distance between two surfaces
        /// </summary>
        [Test]
        public void MinimumDistanceSurfaceSurface()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Surfaces[0];
            var entity2 = _powerSHAPE.ActiveModel.Surfaces[1];

            // Test the distance
            Assert.AreEqual((MM) 23, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test the distance between a surface and a workplane
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceSurfaceWorkplane()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Surfaces[0];
            var entity2 = _powerSHAPE.ActiveModel.Workplanes[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        #endregion

        #region Surface Tests

        #region Properties

        /// <summary>
        /// A test for Area
        /// </summary>
        [Test]
        public void SurfaceAreaTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check surface area
            Assert.AreEqual(2500, surface.Area, "Returned surface area is incorrect");
        }

        /// <summary>
        /// A test for CentreOfGravity
        /// </summary>
        [Test]
        public void SurfaceCentreOfGravityTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check surface COG
            var expectedCOG = new Point(-64, 37, 0);
            Assert.AreEqual(expectedCOG, surface.CentreOfGravity, "Returned centre of gravtity is incorrect");
        }

        /// <summary>
        /// A test for Direction
        /// </summary>
        [Test]
        public void SurfaceDirectionTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check surface direction
            var expectedDirection = new Vector(0, 0, 1);
            Assert.AreEqual(expectedDirection, surface.Direction, "Returned direction is incorrect");
        }

        /// <summary>
        /// A test for IsClosedLaterals
        /// </summary>
        [Test]
        public void IsSurfaceClosedLateralsTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_LATSOPENCLOSED);

            // Check that laterals are returned as being open
            Assert.IsFalse(surface.IsClosedLaterals, "Incorrectly returned that the laterals were closed");

            // Close the laterals
            surface.IsClosedLaterals = true;

            // Check that laterals are returned as being closed
            Assert.IsTrue(surface.IsClosedLaterals, "Incorrectly returned that the laterals were open");
        }

        /// <summary>
        /// A test for IsClosedLongitudinals
        /// </summary>
        [Test]
        public void IsSurfaceClosedLongitudinalsTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_LONSOPENCLOSED);

            // Check that longitudinals are returned as being open
            Assert.IsFalse(surface.IsClosedLongitudinals, "Incorrectly returned that the longitudinals were closed");

            // Close the longitudinals
            surface.IsClosedLongitudinals = true;

            // Check that longitudinals are returned as being closed
            Assert.IsTrue(surface.IsClosedLongitudinals, "Incorrectly returned that the longitudinals were open");
        }

        /// <summary>
        /// A test for IsTrimmed
        /// </summary>
        [Test]
        public void IsSurfaceTrimmedTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_TRIM);

            // Check trim is on
            Assert.IsTrue(surface.IsTrimmed, "Incorrectly returned that surface wasn't trimmed");

            // Switch trim off
            surface.IsTrimmed = false;
            Assert.IsFalse(surface.IsTrimmed, "Incorrectly returned that surface was trimmed");
        }

        /// <summary>
        /// A test for NumberOfBoundaries
        /// </summary>
        [Test]
        public void NumberOfSurfaceBoundariesTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_TRIM);

            // Check number of boundaries
            Assert.AreEqual(2, surface.NumberOfBoundaries, "Returned incorrect number of boundaries");
        }

        /// <summary>
        /// A test for NumberOfLaterals
        /// </summary>
        [Test]
        public void NumberOfSurfaceLateralsTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check number of laterals
            Assert.AreEqual(3, surface.NumberOfLaterals, "Returned incorrect number of laterals");
        }

        /// <summary>
        /// A test for NumberOfLongitudinals
        /// </summary>
        [Test]
        public void NumberOfSurfaceLongitudinalsTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check number of longitudinals
            Assert.AreEqual(3, surface.NumberOfLongitudinals, "Returned incorrect number of longitudinals");
        }

        /// <summary>
        /// A test for NumberOfPCurves
        /// </summary>
        [Test]
        public void NumberOfSurfacePCurvesTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_TRIM);

            // Check number of pCurves
            Assert.AreEqual(5, surface.NumberOfPCurves, "Returned incorrect number of pCurves");
        }

        /// <summary>
        /// A test for HasSpine
        /// </summary>
        [Test]
        public void SurfaceHasSpineTest()
        {
            // Get non-spined surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check that code correctly states it has no spine
            Assert.IsFalse(surface.HasSpine, "Incorrectly returned that surface had spine");

            // Get spined surface
            var spineSurface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE_SPINE);

            // Check that code correctly states that surface has a spine
            Assert.IsTrue(spineSurface.HasSpine, "Incorrectly returned that surface does not have a spine");
        }

        /// <summary>
        /// A test for Type
        /// </summary>
        [Test]
        public void SurfaceTypeTest()
        {
            // Get surface
            var powerSurface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check type
            Assert.AreEqual(SurfaceTypes.PowerSurface, powerSurface.Type, "Returned type was incorrect");
            powerSurface.Delete();

            //TODO: Find a way of importing a block
            //// Get surface
            //PSSurface block = (PSSurface)ImportAndGetEntity(TestFiles.SINGLE_SURFACEBLOCK);

            //// Check type
            //Assert.AreEqual(SurfaceTypes.Block, block.Type, "Returned type was incorrect");
            //block.Delete();

            // Get surface
            var cone = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACECONE);

            // Check type
            Assert.AreEqual(SurfaceTypes.Cone, cone.Type, "Returned type was incorrect");
            cone.Delete();

            // Get surface
            var cylinder = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACECYLINDER);

            // Check type
            Assert.AreEqual(SurfaceTypes.Cylinder, cylinder.Type, "Returned type was incorrect");
            cylinder.Delete();

            // Get surface
            var extrusion = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEEXTRUSION);

            // Check type
            Assert.AreEqual(SurfaceTypes.Extrusion, extrusion.Type, "Returned type was incorrect");
            extrusion.Delete();

            // Get surface
            var plane = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEPLANE);

            // Check type
            Assert.AreEqual(SurfaceTypes.Plane, plane.Type, "Returned type was incorrect");
            plane.Delete();

            //TODO: Find way of importing revolution
            //// Get surface
            //PSSurface revolution = (PSSurface)ImportAndGetEntity(TestFiles.SINGLE_SURFACEREVOLUTION);

            //// Check type
            //Assert.AreEqual(SurfaceTypes.Revolution, revolution.Type, "Returned type was incorrect");
            //revolution.Delete();

            // Get surface
            var sphere = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACESPHERE);

            // Check type
            Assert.AreEqual(SurfaceTypes.Sphere, sphere.Type, "Returned type was incorrect");
            sphere.Delete();

            //TODO: Find way of importing spring
            //// Get surface
            //PSSurface spring = (PSSurface)ImportAndGetEntity(TestFiles.SINGLE_SURFACESPRING);

            //// Check type
            //Assert.AreEqual(SurfaceTypes.Spring, spring.Type, "Returned type was incorrect");
            //spring.Delete();

            // Get surface
            var torus = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACETORUS);

            // Check type
            Assert.AreEqual(SurfaceTypes.Torus, torus.Type, "Returned type was incorrect");
        }

        /// <summary>
        /// A test for Volume
        /// </summary>
        [Test]
        public void SurfaceVolumeTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACECONE);

            // Check volume
            Assert.AreEqual(1.677712, Math.Round(surface.Volume, 6), "Returned volume was incorrect");
        }

        /// <summary>
        /// A test for Laterals
        /// </summary>
        [Test]
        public void LateralsTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check surface laterals
            Assert.AreEqual(3, surface.Laterals.Count, "Number of laterals was incorrect");
            Assert.AreEqual(new Point(-39, 12, 0), surface.Laterals[0].StartPoint, "Lateral 1 start point was incorrect");
            Assert.AreEqual(new Point(-39, 42.741667, 0),
                            surface.Laterals[1].StartPoint,
                            "Lateral 2 start point was incorrect");
            Assert.AreEqual(new Point(-39, 62, 0), surface.Laterals[2].StartPoint, "Lateral 3 start point was incorrect");
        }

        /// <summary>
        /// A test for Longitudinals
        /// </summary>
        [Test]
        public void LongitudinalsTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check surface longitudinals
            Assert.AreEqual(3, surface.Longitudinals.Count, "Number of longitudinals was incorrect");
            Assert.AreEqual(new Point(-39, 12, 0),
                            surface.Longitudinals[0].StartPoint,
                            "Longitudinal 1 start point was incorrect");
            Assert.AreEqual(new Point(-77.3375, 12, 0),
                            surface.Longitudinals[1].StartPoint,
                            "Longitudinal 2 start point was incorrect");
            Assert.AreEqual(new Point(-89, 12, 0),
                            surface.Longitudinals[2].StartPoint,
                            "Longitudinal 3 start point was incorrect");
        }

        /// <summary>
        /// A test for pCurves
        /// </summary>
        [Test]
        public void PCurvesTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_TRIM);

            // Check pCurves
            Assert.AreEqual(surface.NumberOfPCurves, surface.PCurves.Count, "Incorrect number of pCurves returned");
            Assert.AreEqual(new Point(-72.217785, 51.144372, 0),
                            surface.PCurves[0].StartPoint,
                            "Returned first point of first pCurve was incorrect");
            Assert.AreEqual(new Point(-80.752547, 45.544918, 0),
                            surface.PCurves[1].StartPoint,
                            "Returned first point of second pCurve was incorrect");
            Assert.AreEqual(new Point(-55.429587, 38.120681, 0),
                            surface.PCurves[2].StartPoint,
                            "Returned first point of third pCurve was incorrect");
            Assert.AreEqual(new Point(-62.533180, 48.929443, 0),
                            surface.PCurves[3].StartPoint,
                            "Returned first point of fourth pCurve was incorrect");
            Assert.AreEqual(new Point(-42.757160, 20.516042, 0),
                            surface.PCurves[4].StartPoint,
                            "Returned first point of fifth pCurve was incorrect");
        }

        /// <summary>
        /// A test for Shape
        /// </summary>
        [Test]
        public void SurfaceShapeTest()
        {
            // Get surface
            var block = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEBLOCK);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Block, block.Shape, "Returned shape was incorrect");
            block.Delete();

            // Get surface
            var cone = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACECONE);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Cone, cone.Shape, "Returned shape was incorrect");
            cone.Delete();

            // Get surface
            var cylinder = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACECYLINDER);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Cylinder, cylinder.Shape, "Returned shape was incorrect");
            cylinder.Delete();

            // Get surface
            var extrusion = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEEXTRUSION);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Extrusion, extrusion.Shape, "Returned shape was incorrect");
            extrusion.Delete();

            // Get surface
            var plane = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEPLANE);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Plane, plane.Shape, "Returned shape was incorrect");
            plane.Delete();

            // Get surface
            var revolution = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEREVOLUTION);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Revolution, revolution.Shape, "Returned shape was incorrect");
            revolution.Delete();

            // Get surface
            var sphere = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACESPHERE);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Sphere, sphere.Shape, "Returned shape was incorrect");
            sphere.Delete();

            // Get surface
            var spring = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACESPRING);

            // Check shape
            Assert.AreEqual(SurfaceShapes.General_Surface, spring.Shape, "Returned shape was incorrect");
            spring.Delete();

            // Get surface
            var torus = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACETORUS);

            // Check shape
            Assert.AreEqual(SurfaceShapes.Torus, torus.Shape, "Returned shape was incorrect");
        }

        #endregion

        #region Operations

        /// <summary>
        /// A test for Scale, inputting a PowerSurface
        /// </summary>
        [Test]
        public void ScalePowerSurface()
        {
            // Get surface
            var surfaceToScale = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Get intended check point of surface
            var pointVector = new Vector(surfaceToScale.CentreOfGravity, surfaceToScale.Laterals[0].Points[0]);
            var surfaceCheckPoint = surfaceToScale.Laterals[0].Points[0] + pointVector;
            var lateralCheckLength = surfaceToScale.Laterals[0].Length.Value * 2;

            // Scale surface
            surfaceToScale = surfaceToScale.Scale(2, 2, 0, surfaceToScale.CentreOfGravity);

            // Check that it's worked
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Surfaces.Count, "There is no longer one surface in PowerSHAPE");
            Assert.IsTrue(surfaceToScale.Exists, "Surface doesn't exist");
            Assert.IsTrue(lateralCheckLength == surfaceToScale.Laterals[0].Length, "Surface not scaled correctly");
            Assert.IsTrue(surfaceCheckPoint == surfaceToScale.Laterals[0].Points[0],
                          "Surface not scaled to correct origin");
        }

        /// <summary>
        /// A test for Scale, inputting a primitive
        /// </summary>
        [Test]
        public void ScalePrimitiveSurface()
        {
            // Import plane
            var plane = (PSSurfacePlane) ImportAndGetEntity(TestFiles.SINGLE_SURFACEPLANE);

            // Get intended check point of surface
            var pointVector = new Vector(plane.CentreOfGravity, plane.Laterals[0].Points[0]);
            var surfaceCheckPoint = plane.Laterals[0].Points[0] + pointVector;
            var lateralCheckLength = plane.Laterals[0].Length.Value * 2;

            // Scale plane
            var newSurface = plane.Scale(2, 2, 0, plane.CentreOfGravity);

            // Check that it's worked
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Surfaces.Count, "There is no longer one surface in PowerSHAPE");
            Assert.IsFalse(plane.Exists, "Plane still exists");
            Assert.IsFalse(newSurface is PSSurfacePlane, "Surface type has not been changed from being a plane");
            Assert.IsTrue(lateralCheckLength == newSurface.Laterals[0].Length, "Surface not scaled correctly");
            Assert.IsTrue(surfaceCheckPoint == newSurface.Laterals[0].Points[0], "Surface not scaled to correct origin");
        }

        /// <summary>
        /// A test for AddSurfaceCurveByParameter
        /// </summary>
        [Test]
        public void AddSurfaceCurveByParameterTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Carry out operation
            surface.AddSurfaceCurveByParameter(SurfaceCurveTypes.Longitudinal, 1.5F);
            Assert.AreNotEqual(3, surface.NumberOfLongitudinals, "Longitudinal was not created");
            Assert.AreEqual(new Point(-58.16875, 12, 0),
                            surface.Longitudinals[1].StartPoint,
                            "Longitudinal created in wrong position");
        }

        /// <summary>
        /// A test for ConvertSurfaceCurvesToWireframe
        /// </summary>
        [Test]
        public void ConvertSurfaceCurvesToWireframeTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Carry out operation
            var convertedCurves = surface.ConvertSurfaceCurvesToWireframe(SurfaceCurveTypes.Longitudinal, new[] {1, 3});
            Assert.AreEqual(2, convertedCurves.Count, "Incorrect number of curves in list");
            Assert.AreEqual(2,
                            _powerSHAPE.ActiveModel.CompCurves.Count,
                            "Incorrect number of curves in PowerSHAPE collection");
            Assert.AreEqual(surface.Longitudinals[0].StartPoint,
                            convertedCurves[0].StartPoint,
                            "First curve does not match longitudinal");
            Assert.AreEqual(surface.Longitudinals[2].StartPoint,
                            convertedCurves[1].StartPoint,
                            "Second curve does not match longitudinal");
        }

        /// <summary>
        /// A test for Extend
        /// </summary>
        [Test]
        public void ExtendSurfaceTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Carry out operation
            surface.Extend(ExtensionEdges.ONE, 2F, ExtensionType.Linear);
            Assert.IsFalse(surface.Longitudinals[0].Length == 52, "Incorrect edge extended");
            Assert.IsTrue(surface.Laterals[0].Length == 52, "Surface not extended");
            Assert.IsTrue(surface.Longitudinals[0].StartPoint.X - surface.Longitudinals[1].StartPoint.X == 2,
                          "Incorrect edge extended");
        }

        /// <summary>
        /// A test for GetMaxCurvature
        /// </summary>
        [Test]
        public void GetMaxSurfaceCurvatureTest()
        {
            // Get torus surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACETORUS);

            // Check max curvature
            Assert.AreEqual(0.2, Math.Round(surface.GetMaxCurvature(1, 1), 6), "Returned maximum curvature is incorrect");
        }

        /// <summary>
        /// A test for GetMinCurvature
        /// </summary>
        [Test]
        public void GetMinSurfaceCurvatureTest()
        {
            // Get torus surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACETORUS);

            // Check min curvature
            Assert.AreEqual(0.066667,
                            Math.Round(surface.GetMinCurvature(1, 1), 6),
                            "Returned minimum curvature is incorrect");
        }

        /// <summary>
        /// A test for GetNearestTUParameters
        /// </summary>
        [Test]
        public void GetNearestSurfaceTUParametersTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check nearest TU parameters
            double[] expectedParameters = {2, 2};
            var actualParameters = surface.GetNearestTUParameters(new Point(-77.3375, 42.741667, 10), 1, 1);
            Assert.AreEqual(expectedParameters[0], actualParameters[0], "Returned T parameter is incorrect");
            Assert.AreEqual(expectedParameters[1], actualParameters[1], "Returned U parameter is incorrect");
        }

        /// <summary>
        /// A test for GetSurfacePoint
        /// </summary>
        [Test]
        public void GetSurfacePointTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Check surface point
            Assert.AreEqual(new Point(-89, 62, 0), surface.GetSurfacePoint(3, 3), "Returned surface point is incorrect");
        }

        /// <summary>
        /// A test for GetSurfacePoint
        /// </summary>
        [Test]
        public void GetNearestNormalTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE_WITH_BULGE);

            // Check surface point
            Assert.AreEqual(new Vector(0.072234, -0.024487, 0.997087),
                            surface.GetNearestNormal(new Point(-55, 37, 100)),
                            "Returned surface normal is incorrect");
        }

        /// <summary>
        /// A test for ReverseNormal
        /// </summary>
        [Test]
        public void ReverseSurfaceNormalTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACE);

            // Reverse normal
            surface.ReverseSurfaceNormal();

            // Check resultant normal
            Assert.AreEqual(new Vector(0, 0, -1), surface.GetNormal(2, 2), "Resultant normal is incorrect");
        }

        /// <summary>
        /// A test for Normal
        /// </summary>
        [Test]
        public void SurfaceNormalTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACECYLINDER);

            // Check normal at two points
            Assert.AreEqual(new Vector(1, 0, 0), surface.GetNormal(1, 1), "Incorrect normal returned at TU position 1;1");
            Assert.AreEqual(new Vector(0, 1, 0), surface.GetNormal(2, 4), "Incorrect normal returned at TU position 2;4");
        }

        /// <summary>
        /// A test for ConvertBoundaryToCompCurve
        /// </summary>
        [Test]
        public void ConvertSurfaceBoundaryToCompCurveTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_TRIM);

            // Convert CompCurve
            var compCurve = surface.ConvertBoundaryToCompCurve(2);
            Assert.AreEqual(87.2, Math.Round(compCurve.Length.Value, 1), "Returned CompCurve was incorrect");
        }

        /// <summary>
        /// A test for ConvertSurfaceToPowerSurface
        /// </summary>
        [Test]
        public void ConvertSurfaceToPowerSurfaceTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEPLANE);

            // Convert surface
            var newSurface = surface.ConvertSurfaceToPowerSurface();
            Assert.AreEqual(SurfaceTypes.PowerSurface, newSurface.Type, "Returned type was incorrect");
            Assert.IsFalse(surface.Exists);
        }

        /// <summary>
        /// A test for DeleteBoundary
        /// </summary>
        [Test]
        public void DeleteBoundaryTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SURFACE_TRIM);

            // Delete surface boundary
            surface.DeleteBoundary(2);
            Assert.AreEqual(1, surface.NumberOfBoundaries, "New number of boundaries is incorrect");
        }

        /// <summary>
        /// A test for Offset
        /// </summary>
        [Test]
        public void OffsetSurfaceTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACEPLANE);

            // Offset surface
            /*PSSurface newSurface = */
            surface.Offset(5F, OffsetCornerTypes.SHARP, OffsetSplitAtDiscontinuity.SPLITON);

            //Assert.AreEqual(new Point((MM)(-77), (MM)5, (MM)5), newSurface.CentreOfGravity, "Centre of Gravity of new surface was incorrect");
        }

        /// <summary>
        /// A test for SwapSurfaceCurves
        /// </summary>
        [Test]
        public void SwapSurfaceCurvesTest()
        {
            // Get surface
            var surface = (PSSurface) ImportAndGetEntity(TestFiles.SINGLE_SURFACESPRING);

            // Check the length before hand
            Assert.AreEqual(12.567560, Math.Round(surface.Laterals[24].Length.Value, 6));
            Assert.AreEqual(416.609708, Math.Round(surface.Longitudinals[0].Length.Value, 6));

            // Swap curves
            surface.SwapSurfaceCurves();

            // Check after
            Assert.AreEqual(416.609708,
                            Math.Round(surface.Laterals[0].Length.Value, 6),
                            "Surface curves were not swapped");
            Assert.AreEqual(12.567560,
                            Math.Round(surface.Longitudinals[24].Length.Value, 6),
                            "Surface curves were not swapped");
        }

        [Test]
        public void StitchToCurveTest()
        {
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.Import(new File(TestFiles.STITCH_FILE));
            var curve = activeModel.CompCurves[0];
            var surface = activeModel.Surfaces[0];
            surface.StitchToCurve(curve, surface.Laterals[0], StitchDirectionAcross.NONE, StitchDirectionAlong.NONE, 30);
            Assert.That(surface.Laterals[0].Length, Is.EqualTo((MM) 308.166456));
        }

        [Test]
        public void SelectSurfaceCurvesTest()
        {
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.Import(new File(TestFiles.STITCH_FILE));
            var surface = activeModel.Surfaces[0];
            surface.SelectSurfaceCurves(SurfaceCurveTypes.Lateral, new[] {1});
            _powerSHAPE.Execute("CONVERT WIREFRAME");
            activeModel.Refresh();
            Assert.That(activeModel.CompCurves.Count, Is.EqualTo(2));
            Assert.That(activeModel.CompCurves[1].Length, Is.EqualTo((MM) 316.322297));
        }

        [Test]
        public void SelectLonsAndLatsSurfaceCurvesTest()
        {
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.Import(new File(TestFiles.STITCH_FILE));
            var surface = activeModel.Surfaces[0];
            surface.SelectLateralAndLongitudinalSurfaceCurves(new[] {1}, new[] {2});
            _powerSHAPE.Execute("CONVERT WIREFRAME");
            activeModel.Refresh();
            Assert.That(activeModel.CompCurves.Count, Is.EqualTo(3));
            Assert.That(activeModel.CompCurves[1].Length, Is.EqualTo((MM) 316.322297));
            Assert.That(activeModel.CompCurves[2].Length, Is.EqualTo((MM) 20.405713));
        }

        [Test]
        public void TrimmedSurfaceBoundingBoxTest()
        {
            var activeModel = _powerSHAPE.ActiveModel;
            activeModel.Import(new File(TestFiles.TRIMMED_SURFACE));
            var surface = activeModel.Surfaces[0];
            Assert.That(surface.BoundingBox, Is.EqualTo(new BoundingBox(-49.172922, 19.182409, -4.311589, 53.070873, 0, 0)));
        }

        #endregion

        #endregion
    }
}