Public Class Delcam3DModelsList (Of T As Delcam3DModelBase)
    Inherits List(Of T)

#Region "Properties"

    Private objViewport As Viewport3D

#End Region

#Region "Constructors"

    Public Sub New(objViewport As Viewport3D)

        Me.objViewport = objViewport
    End Sub

#End Region

#Region "Properties"

    Public Property Viewport As Viewport3D
        Get
            Return objViewport
        End Get
        Set
            objViewport = value
        End Set
    End Property

#End Region

#Region "Operations"

    Public Overloads Function Contains(strName As String) As Boolean

        For Each objModel As Delcam3DModelBase In Me
            If (objModel.Name = strName) Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function NextFreeName(strName As String) As String

        Dim strNextFreeName As String = strName
        Dim intCounter = 0

        While Contains(strNextFreeName)
            strNextFreeName = strName & "_" & intCounter
            intCounter += 1
        End While

        Return strNextFreeName
    End Function

    Public Overloads Function Remove(strName As String) As Boolean

        For Each objModel As Delcam3DModelBase In Me
            If (objModel.Name = strName) Then
                objModel.DeleteFromViewport(objViewport)
                MyBase.Remove(objModel)
                Return True
            End If
        Next

        Return False
    End Function

#End Region
End Class
