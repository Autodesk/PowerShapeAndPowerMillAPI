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
using System.Windows.Media;

namespace Autodesk.WPFControls
{
    /// <summary>
    /// DelcamButton is the standard button that should be used.  It inherits from the DelcamLinkButton
    /// It has the following features:
    /// - Ability to glow on mouse over and fade on mouse out
    /// - Separate properties for Image and Content
    /// - Alternate content property to cycle through a collection of images and texts on each click.  This
    /// is similar to a toggle button but with multiple states
    /// - A status image intended to indicate Red/Amber/Green status
    /// - Horizontal/Vertical alignment of Image and Content items
    /// </summary>
    [TemplatePart(Name = "PART_StatusImage", Type = typeof(ContentPresenter))]
    public class DelcamButton : DelcamLinkButton
    {
        #region "Template Parts"

        /// <summary>
        /// Constant string to define the name of the StatusImage TemplatePart.
        /// The Status Image indicates a Red/Amber/Green Status
        /// </summary>
        public readonly string PART_StatusImage = "PART_StatusImage";

        #endregion

        #region "Constructors"

        /// <summary>
        /// Shared constructor initialises the default template
        /// </summary>
        static DelcamButton()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamButton), new FrameworkPropertyMetadata(typeof(DelcamButton)));
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// This operation binds the StatusImage's properties to properties of this class
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// This is the Source of the image used to indicate status
        /// </summary>
        private static readonly DependencyProperty StatusImageSourceProperty = DependencyProperty.Register("StatusImageSource",
                                                                                                           typeof(ImageSource),
                                                                                                           typeof(DelcamButton),
                                                                                                           new PropertyMetadata(
                                                                                                               null));

        /// <summary>
        /// This is the Source of the image used to indicate status
        /// </summary>
        /// <returns>Source of the image used to indicate status</returns>
        /// <value>Source of the image used to indicate status</value>
        public ImageSource StatusImageSource
        {
            get { return (ImageSource) GetValue(StatusImageSourceProperty); }

            set
            {
                SetValue(StatusImageSourceProperty, value);
                if (StatusImageHeight == 0)
                {
                    StatusImageHeight = 16;
                }
                if (StatusImageWidth == 0)
                {
                    StatusImageWidth = 16;
                }
            }
        }

        /// <summary>
        /// This is the height of the image used to indicate status
        /// </summary>
        private static readonly DependencyProperty StatusImageHeightProperty = DependencyProperty.Register("StatusImageHeight",
                                                                                                           typeof(int),
                                                                                                           typeof(DelcamButton),
                                                                                                           new PropertyMetadata(
                                                                                                               0));

        /// <summary>
        /// This is the height of the image used to indicate status
        /// </summary>
        /// <returns>Height of the image in pixels</returns>
        /// <value>Height of the image in pixels</value>
        public int StatusImageHeight
        {
            get { return (int) GetValue(StatusImageHeightProperty); }

            set { SetValue(StatusImageHeightProperty, value); }
        }

        /// <summary>
        /// This is the width of the image used to indicate status
        /// </summary>
        private static readonly DependencyProperty StatusImageWidthProperty = DependencyProperty.Register("StatusImageWidth",
                                                                                                          typeof(int),
                                                                                                          typeof(DelcamButton),
                                                                                                          new PropertyMetadata(0))
            ;

        /// <summary>
        /// This is the width of the image used to indicate status
        /// </summary>
        /// <returns>Width of the image in pixels</returns>
        /// <value>Width of the image in pixels</value>
        public int StatusImageWidth
        {
            get { return (int) GetValue(StatusImageWidthProperty); }

            set { SetValue(StatusImageWidthProperty, value); }
        }

        #endregion
    }
}