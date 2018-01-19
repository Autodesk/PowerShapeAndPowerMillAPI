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
    /// This is a test class for LineTest and is intended
    /// to contain all LineTest Unit Tests
    /// </summary>
    [TestFixture]
    public class LineTest : EntityTest<PSLine>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Lines;
            }
            catch (Exception e)
            {
                Assert.Fail("_entityCollection could not be set to Active Model's LinesCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void LineIdTest()
        {
            IdTest(TestFiles.SINGLE_LINE);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void LineIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_LINE, "LINE");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void LineEqualsTest()
        {
            Equals(TestFiles.SINGLE_LINE);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void LineExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_LINE);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddLineToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_LINE);
        }

        [Test]
        public void BlankLineTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_LINE);
        }

        [Test]
        public void LineBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_LINE, new Point(-146.1182565, 17.222912, 0), new Point(-108.5, 69, 0));
        }

        [Test]
        public void CopyLineTest()
        {
            DuplicateTest(TestFiles.SINGLE_LINE);
        }

        [Test]
        public void DeleteLineTest()
        {
            DeleteTest(TestFiles.SINGLE_LINE);
        }

        [Test]
        public void LineLevelTest()
        {
            LevelTest(TestFiles.SINGLE_LINE, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void LineLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_LINE, 1);
        }

        [Test]
        [Ignore("")]
        public void LimitLineWithEntitiesTest()
        {
            //LimitToEntitiesTest(TestFiles.SINGLE_LINE, TestFiles.LINE_LIMITERS, PSEntityLimiter.LimitingModes.IntersectCurveMode);
            Assert.Inconclusive();
        }

        [Test]
        public void MirrorLineTest()
        {
            // Setup parameters
            var initialLineStartPoint = new Point(-108.5, 69, 0);
            var initialLineEndPoint = new Point(-146.118256, -17.222912, 0);

            // Carry out operation
            MirrorTest(TestFiles.SINGLE_LINE, initialLineStartPoint);

            // Get point that should now be on mirror point
            var mirroredLine = _powerSHAPE.ActiveModel.Lines[0];
            Assert.AreEqual(initialLineStartPoint, mirroredLine.StartPoint, "Line was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((initialLineStartPoint.X - initialLineEndPoint.X) * 2.0,
                            mirroredLine.EndPoint.X - initialLineEndPoint.X,
                            "Line was not mirrored in correct plane");
        }

        [Test]
        public void MoveLineTest()
        {
            MoveTest(TestFiles.SINGLE_LINE);
        }

        [Test]
        public void MoveLineCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_LINE);
        }

        [Test]
        public void MoveLineDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_LINE, "StartPoint");
        }

        [Test]
        public void LineNameTest()
        {
            NameTest(TestFiles.SINGLE_LINE, "SingleLine", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("line[NewName].EXISTS"));
        }

        [Test]
        public void RemoveLineFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_LINE);
        }

        [Test]
        public void RotateLineTest()
        {
            RotateTest(TestFiles.SINGLE_LINE);
        }

        [Test]
        public void RotateLineCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_LINE);
        }

        /// <summary>
        /// Test the distance between two lines
        /// </summary>
        [Test]
        public void MinimumDistanceLineLine()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Lines[0];
            var entity2 = _powerSHAPE.ActiveModel.Lines[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 14.9421798712929);
        }

        /// <summary>
        /// Test the distance between a line and a point
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceLinePoint()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Lines[0];
            var entity2 = _powerSHAPE.ActiveModel.Points[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(-1.0, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test for Extend
        /// </summary>
        [Test]
        public void ExtendTest()
        {
            // Get curve
            var line = (PSLine) ImportAndGetEntity(TestFiles.SINGLE_LINE);

            // Get initial length
            var initialLength = line.Length.Value;

            // Check curved curve extension
            line.Extend(ExtensionEnd.TWO, 10F, ExtensionType.Curvature);

            // Check remaining points
            Assert.IsTrue(line.Length > initialLength, "Curvature extension failed");
            Assert.AreEqual(new Point(-151.996109, 9.132742, 0),
                            line.EndPoint,
                            "Curvature extension extended to the incorrect point");

            // Update initial length
            initialLength = line.Length.Value;

            // Check straight curve extension
            line.Extend(ExtensionEnd.TWO, 10F, ExtensionType.Linear);

            // Check remaining points
            Assert.IsTrue(line.Length > initialLength, "Straight extension failed");
            Assert.AreEqual(new Point(-157.873961, 1.042572, 0),
                            line.EndPoint,
                            "Linear extension extended to the incorrect point");
        }

        #endregion
    }
}