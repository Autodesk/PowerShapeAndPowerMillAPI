Public Class Delcam3DBoundingBox

#Region "Attributes"

    Private dblMinX As Double
    Private dblMaxX As Double
    Private dblMinY As Double
    Private dblMaxY As Double
    Private dblMinZ As Double
    Private dblMaxZ As Double

#End Region

#Region "Constructors"

    Friend Sub New(dblMinX As Double,
                   dblMaxX As Double,
                   dblMinY As Double,
                   dblMaxY As Double,
                   dblMinZ As Double,
                   dblMaxZ As Double)

        Me.dblMinX = dblMinX
        Me.dblMaxX = dblMaxX
        Me.dblMinY = dblMinY
        Me.dblMaxY = dblMaxY
        Me.dblMinZ = dblMinZ
        Me.dblMaxZ = dblMaxZ
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property MinX As Double
        Get
            Return dblMinX
        End Get
    End Property

    Public ReadOnly Property MaxX As Double
        Get
            Return dblMaxX
        End Get
    End Property

    Public ReadOnly Property MinY As Double
        Get
            Return dblMinY
        End Get
    End Property

    Public ReadOnly Property MaxY As Double
        Get
            Return dblMaxY
        End Get
    End Property

    Public ReadOnly Property MinZ As Double
        Get
            Return dblMinZ
        End Get
    End Property

    Public ReadOnly Property MaxZ As Double
        Get
            Return dblMaxZ
        End Get
    End Property

    Public ReadOnly Property SizeX As Double
        Get
            Return dblMaxX - dblMinX
        End Get
    End Property

    Public ReadOnly Property SizeY As Double
        Get
            Return dblMaxY - dblMinY
        End Get
    End Property

    Public ReadOnly Property SizeZ As Double
        Get
            Return dblMaxZ - dblMinZ
        End Get
    End Property

#End Region
End Class
