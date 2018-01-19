Public Class Delcam3DDistanceMeasureModel
    Inherits Delcam3DModelBase

#Region "Attributes"

    Private objStartModel As Delcam3DMarker
    Private objEndModel As Delcam3DMarker
    Private objLineModel As Delcam3DLine
    Private dblDistance As Double

#End Region

#Region "Constructors"

    Public Sub New(strName As String)

        MyBase.New(strName)

        Me.objStartModel = Nothing
        Me.objEndModel = Nothing
        Me.objLineModel = Nothing
        Me.dblDistance = Double.NaN
    End Sub

    Public Sub New(strName As String,
                   objStartModel As Delcam3DMarker,
                   objEndModel As Delcam3DMarker,
                   objLineModel As Delcam3DLine)

        MyBase.New(strName)

        Me.objStartModel = objStartModel
        Me.objEndModel = objEndModel
        Me.objLineModel = objLineModel

        CalculateDistance()
    End Sub

#End Region

#Region "Properties"

    Public Property StartModel As Delcam3DMarker
        Get
            Return objStartModel
        End Get
        Set
            objStartModel = value
            CalculateDistance()
        End Set
    End Property

    Public Property EndModel As Delcam3DMarker
        Get
            Return objEndModel
        End Get
        Set
            objEndModel = value
            CalculateDistance()
        End Set
    End Property

    Public Property LineModel As Delcam3DLine
        Get
            Return objLineModel
        End Get
        Set
            objLineModel = value
        End Set
    End Property

    Public ReadOnly Property Distance As Double
        Get
            Return dblDistance
        End Get
    End Property

#End Region

#Region "Operations"

    Friend Overrides Sub DeleteFromViewport(objViewport As Viewport3D)

        objStartModel.DeleteFromViewport(objViewport)
        objEndModel.DeleteFromViewport(objViewport)
        objLineModel.DeleteFromViewport(objViewport)
    End Sub

    Private Sub CalculateDistance()

        If (objStartModel IsNot Nothing) And (objEndModel IsNot Nothing) Then
            dblDistance = Math.Abs((objEndModel.Position - objStartModel.Position).Length)
        End If
    End Sub

#End Region
End Class
