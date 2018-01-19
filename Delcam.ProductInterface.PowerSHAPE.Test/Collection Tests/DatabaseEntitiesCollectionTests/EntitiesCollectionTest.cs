// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for EntitiesCollectionTest and is intended
    /// to contain all EntitiesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public abstract class EntitiesCollectionTest<T> : DatabaseEntitiesCollectionTest<T> where T : PSEntity
    {
        #region Additional test attributes

        //Use TestInitialize to run code before running each test
        [SetUp]
        public override void MyTestInitialize()
        {
            //Close all PowerSHAPE models and create a new one
            base.MyTestInitialize();

            // Test that all PowerSHAPE collections are empty
            Assert.AreEqual(_powerSHAPE.ActiveModel.Annotations.Count, 0, "Annotations collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Arcs.Count, 0, "Arcs collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CompCurves.Count, 0, "CompCurves collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Curves.Count, 0, "Curves collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Lines.Count, 0, "Lines collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Meshes.Count, 0, "Meshes collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Points.Count, 0, "Points collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Solids.Count, 0, "Solids collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Surfaces.Count, 0, "Surfaces collection not empty prior to test");
            Assert.AreEqual(_powerSHAPE.ActiveModel.Workplanes.Count, 0, "Workplanes collection not empty prior to test");
        }

        #endregion

        #region EntitiesCollection Tests

        protected int GetIndexOfNamedEntity(string nameToFind)
        {
            for (int i = 0; i <= 2; i++)
            {
                T item = _powerSHAPECollection[i];
                if (item.Name == nameToFind)
                {
                    return i;
                }
            }
            throw new ApplicationException("Entity with name " + nameToFind + " not found");
        }

        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        public virtual void RemoveAtTest(string fileToImport, string nameOfEntityToRemove)
        {
            // Import file with three entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(fileToImport));

            // Get index of entity to be deleted
            int index;
            try
            {
                index = GetIndexOfNamedEntity(nameOfEntityToRemove);
            }
            catch (ApplicationException e)
            {
                Assert.Fail(e.Message);
                return;
            }

            // Remove second entity
            _powerSHAPECollection.RemoveAt(index);

            // Check resultant PowerSHAPE state
            if (_powerSHAPECollection.Count == 3)
            {
                Assert.Fail("Failed to remove " + _collectionType + " from collection");
            }
            else if (_powerSHAPECollection.Count != 2)
            {
                Assert.Fail("Removed too many " + _collectionType + "s from the collection");
            }
            else
            {
                foreach (T createdEntity in _powerSHAPECollection)
                {
                    if (createdEntity.Name == nameOfEntityToRemove)
                    {
                        Assert.Fail("Incorrect " + _collectionType + " deleted");
                    }
                }
            }
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        public virtual void RemoveTest(string fileToImport, string nameOfEntityToRemove)
        {
            // Import three entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(fileToImport));

            var originalCount = _powerSHAPECollection.Count;

            T entityToDelete = _powerSHAPECollection.FirstOrDefault(x => x.Name == nameOfEntityToRemove);
            if (entityToDelete == null)
            {
                Assert.Fail("Couldn't find entity name " + nameOfEntityToRemove);
            }

            // Remove second entity
            _powerSHAPECollection.Remove(entityToDelete);

            if (_powerSHAPECollection.Count == originalCount)
            {
                Assert.Fail("Failed to remove " + _collectionType.ToLower() + " from collection");
            }
            else if (_powerSHAPECollection.Count != originalCount - 1)
            {
                Assert.Fail("Removed too many " + _collectionType.ToLower() + "s from the collection");
            }
            else
            {
                foreach (T createdEntity in _powerSHAPECollection)
                {
                    if (createdEntity.Name == nameOfEntityToRemove)
                    {
                        Assert.Fail("Incorrect " + _collectionType.ToLower() + " deleted");
                    }
                }
            }
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        public virtual void ClearTest(string multipleEntitiesFile, string singleEntitiesFile)
        {
            // Import three entities to be deleted
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(multipleEntitiesFile));

            // Import single entity to remain
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(singleEntitiesFile));

            // Clear entities
            _powerSHAPECollection.Clear();

            // Carry out checks
            Assert.AreEqual(0,
                            _powerSHAPECollection.Count,
                            "The " + _collectionType.ToLower() + "s were not all removed from the collection");

            _powerSHAPE.ActiveModel.SelectAll(true);
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.SelectedItems.Count, "Operation deleted other entities");
        }

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        public virtual void AddToSelectionTest(string multipleEntitiesFile, string singleEntitiesFile)
        {
            // Import three entities to be deleted
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(multipleEntitiesFile));

            // Import single entity to remain
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(singleEntitiesFile));

            // Get single entity
            PSEntity testEntity = _powerSHAPE.ActiveModel.SelectedItems[0];

            // Clear selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Add multiple entities to selection
            ((PSEntitiesCollection<T>) _powerSHAPECollection).AddToSelection(true);

            // Check the selection contains all three entities
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0) // Nothing was selected
            {
                Assert.Fail("Failed to select anything in PowerSHAPE");
            }
            else if (_powerSHAPE.ActiveModel.SelectedItems.Count == 4) // Everything was selected
            {
                Assert.Fail("Selected everything in PowerSHAPE");
            }

            // Check the test entity is not selected
            foreach (PSEntity selectedEntity in _powerSHAPE.ActiveModel.SelectedItems)
            {
                if (selectedEntity.Equals(testEntity)) // Test entity is in selection
                {
                    Assert.Fail("Added other entities to selection");
                }
            }
            Assert.AreEqual(_powerSHAPE.ActiveModel.SelectedItems.Count,
                            3,
                            "Failed to add all " + _collectionType.ToLower() + " to collection");
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        public virtual void RemoveFromSelectionTest(string multipleEntitiesFile, string singleEntitiesFile)
        {
            // Import multiple entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(multipleEntitiesFile));

            // Import single entity
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(singleEntitiesFile));

            // Get single entity
            PSEntity testEntity = _powerSHAPE.ActiveModel.SelectedItems[0];

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(true);

            // Remove entities from selection
            ((PSEntitiesCollection<T>) _powerSHAPECollection).RemoveFromSelection();

            // Check the selection contains only the single entity
            if (_powerSHAPE.ActiveModel.SelectedItems.Count == 0) // Everything was deselected
            {
                Assert.Fail("Deselected all entities in PowerSHAPE");
            }
            else if (_powerSHAPE.ActiveModel.SelectedItems.Count == 4) // Nothing was deselected
            {
                Assert.Fail("Deselected nothing in PowerSHAPE");
            }
            else if (_powerSHAPE.ActiveModel.SelectedItems.Count != 1) // More than just the single entity is selected
            {
                Assert.Fail("Failed to remove all " + _collectionType.ToLower() + "s from selection");
            }

            Assert.AreEqual(_powerSHAPE.ActiveModel.SelectedItems[0],
                            testEntity,
                            "Removed incorrect entities from the selection");
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        /// <summary>
        /// A test for Contains
        /// </summary>
        public void ContainsTest(FileSystem.File entityToAddFile)
        {
            // Import entity
            T entityToAdd = (T) ImportAndGetEntity(entityToAddFile);

            // Check collection
            Assert.IsTrue(_powerSHAPECollection.Contains(entityToAdd),
                          "Incorrectly returned that collection does not contain entity");
        }

        /// <summary>
        /// A test for Count
        /// </summary>
        public override void CountTest(FileSystem.File entityToAddFile)
        {
            base.CountTest(entityToAddFile);

            // Import entity
            ImportAndGetEntity(entityToAddFile);

            // Check collection
            Assert.AreEqual(1, _powerSHAPECollection.Count, "Collection count is incorrect");
        }

        /// <summary>
        /// This test checks that the Entity's Identifier is as expected
        /// </summary>
        public override void IdentifierTest(FileSystem.File fileToImport, string expectedIdentifier)
        {
            // Get entity
            PSEntity importedEntity = ImportAndGetEntity(fileToImport);

            // Check that the collection Identifier matches the entity type
            string actualIdentifier = (string) importedEntity
                .GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(importedEntity, null);
            Assert.AreEqual(expectedIdentifier, actualIdentifier);
        }

        /// <summary>
        /// A test for Item
        /// </summary>
        public void ItemTest(FileSystem.File entityFile)
        {
            // Get entities
            T entity = (T) ImportAndGetEntity(entityFile);

            // Check item
            Assert.AreEqual(entity, _powerSHAPECollection[0], "Did not return entity");
        }

        /// <summary>
        /// A test for LastItem
        /// </summary>
        public void LastItemTest(FileSystem.File multipleEntitiesFile)
        {
            // Get entities
            List<PSEntity> entities = ImportAndGetEntities(multipleEntitiesFile);

            // Check item
            Assert.AreEqual((T) entities.Last(), _powerSHAPECollection.Last(), "Returned the incorrect entity");
        }

        #endregion

        #region Helper Functions

        protected PSEntity ImportAndGetEntity(FileSystem.File fileToImport)
        {
            // Import entity
            _powerSHAPE.ActiveModel.Import(fileToImport);

            // Get entity
            if (_powerSHAPE.ActiveModel.SelectedItems.Count > 0)
            {
                return _powerSHAPE.ActiveModel.SelectedItems[0];
            }
            return _powerSHAPE.ActiveModel.CreatedItems[0];
        }

        protected List<PSEntity> ImportAndGetEntities(FileSystem.File fileToImport)
        {
            // Import entities
            _powerSHAPE.ActiveModel.Import(fileToImport);

            // Get entities
            return _powerSHAPE.ActiveModel.SelectedItems.ToList();
        }

        #endregion
    }
}