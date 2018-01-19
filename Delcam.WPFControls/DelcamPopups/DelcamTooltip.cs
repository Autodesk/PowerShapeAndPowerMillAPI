// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Windows;
using System.Windows.Controls;

namespace Autodesk.WPFControls
{
    public class DelcamTooltip : ToolTip
    {
        static DelcamTooltip()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamTooltip), new FrameworkPropertyMetadata(typeof(DelcamTooltip)));
        }
    }
}