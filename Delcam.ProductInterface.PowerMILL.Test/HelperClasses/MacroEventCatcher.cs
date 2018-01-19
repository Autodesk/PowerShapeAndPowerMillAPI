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
using Autodesk.ProductInterface.PowerMILL;

namespace Autodesk.ProductInterface.PowerMILLTest.HelperClasses
{
    public class MacroEventCatcher
    {
        public class EventDataItem
        {
            public DateTime Time { get; set; }
            public double Percentage { get; set; }
        }

        public PMMacro Macro { get; set; }
        public List<EventDataItem> DataList = new List<EventDataItem>();

        public MacroEventCatcher(PMMacro macro)
        {
            Macro = macro;

            Macro.StepProcessed += Macro_StepProcessed;
        }

        void Macro_StepProcessed(object sender, MacroStepEventArgs e)
        {
            DataList.Add(new EventDataItem {Time = DateTime.Now, Percentage = e.Percentage});
        }

        public void Run()
        {
            Macro.Run(true, true);
        }
    }
}