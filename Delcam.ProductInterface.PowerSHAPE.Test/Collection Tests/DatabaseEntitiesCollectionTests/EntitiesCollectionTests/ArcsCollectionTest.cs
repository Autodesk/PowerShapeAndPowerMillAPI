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
    /// This is a test class for ArcsCollectionTest and is intended
    /// to contain all ArcsCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class ArcsCollectionTest : EntitiesCollectionTest<PSArc>
    {
        #region Constructors

        public ArcsCollectionTest()
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
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Arcs;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's ArcsCollection\n\n" + e.Message);
            }
        }

        #region ArcsCollection Tests

        /// <summary>
        /// A test for CreateArc
        /// </summary>
        [Test]
        public void CreateCircleTest()
        {
            // Create full arc
            _powerSHAPE.ActiveModel.Arcs.CreateArcCircle(new Point(20, 20, 0),
                                                         new Point(30, 20, 0),
                                                         10);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Arcs.Count, 1, "Failed to add arc to collection");
        }

        /// <summary>
        /// A test for CreateArc
        /// </summary>
        [Test]
        public void CreateArcFromPointsRadiusTest()
        {
            // Create partial arc
            _powerSHAPE.ActiveModel.Arcs.CreateArcSpanExplicit(new Point(20, 20, 0),
                                                               new Point(30, 20, 0),
                                                               10,
                                                               90);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Arcs.Count, 1, "Failed to add arc to collection");
        }

        /// <summary>
        /// A test for CreateArc
        /// </summary>
        [Test]
        public void CreateArcFromCentreStartEndPointsTest()
        {
            // Create partial arc
            _powerSHAPE.ActiveModel.Arcs.CreateArcSpanImplicit(new Point(20, 20, 0),
                                                               new Point(30, 20, 0),
                                                               new Point(20, 30, 0));
            Assert.AreEqual(_powerSHAPE.ActiveModel.Arcs.Count, 1, "Failed to add arc to collection");
        }

        /// <summary>
        /// A test for CreateArcThroughThreePoints
        /// </summary>
        [Test]
        public void CreateArcThroughThreePointsTest()
        {
            var arc = _powerSHAPE.ActiveModel.Arcs.CreateArcThroughThreePoints(new Point(20, 20, 0),
                                                                               new Point(70, 20, 0),
                                                                               new Point(45, 45, 0));
            Assert.AreEqual((MM) 25.0, arc.Radius);
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddArcsToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_ARCS, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveArcAtTest()
        {
            RemoveAtTest(TestFiles.THREE_ARCS, "Arc2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveArcTest()
        {
            RemoveTest(TestFiles.THREE_ARCS, "Arc2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearArcsTest()
        {
            ClearTest(TestFiles.THREE_ARCS, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveArcsFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_ARCS, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierArcTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_ARC), "ARC");
        }

        [Test]
        public void GetArcByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_ARCS));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("Arc2");

            // Check that the correct entity was returned
            Assert.IsTrue(namedEntity.Radius == 25, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsArcTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_ARC));
        }

        [Test]
        public void CountArcTest()
        {
            CountTest(new File(TestFiles.SINGLE_ARC));
        }

        [Test]
        public void EqualsArcTest()
        {
            EqualsTest();
        }

        [Test]
        public void ArcItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_ARC));
        }

        [Test]
        public void ArcLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_ARCS));
        }

        #endregion

        #endregion
    }
}