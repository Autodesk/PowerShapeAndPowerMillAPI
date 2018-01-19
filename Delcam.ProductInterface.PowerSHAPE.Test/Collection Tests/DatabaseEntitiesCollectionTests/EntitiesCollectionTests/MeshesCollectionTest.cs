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
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for MeshesCollectionTest and is intended
    /// to contain all MeshesCollectionTest Unit Tests
    /// </summary>
    [TestFixture]
    public class MeshesCollectionTest : EntitiesCollectionTest<PSMesh>
    {
        #region Constructors

        public MeshesCollectionTest()
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
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Meshes;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's MeshesCollection\n\n" + e.Message);
            }
        }

        #region MeshesCollection Tests

        /// <summary>
        /// A test for CreateMesh from a DMT
        /// </summary>
        [Test]
        public void CreateMeshFromDMTTest()
        {
            // Create DMT model
            DMTModel dmt = DMTModelReader.ReadFile(new FileSystem.File(TestFiles.SINGLE_SURFACE_DMT));

            // Create mesh from dmt
            _powerSHAPE.ActiveModel.Meshes.CreateMeshFromDMT(dmt);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Meshes.Count, 1, "Failed to add mesh to collection");
        }

        /// <summary>
        /// A test for CreateMesh from a Surface
        /// </summary>
        [Test]
        public void CreateMeshFromSurfaceTest()
        {
            // Import Surface dgk
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_SURFACE));

            // Select everything
            _powerSHAPE.ActiveModel.SelectAll(false);

            // Get surface
            PSSurface meshSurface = (PSSurface) _powerSHAPE.ActiveModel.SelectedItems[0];

            // Create mesh from surface
            _powerSHAPE.ActiveModel.Meshes.CreateMeshFromSurface(meshSurface);
            Assert.AreEqual(_powerSHAPE.ActiveModel.Meshes.Count, 1, "Failed to add mesh to collection");
            Assert.AreEqual(_powerSHAPE.ActiveModel.CreatedItems.Count, 1, "Failed to create mesh in PowerSHAPE");
        }

        #endregion

        #region EntitiesCollection Tests

        /// <summary>
        /// A test for AddToSelection
        /// </summary>
        [Test]
        public void AddMeshesToSelectionTest()
        {
            // Import three entities to be deleted
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH1));
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH2));
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH3));

            // Import single entity to remain
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_SURFACE));

            // Get single entity
            PSEntity testEntity = _powerSHAPE.ActiveModel.SelectedItems[0];

            // Clear selection
            _powerSHAPE.ActiveModel.ClearSelectedItems();

            // Add multiple entities to selection
            ((PSEntitiesCollection<PSMesh>) _powerSHAPECollection).AddToSelection(true);

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
        public void RemoveMeshAtTest()
        {
            {
                // Import file with three entities
                _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH1));
                _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH2));
                _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH3));

                // Get index of entity to be deleted
                int index;
                try
                {
                    index = GetIndexOfNamedEntity("2");
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
                    foreach (PSMesh createdEntity in _powerSHAPECollection)
                    {
                        if (createdEntity.Name == "2")
                        {
                            Assert.Fail("Incorrect " + _collectionType + " deleted");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A test for Remove
        /// </summary>
        [Test]
        public void RemoveMeshTest()
        {
            {
                // Import three entities
                _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH1));
                _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH2));
                _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.SINGLE_MESH3));

                // Get entity to be removed
                PSMesh entityToDelete = _powerSHAPECollection[0];
                foreach (PSMesh createdEntity in _powerSHAPECollection)
                {
                    if (createdEntity.Name == "2")
                    {
                        entityToDelete = createdEntity;
                    }
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
                    foreach (PSMesh createdEntity in _powerSHAPECollection)
                    {
                        if (createdEntity.Name == "2")
                        {
                            Assert.Fail("Incorrect " + _collectionType.ToLower() + " deleted");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A test for Clear
        /// </summary>
        [Test]
        public void ClearMeshesTest()
        {
            ClearTest(TestFiles.THREE_MESHES_DMT, TestFiles.SINGLE_SURFACE);
        }

        /// <summary>
        /// A test for RemoveToSelection
        /// </summary>
        [Test]
        public void RemoveMeshesFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.THREE_MESHES_DMT, TestFiles.SINGLE_SURFACE);
        }

        #endregion

        #region DatabaseEntitiesCollection Tests

        #region Properties

        /// <summary>
        /// A test for the Identifier
        /// </summary>
        [Test]
        public void IdentifierMeshTest()
        {
            // Import entity
            ImportAndGetEntity(new FileSystem.File(TestFiles.SINGLE_MESH1));

            // Check that the collection Identifier matches the entity type
            string actualIdentifier = (string) _powerSHAPECollection
                .GetType().GetProperty("Identifier", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(_powerSHAPECollection, null);
            Assert.AreEqual("MESH", actualIdentifier);
        }

        [Test]
        public void GetMeshByNameTest()
        {
            // Import file with multiple entities
            _powerSHAPE.ActiveModel.Import(new FileSystem.File(TestFiles.THREE_MESHES_DMT));

            // Get an entity by its name
            PSMesh namedEntity = _powerSHAPECollection.GetByName("2");

            // Check that the correct entity was returned
            Assert.AreEqual(5625, namedEntity.Area, "Incorrect entity returned");
        }

        #endregion

        #region Operations

        [Test]
        public void ContainsMeshTest()
        {
            ContainsTest(new FileSystem.File(TestFiles.SINGLE_MESH1));
        }

        [Test]
        public void CountMeshTest()
        {
            CountTest(new FileSystem.File(TestFiles.SINGLE_MESH1));
        }

        [Test]
        public void EqualsMeshTest()
        {
            EqualsTest();
        }

        [Test]
        public void MeshItemTest()
        {
            ItemTest(new FileSystem.File(TestFiles.SINGLE_MESH1));
        }

        [Test]
        public void MeshLastItemTest()
        {
            LastItemTest(new FileSystem.File(TestFiles.THREE_MESHES_DMT));
        }

        #endregion

        #endregion
    }
}