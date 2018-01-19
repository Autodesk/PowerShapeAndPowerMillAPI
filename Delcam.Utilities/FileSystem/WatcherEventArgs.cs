// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;

namespace Autodesk.FileSystem
{
    /// <summary>
    /// Encapsulates arguments pertinent to the Watcher class.
    /// </summary>
    public class WatcherEventArgs : EventArgs
    {
        /// <summary>
        /// The directory that was being watched
        /// </summary>
        private Directory _directory;

        /// <summary>
        /// </summary>
        /// <param name="watchedDirectory">Directory being watched</param>
        public WatcherEventArgs(Directory watchedDirectory)
        {
            _directory = watchedDirectory;
        }

        /// <summary>
        /// The directory being watched.
        /// </summary>
        public Directory Directory
        {
            get { return _directory; }
        }
    }
}