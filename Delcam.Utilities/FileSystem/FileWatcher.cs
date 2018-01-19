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
    /// Monitors a single file and raises events when it is modified.
    /// </summary>
    public class FileWatcher : Watcher
    {
        #region Events

        /// <summary>
        /// Occurs when the file is created.
        /// </summary>
        public event EventHandler<FileSystemEventArgs> FileCreated;

        /// <summary>
        /// Occurs when the file is deleted.
        /// </summary>
        public event EventHandler<FileSystemEventArgs> FileDeleted;

        /// <summary>
        /// Occurs when the file name is changed, the file is accessed or the file is written to.
        /// </summary>
        public event EventHandler<FileSystemEventArgs> FileChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a watcher that will monitor all changes to the specified file.
        /// </summary>
        /// <param name="fileIn">DelcamFile to watch</param>
        public FileWatcher(File fileIn) : this(fileIn.Path)
        {
        }

        /// <summary>
        /// Constructs a watcher for the specified file.
        /// </summary>
        /// <param name="fileName">File to monitor.</param>
        public FileWatcher(string fileName) : base(Path.GetDirectoryName(fileName))
        {
            _watcher.Filter = Path.GetFileName(fileName);
            _watcher.NotifyFilter = NotifyFilters.FileName |
                                    NotifyFilters.LastAccess |
                                    NotifyFilters.LastWrite;

            _watcher.Created += _watcher_Created;
            _watcher.Deleted += _watcher_Deleted;
            _watcher.Changed += _watcher_Changed;
            _watcher.EnableRaisingEvents = true;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Raises the FileChanged event.
        /// </summary>
        /// <param name="sender">Object raising event i.e. this.</param>
        /// <param name="e">Encapsulates arguments pertinent to this event.</param>
        protected void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                FileChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the FileDeleted event.
        /// </summary>
        /// <param name="sender">Object raising event i.e. this.</param>
        /// <param name="e">Encapsulates arguments pertinent to this event.</param>
        protected void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (FileDeleted != null)
            {
                FileDeleted(this, e);
            }
        }

        /// <summary>
        /// Raises the FileCreated event.
        /// </summary>
        /// <param name="sender">Object raising event i.e. this.</param>
        /// <param name="e">Encapsulates arguments pertinent to this event.</param>
        protected void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                FileCreated(this, e);
            }
        }

        #endregion
    }
}