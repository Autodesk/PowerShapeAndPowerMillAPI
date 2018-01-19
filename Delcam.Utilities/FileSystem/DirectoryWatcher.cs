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
    /// Monitors directories and raises events when files are created, modified or deleted.
    /// </summary>
    public class DirectoryWatcher : Watcher
    {
        #region events

        /// <summary>
        /// Use this as the prototype to handle the FileCreated event fired by this class.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Encapsulates arguments pertinent to this event.</param>
        public delegate void FileEventHandler(Object sender, FileSystemEventArgs e);

        /// <summary>
        /// Occurs when a file is created in the monitored directory.
        /// </summary>
        public event FileEventHandler FileCreated;

        /// <summary>
        /// Use this as the prototype to handle the FileDeleted event fired by this class.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Encapsulates arguments pertinent to this event.</param>
        public delegate void FileDeletedHandle(Object sender, FileSystemEventArgs e);

        /// <summary>
        /// Occurs when a file is deleted from the monitored directory.
        /// </summary>
        public event FileDeletedHandle FileDeleted;

        #endregion

        #region constructors

        /// <summary>
        /// Constructs a DirectoryWatcher object about the directory specified in the single input argument.
        /// </summary>
        /// <param name="directoryToWatch">Directory to monitor.</param>
        public DirectoryWatcher(Directory directoryToWatch) : this(directoryToWatch, "", false)
        {
        }

        /// <summary>
        /// Constructs a DirectoryWatcher object with options to monitor specific file types and sub-directories.
        /// </summary>
        /// <param name="directoryToWatch">Directory to monitor.</param>
        /// <param name="filter">File types to monitor e.g. *.TXT.</param>
        /// <param name="watchSubdirectories">If True, subdirectories are also monitored.</param>
        public DirectoryWatcher(Directory directoryToWatch, string filter, bool watchSubdirectories) :
            base(directoryToWatch)
        {
            // should be set by call to base.
            _watcher.Path = directoryToWatch.Path;

            if (filter.Trim() != "")
            {
                _watcher.Filter = filter;
            }

            _watcher.IncludeSubdirectories = watchSubdirectories;

            // set notify filters
            _watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
            _watcher.EnableRaisingEvents = true;

            //subscribe to events
            _watcher.Created += _watcher_Created;
            _watcher.Deleted += _watcher_Deleted;
        }

        #endregion

        #region properties

        /// <summary>
        /// If True the raising of events is enabled; suppresed otherwise.
        /// </summary>
        public bool EnableRaisingEvents
        {
            get { return _watcher.EnableRaisingEvents; }
            set { _watcher.EnableRaisingEvents = value; }
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Raises FileCreated event.
        /// </summary>
        /// <param name="sender">Object raising event i.e. this.</param>
        /// <param name="e">Arguments pertinent to this event.</param>
        protected void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                FileCreated(this, e);
            }
        }

        /// <summary>
        /// Raises FileDeleted event.
        /// </summary>
        /// <param name="sender">Object raising event i.e. this.</param>
        /// <param name="e">Arguments pertinent to this event.</param>
        protected void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (FileDeleted != null)
            {
                FileDeleted(this, e);
            }
        }

        #endregion
    }
}