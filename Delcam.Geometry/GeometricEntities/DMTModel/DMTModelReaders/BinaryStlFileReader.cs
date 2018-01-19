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
    /// Reads a binary STL file.
    /// </summary>
    internal class BinaryStlFileReader
    {
        #region Operations

        /// <summary>
        /// Reads a binary STL file filtered by a provided filter.
        /// </summary>
        /// <param name="file">The mesh file.</param>
        /// <param name="filter">The provided filtered.</param>
        /// <returns>The DMTModel that obeys to the filter condition.</returns>
        public List<DMTModel> ReadFile(File file, IDMTModelFilter filter)
        {
            if (file.Exists == false)
            {
                throw new DMTFileException(DMTFileError.FileDoesNotExist);
            }

            var blocksIn = new List<DMTTriangleBlock>();
            var blocksOut = new List<DMTTriangleBlock>();
            var blockIn = new DMTTriangleBlock();
            var blockOut = new DMTTriangleBlock();
            var objReader = new BinaryFileReader(file);

            try
            {
                //Read first 80 characters (bytes) and ignore them
                objReader.ReadBytes(80);

                //Read the next 4 bytes to get an unsigned integer of number of triangles
                var intNoOfFacets = objReader.ReadUInteger();

                //Now keep reading until the end of the file
                var mapPointVertexIndexBlockIn = new Dictionary<Point, int>();
                var mapPointVertexIndexBlockOut = new Dictionary<Point, int>();
                var point1Index = -1;
                var point2Index = -1;
                var point3Index = -1;
                for (uint intCounter = 0; intCounter <= intNoOfFacets - 1; intCounter++)
                {
                    //Read 3 32bit floating point numbers - triangle normal
                    // We do not keep the normals in memory, they take too much space
                    objReader.ReadSingle();
                    objReader.ReadSingle();
                    objReader.ReadSingle();

                    //Read 3 32bit floating point numbers - vertex 1 X/Y/Z
                    var objPoint1 = new Point();
                    objPoint1.X = objReader.ReadSingle();
                    objPoint1.Y = objReader.ReadSingle();
                    objPoint1.Z = objReader.ReadSingle();

                    //Read 3 32bit floating point numbers - vertex 2 X/Y/Z
                    var objPoint2 = new Point();
                    objPoint2.X = objReader.ReadSingle();
                    objPoint2.Y = objReader.ReadSingle();
                    objPoint2.Z = objReader.ReadSingle();

                    //Read 3 32bit floating point numbers - vertex 3 X/Y/Z
                    var objPoint3 = new Point();
                    objPoint3.X = objReader.ReadSingle();
                    objPoint3.Y = objReader.ReadSingle();
                    objPoint3.Z = objReader.ReadSingle();

                    if (filter.CanAddTriangle(objPoint1, objPoint2, objPoint3))
                    {
                        AddTriangle(mapPointVertexIndexBlockIn, blockIn, objPoint1, objPoint2, objPoint3);
                    }
                    else
                    {
                        AddTriangle(mapPointVertexIndexBlockOut, blockOut, objPoint1, objPoint2, objPoint3);
                    }

                    //Read 16 bit number
                    objReader.ReadUInt16();
                }

                blockIn.DoVerticesHaveNormals = false;
                blockOut.DoVerticesHaveNormals = false;
                blocksIn.Add(blockIn);
                blocksOut.Add(blockOut);

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
                objReader.Close();
            }
        }

        #endregion

        #region Implementation

        private static void AddTriangle(
            Dictionary<Point, int> mapPointVertexIndex,
            DMTTriangleBlock block,
            Point objPoint1,
            Point objPoint2,
            Point objPoint3)
        {
            int point1Index;
            int point2Index;
            int point3Index;
            if (!mapPointVertexIndex.TryGetValue(objPoint1, out point1Index))
            {
                point1Index = mapPointVertexIndex.Count;
                mapPointVertexIndex.Add(objPoint1, point1Index);
                block.AddVertex(objPoint1);
            }

            if (!mapPointVertexIndex.TryGetValue(objPoint2, out point2Index))
            {
                point2Index = mapPointVertexIndex.Count;
                mapPointVertexIndex.Add(objPoint2, point2Index);
                block.AddVertex(objPoint2);
            }

            if (!mapPointVertexIndex.TryGetValue(objPoint3, out point3Index))
            {
                point3Index = mapPointVertexIndex.Count;
                mapPointVertexIndex.Add(objPoint3, point3Index);
                block.AddVertex(objPoint3);
            }

            block.AddTriangle(point1Index, point2Index, point3Index);
        }

        #endregion
    }
}