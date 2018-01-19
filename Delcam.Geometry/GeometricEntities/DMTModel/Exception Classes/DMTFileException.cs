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
    internal enum DMTFileError
    {
        NoTriangleBlocks,
        NoVertices,
        NoTriangles,
        UnsupportedFileFormat,
        FileDoesNotExist,
        BlockVersionDoesNotMatchFileVersion,
        FileAlreadyExists,
        UndefinedError
    }
}

namespace Autodesk.Geometry
{
    /// <summary>
    /// Defines the exception to be thrown on DMT file errors.
    /// </summary>
    public class DMTFileException : Exception
    {
        #region "Fields"

        private DMTFileError _error;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Sets the error.
        /// </summary>
        internal DMTFileException(DMTFileError error)
        {
            _error = error;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Textual representation of the error condition.
        /// </summary>
        public override string Message
        {
            get
            {
                switch (_error)
                {
                    case DMTFileError.NoTriangleBlocks:
                        return "No Triangle Blocks are defined";
                    case DMTFileError.NoVertices:
                        return "No Triangle Vertices are defined";
                    case DMTFileError.NoTriangles:
                        return "No Triangles are defined";
                    case DMTFileError.UnsupportedFileFormat:
                        return "Unsupported file format";
                    case DMTFileError.FileDoesNotExist:
                        return "File does not exist";
                    case DMTFileError.BlockVersionDoesNotMatchFileVersion:
                        return "Block Version does not match File Version";
                    case DMTFileError.FileAlreadyExists:
                        return "File already exists";
                    default:
                        return "Undefined Error";
                }
            }
        }

        #endregion
    }
}