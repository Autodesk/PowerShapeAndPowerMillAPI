// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.Extensions
{
    /// <summary>
    /// Contains Boolean extension method
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        /// This operation returns the string "YES" if the input boolean is True,
        /// "NO" otherwise.
        /// </summary>
        /// <param name="inputBoolean">Boolean value to convert.</param>
        /// <returns>"YES" or "NO"</returns>
        public static string ToYesNo(this bool inputBoolean)
        {
            return inputBoolean ? "YES" : "NO";
        }

        /// <summary>
        /// This operation returns the string "ON" if the input boolean is True,
        /// "OFF" otherwise.
        /// </summary>
        /// <param name="inputBoolean">Boolean value to convert.</param>
        /// <returns>"ON" or "OFF"</returns>
        public static string ToOnOff(this bool inputBoolean)
        {
            return inputBoolean ? "ON" : "OFF";
        }

        /// <summary>
        /// This operation returns the string "1" if the input boolean is True,
        /// "0" otherwise.
        /// </summary>
        /// <param name="inputBoolean">Boolean value to convert.</param>
        /// <returns>1 or 0</returns>
        public static int ToOneZero(this bool inputBoolean)
        {
            return inputBoolean ? 1 : 0;
        }
    }
}