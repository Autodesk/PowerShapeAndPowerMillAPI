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
    public class ArcTest : EntityTest<PSArc>
    {
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
                Assert.Fail("_entityCollection could not be set to Active Model's ArcsCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void ArcIdTest()
        {
            IdTest(TestFiles.SINGLE_ARC);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void ArcIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_ARC, "ARC");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void ArcEqualsTest()
        {
            Equals(TestFiles.SINGLE_ARC);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void ArcExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_ARC);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddArcToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_ARC);
        }

        [Test]
        public void BlankArcTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_ARC);
        }

        [Test]
        public void ArcBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_ARC, new Point(-143.213966, -47, 0), new Point(-72, 56.057257, 0));
        }

        [Test]
        public void CopyArcTest()
        {
            DuplicateTest(TestFiles.SINGLE_ARC);
        }

        [Test]
        public void DeleteArcTest()
        {
            DeleteTest(TestFiles.SINGLE_ARC);
        }

        [Test]
        public void ArcLevelTest()
        {
            LevelTest(TestFiles.SINGLE_ARC, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void ArcLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_ARC, 1);
        }

        [Test]
        public void LimitArcWithEntitiesTest()
        {
            LimitToEntitiesTest(TestFiles.SINGLE_ARC, TestFiles.ARC_LIMITERS, LimitingModes.IntersectCurveMode);
        }

        [Test]
        public void MirrorArcTest()
        {
            // Setup parameters
            var initialArcStartPoint = new Point(-72, 55, 0);
            var initialArcEndPoint = new Point(-125, -47, 0);

            // Carry out operation
            MirrorTest(TestFiles.SINGLE_ARC, initialArcStartPoint);

            // Get point that should now be on mirror point
            var mirroredArc = _powerSHAPE.ActiveModel.Arcs[0];
            Assert.AreEqual(initialArcStartPoint, mirroredArc.StartPoint, "Arc was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((initialArcStartPoint.X - initialArcEndPoint.X) * 2.0,
                            mirroredArc.EndPoint.X - initialArcEndPoint.X,
                            "Arc was not mirrored in correct plane");
        }

        [Test]
        public void OffsetArcTest()
        {
            OffsetTest(TestFiles.ARC_TO_OFFSET, new BoundingBox(-96.75, -70.75, 37.5, 63.5, 0, 0));
        }

        [Test]
        public void MoveArcTest()
        {
            MoveTest(TestFiles.SINGLE_ARC);
        }

        [Test]
        public void MoveArcCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_ARC);
        }

        [Test]
        public void MoveArcDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_ARC, "Centre");
        }

        [Test]
        public void ArcNameTest()
        {
            NameTest(TestFiles.SINGLE_ARC, "SingleArc", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("arc[NewName].EXISTS"));
        }

        [Test]
        public void RemoveArcFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_ARC);
        }

        [Test]
        public void RotateArcTest()
        {
            RotateTest(TestFiles.SINGLE_ARC);
        }

        [Test]
        public void RotateArcCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_ARC);
        }

        /// <summary>
        /// Test the distance between two arcs
        /// </summary>
        [Test]
        public void MinimumDistanceArcArc()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Arcs[0];
            var entity2 = _powerSHAPE.ActiveModel.Arcs[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 32.299963352773773);
        }

        /// <summary>
        /// Test the distance between an arc and a compcurve
        /// </summary>
        [Test]
        public void MinimumDistanceArcCompCurve()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Arcs[0];
            var entity2 = _powerSHAPE.ActiveModel.CompCurves[1];

            // Test the distance
            Assert.IsTrue(entity1.DistanceTo(entity2) == 38.606830381235554);
        }

        #endregion

        #region Arc Tests

        #region Properties

        /// <summary>
        /// A test for Centre
        /// </summary>
        [Test]
        public void CentreTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(new Point(-83.213966, -3.942743, 0), arc.Centre, "Centre is not as expected");
        }

        /// <summary>
        /// A test for Centre of Gravity
        /// </summary>
        [Test]
        public void ArcCentreOfGravityTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check bounding box
            Assert.AreEqual(new Point(-107.606983, 4.5286285, 0), arc.CentreOfGravity, "Centre of Gravity is incorrect");
        }

        /// <summary>
        /// A test for EndPoint
        /// </summary>
        [Test]
        public void EndPointTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(new Point(-125, -47, 0), arc.EndPoint, "End point is not as expected");
        }

        /// <summary>
        /// A test for Length
        /// </summary>
        [Test]
        public void LengthTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.IsTrue(arc.Length == 153.550894, "Length is not as expected");
        }

        /// <summary>
        /// A test for Radius
        /// </summary>
        [Test]
        public void RadiusTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.IsTrue(arc.Radius == 60, "Radius is not as expected");
        }

        /// <summary>
        /// A test for Span
        /// </summary>
        [Test]
        public void SpanTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual((Degree) 146.630303, arc.Span, "Span is not as expected");
        }

        /// <summary>
        /// A test for StartPoint
        /// </summary>
        [Test]
        public void StartPointTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(new Point(-72, 55, 0), arc.StartPoint, "Start point is not as expected");
        }

        /// <summary>
        /// A test for Through
        /// </summary>
        [Test]
        public void ThroughTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(new Point(-136.455518, 23.721985, 0), arc.Through, "Through is not as expected");
        }

        /// <summary>
        /// A test for Midpoint
        /// </summary>
        [Test]
        public void MidPointTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(new Point(-136.455518, 23.721985, 0), arc.MidPoint, "Midpoint is not as expected");
        }

        /// <summary>
        /// A test for Start Angle
        /// </summary>
        [Test]
        public void StartAngleTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual((Degree) 79.228106, arc.StartAngle, "Start Angle is not as expected");
        }

        /// <summary>
        /// A test for End Angle
        /// </summary>
        [Test]
        public void EndAngleTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual((Degree) 225.858409, arc.EndAngle, "End Angle is not as expected");
        }

        /// <summary>
        /// A test for Points
        /// </summary>
        [Test]
        public void PointsTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(arc.StartPoint, arc.Points[0], "Start Point is not in Points list");
            Assert.AreEqual(arc.EndPoint, arc.Points[1], "End Point is not in Points list");
        }

        /// <summary>
        /// A test for NumberPoints
        /// </summary>
        [Test]
        public void NumberPointsTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Check property returns correct value
            Assert.AreEqual(2, arc.NumberPoints, "Incorrect number of points returned");
        }

        #endregion

        #region Operations

        /// <summary>
        /// A test for Full
        /// </summary>
        [Test]
        public void FullTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Set span to be 360deg
            arc.Full();

            // Check operation was successful
            Assert.AreEqual((Degree) 360, arc.Span, "Span is actually " + arc.Span);
        }

        /// <summary>
        /// A test for Reverse
        /// </summary>
        [Test]
        public void ReverseTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Get start point
            var initialStartPoint = arc.StartPoint;

            // Reverse arc
            arc.Reverse();

            // Check operation was successful
            Assert.AreEqual(initialStartPoint, arc.EndPoint, "Arc was not reversed");
        }

        /// <summary>
        /// A test for ScaleToCurve
        /// </summary>
        [Test]
        public void ScaleToCurveTest()
        {
            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Get expected end point
            var expectedStartPoint = arc.StartPoint + new Vector(arc.Centre, arc.StartPoint);

            // Scale arc
            var newCurve = arc.ScaleToCurve(2, 2, 0, arc.Centre);

            // Check operation was successful
            Assert.IsFalse(arc.Exists, "Arc still exists");
            Assert.IsTrue(newCurve.Exists, "Curve doesn't exist");
            Assert.AreEqual(expectedStartPoint, newCurve.StartPoint, "Arc was not scaled correctly");
        }

        [Test]
        [Ignore("Need to resolve why the cutting code isn't working at the moment")]
        public void CutArcTest()
        {
            Assert.Inconclusive("Need to resolve why the cutting code isn't working at the moment");

            // Get arc
            var arc = (PSArc) ImportAndGetEntity(TestFiles.SINGLE_ARC);

            // Cut arc
            var newArc = (PSArc) arc.Cut(new Point(-143.186042, -5.773076, 0));

            // Check that new arc segment has been returned
            Assert.IsTrue(newArc.Exists, "New arc was not created");
            Assert.AreEqual(newArc.Span, 44.1103, "New arc segment does not have correct properties");

            // Check that original arc still remains
            Assert.AreEqual(arc.Span, 102.52, "Original arc segment has not been cut correctly");
        }

        #endregion

        #endregion
    }
}