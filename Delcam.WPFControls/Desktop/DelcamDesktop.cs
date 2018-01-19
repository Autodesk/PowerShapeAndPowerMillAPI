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
using System.Windows.Data;
using System.Windows.Input;

namespace Autodesk.WPFControls
{
    [TemplatePart(Name = "PART_HomeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SaveButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_HelpButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_AboutButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ExitButton", Type = typeof(Button))]
    public class DelcamDesktop : Selector, INotifyPropertyChanged
    {
        #region "Template Parts"

        private readonly string PART_HomeButton = "PART_HomeButton";
        private readonly string PART_SaveButton = "PART_SaveButton";
        private readonly string PART_BackButton = "PART_BackButton";
        private readonly string PART_HelpButton = "PART_HelpButton";
        private readonly string PART_AboutButton = "PART_AboutButton";

        private readonly string PART_ExitButton = "PART_ExitButton";

        #endregion

        #region "Events"

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

        #endregion

        #region "Dependency Properties"

        public static readonly DependencyProperty SelectedHeaderProperty;
        public static readonly DependencyProperty SelectedContentProperty;
        public static readonly DependencyProperty CoverallVisibilityProperty;
        public static readonly DependencyProperty CoverallProgressMessageProperty;

        public static readonly DependencyProperty CoverallProgressValueProperty;

        #endregion

        #region "Constructors"

        static DelcamDesktop()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DelcamDesktop), new FrameworkPropertyMetadata(typeof(DelcamDesktop)));
            SelectedHeaderProperty = DependencyProperty.Register("SelectedHeader",
                                                                 typeof(object),
                                                                 typeof(DelcamDesktop),
                                                                 new PropertyMetadata(null, SelectedHeader_Changed));
            SelectedContentProperty = DependencyProperty.Register("SelectedContent",
                                                                  typeof(object),
                                                                  typeof(DelcamDesktop),
                                                                  new PropertyMetadata(null, SelectedContent_Changed));
            CoverallVisibilityProperty = DependencyProperty.Register("CoverallVisibility",
                                                                     typeof(Visibility),
                                                                     typeof(DelcamDesktop),
                                                                     new PropertyMetadata(Visibility.Hidden));
            CoverallProgressMessageProperty = DependencyProperty.Register("CoverallProgressMessage",
                                                                          typeof(string),
                                                                          typeof(DelcamDesktop));
            CoverallProgressValueProperty = DependencyProperty.Register("CoverallProgressValue",
                                                                        typeof(double),
                                                                        typeof(DelcamDesktop));
        }

        #endregion

        #region "Operations"

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            foreach (DelcamDesktopPage objPage in Items)
            {
                if (objPage.IsSelected)
                {
                    SelectedHeader = objPage.Header;
                    SelectedContent = objPage.Content;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var objHomeButton = (Button) GetTemplateChild(PART_HomeButton);
            if (objHomeButton != null)
            {
                objHomeButton.Click += HomeButton_Click;
            }

            Button objSaveButton = (Button) GetTemplateChild(PART_SaveButton);
            if (objSaveButton != null)
            {
                objSaveButton.Click += SaveButton_Click;
            }

            Button objBackButton = (Button) GetTemplateChild(PART_BackButton);
            if (objBackButton != null)
            {
                objBackButton.Click += BackButton_Click;
            }

            Button objHelpButton = (Button) GetTemplateChild(PART_HelpButton);
            if (objHelpButton != null)
            {
                objHelpButton.Click += HelpButton_Click;
            }

            Button objAboutButton = (Button) GetTemplateChild(PART_AboutButton);
            if (objAboutButton != null)
            {
                objAboutButton.Click += AboutButton_Click;
            }

            Button objExitButton = (Button) GetTemplateChild(PART_ExitButton);
            if (objExitButton != null)
            {
                objExitButton.Click += ExitButton_Click;
            }

            if (objHomeButton != null)
            {
                // Bind the command for the home button to the exposed property
                Binding objHomeBinding = new Binding("HomeButtonCommand");
                objHomeBinding.Mode = BindingMode.OneWay;
                objHomeBinding.Source = this;
                objHomeButton.SetBinding(Button.CommandProperty, objHomeBinding);
            }

            if (objSaveButton != null)
            {
                // Bind the command for the back button to the exposed property
                Binding objSaveBinding = new Binding("SaveButtonCommand");
                objSaveBinding.Mode = BindingMode.OneWay;
                objSaveBinding.Source = this;
                objSaveButton.SetBinding(Button.CommandProperty, objSaveBinding);
            }

            if (objBackButton != null)
            {
                // Bind the command for the back button to the exposed property
                Binding objBackBinding = new Binding("BackButtonCommand");
                objBackBinding.Mode = BindingMode.OneWay;
                objBackBinding.Source = this;
                objBackButton.SetBinding(Button.CommandProperty, objBackBinding);
            }

            if (objAboutButton != null)
            {
                // Bind the command for the back button to the exposed property
                Binding objAboutBinding = new Binding("AboutButtonCommand");
                objAboutBinding.Mode = BindingMode.OneWay;
                objAboutBinding.Source = this;
                objAboutButton.SetBinding(Button.CommandProperty, objAboutBinding);
            }

            if (objHelpButton != null)
            {
                // Bind the command for the back button to the exposed property
                Binding objHelpBinding = new Binding("HelpButtonCommand");
                objHelpBinding.Mode = BindingMode.OneWay;
                objHelpBinding.Source = this;
                objHelpButton.SetBinding(Button.CommandProperty, objHelpBinding);
            }

            if (objExitButton != null)
            {
                // Bind the command for the back button to the exposed property
                Binding objExitBinding = new Binding("ExitButtonCommand");
                objExitBinding.Mode = BindingMode.OneWay;
                objExitBinding.Source = this;
                objExitButton.SetBinding(Button.CommandProperty, objExitBinding);
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DelcamDesktopPage;
        }

        #endregion

        #region "Properties"

        #region "Selected Header"

        public object SelectedHeader
        {
            get { return GetValue(SelectedHeaderProperty); }
            set { SetValue(SelectedHeaderProperty, value); }
        }

        private static void SelectedHeader_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamDesktop objDesktop = (DelcamDesktop) d;
            objDesktop.OnSelectedHeaderChanged(e.OldValue, e.NewValue);
        }

        public virtual void OnSelectedHeaderChanged(object objOldValue, object objNewValue)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedHeader"));
            }
        }

        #endregion

        #region "Selected Content"

        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            set { SetValue(SelectedContentProperty, value); }
        }

        private static void SelectedContent_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DelcamDesktop objDesktop = (DelcamDesktop) d;
            objDesktop.OnSelectedContentChanged(e.OldValue, e.NewValue);
        }

        public virtual void OnSelectedContentChanged(object objOldValue, object objNewValue)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedContent"));
            }
        }

        #endregion

        #region "Header Buttons"

        /// <summary>
        /// This is the visibility of the Delcam DPS Logo
        /// </summary>
        private static readonly DependencyProperty AutodeskLogoVisibilityProperty =
            DependencyProperty.Register("AutodeskLogoVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// This is the visibility of the Autodesk Logo
        /// </summary>
        /// <returns>Visibility of the Autodesk Logo</returns>
        /// <value>Visibility of the Autodesk Logo</value>
        public Visibility AutodeskLogoVisibility
        {
            get { return (Visibility) GetValue(AutodeskLogoVisibilityProperty); }

            set { SetValue(AutodeskLogoVisibilityProperty, value); }
        }

        #region "Home button"

        /// <summary>
        /// This is the visibility of the Home Button
        /// </summary>
        private static readonly DependencyProperty HomeButtonVisibilityProperty =
            DependencyProperty.Register("HomeButtonVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// This is the visibility of the Home Button
        /// </summary>
        /// <returns>Visibility of the Home Button</returns>
        /// <value>Visibility of the Home Button</value>
        public Visibility HomeButtonVisibility
        {
            get { return (Visibility) GetValue(HomeButtonVisibilityProperty); }

            set { SetValue(HomeButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets and sets whether the Home Button is Enabled or not
        /// </summary>
        private static readonly DependencyProperty IsHomeButtonEnabledProperty = DependencyProperty.Register(
            "IsHomeButtonEnabled",
            typeof(bool),
            typeof(DelcamDesktop),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether the Home Button is Enabled or not
        /// </summary>
        /// <returns>Whether the Home Button is Enabled or not</returns>
        /// <value>Whether the Home Button is Enabled or not</value>
        public bool IsHomeButtonEnabled
        {
            get { return (bool) GetValue(IsHomeButtonEnabledProperty); }

            set { SetValue(IsHomeButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets and sets the command used when the home button is clicked
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty HomeButtonCommandProperty = DependencyProperty.Register("HomeButtonCommand",
                                                                                                           typeof(ICommand),
                                                                                                           typeof(DelcamDesktop));

        public ICommand HomeButtonCommand
        {
            get { return (ICommand) GetValue(HomeButtonCommandProperty); }
            set { SetValue(HomeButtonCommandProperty, value); }
        }

        #endregion

        #region "Save button"

        /// <summary>
        /// This is the visibility of the Save Button
        /// </summary>
        private static readonly DependencyProperty SaveButtonVisibilityProperty =
            DependencyProperty.Register("SaveButtonVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// This is the visibility of the Save Button
        /// </summary>
        /// <returns>Visibility of the Save Button</returns>
        /// <value>Visibility of the Save Button</value>
        public Visibility SaveButtonVisibility
        {
            get { return (Visibility) GetValue(SaveButtonVisibilityProperty); }

            set { SetValue(SaveButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets and sets whether the Save Button is Enabled or not
        /// </summary>
        private static readonly DependencyProperty IsSaveButtonEnabledProperty = DependencyProperty.Register(
            "IsSaveButtonEnabled",
            typeof(bool),
            typeof(DelcamDesktop),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether the Save Button is Enabled or not
        /// </summary>
        /// <returns>Whether the Save Button is Enabled or not</returns>
        /// <value>Whether the Save Button is Enabled or not</value>
        public bool IsSaveButtonEnabled
        {
            get { return (bool) GetValue(IsSaveButtonEnabledProperty); }

            set { SetValue(IsSaveButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets and sets the command used when the exit button is clicked
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty SaveButtonCommandProperty = DependencyProperty.Register("SaveButtonCommand",
                                                                                                           typeof(ICommand),
                                                                                                           typeof(DelcamDesktop));

        public ICommand SaveButtonCommand
        {
            get { return (ICommand) GetValue(SaveButtonCommandProperty); }
            set { SetValue(SaveButtonCommandProperty, value); }
        }

        #endregion

        #region "Back button"

        /// <summary>
        /// This is the visibility of the Back Button
        /// </summary>
        private static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// This is the visibility of the Back Button
        /// </summary>
        /// <returns>Visibility of the Back Button</returns>
        /// <value>Visibility of the Back Button</value>
        public Visibility BackButtonVisibility
        {
            get { return (Visibility) GetValue(BackButtonVisibilityProperty); }

            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets and sets whether the Home Button is Enabled or not
        /// </summary>
        private static readonly DependencyProperty IsBackButtonEnabledProperty = DependencyProperty.Register(
            "IsBackButtonEnabled",
            typeof(bool),
            typeof(DelcamDesktop),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether the Back Button is Enabled or not
        /// </summary>
        /// <returns>Whether the Back Button is Enabled or not</returns>
        /// <value>Whether the Back Button is Enabled or not</value>
        public bool IsBackButtonEnabled
        {
            get { return (bool) GetValue(IsBackButtonEnabledProperty); }

            set { SetValue(IsBackButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets and sets the command used when the Back button is clicked
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty BackButtonCommandProperty = DependencyProperty.Register("BackButtonCommand",
                                                                                                           typeof(ICommand),
                                                                                                           typeof(DelcamDesktop));

        public ICommand BackButtonCommand
        {
            get { return (ICommand) GetValue(BackButtonCommandProperty); }
            set { SetValue(BackButtonCommandProperty, value); }
        }

        #endregion

        #region "Help button"

        /// <summary>
        /// This is the visibility of the Help Button
        /// </summary>
        private static readonly DependencyProperty HelpButtonVisibilityProperty =
            DependencyProperty.Register("HelpButtonVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// This is the visibility of the Help Button
        /// </summary>
        /// <returns>Visibility of the Help Button</returns>
        /// <value>Visibility of the Help Button</value>
        public Visibility HelpButtonVisibility
        {
            get { return (Visibility) GetValue(HelpButtonVisibilityProperty); }

            set { SetValue(HelpButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets and sets whether the Help Button is Enabled or not
        /// </summary>
        private static readonly DependencyProperty IsHelpButtonEnabledProperty = DependencyProperty.Register(
            "IsHelpButtonEnabled",
            typeof(bool),
            typeof(DelcamDesktop),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether the Help Button is Enabled or not
        /// </summary>
        /// <returns>Whether the Help Button is Enabled or not</returns>
        /// <value>Whether the Help Button is Enabled or not</value>
        public bool IsHelpButtonEnabled
        {
            get { return (bool) GetValue(IsHelpButtonEnabledProperty); }

            set { SetValue(IsHelpButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets and sets the command used when the help button is clicked
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty HelpButtonCommandProperty = DependencyProperty.Register("HelpButtonCommand",
                                                                                                           typeof(ICommand),
                                                                                                           typeof(DelcamDesktop));

        public ICommand HelpButtonCommand
        {
            get { return (ICommand) GetValue(HelpButtonCommandProperty); }
            set { SetValue(HelpButtonCommandProperty, value); }
        }

        #endregion

        #region "About button"

        /// <summary>
        /// This is the visibility of the About Button
        /// </summary>
        private static readonly DependencyProperty AboutButtonVisibilityProperty =
            DependencyProperty.Register("AboutButtonVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// This is the visibility of the About Button
        /// </summary>
        /// <returns>Visibility of the About Button</returns>
        /// <value>Visibility of the About Button</value>
        public Visibility AboutButtonVisibility
        {
            get { return (Visibility) GetValue(AboutButtonVisibilityProperty); }

            set { SetValue(AboutButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets and sets whether the About Button is Enabled or not
        /// </summary>
        private static readonly DependencyProperty IsAboutButtonEnabledProperty =
            DependencyProperty.Register("IsAboutButtonEnabled", typeof(bool), typeof(DelcamDesktop), new PropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether the About Button is Enabled or not
        /// </summary>
        /// <returns>Whether the About Button is Enabled or not</returns>
        /// <value>Whether the About Button is Enabled or not</value>
        public bool IsAboutButtonEnabled
        {
            get { return (bool) GetValue(IsAboutButtonEnabledProperty); }

            set { SetValue(IsAboutButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets and sets the command used when the about button is clicked
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty AboutButtonCommandProperty = DependencyProperty.Register("AboutButtonCommand",
                                                                                                            typeof(ICommand),
                                                                                                            typeof(DelcamDesktop))
            ;

        public ICommand AboutButtonCommand
        {
            get { return (ICommand) GetValue(AboutButtonCommandProperty); }
            set { SetValue(AboutButtonCommandProperty, value); }
        }

        #endregion

        #region "Exit button"

        /// <summary>
        /// This is the visibility of the Exit Button
        /// </summary>
        private static readonly DependencyProperty ExitButtonVisibilityProperty =
            DependencyProperty.Register("ExitButtonVisibility",
                                        typeof(Visibility),
                                        typeof(DelcamDesktop),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// This is the visibility of the Exit Button
        /// </summary>
        /// <returns>Visibility of the Exit Button</returns>
        /// <value>Visibility of the Exit Button</value>
        public Visibility ExitButtonVisibility
        {
            get { return (Visibility) GetValue(ExitButtonVisibilityProperty); }

            set { SetValue(ExitButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets and sets whether the Exit Button is Enabled or not
        /// </summary>
        private static readonly DependencyProperty IsExitButtonEnabledProperty = DependencyProperty.Register(
            "IsExitButtonEnabled",
            typeof(bool),
            typeof(DelcamDesktop),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether the Exit Button is Enabled or not
        /// </summary>
        /// <returns>Whether the Exit Button is Enabled or not</returns>
        /// <value>Whether the Exit Button is Enabled or not</value>
        public bool IsExitButtonEnabled
        {
            get { return (bool) GetValue(IsExitButtonEnabledProperty); }

            set { SetValue(IsExitButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets and sets the command used when the exit button is clicked
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty ExitButtonCommandProperty = DependencyProperty.Register("ExitButtonCommand",
                                                                                                           typeof(ICommand),
                                                                                                           typeof(DelcamDesktop));

        public ICommand ExitButtonCommand
        {
            get { return (ICommand) GetValue(ExitButtonCommandProperty); }
            set { SetValue(ExitButtonCommandProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets and sets the full screen mode
        /// </summary>
        /// <remarks></remarks>
        private static readonly DependencyProperty IsFullScreenModeProperty = DependencyProperty.Register("IsFullScreenMode",
                                                                                                          typeof(bool),
                                                                                                          typeof(DelcamDesktop));

        public bool IsFullScreenMode
        {
            get { return (bool) GetValue(IsFullScreenModeProperty); }
            set { SetValue(IsFullScreenModeProperty, value); }
        }

        #endregion

        #region "Coverall"

        public Visibility CoverallVisibility
        {
            get { return (Visibility) GetValue(CoverallVisibilityProperty); }

            set { SetValue(CoverallVisibilityProperty, value); }
        }

        public string CoverallProgressMessage
        {
            get { return (string) GetValue(CoverallProgressMessageProperty); }

            set { SetValue(CoverallProgressMessageProperty, value); }
        }

        public double CoverallProgressValue
        {
            get { return (double) GetValue(CoverallProgressValueProperty); }

            set { SetValue(CoverallProgressValueProperty, value); }
        }

        #endregion

        #endregion

        #region "Event Handlers"

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            foreach (DelcamDesktopPage objPage in Items)
            {
                if (ReferenceEquals(objPage, SelectedItem))
                {
                    // Force a refresh of the header
                    ClearValue(SelectedHeaderProperty);
                    SetValue(SelectedHeaderProperty, objPage.Header);
                    if (objPage.Header != null && ReferenceEquals(objPage.Header.GetType(), typeof(UIElement)))
                    {
                        ((UIElement) objPage.Header).InvalidateVisual();
                    }

                    // Force a refresh of the content
                    ClearValue(SelectedContentProperty);
                    SetValue(SelectedContentProperty, objPage.Content);
                    if (objPage.Content != null && objPage.Content is UIElement)
                    {
                        ((UIElement) objPage.Content).InvalidateVisual();
                    }
                    objPage.IsSelected = true;
                }
                else
                {
                    objPage.IsExpanded = false;
                    objPage.IsSelected = false;
                }
            }
        }

        public event HomeButtonClickedEventHandler HomeButtonClicked;

        public delegate bool HomeButtonClickedEventHandler();

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var blnCancel = false;

            if (HomeButtonClicked != null)
            {
                blnCancel = HomeButtonClicked();
            }

            if (blnCancel == false)
            {
                SelectedIndex = 0;
            }
        }

        public event SaveButtonClickedEventHandler SaveButtonClicked;

        public delegate void SaveButtonClickedEventHandler();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise the click event
            if (SaveButtonClicked != null)
            {
                SaveButtonClicked();
            }
        }

        public event BackButtonClickedEventHandler BackButtonClicked;

        public delegate void BackButtonClickedEventHandler();

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (BackButtonClicked != null)
            {
                BackButtonClicked();
            }
        }

        public event HelpButtonClickedEventHandler HelpButtonClicked;

        public delegate void HelpButtonClickedEventHandler();

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            //Load the help for the current page
            if (HelpButtonClicked != null)
            {
                HelpButtonClicked();
            }
        }

        public event AboutButtonClickedEventHandler AboutButtonClicked;

        public delegate void AboutButtonClickedEventHandler();

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            //Load the About Dialog
            if (AboutButtonClicked != null)
            {
                AboutButtonClicked();
            }
        }

        public event ExitButtonClickedEventHandler ExitButtonClicked;

        public delegate void ExitButtonClickedEventHandler();

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise the click event
            if (ExitButtonClicked != null)
            {
                ExitButtonClicked();
            }
        }

        #endregion

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new DelcamDesktopAutomationPeer(this);
        }
    }
}