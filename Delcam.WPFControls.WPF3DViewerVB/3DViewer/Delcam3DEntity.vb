Imports System.Windows.Media.Media3D

Public Class Delcam3DEntity

#Region "Attributes"

    Private objFrontVisual As Visual3D
    Private objBackVisual As Visual3D

#End Region

#Region "Constructors"

    Public Sub New()

        Me.objFrontVisual = Nothing
        Me.objBackVisual = Nothing
    End Sub

    Public Sub New(objFrontVisual As Visual3D,
                   Optional ByVal objBackVisual As Visual3D = Nothing)

        Me.objFrontVisual = objFrontVisual
        Me.objBackVisual = objBackVisual
    End Sub

#End Region

#Region "Properties"

    Public Property FrontVisual As Visual3D
        Get
            Return objFrontVisual
        End Get
        Set
            objFrontVisual = value
        End Set
    End Property

    Public Property BackVisual As Visual3D
        Get
            Return objBackVisual
        End Get
        Set
            objBackVisual = value
        End Set
    End Property

#End Region

#Region "Operations"

    Friend Sub DeleteFromViewport(objViewport As Viewport3D)

        If (objFrontVisual IsNot Nothing) AndAlso (objViewport.Children.Contains(objFrontVisual)) Then
            objViewport.Children.Remove(objFrontVisual)
        End If
        If (objBackVisual IsNot Nothing) AndAlso (objViewport.Children.Contains(objBackVisual)) Then
            objViewport.Children.Remove(objBackVisual)
        End If
    End Sub

#End Region
End Class
