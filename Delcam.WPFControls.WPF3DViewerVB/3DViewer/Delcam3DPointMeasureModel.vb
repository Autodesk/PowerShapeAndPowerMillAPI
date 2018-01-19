Public Class Delcam3DPointMeasureModel
    Inherits Delcam3DModelBase

#Region "Attributes"

    Private objMarkerModel As Delcam3DMarker

#End Region

#Region "Constructors"

    Public Sub New(strName As String)

        MyBase.New(strName)

        Me.objMarkerModel = Nothing
    End Sub

    Public Sub New(strName As String,
                   objMarkerModel As Delcam3DMarker)

        MyBase.New(strName)

        Me.objMarkerModel = objMarkerModel
    End Sub

#End Region

#Region "Properties"

    Public Property MarkerModel As Delcam3DMarker
        Get
            Return objMarkerModel
        End Get
        Set
            objMarkerModel = value
        End Set
    End Property

#End Region

#Region "Operations"

    Friend Overrides Sub DeleteFromViewport(objViewport As Viewport3D)

        objMarkerModel.DeleteFromViewport(objViewport)
    End Sub

#End Region
End Class
