Imports System.Windows.Media.Media3D

Public Class Delcam3DModel
    Inherits Delcam3DModelBase

#Region "Attributes"

    Private objEntity As Delcam3DEntity
    Private objBoundingBox As Delcam3DBoundingBox

#End Region

#Region "Constructors"

    Public Sub New(strName As String,
                   objFrontVisual As Visual3D,
                   Optional ByVal objBackVisual As Visual3D = Nothing,
                   Optional ByVal objBoundingBox As Delcam3DBoundingBox = Nothing)

        MyBase.New(strName)
        objEntity = New Delcam3DEntity(objFrontVisual, objBackVisual)
        Me.objBoundingBox = objBoundingBox
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property FrontVisual As Visual3D
        Get
            Return objEntity.FrontVisual
        End Get
    End Property

    Public ReadOnly Property BackVisual As Visual3D
        Get
            Return objEntity.BackVisual
        End Get
    End Property

    Public ReadOnly Property BoundingBox As Delcam3DBoundingBox
        Get
            Return objBoundingBox
        End Get
    End Property

#End Region

#Region "Operations"

    Friend Overrides Sub DeleteFromViewport(objViewport As Viewport3D)

        objEntity.DeleteFromViewport(objViewport)
    End Sub

#End Region
End Class
