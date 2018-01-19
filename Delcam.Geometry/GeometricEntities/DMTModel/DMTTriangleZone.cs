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

namespace Autodesk.Geometry
{
    /// <summary>
    /// Encapsulates the list of triangles that constitute a zone of the model. Zoning a model increases
    /// performance when projecting a point onto the model in the Z axis.
    /// </summary>
    [Serializable]
    public class DMTTriangleZone
    {
        #region "Fields"

        /// <summary>
        /// This is the list of triangles that are in the zone.
        /// </summary>
        private List<DMTTriangleZoneEntry> _triangles;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructs an empty DMTTriangleZone.
        /// </summary>
        public DMTTriangleZone()
        {
            _triangles = new List<DMTTriangleZoneEntry>();
        }

        /// <summary>
        /// Constructs a DMTTriangleZone and populates it with the passed list.
        /// </summary>
        /// <param name="triangles">List of triangles with which to populate the zone.</param>
        public DMTTriangleZone(List<DMTTriangleZoneEntry> triangles)
        {
            _triangles = triangles;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// List of triangles that constitute the zone.
        /// </summary>
        public List<DMTTriangleZoneEntry> Triangles
        {
            get { return _triangles; }

            set { _triangles = value; }
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// Adds a triangle to the zone.
        /// </summary>
        /// <param name="triangle">Triangle to add.</param>
        public void AddTriangle(DMTTriangleZoneEntry triangle)
        {
            _triangles.Add(triangle);
        }

        #endregion
    }
}