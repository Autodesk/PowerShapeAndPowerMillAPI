Imports System.Windows.Media.Media3D

Public Class Delcam3DLine
    Inherits Delcam3DEntity

#Region "Attributes"

    Private objStartPosition As Point3D
    Private objEndPosition As Point3D
    Private dblThickness As Double

#End Region

#Region "Constructors"

    Public Sub New(objStartPosition As Point3D,
                   objEndPosition As Point3D,
                   Optional ByVal dblThickness As Double = 0.1)

        Me.objStartPosition = objStartPosition
        Me.objEndPosition = objEndPosition
        Me.dblThickness = dblThickness
    End Sub

#End Region

#Region "Operations"

    Public Function CreateLine(colColour As Color) As ModelUIElement3D

        Dim objMesh As New MeshGeometry3D
        Dim objVector As Vector3D = objEndPosition - objStartPosition
        Dim objPerp As Vector3D
        If (objVector.X = 0.0 And objVector.Y <> 0.0 And objVector.Z <> 0.0) Then
            objPerp = New Vector3D(1.0, 0.0, 0.0)
        ElseIf (objVector.Y = 0.0 And objVector.X <> 0.0 And objVector.Z <> 0.0) Then
            objPerp = New Vector3D(0.0, 1.0, 0.0)
        ElseIf (objVector.Z = 0.0 And objVector.Y <> 0.0 And objVector.X <> 0.0) Then
            objPerp = New Vector3D(0.0, 0.0, 1.0)
        ElseIf (objVector.Y <> objVector.Z) Then
            objPerp = New Vector3D(objVector.X, - objVector.Z, objVector.Y)
        ElseIf (objVector.X <> objVector.Y) Then
            objPerp = New Vector3D(- objVector.Y, objVector.X, objVector.Z)
        Else
            objPerp = New Vector3D(objVector.Z, objVector.Y, - objVector.X)
        End If

        objVector.Normalize()
        objStartPosition = objStartPosition + (objVector*- dblThickness)
        objEndPosition = objEndPosition + (objVector*dblThickness)

        objPerp.Normalize()
        objPerp *= dblThickness
        Dim objCross As Vector3D = Vector3D.CrossProduct(objVector, objPerp)
        objCross.Normalize()
        objCross *= dblThickness
        Dim objPoint1 As Point3D = objStartPosition + objPerp
        Dim objPoint3 As Point3D = objStartPosition - objPerp
        Dim objPoint2 As Point3D = objStartPosition + objCross
        Dim objPoint4 As Point3D = objStartPosition - objCross

        Dim objPoint5 As Point3D = objEndPosition + objPerp
        Dim objPoint7 As Point3D = objEndPosition - objPerp
        Dim objPoint6 As Point3D = objEndPosition + objCross
        Dim objPoint8 As Point3D = objEndPosition - objCross
        'triangle 1
        objMesh.Positions.Add(objPoint2)
        objMesh.Positions.Add(objPoint5)
        objMesh.Positions.Add(objPoint1)
        'triangle 2
        objMesh.Positions.Add(objPoint2)
        objMesh.Positions.Add(objPoint6)
        objMesh.Positions.Add(objPoint5)
        'triangle 3
        objMesh.Positions.Add(objPoint3)
        objMesh.Positions.Add(objPoint6)
        objMesh.Positions.Add(objPoint2)
        'triangle 4
        objMesh.Positions.Add(objPoint3)
        objMesh.Positions.Add(objPoint7)
        objMesh.Positions.Add(objPoint6)
        'triangle 5
        objMesh.Positions.Add(objPoint4)
        objMesh.Positions.Add(objPoint7)
        objMesh.Positions.Add(objPoint3)
        'triangle 6
        objMesh.Positions.Add(objPoint4)
        objMesh.Positions.Add(objPoint8)
        objMesh.Positions.Add(objPoint7)
        'triangle 7
        objMesh.Positions.Add(objPoint1)
        objMesh.Positions.Add(objPoint8)
        objMesh.Positions.Add(objPoint4)
        'triangle 8
        objMesh.Positions.Add(objPoint1)
        objMesh.Positions.Add(objPoint5)
        objMesh.Positions.Add(objPoint8)

        Dim aMaterial As New DiffuseMaterial(New SolidColorBrush(colColour))
        Dim aModel As New GeometryModel3D(objMesh, aMaterial)
        Dim objVisual As New ModelUIElement3D
        objVisual.Model = aModel

        Me.FrontVisual = objVisual

        Return objVisual
    End Function

#End Region

#Region "Properties"

    Public ReadOnly Property StartPosition As Point3D
        Get
            Return objStartPosition
        End Get
    End Property

    Public ReadOnly Property EndPosition As Point3D
        Get
            Return objEndPosition
        End Get
    End Property

    Public ReadOnly Property Thickness As Double
        Get
            Return dblThickness
        End Get
    End Property

#End Region
End Class
