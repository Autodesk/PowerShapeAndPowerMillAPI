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
    /// DelcamLinkButton is a simple hyperlink style button.  It is also the base class for DelcamButton
    /// It has the following features:
    /// - Ability to glow on mouse over and fade on mouse out
    /// - Separate properties for Image and Content
    /// - Alternate content property to cycle through a collection of images and texts on each click.  This
    /// is similar to a toggle button but with multiple states
    /// - Horizontal/Vertical alignment of Image and Content items
    /// </summary>
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_Image", Type = typeof(Image))]
    public class DelcamLinkButton : Button
    {
        #region "Template Parts"

        /// <summary>
        /// Constant string to define the name of the Content TemplatePart.
        /// Not sure this is needed as the base class is a ContentControl
        /// </summary>
        private static readonly string PART_Content = "PART_Content";

        /// <summary>
        /// Constant string to define the name of the Image TemplatePart.
        /// </summary>
        private static readonly string PART_Image = "PART_Image";

        /// <summary>
        /// Holds whether the Alternative Content/Image are currently displayed or not
        /// </summary>
        private bool _isUsingAlternativeContent;

        /// <summary>
        /// This is the source of the default image
        /// </summary>
        private ImageSource _imageSource;

        /// <summary>
        /// This is the source of the disabled image
        /// </summary>
        private ImageSource _disabledImageSource;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Shared constructor initialises the default style
        /// </summary>
        static DelcamLinkButton()
        {
            //IsEnabledChanged += DelcamLinkButton_IsEnabledChanged;
            //Click += DelcamLinkButton_Click;

            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamLinkButton),
                                                     new FrameworkPropertyMetadata(typeof(DelcamLinkButton)));
        }

        #endregion

        #region "Operations"

        /// <summary>
        /// This operation binds properties of the template parts to properties of this class.
        /// This should be done in Generic.xml
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// This is the Source of the Image on the button
        /// </summary>
        /// <returns>The Source of the Image on the button</returns>
        /// <value>The Source of the Image on the button</value>
        public ImageSource ImageSource
        {
            get { return _imageSource; }

            set
            {
                _imageSource = value;
                if (ImageHeight == 0)
                {
                    ImageHeight = 16;
                }
                if (ImageWidth == 0)
                {
                    ImageWidth = 16;
                }
                SetValue(SelectedImageProperty, value);
            }
        }

        /// <summary>
        /// This is the orientation of the Image and Content
        /// </summary>
        private static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
                                                                                                     typeof(Orientation),
                                                                                                     typeof(DelcamLinkButton),
                                                                                                     new PropertyMetadata(
                                                                                                         Orientation.Horizontal));

        /// <summary>
        /// This is the orientation of the Image and Content.
        /// Horizontal will place the Image on the left and the Content on the right
        /// Vertical will place the Image on the top and the Content on the bottom
        /// </summary>
        /// <returns>The orientation of the Image and Content.  This can be Horizontal or Vertical</returns>
        /// <value>The orientation of the Image and Content.  This can be Horizontal or Vertical</value>
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }

            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// This is the height of the Image in pixels
        /// </summary>
        private static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight",
                                                                                                     typeof(int),
                                                                                                     typeof(DelcamLinkButton),
                                                                                                     new PropertyMetadata(0));

        /// <summary>
        /// This is the height of the Image in pixels
        /// </summary>
        /// <returns>The height in pixels</returns>
        /// <value>The height in pixels</value>
        public int ImageHeight
        {
            get { return (int) GetValue(ImageHeightProperty); }

            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        /// This is the width of the Image in pixels
        /// </summary>
        private static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth",
                                                                                                    typeof(int),
                                                                                                    typeof(DelcamLinkButton),
                                                                                                    new PropertyMetadata(0));

        /// <summary>
        /// This is the width of the Image in pixels
        /// </summary>
        /// <returns>The width in pixels</returns>
        /// <value>The width in pixels</value>
        public int ImageWidth
        {
            get { return (int) GetValue(ImageWidthProperty); }

            set { SetValue(ImageWidthProperty, value); }
        }

        /// <summary>
        /// This is the source of the Alternative Image on the button
        /// </summary>
        private static readonly DependencyProperty AlternativeImageSourceProperty =
            DependencyProperty.Register("AlternativeImageSource",
                                        typeof(ImageSource),
                                        typeof(DelcamLinkButton),
                                        new PropertyMetadata(null));

        /// <summary>
        /// This is the source of the Alternative Image on the button
        /// </summary>
        /// <returns>The Alternative Image on the button</returns>
        /// <value>The Alternative Image on the button</value>
        public ImageSource AlternativeImageSource
        {
            get { return (ImageSource) GetValue(AlternativeImageSourceProperty); }

            set { SetValue(AlternativeImageSourceProperty, value); }
        }

        /// <summary>
        /// This is the Alternative Content on the button
        /// </summary>
        private static readonly DependencyProperty AlternativeContentProperty = DependencyProperty.Register("AlternativeContent",
                                                                                                            typeof(object),
                                                                                                            typeof(
                                                                                                                DelcamLinkButton),
                                                                                                            new PropertyMetadata(
                                                                                                                null));

        /// <summary>
        /// This is the Alternative Content on the button
        /// </summary>
        /// <returns>The Alternative Content on the button</returns>
        /// <value>The Alternative Content on the button</value>
        public object AlternativeContent
        {
            get { return GetValue(AlternativeContentProperty); }

            set { SetValue(AlternativeContentProperty, value); }
        }

        private static readonly DependencyProperty SelectedImageProperty = DependencyProperty.Register("SelectedImage",
                                                                                                       typeof(ImageSource),
                                                                                                       typeof(DelcamLinkButton),
                                                                                                       new PropertyMetadata(null))
            ;

        /// <summary>
        /// This is the currently displayed Image from the ButtonItems.  If the list is empty then it is the value defined in the ImageSource property
        /// </summary>
        /// <returns>The currently displayed Image from the ButtonItems</returns>
        /// <value>The currently displayed Image from the ButtonItems</value>
        public ImageSource SelectedImage
        {
            get { return (ImageSource) GetValue(SelectedImageProperty); }

            set { SetValue(SelectedImageProperty, value); }
        }

        private static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent",
                                                                                                         typeof(object),
                                                                                                         typeof(DelcamLinkButton),
                                                                                                         new PropertyMetadata(
                                                                                                             null));

        /// <summary>
        /// This is the currently displayed Content from the ButtonItems.  If the list is empty then it is the value defined in the Content property
        /// </summary>
        /// <returns>The currently displayed Content from the ButtonItems</returns>
        /// <value>The currently displayed Content from the ButtonItems</value>
        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }

            set { SetValue(SelectedContentProperty, value); }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public ImageSource DisabledImageSource
        {
            get { return _disabledImageSource; }

            set { _disabledImageSource = value; }
        }

        #endregion

        #region "Event Handlers"

        /// <summary>
        /// This operation cycles to the next item in the ButtonItems list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelcamLinkButton_Click(object sender, RoutedEventArgs e)
        {
            // Switch to and from alternative image and content
            if ((AlternativeContent != null) | (AlternativeImageSource != null))
            {
                if (_isUsingAlternativeContent == false)
                {
                    // Switch to alternative content
                    SelectedImage = AlternativeImageSource;
                    SelectedContent = AlternativeContent;
                }
                else
                {
                    // Switch back to normal content
                    SelectedImage = ImageSource;
                    SelectedContent = Content;
                }
                _isUsingAlternativeContent = !_isUsingAlternativeContent;
            }
        }

        /// <summary>
        /// Notifies anyone watching that the SelectedContent has changed
        /// </summary>
        /// <param name="oldContent">The previous content of the button</param>
        /// <param name="newContent">The new content of the button</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            SetValue(SelectedContentProperty, newContent);
        }

        /// <summary>
        /// Changes the image when the object's enabled state changes if there is a disabled image supplied
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelcamLinkButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DisabledImageSource != null)
            {
                if (IsEnabled)
                {
                    SelectedImage = ImageSource;
                }
                else
                {
                    SelectedImage = DisabledImageSource;
                }
            }
        }

        #endregion
    }
}