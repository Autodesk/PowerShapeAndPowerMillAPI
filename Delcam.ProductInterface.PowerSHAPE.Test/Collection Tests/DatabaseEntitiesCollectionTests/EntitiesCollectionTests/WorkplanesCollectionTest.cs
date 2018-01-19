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
    /// This is a test class for WorkplanesCollectionTest and is intended
    /// to contain all WorkplanesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class WorkplanesCollectionTest : EntitiesCollectionTest<PSWorkplane>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Workplanes;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's WorkplanesCollection\n\n" +
                            e.Message);
            }
        }

        #region WorkplanesCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddWorkplanesToSelectionTest()
        {
            AddToSelectionTest(TestFiles.THREE_WORKPLANES, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for CreateWorkplane
        /// </summary>
        [Test]
        public void CreateWorkplaneTest()
        {
            // Create single workplane
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplane(new Workplane(new Point(0, 10, 20),
                                                                             new Vector(1, 0, 0),
                                                                             new Vector(0, 1, 0),
                                                                             new Vector(0, 0, 1)));
            Assert.AreEqual(_powerSHAPE.ActiveModel.Workplanes.Count, 1, "Failed to add workplane to collection");

            // Doesn't work, CreatedItems is cleared every time an axis is changed in the constructor.
            //Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create workplane in PowerSHAPE");
        }

        /// <summary>
        /// A test for CreateWorkplane
        /// </summary>
        [Test]
        public void CreateWorkplaneZAxisTest()
        {
            // Create single workplane
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneFromZAxis(new Point(0, 10, 20),
                                                                        new Vector(0, 0, 1));
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Workplanes.Count, "Failed to add workplane to collection");
            Assert.AreEqual((MM) 1.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().XAxis.I);
            Assert.AreEqual((MM) 0.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().XAxis.J);
            Assert.AreEqual((MM) 0.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().XAxis.K);
            Assert.AreEqual((MM) 0.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().YAxis.I);
            Assert.AreEqual((MM) 1.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().YAxis.J);
            Assert.AreEqual((MM) 0.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().YAxis.K);
        }

        /// <summary>
        /// A test for CreateWorkplane
        /// </summary>
        [Test]
        public void CreateWorkplaneXYAxesTest()
        {
            // Create single workplane
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneFromXYAxes(new Point(0, 10, 20),
                                                                         new Vector(1, 0, 0),
                                                                         new Vector(0, 1, 0));
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Workplanes.Count, "Failed to add workplane to collection");
            Assert.AreEqual((MM) 0.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().ZAxis.I);
            Assert.AreEqual((MM) 0.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().ZAxis.J);
            Assert.AreEqual((MM) 1.0, _powerSHAPE.ActiveModel.Workplanes[0].ToWorkplane().ZAxis.K);
        }

        /// <summary>
        /// A test for CreateWorkplaneAtEntityCentre
        /// </summary>
        [Test]
        public void CreateWorkplaneAtEntityCentreTest()
        {
            // Import solid
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SOLID));

            // Get solid
            var solidForWorkplane = _powerSHAPE.ActiveModel.Solids[0];

            // Create workplane at centre of solid
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneAtEntity(solidForWorkplane,
                                                                       NewWorkplaneInEntityPositions.Centre);

            // Check that workplane was created
            Assert.AreEqual(_powerSHAPE.ActiveModel.Workplanes.Count, 1, "Failed to add workplane to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create workplane in PowerSHAPE");

            // Get created workplane
            var createdWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Check that workplane was created in correct position
            Assert.AreEqual(createdWorkplane.Origin, new Point(-70, 3, 20), "Workplane created in incorrect position");

            // Delete workplane
            createdWorkplane.Delete();
        }

        /// <summary>
        /// A test for CreateWorkplaneAtEntityTop
        /// </summary>
        [Test]
        public void CreateWorkplaneAtEntityTopTest()
        {
            // Import solid
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SOLID));

            // Get solid
            var solidForWorkplane = _powerSHAPE.ActiveModel.Solids[0];

            // Create workplane at top of solid
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneAtEntity(solidForWorkplane,
                                                                       NewWorkplaneInEntityPositions.Top);

            // Check that workplane was created
            Assert.AreEqual(_powerSHAPE.ActiveModel.Workplanes.Count, 1, "Failed to add workplane to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create workplane in PowerSHAPE");

            // Get created workplane
            var createdWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Check that workplane was created in correct position
            Assert.AreEqual(createdWorkplane.Origin, new Point(-70, 3, 40), "Workplane created in incorrect position");

            // Delete workplane
            createdWorkplane.Delete();
        }

        /// <summary>
        /// A test for CreateWorkplaneAtEntityBottom
        /// </summary>
        [Test]
        public void CreateWorkplaneAtEntityBottomTest()
        {
            // Import solid
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SOLID));

            // Get solid
            var solidForWorkplane = _powerSHAPE.ActiveModel.Solids[0];

            // Create workplane at bottom of solid
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneAtEntity(solidForWorkplane,
                                                                       NewWorkplaneInEntityPositions.Bottom);

            // Check that workplane was created
            Assert.AreEqual(_powerSHAPE.ActiveModel.Workplanes.Count, 1, "Failed to add workplane to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create workplane in PowerSHAPE");

            // Get created workplane
            var createdWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Check that workplane was created in correct position
            Assert.AreEqual(createdWorkplane.Origin, new Point(-70, 3, 0), "Workplane created in incorrect position");

            // Delete workplane
            createdWorkplane.Delete();
        }

        /// <summary>
        /// A test for CreateWorkplaneAlignedToEntity
        /// </summary>
        [Test]
        public void CreateWorkplaneAlignedToEntityTest()
        {
            // Import line
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_LINE));

            // Get line
            var lineForWorkplane = _powerSHAPE.ActiveModel.Lines[0];

            // Create workplane at end of line
            _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneAlignedToEntity(lineForWorkplane,
                                                                              lineForWorkplane.EndPoint);

            // Check that workplane was created
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Workplanes.Count, "Failed to add workplane to collection");
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.CreatedItems.Count, "Failed to create workplane in PowerSHAPE");

            // Get created workplane
            var createdWorkplane = (PSWorkplane) _powerSHAPE.ActiveModel.CreatedItems[0];

            // Deactivate workplane
            _powerSHAPE.ActiveModel.ActiveWorkplane = null;

            // Check that workplane was created in correct position
            var lineVector = new Vector(lineForWorkplane.StartPoint, lineForWorkplane.EndPoint);
            lineVector.Normalize();
            Assert.AreEqual(lineForWorkplane.EndPoint,
                            createdWorkplane.Origin,
                            "Workplane created in incorrect position");
            Assert.AreEqual(lineVector, createdWorkplane.ZAxis, "Workplane created at incorrect orientation");
        }

        /// <summary>
        /// A test for CreateWorkplaneAlignedToView
        /// </summary>
        [Test]
        public void CreateWorkplaneAlignedToViewTest()
        {
            // Align the view to top
            _powerSHAPE.SetViewAngle(ViewAngles.ViewFromTop);

            // Create the workplane
            var workplane = _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneAlignedToView(new Point());

            // Check z axis is postivie in world Z
            Assert.AreEqual((MM) 1.0, workplane.ZAxis.K);

            // Create a second one
            workplane.Delete();

            // Align the view to bottom
            _powerSHAPE.SetViewAngle(ViewAngles.ViewFromBottom);

            // Create the workplane
            workplane = _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneAlignedToView(new Point());

            // Check z axis is negative in world Z
            Assert.AreEqual((MM) (-1.0), workplane.ZAxis.K);

            // Tidy
            workplane.Delete();
        }

        /// <summary>
        /// A test for create workplane from three points
        /// </summary>
        [Test]
        public void CreateWorkplaneFromThreePoints()
        {
            var point1 = _powerSHAPE.ActiveModel.Points.CreatePoint(
                new Point(150.217864990234, -132.969268798828, 21.835262298584));
            var point2 = _powerSHAPE.ActiveModel.Points.CreatePoint(
                new Point(150.142395019531, -128.937210083008, 21.4710826873779));
            var worplane = _powerSHAPE.ActiveModel.Workplanes.CreateWorkplaneFromThreePoints(
                new Point(149.091873168945, -132.765274047852, 22.1432628631592),
                new Point(150.217864990234, -132.969268798828, 21.835262298584),
                new Point(150.142395019531, -128.937210083008, 21.4710826873779));

            // If the operation was successful then both of the points should now sit on the XY plane
            Assert.AreEqual((MM) 0.0, point1.Z);
            Assert.AreEqual((MM) 0.0, point2.Z);
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveWorkplaneAtTest()
        {
            RemoveAtTest(TestFiles.THREE_WORKPLANES, "Workplane2");
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveWorkplaneTest()
        {
            RemoveTest(TestFiles.THREE_WORKPLANES, "Workplane2");
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearWorkplanesTest()
        {
            ClearTest(TestFiles.THREE_WORKPLANES, TestFiles.SINGLE_SURFACE);
        }

        // TODO: Find macro code to remove a workplane from the selection
        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveWorkplanesFromSelectionTest()
        {
            // Import multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_WORKPLANES));

            // Import single entity
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_WORKPLANE1));

            // Get single entity
            var testEntity = _powerSHAPE.ActiveModel.SelectedItems[0];

            // Select everything
            _powerSHAPE.ActiveModel.Workplanes.AddToSelection();

            // Remove entities from selection
            ((PSWorkplanesCollection) _powerSHAPECollection).RemoveFromSelection();

            // Check the selection was cleared
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 4) // Nothing was deselected
            {
                Assert.Fail("Deselected nothing in PowerSHAPE");
            }
            else if (_powerSHAPE.ActiveModel.SelectedItems.Count != 0) // Everything was deselected
            {
                Assert.Fail("Failed to remove all " + _collectionType.ToLower() + "s from selection");
            }
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierWorkplaneTest()
        {
            IdentifierTest(new File(TestFiles.SINGLE_WORKPLANE1), "WORKPLANE");
        }

        [Test]
        public void GetWorkplaneByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_WORKPLANES));

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("Workplane2");

            // Check that the correct entity was returned
            Assert.AreEqual((MM) (-12), namedEntity.Origin.X, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsWorkplaneTest()
        {
            ContainsTest(new File(TestFiles.SINGLE_WORKPLANE1));
        }

        [Test]
        public void CountWorkplaneTest()
        {
            CountTest(new File(TestFiles.SINGLE_WORKPLANE1));
        }

        [Test]
        public void EqualsWorkplaneTest()
        {
            EqualsTest();
        }

        [Test]
        public void WorkplaneItemTest()
        {
            ItemTest(new File(TestFiles.SINGLE_WORKPLANE1));
        }

        [Test]
        public void WorkplaneLastItemTest()
        {
            LastItemTest(new File(TestFiles.THREE_WORKPLANES));
        }

        #endregion

        #endregion
    }
}