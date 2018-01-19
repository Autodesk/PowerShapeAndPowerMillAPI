// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Linq;
using Autodesk.FileSystem;
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class DMTModelTest
    {
        [Test]
        public void DMTPointProjectionTest()
        {
            // Ensures result of projecting a known point is as expected.
            DMTModel dmtModel = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("Point Projection.dmt")));

            // testPoint correlates with PowerSHAPE model.
            var testPoint = new Point(0, 0, 998.465026855469);

            // NOTE: ProjectPoints(...) is implicitly tested by a call to ProjectPoint(...).
            Point projectedPoint = dmtModel.ProjectPoint(new Point(0, 0, 2000));

            Assert.AreEqual(testPoint, projectedPoint);
            Assert.Pass();
        }

        [Test]
        public void WhenCoincidentPointsInATriangle_ThenStillAppendBinarySTLFile()
        {
            var mesh = new File(TestFiles.FetchTestFile("CoincidentPointsInATriangle.stl"));
            var importedModel = DMTModelReader.ReadFile(mesh);

            Assert.That(importedModel.TotalNoOfTriangles, Is.EqualTo(3), "Failed expected number of triangles.");
            Assert.That(importedModel.TotalNoOfVertices, Is.EqualTo(3), "Failed expected number of vertices.");

            var expectedOutput =
                DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("CoincidentPointsInATriangle_Expected.stl")));

            Assert.IsTrue(importedModel.BoundingBox.XSize == expectedOutput.BoundingBox.XSize,
                          "It is not giving the expected bounding box xsize.");
            Assert.IsTrue(importedModel.BoundingBox.YSize == expectedOutput.BoundingBox.YSize,
                          "It is not giving the expected bounding box xsize.");
            Assert.IsTrue(importedModel.BoundingBox.ZSize == expectedOutput.BoundingBox.ZSize,
                          "It is not giving the expected bounding box xsize.");
        }

        [Test]
        public void ProjectPointsTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.SmallModel));
            var nearestPoint = modelToZone.ProjectPoint(new Point(-28.186, -4.1135, 1000));
            Assert.AreNotEqual(nearestPoint, null);
        }

        [Test]
        public void GetBoundariesFromAMeshWithOneBoundaryTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("BaseLeftFootTopSurface.dmt")));
            var boundaries = modelToZone.Boundaries();

            Assert.AreEqual(boundaries.Count, 1, "Failed to calculate Boundaries. Mesh should have only one boundary.");
        }

        [Test]
        public void GetBoundariesFromAMeshWithFiveBoundariesTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("MeshForBoundaryCount.dmt")));
            var boundaries = modelToZone.Boundaries();

            Assert.AreEqual(boundaries.Count, 5, "Failed to calculate Boundaries. Mesh only has five boundaries.");
        }

        [Test]
        public void GetBoundariesFromAMeshWithTwoBoundariesTest()
        {
            DMTModel modelToZone =
                DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("MeshForBoundaryCountAfterStitching.dmt")));
            var boundaries = modelToZone.Boundaries();

            Assert.AreEqual(boundaries.Count, 2, "Failed to calculate Boundaries. Mesh only has two boundaries.");
        }

        [Test]
        public void GetBoundaryNodesTest()
        {
            DMTModel modelToZone = DMTModelReader.ReadFile(new File(TestFiles.FetchTestFile("BaseLeftFootTopSurface.dmt")));
            var boundaryNodes = modelToZone.BoundaryNodes();

            Assert.AreEqual(boundaryNodes.Count, 313, "Failed to calculate BoundaryNodes.");
        }

        [Test]
        public void WhenMoveByVector_ThenAllBlocksShouldBeMoved()
        {
            DMTModel modelToMove = DMTModelReader.ReadFile(new File(TestFiles.SmallModel));
            var movement = new Vector(1, 4, 2);
            var originalVertices = modelToMove.TriangleBlocks[0].Vertices.Cast<DMTVertex>().ToList();
            originalVertices.ForEach(x => x.Position = x.Position + movement);
            modelToMove.Move(movement);
            var newVertices = modelToMove.TriangleBlocks[0].Vertices.Cast<DMTVertex>().ToList();

            CollectionAssert.AreEqual(originalVertices, newVertices);
        }
    }
}