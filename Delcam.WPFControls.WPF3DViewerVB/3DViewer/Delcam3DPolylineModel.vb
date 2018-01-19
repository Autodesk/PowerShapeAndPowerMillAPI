Imports System.Windows.Media.Media3D
Imports Autodesk.Geometry

Public Class Delcam3DPolylineModel
    Inherits Delcam3DModelBase

    Private lstLines As List(Of Delcam3DLine)
    Private lstKeyPoints As List(Of Delcam3DMarker)
    Private blnIsClosed As Boolean

    Public Sub New(strName As String)

        MyBase.New(strName)

        lstLines = New List(Of Delcam3DLine)
        lstKeyPoints = New List(Of Delcam3DMarker)
        blnIsClosed = False
    End Sub

    Friend Sub New(polyline As Polyline,
                   colColour As Color,
                   thickness As Double,
                   viewer As Delcam3DViewer)

        MyBase.New("")

        lstKeyPoints = New List(Of Delcam3DMarker)

        lstLines = New List(Of Delcam3DLine)
        For i = 0 To polyline.Count - 2
            lstLines.Add(viewer.AddLine(New Point3D(polyline(i).X, polyline(i).Y, polyline(i).Z),
                                        New Point3D(polyline(i + 1).X, polyline(i + 1).Y, polyline(i + 1).Z),
                                        colColour,
                                        thickness))
        Next
        blnIsClosed = polyline.IsClosed
    End Sub

    Public Property KeyPoints As List(Of Delcam3DMarker)
        Get
            Return lstKeyPoints
        End Get
        Set
            lstKeyPoints = value
        End Set
    End Property

    Public Property Lines As List(Of Delcam3DLine)
        Get
            Return lstLines
        End Get
        Set
            lstLines = value
        End Set
    End Property

    Public Property IsClosed As Boolean
        Get
            Return blnIsClosed
        End Get
        Set
            blnIsClosed = value
        End Set
    End Property

    Friend Overrides Sub DeleteFromViewport(objViewport As Viewport3D)

        For Each objLine As Delcam3DLine In lstLines
            objLine.DeleteFromViewport(objViewport)
        Next
        For Each objMarker As Delcam3DMarker In lstKeyPoints
            objMarker.DeleteFromViewport(objViewport)
        Next
    End Sub
End Class
