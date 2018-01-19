// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates a triangle. One or more triangles constitute a block.
    /// </summary>
    [Serializable]
    public class DMTTriangle
    {
        #region "Fields"

        /// <summary>
        /// Index of the first vertex in the triangle
        /// </summary>
        private int _vertex1;

        /// <summary>
        /// Index of the second vertex in the triangle
        /// </summary>
        private int _vertex2;

        /// <summary>
        /// Index of the third vertex in the triangle
        /// </summary>
        private int _vertex3;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs a DMTTriangle with the specified vertex indices.
        /// </summary>
        /// <param name="vertex1">Vertex index 1.</param>
        /// <param name="vertex2">Vertex index 2.</param>
        /// <param name="vertex3">Vertex index 3.</param>
        public DMTTriangle(int vertex1, int vertex2, int vertex3)
        {
            _vertex1 = vertex1;
            _vertex2 = vertex2;
            _vertex3 = vertex3;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Index of vertex 1.
        /// </summary>
        public int Vertex1
        {
            get { return _vertex1; }

            set { _vertex1 = value; }
        }

        /// <summary>
        /// Index of vertex 2.
        /// </summary>
        public int Vertex2
        {
            get { return _vertex2; }

            set { _vertex2 = value; }
        }

        /// <summary>
        /// Index of vertex 3.
        /// </summary>
        public int Vertex3
        {
            get { return _vertex3; }

            set { _vertex3 = value; }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Returns a vector normal to the surface of the triangle.
        /// </summary>
        /// <param name="block">Block containing this triangle is part of.</param>
        public Vector GetNormal(DMTTriangleBlock block)
        {
            return GetNormal(block.GetVertex(_vertex1), block.GetVertex(_vertex2), block.GetVertex(_vertex3));
        }

        /// <summary>
        /// Returns a unit normal vector to the surface of the triangle.
        /// </summary>
        /// <param name="vertex1">Point for the first triangle vertex.</param>
        /// <param name="vertex2">Point for the second triangle vertex.</param>
        /// <param name="vertex3">Point for the third triangle vertex.</param>
        /// <returns>The unit normal vector for the triangle.</returns>
        /// <remarks></remarks>
        public static Vector GetNormal(Point vertex1, Point vertex2, Point vertex3)
        {
            // Determine the cross product of the vectors from vertex 1 to vertex 2 and vertex 1 to vertex3
            Vector vertex1To2 = vertex2 - vertex1;
            Vector vertex1To3 = vertex3 - vertex1;

            // This will be the normal to the triangles surface
            var normal = Vector.CrossProduct(vertex1To2, vertex1To3);
            normal.Normalize();
            return normal;
        }

        /// <summary>
        /// Gets triangle centroid.
        /// </summary>
        /// <param name="vertex1">Triangle Vertex.</param>
        /// <param name="vertex2">Second vertex.</param>
        /// <param name="vertex3">Third vertex.</param>
        /// <returns>The point that represents the centroid of the triangle.</returns>
        public static Point GetCentroid(Point vertex1, Point vertex2, Point vertex3)
        {
            return new Point((vertex1.X + vertex2.X + vertex3.X) / 3.0,
                             (vertex1.Y + vertex2.Y + vertex3.Y) / 3.0,
                             (vertex1.Z + vertex2.Z + vertex3.Z) / 3.0);
        }

        #endregion

        #region "Clone Operation"

        /// <summary>
        /// Returns a clone of this triangle.
        /// </summary>
        public DMTTriangle Clone()
        {
            DMTTriangle cloneTriangle = new DMTTriangle(Vertex1, Vertex2, Vertex3);
            return cloneTriangle;
        }

        #endregion
    }
}