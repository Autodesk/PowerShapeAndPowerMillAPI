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
    /// Provides a simplified interface for reading binary data from file.
    /// </summary>
    public class BinaryFileReader
    {
        #region Fields

        private System.IO.BinaryReader _binaryReader;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a BinaryFileReader object.
        /// </summary>
        /// <param name="file">File around which to create the binary reader.</param>
        public BinaryFileReader(File file)
        {
            try
            {
                _binaryReader = new System.IO.BinaryReader(System.IO.File.OpenRead(file.Path));
            }
            catch
            {
                Close();
            }
        }

        /// <summary>
        /// Constructs a BinaryFileReader object.
        /// </summary>
        /// <param name="bytes">Byte array around which to create the binary reader.</param>
        public BinaryFileReader(Byte[] bytes)
        {
            try
            {
                _binaryReader = new System.IO.BinaryReader(new System.IO.MemoryStream(bytes));
            }
            catch
            {
                Close();
            }
        }

        #endregion

        #region operations

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            if (_binaryReader != null)
            {
                _binaryReader.Close();
                _binaryReader = null;
            }
        }

        /// <summary>
        /// Reads a single byte from the binary stream and advances the file pointer to the next byte.
        /// </summary>
        /// <returns>Byte read.</returns>
        public Byte ReadByte()
        {
            return ReadBytes(1)[0];
        }

        /// <summary>
        /// Reads from the stream a byte array of the specified length and advances the file pointer to the next position after.
        /// If the end of file marker is reached then the number of bytes returned will be less than that requested.
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns></returns>
        public Byte[] ReadBytes(int count)
        {
            return _binaryReader.ReadBytes(count);
        }

        /// <summary>
        /// Reads a string from the stream of the specified length.
        /// </summary>
        /// <param name="length">String length in bytes (ASCII characters).</param>
        /// <returns></returns>
        public string ReadString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads a string of ASCII text from the stream until the specified terminating character is encountered
        /// or the maximum specified string length is reached. The terminating character is NOT included in
        /// the returned string.
        /// </summary>
        /// <param name="terminator">Byte containing character to break on.</param>
        /// <param name="maximumLength">Maximum permissible string length.</param>
        /// <returns>String of ASCII characters.</returns>
        public string ReadStringUntil(
            Byte terminator,
            int maximumLength)
        {
            return ReadStringUntil(terminator, maximumLength, false);
        }

        /// <summary>
        /// Reads a string of ASCII text from the stream until the specified terminating character is encountered
        /// or the maximum specified string length is reached. The terminating character can optionally be included
        /// in the return string.
        /// </summary>
        /// <param name="terminator">Byte containing character to break on.</param>
        /// <param name="maximumLength">Maximum permissible string length.</param>
        /// <param name="includeTerminator">Includes terminating character in return string if True. </param>
        /// <returns>String of ASCII characters.</returns>
        public string ReadStringUntil(
            Byte terminator,
            int maximumLength,
            bool includeTerminator)
        {
            string strText = "";
            bool blnFinish = false;
            StringBuilder sb = new StringBuilder();

            while (!blnFinish)
            {
                byte[] objByte = ReadBytes(1);

                if (objByte[0] == terminator)
                {
                    if (includeTerminator)
                    {
                        sb.Append(Encoding.ASCII.GetString(objByte));
                    }
                    blnFinish = true;
                }
                else
                {
                    sb.Append(Encoding.ASCII.GetString(objByte));
                    if (sb.Length >= maximumLength)
                    {
                        blnFinish = true;
                    }
                }
            }

            if (sb.Length > 0)
            {
                strText = sb.ToString();
            }

            return strText;
        }

        /// <summary>
        /// Reads an unsigned, 16 bit integer from the binary stream (Same as ReadUShort).
        /// </summary>
        /// <returns></returns>
        public UInt16 ReadUInt16()
        {
            return _binaryReader.ReadUInt16();
        }

        /// <summary>
        /// Reads an unsigned 32 bit integer from the binary stream.
        /// </summary>
        /// <returns></returns>
        public UInt32 ReadUInteger()
        {
            return _binaryReader.ReadUInt32();
        }

        /// <summary>
        /// Reads a 16 bit signed integer from the binary stream.
        /// </summary>
        /// <returns></returns>
        public Int16 ReadShort()
        {
            return _binaryReader.ReadInt16();
        }

        /// <summary>
        /// Reads an unsigned short integer from the binary stream (Same as ReadUInt16).
        /// </summary>
        /// <returns></returns>
        public ushort ReadUShort()
        {
            return _binaryReader.ReadUInt16();
        }

        /// <summary>
        /// Reads a 32 bit integer from the binary stream.
        /// </summary>
        /// <returns></returns>
        public int ReadInteger()
        {
            return _binaryReader.ReadInt32();
        }

        /// <summary>
        /// Reads a four byte (single precision) floating point value from the binary stream.
        /// </summary>
        /// <returns></returns>
        public float ReadSingle()
        {
            return _binaryReader.ReadSingle();
        }

        /// <summary>
        /// Reads an eight byte (double precision) floating point value from the binary stream.
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            return _binaryReader.ReadDouble();
        }

        #endregion
    }
}