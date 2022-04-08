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
using System.Threading;
using Autodesk.FileSystem;
using Autodesk.Geometry;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// Captures a Model in PowerSHAPE.
    /// </summary>
    public class PSModel : PSDatabaseEntity
    {
        #region " Fields "

        // Entities collections
        /// <summary>
        /// .
        /// The List of Annotations in the Model.
        /// </summary>
        protected PSAnnotationsCollection _annotationsCollection;

        /// <summary>
        /// The List of Arcs in the Model.
        /// </summary>
        protected PSArcsCollection _arcsCollection;

        /// <summary>
        /// The List of CompCurves in the Model.
        /// </summary>
        protected PSCompCurvesCollection _compCurvesCollection;

        /// <summary>
        /// The List of Curves in the Model.
        /// </summary>
        protected PSCurvesCollection _curvesCollection;

        /// <summary>
        /// The List of Electrodes in the Model.
        /// </summary>
        protected PSElectrodesCollection _electrodesCollection;

        /// <summary>
        /// The List of Lines in the Model.
        /// </summary>
        protected PSLinesCollection _linesCollection;

        /// <summary>
        /// The List of Meshes in the Model.
        /// </summary>
        protected PSMeshesCollection _meshesCollection;

        /// <summary>
        /// The List of Points in the Model.
        /// </summary>
        protected PSPointsCollection _pointsCollection;

        /// <summary>
        /// .
        /// The List of Solids in the Model.
        /// </summary>
        protected PSSolidsCollection _solidsCollection;

        /// <summary>
        /// The List of Surfaces in the Model.
        /// </summary>
        protected PSSurfacesCollection _surfacesCollection;

        /// <summary>
        /// The List of Workplanes in the Model.
        /// </summary>
        protected PSWorkplanesCollection _workplanesCollection;

        // Other collections
        /// <summary>
        /// The List of Levels in the Model.
        /// </summary>
        protected PSLevelsCollection _levelsCollection;

        /// <summary>
        /// The List of Materials in the Model.
        /// </summary>
        protected PSMaterialsCollection _materialsCollection;

        // Other fields
        /// <summary>
        /// This flag indicates that the Lists of items are out of date and need to be rebuilt.
        /// </summary>
        protected bool _isListsDirty;

        /// <summary>
        /// This flag indicates whether the model is currently in surface comparison mode.
        /// </summary>
        private bool _isInSurfaceComparison;

        /// <summary>
        /// This is the window that's holding the model.
        /// </summary>
        private PSWindow _window;

        /// <summary>
        /// This is the temmporary workplane of the model.  It only exists when explicitly created.
        /// </summary>
        private PSWorkplane _temporaryWorkplane;

        #endregion

        #region " Constructors "

        /// <summary>
        /// This operation initialises a Model.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape.</param>
        /// <param name="id">The model id.</param>
        /// <param name="name">The model name.</param>
        /// <remarks></remarks>
        internal PSModel(PSAutomation powerSHAPE, int id, string name, PSWindow window) : base(powerSHAPE)
        {
            _id = id;
            _name = name;

            // Add the created window to the Windows collection
            _window = window;

            _isListsDirty = true;
        }

        /// <summary>
        /// This constructor creates a new Empty Model.
        /// </summary>
        /// <param name="powerSHAPE">The base instance to interact with PowerShape</param>
        internal PSModel(PSAutomation powerSHAPE) : base(powerSHAPE)
        {
            // Create the new model in PowerSHAPE, turning new model recovery off
            _powerSHAPE.DoCommand("NEWMODEL_RECOVERY OFF");
            _powerSHAPE.DoCommand("FILE NEW");

            // Add the created window to the Windows collection
            _window = new PSWindow(_powerSHAPE, Convert.ToInt32(_powerSHAPE.DoCommandEx("WINDOW.SELECTED")));
            _powerSHAPE.Windows.Add(_window);

            // Get the name and id of the most recent model (ie. the one just created)
            _name = _powerSHAPE.ReadStringValue("WINDOW[" + _window.Name + "].MODEL");
            _id = _powerSHAPE.ReadIntValue("MODEL['" + Name + "'].ID");

            _isListsDirty = true;
        }

        /// <summary>
        /// This constructor will create a model from an existing PowerSHAPE model file.
        /// </summary>
        /// <param name="powerSHAPE">This is the PowerSHAPE Automation object.</param>
        /// <param name="file">The file name of the PowerSHAPE model to open.</param>
        internal PSModel(PSAutomation powerSHAPE, File file, bool openReadOnly) : base(powerSHAPE)
        {
            // Check that the file exists
            if (file.Exists == false)
            {
                throw new Exception("File does not exist");
            }

            // Clear the Selected and Created items
            ClearSelectedItems();
            ClearCreatedItems();

            var previousNumberOfWindows = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");

            var attempts = 0;
            var opened = false;

            powerSHAPE.DialogsOff();

            while ((attempts < 3) & (opened == false))
            {
                _powerSHAPE.DoCommand("FILE OPENPATH", "PATHNAME " + file.Path);
                if (openReadOnly)
                {
                    _powerSHAPE.DoCommand("ACCESS READONLY");
                }
                _powerSHAPE.DoCommand("ACCEPT");

                if (_powerSHAPE.Version >= new Version("12.0"))
                {
                    // Close file doctor
                    _powerSHAPE.DoCommand("CANCEL");
                }

                var newNumberOfWindows = _powerSHAPE.ReadIntValue("WINDOW.NUMBER");

                opened = newNumberOfWindows == previousNumberOfWindows + 1;
                attempts += 1;
                if (opened == false)
                {
                    Thread.Sleep(500);
                }
            }

            powerSHAPE.DialogsOff();

            if (opened == false)
            {
                throw new Exception("Failed to open model");
            }

            _isListsDirty = true;

            // Add the created window to the Windows collection
            _window = new PSWindow(_powerSHAPE, Convert.ToInt32(_powerSHAPE.DoCommandEx("WINDOW.SELECTED")));
            _powerSHAPE.Windows.Add(_window);

            // Get the name and id of the most recent model (ie. the one just created)
            _name = _powerSHAPE.ReadStringValue("WINDOW[" + _window.Name + "].MODEL");
            _id = _powerSHAPE.ReadIntValue("MODEL['" + Name + "'].ID");
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// Gets the identifer for models within PowerSHAPE.
        /// </summary>
        internal override string Identifier
        {
            get { return "MODEL"; }
        }

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets whether the model is in metric or imperial based on the length units of the model.
        /// Sets the model to either BSI (Metric) or ANSI (Imperial)
        /// </summary>
        public LengthUnits Units
        {
            get
            {
                //' Read the current units from PowerShape
                string strUnits = _powerSHAPE.ReadStringValue("unit[length].name");
                if (strUnits.ToLower() == "inches")
                {
                    return LengthUnits.Inches;
                }
                return LengthUnits.MM;
            }

            set
            {
                if (Units == value)
                {
                    // Units are correct. Do Nothing
                }
                else if (Units == LengthUnits.Inches)
                {
                    // Get existing general tolerance
                    Inch tolerance = GeneralTolerance;

                    // Swap units from inches to mm
                    _powerSHAPE.DoCommand("tools preferences unitprefs standard BSI accept");

                    // There is an issue in PowerShape 2018.0.0.18110 where 'Tolerance 0.001'
                    // Does not work immediately after changing the units from ANSI to BSI
                    // Opening and closing the dialog (even with dialogs off) and it works again
                    _powerSHAPE.DoCommand("bsv close tools preferences", "accept");
                    GeneralTolerance = ((MM) tolerance).Value;
                }
                else
                {
                    // Get existing general tolerance
                    MM generalTolerance = GeneralTolerance;

                    // Swap units from mm to inches
                    _powerSHAPE.DoCommand("tools preferences unitprefs standard ANSI accept");
                    GeneralTolerance = ((Inch) generalTolerance).Value;
                }
            }
        }

        /// <summary>
        /// Gets the window to which this model is attached.
        /// </summary>
        public PSWindow Window
        {
            get
            {
                if (_window != null)
                {
                    foreach (PSWindow openWindow in _powerSHAPE.Windows)
                    {
                        if ((openWindow.AttachedProcessName == _name) & (openWindow.Type == WindowTypes.MODEL))
                        {
                            _window = openWindow;
                        }
                    }
                }

                return _window;
            }
        }

        /// <summary>
        /// Gets whether the Model is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _powerSHAPE.ReadIntValue("MODEL[ID " + _id + "].SELECTED") == 1; }
        }

        /// <summary>
        /// Gets the path to the Model file.
        /// </summary>
        public string Path
        {
            get { return _powerSHAPE.ReadStringValue("MODEL[ID " + _id + "].PATH"); }
        }

        /// <summary>
        /// Gets whether the Model is corrupt.
        /// </summary>
        public bool IsCorrupt
        {
            get { return _powerSHAPE.ReadIntValue("MODEL[ID " + _id + "].CORRUPT") == 1; }
        }

        /// <summary>
        /// Gets the collection of Annotations in the Model.
        /// </summary>
        public PSAnnotationsCollection Annotations
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _annotationsCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Arcs in the Model.
        /// </summary>
        public PSArcsCollection Arcs
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _arcsCollection;
            }
        }

        /// <summary>
        /// Gets the Collection of CompCurves in the Model.
        /// </summary>
        public PSCompCurvesCollection CompCurves
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _compCurvesCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Curves in the Model.
        /// </summary>
        public PSCurvesCollection Curves
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _curvesCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Electrodes in the Model.
        /// </summary>
        public PSElectrodesCollection Electrodes
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _electrodesCollection;
            }
        }

        /// <summary>
        /// Gets the Collection of Lines in the Model.
        /// </summary>
        public PSLinesCollection Lines
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _linesCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Meshes in the model.
        /// </summary>
        public PSMeshesCollection Meshes
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _meshesCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Points in the model.
        /// </summary>
        public PSPointsCollection Points
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _pointsCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Solids in the model.
        /// </summary>
        public PSSolidsCollection Solids
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _solidsCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Surfaces in the model.
        /// </summary>
        public PSSurfacesCollection Surfaces
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _surfacesCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Workplanes in the Model.
        /// </summary>
        public PSWorkplanesCollection Workplanes
        {
            get
            {
                if (_isListsDirty)
                {
                    Initialise();
                }
                return _workplanesCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Levels in the model.
        /// </summary>
        public PSLevelsCollection Levels
        {
            get
            {
                if (_levelsCollection == null || _levelsCollection.Count == 0)
                {
                    _levelsCollection.ReadLevels();
                }
                return _levelsCollection;
            }
        }

        /// <summary>
        /// Gets the collection of Materials in the model.
        /// </summary>
        public PSMaterialsCollection Materials
        {
            get
            {
                if (_materialsCollection == null || _materialsCollection.Count == 0)
                {
                    // TODO: Should be able to read the list of materials
                    //_materialsCollection.ReadMaterials()
                }
                return _materialsCollection;
            }
        }

        /// <summary>
        /// Get the selected items in the model.
        /// </summary>
        public List<PSEntity> SelectedItems
        {
            get
            {
                if (_powerSHAPE.Version != null && _powerSHAPE.Version <= new Version("16.1.29"))
                {
                    // Read the number of selected items
                    int numberSelected = _powerSHAPE.ReadIntValue("SELECTION.NUMBER");

                    List<PSEntity> items = new List<PSEntity>();

                    // Populate the List with the selected items
                    for (int index = 0; index <= numberSelected - 1; index++)
                    {
                        string itemType = _powerSHAPE.ReadStringValue("SELECTION.TYPE[" + index + "]");
                        var itemId = _powerSHAPE.ReadIntValue("SELECTION.OBJECT[" + index + "].ID");
                        string itemName = _powerSHAPE.ReadStringValue("SELECTION.NAME[" + index + "]");

                        PSEntity entity = null;
                        if (TryGetEntityFromCache(ref entity, itemId, itemName, itemType))
                        {
                            items.Add(entity);
                        }
                        else
                        {
                            items.Add((PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, itemType, itemId, itemName));
                        }
                    }

                    // Return the items
                    return items;
                }
                else
                {
                    List<PSEntity> items = new List<PSEntity>();

                    object[] selection = _powerSHAPE.ActiveDocument.GetSelectedItems();

                    if (selection != null)
                    {
                        object[] types = null;
                        object[] ids = null;
                        object[] names = null;
                        object[] descriptions = null;
                        object[] levels = null;
                        _powerSHAPE.ActiveDocument.GetItemData(selection,
                                                               out types,
                                                               out ids,
                                                               out names,
                                                               out descriptions,
                                                               out levels);

                        for (var i = 0; i <= selection.Length - 1; i++)
                        {
                            // We wrap this in a try/catch as we do not want the whole function to fail
                            // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                            try
                            {
                                // Get the object using standard entity factory
                                PSEntity entity =
                                    (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE,
                                                                            int.Parse(types[i].ToString()),
                                                                            int.Parse(ids[i].ToString()),
                                                                            names[i].ToString(),
                                                                            descriptions[i].ToString(),
                                                                            levels[i].ToString());

                                // Add the entity to the items to return
                                items.Add(entity);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    // Return the items
                    return items;
                }
            }
        }

        /// <summary>
        /// Gets the BoundingBox of the selection in PowerSHAPE.  If no items are currently selected then the return value is null
        /// </summary>
        /// <returns></returns>
        public BoundingBox BoundingBoxOfSelectedItems
        {
            get
            {
                int numberSelected = _powerSHAPE.ReadIntValue("SELECTION.NUMBER");

                if (numberSelected == 0)
                {
                    return null;
                }

                double[] minSize = _powerSHAPE.DoCommandEx("SELECTION.MIN_RANGE_EXACT") as double[];
                double[] maxSize = _powerSHAPE.DoCommandEx("SELECTION.MAX_RANGE_EXACT") as double[];

                return new BoundingBox(minSize[0], maxSize[0], minSize[1], maxSize[1], minSize[2], maxSize[2]);
            }
        }

        /// <summary>
        /// Gets the created items in the model.
        /// </summary>
        public List<PSEntity> CreatedItems
        {
            get
            {
                // Read the number of created items
                int numberCreated = 0;
                numberCreated = _powerSHAPE.ReadIntValue("CREATED.NUMBER");

                List<PSEntity> items = new List<PSEntity>();

                // Populate the arraylist with the created items
                for (int index = 0; index <= numberCreated - 1; index++)
                {
                    string itemType = _powerSHAPE.ReadStringValue("CREATED.TYPE[" + index + "]");
                    string itemName = _powerSHAPE.ReadStringValue("CREATED.NAME[" + index + "]");
                    int itemId;
                    if (itemType.ToUpper() == PSWorkplane.WORKPLANE_IDENTIFIER)
                    {
                        itemId = _powerSHAPE.ReadIntValue("WORKPLANE['" + itemName + "'].ID");
                    }
                    else
                    {
                        itemId = _powerSHAPE.ReadIntValue("CREATED.OBJECT[" + index + "].ID");
                    }

                    PSEntity entity = null;
                    if (TryGetEntityFromCache(ref entity, itemId, itemName, itemType))
                    {
                        items.Add(entity);
                    }
                    else
                    {
                        items.Add((PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, itemType, itemId, itemName));
                    }
                }

                // Return the items
                return items;
            }
        }

        /// <summary>
        /// Gets the updated items in the model.
        /// </summary>
        public List<PSEntity> UpdatedItems
        {
            get
            {
                // Read the number of updated items
                int numberUpdated = 0;
                numberUpdated = _powerSHAPE.ReadIntValue("UPDATED.NUMBER");

                List<PSEntity> items = new List<PSEntity>();

                //Populate the arraylist with the updated items
                for (int index = 0; index <= numberUpdated - 1; index++)
                {
                    string itemType = _powerSHAPE.ReadStringValue("UPDATED.TYPE[" + index + "]");
                    int itemId = _powerSHAPE.ReadIntValue("UPDATED.OBJECT[" + index + "].ID");
                    string itemName = _powerSHAPE.ReadStringValue("UPDATED.NAME[" + index + "]");

                    PSEntity entity = null;
                    if (TryGetEntityFromCache(ref entity, itemId, itemName, itemType))
                    {
                        items.Add(entity);
                    }
                    else
                    {
                        items.Add((PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, itemType, itemId, itemName));
                    }
                }

                // Return the items
                return items;
            }
        }

        /// <summary>
        /// The currently active Levels.
        /// </summary>
        /// <returns>The list of the active Levels.</returns>
        public List<PSLevel> ActiveLevels
        {
            get
            {
                bool levelActiveFilterOn = IsLevelActiveFilterOn;
                bool levelNamedFilterOn = IsLevelNamedFilterOn;
                bool levelUsedFilterOn = IsLevelUsedFilterOn;

                // Switch filters to be as required
                IsLevelActiveFilterOn = true;
                IsLevelNamedFilterOn = false;
                IsLevelUsedFilterOn = false;

                // Get number of filtered levels
                int numbFilteredLevels = _powerSHAPE.ReadIntValue("LEVEL.FILTERED.NUMBER");

                // Return a list containing each active level
                List<PSLevel> activeLevelsToReturn = new List<PSLevel>();
                for (int i = 0; i <= numbFilteredLevels - 1; i++)
                {
                    int levelNumber = _powerSHAPE.ReadIntValue("LEVEL.FILTERED[" + i + "].INDEX");
                    activeLevelsToReturn.Add(new PSLevel(_powerSHAPE, levelNumber));
                }

                // Return filters to their previous states
                IsLevelActiveFilterOn = levelActiveFilterOn;
                IsLevelNamedFilterOn = levelNamedFilterOn;
                IsLevelUsedFilterOn = levelUsedFilterOn;

                return activeLevelsToReturn;
            }
        }

        /// <summary>
        /// The currently used Levels.
        /// </summary>
        public List<PSLevel> UsedLevels
        {
            get
            {
                bool levelActiveFilterOn = IsLevelActiveFilterOn;
                bool levelNamedFilterOn = IsLevelNamedFilterOn;
                bool levelUsedFilterOn = IsLevelUsedFilterOn;

                // Switch filters to be as required
                IsLevelActiveFilterOn = false;
                IsLevelNamedFilterOn = false;
                IsLevelUsedFilterOn = true;

                // Get number of filtered levels
                int numbFilteredLevels = _powerSHAPE.ReadIntValue("LEVEL.FILTERED.NUMBER");

                // Return a list containing each active level
                List<PSLevel> usedLevelsToReturn = new List<PSLevel>();
                for (int i = 0; i <= numbFilteredLevels - 1; i++)
                {
                    int levelNumber = _powerSHAPE.ReadIntValue("LEVEL.FILTERED[" + i + "].INDEX");
                    usedLevelsToReturn.Add(new PSLevel(_powerSHAPE, levelNumber));
                }

                // Return filters to their previous states
                IsLevelActiveFilterOn = levelActiveFilterOn;
                IsLevelNamedFilterOn = levelNamedFilterOn;
                IsLevelUsedFilterOn = levelUsedFilterOn;

                return usedLevelsToReturn;
            }
        }

        /// <summary>
        /// The currently named Levels.
        /// </summary>
        public List<PSLevel> NamedLevels
        {
            get
            {
                bool levelActiveFilterOn = IsLevelActiveFilterOn;
                bool levelNamedFilterOn = IsLevelNamedFilterOn;
                bool levelUsedFilterOn = IsLevelUsedFilterOn;

                // Switch filters to be as required
                IsLevelActiveFilterOn = false;
                IsLevelNamedFilterOn = true;
                IsLevelUsedFilterOn = false;

                // Get number of filtered levels
                int numbFilteredLevels = _powerSHAPE.ReadIntValue("LEVEL.FILTERED.NUMBER");

                // Return a list containing each active level
                List<PSLevel> namedLevelsToReturn = new List<PSLevel>();
                for (int i = 0; i <= numbFilteredLevels - 1; i++)
                {
                    int levelNumber = _powerSHAPE.ReadIntValue("LEVEL.FILTERED[" + i + "].INDEX");
                    namedLevelsToReturn.Add(new PSLevel(_powerSHAPE, levelNumber));
                }

                // Return filters to their previous states
                IsLevelActiveFilterOn = levelActiveFilterOn;
                IsLevelNamedFilterOn = levelNamedFilterOn;
                IsLevelUsedFilterOn = levelUsedFilterOn;

                return namedLevelsToReturn;
            }
        }

        /// <summary>
        /// Gets and sets the used filter when interrogating Levels.
        /// </summary>
        /// <returns>The status of the used filter.</returns>
        public bool IsLevelUsedFilterOn
        {
// Read the status of the user level filter
            get { return _powerSHAPE.ReadBoolValue("LEVEL.FILTERED.USED"); }
            set
            {
                // Define the setting status
                string filterSet = "ON";

                // Switch the setting status if user wishes to switch filter off 
                if (value == false)
                {
                    filterSet = "OFF";
                }

                // Switch the filter
                _powerSHAPE.DoCommand("LEVEL FILTER USED_" + filterSet);
            }
        }

        /// <summary>
        /// Gets and sets the named filter when interrogating Levels.
        /// </summary>
        /// <returns>The status of the named filter.</returns>
        public bool IsLevelNamedFilterOn
        {
// Read the status of the user level filter
            get { return _powerSHAPE.ReadBoolValue("LEVEL.FILTERED.NAMED"); }
            set
            {
                // Define the setting status
                string filterSet = "ON";

                // Switch the setting status if user wishes to switch filter off 
                if (value == false)
                {
                    filterSet = "OFF";
                }

                // Switch the filter
                _powerSHAPE.DoCommand("LEVEL FILTER NAMED_" + filterSet);
            }
        }

        /// <summary>
        /// Gets and sets the active filter when interrogating Levels.
        /// </summary>
        /// <returns>The status of the active filter.</returns>
        public bool IsLevelActiveFilterOn
        {
// Read the status of the user level filter
            get { return _powerSHAPE.ReadBoolValue("LEVEL.FILTERED.ON"); }
            set
            {
                // Define the setting status
                string filterSet = "ON";

                // Switch the setting status if user wishes to switch filter off 
                if (value == false)
                {
                    filterSet = "OFF";
                }

                // Switch the filter
                _powerSHAPE.DoCommand("LEVEL FILTER ON_" + filterSet);
            }
        }

        /// <summary>
        /// Returns the currently active Workplane or Nothing if World is active.
        /// </summary>
        /// <returns>The Active Workplane or Nothing if World is the Active Workplane.</returns>
        public PSWorkplane ActiveWorkplane
        {
            get
            {
                int activeWorkplaneId = _powerSHAPE.ActiveDocument.ActiveWorkplane;
                if (activeWorkplaneId > 0)
                {
                    return Workplanes.FirstOrDefault(workplane => workplane.CompositeID == activeWorkplaneId);
                }

                // If no workplane is active (ie. World is active) then return Nothing
                return null;
            }

            set
            {
                if (value == null)
                {
                    // Set world workplane active
                    _powerSHAPE.DoCommand("ACTIVATE_WORKPLANE World");
                }
                else if (value.Exists)
                {
                    // Activate the selected workplane
                    _powerSHAPE.DoCommand("ACTIVATE_WORKPLANE " + value.Name);
                }
                else
                {
                    throw new Exception("Unable to activate workplane");
                }
            }
        }

        /// <summary>
        /// The model's temporary workplane
        /// </summary>
        public PSWorkplane TemporaryWorkplane
        {
            get { return _temporaryWorkplane; }
            set { _temporaryWorkplane = value; }
        }

        /// <summary>
        /// This property returns whether the model is in surface comparison mode
        /// </summary>
        private bool IsInSurfaceComparison
        {
            get { return _isInSurfaceComparison; }
        }

        /// <summary>
        /// Gets and sets the general tolerance of the model.
        /// </summary>
        public double GeneralTolerance
        {
            get { return _powerSHAPE.ReadDoubleValue("TOLERANCE.GENERAL"); }
            set { _powerSHAPE.DoCommand("TOLERANCE " + value); }
        }

        /// <summary>
        /// Gets whether the model is open readonly or not
        /// </summary>
        public bool IsReadOnly
        {
            get { return _powerSHAPE.ReadBoolValue("MODEL.READONLY"); }
        }

        #endregion

        #region " Model Operations "

        /// <summary>
        /// Closes this model
        /// </summary>
        public void Delete()
        {
            _powerSHAPE.Models.Remove(this);
        }

        /// <summary>
        /// Sets an undo marker in PowerSHAPE.
        /// </summary>
        public void SetUndoMarker()
        {
            dynamic objDOC = _powerSHAPE.ActiveDocument;
            objDOC.SetUndoMarker(1, "Dummy");
        }

        /// <summary>
        /// Performs an undo in PowerSHAPE.
        /// </summary>
        public void Undo()
        {
            _powerSHAPE.DoCommand("UNDO");
        }

        /// <summary>
        /// Performs a redo in PowerSHAPE.
        /// </summary>
        public void Redo()
        {
            _powerSHAPE.DoCommand("REDO");
        }

        /// <summary>
        /// Clears the list of selected items.
        /// </summary>
        public void ClearSelectedItems()
        {
            _powerSHAPE.DoCommand("SELECT CLEARLIST");
        }

        /// <summary>
        /// Clears the list of created items.
        /// </summary>
        public void ClearCreatedItems()
        {
            // This should be a DoCommandEx function
            _powerSHAPE.DoCommandEx("CREATED.CLEARLIST");
        }

        /// <summary>
        /// Clears the list of updated items.
        /// </summary>
        public void ClearUpdatedItems()
        {
            _powerSHAPE.DoCommand("UPDATED.CLEARLIST");
        }

        /// <summary>
        /// Selects multiple entities in as few commands as possible.
        /// </summary>
        /// <param name="entities">The entities to select.</param>
        /// <param name="emptySelectionFirst">If True, empties the selection first.</param>
        /// <remarks></remarks>
        public void SelectEntities(IEnumerable<PSEntity> entities, bool emptySelectionFirst = false)
        {
            if (emptySelectionFirst)
            {
                ClearSelectedItems();
            }

            if (entities.Count() == 0)
            {
                return;
            }

            var numberOfEntitiesInCommand = 0;
            string command = "";
            foreach (PSEntity entity in entities)
            {
                string commandToAppend = "ADD " + entity.Identifier + " '" + entity.Name + "' ";

                // Command has a character limit of 1024, and must not reference 34 or more entities otherwise Powershape enters a funny state (pshape#28740).
                if ((command.Length + commandToAppend.Length < 500) & (numberOfEntitiesInCommand + 1 < 30))
                {
                    command += commandToAppend;
                    numberOfEntitiesInCommand += 1;
                }
                else
                {
                    _powerSHAPE.DoCommand(command);
                    command = commandToAppend;
                    numberOfEntitiesInCommand = 1;
                }
            }
            _powerSHAPE.DoCommand(command);
        }

        /// <summary>
        /// Selects everything in the Model that is currently visible.
        /// </summary>
        /// <param name="enableDeactivatedLevels">If true, switches filters to be as required.</param>
        /// <remarks></remarks>
        public void SelectAll(bool enableDeactivatedLevels = false)
        {
            if (enableDeactivatedLevels)
            {
                // Switch filters to be as required
                IsLevelActiveFilterOn = false;
                IsLevelNamedFilterOn = true;
                IsLevelUsedFilterOn = true;

                Levels.ActivateAllLevels();
            }

            _powerSHAPE.DoCommand("SELECTALL");
        }

        /// <summary>
        /// Deactivates the active solid (if there is one), preventing headaches when performing a feature operation.
        /// </summary>
        public void DeactivateAllSolids()
        {
            string activeSolidName = _powerSHAPE.ReadStringValue("SOLID.ACTIVE");

            int activeSolidExists = _powerSHAPE.ReadIntValue("SOLID[\"" + activeSolidName + "\"].EXISTS");
            if (activeSolidExists == 1)
            {
                ClearSelectedItems();
                _powerSHAPE.DoCommand("ADD SOLID \"" + activeSolidName + "\"");
                _powerSHAPE.DoCommand("MODIFY MODIFY DEACTIVATE ACCEPT");
                ClearSelectedItems();
            }
        }

        /// <summary>
        /// Refreshes the information available in the current model.
        /// Refresh should retain any existing items in the lists, add new ones and remove old ones.
        /// This function would be used by someone after they have used the "Execute" functionality.
        /// </summary>
        public void Refresh()
        {
            // If we have 2016 SP2 or greater then we can take advantage of the faster code
            if (_powerSHAPE.Version <= new Version("16.1.29"))
            {
                RefreshSlow();
            }
            else
            {
                RefreshFast();
            }
        }

        private void RefreshFast()
        {
            object[] allItems = _powerSHAPE.ActiveDocument.GetModelItems();

            var annotationsCollection = new List<PSAnnotation>();
            var arcsCollection = new List<PSArc>();
            var compCurvesCollection = new List<PSCompCurve>();
            var curvesCollection = new List<PSCurve>();
            var electrodesCollection = new List<PSElectrode>();
            var linesCollection = new List<PSLine>();
            var meshesCollection = new List<PSMesh>();
            var pointsCollection = new List<PSPoint>();
            var solidsCollection = new List<PSSolid>();
            var surfacesCollection = new List<PSSurface>();
            var workplanesCollection = new List<PSWorkplane>();

            if (allItems != null)
            {
                object[] types = null;
                object[] ids = null;
                object[] names = null;
                object[] descriptions = null;
                object[] levels = null;
                _powerSHAPE.ActiveDocument.GetItemData(allItems, out types, out ids, out names, out descriptions, out levels);

                for (var i = 0; i <= allItems.Length - 1; i++)
                {
                    // We wrap this in a try/catch as we do not want the whole function to fail
                    // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                    try
                    {
                        PSEntity cachedEntity = null;

                        // See if the item is already cached
                        if (!TryGetEntityFromCache(ref cachedEntity,
                                                   int.Parse(ids[i].ToString()),
                                                   names[i].ToString(),
                                                   int.Parse(types[i].ToString()),
                                                   descriptions[i].ToString()))
                        {
                            // Get the object using standard entity factory
                            PSEntity entity =
                                (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE,
                                                                        int.Parse(types[i].ToString()),
                                                                        int.Parse(ids[i].ToString()),
                                                                        names[i].ToString(),
                                                                        descriptions[i].ToString(),
                                                                        levels[i].ToString());

                            // Add the entity to the items to return
                            AddNoChecks(entity);
                            cachedEntity = entity;
                        }
                        else
                        {
                            cachedEntity.SetLevelNumber(int.Parse(levels[i].ToString()));
                        }
                        switch (int.Parse(types[i].ToString()))
                        {
                            case 61:
                                annotationsCollection.Add((PSAnnotation) cachedEntity);
                                break;
                            case 52:
                                arcsCollection.Add((PSArc) cachedEntity);
                                break;
                            case 59:
                                compCurvesCollection.Add((PSCompCurve) cachedEntity);
                                break;
                            case 53:
                                curvesCollection.Add((PSCurve) cachedEntity);
                                break;
                            case 58:
                                linesCollection.Add((PSLine) cachedEntity);
                                break;
                            case 49:
                                pointsCollection.Add((PSPoint) cachedEntity);
                                break;
                            case 40:
                                solidsCollection.Add((PSSolid) cachedEntity);
                                break;
                            case 60:
                                switch (descriptions[i].ToString())
                                {
                                    case "Mesh":
                                        meshesCollection.Add((PSMesh) cachedEntity);
                                        break;
                                    case "Point":
                                        pointsCollection.Add((PSPoint) cachedEntity);
                                        break;
                                    case "Electrode":
                                        electrodesCollection.Add((PSElectrode) cachedEntity);
                                        break;
                                }
                                break;
                            case 21:
                                surfacesCollection.Add((PSSurface) cachedEntity);
                                break;
                            case 54:
                                workplanesCollection.Add((PSWorkplane) cachedEntity);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            DeleteEntities(new List<PSEntity>(_annotationsCollection),
                           new List<PSEntity>(annotationsCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_arcsCollection.Cast<PSEntity>()),
                           new List<PSEntity>(arcsCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_compCurvesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(compCurvesCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_curvesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(curvesCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_electrodesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(electrodesCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_linesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(linesCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_meshesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(meshesCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_pointsCollection.Cast<PSEntity>()),
                           new List<PSEntity>(pointsCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_solidsCollection.Cast<PSEntity>()),
                           new List<PSEntity>(solidsCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_surfacesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(surfacesCollection.Cast<PSEntity>()));
            DeleteEntities(new List<PSEntity>(_workplanesCollection.Cast<PSEntity>()),
                           new List<PSEntity>(workplanesCollection.Cast<PSEntity>()));

            RefreshLevels();

            RefreshMaterials();
        }

        /// <summary>
        /// Deletes entities that are in the cache but no longer in PowerShape
        /// </summary>
        /// <param name="cachedEntities">Items currently in the cache</param>
        /// <param name="existingEntities">Items that are in PowerShape</param>
        private void DeleteEntities(List<PSEntity> cachedEntities, List<PSEntity> existingEntities)
        {
            var entitiesToDelete = cachedEntities.Except(existingEntities).ToList();
            foreach (PSEntity entity in entitiesToDelete)
            {
                Remove(entity);
            }
        }

        private void RefreshSlow()
        {
            _isListsDirty = false;

            // Get all the objects currently selected
            List<PSEntity> originalSelection = SelectedItems;

            // Get all the levels currently named/used
            List<PSLevel> originallyUsedNamedLevels = UsedLevels;
            originallyUsedNamedLevels.AddRange(NamedLevels);

            // Get all the levels that are currently active
            List<PSLevel> originallyActiveLevels = new List<PSLevel>();
            foreach (PSLevel levelToCheck in originallyUsedNamedLevels)
            {
                if (levelToCheck.IsActive)
                {
                    originallyActiveLevels.Add(levelToCheck);
                }
            }

            // Add new entities and delete removed entities
            RefreshEntities(new List<PSEntity>(_annotationsCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_arcsCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_compCurvesCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_curvesCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_electrodesCollection.Cast<PSElectrode>()));
            RefreshEntities(new List<PSEntity>(_linesCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_meshesCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_pointsCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_solidsCollection.Cast<PSEntity>()));
            RefreshEntities(new List<PSEntity>(_surfacesCollection.Cast<PSEntity>()));

            // Do same as RefreshEntities but this one is for workplanes
            RefreshWorkplanes();

            RefreshLevels();

            RefreshMaterials();

            // Restore previously active levels
            Levels.DeactivateAllLevels();
            foreach (PSLevel previouslyActiveLevel in originallyActiveLevels)
            {
                previouslyActiveLevel.IsActive = true;
            }

            // Restore previously selected items
            foreach (PSEntity previouslySelectedEntity in originalSelection)
            {
                previouslySelectedEntity.AddToSelection(false);
            }
        }

        /// <summary>
        /// Compares a surface/mesh in PowerSHAPE and displays any selection errors.
        /// </summary>
        /// <param name="surfaceToCompare">The surface to compare.</param>
        /// <param name="meshToCompare">The mesh to compare.</param>
        /// <remarks></remarks>
        public void StartSurfaceComparison(PSSurface surfaceToCompare, PSMesh meshToCompare)
        {
            // Select them both
            surfaceToCompare.AddToSelection(true);
            meshToCompare.AddToSelection();

            // Check that the selection contains just a surface and a mesh
            if (SelectedItems.Count != 2)
            {
                throw new Exception("One surface and one mesh must be selected.");
            }

            // Start comparison
            _powerSHAPE.DoCommand("COMPARISON AUTOMATIC");

            // Set the flag to be true
            _isInSurfaceComparison = true;
        }

        /// <summary>
        /// Outputs a text file containing the Surface Comparison errors.
        /// </summary>
        /// <param name="outputFile">The file path were errors will be written.</param>
        /// <remarks></remarks>
        public void ExportSurfaceComparisonErrorsFile(File outputFile)
        {
            // If the file doesn't already exist, PowerSHAPE throws an error, so create it first
            outputFile.Create();

            // Export file
            _powerSHAPE.DoCommand("EXPORTERRORS Filename FileDialog Write Validate " + outputFile.Path, "OK");
        }

        /// <summary>
        /// Compares two meshes in PowerSHAPE and displays any selection errors.
        /// </summary>
        /// <param name="meshToCompare">First mesh used in the comparison.</param>
        /// <param name="meshToCompareTo">Second mesh used in the comparison.</param>
        /// <remarks></remarks>
        public void StartSurfaceComparison(PSMesh meshToCompare, PSMesh meshToCompareTo)
        {
            // Select them both
            meshToCompare.AddToSelection(true);
            meshToCompareTo.AddToSelection();

            // Check that the selection contains two meshes
            if (SelectedItems.Count != 2)
            {
                throw new Exception("Two meshes must be selected.");
            }

            // Start comparison
            _powerSHAPE.DoCommand("COMPARISON AUTOMATIC");

            // Set the flag to be true
            _isInSurfaceComparison = true;
        }

        /// <summary>
        /// Operation exits surface comparison mode.
        /// </summary>
        public void EndSurfaceComparison()
        {
            if (_isInSurfaceComparison)
            {
                _powerSHAPE.DoCommand("CANCEL");
                _isInSurfaceComparison = false;
            }
        }

        /// <summary>
        /// Saves the current model.
        /// </summary>
        /// <param name="file">The file path to save the model.</param>
        public void Save(File file = null)
        {
            if (file == null)
            {
                // File not previously saved (i.e. model.readonly)
                var isReadOnly = _powerSHAPE.ReadIntValue("MODEL.READONLY");
                var isFileSaved = isReadOnly != 1;

                if (!isFileSaved)
                {
                    // No path provided
                    throw new ArgumentException("File path was not provided. Please insert a file path.");
                }
                _powerSHAPE.DoCommand("FILE SAVE");
            }
            else
            {
                // Ensure that file path extension is psmodel
                var fileEnsureExtension =
                    new File(string.Format("{0}{1}.psmodel", file.ParentDirectory.Path, file.NameWithoutExtension));

                if (fileEnsureExtension.Exists)
                {
                    // The name uniquely identifies a model, it is not possible to save a model with the same name even if we try to save them into different folders
                    var modelPreviousSaved =
                        _powerSHAPE.Models.SingleOrDefault(x => x.Name == fileEnsureExtension.NameWithoutExtension);

                    if (modelPreviousSaved == null)
                    {
                        // The file already exists but it does not match the name of any of the current models
                        // The user wants to overwrite the existing file with the selected model.
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Failed to save model.  Could not delete the existing psmodel file '" +
                                                file.Path + "'");
                        }
                    }
                    else
                    {
                        if (modelPreviousSaved.Id == Id)
                        {
                            //File given and ActiveModel saved a 2nd time
                            _powerSHAPE.DoCommand("FILE SAVE");
                            return;
                        }

                        // File given and ActiveModel saved to another open model file
                        _powerSHAPE.Models.Remove(fileEnsureExtension.NameWithoutExtension);
                        fileEnsureExtension.Delete();
                    }
                }

                _powerSHAPE.DoCommand("FILE SAVEASPATH " + fileEnsureExtension.Path);
                _name = _powerSHAPE.ActiveWindow.AttachedProcessName;
            }

            _id = _powerSHAPE.ReadIntValue("MODEL['" + Name + "'].ID");
        }

        /// <summary>
        /// This operation imports the desired file.
        /// </summary>
        /// <param name="file">The file to be imported.</param>
        public List<PSEntity> Import(File file)
        {
            // Check that the file exists
            if (!file.Exists)
            {
                throw new Exception("File does not exist");
            }

            // Clear the Selected and Created items
            ClearSelectedItems();
            ClearCreatedItems();

            // Import the file
            _powerSHAPE.DoCommand("FILE IMPORT \"" + file.Path + "\"", "DISMISS");

            List<PSEntity> importedItems = new List<PSEntity>();

            // Check that the file imported correctly and add entities to their collections if so
            // Items are usually selected after importing so use this first
            if (SelectedItems.Count == 0)
            {
                // They weren't selected so try and get the created items instead
                if (CreatedItems.Count == 0)
                {
                    throw new Exception("Failed to import model");
                }
                foreach (PSEntity entity in CreatedItems)
                {
                    Add(entity);
                    importedItems.Add(entity);
                }
            }
            else
            {
                foreach (PSEntity entity in SelectedItems)
                {
                    Add(entity);
                    importedItems.Add(entity);
                }
            }

            return importedItems;
        }

        /// <summary>
        /// Exports the selected geometry to create the desired file.
        /// </summary>
        /// <param name="exportFile">The file that will be created after exporting the geometry.</param>
        /// <param name="exportItemsOption">The model items to export.</param>
        /// <param name="exportWorkplaneOption">The workplane to export.</param>
        /// <remarks></remarks>
        public void Export(
            File exportFile,
            ExportItemsOptions exportItemsOption = ExportItemsOptions.All,
            ExportWorkplanes exportWorkplaneOption = ExportWorkplanes.World)
        {
            // If the user is exporting selected and nothing is selected then throw an exception
            if (exportItemsOption == ExportItemsOptions.Selected && SelectedItems.Count == 0)
            {
                throw new Exception("Nothing is selected.  No items to export.");
            }

            // If using pre-version 12, then visible is not an option
            if (exportItemsOption == ExportItemsOptions.Visible && _powerSHAPE.Version < new Version("12.0"))
            {
                throw new ArgumentException("Export visible is not valid in this version of PowerSHAPE");
            }

            if (_powerSHAPE.IsFormUpdateOff == false)
            {
                _powerSHAPE.DoCommand("FORMUPDATE OFF");
            }

            int numberOfDrawings = _powerSHAPE.ReadIntValue("DRAWING.NUMBER");

            // Export the whole model or the selected parts to a file
            _powerSHAPE.DoCommand("FILE EXPORT WIZARD", "TARGET UNKNOWN");

            // If using version 12 or greater then step through the wizard
            if (_powerSHAPE.Version >= new Version("12.0"))
            {
                _powerSHAPE.DoCommand("NEXT", exportFile.Path);

                if (numberOfDrawings != 0)
                {
                    // If there is a drawing then we have to call next again to skip over an extra page in the wizard
                    _powerSHAPE.DoCommand("NEXT");
                }

                switch (exportItemsOption)
                {
                    case ExportItemsOptions.All:
                        _powerSHAPE.DoCommand("EXPORTTYPE ALL");
                        break;
                    case ExportItemsOptions.Selected:
                        _powerSHAPE.DoCommand("EXPORTTYPE SELECTED");
                        break;
                    case ExportItemsOptions.Visible:
                        _powerSHAPE.DoCommand("EXPORTTYPE VISIBLE");
                        break;
                }

                _powerSHAPE.DoCommand("NEXT");

                // Applies the workplane to export
                switch (exportWorkplaneOption)
                {
                    case ExportWorkplanes.Active:
                        _powerSHAPE.DoCommand("EXPORTTYPE ACTIVE");
                        break; // TODO: might not be correct. Was : Exit Select

                    default:
                        _powerSHAPE.DoCommand("EXPORTTYPE WORLD");
                        break; // TODO: might not be correct. Was : Exit Select
                }

                _powerSHAPE.DoCommand("WIZEXPORT");
            }
            else
            {
                _powerSHAPE.DoCommand("WIZEXPORT", exportFile.Path);
            }

            _powerSHAPE.DoCommand("YES", "YES", "DISMISS");

            if (_powerSHAPE.IsFormUpdateOff == false)
            {
                _powerSHAPE.DoCommand("FORMUPDATE ON");
            }
        }

        /// <summary>
        /// Unblanks anything that is currently Blanked.
        /// </summary>
        public void Unblank()
        {
            _powerSHAPE.DoCommand("DISPLAY UNBLANK");
        }

        /// <summary>
        /// This option blanks all visible objects and unblanks all previously blanked objects.
        /// </summary>
        /// <remarks></remarks>
        public void BlankToggle()
        {
            _powerSHAPE.DoCommand("DISPLAY BLANKTOGGLE");
        }

        /// <summary>
        /// Resets the User Interface, cancelling any open dialogs ready for normal operation.
        /// </summary>
        public void ResetUI()
        {
            _powerSHAPE.DoCommand("CANCEL");
        }

        /// <summary>
        /// Exports a screenshot, using PowerSHAPE's Print To File functionality.
        /// </summary>
        /// <param name="outputLocation">A jpg file of the screenshot.</param>
        public void ExportScreenshot(File outputLocation)
        {
            // Check to see whether the file already exists
            bool alreadyExists = false;
            if (outputLocation.Exists)
            {
                alreadyExists = true;
            }

            // Carry out operation
            _powerSHAPE.DoCommand("FILE PRINT Filename FileDialog Write Validate " + outputLocation.Path);

            // If file already exists, confirm overwrite within PowerSHAPE
            if (alreadyExists)
            {
                _powerSHAPE.DoCommand("YES");
            }
        }

        /// <summary>
        /// Print the specified drawings to the specified printer
        /// </summary>
        /// <param name="printerName">Printer name</param>
        /// <param name="drawings">Drawings to print</param>
        public void PrintDrawings(string printerName, string[] drawings)
        {
            _powerSHAPE.DoCommand("FILE PRINTDRAWINGS", "SELECTPRINTER " + printerName, "SELECTIONREPLACE " + drawings[0]);
            for (int i = 1; i <= drawings.Length - 1; i++)
            {
                _powerSHAPE.DoCommand("SELECTIONADD " + drawings[i]);
            }
            _powerSHAPE.DoCommand("SELECTIONEND", "PRINTSELECTED");
        }

        /// <summary>
        /// Retriangulate model to its current shading tolerance.
        /// </summary>
        public void Retriangulate()
        {
            // Carry out operation
            _powerSHAPE.DoCommand("SHADING RETRIANGULATE");
        }

        /// <summary>
        /// Creates a temporary workplane at the specified origin aligned to the current workplane.
        /// </summary>
        /// <param name="origin">The origin of the temporary workplane.</param>
        public void CreateTemporaryWorkplane(Point origin)
        {
            _powerSHAPE.DoCommand("CREATE WORKPLANE SINGLE",
                                  "TEMP_WORKPLANE ON",
                                  origin.X.ToString("0.######") + " " + origin.Y.ToString("0.######") + " " + origin.Z.ToString("0.######"));
        }

        /// <summary>
        /// Deletes the temporary workplane.
        /// </summary>
        public void DeleteTemporaryWorkplane()
        {
            _powerSHAPE.DoCommand("TEMP_WORKPLANE OFF");
        }

        /// <summary>
        /// Sets the creation level for new items
        /// </summary>
        /// <param name="creationLevel">The creation level to be used</param>
        public void SetCreationLevel(PSLevel creationLevel)
        {
            // Clear the selection otherwise the selected items will be added to the specified level rather than setting the creation level
            ClearSelectedItems();

            // The yes command afterwards ensures that the level gets switch on if it is not active already
            _powerSHAPE.DoCommand("LEVELSELECTOR INPUTLEVEL " + creationLevel.Number, "YES");
        }

        /// <summary>
        /// Sets the creation level for new items
        /// </summary>
        /// <param name="creationLevel">The creation level to be used</param>
        /// <exception cref="Exception">The specified level does not exist</exception>
        public void SetCreationLevel(int creationLevel)
        {
            if (creationLevel < 0 || creationLevel >= Levels.Count)
            {
                throw new Exception("The specified level does not exist");
            }

            // Clear the selection otherwise the selected items will be added to the specified level rather than setting the creation level
            ClearSelectedItems();

            // The yes command afterwards ensures that the level gets switch on if it is not active already
            _powerSHAPE.DoCommand("LEVELSELECTOR INPUTLEVEL " + creationLevel, "YES");
        }

        /// <summary>
        /// Gets the nearest surface to the specified point as well as the nearest point on that surface
        /// </summary>
        /// <param name="referencePoint">The point to use for finding the nearest surface</param>
        /// <param name="nearestPoint">Returns the nearest point on the nearest surface. Or null if the nearest surface cannot be found</param>
        /// <param name="useWorldCoordinates">Whether or not to return the nearest point on the surface in world coordinates</param>
        /// <returns>The nearest surface or null if the nearest surface cannot be found</returns>
        public PSSurface GetNearestSurface(Point referencePoint, ref Point nearestPoint, bool useWorldCoordinates = false)
        {
            try
            {
                // If the user has asked to use the active workplane but the active workplane is world then the call
                // will not return a result.  In which case choose to use the world coordinates
                // Else if the user has asked to use world coordinates but there is an active workplane then
                // we need to rebase the values of X, Y and Z for this point back to world
                Point pointToFind = referencePoint.Clone();
                if ((useWorldCoordinates == false) & (_powerSHAPE.ActiveModel.ActiveWorkplane == null))
                {
                    useWorldCoordinates = true;
                }
                else if (useWorldCoordinates & (_powerSHAPE.ActiveModel.ActiveWorkplane != null))
                {
                    pointToFind.RebaseFromWorkplane(_powerSHAPE.ActiveModel.ActiveWorkplane.ToWorkplane());
                }
                List<object> result = ((Array) (object) _powerSHAPE.ActiveDocument.GetNearestSurfacePointData(useWorldCoordinates,
                                                                                                              new[]
                                                                                                              {
                                                                                                                  pointToFind
                                                                                                                      .X.Value,
                                                                                                                  pointToFind
                                                                                                                      .Y.Value,
                                                                                                                  pointToFind
                                                                                                                      .Z.Value
                                                                                                              })).Cast<object>()
                                                                                                                 .ToList();
                if (int.Parse(result[0].ToString()) == -1)
                {
                    // Failed to find a surface
                    return null;
                }
                nearestPoint = new Point(Convert.ToDouble(((object[]) result[3])[0].ToString()),
                                         Convert.ToDouble(((object[]) result[3])[1].ToString()),
                                         Convert.ToDouble(((object[]) result[3])[2].ToString()));
                return _powerSHAPE.ActiveModel.Surfaces.FirstOrDefault(surface => surface.Id == int.Parse(result[1].ToString()));
            }
            catch
            {
            }

            nearestPoint = null;
            return null;
        }

        #endregion

        #region " Private Operations "

        /// <summary>
        /// Initialises the collections.
        /// </summary>
        internal void Initialise()
        {
            // If we have 2016 SP2 or greater then we can take advantage of the faster code
            if (_powerSHAPE.Version != null && _powerSHAPE.Version <= new Version("16.1.29"))
            {
                InitialiseSlow();
            }
            else
            {
                InitialiseFast();
            }
        }

        internal void InitialiseSlow()
        {
            _isListsDirty = false;

            _annotationsCollection = new PSAnnotationsCollection(_powerSHAPE);
            _arcsCollection = new PSArcsCollection(_powerSHAPE);
            _compCurvesCollection = new PSCompCurvesCollection(_powerSHAPE);
            _curvesCollection = new PSCurvesCollection(_powerSHAPE);
            _electrodesCollection = new PSElectrodesCollection(_powerSHAPE);
            _linesCollection = new PSLinesCollection(_powerSHAPE);
            _meshesCollection = new PSMeshesCollection(_powerSHAPE);
            _pointsCollection = new PSPointsCollection(_powerSHAPE);
            _solidsCollection = new PSSolidsCollection(_powerSHAPE);
            _surfacesCollection = new PSSurfacesCollection(_powerSHAPE);
            _workplanesCollection = new PSWorkplanesCollection(_powerSHAPE);

            _levelsCollection = new PSLevelsCollection(_powerSHAPE);
            _materialsCollection = new PSMaterialsCollection(_powerSHAPE);

            // Get all the objects currently selected
            List<PSEntity> originalSelection = SelectedItems;

            // Get all the levels currently named/used
            List<PSLevel> originallyUsedNamedLevels = UsedLevels;
            foreach (PSLevel namedLevel in NamedLevels)
            {
                originallyUsedNamedLevels.Add(namedLevel);
            }

            // Remove all the levels that are not currently active
            List<PSLevel> originallyActiveLevels = new List<PSLevel>();
            foreach (PSLevel levelToCheck in originallyUsedNamedLevels)
            {
                if (levelToCheck.IsActive)
                {
                    originallyActiveLevels.Add(levelToCheck);
                }
            }

            // Get all the objects to loop through
            SelectAll(true);

            object[] selection = _powerSHAPE.ActiveDocument.GetSelectedObjects();

            if (selection != null)
            {
                foreach (int compositeID in selection)
                {
                    // We wrap this in a try/catch as we do not want the whole function to fail
                    // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                    try
                    {
                        // Get the object using standard entity factory
                        PSEntity entity = null;
                        entity = (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, compositeID);

                        // Add the entity to the correct collection
                        AddNoChecks(entity);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            // Do same as add entity but this one is for workplanes
            ClearSelectedItems();
            _powerSHAPE.DoCommand("SELECT WORKPLANES");
            object[] selectionWorkplanes = _powerSHAPE.ActiveDocument.GetSelectedObjects();

            if (selectionWorkplanes != null)
            {
                foreach (int compositeID in selectionWorkplanes)
                {
                    // We wrap this in a try/catch as we do not want the whole function to fail
                    // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                    try
                    {
                        // Get the object using standard entity factory
                        PSEntity entity = null;
                        entity = (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, compositeID);

                        // Add the entity to the workplanes collection
                        if (entity is PSWorkplane)
                        {
                            Workplanes.AddNoChecks((PSWorkplane) entity);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            // Clear the selection
            ClearSelectedItems();

            // Restore previously active levels
            Levels.DeactivateAllLevels();
            foreach (PSLevel previouslyActiveLevel in originallyActiveLevels)
            {
                previouslyActiveLevel.IsActive = true;
            }

            // Restore previously selected items
            foreach (PSEntity previouslySelectedEntity in originalSelection)
            {
                previouslySelectedEntity.AddToSelection(false);
            }
        }

        internal void InitialiseFast()
        {
            _isListsDirty = false;

            _annotationsCollection = new PSAnnotationsCollection(_powerSHAPE);
            _arcsCollection = new PSArcsCollection(_powerSHAPE);
            _compCurvesCollection = new PSCompCurvesCollection(_powerSHAPE);
            _curvesCollection = new PSCurvesCollection(_powerSHAPE);
            _electrodesCollection = new PSElectrodesCollection(_powerSHAPE);
            _linesCollection = new PSLinesCollection(_powerSHAPE);
            _meshesCollection = new PSMeshesCollection(_powerSHAPE);
            _pointsCollection = new PSPointsCollection(_powerSHAPE);
            _solidsCollection = new PSSolidsCollection(_powerSHAPE);
            _surfacesCollection = new PSSurfacesCollection(_powerSHAPE);
            _workplanesCollection = new PSWorkplanesCollection(_powerSHAPE);

            _levelsCollection = new PSLevelsCollection(_powerSHAPE);
            _materialsCollection = new PSMaterialsCollection(_powerSHAPE);

            object[] allItems = _powerSHAPE.ActiveDocument.GetModelItems();

            if (allItems != null)
            {
                object[] types = null;
                object[] ids = null;
                object[] names = null;
                object[] descriptions = null;
                object[] levels = null;
                _powerSHAPE.ActiveDocument.GetItemData(allItems, out types, out ids, out names, out descriptions, out levels);

                for (var i = 0; i <= allItems.Length - 1; i++)
                {
                    // We wrap this in a try/catch as we do not want the whole function to fail
                    // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                    try
                    {
                        // Get the object using standard entity factory
                        PSEntity entity =
                            (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE,
                                                                    int.Parse(types[i].ToString()),
                                                                    int.Parse(ids[i].ToString()),
                                                                    names[i].ToString(),
                                                                    descriptions[i].ToString(),
                                                                    levels[i].ToString());

                        // Add the entity to the items to return
                        if (entity != null)
                        {
                            AddNoChecks(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void RefreshEntities(List<PSEntity> cachedEntities)
        {
            // Get non blanked entities
            SelectAll(true);
            var powerShapeNonBlankedEntities = GetSelectedEntities(cachedEntities);

            // Get blanked entities
            BlankToggle();
            SelectAll(true);
            var powerShapeBlankedEntities = GetSelectedEntities(cachedEntities);

            List<PSEntity> powerShapeAllEntities = new List<PSEntity>();
            powerShapeAllEntities.AddRange(powerShapeBlankedEntities);
            powerShapeAllEntities.AddRange(powerShapeNonBlankedEntities);

            // Reset all level numbers to -1 to indicate they are unknown
            powerShapeAllEntities.ForEach(x => x.SetLevelNumber(-1));

            //Remove old entities from the model
            var entitiesToDelete = cachedEntities.Except(powerShapeAllEntities).ToList();
            foreach (PSEntity entity in entitiesToDelete)
            {
                Remove(entity);
            }

            // Clear selection
            ClearSelectedItems();

            // Keep the blank entities
            BlankToggle();
        }

        private List<PSEntity> GetSelectedEntities(List<PSEntity> cachedEntities)
        {
            // Get all the objects to loop through
            int powerShapeEntities = _powerSHAPE.ReadIntValue("SELECTION.NUMBER");

            var currentPowerShapeEntities = new List<PSEntity>();

            for (int i = 0; i <= powerShapeEntities - 1; i++)
            {
                // We wrap this in a try/catch as we do not want the whole function to fail
                // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                try
                {
                    // Get the object using standard entity factory
                    string entityType = _powerSHAPE.ReadStringValue("SELECTION.TYPE[" + i + "]");
                    string entityName = _powerSHAPE.ReadStringValue("SELECTION.NAME[" + i + "]");
                    var entityId = _powerSHAPE.ReadIntValue(entityType + "['" + entityName + "'].ID");

                    var originalModelEntity = cachedEntities.FirstOrDefault(
                        x => (x.Id == entityId) & (x.Name.ToUpperInvariant() == entityName.ToUpperInvariant()) &
                             (x.Identifier.ToUpperInvariant() == entityType.ToUpperInvariant()));
                    PSEntity currentEntity = null;

                    if (originalModelEntity != null)
                    {
                        // Entity already exits in the model
                        currentPowerShapeEntities.Add(originalModelEntity);
                    }
                    else
                    {
                        currentEntity = (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, entityType, entityId, entityName);

                        // Add the entity to the correct collection
                        Add(currentEntity);
                        currentPowerShapeEntities.Add(currentEntity);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return currentPowerShapeEntities;
        }

        /// <summary>
        /// Refreshes workplanes collection to reflect PowerShape status. It adds a workplane to the collection if exists in PowerShape and it isn't in the active model.
        /// It deletes the workplane from the collection if it no longer exists in PowerShape.
        /// </summary>
        private void RefreshWorkplanes()
        {
            _powerSHAPE.DoCommand("SELECT WORKPLANES");
            int numberOfWorkplanes = _powerSHAPE.ReadIntValue("SELECTION.NUMBER");

            var originalModelWorkplanes = new List<PSEntity>();
            originalModelWorkplanes.AddRange(_workplanesCollection);

            var currentPowerShapeWorkplanes = new List<PSEntity>();

            for (int i = 0; i <= numberOfWorkplanes - 1; i++)
            {
                // We wrap this in a try/catch as we do not want the whole function to fail
                // if a single object hasn't yet been mapped or the collection doesn't have an Add method etc
                try
                {
                    // Get the object using standard entity factory
                    string entityType = _powerSHAPE.ReadStringValue("SELECTION.TYPE[" + i + "]");
                    string entityName = _powerSHAPE.ReadStringValue("SELECTION.NAME[" + i + "]");
                    int entityId = _powerSHAPE.ReadIntValue(entityType + "['" + entityName + "'].ID");

                    var originalModelWorkplane = originalModelWorkplanes.FirstOrDefault(x => x.Id == entityId);
                    if (originalModelWorkplane != null)
                    {
                        //Workplane exists in the model
                        currentPowerShapeWorkplanes.Add(originalModelWorkplane);
                    }
                    else
                    {
                        PSEntity entity = (PSEntity) PSEntityFactory.CreateEntity(_powerSHAPE, entityType, entityId, entityName);

                        if (entity is PSWorkplane)
                        {
                            Workplanes.Add((PSWorkplane) entity);
                            currentPowerShapeWorkplanes.Add(entity);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            //Remove old workplanes from the model
            var workplanesToDelete = originalModelWorkplanes.Except(currentPowerShapeWorkplanes);
            foreach (PSEntity entity in workplanesToDelete)
            {
                Workplanes.Remove((PSWorkplane) entity);
            }

            ClearSelectedItems();
        }

        /// <summary>
        /// Refreshes levels collection to reflect PowerShape status.
        /// </summary>
        /// <remarks></remarks>
        private void RefreshLevels()
        {
            //Original entities
            var originalModelLevels = new List<PSLevel>();
            originalModelLevels.AddRange(_levelsCollection);

            //New entities with the latest data
            _levelsCollection.Clear();
            Levels.ReadLevels();
            var powerShapeLevels = new List<PSLevel>();
            powerShapeLevels.AddRange(_levelsCollection);
            _levelsCollection.Clear();

            foreach (PSLevel powerShapeLevel in powerShapeLevels)
            {
                var originalLevel = originalModelLevels.FirstOrDefault(x => x.Number == powerShapeLevel.Number);
                if (originalLevel == null)
                {
                    _levelsCollection.Add(powerShapeLevel);
                }
                else
                {
                    _levelsCollection.Add(originalLevel);
                }
            }
        }

        /// <summary>
        /// Refreshes materials collection to reflect PowerShape state.
        /// </summary>
        private void RefreshMaterials()
        {
            var materialsToDelete = new List<PSMaterial>();
            foreach (PSMaterial material in _materialsCollection)
            {
                if (material.Exists())
                {
                    //No need to add to the list
                }
                else
                {
                    materialsToDelete.Add(material);
                }
            }

            // Remove old material
            foreach (PSMaterial material in materialsToDelete)
            {
                _materialsCollection.Remove(material);
            }
        }

        /// <summary>
        /// Attempts to get the entity from the active model.
        /// </summary>
        /// <param name="entity">The returned entity from cache.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="entityName">The entity name.</param>
        /// <param name="entityType">The identifier PowerSHAPE uses to identify this type of entity.</param>
        private bool TryGetEntityFromCache(ref PSEntity entity, int entityId, string entityName, string entityType)
        {
            var cache = new List<PSEntity>();
            switch (entityType.ToUpper())
            {
                case PSAnnotation.ANNOTATION_IDENTIFIER:
                    cache = _annotationsCollection.Cast<PSEntity>().ToList();
                    break;
                case PSArc.ARC_IDENTIFIER:
                    cache = _arcsCollection.Cast<PSEntity>().ToList();
                    break;
                case PSCompCurve.COMPCURVE_IDENTIFIER:
                case "COMPOSITE CURVE":
                    cache = _compCurvesCollection.Cast<PSEntity>().ToList();
                    break;
                case PSCurve.CURVE_IDENTIFIER:
                    cache = _curvesCollection.Cast<PSEntity>().ToList();
                    break;
                case PSElectrode.ELECTRODE_IDENTIFIER:
                    cache = _electrodesCollection.Cast<PSEntity>().ToList();
                    break;
                case PSLine.LINE_IDENTIFIER:
                    cache = _linesCollection.Cast<PSEntity>().ToList();
                    break;
                case PSPoint.POINT_IDENTIFIER:
                    cache = _pointsCollection.Cast<PSEntity>().ToList();
                    break;
                case PSSolid.SOLID_IDENTIFIER:
                    cache = _solidsCollection.Cast<PSEntity>().ToList();
                    break;
                case PSMesh.MESH_IDENTIFIER:
                    cache = _meshesCollection.Cast<PSEntity>().ToList();
                    break;
                case PSSurface.SURFACE_IDENTIFIER:
                    cache = _surfacesCollection.Cast<PSEntity>().ToList();
                    break;
                case PSWorkplane.WORKPLANE_IDENTIFIER:
                    cache = _workplanesCollection.Cast<PSEntity>().ToList();
                    break;
            }

            var cacheEntity = cache.FirstOrDefault(x => (x.Id == entityId) &
                                                        (x.Name.ToUpperInvariant() == entityName.ToUpperInvariant()) &
                                                        (x.Identifier.ToUpperInvariant() == entityType.ToUpperInvariant()));
            if (cacheEntity != null)
            {
                entity = cacheEntity;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to get the entity from the active model.
        /// </summary>
        /// <param name="entity">The returned entity from cache.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="entityName">The entity name.</param>
        /// <param name="entityType">The identifier PowerSHAPE uses to identify this type of entity.</param>
        private bool TryGetEntityFromCache(
            ref PSEntity entity,
            int entityId,
            string entityName,
            int entityType,
            string description)
        {
            var cache = new List<PSEntity>();
            switch (entityType)
            {
                case 61:
                    cache = _annotationsCollection.Cast<PSEntity>().ToList();
                    break;
                case 52:
                    cache = _arcsCollection.Cast<PSEntity>().ToList();
                    break;
                case 59:
                    cache = _compCurvesCollection.Cast<PSEntity>().ToList();
                    break;
                case 53:
                    cache = _curvesCollection.Cast<PSEntity>().ToList();
                    break;
                case 60:
                    switch (description)
                    {
                        case "Mesh":
                            cache = _meshesCollection.Cast<PSEntity>().ToList();
                            break;
                        case "Point":
                            cache = _pointsCollection.Cast<PSEntity>().ToList();
                            break;
                        case "Electrode":
                            cache = _electrodesCollection.Cast<PSEntity>().ToList();
                            break;
                    }
                    break;
                case 58:
                    cache = _linesCollection.Cast<PSEntity>().ToList();
                    break;
                case 49:
                    cache = _pointsCollection.Cast<PSEntity>().ToList();
                    break;
                case 40:
                    cache = _solidsCollection.Cast<PSEntity>().ToList();
                    break;
                case 21:
                    cache = _surfacesCollection.Cast<PSEntity>().ToList();
                    break;
                case 54:
                    cache = _workplanesCollection.Cast<PSEntity>().ToList();
                    break;
            }

            var cacheEntity =
                cache.FirstOrDefault(x => (x.Id == entityId) & (x.Name.ToUpperInvariant() == entityName.ToUpperInvariant()));
            if (cacheEntity != null)
            {
                entity = cacheEntity;
                return true;
            }
            return false;
        }

        #endregion

        #region " Friend Operations "

        /// <summary>
        /// Adds the entity to its relevant collection.
        /// </summary>
        /// <param name="entityToAdd">The entity to add.</param>
        /// <remarks></remarks>
        internal void Add(PSEntity entityToAdd)
        {
            switch (entityToAdd.Identifier.ToUpper())
            {
                case PSAnnotation.ANNOTATION_IDENTIFIER:
                    Annotations.Add((PSAnnotation) entityToAdd);
                    break;
                case PSArc.ARC_IDENTIFIER:
                    Arcs.Add((PSArc) entityToAdd);
                    break;
                case PSCompCurve.COMPCURVE_IDENTIFIER:
                case "COMPOSITE CURVE":
                    CompCurves.Add((PSCompCurve) entityToAdd);
                    break;
                case PSCurve.CURVE_IDENTIFIER:
                    Curves.Add((PSCurve) entityToAdd);
                    break;
                case PSElectrode.ELECTRODE_IDENTIFIER:
                    Electrodes.Add((PSElectrode) entityToAdd);
                    break;
                case PSLine.LINE_IDENTIFIER:
                    Lines.Add((PSLine) entityToAdd);
                    break;
                case PSPoint.POINT_IDENTIFIER:
                    Points.Add((PSPoint) entityToAdd);
                    break;
                case PSSolid.SOLID_IDENTIFIER:
                    Solids.Add((PSSolid) entityToAdd);
                    break;
                case PSMesh.MESH_IDENTIFIER:
                    Meshes.Add((PSMesh) entityToAdd);
                    break;
                case "PLANE":
                case PSSurface.SURFACE_IDENTIFIER:
                case "POWERSURFACE":
                    Surfaces.Add((PSSurface) entityToAdd);
                    break;
                case PSWorkplane.WORKPLANE_IDENTIFIER:
                    Workplanes.Add((PSWorkplane) entityToAdd);
                    break;
            }
        }

        /// <summary>
        /// Adds the entity to its relevant collection.
        /// </summary>
        /// <param name="entityToAdd">The entity to add.</param>
        /// <remarks></remarks>
        internal void AddNoChecks(PSEntity entityToAdd)
        {
            switch (entityToAdd.Identifier.ToUpper())
            {
                case PSAnnotation.ANNOTATION_IDENTIFIER:
                    Annotations.AddNoChecks((PSAnnotation) entityToAdd);
                    break;
                case PSArc.ARC_IDENTIFIER:
                    Arcs.AddNoChecks((PSArc) entityToAdd);
                    break;
                case PSCompCurve.COMPCURVE_IDENTIFIER:
                case "COMPOSITE CURVE":
                    CompCurves.AddNoChecks((PSCompCurve) entityToAdd);
                    break;
                case PSCurve.CURVE_IDENTIFIER:
                    Curves.AddNoChecks((PSCurve) entityToAdd);
                    break;
                case PSElectrode.ELECTRODE_IDENTIFIER:
                    Electrodes.AddNoChecks((PSElectrode) entityToAdd);
                    break;
                case PSLine.LINE_IDENTIFIER:
                    Lines.AddNoChecks((PSLine) entityToAdd);
                    break;
                case PSPoint.POINT_IDENTIFIER:
                    Points.AddNoChecks((PSPoint) entityToAdd);
                    break;
                case PSSolid.SOLID_IDENTIFIER:
                    Solids.AddNoChecks((PSSolid) entityToAdd);
                    break;
                case PSMesh.MESH_IDENTIFIER:
                    Meshes.AddNoChecks((PSMesh) entityToAdd);
                    break;
                case "PLANE":
                case PSSurface.SURFACE_IDENTIFIER:
                case "POWERSURFACE":
                    Surfaces.AddNoChecks((PSSurface) entityToAdd);
                    break;
                case PSWorkplane.WORKPLANE_IDENTIFIER:
                    Workplanes.AddNoChecks((PSWorkplane) entityToAdd);
                    break;
            }
        }

        /// <summary>
        /// Removes the entity to its relevant collection.
        /// </summary>
        /// <param name="entityToRemove">The entity to remove.</param>
        internal void Remove(PSEntity entityToRemove)
        {
            switch (entityToRemove.Identifier.ToUpper())
            {
                case PSAnnotation.ANNOTATION_IDENTIFIER:
                    Annotations.Remove((PSAnnotation) entityToRemove);
                    break;
                case PSArc.ARC_IDENTIFIER:
                    Arcs.Remove((PSArc) entityToRemove);
                    break;
                case PSCompCurve.COMPCURVE_IDENTIFIER:
                case "COMPOSITE CURVE":
                    CompCurves.Remove((PSCompCurve) entityToRemove);
                    break;
                case PSCurve.CURVE_IDENTIFIER:
                    Curves.Remove((PSCurve) entityToRemove);
                    break;
                case PSElectrode.ELECTRODE_IDENTIFIER:
                    Electrodes.Remove((PSElectrode) entityToRemove);
                    break;
                case PSLine.LINE_IDENTIFIER:
                    Lines.Remove((PSLine) entityToRemove);
                    break;
                case PSPoint.POINT_IDENTIFIER:
                    Points.Remove((PSPoint) entityToRemove);
                    break;
                case PSSolid.SOLID_IDENTIFIER:
                    Solids.Remove((PSSolid) entityToRemove);
                    break;
                case PSMesh.MESH_IDENTIFIER:
                    Meshes.Remove((PSMesh) entityToRemove);
                    break;
                case "PLANE":
                case PSSurface.SURFACE_IDENTIFIER:
                case "POWERSURFACE":
                    Surfaces.Remove((PSSurface) entityToRemove);
                    break;
                case PSWorkplane.WORKPLANE_IDENTIFIER:
                    Workplanes.Remove((PSWorkplane) entityToRemove);
                    break;
            }
        }

        /// <summary>
        /// Checks whether the relevant collection contains the entity.
        /// </summary>
        /// <param name="entityToCheck">Returns true if it does.</param>
        internal bool Contains(PSEntity entityToCheck)
        {
            switch (entityToCheck.Identifier.ToUpper())
            {
                case PSAnnotation.ANNOTATION_IDENTIFIER:
                    return Annotations.Contains(entityToCheck);
                case PSArc.ARC_IDENTIFIER:
                    return Arcs.Contains(entityToCheck);
                case PSCompCurve.COMPCURVE_IDENTIFIER:
                case "COMPOSITE CURVE":
                    return CompCurves.Contains(entityToCheck);
                case PSCurve.CURVE_IDENTIFIER:
                    return Curves.Contains(entityToCheck);
                case PSElectrode.ELECTRODE_IDENTIFIER:
                    return Electrodes.Contains(entityToCheck);
                case PSLine.LINE_IDENTIFIER:
                    return Lines.Contains(entityToCheck);
                case PSPoint.POINT_IDENTIFIER:
                    return Points.Contains(entityToCheck);
                case PSSolid.SOLID_IDENTIFIER:
                    return Solids.Contains(entityToCheck);
                case PSMesh.MESH_IDENTIFIER:
                    return Meshes.Contains(entityToCheck);
                case "PLANE":
                case PSSurface.SURFACE_IDENTIFIER:
                case "POWERSURFACE":
                    return Surfaces.Contains(entityToCheck);
                case PSWorkplane.WORKPLANE_IDENTIFIER:
                    return Workplanes.Contains(entityToCheck);
            }
            return false;
        }

        #endregion
    }
}