

Public Class Delcam3DDistanceMeasuredEventArgs
    Inherits EventArgs

    Private objMeasureModel As Delcam3DDistanceMeasureModel

    Friend Sub New(objMeasureModel As Delcam3DDistanceMeasureModel)

        Me.objMeasureModel = objMeasureModel
    End Sub

    Public ReadOnly Property MeasureModel As Delcam3DDistanceMeasureModel
        Get
            Return objMeasureModel
        End Get
    End Property
End Class
