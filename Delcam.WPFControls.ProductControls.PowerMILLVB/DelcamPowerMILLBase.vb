Imports Autodesk.ProductInterface.PowerMILL

''' <summary>
''' Allows to show PowerMill embedded into a WPF control.
''' </summary>
''' <remarks></remarks>
Public Class DelcamPowerMILLBase
    Inherits DelcamProductBase

    ''' <summary>
    ''' Gets or sets the base instance to interact with PowerMILL.
    ''' </summary>
    Public Property Automation As PMAutomation
        Get
            Return objApplication
        End Get
        Set
            ' If value is being set to nothing this could just be the control changing owners so ignore it
            If (value Is Nothing OrElse value Is objApplication) Then Return

            If Not objApplication Is Nothing Then
                If objApplication.IsRunning Then
                    objApplication.Quit()
                End If
            End If
            objApplication = value

            MyBase.Initialise()
        End Set
    End Property

    ''' <summary>
    ''' Implementation of the dependency property.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared ReadOnly AutomationProperty As DependencyProperty = DependencyProperty.Register("Automation",
                                                                                                  GetType(PMAutomation),
                                                                                                  GetType(DelcamPowerMILLBase),
                                                                                                  New PropertyMetadata(Nothing,
                                                                                                                       New _
                                                                                                                          PropertyChangedCallback(
                                                                                                                              AddressOf _
                                                                                                                                                     OnAutomationChanged)))

    ''' <summary>
    ''' Change notification callback for AutomationProperty.
    ''' </summary>
    Private Shared Sub OnAutomationChanged(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim powermillFrame = DirectCast(sender, DelcamPowerMILLBase)
        powermillFrame.Automation = e.NewValue
    End Sub

    Public Overrides Sub Close()

        If (Automation IsNot Nothing AndAlso (UseExternalApplicationWindowInDebug = False OrElse Debugger.IsAttached = False)) _
            Then
            Automation.Quit()
        End If
    End Sub
End Class
