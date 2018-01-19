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
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class DMTModelReaderTest
    {
        [Test]
        public void WhenAppendingBinarySTLFile_ThenCheckOutput()
        {
            DMTModel importedModel = DMTModelReader.ReadFile(new File(TestFiles.NormalStl));

            // Ensure that model is loaded correctly
            Assert.AreEqual(importedModel.BoundingBox.MaxX, -54);
            Assert.AreEqual(importedModel.BoundingBox.MaxY, 111);
            Assert.AreEqual(importedModel.BoundingBox.MaxZ, 70);
            Assert.AreEqual(importedModel.BoundingBox.MinX, -124);
            Assert.AreEqual(importedModel.BoundingBox.MinY, 41);
            Assert.AreEqual(importedModel.BoundingBox.MinZ, 0);
            Assert.AreEqual(importedModel.TotalNoOfTriangles, 12);
            Assert.AreEqual(importedModel.TotalNoOfVertices, 8);
        }

        [Test]
        public void WhenAppendingDMTFile_ThenCheckOutput()
        {
            DMTModel importedModel = DMTModelReader.ReadFile(new File(TestFiles.NormalDmt));

            // Ensure that model is loaded correctly
            Assert.AreEqual(-54, importedModel.BoundingBox.MaxX.Value);
            Assert.AreEqual(111, importedModel.BoundingBox.MaxY.Value);
            Assert.AreEqual(70, importedModel.BoundingBox.MaxZ.Value);
            Assert.AreEqual(-124, importedModel.BoundingBox.MinX.Value);
            Assert.AreEqual(41, importedModel.BoundingBox.MinY.Value);
            Assert.AreEqual(0, importedModel.BoundingBox.MinZ.Value);
            Assert.AreEqual(12, importedModel.TotalNoOfTriangles);
            Assert.AreEqual(8, importedModel.TotalNoOfVertices);
            Assert.AreEqual(8, importedModel.TriangleBlocks.First().VertexNormals.Count);
        }

        [Test]
        public void WhenAppendingBinarySTLFile_ThenEnsureDuplicatePointsRemoval()
        {
            var inputFile = new File(TestFiles.FetchTestFile("GetTrianglesByMesh.stl"));
            DMTModel mainMesh = DMTModelReader.ReadFile(inputFile);

            Assert.AreEqual(944, mainMesh.TotalNoOfTriangles);
            Assert.AreEqual(944, mainMesh.TotalNoOfVertices);

            var exportedFile = File.CreateTemporaryFile("stl", true);
            DMTModelWriter.WriteFile(mainMesh, exportedFile);

            DMTModel exportedMesh = DMTModelReader.ReadFile(inputFile);
            Assert.AreEqual(944, exportedMesh.TotalNoOfTriangles);
            Assert.AreEqual(944, exportedMesh.TotalNoOfVertices);
        }

        [Test]
        public void WhenReadingDMTWithTwoTriangleBlocks_ThenCheckOutput()
        {
            var inputFile = new File(TestFiles.FetchTestFile("TwoBlocks.dmt"));
            DMTModel mainMesh = DMTModelReader.ReadFile(inputFile);

            Assert.AreEqual(4, mainMesh.TotalNoOfTriangles);
            Assert.AreEqual(8, mainMesh.TotalNoOfVertices);
            Assert.AreEqual(-62.08, Math.Round(mainMesh.BoundingBox.MinX, 2));
            Assert.AreEqual(78.64, Math.Round(mainMesh.BoundingBox.MaxX, 2));
            Assert.AreEqual(5.70, Math.Round(mainMesh.BoundingBox.MinY, 2));
            Assert.AreEqual(76.08, Math.Round(mainMesh.BoundingBox.MaxY, 2));
            Assert.AreEqual(0, Math.Truncate(mainMesh.BoundingBox.MinZ));
            Assert.AreEqual(0, Math.Truncate(mainMesh.BoundingBox.MaxZ));
        }
    }
}