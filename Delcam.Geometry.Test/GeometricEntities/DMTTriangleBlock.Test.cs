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
using NUnit.Framework;

namespace Autodesk.Geometry.Test.GeometricEntities
{
    [TestFixture]
    public class DMTTriangleBlockTest
    {
        [Test]
        public void AddTriangleTest()
        {
            var model = new DMTModel();
            var block = new DMTTriangleBlock();
            model.AddTriangleBlock(block);

            block.AddTriangle(new Point(-119, 6, 0), new Point(-50, 67, 0), new Point(-13, 4, 0));
            block.AddTriangle(new Point(-13, 4, 0), new Point(-50, 67, 0), new Point(47, 58, 0));

            // Ensure that model is written correctly
            Assert.AreEqual(model.BoundingBox.MaxX, 47);
            Assert.AreEqual(model.BoundingBox.MaxY, 67);
            Assert.AreEqual(model.BoundingBox.MaxZ, 0);
            Assert.AreEqual(model.BoundingBox.MinX, -119);
            Assert.AreEqual(model.BoundingBox.MinY, 4);
            Assert.AreEqual(model.BoundingBox.MinZ, 0);
            Assert.AreEqual(block.NoOfTriangles, 2);
            Assert.AreEqual(block.NoOfVertices, 6);
        }

        [Test]
        public void CloneTest()
        {
            var originalModel = DMTModelReader.ReadFile(new FileSystem.File(TestFiles.NormalStl));
            var cloneModel = originalModel.Clone();

            // Ensure that model is written correctly
            Assert.AreEqual(originalModel.TotalNoOfTriangles, cloneModel.TotalNoOfTriangles);
            Assert.AreEqual(originalModel.TotalNoOfVertices, cloneModel.TotalNoOfVertices);
        }

        [Test]
        public void VerticesTest()
        {
            var model = DMTModelReader.ReadFile(new FileSystem.File(TestFiles.BoundariesTestSquare));
            var triangleBlock = model.TriangleBlocks.Single();

            var fieldInfo = triangleBlock.GetType().GetField("_triangleVertices", BindingFlags.Instance | BindingFlags.NonPublic);
            var vertices = (List<Point>) fieldInfo.GetValue(triangleBlock);

            var triangleBlockVertices = triangleBlock.Vertices.Cast<DMTVertex>().ToList();

            Assert.AreEqual(vertices[0].X, triangleBlockVertices[0].Position.X, "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(vertices[0].Y, triangleBlockVertices[0].Position.Y, "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(vertices[0].Z, triangleBlockVertices[0].Position.Z, "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(vertices[3].X, triangleBlockVertices[3].Position.X, "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(vertices[3].Y, triangleBlockVertices[3].Position.Y, "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(vertices[3].Z, triangleBlockVertices[3].Position.Z, "Vertices is not retrieving the correct vertex.");
        }

        [Test]
        public void TrianglesTest()
        {
            var model = DMTModelReader.ReadFile(new FileSystem.File(TestFiles.BoundariesTestSquare));
            var triangleBlock = model.TriangleBlocks.Single();

            var fieldInfo = triangleBlock.GetType().GetField("_triangleVertices", BindingFlags.Instance | BindingFlags.NonPublic);
            var vertices = (List<Point>) fieldInfo.GetValue(triangleBlock);

            var triangles = triangleBlock.Triangles.Cast<DMTTriangle>().ToList();

            Assert.AreEqual(0,
                            Math.Round(vertices[triangles[0].Vertex1].X.Value),
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(-10,
                            vertices[triangles[0].Vertex1].Y.Value,
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(-10,
                            vertices[triangles[0].Vertex1].Z.Value,
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(0,
                            Math.Round(vertices[triangles[0].Vertex2].X.Value),
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(10,
                            vertices[triangles[0].Vertex2].Y.Value,
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(-10,
                            vertices[triangles[0].Vertex2].Z.Value,
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(0,
                            Math.Round(vertices[triangles[0].Vertex3].X.Value),
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(-10,
                            vertices[triangles[0].Vertex3].Y.Value,
                            "Vertices is not retrieving the correct vertex.");
            Assert.AreEqual(10,
                            vertices[triangles[0].Vertex3].Z.Value,
                            "Vertices is not retrieving the correct vertex.");
        }
    }
}