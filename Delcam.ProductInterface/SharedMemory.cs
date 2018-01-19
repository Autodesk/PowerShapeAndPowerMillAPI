// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.Runtime.InteropServices;

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Provides a minimal .Net interface around unmanaged shared memory resources.
    /// </summary>
    /// <remarks></remarks>
    public class SharedMemory
    {
        [DllImport("kernel32",
            EntryPoint = "CreateFileMappingA",
            CharSet = CharSet.Ansi,
            SetLastError = true,
            ExactSpelling = true)]

        //APIs 
        private static extern int CreateFileMapping(
            int hFile,
            int lpFileMappigAttributes,
            int flProtect,
            int dwMaximumSizeHigh,
            int dwMaximumSizeLow,
            string lpName);

        [DllImport("kernel32", EntryPoint = "MapViewOfFile", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr MapViewOfFile(
            int hFileMappingObject,
            int dwDesiredAccess,
            int dwFileOffsetHigh,
            int dwFileOffsetLow,
            int dwNumberOfBytesToMap);

        [DllImport("kernel32", EntryPoint = "UnmapViewOfFile", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32", EntryPoint = "CloseHandle", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int CloseHandle(int hObject);

        //Constants 
        private const int FILE_MAP_ALL_ACCESS = 0xf001f;

        private const int PAGE_READWRITE = 0x4;

        private const int INVALID_HANDLE_VALUE = -1;

        //Variables 
        private int FileHandle;

        private IntPtr MapHandle;

        /// <summary>
        /// Maps a shared memory file into the address space of this application. If the file does not already exist, it is created.
        /// </summary>
        /// <param name="MemoryName">Name of the memory file.</param>
        /// <param name="Size">Size in bytes of the memory file.</param>
        /// <returns>True if the operation is successful; False otherwise.</returns>
        public bool Open(string MemoryName, int Size)
        {
            FileHandle = CreateFileMapping(INVALID_HANDLE_VALUE, 0, PAGE_READWRITE, 0, Size, MemoryName);

            //Get a pointer to the area of memory we just mapped. 
            if (!(FileHandle == 0))
            {
                MapHandle = MapViewOfFile(FileHandle, FILE_MAP_ALL_ACCESS, 0, 0, Size);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unmaps the shared memory file from the address space of this application.
        /// </summary>
        public void Close()
        {
            //Close the memory handle. 
            UnmapViewOfFile(MapHandle);
            CloseHandle(FileHandle);
        }

        /// <summary>
        /// Clears up unmanaged resources.
        /// </summary>
        /// <remarks></remarks>
        ~SharedMemory()
        {
            //Close the memory handle 
            Close();
        }

        /// <summary>
        /// Returns to the caller the first UTF8 encoded byte of data held in the shared memory buffer.
        /// </summary>
        public string Peek()
        {
            //Create an array to hold the data in memory. 
            byte[] buffer = new byte[1];

            //Copy the first byte of data in memory to the array. 
            Marshal.Copy(MapHandle, buffer, 0, 1);

            //Return Output (Unicode) 
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Writes data to the shared memory file.
        /// </summary>
        /// <param name="Data">Data to be written.</param>
        public void Put(string Data)
        {
            //TODO: The length of this string needs guarding against.
            //Create an array with one element for each character. (Unicode) 
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Data);
            Marshal.Copy(buffer, 0, MapHandle, buffer.Length);
        }
    }
}