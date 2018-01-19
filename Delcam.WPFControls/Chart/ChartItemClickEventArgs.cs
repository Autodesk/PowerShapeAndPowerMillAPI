// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

namespace Autodesk.WPFControls
{
    public class ChartItemClickEventArgs
    {
        private object objData;

        public ChartItemClickEventArgs(object objData)
        {
            this.objData = objData;
        }

        public object Data
        {
            get { return objData; }
            set { objData = value; }
        }
    }
}