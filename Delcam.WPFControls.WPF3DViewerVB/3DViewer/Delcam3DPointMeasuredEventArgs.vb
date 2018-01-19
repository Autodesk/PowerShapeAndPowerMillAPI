

Public Class Delcam3DPointMeasuredEventArgs
    Inherits EventArgs

    Private objMeasureModel As Delcam3DPointMeasureModel

    Friend Sub New(objMeasureModel As Delcam3DPointMeasureModel)

        Me.objMeasureModel = objMeasureModel
    End Sub

    Public ReadOnly Property MeasureModel As Delcam3DPointMeasureModel
        Get
            Return objMeasureModel
        End Get
    End Property
End Class
