// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Autodesk.WPFControls
{
    public class DelcamNavigationButton : Expander, INotifyPropertyChanged
    {
        #region "Events"

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        #endregion

        #region "Dependency Properties"

        public static readonly DependencyProperty NavigationContentProperty;
        public static readonly DependencyProperty IsSelectedProperty;
        public static readonly DependencyProperty ImageSourceProperty;
        public static readonly DependencyProperty ImageHeightProperty;

        public static readonly DependencyProperty ImageWidthProperty;

        #endregion

        #region "Constructors"

        static DelcamNavigationButton()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamNavigationButton),
                                                     new FrameworkPropertyMetadata(typeof(DelcamNavigationButton)));
            NavigationContentProperty = DependencyProperty.Register("NavigationContent",
                                                                    typeof(object),
                                                                    typeof(DelcamNavigationButton),
                                                                    new PropertyMetadata(null, NavigationContent_Changed));
            IsSelectedProperty = DependencyProperty.Register("IsSelected",
                                                             typeof(bool),
                                                             typeof(DelcamNavigationButton),
                                                             new PropertyMetadata(false, IsSelected_Changed));
            ImageSourceProperty = DependencyProperty.Register("ImageSource",
                                                              typeof(ImageSource),
                                                              typeof(DelcamNavigationButton),
                                                              new PropertyMetadata(null));
            ImageHeightProperty = DependencyProperty.Register("ImageHeight",
                                                              typeof(int),
                                                              typeof(DelcamNavigationButton),
                                                              new PropertyMetadata(16));
            ImageWidthProperty = DependencyProperty.Register("ImageWidth",
                                                             typeof(int),
                                                             typeof(DelcamNavigationButton),
                                                             new PropertyMetadata(16));
        }

        #endregion

        #region "Operations"

        #endregion

        #region "Properties"

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public int ImageHeight
        {
            get { return (int) GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public int ImageWidth
        {
            get { return (int) GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        #region "IsSelected"

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void IsSelected_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamNavigationButton objHeader = (DelcamNavigationButton) d;

            if (objHeader != null)
            {
                objHeader.OnIsSelectedChanged(Convert.ToBoolean(e.OldValue), Convert.ToBoolean(e.NewValue));
            }
        }

        public virtual void OnIsSelectedChanged(bool blnOldValue, bool blnNewValue)
        {
            if (IsSelected != IsExpanded)
            {
                IsExpanded = IsSelected;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        #endregion

        #region "Navigation Content"

        public object NavigationContent
        {
            get { return GetValue(NavigationContentProperty); }
            set { SetValue(NavigationContentProperty, value); }
        }

        private static void NavigationContent_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamNavigationButton objHeader = (DelcamNavigationButton) d;

            if (objHeader != null)
            {
                objHeader.OnNavigationContentChanged(e.OldValue, e.NewValue);
            }
        }

        public virtual void OnNavigationContentChanged(object objOldValue, object objNewValue)
        {
            if (IsSelected != IsExpanded)
            {
                IsExpanded = IsSelected;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }

            if (IsSelected)
            {
                DelcamNavigation objNav = (DelcamNavigation) Parent;

                if (objNav != null)
                {
                    objNav.SelectedContent = NavigationContent;
                }
            }
        }

        #endregion

        #endregion

        #region "Event Handlers"

        protected override void OnExpanded()
        {
            base.OnExpanded();

            DelcamNavigation objNav = (DelcamNavigation) Parent;

            if (objNav != null)
            {
                objNav.SelectedItem = this;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsExpanded == false && IsEnabled)
            {
                IsExpanded = true;
            }
        }

        #endregion
    }
}