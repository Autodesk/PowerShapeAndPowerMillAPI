// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for AnnotationTest and is intended
    /// to contain all AnnotationTest Unit Tests
    /// </summary>
    [TestFixture]
    public class AnnotationTest : EntityTest<PSAnnotation>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Annotations;
            }
            catch (Exception e)
            {
                Assert.Fail("_entityCollection could not be set to Active Model's AnnotationsCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void AnnotationIdTest()
        {
            IdTest(TestFiles.SINGLE_ANNOTATION);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void AnnotationIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_ANNOTATION, "TEXT");
        }

        ///// <summary>
        ///// A test for Equals
        ///// </summary>
        //[Test]
        //public void AnnotationEqualsTest()
        //{
        //    base.Equals(TestFiles.SINGLE_ANNOTATION);
        //}

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void AnnotationExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_ANNOTATION);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddAnnotationToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_ANNOTATION);
        }

        [Test]
        public void BlankAnnotationTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_ANNOTATION);
        }

        [Test]
        public void AnnotationBoundingBoxTest()
        {
            try
            {
                BoundingBoxTest(TestFiles.SINGLE_ANNOTATION, new Point(), new Point());
                Assert.Fail("Exception not thrown");
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CopyAnnotationTest()
        {
            DuplicateTest(TestFiles.SINGLE_ANNOTATION);
        }

        [Test]
        public void DeleteAnnotationTest()
        {
            DeleteTest(TestFiles.SINGLE_ANNOTATION);
        }

        [Test]
        public void AnnotationLevelTest()
        {
            LevelTest(TestFiles.SINGLE_ANNOTATION, _powerSHAPE.ActiveModel.Levels[0]);
        }

        [Test]
        public void AnnotationLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_ANNOTATION, 0);
        }

        //[Test]
        //public void LimitAnnotationWithEntitiesTest()
        //{
        //    LimitToEntitiesTest(TestFiles.SINGLE_ANNOTATION, TestFiles.Annotation_LIMITERS, PSEntityLimiter.LimitingModes.IntersectCurveMode);
        //}

        //[Test]
        //public void MirrorAnnotationTest()
        //{
        //    // Setup parameters
        //    Point initialAnnotationStartPoint = new Point((MM)(-72), (MM)55, (MM)0);
        //    Point initialAnnotationEndPoint = new Point((MM)(-125), (MM)(-47), (MM)0);

        //    // Carry out operation
        //    MirrorTest(TestFiles.SINGLE_ANNOTATION, initialAnnotationStartPoint);

        //    // Get point that should now be on mirror point
        //    PSAnnotation mirroredAnnotation = _powerSHAPE.ActiveModel.Annotations[0];
        //    Assert.AreEqual(initialAnnotationStartPoint, mirroredAnnotation.StartPoint, "Annotation was not mirrored around correct point");

        //    // Check that second test point is now the correct distance from its initial position
        //    Assert.AreEqual((initialAnnotationStartPoint.X - initialAnnotationEndPoint.X) * 2, mirroredAnnotation.EndPoint.X - initialAnnotationEndPoint.X, "Annotation was not mirrored in correct plane");
        //}

        //[Test]
        //public void MoveAnnotationTest()
        //{
        //    MoveTest(TestFiles.SINGLE_ANNOTATION);
        //}

        //[Test]
        //public void MoveAnnotationCreatingCopiesTest()
        //{
        //    MoveCreatingCopiesTest(TestFiles.SINGLE_ANNOTATION);
        //}

        //[Test]
        //public void MoveAnnotationDifferentOriginTest()
        //{
        //    MoveDifferentOriginTest(TestFiles.SINGLE_ANNOTATION, "Centre");
        //}

        [Test]
        public void AnnotationNameTest()
        {
            NameTest(TestFiles.SINGLE_ANNOTATION, "ig1_1", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual("1", _powerSHAPE.ExecuteEx("text[NewName].EXISTS").ToString());
        }

        [Test]
        public void RemoveAnnotationFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_ANNOTATION);
        }

        #endregion

        #region Annotation Tests

        [Test]
        public void TextTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(0, 0, 0));
            annotation.Name = "New Name";

            Assert.AreEqual(annotation.Name, "New Name");
        }

        [Test]
        public void PositionTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            var position = annotation.Position;
            Assert.AreEqual(position.X, 1);
            Assert.AreEqual(position.Y, 0);
            Assert.AreEqual(position.Z, 0);
        }

        [Test]
        public void FontTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Font = "Calibri";
            Assert.AreEqual(annotation.Font, "Calibri");
        }

        [Test]
        public void HeightTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Height = 20;
            Assert.AreEqual(annotation.Height, 20);
        }

        [Test]
        public void ItalicTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Italic = true;
            Assert.IsTrue(annotation.Italic);
        }

        [Test]
        public void SpacingTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Spacing = 5;
            Assert.AreEqual(annotation.Spacing, 5);
        }

        [Test]
        public void AngleTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Angle = 45;
            Assert.AreEqual(annotation.Angle.Value, 45);
        }

        [Test]
        public void JustificationTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Justification = TextJustifications.Right;
            Assert.AreEqual(annotation.Justification, TextJustifications.Right);
        }

        [Test]
        public void OriginTest()
        {
            var annotation = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Hello", "Arial", 12, new Point(1, 0, 0));

            annotation.Origin = TextOrigins.LeftCentre;
            Assert.AreEqual(annotation.Origin, TextOrigins.LeftCentre);
        }

        #endregion
    }
}