// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Collections.Generic;
using Autodesk.FileSystem;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a Pattern object in PowerMILL
    /// </summary>
    public class PMPattern : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMPattern(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMPattern(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the visibility of the pattern in PowerMill.
        /// </summary>
        public bool IsVisible
        {
            set
            {
                if (value)
                {
                    PowerMill.DoCommand("DRAW PATTERN '" + Name + "'");
                }
                else
                {
                    PowerMill.DoCommand("UNDRAW PATTERN '" + Name + "'");
                }
            }
        }

        internal static string PATTERN_IDENTIFIER = "PATTERN";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return PATTERN_IDENTIFIER; }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes the pattern from the active project and from PowerMill.
        /// </summary>
        public override void Delete()
        {
            PowerMill.ActiveProject.Patterns.Remove(this);
        }

        /// <summary>
        /// Reverses the selected sections of the Pattern in PowerMill.
        /// </summary>
        public void ReverseSelected()
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' REVERSE");
        }

        /// <summary>
        /// Fits an arc to the selected sections of the Pattern.
        /// </summary>
        /// <param name="tolerance">The Arc fitting tolerance.</param>
        public void ArcFitSelected(double tolerance)
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' ARCFIT " + tolerance);
        }

        /// <summary>
        /// Splines the selected sections of the Pattern.
        /// </summary>
        /// <param name="dblTolerance">The spline fitting tolerance.</param>
        public void SplineSelected(double dblTolerance)
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SPLINE " + dblTolerance);
        }

        /// <summary>
        /// Polygonises the selected sections of the Pattern.
        /// </summary>
        /// <param name="dblTolerance">The Polygonisation tolerance.</param>
        public void PolygoniseSelected(double dblTolerance)
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SMASH " + dblTolerance);
        }

        /// <summary>
        /// Splits the selected sections of the Pattern.
        /// </summary>
        public void SplitSelected()
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SPLIT");
        }

        /// <summary>
        /// Closes the selected sections of the Pattern.
        /// </summary>
        public void CloseSelected()
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' CLOSE");
        }

        /// <summary>
        /// Merges the selected sections of the Pattern.
        /// </summary>
        public void MergeSelected()
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' MERGE");
        }

        /// <summary>
        /// Selects all sections of the Pattern.
        /// </summary>
        public void SelectAll()
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SELECT ALL");
        }

        /// <summary>
        /// Reverses all the sections of the Pattern.
        /// </summary>
        public void ReverseAll()
        {
            MergeAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' REVERSE");
        }

        /// <summary>
        /// Fits an Arc to all the sections of the Pattern.
        /// </summary>
        /// <param name="dblTolerance">The Arc fitting tolerance.</param>
        public void ArcFitAll(double dblTolerance)
        {
            MergeAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' ARCFIT " + dblTolerance);
        }

        /// <summary>
        /// Splines all the sections of the Pattern.
        /// </summary>
        /// <param name="dblTolerance">The Spline fitting tolerance.</param>
        public void SplineAll(double dblTolerance)
        {
            MergeAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SPLINE " + dblTolerance);
        }

        /// <summary>
        /// Polygonises all the sections of the Pattern.
        /// </summary>
        /// <param name="dblTolerance">The Polygonisation tolerance.</param>
        public void PolygoniseAll(double dblTolerance)
        {
            MergeAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SMASH " + dblTolerance);
        }

        /// <summary>
        /// Splits all the sections of the Pattern.
        /// </summary>
        public void SplitAll()
        {
            MergeAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' SPLIT");
        }

        /// <summary>
        /// Closes all the sections of the Pattern.
        /// </summary>
        public void CloseAll()
        {
            MergeAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' CLOSE");
        }

        /// <summary>
        /// Merges all the sections of the Pattern.
        /// </summary>
        public void MergeAll()
        {
            SelectAll();
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' MERGE");
        }

        /// <summary>
        /// Inserts the specified toolpath into this Pattern.
        /// </summary>
        /// <param name="toolpath">The toolpath to insert into this Pattern.</param>
        public void InsertToolpath(PMToolpath toolpath)
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' INSERT TOOLPATH \"" + toolpath.Name + "\"");
        }

        /// <summary>
        /// Inserts the specified file into this Pattern.
        /// </summary>
        /// <param name="file">File to insert into this Pattern</param>
        public void InsertFile(File file)
        {
            PowerMill.DoCommand("EDIT PATTERN '" + Name + "' INSERT FILE \"" + file.Path + "\"");
        }

        /// <summary>
        /// Write the Pattern to the specified file.  Supported file types are: dgk, ddz, ddx, dxf, pic
        /// </summary>
        /// <param name="file">The file to write to.</param>
        public void WriteToFile(File file)
        {
            PowerMill.DoCommand("KEEP PATTERN '" + Name + "' FILESAVE \"" + file.Path + "\"");
        }

        /// <summary>
        /// Converts the PMPattern to a list of Polylines.
        /// </summary>
        /// <returns>A list of Polylines</returns>
        public List<Polyline> ToPolylines()
        {
            File tempFile = File.CreateTemporaryFile("pic");
            WriteToFile(tempFile);
            List<Polyline> polylines = Polyline.ReadFromDUCTPictureFile(tempFile);
            tempFile.Delete();
            return polylines;
        }

        /// <summary>
        /// Converts the PMPattern to a list of Splines.
        /// </summary>
        /// <returns>A list of Splines</returns>
        public List<Spline> ToSplines()
        {
            File tempFile = File.CreateTemporaryFile("pic");
            WriteToFile(tempFile);
            List<Spline> splines = Spline.ReadFromDUCTPictureFile(tempFile);
            tempFile.Delete();
            return splines;
        }

        #endregion
    }
}