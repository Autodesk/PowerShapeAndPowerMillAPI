Imports NUnit.Framework

<TestFixture>
Public Class DelcamPowerSHAPEBase
    ''' <summary>
    ''' Test to see if PowerSHAPE control works correctly
    ''' </summary>
    ''' <remarks></remarks>
    <Test>
    Public Sub PowerSHAPEStartupTest()

        'Create an instance of the control
        Dim objPowerSHAPE As New PowerSHAPE.DelcamPowerSHAPEBase()
        'Make sure it hasn't started PowerSHAPE yet
        If (objPowerSHAPE.Automation IsNot Nothing) Then
            Assert.Fail("Application initialised too early")
        End If
        'Make sure it hasn't started PowerSHAPE yet
        If (objPowerSHAPE.Automation IsNot Nothing) Then
            Assert.Fail("Application initialised too early")
        End If
        'Set whether to use an external app in Debug
        objPowerSHAPE.UseExternalApplicationWindowInDebug = True
        'It should now have started PowerSHAPE
        If (objPowerSHAPE.Automation Is Nothing) Then
            Assert.Fail("Application failed to start")
        End If
    End Sub
End Class
