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
    /// Encapsulates the indices of both block and triangle for the list of triangles that constitute a zone of the model.
    /// </summary>
    [Serializable]
    public class DMTTriangleZoneEntry
    {
        #region "Fields"

        /// <summary>
        /// This is the block that this entry is part of.
        /// </summary>
        private int _block;

        /// <summary>
        /// This is the triangle this entry is part of.
        /// </summary>
        private int _triangle;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Default Constructor. Initialises both block and triangle indices to invalid values.
        /// </summary>
        public DMTTriangleZoneEntry()
        {
            _block = -1;
            _triangle = -1;
        }

        /// <summary>
        /// Initialises indices of both block and triangle to the specified values.
        /// </summary>
        /// <param name="block">Block index.</param>
        /// <param name="triangle">Triangle index.</param>
        public DMTTriangleZoneEntry(int block, int triangle)
        {
            _block = block;
            _triangle = triangle;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Index of the block that contains the triangle.
        /// </summary>
        public int Block
        {
            get { return _block; }

            set { _block = value; }
        }

        /// <summary>
        /// Index of the triangle within the block.
        /// </summary>
        public int Triangle
        {
            get { return _triangle; }

            set { _triangle = value; }
        }

        #endregion
    }
}