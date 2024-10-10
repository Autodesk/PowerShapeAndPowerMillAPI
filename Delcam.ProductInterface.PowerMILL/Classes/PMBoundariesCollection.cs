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
                _powerMILL.DoCommand("EDIT BOUNDARY \"" + newBoundary.Name + "\" INSERT FILE \"" + file.Path + "\"");
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
        /// Creates a Block Boundary in PowerMill.
        /// </summary>        
        /// <returns></returns>
        public PMBoundaryBlock CreateBlockBoundary()
        {
            PMBoundaryBlock newBoundary = null;
           
            if (_powerMILL.ActiveProject.GetBlockLimits().Volume != 0)
            {
                //Create Block Boundary
                _powerMILL.DoCommand("CREATE BOUNDARY ; BLOCK FORM BOUNDARY");
                _powerMILL.DoCommand("EDIT BOUNDARY ; CALCULATE");
                _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");

                newBoundary = (PMBoundaryBlock)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
                Add(newBoundary);
            }
            
            return newBoundary;
        }

        /// <summary>
        /// Creates a Selected Surface Boundary in PowerMill.
        /// </summary>
        /// <param name="top">Controls whether boundary goes top or bottom of vertical walls.</param>
        /// <param name="rollOver">Toggles roll over flag</param>
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <param name="useAxialThickness">Use axial thickness.</param>
        /// <param name="thickness">The thickness of the boundary.</param>
        /// <param name="axialThickness">The axial thickness of the boundary.</param>
        /// <param name="tool">The tool for the boundary.</param>        
        /// <returns></returns>
        public PMBoundarySelectedSurface CreateSelectedSurfaceBoundary(
            bool top,
            bool rollOver,
            double boundaryTolerance,
            bool useAxialThickness,
            double thickness,
            double axialThickness,
            PMTool tool)
        {
            PMBoundarySelectedSurface newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; SELECTED");

            if(top)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; SELVERTICAL UP");
            } else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; SELVERTICAL DOWN");
            }
            
            if (rollOver)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; SELROLLOVER ON");
            }
            else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; SELROLLOVER OFF");
            }
                        
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS \"{0}\"", thickness));
            if (useAxialThickness)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL ON");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS AXIAL \"{0}\"", axialThickness));
            }
            else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL OFF");
            }
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOOL NAME \"{0}\"", tool.Name));           
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
            newBoundary = (PMBoundarySelectedSurface)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Creates a Shallow Boundary in PowerMill.
        /// </summary>
        /// <param name="upperAngle">Max Angle.</param>
        /// <param name="lowerAngle">Min Angle.</param>
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <param name="useAxialThickness">Use axial thickness.</param>
        /// <param name="thickness">The thickness of the boundary.</param>
        /// <param name="axialThickness">The axial thickness of the boundary.</param>
        /// <param name="tool">The tool for the boundary.</param>        
        /// <returns></returns>
        public PMBoundaryShallow CreateShallowBoundary(
            double upperAngle,
            double lowerAngle,
            double boundaryTolerance,
            bool useAxialThickness,
            double thickness,
            double axialThickness,
            PMTool tool)
        {
            PMBoundaryShallow newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; SHALLOW");           
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; SLOPE \"{0}\"", upperAngle));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; SET_LOWER_ANGLE \"{0}\"", lowerAngle));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS \"{0}\"", thickness));
            if (useAxialThickness)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL ON");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS AXIAL \"{0}\"", axialThickness));
            }
            else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL OFF");
            }
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOOL NAME \"{0}\"", tool.Name));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
            newBoundary = (PMBoundaryShallow)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
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
        /// Creates a Collision Safe Boundary in PowerMill.
        /// </summary>
        /// <param name="holderClearance">Holder Clearance</param>
        /// <param name="shankClearance">Shank Clearance</param>
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <param name="useAxialThickness">Use axial thickness.</param>
        /// <param name="thickness">The thickness of the boundary.</param>
        /// <param name="axialThickness">The axial thickness of the boundary.</param>
        /// <param name="tool">The tool for the boundary.</param>        
        /// <returns></returns>
        public PMBoundaryCollisionSafe CreateCollisionSafeBoundary(
            double holderClearance,
            double shankClearance,
            double boundaryTolerance,
            bool useAxialThickness,
            double thickness,
            double axialThickness,
            PMTool tool)
        {
            PMBoundaryCollisionSafe newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; COLLISION");            
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; SET_HOLDER_CLEARANCE \"{0}\"", holderClearance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; SET_SHANK_CLEARANCE \"{0}\"", shankClearance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS \"{0}\"", thickness));
            if (useAxialThickness)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL ON");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS AXIAL \"{0}\"", axialThickness));
            }
            else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL OFF");
            }
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOOL NAME \"{0}\"", tool.Name));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
            newBoundary = (PMBoundaryCollisionSafe)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Creates a Stock Model Rest Boundary in PowerMill.
        /// </summary>
        /// <param name="stockModel">The Stock Model for the Boundary.</param>
        /// <param name="stockModelState">The State of the Stock Model for the Boundary.</param>
        /// <param name="thickerThan">Detect Material Thicker Than.</param>
        /// <param name="expandAreaBy">Expand Area by.</param>
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <param name="useAxialThickness">Use axial thickness.</param>
        /// <param name="thickness">The thickness of the boundary.</param>
        /// <param name="axialThickness">The axial thickness of the boundary.</param>
        /// <param name="tool">The tool for the boundary.</param>        
        /// <returns></returns>
        public PMBoundaryStockModelRest CreateStockModelRestBoundary(
            PMStockModel stockModel,
            string stockModelState,
            double thickerThan,
            double expandAreaBy,
            double boundaryTolerance,
            bool useAxialThickness,
            double thickness,
            double axialThickness,
            PMTool tool)
        {
            PMBoundaryStockModelRest newBoundary = null;
            
            int index = stockModel.States.IndexOf(stockModelState);                        
            _powerMILL.DoCommand("CREATE BOUNDARY ; STOCKMODEL_REST");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; STOCKMODEL \"{0}\"", stockModel.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; STOCKMODEL \"{0}\" STATE \"{1}\"", stockModel.Name, index));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; STOCKMODEL_THICKER \"{0}\"", thickerThan));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EXTEND \"{0}\"", expandAreaBy));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS \"{0}\"", thickness));
            if (useAxialThickness)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL ON");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS AXIAL \"{0}\"", axialThickness));
            }
            else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL OFF");
            }
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOOL NAME \"{0}\"", tool.Name));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
            newBoundary = (PMBoundaryStockModelRest)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Create a Contact Point Boundary from Boundary
        /// </summary>
        /// <param name="boundary">Boundary to convert to Contact Point Boundary.</param>
        /// <param name="modelTolerance">Model Tolerance.</param>
        /// <param name="edgeTolerance">Edge Tolerance.</param>
        /// <returns></returns>
        public PMBoundaryContactPoint CreateContactPointBoundary(
            PMBoundary boundary,
            double modelTolerance,
            double edgeTolerance)
        {
            PMBoundaryContactPoint newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; CONTACTPOINT");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY PREINSERT BOUNDARY \"{0}\"", boundary.Name));            
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; INSERT BOUNDARY FROM_COMBO"));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", modelTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EDGE_TOLERANCE \"{0}\"", edgeTolerance));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");

            newBoundary = (PMBoundaryContactPoint)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Create a Contact Point Boundary from Pattern
        /// </summary>
        /// <param name="pattern">Pattern to convert to Contact Point Boundary.</param>
        /// <param name="modelTolerance">Model Tolerance.</param>
        /// <param name="edgeTolerance">Edge Tolerance.</param>
        /// <returns></returns>
        public PMBoundaryContactPoint CreateContactPointBoundary(
            PMPattern pattern,
            double modelTolerance,
            double edgeTolerance)
        {
            PMBoundaryContactPoint newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; CONTACTPOINT");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY PREINSERT PATTERN \"{0}\"", pattern.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; INSERT PATTERN FROM_COMBO"));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", modelTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EDGE_TOLERANCE \"{0}\"", edgeTolerance));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");

            newBoundary = (PMBoundaryContactPoint)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Create a Contact Point Boundary from Toolpath
        /// </summary>
        /// <param name="toolpath">toolpath to convert to Contact Point Boundary.</param>
        /// <param name="modelTolerance">Model Tolerance.</param>
        /// <param name="edgeTolerance">Edge Tolerance.</param>
        /// <returns></returns>
        public PMBoundaryContactPoint CreateContactPointBoundary(
            PMToolpath toolpath,
            double modelTolerance,
            double edgeTolerance)
        {
            PMBoundaryContactPoint newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; CONTACTPOINT");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY PREINSERT TOOLPATH \"{0}\"", toolpath.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; INSERT TOOLPATH FROM_COMBO"));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", modelTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EDGE_TOLERANCE \"{0}\"", edgeTolerance));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");

            newBoundary = (PMBoundaryContactPoint)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Create a Contact Point Boundary from selected Surfaces.
        /// </summary>        
        /// <param name="modelTolerance">Model Tolerance.</param>
        /// <param name="edgeTolerance">Edge Tolerance.</param>
        /// <returns></returns>
        public PMBoundaryContactPoint CreateContactPointBoundary(            
            double modelTolerance,
            double edgeTolerance)
        {
            PMBoundaryContactPoint newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; CONTACTPOINT");
            _powerMILL.DoCommand("EDIT BOUNDARY ; INSERT MODEL");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", modelTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EDGE_TOLERANCE \"{0}\"", edgeTolerance));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");

            newBoundary = (PMBoundaryContactPoint)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Creates a Contact Conversion Boundary in PowerMill.
        /// </summary>
        /// <param name="contactPointBoundary">The input contact point boundary</param>        
        /// <param name="boundaryTolerance">The boundary tolerance.</param>
        /// <param name="boundaryEdgeTolerance">The boundary tolerance.</param>
        /// <param name="useAxialThickness">Use axial thickness.</param>
        /// <param name="thickness">The thickness of the boundary.</param>
        /// <param name="axialThickness">The axial thickness of the boundary.</param>
        /// <param name="tool">The tool for the boundary.</param>        
        /// <returns></returns>
        public PMBoundaryContactConversion CreateContactConversionBoundary(
            PMBoundary contactPointBoundary,
            double boundaryTolerance,
            double boundaryEdgeTolerance,
            bool useAxialThickness,
            double thickness,
            double axialThickness,
            PMTool tool)
        {
            PMBoundaryContactConversion newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; CONTACTCONV");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; CONTACT_POINT_BOUND \"{0}\"", contactPointBoundary.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", boundaryTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; EDGE_TOLERANCE \"{0}\"", boundaryEdgeTolerance));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS \"{0}\"", thickness));
            if (useAxialThickness)
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL ON");
                _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; THICKNESS AXIAL \"{0}\"", axialThickness));
            }
            else
            {
                _powerMILL.DoCommand("EDIT BOUNDARY ; THICKNESS AXIAL_RADIAL OFF");
            }
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOOL NAME \"{0}\"", tool.Name));
            _powerMILL.DoCommand("EDIT BOUNDARY ; ACCEPT BOUNDARY ACCEPT");
            newBoundary = (PMBoundaryContactConversion)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

            return newBoundary;
        }

        /// <summary>
        /// Creates a Boolean Operation Boundary in PowerMill.
        /// </summary>
        /// <param name="boundaryA">First reference Boundary.</param>        
        /// <param name="boundaryB">Second reference Boundary.</param>
        /// <param name="booleanType">The boolean operation to perform</param>
        /// <param name="tolerance">Accuracy with which the boolean operation will be performed.</param>        
        /// <returns></returns>
        public PMBoundaryBooleanOperation CreateBooleanOperationBoundary(
            PMBoundary boundaryA,
            PMBoundary boundaryB,
            BoundaryBooleanTypes booleanType,            
            double tolerance)
        {
            PMBoundaryBooleanOperation newBoundary = null;

            _powerMILL.DoCommand("CREATE BOUNDARY ; BOOLEANOPERATION");
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; REF_BOUNDARY_A \"{0}\"", boundaryA.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; REF_BOUNDARY_B \"{0}\"", boundaryB.Name));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; OPERATION_TYPE {0}", booleanType));
            _powerMILL.DoCommand(string.Format("EDIT BOUNDARY ; TOLERANCE \"{0}\"", tolerance));
                        
            newBoundary = (PMBoundaryBooleanOperation)_powerMILL.ActiveProject.CreatedItems(typeof(PMBoundary)).Last();
            Add(newBoundary);

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
