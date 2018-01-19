Imports NUnit.Framework

<TestFixture>
Public Class DelcamPowerMILLBase
    ''' <summary>
    ''' Test to see if PowerMLL control works correctly
    ''' </summary>
    ''' <remarks></remarks>
    <Test>
    Public Sub PowerMILLStartupTest()

        'Create an instance of the control

        Dim objPowerMILL As New PowerMILL.DelcamPowerMILLBase()
        'Make sure it hasn't started PowerMILL yet
        If (objPowerMILL.Automation IsNot Nothing) Then
            Assert.Fail("Application initialised too early")
        End If
        'Make sure it hasn't started PowerMILL yet
        If (objPowerMILL.Automation IsNot Nothing) Then
            Assert.Fail("Application initialised too early")
        End If
        'Set whether to use an external app in Debug
        objPowerMILL.UseExternalApplicationWindowInDebug = True
        'It should now have started PowerMILL
        If (objPowerMILL.Automation Is Nothing) Then
            Assert.Fail("Application failed to start")
        End If
    End Sub
End Class
