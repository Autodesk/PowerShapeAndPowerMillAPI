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
using System.Threading.Tasks;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Handles the reading and writing of DMT files and facilitates the manipulation of triangles
    /// therein.
    /// </summary>
    /// <remarks>
    /// A DMT Model contains many triangle blocks, with each block containing many vertices and triangles.
    /// Each vertex has a position, may have a normal and contains a list of indices referencing
    /// the triangles that connect to it.
    /// Each triangle has three indices referencing the vertices that comprise it.
    /// </remarks>
    [Serializable]
    public class DMTModel
    {
        #region "Fields"

        /// <summary>
        /// </summary>
        /// <remarks></remarks>
        private const int ZONE_DENSITY_DEFAULT = 5;

        /// <summary>
        /// Granularity of vertex fencing.
        /// </summary>
        /// <remarks></remarks>
        private int _zoneDensity = ZONE_DENSITY_DEFAULT;

        /// <summary>
        /// This is the list of triangle blocks that make up the <see cref="DMTModel"/>.
        /// </summary>
        private List<DMTTriangleBlock> _blocks;

        /// <summary>
        /// This is the list of triangle zones within the <see cref="DMTModel"/>. These are used during
        /// point projection.
        /// </summary>
        private List<List<DMTTriangleZone>> _zones;

        /// <summary>
        /// This is the total number of zones.
        /// </summary>
        private int _numberOfZones;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an empty <see cref="DMTModel"/> and initialises an empty list of blocks.
        /// </summary>
        public DMTModel()
        {
            _blocks = new List<DMTTriangleBlock>();
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// This is the list of triangle zones within the <see cref="DMTModel"/>. These are used during
        /// point projection.
        /// </summary>
        public List<List<DMTTriangleZone>> Zones
        {
            get { return _zones; }

            set { _zones = value; }
        }

        #endregion

        #region "Transform Operations"

        /// <summary>
        /// Uses the specified scalar to scale the model about the origin.
        /// </summary>
        /// <param name="scalar">Scaling factor.</param>
        public void Scale(double scalar)
        {
            Scale(new Point(0.0, 0.0, 0.0), scalar);
        }

        /// <summary>
        /// Uses the specified scalar to scale the model about the specified origin.
        /// </summary>
        /// <param name="origin">Origin about which to scale the model.</param>
        /// <param name="scalar">Scaling factor.</param>
        public void Scale(Point origin, double scalar)
        {
            Scale(origin, scalar, scalar, scalar);
        }

        /// <summary>
        /// Uses the specified scalars to scale the model about the origin.
        /// </summary>
        /// <param name="scalarX">The factor by which to scale in X.</param>
        /// <param name="scalarY">The factor by which to scale in Y.</param>
        /// <param name="scalarZ">The factor by which to scale in Z.</param>
        public void Scale(double scalarX, double scalarY, double scalarZ)
        {
            Scale(new Point(0.0, 0.0, 0.0), scalarX, scalarY, scalarZ);
        }

        /// <summary>
        /// Uses the specified scalars to scale the model about the specified origin origin.
        /// </summary>
        /// <param name="origin">The origin about which to scale.</param>
        /// <param name="scalarX">The factor by which to scale in X.</param>
        /// <param name="scalarY">The factor by which to scale in Y.</param>
        /// <param name="scalarZ">The factor by which to scale in Z.</param>
        public void Scale(Point origin, double scalarX, double scalarY, double scalarZ)
        {
            // Move the polyline so the specified origin is at the polyline's origin
            Vector offset = new Vector(-origin.X, -origin.Y, -origin.Z);
            Move(offset);

            // Scale the points
            foreach (DMTTriangleBlock block in _blocks)
            {
                foreach (Point vertex in block.TriangleVertices)
                {
                    vertex.X *= scalarX;
                    vertex.Y *= scalarY;
                    vertex.Z *= scalarZ;
                }
            }

            // Move the polyline back
            Move(-1 * offset);
        }

        /// <summary>
        /// Moves the model by the specified Vector in the ActiveWorkplane - or in the world if none is set.
        /// </summary>
        /// <param name="offset">Offset vector.</param>
        public void Move(Vector offset)
        {
            foreach (DMTTriangleBlock block in _blocks)
            {
                for (int i = 0; i < block.TriangleVertices.Count; i++)
                {
                    block.TriangleVertices[i] += offset;
                }
            }
        }

        /// <summary>
        /// Rotates the entire model by the speified angle about the X axis and about the
        /// specified origin in the ActiveWorkplane - or the world if none is set.
        /// </summary>
        /// <param name="origin">Origin of rotation.</param>
        /// <param name="angle">Angle of rotation.</param>
        public void RotateAboutXAxis(Point origin, Radian angle)
        {
            foreach (DMTTriangleBlock block in _blocks)
            {
                foreach (Point vertex in block.TriangleVertices)
                {
                    vertex.RotateAboutXAxis(origin, angle);
                }
            }
        }

        /// <summary>
        /// Rotates the entire model by the speified angle about the Y axis and about the
        /// specified origin in the ActiveWorkplane - or the world if none is set.
        /// </summary>
        /// <param name="origin">Origin of rotation.</param>
        /// <param name="angle">Angle of rotation.</param>
        public void RotateAboutYAxis(Point origin, Radian angle)
        {
            foreach (DMTTriangleBlock block in _blocks)
            {
                foreach (Point vertex in block.TriangleVertices)
                {
                    vertex.RotateAboutYAxis(origin, angle);
                }
            }
        }

        /// <summary>
        /// Rotates the entire model by the speified angle about the Z axis and about the
        /// specified origin in the ActiveWorkplane - or the world if none is set.
        /// </summary>
        /// <param name="origin">Origin of rotation.</param>
        /// <param name="angle">Angle of rotation.</param>
        public void RotateAboutZAxis(Point origin, Radian angle)
        {
            foreach (DMTTriangleBlock block in _blocks)
            {
                foreach (Point vertex in block.TriangleVertices)
                {
                    vertex.RotateAboutZAxis(origin, angle);
                }
            }
        }

        /// <summary>
        /// Rebases the model from the world to the specified workplane.
        /// </summary>
        /// <param name="toWorkplane">Target workplane.</param>
        public void RebaseToWorkplane(Workplane toWorkplane)
        {
            foreach (DMTTriangleBlock block in _blocks)
            {
                for (int intVerticeNo = 0; intVerticeNo <= block.NoOfVertices - 1; intVerticeNo++)
                {
                    block.TriangleVertices.ElementAt(intVerticeNo).RebaseToWorkplane(toWorkplane);
                }
            }
        }

        /// <summary>
        /// Rebases the model from the specified workplane to the world.
        /// </summary>
        /// <param name="fromWorkplane">Source workplane.</param>
        public void RebaseFromWorkplane(Workplane fromWorkplane)
        {
            foreach (DMTTriangleBlock block in _blocks)
            {
                for (int intVerticeNo = 0; intVerticeNo <= block.NoOfVertices - 1; intVerticeNo++)
                {
                    block.TriangleVertices.ElementAt(intVerticeNo).RebaseFromWorkplane(fromWorkplane);
                }
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Zone density used by the model. Default is 5.
        /// </summary>
        public int ZoneDensity
        {
            get { return _zoneDensity; }
            set { _zoneDensity = value; }
        }

        /// <summary>
        /// List of <see cref="DMTTriangleBlock"/> used by the model.
        /// </summary>
        public List<DMTTriangleBlock> TriangleBlocks
        {
            get { return _blocks; }
            set { _blocks = value; }
        }

        /// <summary>
        /// Bounding box of the model.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                MM minX = double.MaxValue;
                MM maxX = double.MinValue;
                MM minY = double.MaxValue;
                MM maxY = double.MinValue;
                MM minZ = double.MaxValue;
                MM maxZ = double.MinValue;

                foreach (DMTTriangleBlock block in _blocks)
                {
                    foreach (Point vertex in block.TriangleVertices)
                    {
                        if (vertex.X < minX)
                        {
                            minX = vertex.X;
                        }
                        if (vertex.X > maxX)
                        {
                            maxX = vertex.X;
                        }

                        if (vertex.Y < minY)
                        {
                            minY = vertex.Y;
                        }
                        if (vertex.Y > maxY)
                        {
                            maxY = vertex.Y;
                        }

                        if (vertex.Z < minZ)
                        {
                            minZ = vertex.Z;
                        }
                        if (vertex.Z > maxZ)
                        {
                            maxZ = vertex.Z;
                        }
                    }
                }

                return new BoundingBox(minX, maxX, minY, maxY, minZ, maxZ);
            }
        }

        /// <summary>
        /// Total number of vertices (<see cref="DMTVertex"/>) that make up the model.
        /// </summary>
        public uint TotalNoOfVertices
        {
            get
            {
                int total = 0;

                foreach (DMTTriangleBlock block in _blocks)
                {
                    total += block.NoOfVertices;
                }

                return (uint) total;
            }
        }

        /// <summary>
        /// Total number of Triangles (<see cref="DMTTriangleBlock"/>) that make up the model.
        /// </summary>
        public uint TotalNoOfTriangles
        {
            get
            {
                int total = 0;

                foreach (DMTTriangleBlock block in _blocks)
                {
                    total += block.NoOfTriangles;
                }

                return (uint) total;
            }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Adds a new Block to the list of blocks
        /// </summary>
        /// <param name="block">Block to be added.</param>
        public void AddTriangleBlock(DMTTriangleBlock block)
        {
            _blocks.Add(block);
        }

        /// <summary>
        /// Returns the model points as a PointCloud.
        /// </summary>
        /// <returns>PointCloud object containing all model points.</returns>
        public PointCloud ToPointCloud()
        {
            PointCloud pointCloud = new PointCloud();

            foreach (DMTTriangleBlock block in _blocks)
            {
                foreach (Point vertex in block.TriangleVertices)
                {
                    pointCloud.Add(vertex);
                }
            }

            return pointCloud;
        }

        /// <summary>
        /// Creates a point cloud of all points that make up the "Top" surface of the
        /// DMT Model (i.e. those points that can be seen from above).
        /// </summary>
        /// <returns>PointCloud object containing the points.</returns>
        public PointCloud PointCloudFromTop()
        {
            PointCloud pointCloud = ToPointCloud();

            // Raise the point up above the highest Z value
            // Work out the highest Z point
            double zMax = pointCloud.BoundingBox.MaxZ.Value;

            // Give it a bit of clearance
            zMax += 1.0;

            // Give all points the same z value
            PointCloud raisedCloud = new PointCloud();
            foreach (Point cloudPoint in pointCloud)
            {
                Point raisedPoint = cloudPoint.Clone();
                raisedPoint.Z = zMax;
                raisedCloud.Add(cloudPoint);
            }

            // Now project them down onto the surface
            ZoneModel();
            List<Point> projectedPoints = null;
            projectedPoints = ProjectPoints(raisedCloud);

            // Remove the points that were not in the original top surface
            PointCloud finalCloud = new PointCloud();
            for (int index = 0; index <= projectedPoints.Count - 1; index++)
            {
                if (projectedPoints[index] == pointCloud[index])
                {
                    finalCloud.Add(pointCloud[index]);
                }
            }

            return finalCloud;
        }

        /// <summary>
        /// Creates a new model from all the points that make up the "Top" surface
        /// of the DMT Model (i.e. those triangles that can be seen in entirety from above).
        /// </summary>
        /// <returns>DMTModel object of the top surface.</returns>
        public DMTModel SurfaceFromTop()
        {
            //Get the points that make up the top surface
            PointCloud topSurfacePoints = PointCloudFromTop();

            DMTModel topSurfaceDMT = new DMTModel();

            foreach (DMTTriangleBlock block in _blocks)
            {
                DMTTriangleBlock newBlock = new DMTTriangleBlock();

                for (int intTriangleNo = 0; intTriangleNo <= TotalNoOfTriangles - 1; intTriangleNo++)
                {
                    Point vertex1 = null;
                    Point vertex2 = null;
                    Point vertex3 = null;
                    vertex1 = block.GetVertex1(intTriangleNo);
                    vertex2 = block.GetVertex2(intTriangleNo);
                    vertex3 = block.GetVertex3(intTriangleNo);

                    // See if this triangle is part of the top surface

                    if (topSurfacePoints.Contains(vertex1) && topSurfacePoints.Contains(vertex2) &&
                        topSurfacePoints.Contains(vertex3))
                    {
                        // Add the vertices to the list of vertices if they are not already in there
                        int vertex1Index = 0;
                        int vertex2Index = 0;
                        int vertex3Index = 0;
                        if (newBlock.TriangleVertices.Contains(vertex1))
                        {
                            vertex1Index = newBlock.TriangleVertices.IndexOf(vertex1);
                        }
                        else
                        {
                            vertex1Index = newBlock.TriangleVertices.Count;
                            newBlock.TriangleVertices.Add(vertex1);
                        }
                        if (newBlock.TriangleVertices.Contains(vertex2))
                        {
                            vertex2Index = newBlock.TriangleVertices.IndexOf(vertex2);
                        }
                        else
                        {
                            vertex2Index = newBlock.TriangleVertices.Count;
                            newBlock.TriangleVertices.Add(vertex2);
                        }
                        if (newBlock.TriangleVertices.Contains(vertex3))
                        {
                            vertex3Index = newBlock.TriangleVertices.IndexOf(vertex3);
                        }
                        else
                        {
                            vertex3Index = newBlock.TriangleVertices.Count;
                            newBlock.TriangleVertices.Add(vertex3);
                        }

                        // Create a new DMTTriangle that points to the Nodes
                        newBlock.TriangleFirstVertexIndices.Add(vertex1Index);
                        newBlock.TriangleSecondVertexIndices.Add(vertex2Index);
                        newBlock.TriangleThirdVertexIndices.Add(vertex3Index);
                    }
                }

                topSurfaceDMT.AddTriangleBlock(newBlock);
            }

            // With the new structure we don't need this
            //' Make sure each vertex knows which triangle it is a part of
            //topSurfaceDMT.RelinkNodesToTriangles()

            // Return the top surface
            return topSurfaceDMT;
        }

        #endregion

        #region "Boundaries"

        /// <summary>
        /// Returns a point cloud of the DMT boundary points.
        /// </summary>
        public PointCloud BoundaryNodes()
        {
            PointCloud vertices = new PointCloud();
            List<Polyline> boundaries = Boundaries();
            foreach (Polyline boundary in boundaries)
            {
                vertices.AddRange(boundary);
            }

            return vertices;
        }

        /// <summary>
        /// Returns a list of Polylines representing the edges of the DMT. Note that if a STL is provided the number of boundaries will equal the number of triangles.
        /// </summary>
        public List<Polyline> Boundaries()
        {
            // Initialise the list of Boundaries
            List<Polyline> boundaryPolylines = new List<Polyline>();

            //Get edges
            List<Edge> edges = new List<Edge>();
            foreach (DMTTriangleBlock block in _blocks)
            {
                // This is the list of triangle edges that make up the boundary
                for (int i = 0; i <= block.NoOfTriangles - 1; i++)
                {
                    //Edge V1 V2
                    edges.Add(new Edge(block.TriangleFirstVertexIndices[i], block.TriangleSecondVertexIndices[i], i));

                    //Edge V2 V3
                    edges.Add(new Edge(block.TriangleSecondVertexIndices[i], block.TriangleThirdVertexIndices[i], i));

                    //Edge V1 V3
                    edges.Add(new Edge(block.TriangleFirstVertexIndices[i], block.TriangleThirdVertexIndices[i], i));
                }

                //Find Border Edges
                List<Edge> borderEdges = new List<Edge>(edges);
                for (int i = borderEdges.Count - 1; i >= 0; i += -1)
                {
                    for (int j = borderEdges.Count - 1; j >= 0; j += -1)
                    {
                        if (borderEdges[i].TriangleIndex != borderEdges[j].TriangleIndex)
                        {
                            //Share Edge
                            if ((borderEdges[i].Point1Index == borderEdges[j].Point1Index) &
                                (borderEdges[i].Point2Index == borderEdges[j].Point2Index))
                            {
                                borderEdges.RemoveAt(i);
                                borderEdges.RemoveAt(j);
                                i = i - 1;
                                break;
                            }

                            if ((borderEdges[i].Point1Index == borderEdges[j].Point2Index) &
                                (borderEdges[i].Point2Index == borderEdges[j].Point1Index))
                            {
                                borderEdges.RemoveAt(i);
                                borderEdges.RemoveAt(j);
                                i = i - 1;
                                break;
                            }
                        }
                    }
                }

                // Having found all the edges in the block we now need to make the Polyline
                while (borderEdges.Count > 0)
                {
                    // Extract a boundary
                    Polyline boundary = NextBoundary(ref borderEdges, block);
                    if (boundary != null)
                    {
                        // Add it to the list
                        boundaryPolylines.Add(boundary);
                    }
                    else
                    {
                        // Something went wrong so exit while
                        break;
                    }
                }
            }

            return boundaryPolylines;
        }

        /// <summary>
        /// This operation extracts a boundary from the list of edges
        /// </summary>
        /// <param name="edges">The list of edges.  As edges are used they are removed from this list</param>
        /// <param name="block">The block from which to extract the eges</param>
        /// <returns>A polyline representing a boundary</returns>
        private Polyline NextBoundary(ref List<Edge> edges, DMTTriangleBlock block)
        {
            // Initialise the next boundary
            Polyline boundary = new Polyline();

            // Return if the list is empty
            if (edges.Count == 0)
            {
                return null;
            }

            // Get the first edge
            Edge firstEdge = edges[0];

            // And remove it from the list
            edges.Remove(firstEdge);

            // Now get the first vertex and the next vertex
            Point firstVertex = block.TriangleVertices[firstEdge.Point1Index];
            boundary.Add(firstVertex);
            int nextNode = firstEdge.Point2Index;
            boundary.Add(block.TriangleVertices[firstEdge.Point2Index]);

            // foundNextEdge will handle cases where the boundary is open
            bool foundNextEdge = true;

            // Loop until we hit the firstNode again or run out of boundary
            while ((firstVertex != nextNode) & foundNextEdge)
            {
                foundNextEdge = false;
                Edge egdeToRemove = null;

                // Find the vertex that nextNode is attached to
                foreach (Edge edge in edges)
                {
                    // See if the edge contains the next vertex
                    if (edge.Point1Index == nextNode)
                    {
                        nextNode = edge.Point2Index;
                    }
                    else if (edge.Point2Index == nextNode)
                    {
                        nextNode = edge.Point1Index;
                    }
                    else
                    {
                        // No so look at the next edge
                        continue;
                    }

                    // Add the next vertex
                    boundary.Add(block.TriangleVertices[nextNode]);
                    egdeToRemove = edge;
                    foundNextEdge = true;
                    break;
                }

                // Remove the last edge
                if (egdeToRemove != null)
                {
                    edges.Remove(egdeToRemove);
                }
            }

            // Return the boundary
            return boundary;
        }

        /// <summary>
        /// This Class holds the index of two vertices that make up an edge where an ege is a part of a
        /// boundary
        /// </summary>
        private class Edge
        {
            /// <summary>
            /// This is the index of the first point in the edge
            /// </summary>
            private int _point1Index;

            /// <summary>
            /// This is the index of the second point in the edge
            /// </summary>
            private int _point2Index;

            /// <summary>
            /// This is the triangle index for the edge
            /// </summary>
            private int _triangleIndex;

            /// <summary>
            /// Constructor initialises the edge
            /// </summary>
            /// <param name="point1Index">The index of the first point</param>
            /// <param name="point2Index">The index of the second point</param>
            /// <param name="triangleIndex">The triangle index for the edge</param>
            public Edge(int point1Index, int point2Index, int triangleIndex)
            {
                _point1Index = point1Index;
                _point2Index = point2Index;
                _triangleIndex = triangleIndex;
            }

            /// <summary>
            /// Gets the first point index
            /// </summary>
            /// <returns>The first point index </returns>
            public int Point1Index
            {
                get { return _point1Index; }
            }

            /// <summary>
            /// Gets the second point index
            /// </summary>
            /// <returns>The second point index</returns>
            public int Point2Index
            {
                get { return _point2Index; }
            }

            /// <summary>
            /// Gets the triangle index for the edge
            /// </summary>
            /// <returns>The triangle index for the edge</returns>
            public int TriangleIndex
            {
                get { return _triangleIndex; }
            }
        }

        #endregion

        #region "Model Zoning"

        /// <summary>
        /// Disects the model into a number of zones.
        /// The zones are roughly made up of the number of triangles specified by the intZoneDensity
        /// </summary>
        private void ZoneModel()
        {
            // Split the model up into boxes along the z axis cut through x and y axis
            // This improves performance when projecting a point down the z axis

            // Math.Floor just gets the integer portion.  The +1 ensures there is always at least 1 zone

            int zoneDensity = _zoneDensity;

            _numberOfZones = (int) (TotalNoOfTriangles / zoneDensity) + 1;

            // Work out the size of the zones in the x and y directions
            double minX = 0;
            double minY = 0;

            double xZoneLength = 0;
            double yZoneLength = 0;

            int numXZones = 0;
            int numYZones = 0;

            BoundingBox theBoundingBox = BoundingBox;
            var _with1 = theBoundingBox;
            xZoneLength = _with1.XSize / Math.Sqrt(_numberOfZones);
            yZoneLength = _with1.YSize / Math.Sqrt(_numberOfZones);

            numXZones = (int) (_with1.XSize / xZoneLength) + 1;
            numYZones = (int) (_with1.YSize / yZoneLength) + 1;

            minX = _with1.MinX;
            minY = _with1.MinY;

            // The zones arraylist will be an array of rows and columns of zones
            // They will be accessed by row, column (e.g. CType(lstZones(intRow), ArrayList)(intCol))
            _zones = new List<List<DMTTriangleZone>>(numXZones);

            for (int i = 0; i <= numXZones - 1; i++)
            {
                _zones.Add(new List<DMTTriangleZone>(numYZones));
                for (int j = 0; j <= numYZones - 1; j++)
                {
                    _zones[i].Add(new DMTTriangleZone());
                }
            }

            // Now need to loop through all the triangles and work out which zone(s) they are in.  Where a triangle
            // is in more than one zone an entry will be added to each zone it falls into

            int blockIndex = 0;
            foreach (DMTTriangleBlock block in _blocks)
            {
                int triangleIndex = 0;

                //For Each triangle As DMTTriangle In block.Triangles
                for (int intCounter = 0; intCounter <= TotalNoOfTriangles - 1; intCounter++)
                {
                    // Work out which row, col we start in and which row, col we finish in
                    Point vertex1 = block.GetVertex1(intCounter);
                    int vertex1Row = 0;
                    int vertex1Col = 0;
                    vertex1Row = (int) ((vertex1.X - minX) / xZoneLength);
                    vertex1Col = (int) ((vertex1.Y - minY) / yZoneLength);

                    Point vertex2 = block.GetVertex2(intCounter);
                    int vertex2Row = 0;
                    int vertex2Col = 0;
                    vertex2Row = (int) ((vertex2.X - minX) / xZoneLength);
                    vertex2Col = (int) ((vertex2.Y - minY) / yZoneLength);

                    Point vertex3 = block.GetVertex3(intCounter);
                    int vertex3Row = 0;
                    int vertex3Col = 0;
                    vertex3Row = (int) ((vertex3.X - minX) / xZoneLength);
                    vertex3Col = (int) ((vertex3.Y - minY) / yZoneLength);

                    int minRow = 0;
                    int maxRow = 0;
                    minRow = Minimum(vertex1Row, vertex2Row, vertex3Row);
                    maxRow = Maximum(vertex1Row, vertex2Row, vertex3Row);

                    int minCol = 0;
                    int maxCol = 0;
                    minCol = Minimum(vertex1Col, vertex2Col, vertex3Col);
                    maxCol = Maximum(vertex1Col, vertex2Col, vertex3Col);

                    // Add the triangle to every row, col zone it falls into
                    for (int rowIndex = minRow; rowIndex <= maxRow; rowIndex++)
                    {
                        for (int colIndex = minCol; colIndex <= maxCol; colIndex++)
                        {
                            _zones[rowIndex][colIndex].AddTriangle(new DMTTriangleZoneEntry(blockIndex, triangleIndex));
                        }
                    }
                    triangleIndex += 1;
                }
                blockIndex += 1;
            }
        }

        /// <summary>
        /// Private operation that returns the minimum value in a given array of Doubles
        /// </summary>
        private double Minimum(params double[] values)
        {
            double result = double.MaxValue;

            for (int i = 0; i <= values.Length - 1; i++)
            {
                result = Math.Min(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Private operation that returns the minimum value in a given array of int
        /// </summary>
        private int Minimum(params int[] values)
        {
            int result = int.MaxValue;

            for (int i = 0; i <= values.Length - 1; i++)
            {
                result = Math.Min(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Private operation that returns the maximum value in a given array of Doubles
        /// </summary>
        private double Maximum(params double[] values)
        {
            double result = double.MinValue;

            for (int i = 0; i <= values.Length - 1; i++)
            {
                result = Math.Max(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Private operation that returns the maximum value in a given array of min
        /// </summary>
        private int Maximum(params int[] values)
        {
            int result = int.MinValue;

            for (int i = 0; i <= values.Length - 1; i++)
            {
                result = Math.Max(result, values[i]);
            }

            return result;
        }

        #endregion

        #region "Point Projection"

        /// <summary>
        /// Projects a point onto the model in the Z axis and returns the nearest point to it.
        /// </summary>
        /// <param name="objPoint">Point to project.</param>
        /// <param name="blnProjectDownOnly">If True, points above the point of projection are ignored.</param>
        public Point ProjectPoint(Point objPoint, bool blnProjectDownOnly = false)
        {
            try
            {
                List<Point> points = new List<Point>();
                points.Add(objPoint);

                return ProjectPoints(points, blnProjectDownOnly)[0];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Project n point onto the model along the n projectionVector.
        /// </summary>
        /// <param name="pointsToProject">List of points to project.</param>
        /// <param name="projectionVectors">List of projection vectors.</param>
        /// <returns>The list of projected points and the direction of the projection.
        /// 1 if projection was along projection vector direction, -1 if it was opposite to the projection vector direction.</returns>
        public List<Tuple<Point, double>> ProjectPoints(List<Point> pointsToProject, List<Vector> projectionVectors)
        {
            var projectedPoints = new List<Tuple<Point,double>>();
            for (int i = 0; i < pointsToProject.Count; i++)
            {
                // We don't use to work with more than one triangle block
                double nearestDistance = Double.MaxValue;
                double nearestSignedDistance = Double.MaxValue;
                Point nearestPoint = null;
                DMTTriangleBlock triangleBlock;
                try
                {
                    triangleBlock = _blocks.Single();
                }
                catch (Exception)
                {
                    throw new ArgumentException("ProjectPoints is expecting one triangle block in the DMTModel.");
                }
 
                for (int j = 0; j < TotalNoOfTriangles; j++)
                {
                    // Get triangle vertices
                    var triangleVertex1 = triangleBlock.GetVertex1(j);
                    var triangleVertex2 = triangleBlock.GetVertex2(j);
                    var triangleVertex3 = triangleBlock.GetVertex3(j);

                    var triangleNormal = Autodesk.Geometry.DMTTriangle.GetNormal(triangleVertex1,
                        triangleVertex2,
                        triangleVertex3);

                    // Project point into the plane defined by the triangle in both directions
                    var projectedPoint = pointsToProject[i].ProjectToPlane(projectionVectors[i], triangleVertex1, triangleNormal);

                    // Check projected point is inside the triangle
                    if (projectedPoint != null &&
                        projectedPoint.IsInsideTriangle(triangleVertex1, triangleVertex2, triangleVertex3))
                    {
                        // Get the closest projection because it may be more than one
                        double distance = pointsToProject[i].DistanceToPoint(projectedPoint);
                        if (distance < nearestDistance)
                        {
                            // It is the nearest so far
                            nearestDistance = distance;
                            nearestPoint = projectedPoint;
                        }
                    }
                }

                double sign = 0;
                if (nearestPoint != null)
                {
                    sign = Vector.DotProduct(projectionVectors[i], new Vector(pointsToProject[i], nearestPoint)) < 0 ? -1:1;
                }
                projectedPoints.Add(new Tuple<Point, double>(nearestPoint, sign));
            }

            return projectedPoints;
        }

        /// <summary>
        /// Project n point onto the model along the n projectionVector.
        /// </summary>
        /// <param name="pointsToProject">List of points to project.</param>
        /// <param name="projectionVectors">List of projection vectors.</param>
        /// <returns>The list of projected points and the direction of the projection.
        /// 1 if projection was along projection vector direction, -1 if it was opposite to the projection vector direction.</returns>
        public List<Tuple<Point,double>> ProjectPointsParallel(List<Point> pointsToProject, List<Vector> projectionVectors)
        {

            var unorderProjectedPoints = new Dictionary<int, Tuple<Point, double>>();
            var orderedProjectedPoints = new List<Tuple<Point,double>>();
            var lockUnorderProjectedPoints = new object();
            Parallel.For(0, pointsToProject.Count, i =>
            {
                // We don't use to work with more than one triangle block
                double nearestDistance = Double.MaxValue;
                Point nearestPoint = null;
                DMTTriangleBlock triangleBlock;
                try
                {
                    triangleBlock = _blocks.Single();
                }
                catch (Exception)
                {
                    throw new ArgumentException("ProjectPoints is expecting one triangle block in the DMTModel.");
                }

                for (int j = 0; j < TotalNoOfTriangles; j++)
                {
                    // Get triangle vertices
                    var triangleVertex1 = triangleBlock.GetVertex1(j);
                    var triangleVertex2 = triangleBlock.GetVertex2(j);
                    var triangleVertex3 = triangleBlock.GetVertex3(j);

                    var triangleNormal = Autodesk.Geometry.DMTTriangle.GetNormal(triangleVertex1,
                        triangleVertex2,
                        triangleVertex3);

                    // Project point into the plane defined by the triangle in both directions
                        var projectingVector = projectionVectors[i].Clone();
                        var projectedPoint = pointsToProject[i].ProjectToPlane(projectingVector, triangleVertex1, triangleNormal);

                        // Check projected point is inside the triangle
                        if (projectedPoint != null &&
                            projectedPoint.IsInsideTriangle(triangleVertex1, triangleVertex2, triangleVertex3))
                        {
                            // Get the closest projection because it may be more than one
                            double distance = pointsToProject[i].DistanceToPoint(projectedPoint);
                            if (distance < nearestDistance)
                            {
                                // It is the nearest so far
                                nearestDistance = distance;
                                nearestPoint = projectedPoint;
                            }
                        }
                }

                lock (lockUnorderProjectedPoints)
                {
                    if (nearestPoint != null)
                    {
                        double sign = Vector.DotProduct(projectionVectors[i], new Vector(pointsToProject[i], nearestPoint)) < 0 ? -1 : 1;
                        unorderProjectedPoints.Add(i, new Tuple<Point, double>(nearestPoint, sign));
                    }
                    else
                    {
                        unorderProjectedPoints.Add(i, new Tuple<Point, double>(nearestPoint, 0));

                    }

                }


            });

            for (int i = 0; i < pointsToProject.Count; i++)
            {
                orderedProjectedPoints.Add(unorderProjectedPoints[i]);
            }

            return orderedProjectedPoints;
        }

        /// <summary>
        /// Projects an array of points onto the model in the Z axis and returns the nearest point to each one.
        /// </summary>
        /// <param name="pointsToProject">List of the points to project.</param>
        /// <param name="projectDownOnly">If True, points above the point of projection are ignored.</param>
        public List<Point> ProjectPoints(List<Point> pointsToProject, bool projectDownOnly = false)
        {
            // Zone model here in case user forgets.
            ZoneModel();

            // Loop through all the points and for each one find the nearest point in the z axis
            List<Point> projectedPoints = new List<Point>();

            Vector projectionVector = new Vector(0.0, 0.0, -1.0);

            // Work out the size of the zones in the x and y directions
            double minX = 0;
            double minY = 0;

            double xZoneLength = 0;
            double yZoneLength = 0;

            int numXZones = 0;
            int numYZones = 0;

            BoundingBox modelBoundingBox = BoundingBox;
            var _with2 = modelBoundingBox;
            xZoneLength = _with2.XSize / Math.Sqrt(_numberOfZones);
            yZoneLength = _with2.YSize / Math.Sqrt(_numberOfZones);

            numXZones = (int) (_with2.XSize / xZoneLength) + 1;
            numYZones = (int) (_with2.YSize / yZoneLength) + 1;

            minX = _with2.MinX;
            minY = _with2.MinY;

            foreach (Point pointToProject in pointsToProject)
            {
                // First find which zone (row and column) the point is in

                int row = 0;
                int col = 0;

                row = (int) ((pointToProject.X - minX) / xZoneLength);
                col = (int) ((pointToProject.Y - minY) / yZoneLength);

                Point nearestPoint = null;

                if ((row >= 0) & (col >= 0) && row < _zones.Count && col < _zones[row].Count)
                {
                    // Get the zone
                    DMTTriangleZone zone = _zones[row][col];

                    // Now loop through all triangles in the zone
                    double nearest = double.MaxValue;

                    // Loop through all the triangles in the zone
                    foreach (DMTTriangleZoneEntry triangleEntry in zone.Triangles)
                    {
                        //' Get the triangle itself
                        //Dim triangle As DMTTriangle
                        //With triangleEntry
                        //    triangle = CType(_blocks(.Block), DMTTriangleBlock).Triangles(.Triangle)
                        //End With

                        // Check to see if the projection point intersects this triangle
                        Point projectedPoint = null;
                        projectedPoint = ProjectPointToTriangle(pointToProject,
                                                                projectionVector,
                                                                _blocks[triangleEntry.Block].GetVertex1(triangleEntry.Triangle),
                                                                _blocks[triangleEntry.Block].GetVertex2(triangleEntry.Triangle),
                                                                _blocks[triangleEntry.Block].GetVertex3(triangleEntry.Triangle));

                        // If it does then see if it is nearer than any point found so far
                        if (projectedPoint != null)
                        {
                            // Ignore if we are projecting down only and the point is above us
                            if (projectDownOnly && pointToProject.Z < projectedPoint.Z)
                            {
                                continue;
                            }
                            double distance = Math.Abs(Convert.ToDouble(pointToProject.Z - projectedPoint.Z));
                            if (distance < nearest)
                            {
                                // It is the nearest so far
                                nearest = distance;
                                nearestPoint = projectedPoint.Clone();
                            }
                        }
                    }
                }

                // Add the nearest point to the list of output points
                projectedPoints.Add(nearestPoint);
            }

            return projectedPoints;
        }

        /// <summary>
        /// This operation projects a point onto a triangle to see if it hits it
        /// </summary>
        /// <param name="pointToProject">This is the point to project</param>
        /// <param name="projectionVector">This is the vector along which to project</param>
        /// <param name="vertex1">This is the first vertex in the triangle</param>
        /// <param name="vertex2">This is the second vertex in the triangle</param>
        /// <param name="vertex3">This is the third vertex in the triangle</param>
        /// <returns>The point on the triangle where the projected point hits, or Nothing if it does not</returns>
        private Point ProjectPointToTriangle(
            Point pointToProject,
            Vector projectionVector,
            Point vertex1,
            Point vertex2,
            Point vertex3)
        {
            // Firstly we can ignore any triangles that are blatantly not under the triangle
            if (pointToProject.X < vertex1.X && pointToProject.X < vertex2.X && pointToProject.X < vertex3.X)
            {
                return null;
            }
            if (pointToProject.X > vertex1.X && pointToProject.X > vertex2.X && pointToProject.X > vertex3.X)
            {
                return null;
            }
            if (pointToProject.Y < vertex1.Y && pointToProject.Y < vertex2.Y && pointToProject.Y < vertex3.Y)
            {
                return null;
            }
            if (pointToProject.Y > vertex1.Y && pointToProject.Y > vertex2.Y && pointToProject.Y > vertex3.Y)
            {
                return null;
            }

            // This sign will tell us where the triangle is in relation to the projection point.
            // As we look to see where the triangle is in relation to the point we should always move
            // in the same direction in the axis between the point and each vertex.  If one is different
            // then one of the vertices is outside the triangle
            int sign = 0;
            Vector toProjectionPoint = null;
            double dotProduct = 0;

            // Firstly get the edge from Vertex1 to Vertex2 and normalize it
            Vector edge1 = vertex2 - vertex1;
            edge1.Normalize();

            // Now find the cross product of the edge and the downward z projection of the point
            Vector edge1Norm = Vector.CrossProduct(edge1, projectionVector);

            // Now get the vector from Vertex2 to the projection point
            toProjectionPoint = pointToProject - vertex2;

            // Now calculate how far the edge is from the point where the projected point crosses the norm
            dotProduct = Vector.DotProduct(toProjectionPoint, edge1Norm);

            // Work out which direction in that projected norm the projected point crosses
            if (dotProduct < 0)
            {
                sign = -1;
            }
            if (dotProduct > 0)
            {
                sign = 1;
            }

            // Get the edge from Vertex2 to Vertex3
            Vector edge2 = vertex3 - vertex2;

            // Now find the cross product between Edge2 and the downward z projection of the point
            Vector edge2Norm = Vector.CrossProduct(edge2, projectionVector);

            // Now get the vector from Vertex3 to the projection point
            toProjectionPoint = pointToProject - vertex3;

            // Now calculate how far the edge is from the point where the projected point crosses the norm
            dotProduct = Vector.DotProduct(toProjectionPoint, edge2Norm);

            // Work out if the point is in the same direction in the axis as the last point
            if (sign != 0)
            {
                if ((dotProduct < 0) & (sign > 0))
                {
                    return null;
                }
                if ((dotProduct > 0) & (sign < 0))
                {
                    return null;
                }
            }
            else
            {
                if (dotProduct < 0)
                {
                    sign = -1;
                }
                if (dotProduct > 0)
                {
                    sign = 1;
                }
            }

            // Get the edge from Vertex3 to Vertex1
            Vector objEdge3 = vertex1 - vertex3;

            // Now find the cross product between Edge2 and the downward z projection of the point
            Vector objEdge3Norm = Vector.CrossProduct(objEdge3, projectionVector);

            // Now get the vector from Vertex1 to the projection point
            toProjectionPoint = pointToProject - vertex1;

            // Now calculate how far the edge is from the point where the projected point crosses the norm
            dotProduct = Vector.DotProduct(toProjectionPoint, objEdge3Norm);

            // Work out if the point is in the same direction in the axis as the last point
            if (sign != 0)
            {
                if ((dotProduct < 0) & (sign > 0))
                {
                    return null;
                }
                if ((dotProduct > 0) & (sign < 0))
                {
                    return null;
                }
            }
            else
            {
                if (dotProduct < 0)
                {
                    sign = -1;
                }
                if (dotProduct > 0)
                {
                    sign = 1;
                }
            }

            // So we will hit the triangle, just need to find where on the triangle, so project
            // the point on the plane of the triangle
            // Get the normal to the plane of the triangle
            Vector normal = Vector.CrossProduct(edge1, edge2);
            normal.Normalize();

            return pointToProject.ProjectToPlane(projectionVector, vertex1, normal);
        }

        #endregion

        #region "Clone Operation"

        /// <summary>
        /// Returns a clone of the current Model.
        /// </summary>
        public DMTModel Clone()
        {
            DMTModel cloneModel = new DMTModel();
            foreach (DMTTriangleBlock block in TriangleBlocks)
            {
                cloneModel.TriangleBlocks.Add(block.Clone());
            }
            return cloneModel;
        }

        #endregion
    }
}