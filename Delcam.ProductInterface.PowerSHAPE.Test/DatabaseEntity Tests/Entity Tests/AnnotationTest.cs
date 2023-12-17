// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Reflection;
using Autodesk.FileSystem;
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
            // Creating rather than importing as 2022 and above doesn't support importing
            var importedEntity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            // Check that the id is correct
            Assert.AreEqual(_powerSHAPE.ExecuteEx(IdentifierAccessor(importedEntity) + "[" + importedEntity.Name + "].ID"),
                            importedEntity.Id,
                            "Returned incorrect id");
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void AnnotationIdentifierTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var importedEntity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            // Check that the collection Identifier matches the entity type
            string actualIdentifier = (string)importedEntity
                                              .GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance)
                                              .GetValue(importedEntity, null);
            Assert.AreEqual("TEXT", actualIdentifier);
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
            // Creating rather than importing as 2022 and above doesn't support importing
            var entity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            // Check that correct value is returned
            Assert.IsTrue(entity.Exists, "Incorrectly stated that entity did not exist");

            // Delete entity and check again
            entity.Delete();
            Assert.IsFalse(entity.Exists, "Incorrectly stated that entity existed");
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddAnnotationToSelectionTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var entityToSelect = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            var otherEntityFile = TestFiles.SINGLE_SURFACE;

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(true);

            // Add desired entity to selection, cancelling current selection
            entityToSelect.AddToSelection(true);

            var count = _powerSHAPE.ActiveModel.SelectedItems.Count;

            // Check the selection contains the correct entity
            if (count == 0)
            {
                Assert.Fail("Failed to select anything in PowerSHAPE");
            }
            else if (count == 2)
            {
                Assert.Fail("Selected everything in PowerSHAPE");
            }

            // Check the other entity is not selected
            Assert.IsTrue(_powerSHAPE.ActiveModel.SelectedItems.Contains(entityToSelect), "Select incorrect entity");

        }

        [Test]
        public void BlankAnnotationTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var entityToBlank = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            var otherEntitiesFile = TestFiles.THREE_SURFACES;

            // Import other entities
            _powerSHAPE.ActiveModel.Import(new File(otherEntitiesFile));

            // Blank entity
            entityToBlank.Blank();

            // Check that entity has been blanked
            _powerSHAPE.ActiveModel.SelectAll(false);
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                _powerSHAPE.ActiveModel.Workplanes.AddToSelection(true);
            }
            Assert.AreEqual(3, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity was not blanked");
        }

        [Test]
        public void AnnotationBoundingBoxTest()
        {
            try
            {
                // Creating rather than importing as 2022 and above doesn't support importing
                var entity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

                // Check bounding box
                Assert.AreEqual(new Point(), entity.BoundingBox.MaximumBounds, "Bounding box maximum bounds are incorrect");
                Assert.AreEqual(new Point(), entity.BoundingBox.MinimumBounds, "Bounding box minimum bounds are incorrect");
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
            // Creating rather than importing as 2022 and above doesn't support importing
            var entityToCopy = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            // Copy and paste entity
            entityToCopy.Duplicate();
            _powerSHAPE.ActiveModel.SelectAll(false);
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                _powerSHAPE.ActiveModel.Workplanes.AddToSelection(false);
            }
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity was not pasted"); }

        [Test]
        public void DeleteAnnotationTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var entityToDelete = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            // Delete entity
            entityToDelete.Delete();
            _powerSHAPE.ActiveModel.SelectAll(false);
            Assert.AreEqual(0, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity not deleted from PowerSHAPE");
            Assert.AreEqual(0, _powerSHAPECollection.Count, "Entity not removed from collection");
        }

        [Test]
        public void AnnotationLevelTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var entity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            var expectedLevel = _powerSHAPE.ActiveModel.Levels[0];
            // Check level
            Assert.AreEqual(expectedLevel, entity.Level, "Returned level is incorrect");

            // Change level
            var newLevel = _powerSHAPE.ActiveModel.Levels[expectedLevel.Number + 1];
            entity.Level = newLevel;
            Assert.AreEqual(newLevel, entity.Level, "Failed to change level");
        }

        [Test]
        public void AnnotationLevelNumberTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var entity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            uint expectedLevelNumber = 0;

            // Check level
            Assert.AreEqual(expectedLevelNumber, entity.LevelNumber, "Returned level number is incorrect");

            // Change level number
            uint newLevelNumber = 1;
            entity.LevelNumber = newLevelNumber;
            Assert.AreEqual(newLevelNumber, entity.LevelNumber, "Failed to change level");
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
            // Creating rather than importing as 2022 and above doesn't support importing
            var entity = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            var expectedName = "1";
            var newName = "NewName";

            // Check name
            Assert.AreEqual(expectedName, entity.Name, "Returned name is incorrect");

            // Change name
            entity.Name = newName;
            Assert.AreEqual(newName, entity.Name, "Entity has not been renamed");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual("1", _powerSHAPE.ExecuteEx("text[NewName].EXISTS").ToString());
        }

        [Test]
        public void RemoveAnnotationFromSelectionTest()
        {
            // Creating rather than importing as 2022 and above doesn't support importing
            var entityToDeselect = _powerSHAPE.ActiveModel.Annotations.CreateAnnotation("Test Text", "Arial", 20, new Point());

            var otherEntityFile = TestFiles.SINGLE_SURFACE;

            // Get other entity
            var otherEntity = ImportAndGetEntity(otherEntityFile);

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(true);
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                _powerSHAPE.ActiveModel.Workplanes.AddToSelection(false);
            }

            // Remove desired entity from selection, cancelling current selection
            entityToDeselect.RemoveFromSelection();

            // Check the selection doesn't contain the entity
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0) // Everything was deselected
            {
                Assert.Fail("Deselected everything in PowerSHAPE");
            }
            else if (_powerSHAPE.ActiveModel.SelectedItems.Count == 2) // Nothing was deselected
            {
                Assert.Fail("Nothing was deselected in PowerSHAPE");
            }

            // Check the entity is not selected
            Assert.IsTrue(_powerSHAPE.ActiveModel.SelectedItems.Contains(otherEntity), "Deselected incorrect entity");
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