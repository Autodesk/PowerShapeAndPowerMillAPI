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
using System.Linq;
using Autodesk.FileSystem;
using Microsoft.VisualBasic;

namespace Autodesk.Geometry
{
    /// <summary>
    /// Reads an ASCII STL file.
    /// </summary>
    internal class AsciiStlFileReader
    {
        #region Operations

        /// <summary>
        /// Reads an ASCII STL file filtered by a provided filter.
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
            var vertices = new List<Point>();

            foreach (var strLine in file.ReadTextLines())
            {
                if (strLine.Trim().StartsWith("vertex "))
                {
                    //Add position
                    var strCoords = strLine.Trim().Split(' ');
                    var intCounter = 0;
                    double x = 0;
                    double y = 0;
                    double z = 0;
                    foreach (var strCoord in strCoords)
                    {
                        if (Information.IsNumeric(strCoord))
                        {
                            if (intCounter == 0)
                            {
                                x = Convert.ToDouble(strCoord);
                            }
                            else if (intCounter == 1)
                            {
                                y = Convert.ToDouble(strCoord);
                            }
                            else
                            {
                                z = Convert.ToDouble(strCoord);
                            }
                            intCounter += 1;
                        }
                    }

                    vertices.Add(new Point(x, y, z));
                }
                else if (strLine.Trim().StartsWith("endloop"))
                {
                    if (filter.CanAddTriangle(vertices.ElementAt(0), vertices.ElementAt(1), vertices.ElementAt(2)))
                    {
                        blockIn.AddTriangle(vertices.ElementAt(0), vertices.ElementAt(1), vertices.ElementAt(2));
                    }
                    else
                    {
                        blockOut.AddTriangle(vertices.ElementAt(0), vertices.ElementAt(1), vertices.ElementAt(2));
                    }

                    vertices.Clear();
                }
            }

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

        #endregion
    }
}