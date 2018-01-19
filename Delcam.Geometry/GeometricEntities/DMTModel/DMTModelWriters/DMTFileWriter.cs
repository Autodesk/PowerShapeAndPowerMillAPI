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
using System.Reflection;
using Autodesk.FileSystem;
using Microsoft.VisualBasic;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Writes a DMT file.
    /// </summary>
    internal class DMTFileWriter
    {
        /// <summary>
        /// Writes the model to a DMT file.
        /// </summary>
        /// <param name="model">The DMTModel to write.</param>
        /// <param name="file">The file to write to.</param>
        public void WriteFile(DMTModel model, File file)
        {
            BinaryFileWriter binaryWriter = null;

            try
            {
                // If there are no blocks then return
                if (model.TriangleBlocks.Count == 0)
                {
                    throw new DMTFileException(DMTFileError.NoTriangleBlocks);
                }

                // If there are no vertices then return
                if (model.TotalNoOfVertices == 0)
                {
                    throw new DMTFileException(DMTFileError.NoVertices);
                }

                // If there are no triangles then return
                if (model.TotalNoOfTriangles == 0)
                {
                    throw new DMTFileException(DMTFileError.NoTriangles);
                }

                // Delete the file
                file.Delete();

                // Create the writer
                binaryWriter = new BinaryFileWriter(file);

                // Write the header
                string header = null;
                header = "DMT Triangles saved by Automation Interface v" +
                         Assembly.GetExecutingAssembly().GetName().Version +
                         " in MM " +
                         DateTime.Now.ToShortDateString();
                if (header.Length > 255)
                {
                    header = header.Substring(0, 255);
                }
                header += Strings.Chr(0);

                binaryWriter.WriteString(header);

                // Write version
                ushort version = 1000;
                binaryWriter.WriteUShort(version);

                // Write file flags (Bit 1 = 0 (doubles), Bit 2 = 1 (MMs))
                uint fileFlags = 2;
                binaryWriter.WriteUInteger(fileFlags);

                // Write total block count
                uint totalNoOfBlocks = 0;
                totalNoOfBlocks = (uint) model.TriangleBlocks.Count;
                binaryWriter.WriteUInteger(totalNoOfBlocks);

                // Write total vertex count
                uint totalNoOfTriangleVertices = 0;
                totalNoOfTriangleVertices = model.TotalNoOfVertices;
                binaryWriter.WriteUInteger(totalNoOfTriangleVertices);

                // Write total triangle count
                uint totalNoOfTriangles = 0;
                var triangleIndex = 0;
                Vector normal = null;
                totalNoOfTriangles = model.TotalNoOfTriangles;
                binaryWriter.WriteUInteger(totalNoOfTriangles);

                foreach (var block in model.TriangleBlocks)
                {
                    binaryWriter.WriteUInteger(block.Flags);
                    binaryWriter.WriteUInteger((uint) block.NoOfVertices);
                    binaryWriter.WriteUInteger((uint) block.NoOfTriangles);

                    for (var vertexNo = 0; vertexNo <= block.TriangleVertices.Count - 1; vertexNo++)
                    {
                        binaryWriter.WriteDouble(block.TriangleVertices.ElementAt(vertexNo).X);
                        binaryWriter.WriteDouble(block.TriangleVertices.ElementAt(vertexNo).Y);
                        binaryWriter.WriteDouble(block.TriangleVertices.ElementAt(vertexNo).Z);

                        if (block.DoVerticesHaveNormals)
                        {
                            binaryWriter.WriteDouble(block.VertexNormals.ElementAt(vertexNo).I.Value);
                            binaryWriter.WriteDouble(block.VertexNormals.ElementAt(vertexNo).J.Value);
                            binaryWriter.WriteDouble(block.VertexNormals.ElementAt(vertexNo).K.Value);
                        }
                    }

                    var use32bitPointers = false;
                    use32bitPointers = block.TriangleVertices.Count > ushort.MaxValue;

                    for (var triangleNo = 0; triangleNo <= block.NoOfTriangles - 1; triangleNo++)
                    {
                        if (use32bitPointers)
                        {
                            binaryWriter.WriteInteger(block.TriangleFirstVertexIndices.ElementAt(triangleNo));
                            binaryWriter.WriteInteger(block.TriangleSecondVertexIndices.ElementAt(triangleNo));
                            binaryWriter.WriteInteger(block.TriangleThirdVertexIndices.ElementAt(triangleNo));
                        }
                        else
                        {
                            binaryWriter.WriteUShort((ushort) block.TriangleFirstVertexIndices.ElementAt(triangleNo));
                            binaryWriter.WriteUShort((ushort) block.TriangleSecondVertexIndices.ElementAt(triangleNo));
                            binaryWriter.WriteUShort((ushort) block.TriangleThirdVertexIndices.ElementAt(triangleNo));
                        }
                    }

                    binaryWriter.WriteUShort(version);
                }

                // All done, close the writer
                binaryWriter.Close();
            }
            finally
            {
                if (binaryWriter != null)
                {
                    binaryWriter.Close();
                }
            }
        }
    }
}