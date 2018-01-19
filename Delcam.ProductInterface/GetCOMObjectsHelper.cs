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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Autodesk.ProductInterface
{
    /// <summary>
    /// Base class to get COM objects instances
    /// </summary>
    public class GetCOMObjectsHelper
    {
        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, ref IBindCtx ppbc);

        [DllImport("ole32.dll")]
        public static extern void GetRunningObjectTable(int reserved, ref IRunningObjectTable prot);

        /// <summary>
        /// Get all running instance by querying ROT
        /// </summary>
        /// <param name="classId">ClassId to search for. Example "PowerMILL.Application"</param>
        /// <returns>A list of COM Objects</returns>
        public static List<object> GetRunningInstances(string classId)
        {
            List<string> clsIds = new List<string>();

            // get the app clsid
            Type type1 = Type.GetTypeFromProgID(classId);
            clsIds.Add(type1.GUID.ToString().ToUpper());

            // get Running Object Table ...
            IRunningObjectTable Rot = null;
            GetRunningObjectTable(0, ref Rot);
            if (Rot == null)
            {
                return null;
            }

            // get enumerator for ROT entries
            IEnumMoniker monikerEnumerator = null;
            Rot.EnumRunning(out monikerEnumerator);

            if (monikerEnumerator == null)
            {
                return null;
            }

            monikerEnumerator.Reset();
            List<object> instances = new List<object>();

            IntPtr pNumFetched = new IntPtr();
            IMoniker[] monikers = new IMoniker[1];

            List<string> ComObjectList = new List<string>();

            // go through all entries and identifies app instances
            while (monikerEnumerator.Next(1, monikers, pNumFetched) == 0)
            {
                IBindCtx bindCtx = null;
                CreateBindCtx(0, ref bindCtx);
                if (bindCtx == null)
                {
                    continue;
                }

                string displayName = null;
                monikers[0].GetDisplayName(bindCtx, null, out displayName);
                ComObjectList.Add(displayName);

                foreach (string clsId in clsIds)
                {
                    if (displayName.Contains(classId))
                    {
                        object ComObject = null;
                        Rot.GetObject(monikers[0], out ComObject);

                        if (ComObject == null)
                        {
                            continue;
                        }

                        instances.Add(ComObject);
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            return instances;
        }
    }
}