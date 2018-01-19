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
    /// This is a test class for PointTest and is intended
    /// to contain all PointTest Unit Tests
    /// </summary>
    [TestFixture]
    public class PointTest : EntityTest<PSPoint>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Points;
            }
            catch (Exception e)
            {
                Assert.Fail("_entityCollection could not be set to Active Model's PointsCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void PointIdTest()
        {
            IdTest(TestFiles.SINGLE_POINT);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void PointIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_POINT, "POINT");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void PointEqualsTest()
        {
            Equals(TestFiles.SINGLE_POINT);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void PointExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_POINT);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddPointToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_POINT);
        }

        [Test]
        public void BlankPointTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_POINT);
        }

        [Test]
        public void PointBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_POINT, new Point(-137.0, 14, 0), new Point(-137.0, 14, 0));
        }

        [Test]
        public void CopyPointTest()
        {
            DuplicateTest(TestFiles.SINGLE_POINT);
        }

        [Test]
        public void DeletePointTest()
        {
            DeleteTest(TestFiles.SINGLE_POINT);
        }

        [Test]
        public void PointLevelTest()
        {
            LevelTest(TestFiles.SINGLE_POINT, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void PointLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_POINT, 1);
        }

        [Test]
        [Ignore("")]
        public void LimitPointWithEntitiesTest()
        {
            //LimitToEntitiesTest(TestFiles.SINGLE_POINT, TestFiles.POINT_LIMITERS, PSEntityLimiter.LimitingModes.IntersectCurveMode);
            Assert.Inconclusive();
        }

        [Test]
        [Ignore("")]
        public void MirrorPointTest()
        {
            //// Setup parameters
            //Point initialPointStartPoint = new Point((MM)(-72), (MM)55, (MM)0);
            //Point initialPointEndPoint = new Point((MM)(-125), (MM)(-47), (MM)0);

            //// Carry out operation
            //MirrorTest(TestFiles.SINGLE_POINT, initialPointStartPoint);

            //// Get point that should now be on mirror point
            //PSPoint mirroredPoint = _powerSHAPE.ActiveModel.Points[0];
            //Assert.AreEqual(initialPointStartPoint, mirroredPoint.StartPoint, "Point was not mirrored around correct point");

            //// Check that second test point is now the correct distance from its initial position
            //Assert.AreEqual((initialPointStartPoint.X - initialPointEndPoint.X) * 2, mirroredPoint.EndPoint.X - initialPointEndPoint.X, "Point was not mirrored in correct plane");
            Assert.Inconclusive();
        }

        [Test]
        public void MovePointTest()
        {
            MoveTest(TestFiles.SINGLE_POINT);
        }

        [Test]
        [Ignore("")]
        public void MovePointCreatingCopiesTest()
        {
            Assert.Inconclusive();

            // Doesn't work as it thinks the created points are meshes
            //MoveCreatingCopiesTest(TestFiles.SINGLE_POINT);
        }

        [Test]
        public void MovePointDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_POINT, "Centre");
        }

        [Test]
        public void PointNameTest()
        {
            NameTest(TestFiles.SINGLE_POINT, "1", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("point[NewName].EXISTS"));
        }

        [Test]
        public void RemovePointFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_POINT);
        }

        [Test]
        public void RotatePointTest()
        {
            RotateTest(TestFiles.SINGLE_POINT);
        }

        [Test]
        [Ignore("")]
        public void RotatePointCreatingCopiesTest()
        {
            Assert.Inconclusive();

            // Doesn't work as it thinks the created points are meshes
            //RotateCreatingCopiesTest(TestFiles.SINGLE_POINT);
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
            Assert.AreEqual((MM) 354.290840976732, entity2.DistanceTo(entity1));
        }

        /// <summary>
        /// Test the distance between two points
        /// </summary>
        [Test]
        public void MinimumDistancePointPoint()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Points[0];
            var entity2 = _powerSHAPE.ActiveModel.Points[1];

            // Test the distance
            Assert.AreEqual((MM) 46.8187996428785, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test the distance between a point and a solid
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistancePointSolid()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Points[0];
            var entity2 = _powerSHAPE.ActiveModel.Solids[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Find the nearest point on the nearest in world
        /// </summary>
        [Test]
        public void NearestSurface()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the nearest surface
            Point nearestPoint = null;
            var nearestSurface = _powerSHAPE.ActiveModel.Points[0].GetNearestSurface(ref nearestPoint);

            // Test the distance
            Assert.AreEqual(5, nearestSurface.Id);
            Assert.AreEqual(new Point(-209, 119, 0), nearestPoint);
        }

        /// <summary>
        /// Find the nearest point on the nearest in the active workplane
        /// </summary>
        [Test]
        public void NearestSurfaceActiveWorkplane()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));
            _powerSHAPE.ActiveModel.ActiveWorkplane = _powerSHAPE.ActiveModel.Workplanes[0];

            // Get the nearest surface
            Point nearestPoint = null;
            var nearestSurface = _powerSHAPE.ActiveModel.Points[0].GetNearestSurface(ref nearestPoint);

            // Test the distance
            Assert.AreEqual(5, nearestSurface.Id);
            Assert.AreEqual(new Point(-189, 200, 0), nearestPoint);
        }

        /// <summary>
        /// Find the nearest point on the nearest surface in world coordinates when there is an active workplane
        /// </summary>
        [Test]
        public void NearestSurfaceActiveWorkplaneWorld()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));
            _powerSHAPE.ActiveModel.ActiveWorkplane = _powerSHAPE.ActiveModel.Workplanes[0];

            // Get the nearest surface
            Point nearestPoint = null;
            var nearestSurface = _powerSHAPE.ActiveModel.Points[0].GetNearestSurface(ref nearestPoint, true);

            // Test the distance
            Assert.AreEqual(5, nearestSurface.Id);
            Assert.AreEqual(new Point(-209, 119, 0), nearestPoint);
        }

        #endregion
    }
}