// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using Autodesk.FileSystem;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Writes a STL binary file.
    /// </summary>
    internal class BinaryStlFileWriter
    {
        /// <summary>
        /// Writes the model to a binary STL file.
        /// </summary>
        /// <param name="model">The DMTModel to write.</param>
        /// <param name="file">The file to write to.</param>
        public void WriteFile(DMTModel model, File file)
        {
            //Check that we are not going to inadvertantly overwrite a file
            file.Delete();

            BinaryFileWriter binaryWriter = null;

            try
            {
                binaryWriter = new BinaryFileWriter(file);

                //If there are no triangles then return
                if (model.TotalNoOfTriangles == 0)
                {
                    throw new DMTFileException(DMTFileError.NoTriangles);
                }

                // Write header
                for (var i = 0; i < 80; i++)
                {
                    byte b = 0;
                    binaryWriter.WriteByte(b);
                }

                // write number of triangles
                binaryWriter.WriteUInteger(model.TotalNoOfTriangles);

                Point vertex1 = null;
                Point vertex2 = null;
                Point vertex3 = null;
                Vector normal = null;
                foreach (var block in model.TriangleBlocks)
                {
                    for (var intCounter = 0; intCounter <= block.NoOfTriangles - 1; intCounter++)
                    {
                        // STL wants a triangle Normal
                        vertex1 = block.GetVertex1(intCounter);
                        vertex2 = block.GetVertex2(intCounter);
                        vertex3 = block.GetVertex3(intCounter);
                        normal = DMTTriangle.GetNormal(vertex1, vertex2, vertex3);
                        binaryWriter.WriteSingle((float) normal.I.Value);
                        binaryWriter.WriteSingle((float) normal.J.Value);
                        binaryWriter.WriteSingle((float) normal.K.Value);

                        // Write vertices
                        var x = (float) vertex1.X;
                        var y = (float) vertex1.Y;
                        var z = (float) vertex1.Z;
                        binaryWriter.WriteSingle(x);
                        binaryWriter.WriteSingle(y);
                        binaryWriter.WriteSingle(z);
                        x = (float) vertex2.X;
                        y = (float) vertex2.Y;
                        z = (float) vertex2.Z;
                        binaryWriter.WriteSingle(x);
                        binaryWriter.WriteSingle(y);
                        binaryWriter.WriteSingle(z);
                        x = (float) vertex3.X;
                        y = (float) vertex3.Y;
                        z = (float) vertex3.Z;
                        binaryWriter.WriteSingle(x);
                        binaryWriter.WriteSingle(y);
                        binaryWriter.WriteSingle(z);

                        // padding to 50 bytes
                        ushort code = 0;
                        binaryWriter.WriteUInt16(code);
                    }
                }
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