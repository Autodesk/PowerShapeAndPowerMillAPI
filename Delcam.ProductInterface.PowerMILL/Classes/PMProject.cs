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
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// Captures a PowerMILL project and gives access to the collections of objects in it.
    /// </summary>
    public class PMProject
    {
        #region Fields

        /// <summary>
        /// The PowerMILL Automation object.
        /// </summary>
        protected PMAutomation _powerMILL;

        /// <summary>
        /// The directory for the PowerMILL Project.
        /// </summary>
        protected Directory _projectDirectory;

        /// <summary>
        /// The list of NC Programs in the Project.
        /// </summary>
        protected PMNCProgramsCollection _NCPrograms;

        /// <summary>
        /// The list of Boundaries in the Project.
        /// </summary>
        protected PMBoundariesCollection _boundaries;

        /// <summary>
        /// The list of FeatureGroups in the Project.
        /// </summary>
        protected PMFeatureGroupsCollection _featureGroups;

        /// <summary>
        /// The list of FeatureSets in the Project.
        /// </summary>
        protected PMFeatureSetsCollection _featureSets;

        /// <summary>
        /// The list of Groups in the Project.
        /// </summary>
        protected PMGroupsCollection _groups;

        /// <summary>
        /// The list of Levels and Sets in the Project.
        /// </summary>
        protected PMLevelOrSetsCollection _levelsOrSets;

        /// <summary>
        /// The list of Models in the Project.
        /// </summary>
        protected PMModelsCollection _models;

        /// <summary>
        /// The list of Patterns in the Project.
        /// </summary>
        protected PMPatternsCollection _patterns;

        /// <summary>
        /// The list of Setups in the Project.
        /// </summary>
        protected PMSetupsCollection _setups;

        /// <summary>
        /// The list of Stock Models in the Project.
        /// </summary>
        protected PMStockModelsCollection _stockModels;

        /// <summary>
        /// The list of Toolpaths in the Project.
        /// </summary>
        protected PMToolpathsCollection _toolpaths;

        /// <summary>
        /// The list of Tools in the Project.
        /// </summary>
        protected PMToolsCollection _tools;

        /// <summary>
        /// The list of Workplanes in the Project.
        /// </summary>
        protected PMWorkplanesCollection _workplanes;

        /// <summary>
        /// The list of tools in the Project.
        /// </summary>
        protected PMMachineToolsCollection _machineTools;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        internal PMProject(PMAutomation powerMILL)
        {
            _powerMILL = powerMILL;
            _projectDirectory = null;
            Initialise();
        }

        /// <summary>
        /// Initialises the item.
        /// </summary>
        /// <param name="powerMILL">The base instance to interact with PowerMILL.</param>
        /// <param name="projectDirectory">The project directory path.</param>
        /// <param name="openReadOnly">If true it will open as read only.</param>
        /// <remarks></remarks>
        internal PMProject(PMAutomation powerMILL, Directory projectDirectory, bool openReadOnly = false)
        {
            _powerMILL = powerMILL;
            _projectDirectory = projectDirectory;
            if (openReadOnly)
            {
                _powerMILL.DoCommand("PROJECT READONLY OPEN '" + projectDirectory.Path + "'");
            }
            else
            {
                _powerMILL.DoCommand("PROJECT OPEN '" + projectDirectory.Path + "'");
            }
            Initialise();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the PowerMILL directory of the active project.
        /// </summary>
        /// <returns>Nothing if project has not been saved.</returns>
        public Directory Directory
        {
            get
            {
                if (_powerMILL.Version.Major < 14)
                {
                    throw new Exception("This property is not suported in versions earlier than 14");
                }
                Directory folder = null;
                string pmpathTemp = (string) _powerMILL.DoCommandEx("print $project_pathname(false)");
                if (!string.IsNullOrWhiteSpace(pmpathTemp))
                {
                    pmpathTemp = pmpathTemp.Replace(((char) 13).ToString(), string.Empty);
                    pmpathTemp = pmpathTemp.Replace(((char) 10).ToString(), string.Empty);
                    string unixDirPath = pmpathTemp;
                    try
                    {
                        pmpathTemp = System.IO.Path.GetFullPath(unixDirPath);

                        //requires user permsissions and can sometimes fail.
                    }
                    catch
                    {
                        //pmill returns unix directory separators and we are expecting window ones.
                        pmpathTemp = unixDirPath.Replace(System.IO.Path.AltDirectorySeparatorChar,
                                                         System.IO.Path.DirectorySeparatorChar);
                    }
                    folder = new Directory(pmpathTemp);
                }
                return folder;
            }
        }

        /// <summary>
        /// Gets the collection of Boundaries currently in PowerMILL.
        /// </summary>
        public PMBoundariesCollection Boundaries
        {
            get { return _boundaries; }
        }

        /// <summary>
        /// Gets the collection of FeatureGroups currently in PowerMILL.
        /// </summary>
        public PMFeatureGroupsCollection FeatureGroups
        {
            get { return _featureGroups; }
        }

        /// <summary>
        /// Gets the collection of FeatureSets currently in PowerMILL.
        /// </summary>
        public PMFeatureSetsCollection FeatureSets
        {
            get { return _featureSets; }
        }

        /// <summary>
        /// Gets the collection of Groups currently in PowerMILL.
        /// </summary>
        public PMGroupsCollection Groups
        {
            get { return _groups; }
        }

        /// <summary>
        /// Gets the collection of Levels and Sets currently in PowerMILL.
        /// </summary>
        public PMLevelOrSetsCollection LevelsAndSets
        {
            get { return _levelsOrSets; }
        }

        /// <summary>
        /// Gets the collection of Machine Tools currently in PowerMILL.
        /// </summary>
        public PMMachineToolsCollection MachineTools
        {
            get { return _machineTools; }
        }

        /// <summary>
        /// Gets the collection of Models currently in PowerMILL.
        /// </summary>
        public PMModelsCollection Models
        {
            get { return _models; }
        }

        /// <summary>
        /// Gets the collection of NCPrograms currently in PowerMILL.
        /// </summary>
        public PMNCProgramsCollection NCPrograms
        {
            get { return _NCPrograms; }
        }

        /// <summary>
        /// Gets the collection of Patterns currently in PowerMILL.
        /// </summary>
        public PMPatternsCollection Patterns
        {
            get { return _patterns; }
        }

        /// <summary>
        /// Gets the collection of Setups currently in PowerMILL.
        /// </summary>
        public PMSetupsCollection Setups
        {
            get { return _setups; }
        }

        /// <summary>
        /// Gets the collection of Stock Models currently in PowerMILL.
        /// </summary>
        public PMStockModelsCollection StockModels
        {
            get { return _stockModels; }
        }

        /// <summary>
        /// Gets the collection of Toolpaths currently in PowerMILL.
        /// </summary>
        public PMToolpathsCollection Toolpaths
        {
            get { return _toolpaths; }
        }

        /// <summary>
        /// Gets the collection of Tools currently in PowerMILL.
        /// </summary>
        public PMToolsCollection Tools
        {
            get { return _tools; }
        }

        /// <summary>
        /// Gets the collection of Workplanes currently in PowerMILL.
        /// </summary>
        public PMWorkplanesCollection Workplanes
        {
            get { return _workplanes; }
        }

        /// <summary>
        /// Returns the currently active Workplane or Nothing if World is active.
        /// </summary>
        /// <returns>The Active Workplane or Nothing if World is the Active Workplane.</returns>
        public PMWorkplane ActiveWorkplane
        {
            get
            {
                string activeName = _powerMILL.DoCommandEx("PRINT PAR terse \"entity('" + PMWorkplane.WORKPLANE_IDENTIFIER + "','').Name\"").ToString();
                return Workplanes.FirstOrDefault(x => x.Name == activeName);
            }

            set
            {
                if (value == null)
                {
                    // Set world workplane active
                    _powerMILL.DoCommand("ACTIVATE WORKPLANE \" \"");
                }
                else if (value.Exists)
                {
                    // Activate the selected workplane
                    _powerMILL.DoCommand("ACTIVATE WORKPLANE \"" + value.Name + "\"");
                }
                else
                {
                    throw new Exception("Unable to activate workplane");
                }
            }
        }


        #endregion

        #region Operations

        /// <summary>
        /// Initialises the project collections based on what is currently in PowerMILL.
        /// </summary>
        public void Initialise()
        {
            // Setup the collections based on what is currently in PowerMILL
            _NCPrograms = new PMNCProgramsCollection(_powerMILL);
            _boundaries = new PMBoundariesCollection(_powerMILL);
            _featureGroups = new PMFeatureGroupsCollection(_powerMILL);
            _featureSets = new PMFeatureSetsCollection(_powerMILL);
            _groups = new PMGroupsCollection(_powerMILL);
            _levelsOrSets = new PMLevelOrSetsCollection(_powerMILL);
            _machineTools = new PMMachineToolsCollection(_powerMILL);
            _models = new PMModelsCollection(_powerMILL);
            _patterns = new PMPatternsCollection(_powerMILL);
            _setups = new PMSetupsCollection(_powerMILL);
            _stockModels = new PMStockModelsCollection(_powerMILL);
            _toolpaths = new PMToolpathsCollection(_powerMILL);
            _tools = new PMToolsCollection(_powerMILL);
            _workplanes = new PMWorkplanesCollection(_powerMILL);
        }

        /// <summary>
        /// This operation refreshes the information available in the current project.
        /// Refresh should retain any existing items in the lists, add new ones and remove old ones.
        /// This function would be used by someone after they have used the "Execute" functionality.
        /// </summary>
        public void Refresh()
        {
            var originalEntities = new List<PMEntity>();
            originalEntities.AddRange(_boundaries);
            RefreshEntities(originalEntities, _boundaries.ReadBoundaries(), PMBoundary.BOUNDARY_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_featureSets);
            RefreshEntities(originalEntities, _featureSets.ReadFeatureSets(), PMFeatureSet.FEATURESET_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_groups);
            RefreshEntities(originalEntities, _groups.ReadGroups(), PMGroup.GROUP_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_levelsOrSets);
            RefreshEntities(originalEntities, _levelsOrSets.ReadLevelsAndSets(), PMLevelOrSet.LEVEL_OR_SET_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_models);
            RefreshEntities(originalEntities, _models.ReadModels(), PMModel.MODEL_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_NCPrograms);
            RefreshEntities(originalEntities, _NCPrograms.ReadNCPrograms(), PMNCProgram.NC_PROGRAM_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_patterns);
            RefreshEntities(originalEntities, _patterns.ReadPatterns(), PMPattern.PATTERN_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_stockModels);
            RefreshEntities(originalEntities, _stockModels.ReadStockModels(), PMStockModel.STOCKMODEL_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_toolpaths);
            RefreshEntities(originalEntities, _toolpaths.ReadToolpaths(), PMToolpath.TOOLPATH_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_tools);
            RefreshEntities(originalEntities, _tools.ReadTools(), PMTool.TOOL_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_workplanes);
            RefreshEntities(originalEntities, _workplanes.ReadWorkplanes(), PMWorkplane.WORKPLANE_IDENTIFIER);
            originalEntities.Clear();
            originalEntities.AddRange(_machineTools);
            RefreshEntities(originalEntities, _machineTools.ReadMachineTools(), PMMachineTool.MACHINE_TOOL_IDENTIFIER);
        }

        /// <summary>
        /// This operation creates a Block in PowerMILL from a DMT file.
        /// </summary>
        /// <param name="blockFile">The DMT file from which to create the block.</param>
        public void CreateBlock(File blockFile)
        {
            _powerMILL.DoCommand("DELETE BLOCK \\r " + "EDIT BLOCKTYPE TRIANGLES \\r " + "EDIT BLOCK COORDINATE WORLD",
                                 "GET BLOCK \"" + blockFile.Path + "\"",
                                 "EDIT BLOCK DRAWMODE 25",
                                 "EDIT BLOCK UPDATEFORM",
                                 "BLOCK ACCEPT");
        }

        /// <summary>
        /// This operation creates a Block in PowerMILL from an existing model.
        /// </summary>
        /// <param name="modelName">The name of the model from which to create the block.</param>
        /// <param name="blockExpansion">Offsets the minimum block size by blockExpansion. If this parameter is 0 the block used to calculat ethe silhouette boundary will have the minimum size to embed the part.</param>
        public void CreateBlock(string modelName, double blockExpansion)
        {
            _powerMILL.DoCommand("EDIT MODEL ALL DESELECT ALL");
            _powerMILL.DoCommand(string.Format("EDIT MODEL \"{0}\" SELECT ALL", modelName));
            _powerMILL.DoCommand("DELETE BLOCK");
            _powerMILL.DoCommand(string.Format("EDIT BLOCK RESETLIMIT \"{0}\"", blockExpansion));
            _powerMILL.DoCommand("EDIT BLOCK LIMITTYPE MODEL");
            _powerMILL.DoCommand("EDIT BLOCK RESET");
            _powerMILL.DoCommand("BLOCK ACCEPT");
        }

        /// <summary>
        /// This operation creates a Block in PowerMILL from an boundary. The heights are calculated automatically 
        /// </summary>
        /// <param name="boundary">The boundary from which to create the block.</param>        
        public BoundingBox CreateBlockFromBoundary(PMBoundary boundary)
        {
            if (boundary == null || !boundary.Exists)
                throw new ArgumentNullException("boundary", "Boundary not found");
                       
            _powerMILL.DoCommand("EDIT MODEL ALL DESELECT ALL");
            _powerMILL.DoCommand(string.Format("ACTIVATE BOUNDARY \"{0}\"", boundary.Name));
            _powerMILL.DoCommand("EDIT BLOCKTYPE BOUNDARY");
            _powerMILL.DoCommand("EDIT BLOCK RESETLIMIT 0");
            _powerMILL.DoCommand("EDIT BLOCK RESET");
            _powerMILL.DoCommand("BLOCK ACCEPT");            
            BoundingBox boundingBox = GetBlockLimits();            
            return boundingBox;
        }

        /// <summary>
        /// This operation creates a Block in PowerMILL from an boundary with Z-Min and Z-Max values.
        /// </summary>
        /// <param name="boundary">The boundary from which to create the block.</param>
        /// <param name="ZMin">Set the minimum Z value of the Block</param>
        /// <param name="ZMax">Set the maximum Z value of the Block</param>
        public BoundingBox CreateBlockFromBoundaryWithLimits(PMBoundary boundary, double ZMin, double ZMax)
        {
            if (boundary == null || !boundary.Exists)
                throw new ArgumentNullException("boundary", "Boundary not found");
                       
            _powerMILL.DoCommand("EDIT MODEL ALL DESELECT ALL");
            _powerMILL.DoCommand(string.Format("ACTIVATE BOUNDARY \"{0}\"", boundary.Name));
            _powerMILL.DoCommand("EDIT BLOCKTYPE BOUNDARY");
            _powerMILL.DoCommand("EDIT BLOCK RESETLIMIT 0");                
            _powerMILL.DoCommand(string.Format("EDIT BLOCK ZMIN \"{0}\"", ZMin));
            _powerMILL.DoCommand(string.Format("EDIT BLOCK ZMAX \"{0}\"", ZMax));
            _powerMILL.DoCommand("BLOCK ACCEPT");           
            BoundingBox boundingBox = GetBlockLimits();
            return boundingBox;
        }

        /// <summary>
        /// This operation exports the Block as DMT file 
        /// </summary>
        /// <param name="file">The file where to save the block (.dmt and .stl only)</param>        
        public void ExportBlock(Autodesk.FileSystem.File file)
        {                      
            if (file.Extension.ToLower() == "dmt" || file.Extension.ToLower() == "stl")
            {
                _powerMILL.DoCommand("EXPORT BLOCK ; '" + file + "' YES");
            }
            else
            {
                throw new Exception("Only .dmt and .stl supported");
            }
        }

        /// <summary>
        /// Returns the block limits
        /// </summary>
        public BoundingBox GetBlockLimits()
        {            
            var tpBlockXmin = _powerMILL.DoCommandEx("PRINT PAR TERSE $Block.Limits.XMin");
            var tpBlockXmax = _powerMILL.DoCommandEx("PRINT PAR TERSE $Block.Limits.XMax");
            var tpBlockYmin = _powerMILL.DoCommandEx("PRINT PAR TERSE $Block.Limits.YMin");
            var tpBlockYmax = _powerMILL.DoCommandEx("PRINT PAR TERSE $Block.Limits.YMax");
            var tpBlockZmin = _powerMILL.DoCommandEx("PRINT PAR TERSE $Block.Limits.ZMin");
            var tpBlockZmax = _powerMILL.DoCommandEx("PRINT PAR TERSE $Block.Limits.ZMax");

            BoundingBox boundingBox = new BoundingBox(Convert.ToDouble(tpBlockXmin), Convert.ToDouble(tpBlockXmax),
                                                      Convert.ToDouble(tpBlockYmin), Convert.ToDouble(tpBlockYmax),
                                                      Convert.ToDouble(tpBlockZmin), Convert.ToDouble(tpBlockZmax));
                        
            return boundingBox;
        }

        /// <summary>
        /// This operation deletes the Block from PowerMill.
        /// </summary>
        public void DeleteBlock()
        {
            _powerMILL.DoCommand("DELETE BLOCK");
        }

        /// <summary>
        /// Gets a list of all Entities not currently in a collection.
        /// </summary>
        /// <param name="type">Used if only want items of a specific type. If set to nothing then it will check all entities.</param>
        internal List<PMEntity> CreatedItems(Type type = null)
        {
            List<PMEntity> createdItemsList = new List<PMEntity>();

            // Read all the entities, if they are not in a list then add them to the list of CreatedItems
            if (type == null || ReferenceEquals(type, typeof(PMNCProgram)))
            {
                foreach (string name in _NCPrograms.ReadNCPrograms())
                {
                    if (_NCPrograms[name] == null)
                    {
                        createdItemsList.Add(new PMNCProgram(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMToolpath)))
            {
                foreach (string name in _toolpaths.ReadToolpaths())
                {
                    if (_toolpaths[name] == null)
                    {
                        createdItemsList.Add(PMToolpathEntityFactory.CreateEntity(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMTool)))
            {
                foreach (string name in _tools.ReadTools())
                {
                    if (_tools[name] == null)
                    {
                        createdItemsList.Add(PMToolEntityFactory.CreateEntity(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMBoundary)))
            {
                foreach (string name in _boundaries.ReadBoundaries())
                {
                    if (_boundaries[name] == null)
                    {
                        createdItemsList.Add(PMBoundaryEntityFactory.CreateEntity(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMPattern)))
            {
                foreach (string name in _patterns.ReadPatterns())
                {
                    if (_patterns[name] == null)
                    {
                        createdItemsList.Add(new PMPattern(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMFeatureSet)))
            {
                foreach (string name in _featureSets.ReadFeatureSets())
                {
                    if (_featureSets[name] == null)
                    {
                        createdItemsList.Add(new PMFeatureSet(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMFeatureGroup)))
            {                 
                foreach (string name in _featureGroups.ReadFeatureGroups())
                {
                    if (_featureGroups[name] == null)
                    {
                        createdItemsList.Add(new PMFeatureGroup(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMWorkplane)))
            {
                foreach (string name in _workplanes.ReadWorkplanes())
                {
                    if (_workplanes[name] == null)
                    {
                        createdItemsList.Add(new PMWorkplane(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMLevelOrSet)))
            {
                foreach (string name in _levelsOrSets.ReadLevelsAndSets())
                {
                    if (_levelsOrSets[name] == null)
                    {
                        createdItemsList.Add(new PMLevelOrSet(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMModel)))
            {
                foreach (string name in _models.ReadModels())
                {
                    if (_models[name] == null)
                    {
                        createdItemsList.Add(new PMModel(_powerMILL, name));
                    }
                }
            }
            if (type == null || ReferenceEquals(type, typeof(PMStockModel)))
            {
                foreach (string name in _stockModels.ReadStockModels())
                {
                    if (_stockModels[name] == null)
                    {
                        createdItemsList.Add(new PMStockModel(_powerMILL, name));
                    }
                }
            }
            return createdItemsList;
        }

        /// <summary>
        /// Imports the specified PowerMILL Template File.
        /// </summary>
        /// <param name="templateFile">The template file to be imported</param>
        /// <exception cref="Exception">If the file does not exist then an exception is thrown.</exception>
        public void ImportTemplateFile(File templateFile)
        {
            if (templateFile.Exists)
            {
                _powerMILL.DoCommand("IMPORT TEMPLATE PROJECT \"" + templateFile.Path + "\"");
                Refresh();
            }
            else
            {
                throw new Exception("Template File does not exist");
            }
        }

        /// <summary>
        /// Saves the project. The project must have been opened from a directory or previously saved using
        /// the SaveAs operation otherwise an exception will be thrown. Saves currently loaded project in Powermill.
        /// </summary>
        public void Save()
        {
            if (_projectDirectory == null)
            {
                throw new Exception("Project not previously saved.  Use SaveAs before using Save.");
            }
            if (_projectDirectory.Exists == false)
            {
                SaveAs(_projectDirectory);

                //doesnt work if project has already been saved.
            }
            else
            {
                _powerMILL.DoCommand("PROJECT SAVE");

                //save existing project
            }

            //SaveAs(_projectDirectory) 'doesnt work if project has already been saved.
        }

        /// <summary>
        /// Saves the project to the specified directory.
        /// </summary>
        /// <param name="projectDirectory">The directory to save the project to.</param>
        public void SaveAs(Directory projectDirectory)
        {
            _projectDirectory = projectDirectory;
            _powerMILL.DoCommand("PROJECT SAVE AS '" + _projectDirectory.Path + "'");
        }

        #endregion

        #region Friend Operations

        /// <summary>
        /// Adds the PMEntity to its collection type.
        /// </summary>
        /// <param name="addItem">The PMEntity to add.</param>
        internal void AddEntityToCollection(PMEntity addItem)
        {
            if (addItem.Identifier == PMNCProgram.NC_PROGRAM_IDENTIFIER)
            {
                //find position in list and add it
                List<string> newList = _NCPrograms.ReadNCPrograms();
                int idx = 0;
                int ipos = -1;
                for (idx = 0; idx <= newList.Count - 1; idx++)
                {
                    if (newList[idx] == addItem.Name)
                    {
                        ipos = idx;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
                if (ipos > -1)
                {
                    if (ipos >= _NCPrograms.Count)
                    {
                        _NCPrograms.Add((PMNCProgram) addItem);
                    }
                    else
                    {
                        _NCPrograms.Insert(ipos, (PMNCProgram) addItem);
                    }
                }
                else
                {
                    throw new ApplicationException("item not found in NC Program List");
                }
            }
            else if (addItem.Identifier == PMBoundary.BOUNDARY_IDENTIFIER)
            {
                _boundaries.Add((PMBoundary) addItem);
            }
            else if (addItem.Identifier == PMFeatureSet.FEATURESET_IDENTIFIER)
            {
                _featureSets.Add((PMFeatureSet) addItem);
            }
            else if (addItem.Identifier == PMGroup.GROUP_IDENTIFIER)
            {
                _groups.Add((PMGroup) addItem);
            }
            else if (addItem.Identifier == PMLevelOrSet.LEVEL_OR_SET_IDENTIFIER)
            {
                _levelsOrSets.Add((PMLevelOrSet) addItem);
            }
            else if (addItem.Identifier == PMModel.MODEL_IDENTIFIER)
            {
                _models.Add((PMModel) addItem);
            }
            else if (addItem.Identifier == PMPattern.PATTERN_IDENTIFIER)
            {
                _patterns.Add((PMPattern) addItem);
            }
            else if (addItem.Identifier == PMStockModel.STOCKMODEL_IDENTIFIER)
            {
                _stockModels.Add((PMStockModel) addItem);
            }
            else if (addItem.Identifier == PMToolpath.TOOLPATH_IDENTIFIER)
            {
                _toolpaths.Add((PMToolpath) addItem);
            }
            else if (addItem.Identifier == PMTool.TOOL_IDENTIFIER)
            {
                _tools.Add((PMTool) addItem);
            }
            else if (addItem.Identifier == PMWorkplane.WORKPLANE_IDENTIFIER)
            {
                _workplanes.Add((PMWorkplane) addItem);
            }
            else if (addItem.Identifier == PMMachineTool.MACHINE_TOOL_IDENTIFIER)
            {
                _machineTools.Add((PMMachineTool) addItem);
            }
            else
            {
                throw new ApplicationException("Identifier not supported");
            }
        }

        /// <summary>
        /// Removes the PMEntity from its collection. It also removes from PowerMill.
        /// </summary>
        /// <param name="itemToRemove">The PMEntity to remove.</param>
        internal void RemoveEntityFromCollection(PMEntity itemToRemove)
        {
            if (itemToRemove.Identifier == PMNCProgram.NC_PROGRAM_IDENTIFIER)
            {
                _NCPrograms.Remove((PMNCProgram) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMBoundary.BOUNDARY_IDENTIFIER)
            {
                _boundaries.Remove((PMBoundary) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMFeatureSet.FEATURESET_IDENTIFIER)
            {
                _featureSets.Remove((PMFeatureSet) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMGroup.GROUP_IDENTIFIER)
            {
                _groups.Remove((PMGroup) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMLevelOrSet.LEVEL_OR_SET_IDENTIFIER)
            {
                _levelsOrSets.Remove((PMLevelOrSet) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMModel.MODEL_IDENTIFIER)
            {
                _models.Remove((PMModel) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMPattern.PATTERN_IDENTIFIER)
            {
                _patterns.Remove((PMPattern) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMStockModel.STOCKMODEL_IDENTIFIER)
            {
                _stockModels.Remove((PMStockModel) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMToolpath.TOOLPATH_IDENTIFIER)
            {
                _toolpaths.Remove((PMToolpath) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMTool.TOOL_IDENTIFIER)
            {
                _tools.Remove((PMTool) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMWorkplane.WORKPLANE_IDENTIFIER)
            {
                _workplanes.Remove((PMWorkplane) itemToRemove);
            }
            else if (itemToRemove.Identifier == PMMachineTool.MACHINE_TOOL_IDENTIFIER)
            {
                _machineTools.Remove((PMMachineTool) itemToRemove);
            }
            else
            {
                throw new ApplicationException("Identifier not supported");
            }
        }

        #endregion

        #region Private Operations

        /// <summary>
        /// Refreshes the project collections with the 'entitiesToRefreshFrom' list.
        /// If 'entitiesToRefreshFrom' has new entities they will be added to the project collection.
        /// If 'entitiesToRefresh' has entities that don't exist in 'entitiesToRefreshFrom' they will be removed.
        /// </summary>
        /// <param name="entitiesToRefresh">All PMEntities in the project.</param>
        /// <param name="entitiesToRefreshFrom">All the names of the entities that must exist on the project collections.</param>
        /// <param name="type">The type of the entity.</param>
        private void RefreshEntities(List<PMEntity> entitiesToRefresh, List<string> entitiesToRefreshFrom, string type)
        {
            var currentEntities = new List<PMEntity>();
            foreach (string name in entitiesToRefreshFrom)
            {
                var originalEntity =
                    entitiesToRefresh.SingleOrDefault(x => x.Name.ToUpperInvariant() == name.ToUpperInvariant());
                if (originalEntity != null)
                {
                    //Entity is already in the project collections
                    currentEntities.Add(originalEntity);
                }
                else
                {
                    PMEntity entity = null;
                    entity = PMEntityFactory.CreateEntity(_powerMILL, type, name);
                    AddEntityToCollection(entity);
                    currentEntities.Add(entity);
                }
            }

            //Remove old entities from the project
            var entitiesToDelete = entitiesToRefresh.Except(currentEntities).ToList();
            foreach (PMEntity entity in entitiesToDelete)
            {
                RemoveEntityFromCollection(entity);
            }
        }

        #endregion
    }
}