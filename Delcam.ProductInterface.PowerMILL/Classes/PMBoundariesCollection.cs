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
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Represents a collection of Boundary objects in the Active PowerMILL Project.
    /// </summary>
    public class PMBoundariesCollection : PMEntitiesCollection<PMBoundary>
    {
        private const string CREATE_BOUNDARY = "CREATE BOUNDARY ;";

        #region Constructors

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <remarks></remarks>
        internal PMBoundariesCollection(PMAutomation powerMILL) : base(powerMILL)
        {
            Initialise();
        }

        #endregion

        #region Operations

        /// <summary>
        /// Initialises the collection with the items in PowerMILL.
        /// </summary>
        internal void Initialise()
        {
            foreach (string name in ReadBoundaries())
            {
                Add(PMBoundaryEntityFactory.CreateEntity(_powerMILL, name));
            }
        }

        /// <summary>
        /// Gets a list of the names of all the Boundaries in PowerMILL.
        /// </summary>
        internal List<string> ReadBoundaries()
        {
            List<string> names = new List<string>();
            foreach (var boundary in _powerMILL.PowerMILLProject.Boundaries)
            {
                names.Add(boundary.Name);
            }
            return names;
        }

        /// <summary>
        /// Creates an empty Boundary in PowerMill and adds it to the boundaries collection.
        /// </summary>
        /// <returns>The new PMBoundary.</returns>
        public PMBoundary CreateEmptyBoundary()
        {
            _powerMILL.DoCommand(CREATE_BOUNDARY);

            var newBoundary = (PMBoundary) _powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Imports a file as a Boundary into PowerMill and adds it to the boundaries collection.
        /// </summary>
        /// <param name="file">The file path of the boundary to import.</param>
        /// <returns>The new PMBoundary.</returns>
        public PMBoundary CreateBoundary(FileSystem.File file)
        {
            PMBoundary newBoundary = null;
            if (file.Exists)
            {
                _powerMILL.DoCommand(CREATE_BOUNDARY);

                // Get the name PowerMILL gave to the last boundary
                newBoundary = (PMBoundary) _powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
                Add(newBoundary);
                _powerMILL.DoCommand("EDIT BOUNDARY '" + newBoundary.Name + "' INSERT FILE '" + file.Path + "'");
            }
            return newBoundary;
        }

        /// <summary>
        /// Imports a Polyline as a Boundary into PowerMill and adds it to the boundaries collection.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        public PMBoundary CreateBoundary(Polyline polyline)
        {
            try
            {
                var file = FileSystem.File.CreateTemporaryFile("pic");
                polyline.WriteToDUCTPictureFile(file);

                var boundary = CreateBoundary(file);

                file.Delete();

                return boundary;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Imports a Spline as a Boundary into PowerMill and adds it to the boundaries collection.
        /// </summary>
        /// <param name="spline"></param>
        /// <returns></returns>
        public PMBoundary CreateBoundary(Spline spline)
        {
            try
            {
                var file = FileSystem.File.CreateTemporaryFile("pic");
                spline.WriteToDUCTPictureFile(file);

                var boundary = CreateBoundary(file);

                file.Delete();

                return boundary;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a Silhouette Boundary in PowerMill.
        /// </summary>
        /// <param name="partName">The name of the model around which the boundary will be created.</param>
        /// <param name="blockExpansion">Offsets the minimum block size by blockExpansion. If this parameter is 0 the block used to calculate the silhouette boundary will have the minimum size to embed the part.</param>
        /// <param name="partBorder">The distance between the silhouette boundary and the part. To achieve this, internally the tool diameter used to calculate the silhouette will be partBorder * 2.</param>
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <returns></returns>
        public PMBoundarySilhouette CreateSilhouetteBoundary(
            string partName,
            double blockExpansion,
            double partBorder,
            double boundaryTolerance)
        {
            PMBoundarySilhouette newBoundary = null;
            if (!string.IsNullOrEmpty(partName))
            {
                _powerMILL.DoCommand("EDIT MODEL ALL DESELECT ALL");
                _powerMILL.DoCommand(string.Format("EDIT MODEL \"{0}\" SELECT ALL", partName));
                _powerMILL.DoCommand("DELETE BLOCK");
                _powerMILL.DoCommand(string.Format("EDIT BLOCK RESETLIMIT \"{0}\"", blockExpansion));
                _powerMILL.DoCommand("EDIT BLOCK LIMITTYPE MODEL");
                _powerMILL.DoCommand("EDIT BLOCK RESET");
                _powerMILL.DoCommand("BLOCK ACCEPT");
                _powerMILL.DoCommand("create tool ; ENDMILL");
                var toolDiameter = partBorder * 2;
                _powerMILL.DoCommand(string.Format("EDIT TOOL ; DIAMETER \"{0}\"", toolDiameter));
                _powerMILL.DoCommand("TOOL ACCEPT");
                _powerMILL.DoCommand("EDIT MODEL ALL DESELECT ALL");
                _powerMILL.DoCommand(string.Format("EDIT MODEL \"{0}\" SELECT ALL", partName));
                _powerMILL.DoCommand("CREATE BOUNDARY ; SILHOUETTE");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
                _powerMILL.DoCommand("EDIT BOUNDARY ; TOOL NAME ;");
                _powerMILL.DoCommand("EDIT BOUNDARY ; CALCULATE");
                _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
                newBoundary = (PMBoundarySilhouette) _powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
                Add(newBoundary);
            }
            return newBoundary;
        }

        /// <summary>
        /// Creates a Rest Boundary in PowerMill.
        /// </summary>
        /// <param name="thickerThan">Detect material thicker than.</param>
        /// <param name="expandBy">Expand Area by.</param>
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <param name="useAxialThickness">Use axial thickness.</param>
        /// <param name="thickness">The thickness of the boundary.</param>
        /// <param name="axialThickness">The axial thickness of the boundary.</param>
        /// <param name="tool">The tool for the boundary.</param>
        /// <param name="refTool">the reference tool for the boundary.</param>
        /// <returns></returns>
        public PMBoundaryRest CreateRestBoundary(
            double thickerThan,
            double expandBy,
            double boundaryTolerance,
            bool useAxialThickness,
            double thickness,
            double axialThickness,
            PMTool tool,
            PMTool refTool)            
        {
            PMBoundaryRest newBoundary = null;
                          
            _powerMILL.DoCommand("CREATE BOUNDARY ; REST3D");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKER \"{0}\"", thickerThan));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EXTEND \"{0}\"", expandBy));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS \"{0}\"", thickness));
            if (useAxialThickness)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL ON");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS AXIAL \"{0}\"", axialThickness));
            } else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL OFF");
            }
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOOL NAME \"{0}\"", tool.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; REFTOOL NAME \"{0}\"", refTool.Name));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
            newBoundary = (PMBoundaryRest)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);
                        
            return newBoundary;
        }

        #endregion
    }
}
