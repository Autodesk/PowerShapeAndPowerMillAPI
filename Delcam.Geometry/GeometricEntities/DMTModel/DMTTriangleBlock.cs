// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Represents a block of triangles. One or more blocks make up a DMT model.
    /// </summary>
    [Serializable]
    public class DMTTriangleBlock
    {
        #region "Fields"

        /// <summary>
        /// These are the first vertices indices of the triangles that make up the block.
        /// </summary>
        private List<int> _triangleFirstVertexIndices;

        /// <summary>
        /// These are the second vertices indices of the triangles that make up the block.
        /// </summary>
        private List<int> _triangleSecondVertexIndices;

        /// <summary>
        /// These are the third vertices indices of the triangles that make up the block.
        /// </summary>
        private List<int> _triangleThirdVertexIndices;

        /// <summary>
        /// These are the raw vertices that make up the triangles of the block.
        /// </summary>
        private List<Point> _triangleVertices;

        /// <summary>
        /// These are the triangle normals
        /// </summary>
        private List<Vector> _vertexNormals;

        /// <summary>
        /// This indicates whether the vertices in this block have normals
        /// </summary>
        private bool _doVerticesHaveNormals;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an empty DMTTriangleBlock.
        /// </summary>
        public DMTTriangleBlock()
        {
            _triangleFirstVertexIndices = new List<int>();
            _triangleSecondVertexIndices = new List<int>();
            _triangleThirdVertexIndices = new List<int>();
            _triangleVertices = new List<Point>();
            _vertexNormals = new List<Vector>();

            // By default nodes don't have normals
            _doVerticesHaveNormals = false;
        }

        /// <summary>
        /// Constructs a DMTTriangleBlock and populates it with the passed list of triangles and vertices.
        /// </summary>
        /// <param name="triangles">List of triangles with which to populate the block.</param>
        /// <param name="vertices">List of vertices.</param>
        /// <param name="nodesHaveIJK">If True, vertices have IJK vector normals.</param>
        public DMTTriangleBlock(List<DMTTriangle> triangles, List<DMTVertex> vertices, bool nodesHaveIJK)
        {
            // Add triangles
            _triangleFirstVertexIndices.AddRange(triangles.Select(x => x.Vertex1));
            _triangleSecondVertexIndices.AddRange(triangles.Select(x => x.Vertex2));
            _triangleThirdVertexIndices.AddRange(triangles.Select(x => x.Vertex3));

            // Add vertices
            _triangleVertices.AddRange(vertices.Select(x => x.Position));

            if (nodesHaveIJK)
            {
                _doVerticesHaveNormals = true;
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// List of first vertice indeces for the triangles that make up the block.
        /// </summary>
        internal List<int> TriangleFirstVertexIndices
        {
            get { return _triangleFirstVertexIndices; }

            set { _triangleFirstVertexIndices = value; }
        }

        /// <summary>
        /// List of second vertice indeces for the triangles that make up the block.
        /// </summary>
        internal List<int> TriangleSecondVertexIndices
        {
            get { return _triangleSecondVertexIndices; }

            set { _triangleSecondVertexIndices = value; }
        }

        /// <summary>
        /// List of third vertice indeces for the triangles that make up the block.
        /// </summary>
        internal List<int> TriangleThirdVertexIndices
        {
            get { return _triangleThirdVertexIndices; }

            set { _triangleThirdVertexIndices = value; }
        }

        /// <summary>
        /// List of all the vertices.
        /// </summary>
        internal List<Point> TriangleVertices
        {
            get { return _triangleVertices; }

            set { _triangleVertices = value; }
        }

        /// <summary>
        /// List of all the vertex normals.
        /// </summary>
        public List<Vector> VertexNormals
        {
            get { return _vertexNormals; }

            set { _vertexNormals = value; }
        }

        /// <summary>
        /// Number of triangles in a block.
        /// </summary>
        public int NoOfTriangles
        {
            get { return _triangleFirstVertexIndices.Count; }
        }

        /// <summary>
        /// Number of vertices in a block.
        /// </summary>
        public int NoOfVertices
        {
            get { return _triangleVertices.Count; }
        }

        /// <summary>
        /// If True, vertices have IJK normals. False otherwise.
        /// </summary>
        public bool DoVerticesHaveNormals
        {
            get { return _doVerticesHaveNormals; }

            set { _doVerticesHaveNormals = value; }
        }

        /// <summary>
        /// Gets the flags for the Block.  1 if nodes have normals, 0 otherwise
        /// </summary>
        public uint Flags
        {
            get
            {
                if (_doVerticesHaveNormals)
                {
                    return 1;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the vertices of a triangle block.
        /// </summary>
        /// <value></value>
        public IEnumerable Vertices
        {
            get { return NextVertex(); }
        }

        private IEnumerable NextVertex()
        {
            int index = 0;
            while (index < NoOfVertices)
            {
                yield return new DMTVertex(GetVertex(index));
                index = index + 1;
            }
        }

        /// <summary>
        /// Gets the triangles of a triangle block.
        /// </summary>
        /// <value></value>
        public IEnumerable Triangles
        {
            get { return NextTriangle(); }
        }

        private IEnumerable NextTriangle()
        {
            int index = 0;
            while (index < NoOfTriangles)
            {
                yield return new DMTTriangle(GetVertex1Index(index), GetVertex2Index(index), GetVertex3Index(index));
                index = index + 1;
            }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a clone of this block.
        /// </summary>
        public DMTTriangleBlock Clone()
        {
            DMTTriangleBlock cloneBlock = new DMTTriangleBlock();
            cloneBlock.DoVerticesHaveNormals = DoVerticesHaveNormals;

            // Clone triangles
            cloneBlock.TriangleFirstVertexIndices.AddRange(_triangleFirstVertexIndices);
            cloneBlock.TriangleSecondVertexIndices.AddRange(_triangleSecondVertexIndices);
            cloneBlock.TriangleThirdVertexIndices.AddRange(_triangleThirdVertexIndices);

            // Clone vertices
            foreach (Point vertex in _triangleVertices)
            {
                cloneBlock.TriangleVertices.Add(vertex.Clone());
            }

            return cloneBlock;
        }

        /// <summary>
        /// Returns the vector normal of the first triangle with that vertex.
        /// </summary>
        /// <returns>The normal for the first triangle with specified vertex.</returns>
        /// <remarks></remarks>
        public Vector GetNormal(Point vertex)
        {
            int triangleIndex = GetTrianglesAtVertex(vertex).First();
            return DMTTriangle.GetNormal(GetVertex1(triangleIndex), GetVertex2(triangleIndex), GetVertex3(triangleIndex));
        }

        /// <summary>
        /// Get the vertex by its index.
        /// </summary>
        /// <param name="vertexIndex">The index of the point that represents vertex.</param>
        /// <returns>The vertex point.</returns>
        /// <remarks></remarks>
        internal Point GetVertex(int vertexIndex)
        {
            return _triangleVertices[vertexIndex];
        }

        /// <summary>
        /// Get the vertex 1 by its triangle index.
        /// </summary>
        /// <param name="triangleIndex">The index of the triangle that contains the vertex.</param>
        /// <returns>The first vertex of the triangle.</returns>
        /// <remarks></remarks>
        internal Point GetVertex1(int triangleIndex)
        {
            return GetVertex(GetVertex1Index(triangleIndex));
        }

        /// <summary>
        /// Get the vertex 2 by its triangle index.
        /// </summary>
        /// <param name="triangleIndex">The index of the triangle that contains the vertex.</param>
        /// <returns>The second vertex of the triangle.</returns>
        /// <remarks></remarks>
        internal Point GetVertex2(int triangleIndex)
        {
            return GetVertex(GetVertex2Index(triangleIndex));
        }

        /// <summary>
        /// Get the vertex 3 by its triangle index.
        /// </summary>
        /// <param name="triangleIndex">The index of the triangle that contains the vertex.</param>
        /// <returns>The third vertex of the triangle.</returns>
        /// <remarks></remarks>
        internal Point GetVertex3(int triangleIndex)
        {
            return GetVertex(GetVertex3Index(triangleIndex));
        }

        /// <summary>
        /// Get the vertex 1 index by its triangle index.
        /// </summary>
        /// <param name="triangleIndex">The index of the triangle that contains the vertex.</param>
        /// <returns>The first vertex index in the list of vertices.</returns>
        /// <remarks></remarks>
        internal int GetVertex1Index(int triangleIndex)
        {
            return _triangleFirstVertexIndices[triangleIndex];
        }

        /// <summary>
        /// Get the vertex 2 index by its triangle index.
        /// </summary>
        /// <param name="triangleIndex">The index of the triangle that contains the vertex.</param>
        /// <returns>The second vertex index in the list of vertices.</returns>
        /// <remarks></remarks>
        internal int GetVertex2Index(int triangleIndex)
        {
            return _triangleSecondVertexIndices[triangleIndex];
        }

        /// <summary>
        /// Get the vertex 3 index by its triangle index.
        /// </summary>
        /// <param name="triangleIndex">The index of the triangle that contains the vertex.</param>
        /// <returns>The third vertex index in the list of vertices.</returns>
        /// <remarks></remarks>
        internal int GetVertex3Index(int triangleIndex)
        {
            return _triangleThirdVertexIndices[triangleIndex];
        }

        /// <summary>
        /// Get triangles indices sharing vertex.
        /// </summary>
        /// <param name="vertex">The vertex to which the triangles are connected.</param>
        /// <returns>The triangle indices sharing the vertex.</returns>
        /// <remarks></remarks>
        internal List<int> GetTrianglesAtVertex(Point vertex)
        {
            var result = new List<int>();

            var vertexIndex = _triangleVertices.IndexOf(vertex);
            result.AddRange(_triangleFirstVertexIndices.Where(index => index == vertexIndex));
            result.AddRange(_triangleSecondVertexIndices.Where(index => index == vertexIndex));
            result.AddRange(_triangleThirdVertexIndices.Where(index => index == vertexIndex));

            return result.Where(x => x != -1).ToList();
        }

        /// <summary>
        /// Adds a triangle to the block using the vertex points.
        /// </summary>
        /// <param name="vertex1">The first vertex.</param>
        /// <param name="vertex2">The second vertex.</param>
        /// <param name="vertex3">The third vertex.</param>
        /// <remarks></remarks>
        public void AddTriangle(Point vertex1, Point vertex2, Point vertex3)
        {
            //Add positions and triangles
            _triangleVertices.Add(vertex1);
            _triangleFirstVertexIndices.Add(_triangleVertices.Count - 1);
            _triangleVertices.Add(vertex2);
            _triangleSecondVertexIndices.Add(_triangleVertices.Count - 1);
            _triangleVertices.Add(vertex3);
            _triangleThirdVertexIndices.Add(_triangleVertices.Count - 1);
        }

        /// <summary>
        /// Adds a triangle to the block using vertex indices.
        /// </summary>
        /// <param name="vertex1Index">The first vertex index.</param>
        /// <param name="vertex2Index">The second vertex index.</param>
        /// <param name="vertex3Index">The third vertex index.</param>
        /// <remarks></remarks>
        public void AddTriangle(int vertex1Index, int vertex2Index, int vertex3Index)
        {
            // Add triangles
            _triangleFirstVertexIndices.Add(vertex1Index);
            _triangleSecondVertexIndices.Add(vertex2Index);
            _triangleThirdVertexIndices.Add(vertex3Index);
        }

        /// <summary>
        /// Manually add a vertex to your triangle block
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        /// <remarks></remarks>
        public void AddVertex(Point vertex)
        {
            _triangleVertices.Add(vertex);
        }

        #endregion
    }
}