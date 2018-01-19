// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using Autodesk.FileSystem;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Reads a DMT file.
    /// </summary>
    internal class DMTFileReader
    {
        #region Operations

        /// <summary>
        /// Reads a DMT file filtered by a provided filter.
        /// </summary>
        /// <param name="file">The mesh file.</param>
        /// <param name="filter">The provided filtered.</param>
        /// <returns>The DMTModel that obeys to the filter condition.</returns>
        public List<DMTModel> ReadFile(File file, IDMTModelFilter filter)
        {
            var blocksIn = new List<DMTTriangleBlock>();
            var blocksOut = new List<DMTTriangleBlock>();
            var binaryReader = new BinaryFileReader(file);
            try
            {
                if (file.Exists == false)
                {
                    throw new DMTFileException(DMTFileError.FileDoesNotExist);
                }

                // Read the file header. It is terminated with a 0 (null) and or has a 
                // maximum of 256 bytes
                string header = null;
                header = binaryReader.ReadStringUntil(0, 256);

                // Read the file version number. Should be 1000 as this is the only format currently supported
                var version = binaryReader.ReadUShort();
                if (version != 1000)
                {
                    //Close the reader and return fail
                    binaryReader.Close();
                    throw new DMTFileException(DMTFileError.UnsupportedFileFormat);
                }

                // Read the file flags
                // The bits of this integer are used as flags to detail file specific features. 
                //   1.  If this bit is set then the vertex data are stored as floats, else doubles. 
                //   2.  If this bit is set then the file contains triangles which are known to have been written in units of MMs.
                //       If the flag is unset then the units used are unknown. This change was introduced by api#25 and dicc34143 
                //
                //   New files (Since TVD release 2000111) MUST be written in MM. The code in dmkdmt always sets the flag and 
                //   the write_node() API ensures that you know about the requirement.
                uint fileFlags = 0;
                fileFlags = binaryReader.ReadUInteger();
                var pointsAreFloats = (fileFlags & 1) == 1;

                // Read the number of triangle blocks in the file. Should not be zero
                uint noOfBlocks = 0;
                noOfBlocks = binaryReader.ReadUInteger();
                if (noOfBlocks == 0)
                {
                    binaryReader.Close();
                    throw new DMTFileException(DMTFileError.NoTriangleBlocks);
                }

                // Read the number of vertices in the file. Should not be zero
                uint totalTriangleVertices = 0;
                totalTriangleVertices = binaryReader.ReadUInteger();
                if (totalTriangleVertices == 0)
                {
                    binaryReader.Close();
                    throw new DMTFileException(DMTFileError.NoVertices);
                }

                // Read the number of triangles in the file. Should not be zero
                uint totalTriangles = 0;
                totalTriangles = binaryReader.ReadUInteger();
                if (totalTriangles == 0)
                {
                    binaryReader.Close();
                    throw new DMTFileException(DMTFileError.NoTriangles);
                }

                // Read the blocks
                for (var blockNo = 0; blockNo <= noOfBlocks - 1; blockNo++)
                {
                    var blockIn = new DMTTriangleBlock();
                    var blockOut = new DMTTriangleBlock();
                    var vertices = new List<Point>();
                    var vertexNormals = new List<Vector>();
                    var oldIndexToNewIndexMapBlockIn = new Dictionary<int, int>();
                    var oldIndexToNewIndexMapBlockOut = new Dictionary<int, int>();

                    // Read the block flags
                    uint blockFlags = 0;
                    blockFlags = binaryReader.ReadUInteger();
                    var verticesHaveNormals = false;
                    verticesHaveNormals = (blockFlags & 1) == 1;
                    blockIn.DoVerticesHaveNormals = verticesHaveNormals;
                    blockOut.DoVerticesHaveNormals = verticesHaveNormals;

                    uint noOfTriangleVertices = 0;
                    noOfTriangleVertices = binaryReader.ReadUInteger();
                    uint noOfTriangles = 0;
                    noOfTriangles = binaryReader.ReadUInteger();

                    // Read the vertices
                    var x = default(MM);
                    var y = default(MM);
                    var z = default(MM);
                    var nx = default(MM);
                    var ny = default(MM);
                    var nz = default(MM);

                    for (var intNodeNo = 0; intNodeNo <= noOfTriangleVertices - 1; intNodeNo++)
                    {
                        // Read the XYZ values
                        if (pointsAreFloats)
                        {
                            x = binaryReader.ReadSingle();
                            y = binaryReader.ReadSingle();
                            z = binaryReader.ReadSingle();
                        }
                        else
                        {
                            x = binaryReader.ReadDouble();
                            y = binaryReader.ReadDouble();
                            z = binaryReader.ReadDouble();
                        }

                        // Continue reading
                        if (verticesHaveNormals)
                        {
                            if (pointsAreFloats)
                            {
                                nx = binaryReader.ReadSingle();
                                ny = binaryReader.ReadSingle();
                                nz = binaryReader.ReadSingle();
                            }
                            else
                            {
                                nx = binaryReader.ReadDouble();
                                ny = binaryReader.ReadDouble();
                                nz = binaryReader.ReadDouble();
                            }
                        }

                        // Store the vertex
                        vertices.Add(new Point(x, y, z));
                        if (verticesHaveNormals)
                        {
                            vertexNormals.Add(new Vector(nx, ny, nz));
                        }
                    }

                    // What size are the pointers?
                    // They will use 32 bit Unsigned Integers if 16 bit Unsigned Integer is not enough
                    var use32bitPointers = noOfTriangleVertices > ushort.MaxValue;

                    // Read the triangles
                    var vertex1Index = 0;
                    var vertex2Index = 0;
                    var vertex3Index = 0;

                    for (var triangleNo = 0; triangleNo <= noOfTriangles - 1; triangleNo++)
                    {
                        if (use32bitPointers)
                        {
                            vertex1Index = binaryReader.ReadInteger();
                            vertex2Index = binaryReader.ReadInteger();
                            vertex3Index = binaryReader.ReadInteger();
                        }
                        else
                        {
                            vertex1Index = binaryReader.ReadUShort();
                            vertex2Index = binaryReader.ReadUShort();
                            vertex3Index = binaryReader.ReadUShort();
                        }

                        if (filter.CanAddTriangle(vertices[vertex1Index], vertices[vertex2Index], vertices[vertex3Index]))
                        {
                            AddTriangle(oldIndexToNewIndexMapBlockIn,
                                        blockIn,
                                        verticesHaveNormals,
                                        vertices,
                                        vertexNormals,
                                        vertex1Index,
                                        vertex2Index,
                                        vertex3Index);
                        }
                        else
                        {
                            AddTriangle(oldIndexToNewIndexMapBlockOut,
                                        blockOut,
                                        verticesHaveNormals,
                                        vertices,
                                        vertexNormals,
                                        vertex1Index,
                                        vertex2Index,
                                        vertex3Index);
                        }
                    }

                    // Check that the version number is ok
                    if (binaryReader.ReadUShort() != version)
                    {
                        throw new DMTFileException(DMTFileError.BlockVersionDoesNotMatchFileVersion);
                    }

                    blocksIn.Add(blockIn);
                    blocksOut.Add(blockOut);
                }

                var modelWithinFilter = new DMTModel();
                var modelOutsideFilter = new DMTModel();
                modelWithinFilter.TriangleBlocks.AddRange(blocksIn);
                modelOutsideFilter.TriangleBlocks.AddRange(blocksOut);
                var result = new List<DMTModel>();
                result.Add(modelWithinFilter);
                result.Add(modelOutsideFilter);
                return result;
            }
            finally
            {
                // Close the binary reader
                if (binaryReader != null)
                {
                    binaryReader.Close();
                }
            }
        }

        #endregion

        #region Implementation

        private static void AddTriangle(
            Dictionary<int, int> oldIndexToNewIndexMap,
            DMTTriangleBlock block,
            bool verticesHaveNormals,
            List<Point> vertices,
            List<Vector> vertexNormals,
            int vertex1Index,
            int vertex2Index,
            int vertex3Index)
        {
            if (oldIndexToNewIndexMap.ContainsKey(vertex1Index))
            {
                block.TriangleFirstVertexIndices.Add(oldIndexToNewIndexMap[vertex1Index]);
            }
            else
            {
                block.TriangleVertices.Add(vertices[vertex1Index]);
                block.TriangleFirstVertexIndices.Add(block.TriangleVertices.Count - 1);
                oldIndexToNewIndexMap.Add(vertex1Index, block.TriangleVertices.Count - 1);

                if (verticesHaveNormals)
                {
                    block.VertexNormals.Add(vertexNormals[vertex1Index]);
                }
            }

            if (oldIndexToNewIndexMap.ContainsKey(vertex2Index))
            {
                block.TriangleSecondVertexIndices.Add(oldIndexToNewIndexMap[vertex2Index]);
            }
            else
            {
                block.TriangleVertices.Add(vertices[vertex2Index]);
                block.TriangleSecondVertexIndices.Add(block.TriangleVertices.Count - 1);
                oldIndexToNewIndexMap.Add(vertex2Index, block.TriangleVertices.Count - 1);

                if (verticesHaveNormals)
                {
                    block.VertexNormals.Add(vertexNormals[vertex2Index]);
                }
            }

            if (oldIndexToNewIndexMap.ContainsKey(vertex3Index))
            {
                block.TriangleThirdVertexIndices.Add(oldIndexToNewIndexMap[vertex3Index]);
            }
            else
            {
                block.TriangleVertices.Add(vertices[vertex3Index]);
                block.TriangleThirdVertexIndices.Add(block.TriangleVertices.Count - 1);
                oldIndexToNewIndexMap.Add(vertex3Index, block.TriangleVertices.Count - 1);

                if (verticesHaveNormals)
                {
                    block.VertexNormals.Add(vertexNormals[vertex3Index]);
                }
            }
        }

        #endregion
    }
}