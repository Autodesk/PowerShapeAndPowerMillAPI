Imports Autodesk.ProductInterface.PowerSHAPE

''' <summary>
''' Allows to show PowerSHAPE embedded into a WPF control.
''' </summary>
''' <remarks></remarks>
Public Class DelcamPowerSHAPEBase
    Inherits DelcamProductBase

    ''' <summary>
    ''' Gets or sets the base instance to interact with PowerSHAPE.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Automation As PSAutomation
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
                                                                                                  GetType(PSAutomation),
                                                                                                  GetType(DelcamPowerSHAPEBase),
                                                                                                  New PropertyMetadata(Nothing,
                                                                                                                       New _
                                                                                                                          PropertyChangedCallback(
                                                                                                                              AddressOf _
                                                                                                                                                     OnAutomationChanged)))

    ''' <summary>
    ''' Change notification callback for AutomationProperty.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Shared Sub OnAutomationChanged(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim powershapeFrame = DirectCast(sender, DelcamPowerSHAPEBase)
        powershapeFrame.Automation = e.NewValue
    End Sub

    Public Overrides Sub Close()

        If (Automation IsNot Nothing AndAlso (UseExternalApplicationWindowInDebug = False OrElse Debugger.IsAttached = False)) _
            Then
            Automation.Quit()
        End If
    End Sub
End Class
