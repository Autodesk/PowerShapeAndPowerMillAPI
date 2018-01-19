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
    /// This is a test class for WorkplaneTest and is intended
    /// to contain all WorkplaneTest Unit Tests
    /// </summary>
    [TestFixture]
    public class WorkplaneTest : EntityTest<PSWorkplane>
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
                Assert.Fail("_entityCollection could not be set to Active Model's WorkplanesCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void WorkplaneIdTest()
        {
            IdTest(TestFiles.SINGLE_WORKPLANE1);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void WorkplaneIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_WORKPLANE1, "WORKPLANE");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void WorkplaneEqualsTest()
        {
            Equals(TestFiles.SINGLE_WORKPLANE1);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void WorkplaneExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_WORKPLANE1);
        }

        #endregion

        #region Entity Tests

        #region Properties

        [Test]
        public void WorkplaneBoundingBoxTest()
        {
            // Get workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Test bounding box
            Assert.AreEqual(workplane.Origin,
                            workplane.BoundingBox.VolumetricCentre,
                            "Bounding box is in incorrect position");
            Assert.AreEqual(0, workplane.BoundingBox.Volume, "Bounding box incorrectly showed that it had volume");
        }

        [Test]
        public void WorkplaneLevelTest()
        {
            LevelTest(TestFiles.SINGLE_WORKPLANE1, _powerSHAPE.ActiveModel.Levels[0]);
        }

        [Test]
        public void WorkplaneLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_WORKPLANE1, 0);
        }

        [Test]
        public void WorkplaneNameTest()
        {
            NameTest(TestFiles.SINGLE_WORKPLANE1, "SingleWorkplane1", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("workplane[NewName].EXISTS"));
        }

        #endregion

        #region Operations

        [Test]
        public void AddWorkplaneToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_WORKPLANE2, TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void BlankWorkplaneTest()
        {
            BlankTest(TestFiles.THREE_WORKPLANES, TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void CopyWorkplaneTest()
        {
            DuplicateTest(TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void DeleteWorkplaneTest()
        {
            DeleteTest(TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void MirrorWorkplaneTest()
        {
            // Setup parameters
            var mirrorPoint = new Point(-72, 55, 0);

            // Carry out operation
            MirrorTest(TestFiles.SINGLE_WORKPLANE1, mirrorPoint);

            // Get point that should now be on mirror point
            var mirroredWorkplane = _powerSHAPE.ActiveModel.Workplanes[0];
            Assert.AreEqual(new Point(-104, -20, 0),
                            mirroredWorkplane.Origin,
                            "Workplane was not mirrored around correct point");

            // Check that workplane axes are orientated correctly
            Assert.AreEqual(new Vector(-1, 0, 0), mirroredWorkplane.XAxis, "Workplane was not mirrored in correct plane");
            Assert.AreEqual(new Vector(0, -1, 0), mirroredWorkplane.YAxis, "Workplane was not mirrored in correct plane");
        }

        [Test]
        public void MoveWorkplaneTest()
        {
            MoveTest(TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void MoveWorkplaneCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void MoveWorkplaneDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_WORKPLANE1, "Origin");
        }

        [Test]
        public void RotateWorkplaneTest()
        {
            RotateTest(TestFiles.SINGLE_WORKPLANE1);
        }

        [Test]
        public void RotateWorkplaneCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_WORKPLANE1);
        }

        /// <summary>
        /// Test the distance between two workplanes
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceWorkplaneWorkplane()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Workplanes[0];
            var entity2 = _powerSHAPE.ActiveModel.Workplanes[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test the distance between a workplane and an annotation
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceWorkplaneAnnotation()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Workplanes[0];
            var entity2 = _powerSHAPE.ActiveModel.Annotations[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        #endregion

        #endregion

        #region Workplane Tests

        #region Properties

        [Test]
        public void AlignWorkplaneToViewTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Change view
            _powerSHAPE.SetViewAngle(ViewAngles.ViewFromRight);

            // Align workplane to view
            workplane.AlignToView();
            Assert.AreEqual(new Vector(0, 1, 0), workplane.XAxis, "Workplane X Axis is not aligned correctly");
            Assert.AreEqual(new Vector(0, 0, 1), workplane.YAxis, "Workplane Y Axis is not aligned correctly");
        }

        [Test]
        public void IsWorkplaneActiveTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Check that property correctly says it's not active
            Assert.IsFalse(workplane.IsActive, "Workplane property incorrectly says it's active");

            // Make workplane active
            _powerSHAPE.ActiveModel.ActiveWorkplane = workplane;
            Assert.IsTrue(workplane.IsActive, "Workplane property incorrectly says it is not active");
        }

        [Test]
        public void IsWorkplaneLockedTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Check that property does not say it's locked
            Assert.IsFalse(workplane.IsLocked, "Workplane property incorrectly says it's locked");

            // Lock workplane
            workplane.IsLocked = true;
            Assert.IsTrue(workplane.IsLocked, "Workplane property incorrectly says it isn't locked");
        }

        [Test]
        public void WorkplaneOriginTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Check origin
            Assert.AreEqual(new Point(-40, -20, 0), workplane.Origin, "Workplane origin is incorrect");
        }

        [Test]
        public void WorkplaneToWorkplaneTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Convert to Geometry.Workplane
            var convertedWorkplane = workplane.ToWorkplane();
            Assert.AreEqual(convertedWorkplane.Origin, workplane.Origin, "Workplane origin converted incorrectly");
            Assert.AreEqual(convertedWorkplane.XAxis, workplane.XAxis, "Workplane XAxis converted incorrectly");
            Assert.AreEqual(convertedWorkplane.YAxis, workplane.YAxis, "Workplane YAxis converted incorrectly");
        }

        [Test]
        public void WorkplaneXAxisTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Rotate workplane
            workplane.Rotate(Axes.Y, 90, 0, workplane.Origin);
            Assert.AreEqual(new Vector(0, 0, -1), workplane.XAxis, "XAxis property returns the incorrect value");

            // Set X Axis
            workplane.XAxis = new Vector(0, 1, 0);
            Assert.AreEqual(new Vector(0, 0, 1), workplane.YAxis, "X Axis was not set correctly");
        }

        [Test]
        public void WorkplaneYAxisTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Rotate workplane
            workplane.Rotate(Axes.Z, 90, 0, workplane.Origin);
            Assert.AreEqual(new Vector(-1, 0, 0), workplane.YAxis, "YAxis property returns the incorrect value");

            // Set Y Axis
            workplane.YAxis = new Vector(0, 0, 1);
            Assert.AreEqual(new Vector(1, 0, 0), workplane.ZAxis, "Y Axis was not set correctly");
        }

        [Test]
        public void WorkplaneZAxisTest()
        {
            // Import workplane
            var workplane = (PSWorkplane) ImportAndGetEntity(TestFiles.SINGLE_WORKPLANE1);

            // Rotate workplane
            workplane.Rotate(Axes.X, 90, 0, workplane.Origin);
            Assert.AreEqual(new Vector(0, -1, 0), workplane.ZAxis, "ZAxis property returns the incorrect value");

            // Set Z Axis
            workplane.ZAxis = new Vector(1, 0, 0);
            Assert.AreEqual(new Vector(0, 1, 0), workplane.XAxis, "Z Axis was not set correctly");
        }

        #endregion

        #endregion
    }
}