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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Autodesk.ProductInterface.PowerMILL
{
    /// <summary>
    /// This is the root class of a PowerMILL instance.
    /// When it is created it creates its own instance of PowerMILL.
    /// When it is destroyed the instance of PowerMILL will also be destroyed.
    /// </summary>
    public class PMAutomation : Automation, IDisposable
    {
        #region Fields

        /// <summary>
        /// The PowerMILL COM object.
        /// </summary>
        private dynamic _powerMILL;

        /// <summary>
        /// The ClassId of the PowerMILL COM object.
        /// </summary>
        private const string CLASS_ID = "PowerMILL.Application";

        /// <summary>
        /// The ClassId of the PowerMILL COM object pre version 9.
        /// </summary>
        private const string OLD_CLASS_ID = "PMILL.Document";

        /// <summary>
        /// The name of the application in the registry.
        /// </summary>
        private const string APP_NAME = "PowerMILL";

        /// <summary>
        /// The name of the application's executable.
        /// </summary>
        private const string EXE_NAME = "PMILL";

        /// <summary>
        /// The arguments to run the executable.
        /// </summary>
        /// <remarks></remarks>
        private const string ARGUMENTS = "-automation -nogui";

        /// <summary>
        /// These are the substitution tokens that will be used by any executing macros.
        /// </summary>
        private PMSubstitutionTokensCollection _substitutionTokens;

        /// <summary>
        /// This is the active project.
        /// </summary>
        private PMProject _activeProject;

        /// <summary>
        /// Application mode.
        /// </summary>
        private Modes _applicationMode;

        #endregion

        #region Constructors

        /// <summary>
        /// Connect to an instance of PowerMill that is passed in (this should be the COM object)
        /// </summary>
        /// <param name="powerMillComObject">The COM object of PowerMill to connect to.</param>
        public PMAutomation(dynamic powerMillComObject)
        {
            _powerMILL = powerMillComObject;

            // Dialogs are on by default
            _isGUIVisible = true;

            // Initialise the Macro substitution tokens collection
            _substitutionTokens = new PMSubstitutionTokensCollection();
            IssueEchoOffCommands = false;
        }

        /// <summary>
        /// Calls down to the base class to start PowerMILL based on the specified
        /// option.
        /// </summary>
        /// <param name="instanceReuse">Specifies how to start the application (e.g. use the running instance, create a new instance).</param>
        /// <param name="applicationMode">Represents the GUI option in PowerMill.</param>
        /// <remarks></remarks>
        public PMAutomation(InstanceReuse instanceReuse, Modes applicationMode = Modes.WithoutGui)
        {
            _applicationMode = applicationMode;
            Initialise(instanceReuse);

            // Ensure PowerMILL has started properly
            while (DoCommandEx("PRINT \"TEST\"").ToString() != "TEST")
            {
                System.Threading.Thread.Sleep(1000);
                DoCommand("ECHO OFF DCPDEBUG UNTRACE COMMAND ACCEPT");
            }

            // Dialogs are on by default
            _isGUIVisible = true;

            // Initialise the Macro substitution tokens collection
            _substitutionTokens = new PMSubstitutionTokensCollection();
            IssueEchoOffCommands = false;
        }

        /// <summary>
        /// Calls down to the base class to start PowerMILL based on the specified options.
        /// </summary>
        /// <param name="instanceReuse">Specifies how to start the application (e.g. use the running instance, create a new instance).</param>
        /// <param name="version">The PowerMill version to start.</param>
        /// <param name="maximumVersion">It will ensure that the PowerMill version to run will be below maximumVersion.</param>
        /// <param name="applicationMode">Represents the GUI option in PowerMill.</param>
        /// <remarks></remarks>
        public PMAutomation(
            InstanceReuse instanceReuse,
            Version version,
            Version maximumVersion = null,
            Modes applicationMode = Modes.WithoutGui)
        {
            _applicationMode = applicationMode;
            Initialise(instanceReuse, version, maximumVersion);

            // Ensure PowerMILL has started properly
            while (DoCommandEx("PRINT \"TEST\"").ToString() != "TEST")
            {
                System.Threading.Thread.Sleep(1000);
                DoCommand("ECHO OFF DCPDEBUG UNTRACE COMMAND ACCEPT");
            }

            //Dialogs are on by default
            _isGUIVisible = true;

            //Initialise the Macro substitution tokens collection
            _substitutionTokens = new PMSubstitutionTokensCollection();
            IssueEchoOffCommands = false;
        }

        #endregion

        #region Operations

        /// <summary>
        /// Attempts to connect to an existing version of PowerMILL or create a new one if no version has been passed.
        /// </summary>
        /// <param name="version">The PowerMill version to start.</param>
        /// <param name="maximumVersion">If filled, it will ensure that the PowerMill version to run will be below maximumVersion.</param>
        protected override void UseExistingInstance(Version version, Version maximumVersion)
        {
            try
            {
                if (version == null)
                {
                    //connect to current running version
                    _powerMILL = Marshal.GetActiveObject(CLASS_ID);
                }
                else
                {
                    //Attempt to connect to an existing instance
                    if (version.Major < 9)
                    {
                        _powerMILL = Marshal.GetActiveObject(OLD_CLASS_ID);
                    }
                    else
                    {
                        _powerMILL = Marshal.GetActiveObject(CLASS_ID);
                    }
                    if (version != null)
                    {
                        if (maximumVersion == null)
                        {
                            if (_powerMILL.Version !=
                                string.Format("{0:#}{1:0}{2:00}", version.Major, version.Minor, version.Build))
                            {
                                throw new Exception("Incorrect version found");
                            }
                        }
                        else
                        {
                            string sver = _powerMILL.Version.ToString();
                            Version foundVersion = null;
                            if (sver.Length > 5)
                            {
                                foundVersion =
                                    new Version(string.Format("{0}.{1}.{2}",
                                                              sver.Substring(0, 4),
                                                              sver.Substring(4, 1),
                                                              sver.Substring(5, 2)));
                            }
                            else if (sver.Length > 4)
                            {
                                foundVersion =
                                    new Version(string.Format("{0}.{1}.{2}",
                                                              sver.Substring(0, 2),
                                                              sver.Substring(2, 1),
                                                              sver.Substring(3, 2)));
                            }
                            else
                            {
                                foundVersion =
                                    new Version(string.Format("{0}.{1}.{2}",
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
                }
            }
            catch
            {
                //Either one didn't exist or it was the wrong version so start a new one
                CreateNewInstance(version, maximumVersion);
            }
        }

        /// <summary>
        /// This operation is not allowed as PowerMILL does not support creating instances via ClassId.
        /// </summary>
        /// <param name="version">The PowerMill version to start.</param>
        /// <param name="maximumVersion">If filled, it will ensure that the PowerMill version to run will be below maximumVersion.</param>
        protected override void CreateNewInstance(Version version, Version maximumVersion)
        {
            string classId = "";
            if (version != null && version.Major < 9)
            {
                classId = OLD_CLASS_ID;
            }
            else
            {
                classId = CLASS_ID;
            }
            if (_applicationMode == Modes.WithGui)
            {
                throw new Exception("CreateNewInstance cannot be used with the WithGui mode");
            }
            try
            {
                _powerMILL = StartByClassId(classId, EXE_NAME, version, maximumVersion);
            }
            catch
            {
                // COM register the correct version and see how that goes
                StartExecutable(APP_NAME, version, maximumVersion, classId, false, "", true);

                // Try connecting again
                _powerMILL = StartByClassId(classId, EXE_NAME, version, maximumVersion);
            }
        }

        /// <summary>
        /// Closes all running instance of PowerMILL and start a new one.
        /// </summary>
        /// <param name="version">The PowerMill version to start.</param>
        /// <param name="maximumVersion">If filled, it will ensure that the PowerMill version to run will be below maximumVersion.</param>
        protected override void CreateSingleInstance(Version version, Version maximumVersion)
        {
            CloseAllInstancesOf(EXE_NAME);
            string classId = "";
            if (version != null && version.Major < 9)
            {
                classId = OLD_CLASS_ID;
            }
            else
            {
                classId = CLASS_ID;
            }
            StartExecutable(APP_NAME, version, maximumVersion, classId, false, ARGUMENTS);
            _powerMILL = Marshal.GetActiveObject(classId);
        }

        /// <summary>
        /// Closes the open project and resets PowerMILL back to an empty initial state.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public PMProject Reset()
        {
            CloseProject();
            return ActiveProject;
        }

        /// <summary>
        /// Closes the instance of the PowerMILL.
        /// </summary>
        public override void Quit()
        {
            Process process;
            try
            {
                process = Process.GetProcessById(ProcessId);
            }
            catch (Exception)
            {
                process = null;
            }

            //Turn shading off to speed it up
            DoCommand("VIEW MODEL ; SHADE OFF");

            //Make sure the explorer will not disappear in future
            DoCommand("SPLITTER TABBROWSER WIDTH 240");
            _powerMILL.Quit();
            while (process != null && process.HasExited == false)
                System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Starts recording a macro.
        /// </summary>
        /// <param name="macroFilename">The file path to which save the macro.</param>
        public override void RecordMacro(string macroFilename)
        {
            DoCommand("MACRO RECORD \"" + macroFilename + "\" YES");
        }

        /// <summary>
        /// Stops recording macro.
        /// </summary>
        public override void StopMacroRecording()
        {
            DoCommand("Macro Stop");
        }

        /// <summary>
        /// Returns a list of PowerMill COM object instances
        /// </summary>
        /// <returns>List of COM objects</returns>
        public static List<object> GetListOfPmComObjects()
        {
            return GetCOMObjectsHelper.GetRunningInstances(CLASS_ID);
        }

        #endregion

        #region Properties

        public override Version Version
        {
            get
            {
                if (_version == null)
                {
                    string sver = _powerMILL.Version.ToString();
                    if (sver.Length > 5)
                    {
                        _version =
                            new Version(string.Format("{0}.{1}.{2}",
                                                      sver.Substring(0, 4),
                                                      sver.Substring(4, 1),
                                                      sver.Substring(5, 2)));
                    }
                    else if (sver.Length > 4)
                    {
                        // PM 2017 and before
                        _version =
                            new Version(string.Format("{0}.{1}.{2}",
                                                      sver.Substring(0, 2),
                                                      sver.Substring(2, 1),
                                                      sver.Substring(3, 2)));
                    }
                    else
                    {
                        // PM 9 and before
                        _version =
                            new Version(string.Format("{0}.{1}.{2}",
                                                      sver.Substring(0, 1),
                                                      sver.Substring(1, 1),
                                                      sver.Substring(2, 2)));
                    }
                }
                return _version;
            }
        }

        /// <summary>
        /// Gets the name of the process in the process list.
        /// </summary>
        protected override string ExecutableName
        {
            get { return EXE_NAME; }
        }

        /// <summary>
        /// Process id of the application instance.
        /// </summary>
        public override int ProcessId => _powerMILL.Debug.ProcessId;

        /// <summary>
        /// Gets and sets the collection of SubstitutionTokens that will be used by executing macros.
        /// </summary>
        public PMSubstitutionTokensCollection SubstitutionTokens
        {
            get { return _substitutionTokens; }
            set { _substitutionTokens = value; }
        }

        /// <summary>
        /// Gets the main window ID.
        /// </summary>
        public override int MainWindowId
        {
            get { return _powerMILL.ParentWindow; }
        }

        /// <summary>
        /// Determines whether powermill echo off commmand should be fired prior to an ExecuteEx command to
        /// avoid issue of retrieving information when the PM echo window is displayed.
        /// </summary>
        public bool IssueEchoOffCommands { get; set; }

        /// <summary>
        /// Gets the COM Project object
        /// </summary>
        /// <returns></returns>
        internal dynamic PowerMILLProject
        {
            get { return _powerMILL.Project; }
        }

        /// <summary>
        /// Gets whether PowerMill is busy doing something or not
        /// </summary>
        public bool IsBusy
        {
            get { return _powerMILL.Busy; }
        }

        #endregion

        #region Window Operations

        /// <summary>
        /// Gets or sets the property showing whether PowerMILL is visible or not.
        /// </summary>
        public override bool IsVisible
        {
            get { return _powerMILL.Visible; }
            set
            {
                // Have two goes.  For some reason in PM2015 the first one doesn't work!
                _powerMILL.Visible = value;
                _powerMILL.Visible = value;
            }
        }

        /// <summary>
        /// Shows/Hides the User Interface (toolbars etc).
        /// </summary>
        public override bool IsGUIVisible
        {
            get { return _isGUIVisible; }
            set
            {
                if (value)
                {
                    //Show GUI
                    if (Version.Major >= 2018)
                    {
                        DoCommand("GUI ON", "SPLITTER TABBROWSER WIDTH 240", "EXPLORER RAISE", "STATUS RAISE", "FORM RIBBON MAXIMISE");
                    }
                    else
                    {
                        DoCommand("GUI ON", "SPLITTER TABBROWSER WIDTH 240", "EXPLORER RAISE", "STATUS RAISE");
                    }
                }
                else
                {
                    //Hide GUI
                    if (Version.Major >= 2018)
                    {
                        DoCommand("GUI OFF", "SPLITTER TABBROWSER WIDTH 1", "EXPLORER LOWER", "STATUS LOWER", "FORM RIBBON MINIMISE");
                    }
                    else
                    {
                        DoCommand("GUI OFF", "SPLITTER TABBROWSER WIDTH 1", "EXPLORER LOWER", "STATUS LOWER");
                    }
                }
                _isGUIVisible = value;
            }
        }

        #endregion

        #region Project Operations

        /// <summary>
        /// Gets the Active Project in PowerMILL.
        /// </summary>
        public PMProject ActiveProject
        {
            get
            {
                if (_activeProject == null)
                {
                    //Populate the PowerMILLProject item with all the values currently in PowerMILL
                    PMProject powerMILLProject = null;
                    powerMILLProject = new PMProject(this);
                    _activeProject = powerMILLProject;
                }
                return _activeProject;
            }
        }

        /// <summary>
        /// Loads the project in the specified path into PowerMILL.
        /// </summary>
        /// <param name="projectDirectory">The project directory path for the PowerMILL project.</param>
        /// <param name="openReadOnly">If true it will open as read only.</param>
        /// <returns>The loaded project.</returns>
        public PMProject LoadProject(FileSystem.Directory projectDirectory, bool openReadOnly = false)
        {
            //Test that the file exists
            if (projectDirectory.Exists == false)
            {
                throw new Exception("The PowerMILL project does not exists");
            }
            _activeProject = new PMProject(this, projectDirectory, openReadOnly);
            return _activeProject;
        }

        /// <summary>
        /// Closes the current Project without saving changes.
        /// </summary>
        public void CloseProject()
        {
            DoCommand("PROJECT RESET NO", "DELETE ALL YES");
            _activeProject = null;
        }

        #endregion

        #region Unit Operations

        /// <summary>
        /// Gets and sets the units of the active project.
        /// </summary>
        public LengthUnits Units
        {
            get
            {
                //Read the current units from PowerMILL
                string strUnits = (string) DoCommandEx("PRINT PAR TERSE 'Units'");
                if (strUnits.ToLower() == "imperial")
                {
                    return LengthUnits.Inches;
                }
                return LengthUnits.MM;
            }
            set
            {
                switch (value)
                {
                    case LengthUnits.MM:
                        DoCommand("UNITS MM");
                        break;
                    case LengthUnits.Inches:
                        DoCommand("UNITS INCHES");
                        break;
                }
            }
        }

        #endregion

        #region View Operations

        /// <summary>
        /// Sets the view angle of the current project based on the Active Workplane.
        /// </summary>
        /// <param name="viewAngle">The view orientation to set in PowerMill.</param>
        public override void SetViewAngle(ViewAngles viewAngle)
        {
            switch (viewAngle)
            {
                case ViewAngles.ISO1:
                    DoCommand("ROTATE TRANSFORM ISO1 VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ISO2:
                    DoCommand("ROTATE TRANSFORM ISO2 VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ISO3:
                    DoCommand("ROTATE TRANSFORM ISO3 VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ISO4:
                    DoCommand("ROTATE TRANSFORM ISO4 VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ViewFromTop:
                    DoCommand("ROTATE TRANSFORM TOP VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ViewFromBottom:
                    DoCommand("ROTATE TRANSFORM BOTTOM VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ViewFromLeft:
                    DoCommand("ROTATE TRANSFORM LEFT VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ViewFromRight:
                    DoCommand("ROTATE TRANSFORM RIGHT VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ViewFromFront:
                    DoCommand("ROTATE TRANSFORM FRONT VIEWMILL RESIZEVIEW");
                    break;
                case ViewAngles.ViewFromBack:
                    DoCommand("ROTATE TRANSFORM BACK VIEWMILL RESIZEVIEW");
                    break;
            }
        }

        /// <summary>
        /// Sets the active plane in PowerMILL.
        /// </summary>
        /// <param name="desiredPlane">Plane to activate.</param>
        public void SetActivePlane(Planes desiredPlane)
        {
            switch (desiredPlane)
            {
                case Planes.XY:
                case Planes.YZ:
                case Planes.ZX:
                    DoCommand("STATUS EDITING_PLANE " + desiredPlane);
                    break;
            }
        }

        /// <summary>
        /// Undraw all Entities.
        /// </summary>        
        public void UndrawAll()
        {            
            DoCommand("UNDRAW ALL");            
        }
        #endregion

        #region Execute Operations

        /// <summary>
        /// Runs an array of commands in PowerMILL.  It is marked as obsolete as all commands should be
        /// available as functions in this Automation Interface.
        /// </summary>
        /// <param name="commands">The commands to run.</param>
        [Obsolete("This function should only be used when no alternative is available in Custom Software Core." +
                  "\r\n" +
                  "You should request the functionality you need to be added via the Custom Software Core forum at https://forums.autodesk.com/t5/powershape-and-powermill-api/bd-p/298"
        )]
        public void Execute(params string[] commands)
        {
            DoCommand(commands);
        }

        /// <summary>
        /// Runs the given command in PowerMILL.  It is marked as obsolete as all commands should be
        /// available as functions in this Automation Interface.
        /// </summary>
        /// <param name="command">The command to run in PowerMILL.</param>
        /// <returns>The value returned by PowerMill.</returns>
        [Obsolete("This function should only be used when no alternative is available in Custom Software Core. \r\n" +
                  "You should request the functionality you need to be added via the Custom Software Core forum at https://forums.autodesk.com/t5/powershape-and-powermill-api/bd-p/298"
        )]
        public object ExecuteEx(string command)
        {
            return DoCommandEx(command);
        }

        /// <summary>
        /// Runs an array of commands in PowerMILL.  It is a friend version equivalent to Execute
        /// but does not give the obsolete warning.
        /// </summary>
        internal void DoCommand(params string[] commands)
        {
            for (int i = 0; i <= commands.Length - 1; i++)
            {
                _powerMILL.DoCommand(commands[i]);
            }
        }

        /// <summary>
        /// Runs the given command in PowerMILL.  It is a friend version equivalent to ExecuteEx
        /// but does not give the obsolete warning.
        /// </summary>
        internal object DoCommandEx(string command)
        {
            if (IssueEchoOffCommands)
            {
                EchoCommandsOff();
            }
            object response = null;
            _powerMILL.DoCommandEx(command, ref response);

            if (response == null)
            {
                return string.Empty;
            }

            // If echo commands is enabled, response needs to be cleaned.
            double resultCheck;
            if (!double.TryParse(response.ToString(), out resultCheck))
            {
                var result = response.ToString().Trim();
                if (result.StartsWith("Process Command :"))
                {
                    var search = string.Format("Process Command : [{0}\\n]", command);
                    result = result.Replace(search, "");
                    result = result.Trim();
                }
                return result;
            }

            //If it is a string then trim the end off
            if (ReferenceEquals(response.GetType(), typeof(string)))
            {
                if (response.ToString().EndsWith("\r\n"))
                {
                    return response.ToString().Substring(0, response.ToString().Length - 2);
                }
                return response.ToString().Trim();
            }
            return response;
        }

        /// <summary>
        /// Turns command echoing on.
        /// </summary>
        /// <remarks></remarks>
        public void EchoCommandsOn()
        {
            _powerMILL.DoCommand("ECHO ON DCPDEBUG TRACE COMMAND ACCEPT");
        }

        /// <summary>
        /// Turns command echoing on.
        /// </summary>
        /// <remarks></remarks>
        public void EchoCommandsOff()
        {
            _powerMILL.DoCommand("ECHO OFF DCPDEBUG UNTRACE COMMAND ACCEPT");
        }

        #endregion

        #region Dispose Code

        /// <summary>
        /// Disposal code closes the PowerMILL instance that was created by the constructor.  It exits without
        /// saving changes.  It up to the developer to ensure that all changes are saved prior to exiting.
        /// </summary>
        // To detect redundant calls
        private bool disposedValue;

        /// <summary>
        /// Implements the IDisposable interface.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (InstanceReuse != InstanceReuse.UseExistingInstance)
                    {
                        //Exit without saving
                        Quit();
                    }
                }
            }
            disposedValue = true;
        }

        /// <summary>
        /// Implements the IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dialog Operations

        /// <summary>
        /// Turns off the warnings and error messages in PowerMILL.
        /// </summary>
        public void DialogsOff()
        {
            DoCommand("DIALOGS MESSAGE OFF", "DIALOGS ERROR OFF");
        }

        /// <summary>
        /// Turns on the warnings and error messages in PowerMILL.
        /// </summary>
        public void DialogsOn()
        {
            DoCommand("DIALOGS MESSAGE ON", "DIALOGS ERROR ON");
        }

        #endregion

        #region Collision Operations

        /// <summary>
        /// Turns off the collision checking in PowerMILL.
        /// </summary>
        public void CollisionsOff()
        {
            DoCommand("SIMULATE COLLISIONS OFF");
        }

        /// <summary>
        /// Turns on the collision checking in PowerMILL.
        /// </summary>
        public void CollisionsOn()
        {
            DoCommand("SIMULATE COLLISIONS STATIC");
        }

        /// <summary>
        /// Checking collision status
        /// </summary>
        /// <returns> returns true if the collision status = ON / returns false if the collision status = OFF</returns>
        public bool CheckCollisionStatus()
        {
            var result = DoCommandEx(@"PRINT PAR terse 'simulationstate.issues.collisions.checkcollisions'").ToString();
            if (!string.IsNullOrEmpty(result))
            {
                return result == "1";
            }
            throw new InvalidPowerMillParameterException(Version, "simulationstate.issues.collisions.checkcollisions");
        }

        #endregion

        #region Graphics Operations

        /// <summary>
        /// Turns graphics updates on.
        /// </summary>
        public void RefreshOn()
        {
            // Turns graphics updates on
            DoCommand("GRAPHICS UNLOCK");
        }

        /// <summary>
        /// Turns graphics updates off.
        /// </summary>
        public void RefreshOff()
        {
            // Turns graphics updates off
            DoCommand("GRAPHICS LOCK");
        }

        #endregion

        #region Macro Operations

        /// <summary>
        /// Runs the Macro at the given location.
        /// </summary>
        /// <param name="macroFile">The file path to the macro.</param>
        public void RunMacro(FileSystem.File macroFile)
        {
            PMMacro macro = null;
            macro = LoadMacro(macroFile);
            macro.Run();
        }

        /// <summary>
        /// Runs the Macro at the given location.
        /// </summary>
        /// <param name="macro">The mocro lines.</param>
        public void RunMacro(params string[] macro)
        {
            var pmMacro = LoadMacro(macro);
            pmMacro.Run();
        }

        /// <summary>
        /// Runs the specified Macro.
        /// </summary>
        /// <param name="macro">The macro to be run</param>
        public void RunMacro(PMMacro macro)
        {
            macro.Run();
        }

        /// <summary>
        /// Loads a macro from the specified file that can be stepped through or run with progress
        /// feedback.
        /// </summary>
        /// <param name="macroFile">The file path to the macro.</param>
        /// <returns>The new PMMacro.</returns>
        public PMMacro LoadMacro(FileSystem.File macroFile)
        {
            PMMacro macro = new PMMacro(this, macroFile);
            return macro;
        }

        /// <summary>
        /// Create a macro from a string.
        /// </summary>
        /// <param name="macro">macro list.</param>
        /// <returns>The new PMMacro.</returns>
        public PMMacro LoadMacro(params string[] macro)
        {
            return new PMMacro(this, macro);
        }

        #endregion

        #region General Operations

        /// <summary>
        /// Sets whether the block is visible or not.
        /// </summary>
        /// <value></value>
        public bool IsBlockVisible
        {
            set
            {
                if (value)
                {
                    DoCommand("DRAW BLOCK");
                }
                else
                {
                    DoCommand("UNDRAW BLOCK");
                }
            }
        }

        #endregion
    }
}