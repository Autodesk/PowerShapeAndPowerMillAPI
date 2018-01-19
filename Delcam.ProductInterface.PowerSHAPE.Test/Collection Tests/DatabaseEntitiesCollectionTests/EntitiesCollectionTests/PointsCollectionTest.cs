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
    /// This is a test class for PointsCollectionTest and is intended
    /// to contain all PointsCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class PointsCollectionTest : EntitiesCollectionTest<PSPoint>
    {
        #region Constructors

        public PointsCollectionTest()
        {
            // Initialise PowerSHAPE
            if (_powerSHAPE == null)
            {
                _powerSHAPE = new PSAutomation(InstanceReuse.UseExistingInstance, Modes.PShapeMode);
                _powerSHAPE.DialogsOff();
            }
        }

        #endregion

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
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's PointsCollection\n\n" + e.Message);
            }
        }

        #region PointsCollection Tests

        /// <summary>
        /// A test for CreatePoint
        /// </summary>
        [Test]
        public void CreatePointTest()
        {
            // Create point
            _powerSHAPE.ActiveModel.Points.CreatePoint(new Point(20, 10, 0));
            Assert.AreEqual(_powerSHAPE.ActiveModel.Points.Count, 1, "Failed to add point to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create point in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreatePointBetweenPoints
        /// </summary>
        [Test]
        public void CreatePointBetweenPointsTest()
        {
            // Create point
            var createdPoint = _powerSHAPE.ActiveModel.Points.CreatePointBetweenPoints(new Point(20, 10, 0),
                                                                                       new Point(40, 10, 0));
            Assert.AreEqual(_powerSHAPE.ActiveModel.Points.Count, 1, "Failed to add point to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create point in PowerSHAPE");
            Assert.IsTrue(createdPoint.X == 30, "Failed to create point in correct position in PowerSHAPE");
        }

        [Test]
        public void CreatePointAtIntersectionOfSurfaceAndWireframeTest()
        {
            // Create surface
            var surface = _powerSHAPE.ActiveModel.Surfaces.CreatePlane(new Point(), Planes.XY);

            // Create line
            var line = _powerSHAPE.ActiveModel.Lines.CreateLine(new Point(15, 15, 50), new Point(15, 15, -50));

            // Create point
            var point = _powerSHAPE.ActiveModel.Points.CreatePointAtIntersectionOfSurfaceAndWireframe(surface, line);

            // Check we got a point at 15, 15, 0
            Assert.IsNotNull(point);
            Assert.AreEqual(new Point(15, 15, 0), point.ToPoint());
        }

        [Test]
        public void CreatePointAtIntersectionOfSurfaceAndWireframeWithNoIntersectionTest()
        {
            // Create surface
            var surface = _powerSHAPE.ActiveModel.Surfaces.CreatePlane(new Point(), Planes.XY);

            // Create line (such that it doesn't intersect the surface)
            var line = _powerSHAPE.ActiveModel.Lines.CreateLine(new Point(50, 50, 50), new Point(-50, -50, 10));

            // Create point
            var point = _powerSHAPE.ActiveModel.Points.CreatePointAtIntersectionOfSurfaceAndWireframe(surface, line);

            // Check we got a point at 0, 0, 0
            Assert.IsNotNull(point);
            Assert.AreEqual(new Point(), point.ToPoint());
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddPointsToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_POINTS, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemovePointAtTest()
        {
            RemoveAtTest(TestFiles.THREE_POINTS, "2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemovePointTest()
        {
            RemoveTest(TestFiles.THREE_POINTS, "2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearPointsTest()
        {
            ClearTest(TestFiles.THREE_POINTS, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemovePointsFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_POINTS, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierPointTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_POINT), "POINT");
        }

        [Test]
        public void GetPointByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_POINTS));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("2");

            // Check that the correct entity was returned
            Assert.IsTrue(namedEntity.X == -137, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsPointTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_POINT));
        }

        [Test]
        public void CountPointTest()
        {
            CountTest(new File(TestFiles.SINGLE_POINT));
        }

        [Test]
        public void EqualsPointTest()
        {
            EqualsTest();
        }

        [Test]
        public void PointItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_POINT));
        }

        [Test]
        public void PointLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_POINTS));
        }

        #endregion

        #endregion
    }
}