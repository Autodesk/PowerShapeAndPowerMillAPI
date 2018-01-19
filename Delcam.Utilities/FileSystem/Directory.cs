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
using System.IO;
using System.Linq;

namespace Autodesk.FileSystem
{
    /// <summary>
    /// Simplifies certain operations on file system directories.
    /// </summary>
    [Serializable]
    public class Directory
    {
        #region Constructors

        /// <summary>
        /// Creates a new Directory object.
        /// </summary>
        /// <param name="path">The directory path.</param>
        public Directory(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException();
            }

            string sep = System.IO.Path.DirectorySeparatorChar.ToString();
            if (path.EndsWith(sep))
            {
                Path = path;
            }
            else
            {
                Path = path + sep;
            }
        }

        /// <summary>
        /// Creates a new Directory object with a path relative to the specified parent.
        /// </summary>
        /// <param name="parentDirectory">Parent directory of the new directory.</param>
        /// <param name="directoryName">Directory to add to Parent directory.</param>
        public Directory(Directory parentDirectory, string directoryName)
        {
            Path = System.IO.Path.Combine(parentDirectory.Path, directoryName);

            string sep = System.IO.Path.DirectorySeparatorChar.ToString();
            if (!Path.EndsWith(sep))
            {
                Path += sep;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Directory path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Directory name.
        /// </summary>
        public string Name
        {
            get
            {
                // This function has to remove the training slash before getting the actual directory name
                return System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(Path));
            }
        }

        /// <summary>
        /// True if directory exists, false otherwise.
        /// </summary>
        public bool Exists
        {
            get { return System.IO.Directory.Exists(Path); }
        }

        /// <summary>
        /// True if directory is empty, false otherwise.
        /// </summary>
        public bool IsEmpty
        {
            get { return System.IO.Directory.GetFileSystemEntries(Path).Length == 0; }
        }

        /// <summary>
        /// List of directories contained within this directory.
        /// </summary>
        public List<Directory> Directories
        {
            get { return System.IO.Directory.GetDirectories(Path).Select(directory => new Directory(directory)).ToList(); }
        }

        /// <summary>
        /// List of files contained within this directory.
        /// </summary>
        public List<File> Files
        {
            get { return System.IO.Directory.GetFiles(Path).Select(file => new File(file)).ToList(); }
        }

        /// <summary>
        /// Parent directory of this directory object.
        /// </summary>
        public Directory ParentDirectory
        {
            get
            {
                Directory myParentDirectory = null;

                string tmpPath = System.IO.Path.GetDirectoryName(Path);

                if (!String.IsNullOrWhiteSpace(tmpPath))
                {
                    string parentDir = System.IO.Path.GetDirectoryName(tmpPath);

                    if (!String.IsNullOrWhiteSpace(parentDir))
                    {
                        myParentDirectory = new Directory(parentDir);
                    }
                }

                return myParentDirectory;
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Create the directory
        /// </summary>
        public void Create()
        {
            if (!Exists)
            {
                System.IO.Directory.CreateDirectory(Path);
            }
        }

        /// <summary>
        /// Delete this directory and all its contents from disk.
        /// </summary>
        /// <exception cref="IOException">Please refer to the System.IO.Directory Delete method.</exception>
        /// <exception cref="UnauthorizedAccessException">Please refer to the System.IO.Directory Delete method.</exception>
        /// <exception cref="DirectoryNotFoundException">Please refer to the System.IO.Directory Delete method.</exception>
        public void Delete()
        {
            if (Exists)
            {
                foreach (Directory directory in Directories)
                {
                    directory.Delete();
                }

                foreach (File file in Files)
                {
                    file.Delete();
                }

                System.IO.Directory.Delete(Path, true);
            }
        }

        /// <summary>
        /// Moves the directory and its contents to a new path.
        /// </summary>
        /// <param name="newPath">New directory path.</param>
        /// <exception cref="Exception">The new path already exists.</exception>
        public void Move(Directory newPath)
        {
            if (newPath.Exists)
            {
                throw new Exception(String.Format("Directory already exists '{0}'", newPath.Path));
            }

            if (!newPath.ParentDirectory.Exists)
            {
                newPath.ParentDirectory.Create();
            }

            System.IO.Directory.Move(Path, newPath.Path);

            Path = newPath.Path;
        }

        /// <summary>
        /// Move directory to the new specified parent
        /// </summary>
        /// <param name="newParentDirectory">New parent directory.</param>
        /// <exception cref="Exception">The new parent exists already.</exception>
        public void MoveToNewParent(Directory newParentDirectory)
        {
            Move(new Directory(newParentDirectory, Name));
        }

        /// <summary>
        /// Copies the directory and its contents to the specified path, overwriting all files with the same names.
        /// </summary>
        /// <param name="newPath">Destination path.</param>
        public void Copy(Directory newPath)
        {
            newPath.Create();
            CopyContents(this, newPath);
        }

        /// <summary>
        /// Copy directory and its contents to the specified new parent, overwriting all files with the same names.
        /// </summary>
        /// <param name="newParentDirectory">Destination directory.</param>
        public void CopyToNewParent(Directory newParentDirectory)
        {
            Copy(new Directory(newParentDirectory, Name));
        }

        /// <summary>
        /// Copy the contents of the source directory into the target directory, overwriting all files with the same names.
        /// </summary>
        /// <param name="source">Source directory.</param>
        /// <param name="target">Target directory.</param>
        private void CopyContents(Directory source, Directory target)
        {
            foreach (File file in source.Files)
            {
                file.Copy(new File(target, file.Name));
            }

            foreach (Directory directory in source.Directories)
            {
                Directory newDirectory = new Directory(target, directory.Name);
                newDirectory.Create();
                CopyContents(directory, newDirectory);
            }
        }

        /// <summary>
        /// Creates a new logical temporary Directory object within the temp folder of the logged in user and, optionally,
        /// creates the directory on disk. The directory is created with the name format TMP00000, thus providing
        /// a maximum upper limit of 99999 directories.
        /// </summary>
        /// <param name="createFolder">If True, the directory is created on disk.</param>
        /// <exception cref="Exception">Limit of temporary directory names exceeded.</exception>
        /// <returns>Directory</returns>
        public static Directory CreateTemporaryDirectory(bool createFolder = false)
        {
            Random rnd = new Random();
            Directory tempDirectory = new Directory(System.IO.Path.GetTempPath());
            Directory newDirectory = null;
            int attempts = 0;
            do
            {
                newDirectory = new Directory(tempDirectory, string.Format("TMP{0:00000}", rnd.Next(0, 99999)));
                if (!newDirectory.Exists)
                {
                    break;
                }

                attempts++;
            } while (attempts < 1000000);

            if (newDirectory.Exists)
            {
                throw new Exception("Ran out of temporary directory names.Try cleaning temp folder!");
            }

            if (createFolder)
            {
                newDirectory.Create();
            }

            return newDirectory;
        }

        #endregion
    }
}