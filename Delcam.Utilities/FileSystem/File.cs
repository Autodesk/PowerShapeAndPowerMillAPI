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
using System.Text;

namespace Autodesk.FileSystem
{
    /// <summary>
    /// Provides a thin wrapper around System.IO File to simplify certain operations.
    /// </summary>
    [Serializable]
    public class File
    {
        #region Construtors

        /// <summary>
        /// Creates a new File object for the specified path. Note: the file is not created on disk at this point.
        /// </summary>
        /// <param name="filePath">Fully qualified path to the destination file.</param>
        /// <exception cref="ArgumentNullException">Results from a null or empty filePath argument.</exception>
        /// <exception cref="ArgumentException">No containing folder: only a file name is specified in the filePath argument.</exception>
        public File(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException();
            }

            SetPath(filePath);
        }

        /// <summary>
        /// Creates a new File object with path specified by the parentDirectory argument and filename
        /// specified in the string argument. NOTE: the file is not created on disk at this point.
        /// </summary>
        /// <param name="parentDirectory">Parent directory of the file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="Exception">Results from a null or empty fileName argument.</exception>
        public File(Directory parentDirectory, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception(string.Format("'{0}' is not a valid filename.", fileName));
            }

            Path = System.IO.Path.Combine(parentDirectory.Path, fileName);
            ParentDirectory = parentDirectory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if the file exists.
        /// </summary>
        public bool Exists
        {
            get { return System.IO.File.Exists(Path); }
        }

        /// <summary>
        /// File's parent Directory object..
        /// </summary>
        public Directory ParentDirectory { get; private set; }

        /// <summary>
        /// Full path to the file.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Filename.
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        /// <summary>
        /// Filename without extension.
        /// </summary>
        public string NameWithoutExtension
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
        }

        /// <summary>
        /// File extension (without the '.').
        /// </summary>
        public string Extension
        {
            get
            {
                // System.IO.Path.GetExtension includes the period
                return System.IO.Path.GetExtension(Path).TrimStart('.');
            }
        }

        /// <summary>
        /// Date the file was last modifed.
        /// </summary>
        public DateTime LastChangedDate
        {
            get { return System.IO.File.GetLastWriteTime(Path); }
        }

        #endregion

        #region Operations

        private void SetPath(string filePath)
        {
            string directoryPath = System.IO.Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException(string.Format("Directory not found for filename '{0}'.", filePath), filePath);
            }

            ParentDirectory = new Directory(directoryPath);
            Path = filePath;
        }

        /// <summary>
        /// Create the file if it does not already exist.
        /// </summary>
        public void Create()
        {
            if (!Exists)
            {
                // This will recursively create parent directory if any of them not existed.
                ParentDirectory.Create();

                System.IO.File.Create(Path).Dispose();
            }
        }

        /// <summary>
        /// Delete the file.
        /// </summary>
        public void Delete()
        {
            System.IO.File.Delete(Path);
        }

        /// <summary>
        /// Copy the file to a new filepath. If a file with the same name and path already exists, it will be overwritten.
        /// </summary>
        /// <param name="destination">File object representing new path and filename.</param>
        public void Copy(File destination)
        {
            System.IO.File.Copy(Path, destination.Path, true);
        }

        /// <summary>
        /// Move File to new path
        /// </summary>
        /// <param name="newFile">File object representing new path.</param>
        /// <exception cref="IOException">Thrown if file already exists in destination path.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not possess the required permission.</exception>
        public void Move(File newFile)
        {
            newFile.Delete();

            System.IO.File.Move(Path, newFile.Path);

            SetPath(newFile.Path);
        }

        /// <summary>
        /// Copies the file to the specified new parent directory. If a file with the same name already exists, it will be overwritten.
        /// </summary>
        /// <param name="newParentDirectory">Directory to which the file is to be copied.</param>
        public void CopyToDirectory(Directory newParentDirectory)
        {
            var newFile = new File(newParentDirectory, Name);
            Copy(newFile);
        }

        /// <summary>
        /// Renames file within the current directory.
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <exception cref="ArgumentException">Thrown if input argument is not a valid filename.</exception>
        public void Rename(string newFileName)
        {
            // Ensure that value is not a filepath.
            if (System.IO.Path.GetFileName(Name) != Name)
            {
                throw new ArgumentException(string.Format("'{0}' is not a filename.", newFileName));
            }

            var renamedFile = new File(ParentDirectory, newFileName);
            renamedFile.Delete();

            System.IO.File.Move(Path, renamedFile.Path);
            SetPath(renamedFile.Path);
        }

        /// <summary>
        /// Moves file to a new directory.
        /// </summary>
        /// <param name="newDirectory">Target Directory.</param>
        /// <exception cref="IOException">Thrown if file already exists in destination path.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not possess the required permission.</exception>
        public void MoveToDirectory(Directory newDirectory)
        {
            var newFile = new File(newDirectory, Name);
            Move(newFile);
            ParentDirectory = newDirectory;
        }

        /// <summary>
        /// Returns Entire file contents using UTF8 encoding.
        /// </summary>
        /// <returns>String containing file contents.</returns>
        public string ReadText()
        {
            return ReadText(Encoding.UTF8);
        }

        /// <summary>
        /// Returns entire file contents using the encoding passed in the single input argument.
        /// </summary>
        /// <param name="encoding">System.Text.Encoding to use.</param>
        /// <returns>String containing file Contents.</returns>
        public string ReadText(Encoding encoding)
        {
            if (encoding == null)
            {
                return System.IO.File.ReadAllText(Path, new UTF8Encoding(false));
            }

            return System.IO.File.ReadAllText(Path, encoding);
        }

        /// <summary>
        /// Returns file contents as a list of strings using UTF8 encoding. Each string represents a seperate line of text in the file.
        /// </summary>
        /// <returns>List of strings.</returns>
        public List<string> ReadTextLines()
        {
            return ReadTextLines(new UTF8Encoding(false));
        }

        /// <summary>
        /// Returns the text of the first line of the file using UTF8 encoding.
        /// </summary>
        /// <returns>The first line of the file.</returns>
        public string ReadFirstLine()
        {
            string firstLine;
            using (var reader = new StreamReader(Path, new UTF8Encoding(false)))
            {
                firstLine = reader.ReadLine();
            }

            return firstLine;
        }

        /// <summary>
        /// Returns file contents as a list of strings using the encoding specified in the single input argument.
        /// Each string represents a seperate line of text in the file.
        /// </summary>
        /// <param name="encoding">System.Text.Encoding to use.</param>
        /// <returns>List of strings</returns>
        public List<string> ReadTextLines(Encoding encoding)
        {
            List<string> lines = new List<string>();

            string line;
            using (var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fileStream, encoding))
                {
                    while ((line = sr.ReadLine()) != null)
                        lines.Add(line);
                }
            }

            return lines;
        }

        /// <summary>
        /// Writes a string of text to file using UTF8 encoding.
        /// </summary>
        /// <param name="text">String containing text to be written.</param>
        public void WriteText(string text)
        {
            WriteText(text, false); // create new file and default encoding.
        }

        /// <summary>
        /// Writes a string of text to file using UTF8 encoding.
        /// </summary>
        /// <param name="text">String containing text to be written.</param>
        /// <param name="appendText">If True, text is appended to the file; otherwise the file will be overwritten.</param>
        public void WriteText(string text, bool appendText)
        {
            WriteText(text, appendText, new UTF8Encoding(false)); // use UTF encoding without the Byte Order Mark
        }

        /// <summary>
        /// Writes a string of text to file using the specified encoding.
        /// </summary>
        /// <param name="text">String containing text to be written.</param>
        /// <param name="appendText">If True, text is appended to the file; otherwise the file will be overwritten.</param>
        /// <param name="encoding">System.Text.Encoding to use.</param>
        public void WriteText(string text, bool appendText, Encoding encoding)
        {
            using (var stream = new StreamWriter(Path, appendText, encoding))
            {
                stream.Write(text);
            }
        }

        /// <summary>
        /// Writes a single line of text to file using the specified encoding.
        /// </summary>
        /// <param name="text">String containing text to be written.</param>
        /// <param name="appendText">If True, text is appended to the file; otherwise the file will be overwritten.</param>
        /// <param name="encoding">System.Text.Encoding to use.</param>
        public void WriteTextLine(string text, bool appendText, Encoding encoding)
        {
            using (var stream = new StreamWriter(Path, appendText, encoding))
            {
                stream.WriteLine(text);
            }
        }

        /// <summary>
        /// Writes to file all lines of text encapsulated within the passed array of strings.
        /// </summary>
        /// <param name="textLines">Lines of text to be written.</param>
        /// <param name="appendText">If True, text is appended to the file; otherwise the file will be overwritten.</param>
        /// <param name="encoding">System.Text.Encoding to use.</param>
        public void WriteTextLines(List<string> textLines, bool appendText = false, Encoding encoding = null)
        {
            StreamWriter ws;

            if (encoding == null)
            {
                ws = new StreamWriter(Path, appendText, new UTF8Encoding(false));
            }
            else
            {
                ws = new StreamWriter(Path, appendText, encoding);
            }

            foreach (string textLine in textLines)
            {
                ws.WriteLine(textLine);
            }

            ws.Flush();
            ws.Close();
        }

        /// <summary>
        /// Overrides ToString method. Returns full path to this file.
        /// </summary>
        /// <returns>Full path to file.</returns>
        public override string ToString()
        {
            return Path;
        }

        #endregion

        #region Static operations

        /// <summary>
        /// Creates a new File object representing a temporary file (i.e. in the user's temporary file folder) and with the specified extension.
        /// The file will, optionally, be created on disk.
        /// </summary>
        /// <param name="extension">File extension.</param>
        /// <param name="createFile">If True, a zero-byte file is created on disk.</param>
        /// <returns>DelcamFile</returns>
        public static File CreateTemporaryFile(string extension, bool createFile = false)
        {
            // Remove any period at the start
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            File tempFile;

            do
            {
                // Get a unique file name, without creating it
                var fileName = System.IO.Path.GetRandomFileName();

                tempFile = new File(System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName + "." + extension));

                // Keep looping until the file is unique
            } while (tempFile.Exists);

            if (createFile)
            {
                // Create a zero-byte file
                tempFile.Create();
            }

            return tempFile;
        }

        #endregion
    }
}