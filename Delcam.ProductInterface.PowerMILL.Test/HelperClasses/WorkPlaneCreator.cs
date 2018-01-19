// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autodesk.FileSystem;
using DG = Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILLTest.HelperClasses
{
    /// <summary>
    /// Helper class that creates workplans from test files
    /// </summary>
    public class WorkPlaneCreator
    {
        /// <summary>
        /// Creates a workplane based on information in a text file
        /// </summary>
        /// <param name="wpDataFile"></param>
        /// <returns></returns>
        public static DG.Workplane CreateWorkplaneFromFile(File wpDataFile)
        {
            List<string> wpLines = wpDataFile.ReadTextLines();

            string wpOrigin = wpLines[0].Substring(wpLines[0].IndexOf(":") + 1).Trim();
            string xVect = wpLines[1].Substring(wpLines[0].IndexOf(":") + 1).Trim();
            string yVect = wpLines[2].Substring(wpLines[0].IndexOf(":") + 1).Trim();
            string zVect = wpLines[3].Substring(wpLines[0].IndexOf(":") + 1).Trim();

            Regex r = new Regex(@"\s+"); //strip multiple spaces
            wpOrigin = r.Replace(wpOrigin, @" ");
            xVect = r.Replace(xVect, @" ");
            yVect = r.Replace(yVect, @" ");
            zVect = r.Replace(zVect, @" ");

            System.Globalization.CultureInfo lCurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            DG.Workplane dgWp = null;

            try
            {
                // Ensure always reading in English
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                dgWp = new DG.Workplane(new DG.Point(wpOrigin),
                                        new DG.Vector(xVect, ' '),
                                        new DG.Vector(yVect, ' '),
                                        new DG.Vector(zVect, ' '));
            }
            catch
            {
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = lCurrentCulture;
            }

            return dgWp;
        }

        /// <summary>
        /// Returns whether 2 workplanes are equal based on origin, x,y,z vector axis
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True/False</returns>
        public static bool AreWorkplanesEqual(DG.Workplane a, DG.Workplane b)
        {
            bool result = a.Origin == b.Origin && a.XAxis == b.XAxis && a.YAxis == b.YAxis &&
                          a.ZAxis == b.ZAxis;

            return result;
        }
    }
}