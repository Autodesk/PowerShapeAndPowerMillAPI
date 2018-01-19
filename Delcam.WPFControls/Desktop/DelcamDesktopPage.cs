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

namespace Autodesk.WPFControls
{
    public class DelcamDesktopPage : Expander, INotifyPropertyChanged
    {
        #region "Events"

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        #endregion

        #region "Dependency Properties"

        public static readonly DependencyProperty IsSelectedProperty;

        #endregion

        #region "Constructors"

        static DelcamDesktopPage()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamDesktopPage),
                                                     new FrameworkPropertyMetadata(typeof(DelcamDesktopPage)));
            IsSelectedProperty = DependencyProperty.Register("IsSelected",
                                                             typeof(bool),
                                                             typeof(DelcamDesktopPage),
                                                             new PropertyMetadata(false, IsSelected_Changed));
        }

        #endregion

        #region "Operations"

        #endregion

        #region "Properties"

        #region "IsSelected"

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void IsSelected_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamDesktopPage objDesktopPage = (DelcamDesktopPage) d;

            objDesktopPage.OnIsSelectedChanged(Convert.ToBoolean(e.OldValue), Convert.ToBoolean(e.NewValue));
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

        #region "Event Handlers"

        protected override void OnExpanded()
        {
            base.OnExpanded();

            DelcamDesktop objDesktop = (DelcamDesktop) Parent;

            if (objDesktop != null)
            {
                objDesktop.SelectedItem = this;
            }
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);

            if (IsSelected)
            {
                DelcamDesktop objDesktop = (DelcamDesktop) Parent;

                if (objDesktop != null)
                {
                    objDesktop.SelectedHeader = Header;
                }
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (IsSelected)
            {
                DelcamDesktop objDesktop = (DelcamDesktop) Parent;

                if (objDesktop != null)
                {
                    objDesktop.SelectedContent = Content;
                }
            }
        }

        #endregion

        #endregion
    }
}