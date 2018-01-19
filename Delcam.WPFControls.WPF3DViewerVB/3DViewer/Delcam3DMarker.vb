Imports System.Windows.Media.Media3D

Public Class Delcam3DMarker
    Inherits Delcam3DEntity

#Region "Attributes"

    Private objPosition As Point3D
    Private objNormal As Vector3D

#End Region

#Region "Constructors"

    Public Sub New(objPosition As Point3D,
                   objNormal As Vector3D)

        Me.objPosition = objPosition
        Me.objNormal = objNormal
    End Sub

#End Region

#Region "Operations"

    Public Function CreateMarker() As ModelUIElement3D

        Dim objMesh As New MeshGeometry3D
        With objPosition
            Dim objPointPlusNormal As Point3D = objPosition + (objNormal*5)
            Dim objVector As New Vector3D(objNormal.X, - objNormal.Z, objNormal.Y)
            Dim objCross As Vector3D = Vector3D.CrossProduct(objNormal, objVector)
            objCross.Normalize()
            objCross *= 2
            Dim objCross2 As Vector3D = Vector3D.CrossProduct(objCross, objNormal)
            objCross2.Normalize()
            objCross2 *= 2
            Dim objPoint1 As Point3D = objPointPlusNormal + objCross
            Dim objPoint3 As Point3D = objPointPlusNormal - objCross
            Dim objPoint2 As Point3D = objPointPlusNormal + objCross2
            Dim objPoint4 As Point3D = objPointPlusNormal - objCross2
            'triangle 1
            objMesh.Positions.Add(objPosition)
            objMesh.Positions.Add(objPoint1)
            objMesh.Positions.Add(objPoint2)
            'triangle 2
            objMesh.Positions.Add(objPosition)
            objMesh.Positions.Add(objPoint2)
            objMesh.Positions.Add(objPoint3)
            'triangle 3
            objMesh.Positions.Add(objPosition)
            objMesh.Positions.Add(objPoint3)
            objMesh.Positions.Add(objPoint4)
            'triangle 4
            objMesh.Positions.Add(objPosition)
            objMesh.Positions.Add(objPoint4)
            objMesh.Positions.Add(objPoint1)
            'triangle 5
            objMesh.Positions.Add(objPoint3)
            objMesh.Positions.Add(objPoint2)
            objMesh.Positions.Add(objPoint1)
            'triangle 6
            objMesh.Positions.Add(objPoint1)
            objMesh.Positions.Add(objPoint4)
            objMesh.Positions.Add(objPoint3)
        End With

        Dim objMaterial As New DiffuseMaterial(New SolidColorBrush(Colors.Blue))
        Dim objModel As New GeometryModel3D(objMesh, objMaterial)
        Dim objVisual As New ModelUIElement3D
        objVisual.Model = objModel

        Me.FrontVisual = objVisual

        Return objVisual
    End Function

#End Region

#Region "Properties"

    Public ReadOnly Property Position As Point3D
        Get
            Return objPosition
        End Get
    End Property

    Public ReadOnly Property Normal As Vector3D
        Get
            Return objNormal
        End Get
    End Property

#End Region
End Class
