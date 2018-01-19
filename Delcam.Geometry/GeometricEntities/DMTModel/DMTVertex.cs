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
    /// Encapsulates DMT mesh vertices. A vertex has a position, a normal and 'knows' what triangles it is a part of.
    /// </summary>
    [Serializable]
    public class DMTVertex : IEquatable<DMTVertex>
    {
        #region "Fields"

        /// <summary>
        /// This is the position of the vertex.
        /// </summary>
        private Point _position;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initialises the vertex position to the specified values.
        /// </summary>
        /// <param name="x">Position in X.</param>
        /// <param name="y">Position in Y.</param>
        /// <param name="z">Position in Z.</param>
        public DMTVertex(MM x, MM y, MM z)
        {
            _position = new Point(x, y, z);
        }

        /// <summary>
        /// Initialises the vertex position and the IJK components of the normal to the values specified.
        /// </summary>
        /// <param name="x">Vertex position in X.</param>
        /// <param name="y">Vertex position in Y.</param>
        /// <param name="z">Vertex position in Z.</param>
        /// <param name="i">I component of the normal vector.</param>
        /// <param name="j">J component of the normal vector.</param>
        /// <param name="k">K component of the normal vector.</param>
        public DMTVertex(MM x, MM y, MM z, MM i, MM j, MM k)
        {
            _position = new Point(x, y, z);
        }

        /// <summary>
        /// Initialises the vertex position to the specified value.
        /// </summary>
        /// <param name="position">Captures vertex position in three axes.</param>
        public DMTVertex(Point position)
        {
            _position = position;
        }

        /// <summary>
        /// Initialises vertex position and normal to the specified values.
        /// </summary>
        /// <param name="position">Captures vertex position in three axes.</param>
        /// <param name="normal">Vector normal to the vertex.</param>
        public DMTVertex(Point position, Vector normal)
        {
            _position = position;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Vertex position.
        /// </summary>
        public Point Position
        {
            get { return _position; }

            set { _position = value; }
        }

        #endregion

        #region "Clone Operation"

        /// <summary>
        /// Returns a clone of this vertex.
        /// </summary>
        public DMTVertex Clone()
        {
            DMTVertex cloneNode = new DMTVertex(Position.Clone());
            return cloneNode;
        }

        #endregion

        public bool Equals(DMTVertex other)
        {
            return Position == other.Position;
        }
    }
}