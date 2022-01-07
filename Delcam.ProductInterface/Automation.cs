// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Base class for all Automation.
    /// </summary>
    public abstract class Automation
    {
        #region " Import "

        [DllImport("ole32.dll")]
        static extern int CLSIDFromProgID(
            [MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid pclsid);

        [DllImport("oleaut32.dll", PreserveSig = false)]
        static extern void GetActiveObject(ref Guid rclsid, IntPtr pvReserved,
                                           [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

        protected dynamic GetCOMObject(string progId)
        {
            Guid classId;
            CLSIDFromProgID(progId, out classId);
            object comObject = null;
            GetActiveObject(ref classId, IntPtr.Zero, out comObject);
            return comObject;
        }

        #endregion

        #region " Fields "

        /// <summary>
        /// Application process id.
        /// </summary>
        protected int _processId;

        /// <summary>
        /// Indicates whether a new instance of the application should be launched or an existing one be used.
        /// </summary>
        protected InstanceReuse InstanceReuse;

        /// <summary>
        /// If True, the GUI is shown; false otherwise.
        /// </summary>
        //Ideally we would be able to ask the products if the GUI is visible but it appears that this is not possible.
        protected bool _isGUIVisible;

        /// <summary>
        /// Specifies the application version to use.
        /// </summary>
        protected Version _version;

        #endregion

        #region " Constructors "

        /// <summary>
        /// Specifies whether an existing instance of the application be used or a new one created. This implementation is
        /// version non-specific and either the first existing instance of the application found or the latest installed version will be used.
        /// </summary>
        /// <param name="InstanceReuse">Specifies re-use or new instance.</param>
        protected void Initialise(InstanceReuse InstanceReuse)
        {
            this.InstanceReuse = InstanceReuse;

            // Either connect to an existing instance or create a new one
            switch (this.InstanceReuse)
            {
                case InstanceReuse.UseExistingInstance:

                    // Use an existing instance
                    UseExistingInstance(null, null);
                    break;
                case InstanceReuse.CreateNewInstance:

                    // Create a new instance
                    CreateNewInstance(null, null);
                    break;
                case InstanceReuse.CreateSingleInstance:

                    // Close all other instances and create a new one
                    CreateSingleInstance(null, null);
                    break;
            }

            _isGUIVisible = true;
        }

        /// <summary>
        /// Specifies whether an existing instance of the application be used or a new one created.
        /// The application version must be specified and, optionally, a maximum version. If a version
        /// greater than that specified in <i>version</i>, but less than or equal to that specified by <i>maximumVersion</i> is found,
        /// then this will be used. Should <i>maximumVersion</i> be unspecified, only the absolute version specified
        /// <i>in version</i> will be used.
        /// </summary>
        /// <param name="InstanceReuse">Specifies re-use or new instance.</param>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        protected void Initialise(InstanceReuse InstanceReuse, Version version, Version maximumVersion)
        {
            _version = version;

            this.InstanceReuse = InstanceReuse;

            // Either connect to an existing instance or create a new one
            switch (this.InstanceReuse)
            {
                case InstanceReuse.UseExistingInstance:

                    // Use an existing instance
                    UseExistingInstance(version, maximumVersion);
                    break;
                case InstanceReuse.CreateNewInstance:

                    // Create a new instance
                    CreateNewInstance(version, maximumVersion);
                    break;
                case InstanceReuse.CreateSingleInstance:

                    // Close all other instances and create a new one
                    CreateSingleInstance(version, maximumVersion);
                    break;
            }

            _isGUIVisible = true;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// Should attempt to connect to a running application instance of the correct version.  If it
        /// cannot, then it should start a new one.
        /// </summary>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        protected abstract void UseExistingInstance(Version version, Version maximumVersion);

        /// <summary>
        /// Should create a new application instance alongside existing instances.
        /// </summary>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        protected abstract void CreateNewInstance(Version version, Version maximumVersion);

        /// <summary>
        /// Should close all other instances and create a new one.
        /// </summary>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        protected abstract void CreateSingleInstance(Version version, Version maximumVersion);

        /// <summary>
        /// Should close this instance of the application.
        /// </summary>
        public abstract void Quit();

        /// <summary>
        /// Should implement macro recording.
        /// </summary>
        /// <param name="macroFilename">Name of file in which macro will be recorded.</param>
        public abstract void RecordMacro(string macroFilename);

        /// <summary>
        /// Should stop macro recording.
        /// </summary>
        public abstract void StopMacroRecording();

        #endregion

        #region " Process Operations "

        /// <summary>
        /// Process id of the application instance.
        /// </summary>
        public abstract int ProcessId { get; }

        /// <summary>
        /// Handle of the main window of this application instance.
        /// </summary>
        public virtual int MainWindowId
        {
            get { return (int) Process.GetProcessById(ProcessId).MainWindowHandle; }
        }

        /// <summary>
        /// True if the process is running; false otherwise.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                Process process = null;

                try
                {
                    Process.GetProcessById(_processId);
                }
                catch (Exception ex)
                {
                    // Cannot find process, must have ended
                    return false;
                }

                if (process == null || process.HasExited)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Closes all instances of a process with the specified name.
        /// </summary>
        /// <param name="name">Name of the process.</param>
        /// <exception cref="Exception">Thrown if it was not possible to close all instances with the specified name.</exception>
        protected void CloseAllInstancesOf(string name)
        {
            Console.WriteLine("Closing all instances...");

            var processesRunning = true;
            var startTime = DateTime.UtcNow;
            while (processesRunning && DateTime.UtcNow - startTime < TimeSpan.FromSeconds(10))
            {
                foreach (Process process in Process.GetProcessesByName(name))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                    }
                }

                processesRunning = Process.GetProcessesByName(name).Length > 0;
            }

            if (Process.GetProcessesByName(name).Length > 0)
            {
                throw new Exception("Failed to close all running instances of " + name);
            }

            Console.WriteLine("Done");
        }

        /// <summary>
        /// Launches the executable located at the path retrieved from the registry for the specified application name, version and maximum version.
        /// If a version greater than that specified in <i>version</i>, but less than or equal to that specified by <i>maximumVersion</i> is found,
        /// then this will be used. Should <i>maximumVersion</i> be unspecified, only the absolute version specified <i>in version</i> will be used.
        /// </summary>
        /// <param name="application">Name of the application to be launched.</param>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        /// <param name="classId">
        /// Represents the class of a COM object and takes the form <i>appname.objecttype</i>, where
        /// <i>appname</i> is the name of the application providing the object and <i>objecttype</i> is the type or class of object to create.
        /// </param>
        /// <param name="showWindow">Specify True if the main application window is to be displayed; false otherwise.</param>
        /// <param name="arguments">Arguments to be passed to the application.</param>
        /// <param name="runRegServer">If True, COM component will be registered. Assumes administrative rights are avaiable.</param>
        /// <exception cref="Exception">Thrown if the executable cannot be started.</exception>
        protected virtual void StartExecutable(
            string application,
            Version version,
            Version maximumVersion,
            string classId,
            bool showWindow,
            string arguments = "",
            bool runRegServer = false)
        {
            Console.WriteLine("Starting executable...");
            string executable = ExecutablePath(application, version, maximumVersion);

            // Creates a new instance of the Application and connects to it
            ProcessStartInfo processStart = new ProcessStartInfo();
            processStart.FileName = executable;
            if (runRegServer)
            {
                processStart.Verb = "runas";
                processStart.Arguments = "-regserver";
            }
            else
            {
                processStart.Arguments = arguments;
            }
            if (showWindow)
            {
                processStart.WindowStyle = ProcessWindowStyle.Normal;
            }
            else
            {
                processStart.WindowStyle = ProcessWindowStyle.Hidden;
            }
            processStart.CreateNoWindow = true;
            processStart.UseShellExecute = true;

            var process = Process.Start(processStart);

            if (runRegServer)
            {
                Console.WriteLine("Start called -regserver");

                // Just wait for the process to exit
                while (process.HasExited == false)
                {
                    Console.WriteLine("Waiting for regasm to finish - started at " + process.StartTime.ToLongTimeString());
                    if ((DateTime.Now - process.StartTime).TotalSeconds > 60)
                    {
                        Console.WriteLine("Given up");
                        process.Kill();
                    }
                    Thread.Sleep(1000);
                }
            }
            else
            {
                Console.WriteLine("Start called");

                // Give the process 60 seconds to start up
                DateTime startTime = DateTime.Now;
                Thread.Sleep(10000);
                object product = null;
                bool started = false;
                while ((DateTime.Now - startTime).Ticks < 60 * TimeSpan.TicksPerSecond)
                    try
                    {
                        // May need a better way of doing this as it wont work if another instance is running
                        // I tried WaitForIdleInput on the Process but PowerMILL didnt like this at all
                        Console.WriteLine("Waiting for product to start");
                        product = GetCOMObject(classId);
                        started = true;
                        break; // TODO: might not be correct. Was : Exit While
                    }
                    catch
                    {
                        // Wait 1 second until the next go
                        Thread.Sleep(1000);
                    }

                if (started == false)
                {
                    throw new Exception("Failed to start application " + application + Environment.NewLine + "Using executable " +
                                        executable);
                }

                _processId = process.Id;
            }
            Console.WriteLine("Executable started");
        }

        /// <summary>
        /// Creates and returns a reference to a COM object as specified by the ClassId argument.
        /// If a version greater than that specified in <i>version</i>, but less than or equal to that specified by <i>maximumVersion</i> is found,
        /// then this will be used. Should <i>maximumVersion</i> be unspecified, only the absolute version specified <i>in version</i> will be used.
        /// </summary>
        /// <param name="classId">Program Id of object to create e.g. PowerSHAPE.Application.</param>
        /// <param name="processName">Name of the executable e.g. powershape.</param>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        /// <exception cref="Exception">Thrown if the executable cannot be started.</exception>
        /// <returns>Reference to COM object.</returns>
        protected object StartByClassId(string classId, string processName, Version version, Version maximumVersion)
        {
            bool applicationStarted = false;
            dynamic application = null;

            while (applicationStarted == false)
            {
                DateTime startTime = DateTime.Now;

                // Create a new instance of the application
                Console.WriteLine("Creating by class id");
                application = Activator.CreateInstance(Type.GetTypeFromProgID(classId));
                application.Visible = true;

                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    // Ensure we are only interested in processes started since we called CreateObject
                    if (process.StartTime >= startTime)
                    {
                        applicationStarted = true;
                        _processId = process.Id;
                        Console.WriteLine("Found the process");
                        if (version != null)
                        {
                            if (maximumVersion == null)
                            {
                                // We are being picky about a version number so check that it is the correct version
                                var _with2 = version;
                                if (application.Version.ToString() !=
                                    string.Format("{0:#}{1:0}{2:00}", _with2.Major, _with2.Minor, _with2.Build))
                                {
                                    process.Kill();
                                    throw new Exception("Failed to start specified version by ClassId");
                                }
                            }
                            else
                            {
                                string sver = application.Version.ToString();
                                Version foundVersion = null;
                                if (sver.Length > 5)
                                {
                                    foundVersion = new Version(string.Format("{0}.{1}.{2}",
                                                                             sver.Substring(0, 4),
                                                                             sver.Substring(4, 1),
                                                                             sver.Substring(5, 2)));
                                }
                                else if (sver.Length > 4)
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
                                    process.Kill();
                                    throw new Exception("Failed to start specified version by ClassId");
                                }
                            }
                        }
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            Console.WriteLine("Done");
            return application;
        }

        /// <summary>
        /// Retrieves from the registry the path to the specified application.
        /// If a version greater than that specified in <i>version</i>, but less than or equal to that specified by <i>maximumVersion</i> is found,
        /// then the path to this executable will be returned.
        /// Should <i>maximumVersion</i> be unspecified, only the path to the absolute version specified <i>in version</i> will be returned.
        /// </summary>
        /// <param name="application">Application name.</param>
        /// <param name="version">Nominal application version to use.</param>
        /// <param name="maximumVersion">Maximum application version to use.</param>
        /// <exception cref="Exception">Thrown if no version found.</exception>
        /// <returns>Path to requested executable.</returns>
        protected string ExecutablePath(string application, Version version, Version maximumVersion)
        {
            string path = TryGetPathFromRegistry("Autodesk", application, version, maximumVersion);
            if (string.IsNullOrWhiteSpace(path))
            {
                path = TryGetPathFromRegistry("Delcam", application, version, maximumVersion);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                if (version == null)
                {
                    throw new Exception("Failed to get executable path for " + application);
                }
                var _with3 = version;
                throw new Exception(application + " version " +
                                    string.Format("{0:#}.{1:0}.{2:00}", _with3.Major, _with3.Minor, _with3.Build) +
                                    " cannot be found");
            }
            return path;
        }

        private string TryGetPathFromRegistry(string companyName, string application, Version version, Version maximumVersion)
        {
            try
            {
                RegistryKey companyKey = null;
                RegistryKey applicationKey = null;
                RegistryKey versionKey = null;

                // Get the Delcam Key
                companyKey = Registry.LocalMachine.OpenSubKey("Software\\" + companyName);

                // This key should contain the 64 bit versions on a 64 bit machine and the 32 bit versions on a
                // 32 bit machine
                // Firstly does it exist?
                if (companyKey != null)
                {
                    // Get the Applications Key
                    if (version != null)
                    {
                        if (maximumVersion == null)
                        {
                            string expectedVersionNumber = null;
                            if (version.Major >= 2018)
                            {
                                expectedVersionNumber = version.Major.ToString();
                            }
                            else
                            {
                                expectedVersionNumber =
                                    string.Format("{0:#}.{1:0}.{2:00}", version.Major, version.Minor, version.Build);
                            }

                            // Open the specific key - handle the case where some keys have extra data after the version number
                            foreach (string keyName in companyKey.OpenSubKey(application).GetSubKeyNames())
                            {
                                if (keyName.StartsWith(expectedVersionNumber))
                                {
                                    versionKey = companyKey.OpenSubKey(application).OpenSubKey(keyName);
                                }
                            }
                        }
                        else
                        {
                            foreach (string keyName in companyKey.OpenSubKey(application).GetSubKeyNames())
                            {
                                // PowerShape and PowerMill 2017 had an empty key for 2017 but no path key
                                if (keyName == "2017")
                                {
                                    continue;
                                }
                                string versionString = keyName;
                                if (versionString.IndexOf(".") < 0)
                                {
                                    versionString = versionString + "." + version.Minor + "." + version.Build;
                                }
                                while (versionString.Count(x => x == '.') < 2) versionString = versionString + ".0";
                                Version keyVersion = null;
                                if (Version.TryParse(versionString, out keyVersion))
                                {
                                    if ((keyVersion >= version) & (keyVersion <= maximumVersion))
                                    {
                                        versionKey = companyKey.OpenSubKey(application).OpenSubKey(keyName);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Open the latest version number.  Year integer numbers are later than version numbers
                        applicationKey = companyKey.OpenSubKey(application);
                        var latestMajor = 0;
                        for (int i = 0; i < applicationKey.SubKeyCount; i++)
                        {
                            string applicationKeyName = applicationKey.GetSubKeyNames()[i];
                            Version keyVersion = null;
                            if (Version.TryParse(applicationKeyName, out keyVersion))
                            {
                                if (keyVersion.Major > latestMajor)
                                {
                                    versionKey = applicationKey.OpenSubKey(applicationKeyName);
                                    latestMajor = keyVersion.Major;
                                }
                            }
                            // If the key is a year then test it as an integer.
                            int intVersion = 0;
                            if (int.TryParse(applicationKeyName, out intVersion))
                            {
                                // Ignore 2017 as this didn't contain the application path
                                if (intVersion != 2017 && intVersion > latestMajor)
                                {
                                    versionKey = applicationKey.OpenSubKey(applicationKeyName);
                                    latestMajor = intVersion;
                                }
                            }
                        }
                    }
                }

                if (versionKey == null)
                {
                    // Try the alternate version
                    if (Environment.Is64BitProcess)
                    {
                        // Get the 32 bit key
                        companyKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                                               .OpenSubKey("Software\\" + companyName);
                    }
                    else
                    {
                        // Get the 64 bit key
                        companyKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                                               .OpenSubKey("Software\\" + companyName);
                    }

                    if (companyKey != null)
                    {
                        // Get the Applications Key
                        if (version != null)
                        {
                            if (maximumVersion == null)
                            {
                                // Open the specific key - handle the case where some keys have extra data after the version number
                                foreach (string keyName in companyKey.OpenSubKey(application).GetSubKeyNames())
                                {
                                    if (keyName.StartsWith(
                                        string.Format("{0:#}.{1:0}.{2:00}", version.Major, version.Minor, version.Build)))
                                    {
                                        versionKey = companyKey.OpenSubKey(application).OpenSubKey(keyName);
                                    }
                                }
                            }
                            else
                            {
                                foreach (string keyName in companyKey.OpenSubKey(application).GetSubKeyNames())
                                {
                                    Version keyVersion = null;
                                    if (Version.TryParse(keyName, out keyVersion))
                                    {
                                        if ((keyVersion >= version) & (keyVersion <= maximumVersion))
                                        {
                                            versionKey = companyKey.OpenSubKey(application).OpenSubKey(keyName);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Open the latest version number.  Year integer numbers are later than version numbers
                            applicationKey = companyKey.OpenSubKey(application);
                            var latestMajor = 0;
                            for (int i = 0; i < applicationKey.SubKeyCount; i++)
                            {
                                string applicationKeyName = applicationKey.GetSubKeyNames()[i];
                                Version keyVersion = null;
                                if (Version.TryParse(applicationKeyName, out keyVersion))
                                {
                                    if (keyVersion.Major > latestMajor)
                                    {
                                        versionKey = applicationKey.OpenSubKey(applicationKeyName);
                                        latestMajor = keyVersion.Major;
                                    }
                                }
                                // If the key is a year then test it as an integer.
                                int intVersion = 0;
                                if (int.TryParse(applicationKeyName, out intVersion))
                                {
                                    // Ignore 2017 as this didn't contain the application path
                                    if (intVersion != 2017 && intVersion > latestMajor)
                                    {
                                        versionKey = applicationKey.OpenSubKey(applicationKeyName);
                                        latestMajor = intVersion;
                                    }
                                }
                            }
                        }
                    }
                }

                // Now return the executable's path
                return versionKey.GetValue("ExecutablePath").ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        #endregion

        #region " Window Operations "

        /// <summary>
        /// If True, the application is visible; False otherwise.
        /// </summary>
        public abstract bool IsVisible { get; set; }

        /// <summary>
        /// If True, the GUI (toolbars etc) is visible; False otherwise.
        /// </summary>
        public abstract bool IsGUIVisible { get; set; }

        #endregion

        #region " View Operations "

        /// <summary>
        /// Sets the view angle of the current project based on the Active Workplane.
        /// </summary>
        /// <param name="viewAngle">Angle of view.</param>
        public abstract void SetViewAngle(ViewAngles viewAngle);

        #endregion

        #region " Properties "

        /// <summary>
        /// Version of the current application.
        /// </summary>
        public virtual Version Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Name of the process.
        /// </summary>
        protected abstract string ExecutableName { get; }

        #endregion
    }
}