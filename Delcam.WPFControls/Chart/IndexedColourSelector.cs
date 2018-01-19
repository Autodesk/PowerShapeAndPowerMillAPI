// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Autodesk.WPFControls
{
    public class IndexedColourSelector : DependencyObject, IColourSelector
    {
        public static readonly DependencyProperty ColoursProperty = DependencyProperty.Register("Colours",
                                                                                                typeof(Color[]),
                                                                                                typeof(IndexedColourSelector),
                                                                                                new UIPropertyMetadata(null));

        public Color[] Colours
        {
            get { return (Color[]) GetValue(ColoursProperty); }
            set { SetValue(ColoursProperty, value); }
        }

        public Color SelectColor(object objItem, int intIndex)
        {
            if (Colours == null || Colours.Count() == 0)
            {
                return Colors.Green;
            }

            return Colours[intIndex % Colours.Count()];
        }
    }
}