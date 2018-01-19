// **********************************************************************
// *         © COPYRIGHT 2018 Autodesk, Inc.All Rights Reserved         *
// *                                                                    *
// *  Use of this software is subject to the terms of the Autodesk      *
// *  license agreement provided at the time of installation            *
// *  or download, or which otherwise accompanies this software         *
// *  in either electronic or hard copy form.                           *
// **********************************************************************

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Autodesk.WPFControls
{
    public class DelcamNavigation : Selector, INotifyPropertyChanged
    {
        #region "Events"

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        #endregion

        #region "Dependency Properties"

        public static readonly DependencyProperty SelectedContentProperty;

        public static readonly DependencyProperty NavigationPanelCoverVisiblityProperty;

        #endregion

        #region "Constructors"

        static DelcamNavigation()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamNavigation),
                                                     new FrameworkPropertyMetadata(typeof(DelcamNavigation)));
            SelectedContentProperty = DependencyProperty.Register("SelectedContent",
                                                                  typeof(object),
                                                                  typeof(DelcamNavigation),
                                                                  new PropertyMetadata(null, SelectedContent_Changed));
            NavigationPanelCoverVisiblityProperty = DependencyProperty.Register("NavigationPanelCoverVisiblity",
                                                                                typeof(Visibility),
                                                                                typeof(DelcamNavigation),
                                                                                new PropertyMetadata(Visibility.Hidden));
        }

        #endregion

        #region "Operations"

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            foreach (DelcamNavigationButton objHeader in Items)
            {
                if (objHeader.IsSelected)
                {
                    SelectedContent = objHeader.NavigationContent;
                }
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DelcamNavigationButton;
        }

        #endregion

        #region "Properties"

        #region "Selected Content"

        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            set { SetValue(SelectedContentProperty, value); }
        }

        private static void SelectedContent_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamNavigation objNav = (DelcamNavigation) d;
            objNav.SelectedContentChanged(e.OldValue, e.NewValue);
        }

        public virtual void SelectedContentChanged(object objOldValue, object objNewValue)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedContent"));
            }
        }

        #endregion

        public Visibility NavigationPanelCoverVisiblity
        {
            get { return (Visibility) GetValue(NavigationPanelCoverVisiblityProperty); }

            set { SetValue(NavigationPanelCoverVisiblityProperty, value); }
        }

        #endregion

        #region "Event Handlers"

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Refresh the content
            object content = GetValue(SelectedContentProperty);
            ClearValue(SelectedContentProperty);
            SetValue(SelectedContentProperty, content);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            foreach (DelcamNavigationButton objHeader in Items)
            {
                if (ReferenceEquals(objHeader, SelectedItem))
                {
                    if (objHeader.NavigationContent != null)
                    {
                        // Force a refresh of the content
                        ClearValue(SelectedContentProperty);
                        SetValue(SelectedContentProperty, objHeader.NavigationContent);
                        if (objHeader.NavigationContent != null && objHeader.NavigationContent is UIElement)
                        {
                            ((UIElement) objHeader.NavigationContent).InvalidateVisual();
                        }
                    }
                    objHeader.IsExpanded = true;
                }
                else
                {
                    objHeader.IsExpanded = false;
                }
            }
        }

        #endregion
    }
}