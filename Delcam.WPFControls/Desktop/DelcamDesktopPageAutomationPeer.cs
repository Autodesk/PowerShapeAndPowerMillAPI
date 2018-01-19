// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Runtime;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Autodesk.WPFControls
{
    public class DelcamDesktopPageAutomationPeer : SelectorItemAutomationPeer, ISelectionItemProvider
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public DelcamDesktopPageAutomationPeer(object owner, DelcamDesktopAutomationPeer desktopAutomationPeer)
            : base(owner, desktopAutomationPeer)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "DelcamDesktopPage";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            return nameCore;

            //If (Not String.IsNullOrEmpty(nameCore)) Then
            //    Dim tabItem As DelcamDesktopPage = MyBase.GetWrapper()
            //    If (tabItem IsNot Nothing AndAlso tabItem.Header.GetType() Is GetType(String)) Then
            //        'Return AccessText.RemoveAccessKeyMarker(nameCore)
            //        Return nameCore
            //    End If
            //    Return nameCore
            //End If
        }
    }
}