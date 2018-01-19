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
    /// This is a test class for MeshTest and is intended
    /// to contain all MeshTest Unit Tests
    /// </summary>
    [TestFixture]
    public class MeshTest : EntityTest<PSMesh>
    {
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
                Assert.Fail("_entityCollection could not be set to Active Model's MeshesCollection\n\n" + e.Message);
            }
        }

        #region DatabaseEntity Tests

        /// <summary>
        /// A test for Id
        /// </summary>
        [Test]
        public void MeshIdTest()
        {
            IdTest(TestFiles.SINGLE_MESH);
        }

        /// <summary>
        /// A test for Identifier
        /// </summary>
        [Test]
        public void MeshIdentifierTest()
        {
            IdentifierTest(TestFiles.SINGLE_MESH, "SYMBOL");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void MeshEqualsTest()
        {
            Equals(TestFiles.SINGLE_MESH);
        }

        /// <summary>
        /// A test for Exists
        /// </summary>
        [Test]
        public void MeshExistsTest()
        {
            ExistsTest(TestFiles.SINGLE_MESH);
        }

        #endregion

        #region Entity Tests

        [Test]
        public void AddMeshToSelectionTest()
        {
            AddToSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_MESH);
        }

        [Test]
        public void BlankMeshTest()
        {
            BlankTest(TestFiles.THREE_SURFACES, TestFiles.SINGLE_MESH);
        }

        [Test]
        public void MeshBoundingBoxTest()
        {
            BoundingBoxTest(TestFiles.SINGLE_MESH, new Point(-67.5, -215, 0), new Point(22.5, -125, 90));
        }

        [Test]
        public void CopyMeshTest()
        {
            DuplicateTest(TestFiles.SINGLE_MESH);
        }

        [Test]
        public void DeleteMeshTest()
        {
            DeleteTest(TestFiles.SINGLE_MESH);
        }

        [Test]
        public void MeshLevelTest()
        {
            LevelTest(TestFiles.SINGLE_MESH, _powerSHAPE.ActiveModel.Levels[0]);
        }

        [Test]
        public void MeshLevelNumberTest()
        {
            LevelNumberTest(TestFiles.SINGLE_MESH, 0);
        }

        [Test]
        [Ignore("")]
        public void LimitMeshWithEntitiesTest()
        {
            // LimitToEntitiesTest(TestFiles.SINGLE_MESH, TestFiles.MESH_LIMITERS, PSEntityLimiter.LimitingModes.IntersectCurveMode);
            Assert.Inconclusive();
        }

        [Test]
        public void ScaleMeshTest()
        {
            ScaleNonUniformTest(TestFiles.SINGLE_MESH);

            ScaleUniformTest(TestFiles.SINGLE_MESH);
        }

        [Test]
        public void MirrorMeshTest()
        {
            // Setup parameters
            var initialMaxXPoint = new Point(22.5, -125.0, 90);
            var initialMinXPoint = new Point(-67.5, -215.0, 0);

            // Carry out operation
            MirrorTest(TestFiles.SINGLE_MESH, initialMaxXPoint);

            // Get point that should now be on mirror point
            var mirroredMesh = _powerSHAPE.ActiveModel.Meshes[0];
            Assert.AreEqual(initialMaxXPoint.X,
                            mirroredMesh.BoundingBox.MinX,
                            "Line was not mirrored around correct point");

            // Check that second test point is now the correct distance from its initial position
            Assert.AreEqual((initialMaxXPoint.X - initialMinXPoint.X) * 2.0,
                            mirroredMesh.BoundingBox.MaxX - initialMinXPoint.X,
                            "Line was not mirrored in correct plane");
        }

        [Test]
        public void OffsetMeshTest()
        {
            OffsetTest(TestFiles.SINGLE_MESH, new BoundingBox(-77.497276, 32.499458, -224.994537, -115.001091, -10, 100.0));
        }

        [Test]
        public void MoveMeshTest()
        {
            MoveTest(TestFiles.SINGLE_MESH);
        }

        [Test]
        public void MoveMeshCreatingCopiesTest()
        {
            MoveCreatingCopiesTest(TestFiles.SINGLE_MESH);
        }

        [Test]
        public void MoveMeshDifferentOriginTest()
        {
            MoveDifferentOriginTest(TestFiles.SINGLE_MESH, "Centre");
        }

        [Test]
        public void MeshNameTest()
        {
            NameTest(TestFiles.SINGLE_MESH, "1", "NewName");

            // Ensures that PowerShape entity was updated
            Assert.AreEqual(1, _powerSHAPE.ExecuteEx("symbol[NewName].EXISTS"));
        }

        [Test]
        public void RemoveMeshFromSelectionTest()
        {
            RemoveFromSelectionTest(TestFiles.SINGLE_SURFACE, TestFiles.SINGLE_MESH);
        }

        [Test]
        public void RotateMeshTest()
        {
            RotateTest(TestFiles.SINGLE_MESH);
        }

        [Test]
        public void RotateMeshCreatingCopiesTest()
        {
            RotateCreatingCopiesTest(TestFiles.SINGLE_MESH);
        }

        /// <summary>
        /// Test the distance between two Meshes
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceMeshMesh()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Meshes[0];
            var entity2 = _powerSHAPE.ActiveModel.Meshes[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        /// <summary>
        /// Test the distance between an Mesh and a compcurve
        /// </summary>
        [Test]
        [Ignore("")]
        public void MinimumDistanceMeshCompCurve()
        {
            // Import the PS Model
            _powerSHAPE.Models.CreateModelFromFile(new File(TestFiles.MINIMUM_DISTANCE_TESTS));

            // Get the objects for test
            var entity1 = _powerSHAPE.ActiveModel.Meshes[0];
            var entity2 = _powerSHAPE.ActiveModel.CompCurves[1];

            // Test the distance
            Assert.Inconclusive();

            //Assert.AreEqual(0.0, entity1.DistanceTo(entity2));
        }

        #endregion

        #region " Mesh tests "

        [Test]
        public void Boundaries()
        {
            _powerSHAPE.ActiveModel.Import(new File(TestFiles.THREE_MESHES_COMBINED));
            var boundaries = _powerSHAPE.ActiveModel.Meshes[0].CreateBoundary();
            Assert.AreEqual(3, boundaries.Count);
        }

        [Test]
        public void NumberOfTrianglesTest()
        {
            var importedMeshes = _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_MESH));
            Assert.AreEqual(12, ((PSMesh) importedMeshes[0]).NumberOfTriangles);
        }

        [Test]
        public void NumberOfNodesTest()
        {
            var importedMeshes = _powerSHAPE.ActiveModel.Import(new File(TestFiles.SINGLE_MESH));
            Assert.AreEqual(8, ((PSMesh) importedMeshes[0]).NumberOfNodes);
        }

        #endregion
    }
}