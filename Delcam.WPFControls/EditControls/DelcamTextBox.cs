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
    /// <summary>
    /// This class is the Delcam Text Box.
    /// It is like a normal TextBox but allows you to add a cue that will fade out as you type
    /// It can also be used as a search box with a magnifying glass
    /// </summary>
    public class DelcamTextBox : TextBox
    {
        #region "Constructors"

        /// <summary>
        /// Shared constructor initializes look and behavior
        /// </summary>
        static DelcamTextBox()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamTextBox), new FrameworkPropertyMetadata(typeof(DelcamTextBox)));

            TextProperty.OverrideMetadata(typeof(DelcamTextBox), new FrameworkPropertyMetadata(TextPropertyChanged));
        }

        #endregion

        #region "Dependency Properties"

        /// <summary>
        /// This is the cue dependency property.  Whatever you put in here will be shown when there is no text and will
        /// fade out as you type
        /// </summary>
        public static DependencyProperty TextBoxCueProperty = DependencyProperty.Register("TextBoxCue",
                                                                                          typeof(string),
                                                                                          typeof(DelcamTextBox),
                                                                                          new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets and sets the cue property.  Whatever you put in here will be shown when there is no text and will
        /// fade out as you type
        /// </summary>
        public string TextBoxCue
        {
            get { return (string) GetValue(TextBoxCueProperty); }

            set { SetValue(TextBoxCueProperty, value); }
        }

        /// <summary>
        /// This is the Key for the IsEmpty DependencyProperty
        /// </summary>
        public static readonly DependencyPropertyKey IsEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsEmpty",
                typeof(bool),
                typeof(DelcamTextBox),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// This is the IsEmpty DependencyProperty.  It identifies whether or not there is text in the TextBox
        /// </summary>
        public static readonly DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether or not there is text in the TextBox
        /// </summary>
        /// <returns>Whether or not there is text in the TextBox</returns>
        public bool IsEmpty
        {
            get { return (bool) GetValue(IsEmptyProperty); }
        }

        /// <summary>
        /// This dependency property specifies whether it is a search box. If it is then a magnifying glass will appear
        /// </summary>
        public static readonly DependencyProperty IsSearchBoxProperty = DependencyProperty.Register("IsSearchBox",
                                                                                                    typeof(bool),
                                                                                                    typeof(DelcamTextBox),
                                                                                                    new PropertyMetadata(false));

        /// <summary>
        /// Gets and sets whether or not this is a search box.  If it is then a magnifying glass will appear
        /// </summary>
        public bool IsSearchBox
        {
            get { return (bool) GetValue(IsSearchBoxProperty); }

            set { SetValue(IsSearchBoxProperty, value); }
        }

        #endregion

        #region "Event Handlers"

        /// <summary>
        /// This operation keeps the IsEmpty DependencyProperty up to date
        /// </summary>
        public static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DelcamTextBox target = (DelcamTextBox) sender;

            bool isEmpty = target.Text.Length == 0;
            if (isEmpty != target.IsEmpty)
            {
                target.SetValue(IsEmptyPropertyKey, isEmpty);
            }
        }

        #endregion
    }
}