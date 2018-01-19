// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Text;

namespace Autodesk.FileSystem
{
    /// <summary>
    /// Provides a simplified interface for writing binary data to file.
    /// </summary>
    public class BinaryFileWriter
    {
        #region Fields

        private System.IO.BinaryWriter _binaryWriter;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a BinaryFileWriter object.
        /// </summary>
        /// <param name="file">File around which to create the writer.</param>
        public BinaryFileWriter(File file)
        {
            try
            {
                _binaryWriter = new System.IO.BinaryWriter(System.IO.File.OpenWrite(file.Path));
            }
            catch
            {
                Close();
            }
        }

        /// <summary>
        /// Constructs a BinaryFileWriter object.
        /// </summary>
        /// <param name="file">File around which to create the writer.</param>
        /// <param name="encoding">Character encoding to use when writing to file.</param>
        public BinaryFileWriter(File file, Encoding encoding)
        {
            try
            {
                _binaryWriter = new System.IO.BinaryWriter(System.IO.File.OpenWrite(file.Path), encoding);
            }
            catch
            {
                Close();
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Closes the file.
        /// </summary>
        public void Close()
        {
            if (_binaryWriter != null)
            {
                _binaryWriter.Close();
                _binaryWriter = null;
            }
        }

        /// <summary>
        /// Writes a single byte to file.
        /// </summary>
        /// <param name="value">The byte value to write.</param>
        public void WriteByte(byte value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes a byte array to file.
        /// </summary>
        /// <param name="value">Array to write.</param>
        public void WriteBytes(byte[] value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes string to file .
        /// </summary>
        /// <param name="value">String to be written.</param>
        public void WriteString(string value)
        {
            // Must use char array otherwise binarywriter.write(string) prefixes with no of characters.
            _binaryWriter.Write(value.ToCharArray());
        }

        /// <summary>
        /// Writes an unsigned 16 bit integer to file (same as WriteUShort).
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteUInt16(UInt16 value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 32 bit integer to file.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteUInteger(uint value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes a signed 16 bit integer to file.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteShort(short value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Writes a 16 bit unsigned integer to file (same as WriteUInt16).
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteUShort(ushort value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Write an unsigned 32 bit integer to file.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteUInt32(UInt32 value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Write a signed 32 bit integer to file.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteInteger(int value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Write a four byte (single precision) floating point number to file.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteSingle(Single value)
        {
            _binaryWriter.Write(value);
        }

        /// <summary>
        /// Write an eight byte (double precision) floating point number to file.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WriteDouble(double value)
        {
            _binaryWriter.Write(value);
        }

        #endregion
    }
}