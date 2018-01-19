// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Runtime.InteropServices;

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Facilitates the hiding of a named window.
    /// </summary>
    public class WindowHider
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        #region " Declares "

        private static extern int EnumWindows(CallBack enumFunc, int param);

        private delegate bool CallBack(int hwnd, int param);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern bool IsWindowVisible(int hwnd);

        [DllImport("user32", EntryPoint = "GetWindowTextA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GetWindowText(int hwnd, string windowTitle, int noOfCharactersToRead);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int ShowWindow(int hwnd, int showOrHide);

        #endregion

        #region " Fields "

        /// <summary>
        /// The title of the window to hide
        /// </summary>
        private string _windowTitle;

        #endregion

        #region " Shared Operations "

        /// <summary>
        /// Attempts to hide the window with the specified title.
        /// </summary>
        /// <param name="windowTitle">Title of window to hide.</param>
        public static void HideWindow(string windowTitle)
        {
            WindowHider myHider = new WindowHider(windowTitle);
            EnumWindows(myHider.CallBackFunction, 0);
        }

        #endregion

        #region " Constructors "

        /// <summary>
        /// Constructs a WindowHider object initialised with the title of the window to hide.
        /// </summary>
        /// <param name="windowTitle">Title of the window to hide.</param>
        private WindowHider(string windowTitle)
        {
            _windowTitle = windowTitle;
        }

        #endregion

        #region " Operations "

        /// <summary>
        /// This callback function is called by Windows (from the EnumWindows API call) for every window that exists
        /// </summary>
        /// <param name="hwnd">The handle of the window</param>
        /// <param name="param">The show/hide parameter.  0 to hide, 1 to show</param>
        /// <returns>True or false to indicate whether all was fine</returns>
        private bool CallBackFunction(int hwnd, int param)
        {
            int lReturn = 0;
            string thisWindowTitle = null;

            if (IsWindowVisible(hwnd))
            {
                thisWindowTitle = new string(' ', 256);
                lReturn = GetWindowText(hwnd, thisWindowTitle, thisWindowTitle.Length);

                // Remove any excess
                string trimmedWindowTitle = thisWindowTitle.Trim();
                if (_windowTitle.Length <= trimmedWindowTitle.Length)
                {
                    trimmedWindowTitle = trimmedWindowTitle.Substring(0, _windowTitle.Length);
                }

                // If it is the specified text then hide the window
                if (trimmedWindowTitle == _windowTitle)
                {
                    ShowWindow(hwnd, param);
                }
            }

            // Return that all went ok
            return true;
        }

        #endregion
    }
}