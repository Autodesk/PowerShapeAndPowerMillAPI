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
using Autodesk.FileSystem;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a Boundary object in PowerMILL.
    /// </summary>
    public abstract class PMBoundary : PMEntity
    {
        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMBoundary(PMAutomation powerMILL) : base(powerMILL)
        {
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="name">The new instance name.</param>
        internal PMBoundary(PMAutomation powerMILL, string name) : base(powerMILL, name)
        {
        }

        #endregion

        #region Properties

        internal static string BOUNDARY_IDENTIFIER = "BOUNDARY";

        /// <summary>
        /// Gets the identifier PowerMILL uses to identify this type of entity.
        /// </summary>
        internal override string Identifier
        {
            get { return BOUNDARY_IDENTIFIER; }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Deletes boundary from PowerMill and from the active project boundaries list.
        /// </summary>
        public override void Delete()
        {
            PowerMill.ActiveProject.Boundaries.Remove(this);
        }

        /// <summary>
        /// Inserts the specified toolpath into this Boundary.
        /// </summary>
        /// <param name="toolpath">The toolpath to insert into this Boundary.</param>
        public void InsertToolpath(PMToolpath toolpath)
        {
            PowerMill.DoCommand("EDIT BOUNDARY '" + Name + "' INSERT TOOLPATH \"" + toolpath.Name + "\"");
        }

        /// <summary>
        /// Inserts the specified file into this Boundary.
        /// </summary>
        /// <param name="file">The file to insert into this Boundary.</param>
        public void InsertFile(File file)
        {
            PowerMill.DoCommand("EDIT BOUNDARY '" + Name + "' INSERT FILE \"" + file.Path + "\"");
        }

        /// <summary>
        /// Write the Boundary to the specified file.  Supported file types are: dgk, ddz, ddx, dxf, pic
        /// </summary>
        /// <param name="file">The file to write to.</param>
        public void WriteToFile(File file)
        {
            PowerMill.DoCommand("KEEP BOUNDARY '" + Name + "' FILESAVE \"" + file.Path + "\"");
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

        /// <summary>
        /// Fatten the boundary onto the XY Plane at Z = 0.
        /// </summary>
        public void Flat()
        {
            PowerMill.DoCommand(string.Format("EDIT BOUNDARY \"{0}\" CURVEEDITOR START", Name));
            PowerMill.DoCommand("CURVEEDITOR FLATTEN SELECTED");
            PowerMill.DoCommand("CURVEEDITOR FINISH ACCEPT");
        }

        /// <summary>
        /// Polygonise a boundary to a specified tolerance.
        /// </summary>
        /// <param name="smashTolerance">The tolerance used to polygonise boundary.</param>
        public void Smash(double smashTolerance)
        {
            PowerMill.DoCommand(string.Format("EDIT BOUNDARY \"{0}\" SMASH {1}", Name, smashTolerance));
        }

        /// <summary>
        /// Inserts the specified Boundary into this Boundary.
        /// </summary>
        /// <param name="boundary">The boundary to insert into this Boundary.</param>
        public void InsertBoundary(PMBoundary boundary)
        {
            if (boundary == null || !boundary.Exists)
                throw new ArgumentNullException("boundary", "Boundary not found");

            PowerMill.DoCommand("EDIT BOUNDARY '" + Name + "' INSERT BOUNDARY '" + boundary.Name + "'");
        }

        /// <summary>
        /// Inserts the specified Boundary into this Boundary.
        /// </summary>
        /// <param name="boundaryName">The name of the boundary to insert into this Boundary.</param>
        public void InsertBoundaryByName(string boundaryName)
        {
            PMBoundary boundary = PowerMill.ActiveProject.Boundaries.GetByName(boundaryName);

            if (boundary == null || !boundary.Exists)
                throw new ArgumentNullException("boundary", "Boundary not found");

            PowerMill.DoCommand("EDIT BOUNDARY '" + Name + "' INSERT BOUNDARY '" + boundary.Name + "'");
        }

        #endregion
    }
}