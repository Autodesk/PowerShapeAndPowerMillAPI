

Public MustInherit Class Delcam3DModelBase

#Region "Attributes"

    Private strName As String

#End Region

#Region "Constructors"

    Public Sub New(strName As String)

        Me.strName = strName
    End Sub

#End Region

#Region "Properties"

    Public Property Name As String
        Get
            Return strName
        End Get
        Set
            strName = value
        End Set
    End Property

#End Region

#Region "Operations"

    Friend MustOverride Sub DeleteFromViewport(objViewport As Viewport3D)

#End Region
End Class
