// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for EntityTest and is intended
    /// to contain all EntityTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class EntityTest<T> : DatabaseEntityTest<T> where T : PSEntity
    {
        #region Entity Tests

        #region Operations

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        public void AddToSelectionTest(string otherEntityFile, string entityToSelectFile)
        {
            // Get entity to select
            var entityToSelect = ImportAndGetEntity(entityToSelectFile);

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

        /// <summary>
        /// A test for Blank
        /// </summary>
        public void BlankTest(string otherEntitiesFile, string entityToBlankFile)
        {
            // Import other entities
            _powerSHAPE.ActiveModel.Import(new File(otherEntitiesFile));

            // Get entity to blank
            var entityToBlank = ImportAndGetEntity(entityToBlankFile);

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

        /// <summary>
        /// A test for Copy
        /// </summary>
        public void DuplicateTest(string entityToCopyFile)
        {
            // Get entity
            var entityToCopy = ImportAndGetEntity(entityToCopyFile);

            // Copy and paste entity
            entityToCopy.Duplicate();
            _powerSHAPE.ActiveModel.SelectAll(false);
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0)
            {
                _powerSHAPE.ActiveModel.Workplanes.AddToSelection(false);
            }
            Assert.AreEqual(2, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity was not pasted");
        }

        /// <summary>
        /// A test for Delete
        /// </summary>
        public void DeleteTest(string entityToDeleteFile)
        {
            // Get entity
            var entityToDelete = ImportAndGetEntity(entityToDeleteFile);

            // Delete entity
            entityToDelete.Delete();
            _powerSHAPE.ActiveModel.SelectAll(false);
            Assert.AreEqual(0, _powerSHAPE.ActiveModel.SelectedItems.Count, "Entity not deleted from PowerSHAPE");
            Assert.AreEqual(0, _powerSHAPECollection.Count, "Entity not removed from collection");
        }

        /// <summary>
        /// A test for GetHashCode
        /// </summary>
        public void GetHashCodeTest()
        {
        }

        public void ScaleNonUniformTest(string entityToScaleFile)
        {
            // Get entity to select
            var entityToScale = ImportAndGetEntity(entityToScaleFile);

            Assert.AreEqual(entityToScale != null, true, "entity not imported");

            //as IPSScalable

            var entToScale = entityToScale as IPSScalable;

            Assert.True(entToScale != null, "IPSScalable not implemented by this object");

            var bx = entityToScale.BoundingBox;

            var centre = bx.VolumetricCentre;

            var Origin = new Point();
            var vecOrigin = Origin - centre;

            var entToMove = entityToScale as IPSMoveable;
            Assert.True(entToMove != null, "IPSMoveable not implemented by this object");

            entToMove.MoveByVector(vecOrigin, 0);

            var bx2 = entityToScale.BoundingBox;

            Assert.AreEqual(Origin, bx2.VolumetricCentre, "Cube not moved to Origin");

            var scaleFactor = 2.0;

            entToScale.ScaleNonUniform(scaleFactor, scaleFactor, scaleFactor, false, false, false);

            var bx3 = entityToScale.BoundingBox;

            Assert.AreEqual(bx2.XSize * 2.0, bx3.XSize, "Xsize not equal");
            Assert.AreEqual(bx2.YSize * 2.0, bx3.YSize, "Ysize not equal");
            Assert.AreEqual(bx2.ZSize * 2.0, bx3.ZSize, "Zsize not equal");

            entityToScale.Delete();
        }

        public void ScaleUniformTest(string entityToScaleFile)
        {
            // Get entity to select
            var entityToScale = ImportAndGetEntity(entityToScaleFile);

            Assert.AreEqual(entityToScale != null, true, "entity not imported");

            //as IPSScalable

            var entToScale = entityToScale as IPSScalable;

            Assert.True(entToScale != null, "IPSScalable not implemented by this object");

            var bx = entityToScale.BoundingBox;

            var centre = bx.VolumetricCentre;

            var Origin = new Point();
            var vecOrigin = Origin - centre;

            var entToMove = entityToScale as IPSMoveable;
            Assert.True(entToMove != null, "IPSMoveable not implemented by this object");

            entToMove.MoveByVector(vecOrigin, 0);

            var bx2 = entityToScale.BoundingBox;

            Assert.AreEqual(Origin, bx2.VolumetricCentre, "Cube not moved to Origin");

            var scaleFactor = 2.0;

            entToScale.ScaleUniform(scaleFactor, false, false, false);

            var bx3 = entityToScale.BoundingBox;

            Assert.AreEqual(bx2.XSize * 2.0, bx3.XSize, "Xsize not equal");
            Assert.AreEqual(bx2.YSize * 2.0, bx3.YSize, "Ysize not equal");
            Assert.AreEqual(bx2.ZSize * 2.0, bx3.ZSize, "Zsize not equal");

            entityToScale.Delete();
        }

        /// <summary>
        /// A test for Mirror
        /// </summary>
        public void MirrorTest(string entityToMirrorFile, Point mirrorPoint)
        {
            // Get entity
            var entityToMirror = (IPSMirrorable) ImportAndGetEntity(entityToMirrorFile);

            // Mirror arc
            entityToMirror.Mirror(Planes.YZ, mirrorPoint);

            // Check that operation did not duplicate entity
            Assert.AreEqual(1, _powerSHAPECollection.Count, "Entity was duplicated in PowerSHAPE");
        }

        /// <summary>
        /// A test for Offset
        /// </summary>
        public void OffsetTest(string entityToMoveFile, BoundingBox sizeAfter)
        {
            // Get entity
            var entityToOffset = (IPSOffsetable) ImportAndGetEntity(entityToMoveFile);

            // Move entity
            entityToOffset.Offset(10.0, 0);
            Assert.AreEqual(sizeAfter,
                            (entityToOffset as PSEntity).BoundingBox,
                            "Entity was not offset correctly");
        }

        /// <summary>
        /// A test for Move
        /// </summary>
        public void MoveTest(string entityToMoveFile)
        {
            // Get entity
            var entityToMove = (IPSMoveable) ImportAndGetEntity(entityToMoveFile);

            // Get expected centre of the entity after the operation
            var moveVector = new Vector(0, 10, 0);
            var expectedMovedCentre = (entityToMove as PSEntity).BoundingBox.VolumetricCentre + moveVector;

            // Move entity
            entityToMove.MoveByVector(moveVector, 0);
            Assert.AreEqual(expectedMovedCentre,
                            (entityToMove as PSEntity).BoundingBox.VolumetricCentre,
                            "Entity was not moved");
        }

        /// <summary>
        /// A test for Move
        /// </summary>
        public void MoveCreatingCopiesTest(string entityToMoveFile)
        {
            // Get entity
            var entityToMove = (IPSMoveable) ImportAndGetEntity(entityToMoveFile);

            // Get expected centres of the entities after the operation
            var moveVector = new Vector(0, 10, 0);
            var expectedMovedCentre1 = ((PSEntity) entityToMove).BoundingBox.VolumetricCentre + moveVector;
            var expectedMovedCentre2 = expectedMovedCentre1 + moveVector;
            var expectedMovedCentre3 = expectedMovedCentre2 + moveVector;

            // Move entity
            IList<PSEntity> movedEntities = entityToMove.MoveByVector(moveVector, 3);
            Assert.AreEqual(4, _powerSHAPECollection.Count, "Not all entities were created");
            Assert.AreEqual(3, movedEntities.Count, "Not all moved entities were added to returned list");
            Assert.AreEqual(expectedMovedCentre1,
                            movedEntities[0].BoundingBox.VolumetricCentre,
                            "First entity was not moved to the correct position");
            Assert.AreEqual(expectedMovedCentre2,
                            movedEntities[1].BoundingBox.VolumetricCentre,
                            "Second entity was not moved to the correct position");
            Assert.AreEqual(expectedMovedCentre3,
                            movedEntities[2].BoundingBox.VolumetricCentre,
                            "Third entity was not moved to the correct position");
        }

        /// <summary>
        /// A test for Move
        /// </summary>
        public void MoveDifferentOriginTest(string entityToMoveFile, string newOriginPropertyName)
        {
            // Get entity
            var entityToMove = (IPSMoveable) ImportAndGetEntity(entityToMoveFile);

            // Get entity keypoint
            var origin =
                (Point)
                entityToMove.GetType()
                            .GetProperty(newOriginPropertyName, BindingFlags.Public | BindingFlags.Instance)
                            .GetValue(entityToMove, null);

            // Move entity
            entityToMove.MoveBetweenPoints(origin, new Point(), 0);

            // Get new entity keypoint and check that operation was successful
            origin =
                (Point)
                (entityToMove as PSEntity).GetType()
                                          .GetProperty(newOriginPropertyName, BindingFlags.Public | BindingFlags.Instance)
                                          .GetValue(entityToMove, null);
            Assert.AreEqual(new Point(), origin, "Entity was not moved");
        }

        /// <summary>
        /// A test for LimitToEntities
        /// </summary>
        public void LimitToEntitiesTest(
            string entityToLimitFile,
            string entitiesToLimitAgainstFile,
            LimitingModes limitingMode)
        {
            // Get entity
            var entityToLimit = (IPSLimitable) ImportAndGetEntity(entityToLimitFile);

            // Import multiple entities
            _powerSHAPE.ActiveModel.Import(new File(entitiesToLimitAgainstFile));

            // Add new entities to list
            _powerSHAPE.ActiveModel.SelectAll(true);
            ((PSEntity) entityToLimit).RemoveFromSelection();
            var entitiesToLimitAgainst = _powerSHAPE.ActiveModel.SelectedItems.ToList();

            // Limit entity
            entityToLimit.LimitToEntities(entitiesToLimitAgainst, limitingMode);
            Assert.AreEqual(3, _powerSHAPECollection.Count, "Entity was not limited");
        }

        /// <summary>
        /// A test for RemoveFromSelection
        /// </summary>
        public void RemoveFromSelectionTest(string otherEntityFile, string entityToDeselectFile)
        {
            // Get other entity
            var otherEntity = ImportAndGetEntity(otherEntityFile);

            // Get entity to select
            var entityToDeselect = ImportAndGetEntity(entityToDeselectFile);

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

        /// <summary>
        /// A test for Rotate
        /// </summary>
        public void RotateTest(string entityToRotateFile)
        {
            // Get entity
            var entityToRotate = (IPSRotateable) ImportAndGetEntity(entityToRotateFile);

            // Rotate entity
            entityToRotate.Rotate(Axes.Z, 20, 0, (entityToRotate as PSEntity).BoundingBox.VolumetricCentre);
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.UpdatedItems.Count, "Entity was not rotated");
        }

        /// <summary>
        /// A test for Rotate
        /// </summary>
        public void RotateCreatingCopiesTest(string entityToRotateFile)
        {
            // Get entity
            var entityToRotate = (IPSRotateable) ImportAndGetEntity(entityToRotateFile);

            // Rotate entity
            var rotatedEntities = entityToRotate.Rotate(Axes.Z,
                                                        20,
                                                        3,
                                                        (entityToRotate as PSEntity).BoundingBox.VolumetricCentre);
            Assert.AreEqual(4, _powerSHAPECollection.Count, "Not all entities were created");
            Assert.AreNotEqual(0, rotatedEntities.Count, "Not all rotated entities were added to returned list");
        }

        #endregion

        #region Properties

        /// <summary>
        /// A test for BoundingBox
        /// </summary>
        public void BoundingBoxTest(string entityFile, Point minimumBounds, Point maximumBounds)
        {
            // Get entity
            var entity = ImportAndGetEntity(entityFile);

            // Check bounding box
            Assert.AreEqual(maximumBounds, entity.BoundingBox.MaximumBounds, "Bounding box maximum bounds are incorrect");
            Assert.AreEqual(minimumBounds, entity.BoundingBox.MinimumBounds, "Bounding box minimum bounds are incorrect");
        }

        /// <summary>
        /// A test for Level
        /// </summary>
        public void LevelTest(string entityFile, PSLevel expectedLevel)
        {
            // Get entity
            var entity = ImportAndGetEntity(entityFile);

            // Check level
            Assert.AreEqual(expectedLevel, entity.Level, "Returned level is incorrect");

            // Change level
            var newLevel = _powerSHAPE.ActiveModel.Levels[expectedLevel.Number + 1];
            entity.Level = newLevel;
            Assert.AreEqual(newLevel, entity.Level, "Failed to change level");
        }

        /// <summary>
        /// A test for LevelNumber
        /// </summary>
        public void LevelNumberTest(string entityFile, uint expectedLevelNumber)
        {
            // Get entity
            var entity = ImportAndGetEntity(entityFile);

            // Check level
            Assert.AreEqual(expectedLevelNumber, entity.LevelNumber, "Returned level number is incorrect");

            // Change level number
            var newLevelNumber = expectedLevelNumber + 1;
            entity.LevelNumber = newLevelNumber;
            Assert.AreEqual(newLevelNumber, entity.LevelNumber, "Failed to change level");
        }

        /// <summary>
        /// A test for Name
        /// </summary>
        public void NameTest(string entityFile, string expectedName, string newName)
        {
            // Get entity
            var entity = ImportAndGetEntity(entityFile);

            // Check name
            Assert.AreEqual(expectedName, entity.Name, "Returned name is incorrect");

            // Change name
            entity.Name = newName;
            Assert.AreEqual(newName, entity.Name, "Entity has not been renamed");
        }

        /// <summary>
        /// A test for Transparency
        /// </summary>
        // TODO: Write transparency tests
        public void TransparencyTest()
        {
        }

        #endregion

        #endregion
    }
}