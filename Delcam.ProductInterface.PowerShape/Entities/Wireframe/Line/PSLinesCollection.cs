// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a collection of Lines in a Project
    /// </summary>
    public class PSLinesCollection : PSEntitiesCollection<PSLine>
    {
        #region " Fields "

        #endregion

        #region " Constructors "

        /// <summary>
        /// This constructor is required as collection inherits from EntitiesCollection
        /// </summary>
        internal PSLinesCollection(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifier used by PowerSHAPE when selecting a Line
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        internal override string Identifier
        {
            get { return "LINE"; }
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Creates a line between the specified points.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The new line.</returns>
        /// <remarks></remarks>
        public PSLine CreateLine(Geometry.Point startPoint, Geometry.Point endPoint)
        {
            var objLine = new PSLine(_powerSHAPE, startPoint, endPoint);
            Add(objLine);
            return objLine;
        }

        /// <summary>
        /// Creates lines between the specified points.
        /// A line will be created between each two consecutive points (e.g. Line between 1st and 2nd points, line between 2nd and 3rd points, etc.).
        /// </summary>
        /// <param name="points">The list of points used to create the lines.</param>
        /// <returns>The new lines.</returns>
        /// <remarks></remarks>
        public List<PSLine> CreateLines(Geometry.Point[] points)
        {
            List<PSLine> lines = new List<PSLine>();

            for (int index = 1; index <= points.Length - 1; index++)
            {
                var line = new PSLine(_powerSHAPE, points[index - 1], points[index]);
                lines.Add(line);
                Add(line);
            }

            return lines;
        }

        #endregion
    }
}