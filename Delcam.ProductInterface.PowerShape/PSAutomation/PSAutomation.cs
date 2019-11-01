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
using System.Runtime.InteropServices;
using System.Threading;
using Autodesk.Extensions;

namespace Autodesk.ProductInterface.PowerSHAPE
{
    /// <summary>
    /// This is the root class of a PowerSHAPE instance.
    /// When it is created it creates its own instance of PowerSHAPE.
    /// When it is destroyed the instance of PowerSHAPE will also be destroyed.
    /// </summary>
    public class PSAutomation : Automation, IDisposable
    {
        #region " Fields "

        /// <summary>
        /// The COM reference to the instance of PowerSHAPE.
        /// </summary>
        private dynamic _powerSHAPE;

        /// <summary>
        /// PowerShape true value represented in 1 means true and 0 means false;
        /// </summary>
        private string TrueValue = "1";

        /// <summary>
        /// The Class Id for the PowerSHAPE COM server.
        /// </summary>
        private const string CLASS_ID = "PowerSHAPE.Application";

        /// <summary>
        /// The Class Id for the PowerSHAPE Surfacer COM server.
        /// </summary>
        private const string PS_SURFACER_CLASS_ID = "PS-Surfacer.Application";

        /// <summary>
        /// The Class Id for the PowerSHAPE Viewer COM server.
        /// </summary>
        private const string PS_VIEWER_CLASS_ID = "PSViewer.Application";

        /// <summary>
        /// The Class Id for the PowerSHAPE Estimator COM server.
        /// </summary>
        private const string PS_ESTIMATOR_CLASS_ID = "PSEstimator.Application";

        /// <summary>
        /// The Class Id for the PowerMill Modeller COM server.
        /// </summary>
        private const string PMILL_MODELLING_CLASS_ID = "PMillModelling.Application";

        /// <summary>
        /// The Class Id for the PowerMill Sketcher COM server.
        /// </summary>
        private const string PMILL_SKETCHER_CLASS_ID = "PMillSketcher.Application";

        /// <summary>
        /// The Class Id for the PowerMill Electrode COM server.
        /// </summary>
        private const string PMILL_ELECTRODE_CLASS_ID = "PMillElectrode.Application";

        /// <summary>
        /// The name of the Application as entered in the registry.
        /// </summary>
        private const string APP_NAME = "PowerSHAPE";

        /// <summary>
        /// The name of the PowerSHAPE executable.
        /// </summary>
        private const string EXE_NAME = "powershape";

        /// <summary>
        /// The arguments used to hide powershape when it is starting.
        /// </summary>
        private const string ARGUMENTS = "-embedding -automation -norecovery -nogui";

        /// <summary>
        /// The arguments used to hide powershape-e when it is starting.
        /// </summary>
        private const string PS_E_ARGUMENTS = "-embedding -automation -norecovery -nogui -e";

        /// <summary>
        /// The Class Id for the PowerSHAPE Surfacer COM server.
        /// </summary>
        private const string PS_SURFACER_ARGUMENTS = "-embedding -automation -norecovery -nogui";

        /// <summary>
        /// The Class Id for the PowerSHAPE Viewer COM server.
        /// </summary>
        private const string PS_VIEWER_ARGUMENTS = "-embedding -automation -norecovery -nogui";

        /// <summary>
        /// The Class Id for the PowerSHAPE Estimator COM server.
        /// </summary>
        private const string PS_ESTIMATOR_ARGUMENTS = "-embedding -automation -norecovery -nogui";

        /// <summary>
        /// The Class Id for the PowerMill Modeller COM server.
        /// </summary>
        private const string PMILL_MODELLING_ARGUMENTS = "-embedding -automation -mill -norecovery -nogui";

        /// <summary>
        /// The Class Id for the PowerMill Sketcher COM server.
        /// </summary>
        private const string PMILL_SKETCHER_ARGUMENTS = "-embedding -automation -norecovery -nogui";

        /// <summary>
        /// The Class Id for the PowerMill Electrode COM server.
        /// </summary>
        private const string PMILL_ELECTRODE_ARGUMENTS = "-embedding -automation -norecovery -nogui";

        /// <summary>
        /// The list of Models that is currently in PowerSHAPE.
        /// </summary>
        private PSModelsCollection _models;

        /// <summary>
        /// The list of Windows that is currently in PowerSHAPE.
        /// </summary>
        private PSWindowsCollection _windows;

        /// <summary>
        /// This is a flag to indicate whether PowerSHAPE is in trim region editing mode when a surface is selected.
        /// </summary>
        private bool _isInTrimRegionEditing;

        /// <summary>
        /// The current trim region editing mode.
        /// </summary>
        private TrimRegionEditingModes _trimRegionEditingMode;

        /// <summary>
        /// The mode in which the application was started.
        /// </summary>
        private Modes _applicationMode;

        /// <summary>
        /// The last view that was activated.  It is required because before opening dynamic
        /// sectioning you must register the view with PowerSHAPE.
        /// </summary>
        private string _lastActivatedView = "TOP";

        /// <summary>
        /// This is a flag to indicate the status of the intelligent cursor and whether it is turned on or off.
        /// </summary>
        /// <remarks></remarks>
        private bool _isIntelligentCursorOn;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Connect to an instance of PowerShape that is passed in (this should be the COM object)
        /// </summary>
        /// <param name="powerShapeComObject">The COM object of PowerShape to connect to.</param>
        public PSAutomation(object powerShapeComObject)
        {
            _powerSHAPE = powerShapeComObject;

            _version = new Version(ReadIntValue("VERSION.MAJOR"),
                                   ReadIntValue("VERSION.MINOR"),
                                   ReadIntValue("VERSION.REVISION"));

            // Initialize Windows and models list
            RefreshWindowsAndModelsList();
        }

        /// <summary>
        /// Calls down to the base class to start PowerSHAPE based on the specified startup option.
        /// </summary>
        /// <param name="instanceReuse">Specifies how to start the application (e.g. use existing running PowerShape instance, create a new instance alongside any existing instances, etc.)</param>
        /// <param name="applicationMode">The mode in which PowerSHAPE will run.</param>
        /// <remarks></remarks>
        public PSAutomation(InstanceReuse instanceReuse, Modes applicationMode = Modes.PShapeMode)
        {
            _applicationMode = applicationMode;

            Initialise(instanceReuse);

            // Initialize Windows and models list
            RefreshWindowsAndModelsList();
        }

        /// <summary>
        /// Calls down to the base class to start PowerSHAPE based on the specified options.
        /// </summary>
        /// <param name="instanceReuse">Specifies how to start the application (e.g. use existing running PowerShape instance, create a new instance alongside any existing instances, etc.)</param>
        /// <param name="version">The version to run.</param>
        /// <param name="maximumVersion">If filled, the version of PowerShape to run has to be less of equal to 'maximumVersion'.</param>
        /// <param name="applicationMode">The mode in which PowerSHAPE will run.</param>
        /// <remarks></remarks>
        public PSAutomation(
            InstanceReuse instanceReuse,
            Version version,
            Version maximumVersion = null,
            Modes applicationMode = Modes.PShapeMode)
        {
            _applicationMode = applicationMode;

            Initialise(instanceReuse, version, maximumVersion);

            // Initialize Windows and models list
            RefreshWindowsAndModelsList();
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Attempts to connect to an existing version of PowerSHAPE or create a new one.
        /// </summary>
        /// <param name="version">The version to run.</param>
        /// <param name="maximumVersion">If filled, the version of PowerShape to run has to be less of equal to 'maximumVersion'.</param>
        /// <remarks></remarks>
        protected override void UseExistingInstance(Version version, Version maximumVersion)
        {
            try
            {
                TweakVersionNumbersFrom2018(ref version, ref maximumVersion);

                // Try to get an existing instance of PowerSHAPE
                _powerSHAPE = Marshal.GetActiveObject(ClassId);
                DoCommand("NOTHING");

                if (version != null)
                {
                    if (maximumVersion == null)
                    {
                        var _with1 = version;
                        if (_powerSHAPE.Version.ToString() != string.Format("{0:#}{1:0}{2:00}", _with1.Major, _with1.Minor, _with1.Build))
                        {
                            throw new Exception("Incorrect version found");
                        }
                    }
                    else
                    {
                        string sver = _powerSHAPE.Version.ToString();
                        Version foundVersion = null;
                        if (sver.Length > 4)
                        {
                            foundVersion = new Version(string.Format("{0}.{1}.{2}",
                                                                     sver.Substring(0, 2),
                                                                     sver.Substring(2, 1),
                                                                     sver.Substring(3, 2)));
                        }
                        else
                        {
                            foundVersion = new Version(string.Format("{0}.{1}.{2}",
                                                                     sver.Substring(0, 1),
                                                                     sver.Substring(1, 1),
                                                                     sver.Substring(2, 2)));
                        }
                        if ((foundVersion < version) | (foundVersion > maximumVersion))
                        {
                            throw new Exception("Incorrect version found");
                        }
                    }
                }
                else
                {
                    _version = new Version(ReadIntValue("VERSION.MAJOR"),
                                           ReadIntValue("VERSION.MINOR"),
                                           ReadIntValue("VERSION.REVISION"));
                }
            }
            catch
            {
                //Either one didn't exist or it was the wrong version so start a new one
                CreateNewInstance(version, maximumVersion);
            }

            // Ensure any previously open dialogs are closed
            DoCommand("CANCEL");

            IsIntelligentCursorOn = true;
        }

        /// <summary>
        /// Creates a new instance of PowerShape.
        /// </summary>
        /// <param name="version">The version to run.</param>
        /// <param name="maximumVersion">If filled, the version of PowerShape to run has to be less of equal to 'maximumVersion'.</param>
        /// <remarks></remarks>
        protected override void CreateNewInstance(Version version, Version maximumVersion)
        {
            try
            {
                Version tweakedVersion = version;
                Version tweakedMaximumVersion = maximumVersion;
                TweakVersionNumbersFrom2018(ref tweakedVersion, ref tweakedMaximumVersion);
                _powerSHAPE = StartByClassId(ClassId, EXE_NAME, tweakedVersion, tweakedMaximumVersion);
            }
            catch
            {
                Console.WriteLine("Didnt get it by class id...");

                // COM register the correct version and see how that goes
                Version tweakedVersion = version;
                Version tweakedMaximumVersion = maximumVersion;
                TweakVersionNumbersTo2018(ref tweakedVersion, ref tweakedMaximumVersion);
                StartExecutable(EXE_NAME, tweakedVersion, tweakedMaximumVersion, ClassId, false, "", true);

                // Try connecting again
                tweakedVersion = version;
                tweakedMaximumVersion = maximumVersion;
                TweakVersionNumbersFrom2018(ref tweakedVersion, ref tweakedMaximumVersion);
                _powerSHAPE = StartByClassId(ClassId, EXE_NAME, tweakedVersion, tweakedMaximumVersion);
                Console.WriteLine("Created by class id");
            }

            // Wait for PowerSHAPE to become ready
            try
            {
                // Adding delays in here seems to resolve an issue with reading the number of windows
                Thread.Sleep(500);
                DoCommand("NOTHING");
                Thread.Sleep(500);

                while (ReadIntValue("WINDOW.NUMBER") == 0) Thread.Sleep(500);

                _version = new Version(ReadIntValue("VERSION.MAJOR"),
                                       ReadIntValue("VERSION.MINOR"),
                                       ReadIntValue("VERSION.REVISION"));
            }
            catch
            {
            }
            IsIntelligentCursorOn = true;
            Console.WriteLine("Done");
        }

        /// <summary>
        /// Closes all running instance of PowerSHAPE and start a new one.
        /// </summary>
        /// <param name="version">The version to run.</param>
        /// <param name="maximumVersion">If filled, the version of PowerShape to run has to be less of equal to 'maximumVersion'.</param>
        /// <remarks></remarks>
        protected override void CreateSingleInstance(Version version, Version maximumVersion)
        {
            CloseAllInstancesOf(EXE_NAME);
            Version tweakedVersion = version;
            Version tweakedMaximumVersion = maximumVersion;
            TweakVersionNumbersTo2018(ref tweakedVersion, ref tweakedMaximumVersion);
            StartExecutable(APP_NAME, tweakedVersion, tweakedMaximumVersion, ClassId, false, CommandArguments);
            _powerSHAPE = Marshal.GetActiveObject(ClassId);
            Console.WriteLine("Created Single Instance");
            try
            {
                // Adding delays in here seems to resolve an issue with reading the number of windows
                Thread.Sleep(500);
                DoCommand("NOTHING");
                Thread.Sleep(500);

                while (ReadIntValue("WINDOW.NUMBER") == 0) Thread.Sleep(500);

                _version = new Version(ReadIntValue("VERSION.MAJOR"),
                                       ReadIntValue("VERSION.MINOR"),
                                       ReadIntValue("VERSION.REVISION"));
            }
            catch
            {
            }
            IsIntelligentCursorOn = true;
            Console.WriteLine("Done");
        }

        private void TweakVersionNumbersTo2018(ref Version tweakedVersion, ref Version tweakedMaximumVersion)
        {
            if (IsItAVersionEqualOrAbove2018(tweakedVersion))
            {
                if (tweakedVersion.Major > 2018)
                {
                    tweakedVersion = new Version(tweakedVersion.Major, 0, 0);
                }
                else
                {
                    tweakedVersion = new Version(tweakedVersion.Major + 2000, 0, 0);
                }

                if (tweakedMaximumVersion == null)
                {
                    tweakedMaximumVersion = new Version(tweakedVersion.Major, 99, 99);
                }
                else if (tweakedMaximumVersion.Major > 2018)
                {
                    tweakedMaximumVersion = new Version(tweakedMaximumVersion.Major, 99, 99);
                }
                else
                {
                    tweakedMaximumVersion = new Version(tweakedMaximumVersion.Major + 2000, 99, 99);
                }
            }
        }

        private void TweakVersionNumbersFrom2018(ref Version tweakedVersion, ref Version tweakedMaximumVersion)
        {
            if (IsItAVersionEqualOrAbove2018(tweakedVersion))
            {
                if (tweakedVersion.Major > 2018)
                {
                    tweakedVersion = new Version(tweakedVersion.Major - 2000, 0, 0);
                }
                else
                {
                    tweakedVersion = new Version(tweakedVersion.Major, 0, 0);
                }

                if (tweakedMaximumVersion == null)
                {
                    tweakedMaximumVersion = new Version(tweakedVersion.Major, 99, 99);
                }
                else if (tweakedMaximumVersion.Major > 2018)
                {
                    tweakedMaximumVersion = new Version(tweakedMaximumVersion.Major - 2000, 99, 99);
                }
                else
                {
                    tweakedMaximumVersion = new Version(tweakedMaximumVersion.Major, 99, 99);
                }
            }
        }

        private bool IsItAVersionEqualOrAbove2018(Version version)
        {
            return version != null && (version.Major >= 18 || version.Major >= 2018);
        }

        /// <summary>
        /// Closes the instance of the PowerSHAPE.
        /// </summary>
        public override void Quit()
        {
            Windows.Clear();
            _powerSHAPE.Exit();
            _powerSHAPE = null;
        }

        /// <summary>
        /// Records the macro into file path.
        /// </summary>
        /// <param name="macroFilename">The file path to save the macro.</param>
        /// <remarks></remarks>
        public override void RecordMacro(string macroFilename)
        {
            DoCommand("MACRO RECORD \"" + macroFilename + "\"");
            DoCommand("YES");

            //overwrite if prompted.
        }

        /// <summary>
        /// Stops recording macro.
        /// </summary>
        /// <remarks></remarks>
        public override void StopMacroRecording()
        {
            DoCommand("Macro Stop");
        }

        /// <summary>
        /// Closes all Models in PowerSHAPE and create a new one.
        /// </summary>
        public PSModel Reset()
        {
            Windows.Clear();
            Models.CreateEmptyModel();
            return ActiveModel;
        }

        /// <summary>
        /// Refresh the list of windows and models.
        /// Use it if a model has been added manually.
        /// </summary>
        public void RefreshWindowsAndModelsList()
        {
            _models = new PSModelsCollection(this);
            _windows = new PSWindowsCollection(this);

            // Get all windows/models
            string modelName = "";
            string numberOfWindowsString = null;
            while (string.IsNullOrWhiteSpace(numberOfWindowsString))
                try
                {
                    Thread.Sleep(1000);
                    numberOfWindowsString = DoCommandEx("WINDOW.NUMBER").ToString();
                }
                catch
                {
                }
            int numberOfWindows = int.Parse(numberOfWindowsString);
            var exitCount = 0;
            if (numberOfWindows != 0)
            {
                // Ensure PowerSHAPE has started correctly
                while (modelName.Length == 0)
                {
                    exitCount += 1;
                    modelName = DoCommandEx("MODEL.SELECTED").ToString();
                    if (exitCount > 100)
                    {
                        throw new Exception("Unable to start PowerSHAPE");
                    }
                }

                // Add all windows to the windows collection
                int windowCount = 1;
                while (_windows.Count != numberOfWindows)
                {
                    PSWindow currentWindow = new PSWindow(this, windowCount);
                    if (currentWindow.Exists)
                    {
                        _windows.Add(currentWindow);
                    }
                    windowCount += 1;
                }

                // Add all models to the models collection
                foreach (PSWindow window in _windows)
                {
                    if (window.Type == WindowTypes.MODEL)
                    {
                        int modelId = ReadIntValue("MODEL['" + window.AttachedProcessName + "'].ID");
                        PSModel newModel = new PSModel(this, modelId, window.AttachedProcessName, window);
                        _models.Add(newModel);
                    }
                    else
                    {
                        throw new NotImplementedException("Software can't evaluate drawings");
                    }
                }

                // Initialise currently active model
                ActiveModel.Initialise();
            }
        }

        #endregion

        #region " Macro Operations "

        /// <summary>
        /// Runs the Macro at the given location.
        /// </summary>
        /// <param name="macroFile">The file path to the macro.</param>
        /// <remarks></remarks>
        public void RunMacro(FileSystem.File macroFile)
        {
            PSMacro macro = null;
            macro = LoadMacro(macroFile);

            macro.Run();
        }

        /// <summary>
        /// Runs the specified Macro.
        /// </summary>
        /// <param name="macro">The macro to be run.</param>
        public void RunMacro(PSMacro macro)
        {
            macro.Run();
        }

        /// <summary>
        /// Loads a macro from the specified file that can be stepped through or run with progress feedback.
        /// </summary>
        /// <param name="macroFile">The file path from which to load the macro.</param>
        /// <returns>The new loaded macro.</returns>
        /// <remarks></remarks>
        public PSMacro LoadMacro(FileSystem.File macroFile)
        {
            PSMacro macro = new PSMacro(this, macroFile);
            return macro;
        }

        /// <summary>
        /// Create a macro from a string.
        /// </summary>
        /// <param name="macro">macro list.</param>
        /// <returns>The new PMMacro.</returns>
        public PSMacro LoadMacro(params string[] macro)
        {
            return new PSMacro(this, macro);
        }

        /// <summary>
        /// Runs the Macro at the given location.
        /// </summary>
        /// <param name="macro">The macro lines.</param>
        public void RunMacro(params string[] macro)
        {
            var psMacro = LoadMacro(macro);
            psMacro.Run();
        }

        #endregion

        #region " Window Operations "

        /// <summary>
        /// Gets the handle of the main window for PowerSHAPE.
        /// </summary>
        public override int MainWindowId
        {
            get { return _powerSHAPE.MainWindowHWND; }
        }

        /// <summary>
        /// Gets or sets the property showing whether PowerSHAPE is visible or not.
        /// </summary>
        public override bool IsVisible
        {
            get { return _powerSHAPE.Visible; }
            set { _powerSHAPE.Visible = value; }
        }

        /// <summary>
        /// Shows/Hides the User Interface (toolbars, etc.).
        /// </summary>
        public override bool IsGUIVisible
        {
            get { return _isGUIVisible; }
            set
            {
                if (value)
                {
                    // Show GUI
                    if (Version.Major >= 18)
                    {
                        DoCommand("GUI MENUS ON", "GUI TOOLBARS ON", "GUI STATUSINFO ON", "GUI STATUSBAR ON", "EDIT RIBBON MINIMISE_RIBBON FALSE");
                    }
                    else
                    {
                        DoCommand("GUI MENUS ON", "GUI TOOLBARS ON", "GUI STATUSINFO ON", "GUI STATUSBAR ON");
                    }

                    DialogsOn();
                    FormUpdateOn();

                    try
                    {
                        if (Version.Major >= 8)
                        {
                            DoCommand("TOOLBAR SURFCURVEEDIT RAISE");
                        }
                    }
                    catch
                    {
                    }

                    DoCommand("TOOLBAR LEVEL RAISE", "TOOLBAR VIEWS RAISE");
                }
                else
                {
                    // Hide GUI
                    if (Version.Major >= 18)
                    {
                        DoCommand("GUI MENUS OFF", "GUI TOOLBARS OFF", "GUI STATUSINFO OFF", "GUI STATUSBAR OFF", "EDIT RIBBON MINIMISE_RIBBON TRUE");
                    }
                    else
                    {
                        DoCommand("GUI MENUS OFF", "GUI TOOLBARS OFF", "GUI STATUSINFO OFF", "GUI STATUSBAR OFF");
                    }
                        
                    DialogsOff();
                    FormUpdateOff();

                    try
                    {
                        if (Version.Major >= 8)
                        {
                            DoCommand("TOOLBAR SURFCURVEEDIT LOWER");
                        }
                    }
                    catch
                    {
                    }

                    DoCommand("TOOLBAR LEVEL LOWER", "TOOLBAR VIEWS LOWER");
                }
                _isGUIVisible = value;
            }
        }

        #endregion

        #region " Execute Operations "

        /// <summary>
        /// Runs an array of commands in PowerSHAPE.  It is marked as obsolete as all commands should be
        /// available as functions in this Automation Interface.
        /// </summary>
        /// <param name="commands">The commands to execute.</param>
        /// <remarks></remarks>
        [Obsolete("This function should only be used when no alternative is available in Custom Software Core." +
                  "You should request the functionality you need to be added via the Custom Software Core forum at https://forums.autodesk.com/t5/powershape-and-powermill-api/bd-p/298")]
        public void Execute(params string[] commands)
        {
            DoCommand(commands);
        }

        /// <summary>
        /// Evaluates the given command in PowerSHAPE.  It is marked as obsolete as all commands should be
        /// available as functions in this Automation Interface.
        /// </summary>
        /// <param name="command">The commands to execute.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [Obsolete("This function should only be used when no alternative is available in Custom Software Core." +
                  "You should request the functionality you need to be added via the Custom Software Core forum at https://forums.autodesk.com/t5/powershape-and-powermill-api/bd-p/298")]
        public object ExecuteEx(string command)
        {
            return DoCommandEx(command);
        }

        /// <summary>
        /// Runs an array of commands in PowerSHAPE.  It is a friend version equivalent to Execute
        /// but does not give the obsolete warning.
        /// </summary>
        internal void DoCommand(params string[] commands)
        {
            foreach (string i in commands)
            {
                _powerSHAPE.Exec(i);
            }
        }

        /// <summary>
        /// Runs the given command in PowerSHAPE.  It is a friend version equivalent to ExecuteEx
        /// but does not give the obsolete warning.
        /// </summary>
        internal object DoCommandEx(string command)
        {
            return _powerSHAPE.Evaluate(command);
        }

        /// <summary>
        /// Runs the given command in PowerSHAPE and return the string result.
        /// </summary>
        internal string ReadStringValue(string command)
        {
            return DoCommandEx(command).ToString();
        }

        /// <summary>
        /// Runs the given command in PowerSHAPE and return bool value.
        /// </summary>
        internal bool ReadBoolValue(string command)
        {
            return ReadStringValue(command) == TrueValue;
        }

        /// <summary>
        /// Runs the given command in PowerSHAPE and return int value.
        /// </summary>
        internal int ReadIntValue(string command)
        {
            var value = ReadStringValue(command);
            int result;
            int.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// Runs the given command in PowerSHAPE and return double value.
        /// </summary>
        internal double ReadDoubleValue(string command)
        {
            var value = ReadStringValue(command);
            double result;
            double.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// Evaluates whether PowerSHAPE is busy or not.
        /// </summary>
        internal bool IsBusy()
        {
            return _powerSHAPE.Busy();
        }

        #endregion

        #region " Dispose Code "

        /// <summary>
        /// Disposal code closes the PowerSHAPE instance that was created by the constructor.  It exits without
        /// saving changes.  It up to the developer to ensure that all changes are saved prior to exiting.
        /// </summary>
        // To detect redundant calls
        private bool _disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (InstanceReuse != InstanceReuse.UseExistingInstance)
                    {
                        // Exit without saving
                        Quit();
                    }
                }
            }
            _disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region " View Operations "

        /// <summary>
        /// Sets the View Angle of PowerSHAPE.
        /// </summary>
        /// <param name="viewAngle">The view angle to set.</param>
        /// <remarks></remarks>
        public override void SetViewAngle(ViewAngles viewAngle)
        {
            switch (viewAngle)
            {
                case ViewAngles.ISO1:
                    DoCommand("VIEW EDIT ISO1");
                    _lastActivatedView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    DoCommand("VIEW EDIT ISO2");
                    _lastActivatedView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    DoCommand("VIEW EDIT ISO3");
                    _lastActivatedView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    DoCommand("VIEW EDIT ISO4");
                    _lastActivatedView = "ISO4";
                    break;
                case ViewAngles.ViewFromTop:
                    DoCommand("VIEW EDIT TOP");
                    _lastActivatedView = "TOP";
                    break;
                case ViewAngles.ViewFromBottom:
                    DoCommand("VIEW EDIT BOTTOM");
                    _lastActivatedView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    DoCommand("VIEW EDIT LEFT");
                    _lastActivatedView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    DoCommand("VIEW EDIT RIGHT");
                    _lastActivatedView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    DoCommand("VIEW EDIT FRONT");
                    _lastActivatedView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    DoCommand("VIEW EDIT BACK");
                    _lastActivatedView = "BACK";
                    break;
            }
        }

        /// <summary>
        /// Sets a multiple view angle rather than a single view angle.  This will split the screen into
        /// three or four views.
        /// </summary>
        /// <param name="viewAngle">The view angle required.</param>
        public void SetMultipleViewAngle(MultipleViewAngles viewAngle)
        {
            switch (viewAngle)
            {
                case MultipleViewAngles.FirstAngle:
                    DoCommand("VIEW EDIT FIRST_ANGLE");
                    break;
                case MultipleViewAngles.FirstAnglePlusISO:
                    DoCommand("VIEW EDIT FIRST_ANGLE_ISO");
                    break;
                case MultipleViewAngles.ThirdAngle:
                    DoCommand("VIEW EDIT THIRD_ANGLE");
                    break;
                case MultipleViewAngles.ThirdAnglePlusISO:
                    DoCommand("VIEW EDIT THIRD_ANGLE_ISO");
                    break;
            }
        }

        /// <summary>
        /// Sets the view angles and relationships for a 2 split view.  The relationship locks the child's
        /// rotation to that of the parent.
        /// </summary>
        /// <param name="leftAngle">The view angle for the left pane.</param>
        /// <param name="rightAngle">The view angle for the right pane.</param>
        /// <param name="leftRelationship">The relationship of the left pane to the right.</param>
        /// <param name="rightRelationship">The relationship of the right pane to the left.</param>
        /// <remarks></remarks>
        /// .
        public void SetSplit2ViewAngle(
            ViewAngles leftAngle,
            ViewAngles rightAngle,
            ViewRelationships leftRelationship,
            ViewRelationships rightRelationship)
        {
            DoCommand("VIEW EDIT RAISE", "LAYOUT SPLIT2");

            string leftView = "";
            switch (leftAngle)
            {
                case ViewAngles.ISO1:
                    leftView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    leftView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    leftView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    leftView = "ISO4";
                    break;
                case ViewAngles.ViewFromTop:
                    leftView = "TOP";
                    break;
                case ViewAngles.ViewFromBottom:
                    leftView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    leftView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    leftView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    leftView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    leftView = "BACK";
                    break;
            }
            DoCommand("LEFTSIDE " + leftView);

            string rightView = "";
            switch (rightAngle)
            {
                case ViewAngles.ISO1:
                    rightView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    rightView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    rightView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    rightView = "ISO4";
                    break;
                case ViewAngles.ViewFromTop:
                    rightView = "TOP";
                    break;
                case ViewAngles.ViewFromBottom:
                    rightView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    rightView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    rightView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    rightView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    rightView = "BACK";
                    break;
            }
            DoCommand("RIGHTSIDE " + rightView);

            if ((leftRelationship == ViewRelationships.Parent) & (rightRelationship == ViewRelationships.Child))
            {
                DoCommand("LEFTSIDE PARENT", "RIGHTSIDE CHILD");
                _lastActivatedView = leftView;
            }
            else if ((leftRelationship == ViewRelationships.Child) & (rightRelationship == ViewRelationships.Parent))
            {
                DoCommand("RIGHTSIDE PARENT", "LEFTSIDE CHILD");
                _lastActivatedView = rightView;
            }
            else
            {
                // All other combindations invalid so set unrelated
                DoCommand("LEFTSIDE UNRELATED", "RIGHTSIDE UNRELATED");
            }

            DoCommand("APPLY", "ACCEPT");
        }

        /// <summary>
        /// Sets the view angles and relationships for a 4 split view.  The relationship locks the child's
        /// rotation to that of the parent.
        /// </summary>
        /// <param name="topLeftAngle">The view angle for the top left pane.</param>
        /// <param name="topRightAngle">The view angle for the top right pane.</param>
        /// <param name="bottomLeftAngle">The view angle for the bottom left pane.</param>
        /// <param name="bottomRightAngle">The view angle for the bottom right pane.</param>
        /// <param name="topLeftRelationship">The relationship of the top left pane to the other panes.</param>
        /// <param name="topRightRelationship">The relationship of the top right pane to the other panes.</param>
        /// <param name="bottomLeftRelationship">The relationship of the bottom left pane to the other panes.</param>
        /// <param name="bottomRightRelationship">The relationship of the bottom right pane to the other panes.</param>
        public void SetSplit4ViewAngle(
            ViewAngles topLeftAngle,
            ViewAngles topRightAngle,
            ViewAngles bottomLeftAngle,
            ViewAngles bottomRightAngle,
            ViewRelationships topLeftRelationship,
            ViewRelationships topRightRelationship,
            ViewRelationships bottomLeftRelationship,
            ViewRelationships bottomRightRelationship)
        {
            DoCommand("VIEW EDIT RAISE", "LAYOUT SPLIT4");

            string topLeftView = "";
            switch (topLeftAngle)
            {
                case ViewAngles.ISO1:
                    topLeftView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    topLeftView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    topLeftView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    topLeftView = "ISO4";
                    break;
                case ViewAngles.ViewFromTop:
                    topLeftView = "TOP";
                    break;
                case ViewAngles.ViewFromBottom:
                    topLeftView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    topLeftView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    topLeftView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    topLeftView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    topLeftView = "BACK";
                    break;
            }
            DoCommand("UPPERLEFT " + topLeftView);

            string topRightView = "";
            switch (topRightAngle)
            {
                case ViewAngles.ISO1:
                    topRightView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    topRightView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    topRightView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    topRightView = "ISO4";
                    break;
                case ViewAngles.ViewFromTop:
                    topRightView = "TOP";
                    break;
                case ViewAngles.ViewFromBottom:
                    topRightView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    topRightView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    topRightView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    topRightView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    topRightView = "BACK";
                    break;
            }
            DoCommand("UPPERRIGHT " + topRightView);

            string bottomLeftView = "";
            switch (bottomLeftAngle)
            {
                case ViewAngles.ISO1:
                    bottomLeftView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    bottomLeftView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    bottomLeftView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    bottomLeftView = "ISO4";
                    break;
                case ViewAngles.ViewFromBottom:
                    bottomLeftView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    bottomLeftView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    bottomLeftView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    bottomLeftView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    bottomLeftView = "BACK";
                    break;
            }
            DoCommand("LOWERLEFT " + bottomLeftView);

            string bottomRightView = "";
            switch (bottomRightAngle)
            {
                case ViewAngles.ISO1:
                    bottomRightView = "ISO1";
                    break;
                case ViewAngles.ISO2:
                    bottomRightView = "ISO2";
                    break;
                case ViewAngles.ISO3:
                    bottomRightView = "ISO3";
                    break;
                case ViewAngles.ISO4:
                    bottomRightView = "ISO4";
                    break;
                case ViewAngles.ViewFromBottom:
                    bottomRightView = "BOTTOM";
                    break;
                case ViewAngles.ViewFromLeft:
                    bottomRightView = "LEFT";
                    break;
                case ViewAngles.ViewFromRight:
                    bottomRightView = "RIGHT";
                    break;
                case ViewAngles.ViewFromFront:
                    bottomRightView = "FRONT";
                    break;
                case ViewAngles.ViewFromBack:
                    bottomRightView = "BACK";
                    break;
            }
            DoCommand("LOWERRIGHT " + bottomRightView);

            if (topLeftRelationship == ViewRelationships.Parent)
            {
                DoCommand("UPPERLEFT PARENT");
                if (topRightRelationship == ViewRelationships.Child)
                {
                    DoCommand("UPPERRIGHT CHILD");
                }
                if (bottomLeftRelationship == ViewRelationships.Child)
                {
                    DoCommand("LOWERLEFT CHILD");
                }
                if (bottomRightRelationship == ViewRelationships.Child)
                {
                    DoCommand("LOWERRIGHT CHILD");
                }
            }
            else if (topRightRelationship == ViewRelationships.Parent)
            {
                DoCommand("UPPERRIGHT PARENT");
                if (topLeftRelationship == ViewRelationships.Child)
                {
                    DoCommand("UPPERLEFT CHILD");
                }
                if (bottomLeftRelationship == ViewRelationships.Child)
                {
                    DoCommand("LOWERLEFT CHILD");
                }
                if (bottomRightRelationship == ViewRelationships.Child)
                {
                    DoCommand("LOWERRIGHT CHILD");
                }
            }
            else if (bottomLeftRelationship == ViewRelationships.Parent)
            {
                DoCommand("LOWERLEFT PARENT");
                if (topLeftRelationship == ViewRelationships.Child)
                {
                    DoCommand("UPPERLEFT CHILD");
                }
                if (topRightRelationship == ViewRelationships.Child)
                {
                    DoCommand("UPPERRIGHT CHILD");
                }
                if (bottomRightRelationship == ViewRelationships.Child)
                {
                    DoCommand("LOWERRIGHT CHILD");
                }
            }
            else if (bottomRightRelationship == ViewRelationships.Parent)
            {
                DoCommand("LOWERRIGHT PARENT");
                if (topLeftRelationship == ViewRelationships.Child)
                {
                    DoCommand("UPPERLEFT CHILD");
                }
                if (topRightRelationship == ViewRelationships.Child)
                {
                    DoCommand("UPPERRIGHT CHILD");
                }
                if (bottomLeftRelationship == ViewRelationships.Child)
                {
                    DoCommand("LOWERLEFT CHILD");
                }
            }
            else
            {
                DoCommand("UPPERLEFT UNRELATED", "UPPERRIGHT UNRELATED", "LOWERLEFT UNRELATED", "LOWERRIGHT UNRELATED");
            }

            DoCommand("APPLY", "ACCEPT");
        }

        /// <summary>
        /// Resize the view to fit all visible items.
        /// </summary>
        public void ResizeViewToFit()
        {
            DoCommand("ZOOM FULL");
        }

        /// <summary>
        /// Sets the active plane of PowerSHAPE.
        /// </summary>
        /// <param name="desiredPlane">The plane to activate.</param>
        /// <remarks></remarks>
        public void SetActivePlane(Planes desiredPlane)
        {
            switch (desiredPlane)
            {
                case Planes.XY:
                case Planes.YZ:
                case Planes.ZX:
                    DoCommand("PRINCIPALPLANE " + desiredPlane);
                    break;
            }
        }

        /// <summary>
        /// Sets the view to show wireframes with a shaded model.
        /// </summary>
        public void SetShadedWireframeView()
        {
            DoCommand("VIEW DETAIL SHADEDWIRE");
        }

        /// <summary>
        /// Sets the view to show wireframes.
        /// </summary>
        public void SetWireframeView()
        {
            DoCommand("VIEW DETAIL WIRE");
        }

        /// <summary>
        /// Sets the view to show wireframes including hidden wireframe.
        /// </summary>
        public void SetHiddenWireframeView()
        {
            DoCommand("VIEW DETAIL HIDDENWIRE");
        }

        /// <summary>
        /// Sets the view to enhanced shading.
        /// </summary>
        public void SetEnhancedShading()
        {
            DoCommand("VIEW DETAIL ENHANCEDSHADED");
        }

        /// <summary>
        /// Sets the view to normal shading.
        /// </summary>
        public void SetShadedView()
        {
            DoCommand("VIEW DETAIL SHADED");
        }

        /// <summary>
        /// Toggles transparent shading.
        /// </summary>
        public void ToggleTransparentView()
        {
            DoCommand("VIEW DETAIL TOGGLE TRANSPARENT");
        }

        /// <summary>
        /// Sets the view to show shaded wireframe.
        /// </summary>
        public void SetShadedAndWireframeView()
        {
            DoCommand("VIEW DETAIL SHADEDWIRE");
        }

        /// <summary>
        /// Sets the view for a Mesh to show open edges with the triangles shaded.
        /// </summary>
        public void SetMeshViewOpenEdgesOnlyShaded()
        {
            DoCommand("VIEW MESHREP NOEDGES");
        }

        /// <summary>
        /// Sets the view for a Mesh to show open edges and triangle edges with the triangles shaded.
        /// </summary>
        public void SetMeshViewOpenAndInteriorEdgesShaded()
        {
            DoCommand("VIEW MESHREP SOLIDEDGES");
        }

        /// <summary>
        /// Sets the view for a Mesh to show open edges and triangle edges and triangle nodes
        /// with the triangles shaded.
        /// </summary>
        public void SetMeshViewOpenAndInteriorEdgesAndNodesShaded()
        {
            DoCommand("VIEW MESHREP SOLIDNODES");
        }

        /// <summary>
        /// Sets the view for a Mesh to show open edges and triangle edges with the triangles hollow.
        /// </summary>
        public void SetMeshViewOpenAndInteriorEdgesHollow()
        {
            DoCommand("VIEW MESHREP HOLLOWEDGES");
        }

        /// <summary>
        /// Sets the view for a Mesh to show open edges and triangle edges and triangle nodes
        /// with the triangles hollow.
        /// </summary>
        public void SetMeshViewOpenAndInteriorEdgesAndNodesHollow()
        {
            DoCommand("VIEW MESHREP HOLLOWNODES");
        }

        /// <summary>
        /// Sets the shading tolerance for PowerSHAPE and regenerates triangles
        /// </summary>
        /// <param name="tolerance">The required shading tolerance</param>
        public void SetShadingTolerance(double tolerance)
        {
            DoCommand("TOOLS PREFERENCES", "SHADING TOLERANCE " + tolerance, "SHADING Retriangulate", "ACCEPT");
        }

        #endregion

        #region " Dynamic Sectioning "

        /// <summary>
        /// Open/Reset the Dynamic Sectioning view.
        /// </summary>
        /// <param name="drawEdges">Select this option to draw the edges of the section.</param>
        /// <param name="translucency">Select this option to draw the section translucent.</param>
        /// <param name="capSolids">Select this option to display a cap on the solid where it is intersects with the clipping plane.</param>
        /// <remarks></remarks>
        public void ResetDynamicSectioning(bool drawEdges = false, bool translucency = false, bool capSolids = false)
        {
            CloseDynamicSectioning();

            // Register the active view before opening the dynamic sectioning
            DoCommand("VIEW SELECT " + _lastActivatedView);

            // Open Dynamic sectioning
            DoCommand("VIEW CLIPPLANES RAISE");

            // Set draw edges
            SetDynamicSectioningDrawEdges(drawEdges);

            // Set translucency
            SetDynamicSectioningTranslucency(translucency);

            // Set cap solids
            SetDynamicSectioningCapSolids(capSolids);
        }

        /// <summary>
        /// Sets whether edges are draw in dynamic sectioning mode.
        /// </summary>
        /// <param name="drawEdges">Draw edges in dynamic sectioning mode.</param>
        public void SetDynamicSectioningDrawEdges(bool drawEdges)
        {
            if (drawEdges)
            {
                DoCommand("VIEW CLIPPLANES EDGES ON");
            }
            else
            {
                DoCommand("VIEW CLIPPLANES EDGES OFF");
            }
        }

        /// <summary>
        /// Sets whether transparency is shown in dynamic sectioning mode.
        /// </summary>
        /// <param name="translucency">Shows transparency in dynamic sectioning mode.</param>
        public void SetDynamicSectioningTranslucency(bool translucency)
        {
            if (translucency)
            {
                DoCommand("VIEW CLIPPLANES ACTION TRANSLUCENT");
            }
            else
            {
                DoCommand("VIEW CLIPPLANES ACTION INVISIBLE");
            }
        }

        /// <summary>
        /// Sets whether cap solids are draw in dynamic sectioning mode.
        /// </summary>
        /// <param name="capSolids">Draw cap solids in dynamic sectioning mode.</param>
        public void SetDynamicSectioningCapSolids(bool capSolids)
        {
            if (capSolids)
            {
                DoCommand("VIEW CLIPPLANES CAPSOLIDS ON");
            }
            else
            {
                DoCommand("VIEW CLIPPLANES CAPSOLIDS OFF");
            }
        }

        /// <summary>
        /// Closes the Dynamic Sectioning view.
        /// </summary>
        public void CloseDynamicSectioning()
        {
            DoCommand("VIEW CLIPPLANES CLOSE");
        }

        /// <summary>
        /// Set the position of the front plane for Dynamic Sectioning.
        /// </summary>
        /// <param name="planePosition">The position of the front plane.</param>
        public void SetDynamicSectioningFrontPlane(double planePosition)
        {
            DoCommand("VIEW CLIPPLANES FRONT POSITION " + planePosition);
        }

        /// <summary>
        /// Creates a workplane at the centre of the front plane for Dynamic Sectioning.
        /// </summary>
        /// <returns>The created Workplane or Nothing if this failed.</returns>
        public PSWorkplane CreateWorkplaneAtFrontPlane()
        {
            // Create a workplane at the centre of the front plane
            ActiveModel.ClearCreatedItems();
            DoCommand("VIEW CLIPPLANES FRONT WORKPLANE");

            // Add any created items (should be one at most) to the model's collections
            foreach (PSEntity createdItem in ActiveModel.CreatedItems)
            {
                ActiveModel.Add(createdItem);
            }

            // If there was only one then return it, it will be the workplane
            if (ActiveModel.CreatedItems.Count == 1)
            {
                return (PSWorkplane) ActiveModel.CreatedItems[0];
            }
            return null;
        }

        /// <summary>
        /// Creates composite curves through the model at the front plane for Dynamic Sectioning.
        /// </summary>
        /// <returns>A list of created curves</returns>
        public List<PSCompCurve> CreateWireframeAtFrontPlane()
        {
            // Create the wireframe at back plane
            ActiveModel.ClearCreatedItems();
            DoCommand("VIEW CLIPPLANES FRONT WIREFRAME");

            // Add any created items to the model's collections
            List<PSCompCurve> curves = new List<PSCompCurve>();
            foreach (PSEntity createdItem in ActiveModel.CreatedItems)
            {
                if (createdItem is PSCompCurve)
                {
                    ActiveModel.Add(createdItem);
                    curves.Add((PSCompCurve) createdItem);
                }
            }

            return curves;
        }

        /// <summary>
        /// Sets the position of the rear plane for Dynamic Sectioning.
        /// </summary>
        /// <param name="planePosition">The position of the rear plane.</param>
        public void SetDynamicSectioningRearPlane(double planePosition)
        {
            DoCommand("VIEW CLIPPLANES BACK POSITION " + planePosition);
        }

        /// <summary>
        /// Creates a workplane at the centre of the rear plane for Dynamic Sectioning.
        /// </summary>
        /// <returns>The created Workplane or Nothing if this failed.</returns>
        public PSWorkplane CreateWorkplaneAtRearPlane()
        {
            // Create a workplane at the centre of the back plane
            ActiveModel.ClearCreatedItems();
            DoCommand("VIEW CLIPPLANES BACK WORKPLANE");

            // Add any created items (should be one at most) to the model's collections
            foreach (PSEntity createdItem in ActiveModel.CreatedItems)
            {
                ActiveModel.Add(createdItem);
            }

            // If there was only one then return it, it will be the workplane
            if (ActiveModel.CreatedItems.Count == 1)
            {
                return (PSWorkplane) ActiveModel.CreatedItems[0];
            }
            return null;
        }

        /// <summary>
        /// Creates composite curves through the model at the rear plane for Dynamic Sectioning.
        /// </summary>
        /// <returns>The list of created curves.</returns>
        public List<PSCompCurve> CreateWireframeAtRearPlane()
        {
            // Create the wireframe at back plane
            ActiveModel.ClearCreatedItems();
            DoCommand("VIEW CLIPPLANES BACK WIREFRAME");

            // Add any created items to the model's collections
            List<PSCompCurve> curves = new List<PSCompCurve>();
            foreach (PSEntity createdItem in ActiveModel.CreatedItems)
            {
                ActiveModel.Add(createdItem);
                curves.Add((PSCompCurve) createdItem);
            }

            return curves;
        }

        #endregion

        #region " Active Document "

        /// <summary>
        /// Gets the ActiveDocument object for PowerSHAPE.
        /// </summary>
        internal dynamic ActiveDocument
        {
            get { return _powerSHAPE.ActiveDocument; }
        }

        #endregion

        #region " Properties "

        /// <summary>
        /// The name of the process in the process list.
        /// </summary>
        protected override string ExecutableName
        {
            get { return EXE_NAME; }
        }

        /// <summary>
        /// Process id of the application instance.
        /// </summary>
        public override int ProcessId => int.Parse(DoCommandEx("app.pid").ToString());

        /// <summary>
        /// The class Id to use based on the Application Mode selected through the constructor.
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        private string ClassId
        {
            get
            {
                switch (_applicationMode)
                {
                    case Modes.SurfacerMode:
                        return PS_SURFACER_CLASS_ID;
                    case Modes.EstimatorMode:
                        return PS_ESTIMATOR_CLASS_ID;
                    case Modes.PMillModellingMode:
                        return PMILL_MODELLING_CLASS_ID;
                    case Modes.ElectrodemakerMode:
                        return PMILL_ELECTRODE_CLASS_ID;
                    default:
                        return CLASS_ID;
                }
            }
        }

        /// <summary>
        /// Gets the arguments to use based on the Application Mode selected through the constructor.
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        private string CommandArguments
        {
            get
            {
                switch (_applicationMode)
                {
                    case Modes.SurfacerMode:
                        return PS_SURFACER_ARGUMENTS;
                    case Modes.EstimatorMode:
                        return PS_ESTIMATOR_ARGUMENTS;
                    case Modes.PMillModellingMode:
                        return PMILL_MODELLING_ARGUMENTS;
                    case Modes.ElectrodemakerMode:
                        return PMILL_ELECTRODE_ARGUMENTS;
                    case Modes.EvaluationMode:
                        return PS_E_ARGUMENTS;
                    case Modes.PShapeNoParaSolidMode:
                    case Modes.ProNoParaSolidMode:
                        return ARGUMENTS + " -noparasolid";
                    default:
                        return ARGUMENTS;
                }
            }
        }

        /// <summary>
        /// Gets the mode in which PowerSHAPE is running.
        /// </summary>
        public Modes Mode
        {
            get
            {
                if (ReadIntValue("ARM_MODE") == 1)
                {
                    return Modes.ArmMode;
                }
                if (ReadIntValue("DRAFT_MODE") == 1)
                {
                    return Modes.DraftMode;
                }
                if (ReadIntValue("ELECTRODEMAKER_MODE") == 1)
                {
                    return Modes.ElectrodemakerMode;
                }
                if (ReadIntValue("ESTIMATOR_MODE") == 1)
                {
                    return Modes.EstimatorMode;
                }
                if (ReadIntValue("EVALUATION_MODE") == 1)
                {
                    return Modes.EvaluationMode;
                }
                if (ReadIntValue("JEWELSMITH_MODE") == 1)
                {
                    return Modes.JewelsmithMode;
                }
                if (ReadIntValue("MOLDMAKER_MODE") == 1)
                {
                    return Modes.MoldmakerMode;
                }
                if (ReadIntValue("PMILL_MODELLING_MODE") == 1)
                {
                    return Modes.PMillModellingMode;
                }
                if (ReadIntValue("PRESSMAKER_MODE") == 1)
                {
                    return Modes.PressmakerMode;
                }
                if (ReadIntValue("DESIGNERS_MODE") == 1)
                {
                    return Modes.ProMode;
                }
                if (ReadIntValue("PSHAPE_MODE") == 1)
                {
                    return Modes.PShapeMode;
                }
                if (ReadIntValue("SHOEMAKER_MODE") == 1)
                {
                    return Modes.ShoemakerMode;
                }
                if (ReadIntValue("SURFACER_MODE") == 1)
                {
                    return Modes.SurfacerMode;
                }
                if (ReadIntValue("LICENCE_CHECK_MODE") == 1)
                {
                    return Modes.ArmMode;
                }

                // The current mode is undocumented
                return Modes.Unknown;
            }
        }

        /// <summary>
        /// Gets/sets whether PowerSHAPE is in surface trim region editing mode.
        /// </summary>
        public bool IsInTrimRegionEditing
        {
            get { return _isInTrimRegionEditing; }
            set
            {
                if (value)
                {
                    DoCommand("TRIMREGIONEDIT");
                    _isInTrimRegionEditing = true;
                }
                else
                {
                    DoCommand("TOOLBAR TREDIT LOWER SELECT");
                    _isInTrimRegionEditing = false;
                }
            }
        }

        /// <summary>
        /// Toggles the current trim editing mode between boundary editing and pCurve editing.
        /// </summary>
        public TrimRegionEditingModes TrimRegionEditingMode
        {
            get { return _trimRegionEditingMode; }
            set
            {
                // Make sure PowerSHAPE is in trim region editing
                IsInTrimRegionEditing = true;
                _trimRegionEditingMode = value;

                if (_trimRegionEditingMode == TrimRegionEditingModes.BoundaryEditing)
                {
                    // Switch on boundary editing
                    DoCommand("EDITBOUNDARY");
                }
                else
                {
                    // Switch on pCurve editing
                    DoCommand("EDITPCURVE");
                }
            }
        }

        /// <summary>
        /// Gets and sets the drawing tolerance of all models.
        /// </summary>
        public double DrawingTolerance
        {
            get { return (double) DoCommandEx("TOLERANCE.DRAWING"); }
            set
            {
                // Open the options dialog
                OpenOptions(OptionsSections.UNITPREFS);

                // Change the tolerance
                DoCommand("DRAWTOL " + value);

                // Close the options dialog
                CloseOptions();
            }
        }

        /// <summary>
        /// Gets and sets whether the intelligent cursor is on or off.
        /// </summary>
        public bool IsIntelligentCursorOn
        {
            get { return _isIntelligentCursorOn; }
            set
            {
                if (value != _isIntelligentCursorOn)
                {
                    _isIntelligentCursorOn = value;
                    if (value)
                    {
                        DoCommand("TOOLS PREFERENCES CURSOR ON", "ACCEPT");
                    }
                    else
                    {
                        DoCommand("TOOLS PREFERENCES CURSOR OFF", "ACCEPT");
                    }
                }
            }
        }

        /// <summary>
        /// Sets whether to arc fit comp curves when importing DUCT Picture files
        /// </summary>
        public bool ArcFitPicFiles
        {
            set { DoCommand("TOOLS PREFERENCES", "UNITPREFS", "PICPREFS", "ARCFIT PIC " + value.ToOnOff(), "ACCEPT"); }
        }

        #region " Model Properties "

        /// <summary>
        /// Gets the active window from within PowerSHAPE.
        /// </summary>
        public PSWindow ActiveWindow
        {
            get { return _windows[DoCommandEx("WINDOW.SELECTED").ToString()]; }
        }

        /// <summary>
        /// Gets the Active Model from within PowerSHAPE.
        /// </summary>
        public PSModel ActiveModel
        {
            get { return _models[Convert.ToString(DoCommandEx("MODEL.SELECTED"))]; }
        }

        /// <summary>
        /// Gets the list of Models currently open in PowerSHAPE.
        /// </summary>
        /// <returns>The list of Models currently open in PowerSHAPE.</returns>
        public PSModelsCollection Models
        {
            get { return _models; }
        }

        /// <summary>
        /// Gets the list of Windows currently open in PowerSHAPE.
        /// </summary>
        /// <returns></returns>
        /// <value></value>
        public PSWindowsCollection Windows
        {
            get { return _windows; }
        }

        #endregion

        #region " Curves Properties "

        /// <summary>
        /// Sets whether the Curvature Combs are visible or not.
        /// </summary>
        public bool IsCurveCurvatureCombsVisible
        {
            set
            {
                if (value)
                {
                    DoCommand("COMB ON");
                }
                else
                {
                    DoCommand("COMB OFF");
                }
            }
        }

        /// <summary>
        /// Sets whether the Point Labels are visible or not.
        /// </summary>
        public bool IsCurvePointLabelsVisible
        {
            set
            {
                if (value)
                {
                    DoCommand("LABEL ON");
                }
                else
                {
                    DoCommand("LABEL OFF");
                }
            }
        }

        /// <summary>
        /// Sets whether the Curve apply smoothing to end points is enabled.
        /// Create a dummy curve and delete it to refresh Powershape button state.
        /// </summary>
        public bool IsCurveApplySmoothness
        {
            set
            {
                var dummyCurve = ActiveModel.Curves.CreateCurveThroughPoints(CurveTypes.Bezier,
                                                                             new[]
                                                                             {
                                                                                 new Geometry.Point(0, 0, 0),
                                                                                 new Geometry.Point(1, 1, 1),
                                                                                 new Geometry.Point(2, 2, 2)
                                                                             });

                if (value)
                {
                    DoCommand("SMOOTH ON");
                }
                else
                {
                    DoCommand("SMOOTH OFF");
                }

                dummyCurve.Delete();
            }
        }

        #endregion

        #region " Plane Properties "

        /// <summary>
        /// Sets whether or not planes are locked.
        /// </summary>
        public bool IsPlanesLocked
        {
            set
            {
                if (value)
                {
                    DoCommand("LOCK ON");
                }
                else
                {
                    DoCommand("LOCK OFF");
                }
            }
        }

        #endregion

        #endregion

        #region " Operations "

        /// <summary>
        /// Switches the dialogs on in PowerSHAPE.
        /// </summary>
        public void DialogsOn()
        {
            // Switch dialogs on
            DoCommand("DIALOG ON");
        }

        /// <summary>
        /// Switches the dialogs off in PowerSHAPE.
        /// </summary>
        public void DialogsOff()
        {
            // Switch dialogs off
            DoCommand("DIALOG OFF");
        }

        internal bool IsFormUpdateOff { get; set; }

        /// <summary>
        /// Turns Forms on.  Any forms raised will be shown on screen.
        /// </summary>
        public void FormUpdateOn()
        {
            // Turn forms On
            DoCommand("FORMUPDATE ON", "FORM DELETION ON", "TOOLS PREFERENCES", "SAVETIME ON", "ACCEPT");

            IsFormUpdateOff = false;
        }

        /// <summary>
        /// Turns Forms off.  Any forms raised will not be shown.
        /// </summary>
        public void FormUpdateOff()
        {
            // Turn forms off
            DoCommand("FORMUPDATE OFF", "FORM DELETION OFF", "TOOLS PREFERENCES", "SAVETIME OFF", "ACCEPT");

            IsFormUpdateOff = true;
        }

        /// <summary>
        /// Turns graphics updates on.
        /// </summary>
        public void RefreshOn()
        {
            //Workaround to prevent the following issue ACCS-471: REFRESH ON FORCE won't work if curve selected
            ActiveModel.ClearSelectedItems();

            if (Version < new Version("17.1.34"))
            {
                // Turns graphics updates on
                DoCommand("REFRESH ON");
            }
            else
            {
                // Turns graphics updates on. This command ensures that graphics will update no matter how many times REFRESH OFF is executed (pshape#29989).
                DoCommand("REFRESH ON FORCE");
            }
        }

        /// <summary>
        /// Turns graphics updates off.
        /// </summary>
        public void RefreshOff()
        {
            //Workaround to prevent the following issue ACCS-471: REFRESH ON FORCE won't work if curve selected
            ActiveModel.ClearSelectedItems();

            // Turns graphics updates off
            DoCommand("REFRESH OFF");
        }

        /// <summary>
        /// Opens the options menu found in Tools.
        /// </summary>
        /// <param name="sectionToOpen">This is the desired section of the options menu</param>
        internal void OpenOptions(OptionsSections sectionToOpen)
        {
            // Open tools dialog
            DoCommand("TOOLS PREFERENCES");

            // Go to desired section
            DoCommand(sectionToOpen.ToString());
        }

        /// <summary>
        /// Closes the Tools option menu.
        /// </summary>
        internal void CloseOptions()
        {
            // Close tools dialog
            DoCommand("ACCEPT");
        }

        #endregion

        #region " User Information "

        /// <summary>
        /// The user's login
        /// </summary>
        /// <returns></returns>
        public string UserLogin
        {
            get { return DoCommandEx("USER.LOGIN").ToString(); }
        }

        /// <summary>
        /// The user's name
        /// </summary>
        /// <returns></returns>
        public string UserName
        {
            get { return DoCommandEx("USER.NAME").ToString(); }
        }

        #endregion

        #region " Point Picking "

        /// <summary>
        /// Starts a point picking function in PowerSHAPE and will wait until the point is picked.
        /// </summary>
        /// <param name="cancelPointPickingCheck">Contains a previous state of CancelPointPickCheck.</param>
        /// <returns>The point picked by the user or nothing if cancelled.</returns>
        /// <remarks></remarks>
        public Geometry.Point PickPoint(ICancelPointPickCheck cancelPointPickingCheck = null)
        {
            Geometry.Point pickedPoint = null;
            DoCommand("INPUT POINT'' $PickPoint");

            bool isPointPickingCancelled = false;

            // Wait for pick in PowerSHAPE
            Thread.Sleep(1000);
            do
            {
                Thread.Sleep(100);
                if (cancelPointPickingCheck != null && cancelPointPickingCheck.IsCancelPointPick())
                {
                    isPointPickingCancelled = true;
                    DoCommand("CANCEL");
                    break; // TODO: might not be correct. Was : Exit Do
                }
            } while (!(IsBusy() == false));

            if (isPointPickingCancelled == false)
            {
                Geometry.MM x = Convert.ToDouble(DoCommandEx("$PickPoint_x"));
                Geometry.MM y = Convert.ToDouble(DoCommandEx("$PickPoint_y"));
                Geometry.MM z = Convert.ToDouble(DoCommandEx("$PickPoint_z"));
                pickedPoint = new Geometry.Point(x, y, z);
            }

            return pickedPoint;
        }

        #endregion
    }
}