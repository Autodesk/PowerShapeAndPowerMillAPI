Imports Autodesk.ProductInterface

Public MustInherit Class DelcamProductBase
    Inherits Border

#Region "Attributes"

    Private objProductHost As DelcamProductHost
    Protected objApplication As Automation
    Protected blnUseExternalApplicationWindowInDebug As Boolean
    Protected objVersion As Version
    Private objPanel As DockPanel
    Private objToolbar As StackPanel

#End Region

#Region "Constructors"

    Public Sub New()

        IsLooseEmbedded = False

        'Setup the Toolbar
        objToolbar = New StackPanel
        objToolbar.Orientation = Orientation.Vertical

        'Add the buttons
        Dim objViewZPos As New DelcamButton
        With objViewZPos
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ViewFromTop
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Z+.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewZPos)

        Dim objViewZNeg As New DelcamButton
        With objViewZNeg
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ViewFromBottom
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Z-.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewZNeg)

        objToolbar.Children.Add(New Separator)

        Dim objViewXPos As New DelcamButton
        With objViewXPos
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ViewFromRight
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/X+.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewXPos)

        Dim objViewXNeg As New DelcamButton
        With objViewXNeg
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ViewFromLeft
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/X-.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewXNeg)

        objToolbar.Children.Add(New Separator)

        Dim objViewYPos As New DelcamButton
        With objViewYPos
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ViewFromBack
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Y+.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewYPos)

        Dim objViewYNeg As New DelcamButton
        With objViewYNeg
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ViewFromFront
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Y-.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewYNeg)

        objToolbar.Children.Add(New Separator)

        Dim objViewIso1 As New DelcamButton
        With objViewIso1
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ISO1
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Iso1.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewIso1)

        Dim objViewIso2 As New DelcamButton
        With objViewIso2
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ISO2
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Iso2.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewIso2)

        Dim objViewIso3 As New DelcamButton
        With objViewIso3
            .ImageWidth = 16
            .ImageHeight = 16
            .Tag = ViewAngles.ISO3
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Iso3.ico",
                                                   UriKind.Relative))
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewIso3)

        Dim objViewIso4 As New DelcamButton
        With objViewIso4
            .ImageWidth = 16
            .ImageHeight = 16
            .ImageSource = New BitmapImage(New Uri("/Autodesk.WPFControls.ProductControls;component/Images/Iso4.ico",
                                                   UriKind.Relative))
            .Tag = ViewAngles.ISO4
            AddHandler .Click, AddressOf ButtonClicked
        End With
        objToolbar.Children.Add(objViewIso4)
    End Sub

    Public Sub Initialise()

        objPanel = New DockPanel

        objPanel.Children.Add(objToolbar)
        objToolbar.SetValue(DockPanel.DockProperty, Dock.Right)

        If (Debugger.IsAttached And blnUseExternalApplicationWindowInDebug) Then
            objApplication.IsVisible = True
        Else
            objProductHost = New DelcamProductHost(objApplication, Me)
            objPanel.Children.Add(objProductHost)
        End If

        Me.Child = objPanel
    End Sub

#End Region

#Region "Properties"

    Public Property UseExternalApplicationWindowInDebug As Boolean
        Get
            Return blnUseExternalApplicationWindowInDebug
        End Get
        Set
            blnUseExternalApplicationWindowInDebug = value
        End Set
    End Property

    ''' <summary>
    ''' Signifies whether the embedded window will be embedded with the window edges
    ''' visible or not.  If set to true then the window edges will be visible.  Hence
    ''' it is loosely embedded rather than being seamlessly embedded
    ''' </summary>
    Public Property IsLooseEmbedded As Boolean

    Public Property Toolbar As StackPanel
        Get
            Return objToolbar
        End Get
        Set
            objToolbar = value
        End Set
    End Property

    Public Property ToolbarPosition As Dock
        Get
            Return objToolbar.GetValue(DockPanel.DockProperty)
        End Get
        Set
            objToolbar.SetValue(DockPanel.DockProperty, value)
        End Set
    End Property

    Public Property ToolbarVisibility As Visibility
        Get
            Return objToolbar.Visibility
        End Get
        Set
            objToolbar.Visibility = value
        End Set
    End Property

#End Region

#Region " Operations "

    Public MustOverride Sub Close()

#End Region

#Region "Event Handlers"

    Private Sub ButtonClicked(sender As Object,
                              e As RoutedEventArgs)

        objApplication.SetViewAngle(sender.Tag)
    End Sub

#End Region
End Class
