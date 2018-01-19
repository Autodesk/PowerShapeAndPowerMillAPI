// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.IO;

namespace Autodesk.FileSystem
{
    /// <summary>
    /// Abstract class on which are built the Delcam directory and file watcher classes. Regularly checks the specified
    /// directory and raises a notification event should it no longer exist.
    /// </summary>
    public abstract class Watcher
    {
        #region fields

        /// <summary>
        /// System watcher object upon which this class leverages.
        /// </summary>
        protected FileSystemWatcher _watcher;

        private System.Timers.Timer _timer;
        private Directory _directory;

        #endregion

        #region events

        /// <summary>
        /// Use this as the prototype to handle the DirectoryUnavailable event fired by this class.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Encapsulates arguments pertinent to this event.</param>
        public delegate void DirectoryUnavailableHandler(
            Object sender,
            WatcherEventArgs e);

        /// <summary>
        /// Occurs when the directory monitored by this object has been deleted.
        /// </summary>
        public event DirectoryUnavailableHandler DirectoryUnavailable;

        #endregion

        #region properties

        /// <summary>
        /// True if timer running; False otherwise.
        /// </summary>
        public bool DirectoryUnavailableTimerStatus
        {
            get { return _timer.Enabled; }
            set
            {
                // _timer.Enabled = true;
                _timer.Start();
            }
        }

        #endregion

        /// <summary>
        /// Constructs a Watcher object about the directory specified in the single input argument.
        /// </summary>
        /// <param name="directoryToWatch">Directory to monitor.</param>
        public Watcher(Directory directoryToWatch)
        {
            _directory = directoryToWatch;

            //setup watch
            _watcher = new FileSystemWatcher(_directory.Path);

            //setup timer interval
            double interval = 5000;
            _timer = new System.Timers.Timer(interval);
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// Creates a new directory on disk and constructs a Watcher object about it.
        /// </summary>
        /// <param name="directoryPath">String containing path of new directory to create.</param>
        public Watcher(string directoryPath) : this(new Directory(directoryPath))
        {
        }

        #region event handlers

        /// <summary>
        /// Timer event handler. Checks to see if directory is still available in the file system and raises
        /// an event if it is not.
        /// </summary>
        /// <param name="sender">Object raising this event.</param>
        /// <param name="e">Raised event arguments.</param>
        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DirectoryUnavailable != null)
            {
                if (!_directory.Exists)
                {
                    WatcherEventArgs ea = new WatcherEventArgs(_directory);
                    DirectoryUnavailable(this, ea); // raise event to subscribers
                }
                else
                {
                    _timer.Start(); // restart timer
                }
            }
            else
            {
                _timer.Start();
            }
        }

        #endregion
    }
}