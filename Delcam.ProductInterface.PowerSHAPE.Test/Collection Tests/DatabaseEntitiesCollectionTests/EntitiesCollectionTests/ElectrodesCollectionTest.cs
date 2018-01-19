// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Linq;
using Autodesk.FileSystem;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for ElectrodesCollectionTest and is intended
    /// to contain all ElectrodesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class ElectrodesCollectionTest : EntitiesCollectionTest<PSElectrode>
    {
        #region Constructors

        public ElectrodesCollectionTest()
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
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's ElectrodesCollection\n\n" + e.Message);
            }
        }

        #region ElectrodesCollection Tests

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddElectrodesToSelectionTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));

            // Import single entity to remain
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SURFACE));

            // Get single entity
            PSEntity testEntity = _powerSHAPE.ActiveModel.SelectedItems[0];

            // Clear selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Add multiple entities to selection
            _powerSHAPE.ActiveModel.Electrodes.AddToSelection(true);

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
        /// A test for RemoveAt
        /// </summary>
        [Test]
        public void RemoveElectrodeAtTest()
        {
            // Import file with three entities
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Get index of entity to be deleted
            int index;
            try
            {
                index = GetIndexOfNamedEntity("SPM1_2");
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
                foreach (PSElectrode createdEntity in _powerSHAPECollection)
                {
                    if (createdEntity.Name == "SPM1_2")
                    {
                        Assert.Fail("Incorrect " + _collectionType + " deleted");
                    }
                }
            }
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveElectrodeTest()
        {
            // Import file with three entities
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Get entity to be removed
            PSElectrode entityToDelete = _powerSHAPECollection[0];
            foreach (PSElectrode createdEntity in
                _powerSHAPECollection.Where(createdEntity => createdEntity.Name == "SPM1_2"))
            {
                entityToDelete = createdEntity;
            }

            // Remove second entity
            _powerSHAPECollection.Remove(entityToDelete);

            if (_powerSHAPECollection.Count == 3)
            {
                Assert.Fail("Failed to remove " + _collectionType.ToLower() + " from collection");
            }
            else if (_powerSHAPECollection.Count != 2)
            {
                Assert.Fail("Removed too many " + _collectionType.ToLower() + "s from the collection");
            }
            else
            {
                foreach (PSElectrode createdEntity in _powerSHAPECollection)
                {
                    if (createdEntity.Name == "SPM1_2")
                    {
                        Assert.Fail("Incorrect " + _collectionType.ToLower() + " deleted");
                    }
                }
            }
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearElectrodesTest()
        {
            // Import file with three entities
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Import single entity to remain
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SURFACE));

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
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveElectrodesFromSelectionTest()
        {
            // Import file with three entities
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Import single entity
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_SURFACE));

            // Get single entity
            PSEntity testEntity = _powerSHAPE.ActiveModel.SelectedItems[0];

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(true);

            // Remove entities from selection
            ((PSEntitiesCollection<PSElectrode>) _powerSHAPECollection).RemoveFromSelection();

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

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            Assert.AreEqual(1, _powerSHAPE.ActiveModel.Electrodes.Count);
        }

        [Test]
        public void GetElectrodeByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Get an entity by its name
            var namedEntity = _powerSHAPECollection.GetByName("SPM1_2");

            // Check that the correct entity was returned
            Assert.IsTrue(namedEntity.Datum.X == -150.370775, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var entityToAdd = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check collection
            Assert.IsTrue(_powerSHAPECollection.Contains(entityToAdd),
                          "Incorrectly returned that collection does not contain entity");
        }

        [Test]
        public void CountElectrodeTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Check collection
            Assert.AreEqual(1, _powerSHAPECollection.Count, "Collection count is incorrect");
        }

        [Test]
        public void EqualsElectrodeTest()
        {
            EqualsTest();
        }

        [Test]
        public void ElectrodeItemTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.SINGLE_ELECTRODE_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;
            var entity = _powerSHAPE.ActiveModel.Electrodes[0];

            // Check item
            Assert.AreEqual(entity, _powerSHAPECollection[0], "Did not return entity");
        }

        [Test]
        public void ElectrodeLastItemTest()
        {
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.THREE_ELECTRODES_MODEL));
            _powerSHAPECollection = _powerSHAPE.ActiveModel.Electrodes;

            // Check item
            Assert.AreEqual(_powerSHAPECollection.Last(), _powerSHAPECollection.Last(), "Returned the incorrect entity");
        }

        #endregion

        #endregion
    }
}