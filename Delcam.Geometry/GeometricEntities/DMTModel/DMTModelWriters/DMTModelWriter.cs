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
    /// Writes a DMTModel model to a file.
    /// </summary>
    public class DMTModelWriter
    {
        #region Operations

        /// <summary>
        /// Write the contents of the model into the specified file. If the provided file is a DMT file, it will use the DMT file writer. If the provided file is a STL file, it will use a STL file writer.
        /// </summary>
        /// <param name="dmtModel">The DMTModel to write.</param>
        /// <param name="file">Destination file.</param>
        public static void WriteFile(DMTModel dmtModel, File file)
        {
            switch (file.Extension.ToUpper())
            {
                case "DMT":
                    WriteToDMTFile(dmtModel, file);
                    break;
                case "STL":
                    WriteToSTLFile(dmtModel, file);
                    break;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Writes the contents of the model to the specified DMT <paramref name="file"/>.
        /// </summary>
        /// <param name="dmtModel">The DMTModel to write.</param>
        /// <param name="file">Destination file.</param>
        /// <exception cref="DMTFileException">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// The model contains no blocks.<br></br>The model contains no vertices.<br></br>The model contains no triangles.
        /// </exception>
        private static void WriteToDMTFile(DMTModel dmtModel, File file)
        {
            var dmtFileWriter = new DMTFileWriter();
            dmtFileWriter.WriteFile(dmtModel, file);
        }

        /// <summary>
        /// Writes the contents of the model to the specified STL <paramref name="file"/>.
        /// </summary>
        /// <param name="dmtModel">The DMTModel to write.</param>
        /// <param name="file">Destination file.</param>
        private static void WriteToSTLFile(DMTModel dmtModel, File file)
        {
            var stlFileWriter = new BinaryStlFileWriter();
            stlFileWriter.WriteFile(dmtModel, file);
        }

        #endregion
    }
}