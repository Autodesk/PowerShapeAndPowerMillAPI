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
    /// Reads a mesh file.
    /// </summary>
    public class DMTModelReader
    {
        #region Operations

        /// <summary>
        /// Reads a mesh file.
        /// </summary>
        /// <param name="file">The mesh file.</param>
        /// <returns>The DMTModel that represents the mesh file.</returns>
        public static DMTModel ReadFile(File file)
        {
            return ReadFile(file, new DMTModelFilterByNone());
        }

        /// <summary>
        /// Reads a mesh file filtered by filter.
        /// </summary>
        /// <param name="file">The mesh file.</param>
        /// <param name="filter">The filter to filter by.</param>
        /// <returns>The DMTModel that represents the mesh that obeys to the filter condition.</returns>
        /// <exception cref="DMTFileException">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// Specified file does not exist.<br></br>File format is unsupported.<br></br>File contains no triangle blocks.<br></br>
        /// File contains no vertices.<br></br>File contains no triangles.<br></br>Block version does not match file version.
        /// </exception>
        public static DMTModel ReadFile(File file, IDMTModelFilter filter)
        {
            DMTModel modelOutsideFilter;
            return ReadFile(file, filter, out modelOutsideFilter);
        }

        /// <summary>
        /// Reads a mesh file filtered by filter.
        /// </summary>
        /// <param name="file">The mesh file.</param>
        /// <param name="filter">The filter to filter by.</param>
        /// <param name="excludedModel">The DMTModel that doesn't obey to the filter condition.</param>
        /// <returns>The DMTModel that obeys to the filter condition.</returns>
        /// <exception cref="DMTFileException">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// Specified file does not exist.<br></br>File format is unsupported.<br></br>File contains no triangle blocks.<br></br>
        /// File contains no vertices.<br></br>File contains no triangles.<br></br>Block version does not match file version.
        /// </exception>
        public static DMTModel ReadFile(File file, IDMTModelFilter filter, out DMTModel excludedModel)
        {
            var dmtModel = new DMTModel();
            AppendFile(dmtModel, file, filter, out excludedModel);
            return dmtModel;
        }

        /// <summary>
        /// Appends the contents of the specified DMT file to the model.
        /// </summary>
        /// <param name="dmtModel">The DMTModel to append to.</param>
        /// <param name="file">Path and filename of the DMT model.</param>
        /// <exception cref="DMTFileException">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// Specified file does not exist.<br></br>File format is unsupported.<br></br>File contains no triangle blocks.<br></br>
        /// File contains no vertices.<br></br>File contains no triangles.<br></br>Block version does not match file version.
        /// </exception>
        public static void AppendFile(DMTModel dmtModel, File file)
        {
            AppendFile(dmtModel, file, new DMTModelFilterByNone());
        }

        /// <summary>
        /// Appends the contents of the specified DMT file that obeys to a specified filter to the existing model.
        /// </summary>
        /// <param name="dmtModel">The DMTModel to append to.</param>
        /// <param name="file">Path and filename of the DMT model.</param>
        /// <param name="filter">The filterto filter by.</param>
        /// <exception cref="DMTFileException">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// Specified file does not exist.<br></br>File format is unsupported.<br></br>File contains no triangle blocks.<br></br>
        /// File contains no vertices.<br></br>File contains no triangles.<br></br>Block version does not match file version.
        /// </exception>
        public static void AppendFile(DMTModel dmtModel, File file, IDMTModelFilter filter)
        {
            DMTModel excludedModel;
            AppendFile(dmtModel, file, filter, out excludedModel);
        }

        /// <summary>
        /// Appends the contents of the specified DMT file that obeys to a specified filter to the existing model.
        /// </summary>
        /// <param name="dmtModel">The DMTModel to append to.</param>
        /// <param name="file">Path and filename of the DMT model.</param>
        /// <param name="filter">The filterto filter by.</param>
        /// <param name="excludedModel">The DMTModel that doesn't obey to the filter condition.</param>
        /// <exception cref="DMTFileException">
        /// Thrown for any of the following reasons:<br></br><br></br>
        /// Specified file does not exist.<br></br>File format is unsupported.<br></br>File contains no triangle blocks.<br></br>
        /// File contains no vertices.<br></br>File contains no triangles.<br></br>Block version does not match file version.
        /// </exception>
        public static void AppendFile(DMTModel dmtModel, File file, IDMTModelFilter filter, out DMTModel excludedModel)
        {
            excludedModel = new DMTModel();
            switch (file.Extension.ToUpper())
            {
                case "DMT":
                    AppendDMTFile(dmtModel, file, filter, out excludedModel);
                    break;
                case "STL":
                    AppendSTLFile(dmtModel, file, filter, out excludedModel);
                    break;
            }
        }

        #endregion

        #region Implementation

        private static void AppendDMTFile(
            DMTModel dmtModel,
            File fileToAppend,
            IDMTModelFilter filter,
            out DMTModel excludedDmtModel)
        {
            var dmtReader = new DMTFileReader();
            var models = dmtReader.ReadFile(fileToAppend, filter);
            var withinFilter = models[0];
            excludedDmtModel = models[1];

            dmtModel.TriangleBlocks.AddRange(withinFilter.TriangleBlocks);
        }

        /// <summary>
        /// Appends the contents of the specified STL <paramref name="file"/> to the model.
        /// </summary>
        /// <param name="file">Path and filename of the STL model.</param>
        private static void AppendSTLFile(DMTModel dmtModel, File file, IDMTModelFilter filter, out DMTModel excludedDmtModel)
        {
            var asciiStlReader = new AsciiStlFileReader();
            var binaryStlReader = new BinaryStlFileReader();
            var models = file.ReadFirstLine().StartsWith("solid")
                ? asciiStlReader.ReadFile(file, filter)
                : binaryStlReader.ReadFile(file, filter);

            var withinFilter = models[0];
            excludedDmtModel = models[1];

            dmtModel.TriangleBlocks.AddRange(withinFilter.TriangleBlocks);
        }

        #endregion
    }
}