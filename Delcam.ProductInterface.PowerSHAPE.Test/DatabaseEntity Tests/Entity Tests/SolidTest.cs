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
using Autodesk.FileSystem;
using Autodesk.Geometry;
using Autodesk.ProductInterface.PowerSHAPE;
using NUnit.Framework;

namespace Autodesk.ProductInterface.PowerSHAPETest
{
    /// <summary>
    /// This is a test class for SolidTest and is intended
    /// to contain all SolidTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SolidTest : EntityTest<PSSolid>
    {
        [SetUp]
        public override void MyTestInitialize()
        {
            base.MyTestInitialize();
            try
            {
                _powerSHAPECollection = _powerSHAPE.ActiveModel.Solids;
            }
            catch (Exception e)
            {
                Assert.Fail("_powerSHAPECollection could not be set to Active Model's SolidsCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void SolidIdTest()
        {
            IdTest(TestFiles.SINGLE_SOLID);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void SolidIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_SOLID, "SOLID");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void SolidEqualsTest()
        {
            Equals(TestFiles.SINGLE_SOLID);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void SolidExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_SOLID);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddSolidToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_LINE, TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void BlankSolidTest()
        {
            BlankTest(TestFiles.THREE_LINES, TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void SolidBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_SOLID, new Point(-90, -17, 0), new Point(-50, 23, 40));
        }

        [Test]
        public void CopySolidTest()
        {
            DuplicateTest(TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void DeleteSolidTest()
        {
            DeleteTest(TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void SolidLevelTest()
        {
            LevelTest(TestFiles.SINGLE_SOLID_DGK, _powerSHAPE.ActiveModel.Levels[1]);
        }

        [Test]
        public void SolidLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_SOLID_DGK, 1);
        }

        [Test]
        public void MirrorSolidTest()
        {
            // There is a bug in PowerSHAPE 11.1 that causes this test to fail so if we are pre-11.2 then ignore this test
            if (_powerSHAPE.Version < new Version("11.2"))
            {
                Assert.Ignore(
                    "There is a bug in PowerSHAPE 11.1 that causes this test to fail so if we are pre-11.2 then ignore this test");
            }

            // Carry out operation
            var mirrorPoint = new Point(-50, -17, 0);
            var initialCentrePoint = new Point(-70, 3, 20);

            MirrorTest(TestFiles.SINGLE_SOLID, mirrorPoint);

            // Get point that should now be on mirror point
            var mirroredSolid = _powerSHAPE.ActiveModel.Solids[0];
            Assert.AreEqual(mirrorPoint,
                            mirroredSolid.BoundingBox.MinimumBounds,
                            "Solid was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((mirrorPoint.X - initialCentrePoint.X) * 2.0,
                            mirroredSolid.CentreOfGravity.X - initialCentrePoint.X,
                            "Arc was not mirrored in correct plane");
        }

        [Test]
        public void MoveSolidTest()
        {
            MoveTest(TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void MoveSolidCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void MoveSolidDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_SOLID, "CentreOfGravity");
        }

        [Test]
        public void ScaleSolidTest()
        {
            ScaleNonUniformTest(TestFiles.SINGLE_SOLID);

            ScaleUniformTest(TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void OffsetSolidTest()
        {
            OffsetTest(TestFiles.SINGLE_SOLID, new BoundingBox(-100, -40, -27, 33, -10, 50));
        }

        [Test]
        public void SolidNameTest()
        {
            NameTest(TestFiles.SINGLE_SOLID, "SingleSolid", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("solid[NewName].EXISTS"));
        }

        [Test]
        public void RemoveSolidFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_LINE, TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void RotateSolidTest()
        {
            RotateTest(TestFiles.SINGLE_SOLID);
        }

        [Test]
        public void RotateSolidCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_SOLID);
        }

        /// <summary>
        /// Test the distance between two solids
        /// </summary>
        [Test]
        public void MinimumDistanceSolidSolid()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Solids[0];
            var entity2 = _powerSHAPE.ActiveModel.Solids[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test the distance between a solid and a surface
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceSolidSurface()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Solids[0];
            var entity2 = _powerSHAPE.ActiveModel.Surfaces[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        #endregion

        #region Solid Tests

        #region Properties

        /// <summary>
        /// A test for Area
        /// </summary>
        [Test]
        public void SolidAreaTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid area
            Assert.AreEqual(9600, solid.Area, "Returned solid area is incorrect");
        }

        /// <summary>
        /// A test for CentreOfGravity
        /// </summary>
        [Test]
        public void SolidCentreOfGravityTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid COG
            var expectedCOG = new Point(-70, 3, 20);
            Assert.AreEqual(expectedCOG, solid.CentreOfGravity, "Returned centre of gravtity is incorrect");
        }

        /// <summary>
        /// A test for AverageEdgeTolerance
        /// </summary>
        [Test]
        public void SolidAverageEdgeToleranceTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.AreEqual(0.001,
                            Math.Round(solid.AverageEdgeTolerance, 3),
                            "Returned incorrect average edge tolerance");
        }

        /// <summary>
        /// A test for HalfEdgeTolerance
        /// </summary>
        [Test]
        public void SolidHalfEdgeToleranceTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.AreEqual(0.01, Math.Round(solid.HalfEdgeTolerance, 2), "Returned incorrect half-edge tolerance");
        }

        /// <summary>
        /// A test for IsActive
        /// </summary>
        [Test]
        public void IsSolidActiveTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.IsFalse(solid.IsActive, "Incorrectly returned that solid was active");

            // Change active status
            solid.IsActive = true;

            // Check solid
            Assert.IsTrue(solid.IsActive, "Incorrectly returned that solid was not active");
        }

        /// <summary>
        /// A test for IsClosed
        /// </summary>
        [Test]
        public void IsSolidClosedTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.IsTrue(solid.IsClosed, "Incorrectly returned that solid was open");

            // Import open solid
            var openSolid = (PSSolid) ImportAndGetEntity(TestFiles.SOLID_OPEN);

            // Check solid
            Assert.IsFalse(openSolid.IsClosed, "Incorrectly returned that solid was closed");
        }

        /// <summary>
        /// A test for IsConnected
        /// </summary>
        [Test]
        public void IsSolidConnectedTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.IsTrue(solid.IsConnected, "Incorrectly returned that solid was disconnected");

            // Get solid to remove
            var solidToRemove = (PSSolid) ImportAndGetEntity(TestFiles.SOLID_TO_REMOVE);

            // Remove solid
            solid.SubtractEntityFromEntity(solidToRemove);

            // Check solid
            Assert.IsFalse(solid.IsConnected, "Incorrectly returned that solid was connected");
        }

        /// <summary>
        /// A test for IsGhost
        /// </summary>
        [Test]
        [Ignore("Need to determine what we want to do with ghost solids")]
        public void IsSolidGhostTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Get solid to remove
            var solidToRemove = (PSSolid) ImportAndGetEntity(TestFiles.SOLID_TO_REMOVE);

            // Remove solid
            solid.SubtractEntityFromEntity(solidToRemove);

            // Check solids
            Assert.IsFalse(solid.IsGhost, "Incorrectly returned that solid was a ghost");
            Assert.IsTrue(solidToRemove.IsGhost, "Incorrectly returned that solid was not a ghost");
        }

        /// <summary>
        /// A test for IsHidden
        /// </summary>
        //[Test]
        public void IsSolidHiddenTest()
        {
            //TODO: Find out what it means to have a hidden solid in PowerSHAPE
        }

        /// <summary>
        /// A test for IsTrimmingValid
        /// </summary>
        //[Test]
        public void IsSolidTrimmingValidTest()
        {
            // TODO: Can't import a badly trimmed solid
        }

        /// <summary>
        /// A test for IsWatertight
        /// </summary>
        [Test]
        public void IsSolidWatertightTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.IsTrue(solid.IsWatertight, "Incorrectly returned that solid was not watertight");

            // Import open solid
            var openSolid = (PSSolid) ImportAndGetEntity(TestFiles.SOLID_OPEN);

            // Check solid
            Assert.IsFalse(openSolid.IsWatertight, "Incorrectly returned that solid was watertight");
        }

        /// <summary>
        /// A test for MaximumEdgeTolerance
        /// </summary>
        [Test]
        public void SolidMaximumEdgeToleranceTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.AreEqual(0.001,
                            Math.Round(solid.MaximumEdgeTolerance, 3),
                            "Returned incorrect maximum edge tolerance");
        }

        /// <summary>
        /// A test for MomentOfInertia
        /// </summary>
        [Test]
        public void SolidMomentOfInertiaTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check solid
            Assert.AreEqual(43242667,
                            Math.Round(solid.MomentOfInertia[0], 0),
                            "Returned incorrect X component of moment of inertia");
            Assert.AreEqual(356266667,
                            Math.Round(solid.MomentOfInertia[1], 0),
                            "Returned incorrect Y component of moment of inertia");
            Assert.AreEqual(331242667,
                            Math.Round(solid.MomentOfInertia[2], 0),
                            "Returned incorrect Z component of moment of inertia");
        }

        /// <summary>
        /// A test for NumberOfAnalyticSurfaces
        /// </summary>
        //[Test]
        public void NumberOfSolidAnalyticSurfacesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfErrorSuppressedFeatures
        /// </summary>
        //[Test]
        public void NumberOfSolidErrorSuppressedFeaturesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfFeatures
        /// </summary>
        //[Test]
        public void NumberOfSolidFeaturesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfFeaturesInclSubBanches
        /// </summary>
        //[Test]
        public void NumberOfSolidFeaturesInclSubBanchesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfGroupFeatures
        /// </summary>
        //[Test]
        public void NumberOfSolidGroupFeaturesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfGroupFeaturesInclSubBranches
        /// </summary>
        //[Test]
        public void NumberOfSolidGroupFeaturesInclSubBranchesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfLinkedHalfEdges
        /// </summary>
        //[Test]
        public void NumberOfSolidLinkedHalfEdgesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfRedefinedFeatures
        /// </summary>
        //[Test]
        public void NumberOfSolidRedefinedFeaturesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfSelectedFeatures
        /// </summary>
        //[Test]
        public void NumberOfSolidSelectedFeaturesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfSurfaces
        /// </summary>
        //[Test]
        public void NumberOfSolidSurfacesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfUnlinkedHalfEdges
        /// </summary>
        //[Test]
        public void NumberOfSolidUnlinkedHalfEdgesTest()
        {
        }

        /// <summary>
        /// A test for NumberOfVerticesAlongEdges
        /// </summary>
        //[Test]
        public void NumberOfSolidVerticesAlongEdgesTest()
        {
        }

        /// <summary>
        /// A test for RepresentationNumber
        /// </summary>
        //[Test]
        public void SolidRepresentationNumberTest()
        {
        }

        /// <summary>
        /// A test for Thickness
        /// </summary>
        //[Test]
        public void SolidThicknessTest()
        {
        }

        /// <summary>
        /// A test for Type
        /// </summary>
        //[Test]
        public void SolidTypeTest()
        {
            // TODO: Setup Solid Primitive classes
        }

        /// <summary>
        /// A test for Version
        /// </summary>
        [Test]
        public void SolidVersionTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check version
            Assert.AreEqual(SolidVersions.Parasolid, solid.Version, "Did not return that solid was a Parasolid");

            // Change version
            solid.Version = SolidVersions.Version8;

            // Check version
            Assert.AreEqual(SolidVersions.Version8, solid.Version, "Did not return that solid was a Version 8");
        }

        /// <summary>
        /// A test for Volume
        /// </summary>
        [Test]
        public void SolidVolumeTest()
        {
            // Get solid
            var solid = (PSSolid) ImportAndGetEntity(TestFiles.SINGLE_SOLID);

            // Check volume
            Assert.AreEqual(6.4, Math.Round(solid.Volume, 1), "Returned volume was incorrect");
        }

        #endregion

        #region Add Operations

        /// <summary>
        /// A test for AddSolidToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddSolidToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solids
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var solidToRemove = _powerSHAPE.ActiveModel.Solids[1];

            // Do the add
            var returnedItem = (PSSolid) activeSolid.AddEntityToEntity(solidToRemove);

            // Test that the active solid still exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the solid to remove doesn't exist
            Assert.IsFalse(solidToRemove.Exists);

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for AddSolidsToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddSolidsToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solids
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var solidsToRemove = new List<PSSolid>
            {
                _powerSHAPE.ActiveModel.Solids[1],
                _powerSHAPE.ActiveModel.Solids[2]
            };

            // Do the add
            var returnedItem = (PSSolid) activeSolid.AddEntitiesToEntity(solidsToRemove);

            // Test that the active solid still exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the solids exist but are not in the collection
            foreach (var solidToRemove in solidsToRemove)
            {
                Assert.IsFalse(_powerSHAPE.ActiveModel.Solids.Contains(solidToRemove));
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for AddMeshToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddMeshToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var meshToRemove = _powerSHAPE.ActiveModel.Meshes[0];

            // Do the add
            var returnedItem = (PSMesh) activeSolid.AddEntityToEntity(meshToRemove);

            // Test that the active solid doesn't exist
            Assert.IsFalse(activeSolid.Exists);

            // Test that the mesh to remove doesn't exist
            Assert.IsFalse(meshToRemove.Exists);

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for AddMeshesToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddMeshesToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and two meshes
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var meshesToRemove = new List<PSMesh> {_powerSHAPE.ActiveModel.Meshes[0], _powerSHAPE.ActiveModel.Meshes[1]};

            // Do the add
            var returnedItem = (PSMesh) activeSolid.AddEntitiesToEntity(meshesToRemove);

            // Test that the active solid doesn't exist
            Assert.IsFalse(activeSolid.Exists);

            // Test that the meshes to remove doesn't exist
            foreach (var meshToRemove in meshesToRemove)
            {
                Assert.IsFalse(meshToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for AddSurfaceToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddSurfaceToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var surfaceToRemove = _powerSHAPE.ActiveModel.Surfaces[0];

            // Do the add
            var returnedItem = (PSSolid) activeSolid.AddEntityToEntity(surfaceToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the surface to remove doesn't exist
            Assert.IsFalse(surfaceToRemove.Exists);

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for AddSurfacesToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddSurfacesToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and two surfaces
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var surfacesToRemove = new List<PSSurface>
            {
                _powerSHAPE.ActiveModel.Surfaces[0],
                _powerSHAPE.ActiveModel.Surfaces[1]
            };

            // Do the add
            var returnedItem = (PSSolid) activeSolid.AddEntitiesToEntity(surfacesToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the surfaces to remove don't exist
            foreach (var surfaceToRemove in surfacesToRemove)
            {
                Assert.IsFalse(surfaceToRemove.Exists);
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for AddMeshToSurfaceFromSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddMeshAndSurfaceToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSAddable>
            {
                _powerSHAPE.ActiveModel.Meshes[0],
                _powerSHAPE.ActiveModel.Surfaces[0]
            };

            // Do the add
            var returnedItem = (PSMesh) activeSolid.AddEntitiesToEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsFalse(activeSolid.Exists);

            // Test that the enities to remove don't exist
            foreach (PSEntity entityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(entityToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for AddMeshAndSolidToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddMeshAndSolidToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSAddable>
            {
                _powerSHAPE.ActiveModel.Meshes[0],
                _powerSHAPE.ActiveModel.Solids[1]
            };

            // Do the subtraction
            var returnedItem = (PSMesh) activeSolid.AddEntitiesToEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsFalse(activeSolid.Exists);

            // Test that the enities to remove don't exist
            foreach (PSEntity entityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(entityToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for AddSolidAndSurfaceToSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void AddSolidAndSurfaceToSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a solid and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSAddable>
            {
                _powerSHAPE.ActiveModel.Solids[1],
                _powerSHAPE.ActiveModel.Surfaces[0]
            };

            // Do the add
            var returnedItem = (PSSolid) activeSolid.AddEntitiesToEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the enitities to remove don't exist
            foreach (PSEntity enitityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(enitityToRemove.Exists);
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        #endregion

        #region Intersect Operations

        /// <summary>
        /// A test for IntersectSolidWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectSolidWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solids
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var solidToRemove = _powerSHAPE.ActiveModel.Solids[1];

            // Do the intersection
            var returnedItem = (PSSolid) activeSolid.IntersectEntityWithEntity(solidToRemove);

            // Test that the active solid still exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the solid to remove doesn't exist
            Assert.IsFalse(solidToRemove.Exists);

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for IntersectSolidsWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectSolidsWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solids
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var solidsToRemove = new List<PSSolid>
            {
                _powerSHAPE.ActiveModel.Solids[1],
                _powerSHAPE.ActiveModel.Solids[2]
            };

            // Do the intersection
            var returnedItems = activeSolid.IntersectEntitiesWithEntity(solidsToRemove);

            // Test that the active solid still exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the solids exist but are not in the collection
            foreach (var solidToRemove in solidsToRemove)
            {
                Assert.IsFalse(_powerSHAPE.ActiveModel.Solids.Contains(solidToRemove));
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItems[0].Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for IntersectMeshWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectMeshWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var meshToRemove = _powerSHAPE.ActiveModel.Meshes[0];

            // Do the intersection
            var returnedItem = (PSMesh) activeSolid.IntersectEntityWithEntity(meshToRemove);

            // Test that the active solid doesn't exist
            Assert.IsFalse(activeSolid.Exists);

            // Test that the mesh to remove doesn't exist
            Assert.IsFalse(meshToRemove.Exists);

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for IntersectMeshesWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectMeshesWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and two meshes
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var meshesToRemove = new List<PSMesh> {_powerSHAPE.ActiveModel.Meshes[0], _powerSHAPE.ActiveModel.Meshes[1]};

            // Do the intersection
            var returnedItems = activeSolid.IntersectEntitiesWithEntity(meshesToRemove);

            // Test that the active solid doesn't exist
            Assert.IsFalse(activeSolid.Exists);

            // Test that the meshes to remove doesn't exist
            foreach (var meshToRemove in meshesToRemove)
            {
                Assert.IsFalse(meshToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItems[0].Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains((PSMesh) returnedItems[0]));
        }

        /// <summary>
        /// A test for IntersectSurfaceWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectSurfaceWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var surfaceToRemove = _powerSHAPE.ActiveModel.Surfaces[0];

            // Do the intersection
            var returnedItem = (PSSolid) activeSolid.IntersectEntityWithEntity(surfaceToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the surface to remove doesn't exist
            Assert.IsFalse(surfaceToRemove.Exists);

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for IntersectSurfacesWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectSurfacesWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and two surfaces
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var surfacesToRemove = new List<PSSurface>
            {
                _powerSHAPE.ActiveModel.Surfaces[0],
                _powerSHAPE.ActiveModel.Surfaces[1]
            };

            // Do the intersection
            var returnedItems = activeSolid.IntersectEntitiesWithEntity(surfacesToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);
            if (_powerSHAPE.Version >= new Version("11.3"))
            {
                // Test that the surfaces to remove don't exist
                foreach (var surfaceToRemove in surfacesToRemove)
                {
                    Assert.IsFalse(surfaceToRemove.Exists);
                }

                // Also check that the returned solid is the same as the original solid
                Assert.AreEqual(returnedItems[0].Id, activeSolid.Id);

                // Also check that two solids were created
                Assert.AreEqual(2, returnedItems.Count);
            }
            else
            {
                // First surface to remove should be gone but second one remains
                Assert.IsFalse(surfacesToRemove[0].Exists);
                Assert.IsTrue(surfacesToRemove[1].Exists);

                // Also check that the returned solid is the same as the original solid
                Assert.AreEqual(returnedItems[0].Id, activeSolid.Id);
            }
        }

        /// <summary>
        /// A test for IntersectMeshAndSurfaceWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectMeshAndSurfaceWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSIntersectable>
            {
                _powerSHAPE.ActiveModel.Meshes[0],
                _powerSHAPE.ActiveModel.Surfaces[0]
            };

            // Do the intersection
            var returnedItems = activeSolid.IntersectEntitiesWithEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsFalse(activeSolid.Exists);

            // Test that the enities to remove don't exist
            foreach (PSEntity entityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(entityToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItems[0].Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains((PSMesh) returnedItems[0]));
        }

        /// <summary>
        /// A test for IntersectMeshAndSolidWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectMeshAndSolidWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSIntersectable>
            {
                _powerSHAPE.ActiveModel.Meshes[0],
                _powerSHAPE.ActiveModel.Solids[1]
            };

            // Do the intersection
            var returnedItems = activeSolid.IntersectEntitiesWithEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsFalse(activeSolid.Exists);

            // Test that the enities to remove don't exist
            foreach (PSEntity entityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(entityToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItems[0].Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains((PSMesh) returnedItems[0]));
        }

        /// <summary>
        /// A test for IntersectSolidAndSurfaceWithSolidTest
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void IntersectSolidAndSurfaceWithSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a solid and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSIntersectable>
            {
                _powerSHAPE.ActiveModel.Solids[1],
                _powerSHAPE.ActiveModel.Surfaces[0]
            };

            // Do the intersection
            var returnedItems = activeSolid.IntersectEntitiesWithEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the enitities to remove don't exist
            foreach (PSEntity enitityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(enitityToRemove.Exists);
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItems[0].Id, activeSolid.Id);
        }

        #endregion

        #region Subtract Operations

        /// <summary>
        /// A test for RemoveSolidFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveSolidFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solids
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var solidToRemove = _powerSHAPE.ActiveModel.Solids[1];

            // Do the subtraction
            var returnedItem = (PSSolid) activeSolid.SubtractEntityFromEntity(solidToRemove);

            // Test that the active solid still exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the solid to remove doesn't exist
            Assert.IsFalse(solidToRemove.Exists);

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for RemoveSolidsFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveSolidsFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solids
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var solidsToRemove = new List<PSSolid>
            {
                _powerSHAPE.ActiveModel.Solids[1],
                _powerSHAPE.ActiveModel.Solids[2]
            };

            // Do the subtraction
            var returnedItem = (PSSolid) activeSolid.SubtractEntitiesFromEntity(solidsToRemove);

            // Test that the active solid still exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the solids exist but are not in the collection
            foreach (var solidToRemove in solidsToRemove)
            {
                Assert.IsFalse(_powerSHAPE.ActiveModel.Solids.Contains(solidToRemove));
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for RemoveSolidFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveMeshFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var meshToRemove = _powerSHAPE.ActiveModel.Meshes[0];

            // Do the subtraction
            var returnedItem = (PSMesh) activeSolid.SubtractEntityFromEntity(meshToRemove);

            // Test that the active solid doesn't exist
            Assert.IsFalse(activeSolid.Exists);

            // Test that the mesh to remove doesn't exist
            Assert.IsFalse(meshToRemove.Exists);

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for RemoveSolidsFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveMeshesFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and two meshes
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var meshesToRemove = new List<PSMesh> {_powerSHAPE.ActiveModel.Meshes[0], _powerSHAPE.ActiveModel.Meshes[1]};

            // Do the subtraction
            var returnedItem = (PSMesh) activeSolid.SubtractEntitiesFromEntity(meshesToRemove);

            // Test that the active solid doesn't exist
            Assert.IsFalse(activeSolid.Exists);

            // Test that the meshes to remove doesn't exist
            foreach (var meshToRemove in meshesToRemove)
            {
                Assert.IsFalse(meshToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for RemoveSurfaceFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveSurfaceFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var surfaceToRemove = _powerSHAPE.ActiveModel.Surfaces[0];

            // Do the subtraction
            var returnedItem = (PSSolid) activeSolid.SubtractEntityFromEntity(surfaceToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the surface to remove doesn't exist
            Assert.IsFalse(surfaceToRemove.Exists);

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for RemoveSolidsFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveSurfacesFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and two surfaces
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var surfacesToRemove = new List<PSSurface>
            {
                _powerSHAPE.ActiveModel.Surfaces[0],
                _powerSHAPE.ActiveModel.Surfaces[1]
            };

            // Do the subtraction
            var returnedItem = (PSSolid) activeSolid.SubtractEntitiesFromEntity(surfacesToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the surfaces to remove don't exist
            foreach (var surfaceToRemove in surfacesToRemove)
            {
                Assert.IsFalse(surfaceToRemove.Exists);
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        /// <summary>
        /// A test for RemoveSolidFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveMeshAndSurfaceFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSSubtractable>
            {
                _powerSHAPE.ActiveModel.Meshes[0],
                _powerSHAPE.ActiveModel.Surfaces[0]
            };

            // Do the subtraction
            var returnedItem = (PSMesh) activeSolid.SubtractEntitiesFromEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsFalse(activeSolid.Exists);

            // Test that the enities to remove don't exist
            foreach (PSEntity entityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(entityToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for RemoveSolidsFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveMeshAndSolidFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a mesh and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSSubtractable>
            {
                _powerSHAPE.ActiveModel.Meshes[0],
                _powerSHAPE.ActiveModel.Solids[1]
            };

            // Do the subtraction
            var returnedItem = (PSMesh) activeSolid.SubtractEntitiesFromEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsFalse(activeSolid.Exists);

            // Test that the enities to remove don't exist
            foreach (PSEntity entityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(entityToRemove.Exists);
            }

            // Also check that the returned mesh exits
            Assert.IsTrue(returnedItem.Exists);

            // and is in the list of meshes
            Assert.IsTrue(_powerSHAPE.ActiveModel.Meshes.Contains(returnedItem));
        }

        /// <summary>
        /// A test for RemoveSolidFromSolid
        /// </summary>
        [Test]
        [Category("SolidTest")]
        public void RemoveSolidAndSurfaceFromSolidTest()
        {
            // Import the test model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.ADD_INTERSECT_SUBTRACT_MODEL));

            // Get the solid and a solid and a surface
            var activeSolid = _powerSHAPE.ActiveModel.Solids[0];
            var entitiesToRemove = new List<IPSSubtractable>
            {
                _powerSHAPE.ActiveModel.Solids[1],
                _powerSHAPE.ActiveModel.Surfaces[0]
            };

            // Do the subtraction
            var returnedItem = (PSSolid) activeSolid.SubtractEntitiesFromEntity(entitiesToRemove);

            // Test that the active solid exists
            Assert.IsTrue(activeSolid.Exists);

            // Test that the enitities to remove don't exist
            foreach (PSEntity enitityToRemove in entitiesToRemove)
            {
                Assert.IsFalse(enitityToRemove.Exists);
            }

            // Also check that the returned solid is the same as the original solid
            Assert.AreEqual(returnedItem.Id, activeSolid.Id);
        }

        #endregion

        #endregion
    }
}