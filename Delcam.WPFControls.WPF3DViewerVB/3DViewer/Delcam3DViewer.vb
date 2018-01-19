Imports System.IO
Imports System.Timers
Imports System.Windows.Controls.Primitives
Imports System.Windows.Media.Media3D
Imports Autodesk.FileSystem
Imports Autodesk.Geometry
Imports File = Autodesk.FileSystem.File
'Imports Autodesk.DelcamWPFControls
'Imports System.Windows.Forms

<TemplatePart(Name := "PART_Camera", Type := GetType(OrthographicCamera))> _
<TemplatePart(Name := "PART_Light", Type := GetType(DirectionalLight))> _
<TemplatePart(Name := "PART_Viewport3D", Type := GetType(Viewport3D))> _
<TemplatePart(Name := "PART_Views", Type := GetType(Expander))>
Public Class Delcam3DViewer
    Inherits Control

#Region "Constants"

    Private Shared ReadOnly PART_Camera As String = "PART_Camera"
    Private Shared ReadOnly PART_Light As String = "PART_Light"
    Private Shared ReadOnly PART_Viewport3D As String = "PART_Viewport3D"
    Private Shared ReadOnly PART_Views As String = "PART_Views"

#End Region

#Region "Enumerations"

    Private Enum E_LeftClickMode
        None
        MeasurePoint
        RemeasurePoint
        MeasureDistanceStartPoint
        MeasureDistanceEndPoint
        RemeasureDistanceStartPoint
        RemeasureDistanceEndPoint
        SketchCurve
        ReselectCurvePoint
    End Enum

    Private Enum E_ActiveCommand
        None
        PointMeasure
        DistanceMeasure
        SketchCurve
    End Enum

#End Region

#Region "Attributes"

    Private lstModels As Delcam3DModelsList(Of Delcam3DModel)

    Private lstPointMeasureModels As Delcam3DModelsList(Of Delcam3DPointMeasureModel)
    Private lstDistanceMeasureModels As Delcam3DModelsList(Of Delcam3DDistanceMeasureModel)
    Private lstCurveModels As Delcam3DModelsList(Of Delcam3DPolylineModel)

    Private enmLeftClickMode As E_LeftClickMode
    Private enmActiveCommand As E_ActiveCommand

    Private _isPanningAllowed As Boolean = True
    Private _isZoomingAllowed As Boolean = True
    Private _isRotatingAllowed As Boolean = True

#End Region

#Region "Constructors"

    Shared Sub New()

        'This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
        'This style is defined in themes\generic.xaml
        DefaultStyleKeyProperty.OverrideMetadata(GetType(Delcam3DViewer), New FrameworkPropertyMetadata(GetType(Delcam3DViewer)))
    End Sub

    Public Sub New()

        InitialiseViewCommands()
        InitialiseMeasureCommands()
        InitialiseSketchCurveCommands()
    End Sub

#End Region

#Region "Operations"

    Public Overrides Sub OnApplyTemplate()

        Dim objViewport As Viewport3D = GetTemplateChild(PART_Viewport3D)

        If (lstModels Is Nothing) Then
            lstModels = New Delcam3DModelsList(Of Delcam3DModel)(objViewport)
        Else
            lstModels.Viewport = objViewport
        End If

        If (lstCurveModels Is Nothing) Then
            lstCurveModels = New Delcam3DModelsList(Of Delcam3DPolylineModel)(objViewport)
        Else
            lstCurveModels.Viewport = objViewport
        End If

        If (lstPointMeasureModels Is Nothing) Then
            lstPointMeasureModels = New Delcam3DModelsList(Of Delcam3DPointMeasureModel)(objViewport)
        Else
            lstPointMeasureModels.Viewport = objViewport
        End If

        If (lstDistanceMeasureModels Is Nothing) Then
            lstDistanceMeasureModels = New Delcam3DModelsList(Of Delcam3DDistanceMeasureModel)(objViewport)
        Else
            lstDistanceMeasureModels.Viewport = objViewport
        End If
    End Sub

    Public Sub ClearAll()

        ClearModels()
        ClearMarkers()
        ClearCurves()
    End Sub

    Public Sub ClearModels()

        For Each objModel As Delcam3DModel In lstModels
            objModel.DeleteFromViewport(GetTemplateChild(PART_Viewport3D))
        Next
        lstModels.Clear()
    End Sub

    Public Sub ClearMarkers()

        For Each objModel As Delcam3DPointMeasureModel In lstPointMeasureModels
            objModel.DeleteFromViewport(GetTemplateChild(PART_Viewport3D))
        Next
        lstPointMeasureModels.Clear()
        For Each objModel As Delcam3DDistanceMeasureModel In lstDistanceMeasureModels
            objModel.DeleteFromViewport(GetTemplateChild(PART_Viewport3D))
        Next
        lstDistanceMeasureModels.Clear()
    End Sub

    Public Sub ClearCurves()

        For Each objModel As Delcam3DPolylineModel In lstCurveModels
            objModel.DeleteFromViewport(GetTemplateChild(PART_Viewport3D))
        Next
        lstCurveModels.Clear()
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property Camera As OrthographicCamera
        Get
            Return Template.FindName(PART_Camera, Me)
        End Get
    End Property

    Public Property IsPanningAllowed As Boolean
        Get
            Return _isPanningAllowed
        End Get
        Set
            _isPanningAllowed = value
        End Set
    End Property

    Public Property IsZoomingAllowed As Boolean
        Get
            Return _isZoomingAllowed
        End Get
        Set
            _isZoomingAllowed = value
        End Set
    End Property

    Public Property IsRotatingAllowed As Boolean
        Get
            Return _isRotatingAllowed
        End Get
        Set
            _isRotatingAllowed = value
        End Set
    End Property

    Public Shared ReadOnly _
        ViewsBarVisibilityProperty As DependencyProperty = DependencyProperty.Register("ViewsBarVisibility",
                                                                                       GetType(Visibility),
                                                                                       GetType(Delcam3DViewer),
                                                                                       New PropertyMetadata(Visibility.Visible))

    Public Property ViewsBarVisibility As Visibility
        Get
            Return DirectCast(GetValue(ViewsBarVisibilityProperty), Visibility)
        End Get
        Set
            SetValue(ViewsBarVisibilityProperty, value)
        End Set
    End Property

#End Region

#Region "Mouse Interaction"

#Region "Attributes"

    Private objLastMousePosition As Windows.Point

#End Region

#Region "Operations"

    Private Sub Me_MouseMove(sender As Object,
                             e As MouseEventArgs) Handles Me.MouseMove

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            If (e.MiddleButton = MouseButtonState.Pressed) Then
                Dim newMousePos As Windows.Point = e.GetPosition(Me)
                Dim xChange As Integer = (objLastMousePosition.X - newMousePos.X)*(CDbl(myCamera.Width)/CDbl(Me.ActualWidth))
                Dim yChange As Integer = (objLastMousePosition.Y - newMousePos.Y)*(CDbl(myCamera.Width)/CDbl(Me.ActualWidth))

                If (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) And _isPanningAllowed Then
                    myCamera.Position = New Point3D(myCamera.Position.X + xChange,
                                                    myCamera.Position.Y - yChange,
                                                    myCamera.Position.Z)
                ElseIf (_isRotatingAllowed) Then

                    'Rotate the view

                    Dim currentPosition3D As Vector3D = ProjectToTrackball(Me.ActualWidth, Me.ActualHeight, newMousePos)

                    If (currentPosition3D <> ProjectToTrackball(Me.ActualWidth, Me.ActualHeight, objLastMousePosition)) Then

                        Dim axis As Vector3D = Vector3D.CrossProduct(
                            ProjectToTrackball(Me.ActualWidth, Me.ActualHeight, objLastMousePosition),
                            currentPosition3D)
                        Dim angle As Double = Vector3D.AngleBetween(
                            ProjectToTrackball(Me.ActualWidth, Me.ActualHeight, objLastMousePosition),
                            currentPosition3D)
                        Dim delta As New Quaternion(axis, - angle)

                        Dim rt As RotateTransform3D = myCamera.Transform
                        Dim q As Quaternion = CType(rt.Rotation, QuaternionRotation3D).Quaternion
                        'Compose the delta with the previous orientation
                        q *= delta

                        myCamera.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
                        myLight.Transform = New RotateTransform3D(New QuaternionRotation3D(q))

                    End If

                End If
                objLastMousePosition = newMousePos
            End If
        End If
    End Sub

    Private Function ProjectToTrackball(width As Double,
                                        height As Double,
                                        point As Windows.Point) As Vector3D
        Dim x As Double = point.X/(width/2)
        Dim y As Double = point.Y/(height/2)

        x = x - 1
        y = 1 - y

        Dim z2 As Double = 1 - x*x - y*y
        Dim z As Double = IIf(z2 > 0, Math.Sqrt(z2), 0)

        Return New Vector3D(x, y, z)
    End Function

    Private Sub Me_PreviewMouseMiddleButtonDown(sender As Object,
                                                e As MouseButtonEventArgs) Handles Me.PreviewMouseDown

        If (e.ChangedButton = MouseButton.Middle) Then
            objLastMousePosition = e.GetPosition(Me)
        End If
    End Sub

    Private Sub Me_MouseWheel(sender As Object,
                              e As MouseWheelEventArgs) Handles Me.MouseWheel

        If (_isZoomingAllowed = False) Then Return

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)

        If (myCamera IsNot Nothing) Then
            If ((e.Delta*0.1) < myCamera.Width) Then
                myCamera.Width -= (e.Delta*0.1)
            End If
        End If
    End Sub

    Private Sub Me_MouseLeftButtonDown(sender As Object,
                                       e As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown

        Select Case enmLeftClickMode
            Case E_LeftClickMode.None
                'Nothing to do
            Case E_LeftClickMode.MeasurePoint,
                E_LeftClickMode.RemeasurePoint
                PointMeasure_MouseLeftButtonDown(sender, e)
            Case E_LeftClickMode.MeasureDistanceStartPoint,
                E_LeftClickMode.MeasureDistanceEndPoint,
                E_LeftClickMode.RemeasureDistanceStartPoint,
                E_LeftClickMode.RemeasureDistanceEndPoint
                DistanceMeasure_MouseLeftButtonDown(sender, e)
            Case E_LeftClickMode.SketchCurve,
                E_LeftClickMode.ReselectCurvePoint
                SketchCurve_MouseLeftButtonDown(sender, e)
        End Select
    End Sub

#End Region

#End Region

#Region "Curve Creation"

    Public Sub CreateCurve(polyline As Polyline,
                           colColour As Color,
                           thickness As Double)

        lstCurveModels.Add(New Delcam3DPolylineModel(polyline, colColour, thickness, Me))
    End Sub

#End Region

#Region "File Importers"

    Public Sub ImportFile(modelFile As FileSystem.File,
                          Optional ByVal frontOnly As Boolean = False)

        ImportFile(modelFile, Colors.PapayaWhip, Colors.WhiteSmoke, frontOnly)
    End Sub

    Public Sub ImportFile(modelFile As FileSystem.File,
                          objFrontColour As Color,
                          objBackColour As Color,
                          Optional ByVal frontOnly As Boolean = False)

        Select Case modelFile.Extension.ToUpper
            Case "OBJ"
                ImportOBJFile(modelFile, objFrontColour, objBackColour, frontOnly)
            Case "STL"
                ImportSTLFile(modelFile, objFrontColour, objBackColour, frontOnly)
            Case "DMT"
                ImportDMTFile(modelFile, objFrontColour, objBackColour, frontOnly)
        End Select
    End Sub

#Region "OBJ File Importer"

    Private Sub ImportOBJFile(modelFile As FileSystem.File,
                              objFrontColour As Color,
                              objBackColour As Color,
                              Optional ByVal frontOnly As Boolean = False)

        Dim objViewer As Viewport3D = GetTemplateChild(PART_Viewport3D)
        If objViewer Is Nothing Then Return

        If (modelFile.Exists = False) Then
            Throw New FileNotFoundException(modelFile.Name & " not found")
        End If

        Dim objMesh As New MeshGeometry3D
        Dim objBackMesh As New MeshGeometry3D
        Dim objMaterials As MaterialGroup = Nothing
        Dim lstVertices As New List(Of Point3D)
        Dim lstNormals As New List(Of Vector3D)
        Dim lstTextureCoords As New List(Of Windows.Point)
        Dim lstMaterials As New Dictionary(Of String, MaterialGroup)

        For Each strLine As String In modelFile.ReadTextLines()
            If (strLine.StartsWith("#")) Then
                'comment
            ElseIf (strLine.StartsWith("v ")) Then
                'vertex
                lstVertices.Add(Point3DFromLine(strLine))
            ElseIf (strLine.StartsWith("vt ")) Then
                'texture coord
                lstTextureCoords.Add(PointFromLine(strLine))
            ElseIf (strLine.StartsWith("vn ")) Then
                'vertex normal
                lstNormals.Add(Vector3DFromLine(strLine))
            ElseIf (strLine.StartsWith("f ")) Then
                'triangle face
                Dim strVertices() As String = strLine.Split(" ")
                'Ignore identity entry
                For i = 1 To strVertices.Length - 1
                    Dim strParts() As String = strVertices(i).Split("/")
                    objMesh.Positions.Add(lstVertices(strParts(0) - 1))
                    objMesh.TextureCoordinates.Add(lstTextureCoords.Item(strParts(1) - 1))
                    objMesh.Normals.Add(lstNormals.Item(strParts(2) - 1))
                Next
                If (frontOnly = False) Then
                    'Populate the Back Mesh too
                    For i = strVertices.Length - 1 To 1 Step - 1
                        Dim strParts() As String = strVertices(i).Split("/")
                        objBackMesh.Positions.Add(lstVertices(strParts(0) - 1))
                    Next
                End If
            ElseIf (strLine.StartsWith("g ")) Then
                'group
            ElseIf (strLine.StartsWith("mtllib ")) Then
                'materials
                MaterialsFromLine(strLine, modelFile.ParentDirectory, lstMaterials)
            ElseIf (strLine.StartsWith("usemtl ")) Then
                'set material
                Dim strName As String = strLine.Substring(strLine.IndexOf(" ") + 1)
                objMaterials = lstMaterials(strName)
            End If
        Next

        'Add the front visual
        Dim objModel As New GeometryModel3D(objMesh, objMaterials)
        Dim objFrontGroup As New Model3DGroup
        objFrontGroup.Children.Add(objModel)
        Dim objVisual As New ModelUIElement3D
        objVisual.Model = objFrontGroup
        objViewer.Children.Add(objVisual)

        Dim objBackVisual As ModelUIElement3D = Nothing
        If (frontOnly = False) Then
            'Add the back visual
            Dim objBackModel As New GeometryModel3D(objBackMesh, New DiffuseMaterial(New SolidColorBrush(objBackColour)))
            Dim objBackGroup As New Model3DGroup
            objBackGroup.Children.Add(objBackModel)
            objBackVisual = New ModelUIElement3D
            objBackVisual.Model = objBackGroup
            objViewer.Children.Add(objBackVisual)
        End If

        Dim strModelName As String = lstModels.NextFreeName(modelFile.NameWithoutExtension)
        lstModels.Add(New Delcam3DModel(strModelName, objVisual, objBackVisual))
    End Sub

    Private Function PointFromLine(strLine As String) As Windows.Point

        Dim strPoints() As String = strLine.Split(" ")

        'First entry is the identity character so ignore that one
        Return New Windows.Point(strPoints(1), 1.0 - strPoints(2))
    End Function

    Private Function Point3DFromLine(strLine As String) As Point3D

        Dim strPoints() As String = strLine.Split(" ")

        If (strPoints.Length <> 4) Then
            Debug.Print("dd")
        End If

        'First entry is the identity character so ignore that one
        Return New Point3D(strPoints(1), strPoints(2), strPoints(3))
    End Function

    Private Function Vector3DFromLine(strLine As String) As Vector3D

        Dim strComponents() As String = strLine.Split(" ")

        'First entry is the identity character so ignore that one
        Dim objVector As New Vector3D(strComponents(1), strComponents(2), strComponents(3))

        Return objVector
    End Function

    Private Sub MaterialsFromLine(strLine As String,
                                  fileDirectory As FileSystem.Directory,
                                  objDictionary As Dictionary(Of String, MaterialGroup))

        Dim materialFile As New File(fileDirectory, strLine.Substring(strLine.IndexOf(" ") + 1))

        Dim strMaterialName = ""
        Dim objMaterialGroup As New MaterialGroup

        For Each strMaterialLine As String In materialFile.ReadTextLines()
            If (strMaterialLine.StartsWith("newmtl ")) Then
                strMaterialName = strMaterialLine.Substring(strLine.IndexOf(" ") + 1)
            ElseIf (strMaterialLine.StartsWith("map_Ka ")) Then
                Dim strFileName As String = strMaterialLine.Substring(strLine.IndexOf(" ") + 1)
                Dim objBrush As New ImageBrush(New BitmapImage(New Uri(fileDirectory.Path & strFileName)))
                Dim objEmissiveMaterial As New EmissiveMaterial(objBrush)
                Dim objDiffuseMaterial As New DiffuseMaterial(objBrush)
                objMaterialGroup.Children.Add(objEmissiveMaterial)
                objMaterialGroup.Children.Add(objDiffuseMaterial)
            End If
        Next

        objDictionary.Add(strMaterialName, objMaterialGroup)
    End Sub

#End Region

#Region "STL File Importer"

    Private Sub ImportSTLFile(modelFile As FileSystem.File,
                              objFrontColour As Color,
                              objBackColour As Color,
                              Optional ByVal frontOnly As Boolean = False)

        If (modelFile.ReadText().StartsWith("solid")) Then
            ImportASCIISTLFile(modelFile, objFrontColour, objBackColour, frontOnly)
        Else
            ImportBinarySTLFile(modelFile, objFrontColour, objBackColour, frontOnly)
        End If
    End Sub

    Private Sub ImportASCIISTLFile(modelFile As FileSystem.File,
                                   objFrontColour As Color,
                                   objBackColour As Color,
                                   Optional ByVal frontOnly As Boolean = False)

        Dim objViewer As Viewport3D = GetTemplateChild(PART_Viewport3D)
        If objViewer Is Nothing Then Return

        If (modelFile.Exists = False) Then
            Throw New FileNotFoundException(modelFile.Name & " not found")
        End If

        Dim objMesh As New MeshGeometry3D
        Dim objBackMesh As New MeshGeometry3D

        Dim dblMinX As Double = Double.MaxValue
        Dim dblMinY As Double = Double.MaxValue
        Dim dblMinZ As Double = Double.MaxValue

        Dim dblMaxX As Double = Double.MinValue
        Dim dblMaxY As Double = Double.MinValue
        Dim dblMaxZ As Double = Double.MinValue

        For Each strLine As String In modelFile.ReadTextLines()
            If (strLine.Trim.StartsWith("facet normal ")) Then
                'Add three normals
                Dim strComponents As String() = strLine.Trim.Split(" ")
                Dim intCounter = 0
                Dim objVector As New Vector3D
                For Each strComponent As String In strComponents
                    If (IsNumeric(strComponent)) Then
                        If (intCounter = 0) Then
                            objVector.X = strComponent
                        ElseIf (intCounter = 1) Then
                            objVector.Y = strComponent
                        Else
                            objVector.Z = strComponent
                        End If
                        intCounter += 1
                    End If
                Next
                objMesh.Normals.Add(objVector)
                objMesh.Normals.Add(objVector)
                objMesh.Normals.Add(objVector)
                If (frontOnly = False) Then
                    'Add the back surface normals
                    Dim objBackVector As Vector3D = objVector
                    objBackVector.Negate()
                    objBackMesh.Normals.Add(objBackVector)
                    objBackMesh.Normals.Add(objBackVector)
                    objBackMesh.Normals.Add(objBackVector)
                End If
            ElseIf (strLine.Trim.StartsWith("vertex ")) Then
                'Add position
                Dim strCoords As String() = strLine.Trim.Split(" ")
                Dim intCounter = 0
                Dim objPoint As New Point3D
                For Each strCoord As String In strCoords
                    If (IsNumeric(strCoord)) Then
                        If (intCounter = 0) Then
                            objPoint.X = strCoord
                        ElseIf (intCounter = 1) Then
                            objPoint.Y = strCoord
                        Else
                            objPoint.Z = strCoord
                        End If
                        intCounter += 1
                    End If
                Next
                objMesh.Positions.Add(objPoint)
                If (objPoint.X < dblMinX) Then dblMinX = objPoint.X
                If (objPoint.Y < dblMinY) Then dblMinY = objPoint.Y
                If (objPoint.Z < dblMinZ) Then dblMinZ = objPoint.Z
                If (objPoint.X > dblMaxX) Then dblMaxX = objPoint.X
                If (objPoint.Y > dblMaxY) Then dblMaxY = objPoint.Y
                If (objPoint.Z > dblMaxZ) Then dblMaxZ = objPoint.Z
            Else
                'ignore
            End If
        Next

        If (frontOnly = False) Then
            'Add the back points
            For intIndex = 2 To objMesh.Positions.Count - 1 Step 3
                objBackMesh.Positions.Add(objMesh.Positions(intIndex))
                objBackMesh.Positions.Add(objMesh.Positions(intIndex - 1))
                objBackMesh.Positions.Add(objMesh.Positions(intIndex - 2))
            Next
        End If

        'Add the visual
        Dim objModel As New GeometryModel3D(objMesh, New DiffuseMaterial(New SolidColorBrush(objFrontColour)))
        Dim objFrontGroup As New Model3DGroup
        objFrontGroup.Children.Add(objModel)
        Dim objVisual As New ModelUIElement3D
        objVisual.Model = objFrontGroup
        objViewer.Children.Add(objVisual)

        Dim objBackVisual As ModelUIElement3D = Nothing
        If (frontOnly = False) Then
            'Add the back visual
            Dim objBackModel As New GeometryModel3D(objBackMesh, New EmissiveMaterial(New SolidColorBrush(objBackColour)))
            Dim objBackGroup As New Model3DGroup
            objBackGroup.Children.Add(objBackModel)
            objBackVisual = New ModelUIElement3D
            objBackVisual.Model = objBackGroup
            objViewer.Children.Add(objBackVisual)
        End If

        Dim strModelName As String = lstModels.NextFreeName(modelFile.NameWithoutExtension)
        lstModels.Add(New Delcam3DModel(strModelName,
                                        objVisual,
                                        objBackVisual,
                                        New Delcam3DBoundingBox(dblMinX,
                                                                dblMaxX,
                                                                dblMinY,
                                                                dblMaxY,
                                                                dblMinZ,
                                                                dblMaxZ)))
    End Sub

    Private Sub ImportBinarySTLFile(modelFile As FileSystem.File,
                                    objFrontColour As Color,
                                    objBackColour As Color,
                                    Optional ByVal frontOnly As Boolean = False)

        Dim objViewer As Viewport3D = GetTemplateChild(PART_Viewport3D)
        If objViewer Is Nothing Then Return

        If (modelFile.Exists = False) Then
            Throw New FileNotFoundException(modelFile.Path & " not found")
        End If

        Dim objMesh As New MeshGeometry3D
        Dim objBackMesh As New MeshGeometry3D

        Dim objReader As New BinaryFileReader(modelFile)

        'Read first 80 characters (bytes) and ignore them
        objReader.ReadBytes(80)

        'Read the next 4 bytes to get an unsigned integer of number of triangles
        Dim intNoOfFacets As UInteger = objReader.ReadUInteger

        'Now keep reading until the end of the file
        For intCounter = 0 To intNoOfFacets - 1
            'Read 3 32bit floating point numbers - triangle normal
            Dim objVector As New Vector3D
            objVector.X = objReader.ReadSingle
            objVector.Y = objReader.ReadSingle
            objVector.Z = objReader.ReadSingle
            'Add three normals
            objMesh.Normals.Add(objVector)
            objMesh.Normals.Add(objVector)
            objMesh.Normals.Add(objVector)
            If (frontOnly = False) Then
                'Add the back surface normals
                Dim objBackVector As Vector3D = objVector
                objBackVector.Negate()
                objBackMesh.Normals.Add(objBackVector)
                objBackMesh.Normals.Add(objBackVector)
                objBackMesh.Normals.Add(objBackVector)
            End If

            'Read 3 32bit floating point numbers - vertex 1 X/Y/Z
            Dim objPoint1 As New Point3D
            objPoint1.X = objReader.ReadSingle
            objPoint1.Y = objReader.ReadSingle
            objPoint1.Z = objReader.ReadSingle

            'Read 3 32bit floating point numbers - vertex 2 X/Y/Z
            Dim objPoint2 As New Point3D
            objPoint2.X = objReader.ReadSingle
            objPoint2.Y = objReader.ReadSingle
            objPoint2.Z = objReader.ReadSingle

            'Read 3 32bit floating point numbers - vertex 3 X/Y/Z
            Dim objPoint3 As New Point3D
            objPoint3.X = objReader.ReadSingle
            objPoint3.Y = objReader.ReadSingle
            objPoint3.Z = objReader.ReadSingle

            'Add positions
            objMesh.Positions.Add(objPoint1)
            objMesh.Positions.Add(objPoint2)
            objMesh.Positions.Add(objPoint3)
            If (frontOnly = False) Then
                'Add back positions
                objBackMesh.Positions.Add(objPoint3)
                objBackMesh.Positions.Add(objPoint2)
                objBackMesh.Positions.Add(objPoint1)
            End If

            'Read 16 bit number
            Dim intColour As UInt16 = objReader.ReadUInt16

            'First 5 bits are blue
            Dim intBlue As UInt16 = intColour And &H1F

            'Second 5 bits are green
            Dim intGreen As UInt16 = (intColour And &H3E0)/Math.Pow(2, 5)

            'Third 5 bits are red
            Dim intRed As UInt16 = (intColour And &H7C00)/Math.Pow(2, 10)

            '16th bit indicates whether to use colour or not (1=colour defined, 0=no colour defined)
            Dim objColour As Color
            If (intColour And &H8000) = &H8000 Then
                'Set the colour
                objColour = Color.FromRgb(intRed*8, intGreen*8, intBlue*8)
            Else
                objColour = Colors.WhiteSmoke
            End If
        Next
        objReader.Close()

        'Add the visual
        Dim objModel As New GeometryModel3D(objMesh, New DiffuseMaterial(New SolidColorBrush(objFrontColour)))
        Dim objFrontGroup As New Model3DGroup
        objFrontGroup.Children.Add(objModel)
        Dim objVisual As New ModelUIElement3D
        objVisual.Model = objFrontGroup
        objViewer.Children.Add(objVisual)

        Dim objBackVisual As ModelUIElement3D = Nothing
        If (frontOnly = False) Then
            'Add the back visual
            Dim objBackModel As New GeometryModel3D(objBackMesh, New EmissiveMaterial(New SolidColorBrush(objBackColour)))
            Dim objBackGroup As New Model3DGroup
            objBackGroup.Children.Add(objBackModel)
            objBackVisual = New ModelUIElement3D
            objBackVisual.Model = objBackGroup
            objViewer.Children.Add(objBackVisual)
        End If

        Dim strModelName As String = lstModels.NextFreeName(modelFile.NameWithoutExtension)
        lstModels.Add(New Delcam3DModel(strModelName, objVisual, objBackVisual))
    End Sub

#End Region

#Region "DMT File Importer"

    Private Sub ImportDMTFile(modelFile As FileSystem.File,
                              objFrontColour As Color,
                              objBackColour As Color,
                              Optional ByVal frontOnly As Boolean = False)

        Dim objViewer As Viewport3D = GetTemplateChild(PART_Viewport3D)
        If objViewer Is Nothing Then Return

        If (modelFile.Exists = False) Then
            Throw New FileNotFoundException(modelFile.Name & " not found")
        End If

        Dim objDMTModel = DMTModelReader.ReadFile(modelFile)

        Dim objFrontMeshGroup As New Model3DGroup
        Dim objBackMeshGroup As New Model3DGroup

        For Each objBlock As DMTTriangleBlock In objDMTModel.TriangleBlocks
            Dim objMesh As New MeshGeometry3D
            Dim objBackMesh As New MeshGeometry3D

            'Add the positions
            For Each objTriangleVertex As DMTVertex In objBlock.Vertices
                'Add to front mesh
                objMesh.Positions.Add(New Point3D(objTriangleVertex.Position.X,
                                                  objTriangleVertex.Position.Y,
                                                  objTriangleVertex.Position.Z))
                'Add to back mesh
                objBackMesh.Positions.Add(New Point3D(objTriangleVertex.Position.X,
                                                      objTriangleVertex.Position.Y,
                                                      objTriangleVertex.Position.Z))
                'Work out the normal
                If (objBlock.DoVerticesHaveNormals) Then
                    'Calculate the normal
                    Dim normal = objBlock.GetNormal(objTriangleVertex.Position)
                    Dim objVector = New Vector3D(normal.I, normal.J, normal.K)

                    'Add the normal
                    objMesh.Normals.Add(objVector)
                    If (frontOnly = False) Then
                        'Invert and to the back mesh
                        objVector.Negate()
                        objBackMesh.Normals.Add(objVector)
                    End If
                Else
                    Dim normal = objBlock.GetNormal(objTriangleVertex.Position)
                    Dim objVector As New Vector3D(normal.I, normal.J, normal.K)

                    'Add the normal
                    objMesh.Normals.Add(objVector)
                    If (frontOnly = False) Then
                        'Invert and to the back mesh
                        objVector.Negate()
                        objBackMesh.Normals.Add(objVector)
                    End If
                End If
            Next
            'Now do the indices
            For Each objTriangle As DMTTriangle In objBlock.Triangles
                'Add to front mesh
                objMesh.TriangleIndices.Add(objTriangle.Vertex1)
                objMesh.TriangleIndices.Add(objTriangle.Vertex2)
                objMesh.TriangleIndices.Add(objTriangle.Vertex3)
                If (frontOnly = False) Then
                    'Reverse for back mesh
                    objBackMesh.TriangleIndices.Add(objTriangle.Vertex3)
                    objBackMesh.TriangleIndices.Add(objTriangle.Vertex2)
                    objBackMesh.TriangleIndices.Add(objTriangle.Vertex1)
                End If
            Next

            objMesh.Freeze()
            objBackMesh.Freeze()

            Dim objFrontBrush As New SolidColorBrush(objFrontColour)
            objFrontBrush.Freeze()
            Dim objFrontMaterial As New DiffuseMaterial(objFrontBrush)
            objFrontMaterial.Freeze()
            Dim objModel As New GeometryModel3D(objMesh, objFrontMaterial)
            objFrontMeshGroup.Children.Add(objModel)
            If (frontOnly = False) Then
                Dim objBackBrush As New SolidColorBrush(objBackColour)
                objBackBrush.Freeze()
                Dim objBackMaterial As New EmissiveMaterial(objBackBrush)
                objFrontMaterial.Freeze()
                Dim objBackModel As New GeometryModel3D(objBackMesh, objBackMaterial)
                objBackMeshGroup.Children.Add(objBackModel)
            End If
        Next

        'Add the visual
        Dim objVisual As New ModelUIElement3D
        objVisual.Model = objFrontMeshGroup
        objFrontMeshGroup.Freeze()
        objViewer.Children.Add(objVisual)

        Dim objBackVisual As ModelUIElement3D = Nothing
        If (frontOnly = False) Then
            'Add the back visual
            objBackVisual = New ModelUIElement3D()
            objBackVisual.Model = objBackMeshGroup
            objBackMeshGroup.Freeze()
            objViewer.Children.Add(objBackVisual)
        End If

        Dim strModelName As String = lstModels.NextFreeName(modelFile.NameWithoutExtension)
        lstModels.Add(New Delcam3DModel(strModelName, objVisual, objBackVisual))
    End Sub

#End Region

#End Region

#Region "Commands"

    Private Sub AbortActiveCommand()

        Select Case enmActiveCommand
            Case E_ActiveCommand.None
                'Nothing to do
            Case E_ActiveCommand.PointMeasure
                AbortPointMeasure()
            Case E_ActiveCommand.DistanceMeasure
                AbortDistanceMeasure()
            Case E_ActiveCommand.SketchCurve
                AbortSketchCurve()
        End Select
    End Sub

#Region "View Commands"

#Region "Commands"

    Public Shared ReadOnly ViewTopCommand As RoutedCommand = New RoutedCommand("ViewTopCommand", GetType(Delcam3DViewer))
    Public Shared ReadOnly ViewBottomCommand As RoutedCommand = New RoutedCommand("ViewBottomCommand", GetType(Delcam3DViewer))
    Public Shared ReadOnly ViewLeftCommand As RoutedCommand = New RoutedCommand("ViewLeftCommand", GetType(Delcam3DViewer))
    Public Shared ReadOnly ViewRightCommand As RoutedCommand = New RoutedCommand("ViewRightCommand", GetType(Delcam3DViewer))
    Public Shared ReadOnly ViewFrontCommand As RoutedCommand = New RoutedCommand("ViewFrontCommand", GetType(Delcam3DViewer))
    Public Shared ReadOnly ViewBackCommand As RoutedCommand = New RoutedCommand("ViewBackCommand", GetType(Delcam3DViewer))

#End Region

#Region "Operations"

    Private Sub InitialiseViewCommands()

        Dim objViewTop As New CommandBinding(ViewTopCommand, AddressOf ViewTop_Execute)
        Dim objViewBottom As New CommandBinding(ViewBottomCommand, AddressOf ViewBottom_Execute)
        Dim objViewLeft As New CommandBinding(ViewLeftCommand, AddressOf ViewLeft_Execute)
        Dim objViewRight As New CommandBinding(ViewRightCommand, AddressOf ViewRight_Execute)
        Dim objViewFront As New CommandBinding(ViewFrontCommand, AddressOf ViewFront_Execute)
        Dim objViewBack As New CommandBinding(ViewBackCommand, AddressOf ViewBack_Execute)

        Me.CommandBindings.Add(objViewTop)
        Me.CommandBindings.Add(objViewBottom)
        Me.CommandBindings.Add(objViewLeft)
        Me.CommandBindings.Add(objViewRight)
        Me.CommandBindings.Add(objViewFront)
        Me.CommandBindings.Add(objViewBack)
    End Sub

    Private Sub ViewTop_Execute(sender As Object,
                                e As ExecutedRoutedEventArgs)

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            myCamera.Position = New Point3D(0, 50, 400)
            myCamera.Transform = New RotateTransform3D()

            myLight.Transform = New RotateTransform3D()
        End If
    End Sub

    Private Sub ViewBottom_Execute(sender As Object,
                                   e As ExecutedRoutedEventArgs)

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            myCamera.Position = New Point3D(0, 50, 400)
            Dim q As New Quaternion(New Vector3D(0, 1, 0), 180)
            myCamera.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
            myLight.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
        End If
    End Sub

    Private Sub ViewLeft_Execute(sender As Object,
                                 e As ExecutedRoutedEventArgs)

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            myCamera.Position = New Point3D(0, 50, 400)
            Dim q As New Quaternion(New Vector3D(0, 0, 1), - 90)
            Dim r As New Quaternion(New Vector3D(1, 0, 0), 90)
            q *= r
            myCamera.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
            myLight.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
        End If
    End Sub

    Private Sub ViewRight_Execute(sender As Object,
                                  e As ExecutedRoutedEventArgs)

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            myCamera.Position = New Point3D(0, 50, 400)
            Dim q As New Quaternion(New Vector3D(0, 0, 1), 90)
            Dim r As New Quaternion(New Vector3D(1, 0, 0), 90)
            q *= r
            myCamera.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
            myLight.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
        End If
    End Sub

    Private Sub ViewFront_Execute(sender As Object,
                                  e As ExecutedRoutedEventArgs)

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            myCamera.Position = New Point3D(0, 50, 400)
            Dim q As New Quaternion(New Vector3D(1, 0, 0), - 90)
            Dim r As New Quaternion(New Vector3D(0, 0, 1), 180)
            q *= r
            myCamera.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
            myLight.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
        End If
    End Sub

    Private Sub ViewBack_Execute(sender As Object,
                                 e As ExecutedRoutedEventArgs)

        Dim myCamera As OrthographicCamera = GetTemplateChild(PART_Camera)
        Dim myLight As DirectionalLight = GetTemplateChild(PART_Light)

        If (myCamera IsNot Nothing) AndAlso (myLight IsNot Nothing) Then
            myCamera.Position = New Point3D(0, 50, 400)
            Dim q As New Quaternion(New Vector3D(1, 0, 0), 90)
            myCamera.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
            myLight.Transform = New RotateTransform3D(New QuaternionRotation3D(q))
        End If
    End Sub

#End Region

#End Region

#Region "Measure Commands"

#Region "Commands"

    Public Shared ReadOnly _
        PointMeasureCommand As RoutedCommand = New RoutedCommand("PointMeasureCommand", GetType(Delcam3DViewer))

    Public Shared ReadOnly _
        DistanceMeasureCommand As RoutedCommand = New RoutedCommand("DistanceMeasureCommand", GetType(Delcam3DViewer))

#End Region

#Region "Operations"

    Private Sub InitialiseMeasureCommands()

        Dim _
            objPointMeasure As _
                New CommandBinding(PointMeasureCommand, AddressOf PointMeasure_Execute, AddressOf PointMeasure_CanExecute)
        Dim _
            objDistanceMeasure As _
                New CommandBinding(DistanceMeasureCommand,
                                   AddressOf DistanceMeasure_Execute,
                                   AddressOf DistanceMeasure_CanExecute)

        Me.CommandBindings.Add(objPointMeasure)
        Me.CommandBindings.Add(objDistanceMeasure)
    End Sub

    Private Function AddMarker(objPoint As Point3D,
                               objNormal As Vector3D) As Delcam3DEntity

        Dim objMarker As New Delcam3DMarker(objPoint, objNormal)
        Dim objVisual As ModelUIElement3D = objMarker.CreateMarker

        AddHandler objVisual.MouseRightButtonDown, AddressOf DisplayMarkerCommands
        AddHandler objVisual.MouseEnter, AddressOf DisplayTooltip
        AddHandler objVisual.MouseLeave, AddressOf HideTooltip
        CType(GetTemplateChild(PART_Viewport3D), Viewport3D).Children.Add(objVisual)

        Return objMarker
    End Function

    Private Sub DisplayMarkerCommands(sender As Object,
                                      e As MouseEventArgs)

        If enmLeftClickMode <> E_LeftClickMode.None Then Return

        'display tooltip
        Dim objButton As DelcamButton
        If (objMarkerTip.Child Is Nothing) Then
            objMarkerTip.AllowsTransparency = True
            objMarkerTip.Opacity = 0.0
            objMarkerTip.PopupAnimation = PopupAnimation.Fade
            objMarkerTip.Focusable = True
            objButton = New DelcamButton
            objButton.Content = "Reselect point"
            objMarkerTip.Child = objButton
            objMarkerTip.Placement = PlacementMode.Mouse
            AddHandler objButton.Click, AddressOf ReselectPointClicked
        Else
            objButton = objMarkerTip.Child
        End If
        objButton.Tag = sender
        For Each objModel As Delcam3DPointMeasureModel In lstPointMeasureModels
            If (objModel.MarkerModel.FrontVisual Is sender) Then
                objMarkerTip.IsOpen = True
                ReadyHideOfMarkerCommands()
                Return
            End If
        Next
        For Each objModel As Delcam3DDistanceMeasureModel In lstDistanceMeasureModels
            If (objModel.StartModel.FrontVisual Is sender) Then
                objMarkerTip.IsOpen = True
                ReadyHideOfMarkerCommands()
                Return
            ElseIf (objModel.EndModel.FrontVisual Is sender) Then
                objMarkerTip.IsOpen = True
                ReadyHideOfMarkerCommands()
                Return
            End If
        Next
        For Each objModel As Delcam3DPolylineModel In lstCurveModels
            For Each objKeyPoint As Delcam3DMarker In objModel.KeyPoints
                If (objKeyPoint.FrontVisual Is sender) Then
                    objMarkerTip.IsOpen = True
                    ReadyHideOfMarkerCommands()
                End If
            Next
        Next
    End Sub

    Private Sub ReadyHideOfMarkerCommands()

        If (objMarkerTip IsNot Nothing) AndAlso (objMarkerTip.IsOpen) Then
            With objTimer
                .AutoReset = False
                .Interval = 2000
                .Enabled = True
            End With
        End If
    End Sub

    Private Sub ReselectPointClicked(sender As Object,
                                     e As RoutedEventArgs)

        objMarkerTip.IsOpen = False

        For Each objModel As Delcam3DPointMeasureModel In lstPointMeasureModels
            If (objModel.MarkerModel.FrontVisual Is sender.Tag) Then
                objPointMeasureModel = objModel
                enmLeftClickMode = E_LeftClickMode.RemeasurePoint
                Me.Cursor = Cursors.Cross
                Return
            End If
        Next

        For Each objModel As Delcam3DDistanceMeasureModel In lstDistanceMeasureModels
            If (objModel.StartModel.FrontVisual Is sender.Tag) Then
                objDistanceMeasureModel = objModel
                enmLeftClickMode = E_LeftClickMode.RemeasureDistanceStartPoint
                Me.Cursor = Cursors.Cross
                Return
            ElseIf (objModel.EndModel.FrontVisual Is sender.Tag) Then
                objDistanceMeasureModel = objModel
                enmLeftClickMode = E_LeftClickMode.RemeasureDistanceEndPoint
                Me.Cursor = Cursors.Cross
                Return
            End If
        Next

        For Each objModel As Delcam3DPolylineModel In lstCurveModels
            Dim i = 0
            For Each objKeypoint As Delcam3DMarker In objModel.KeyPoints
                If (objKeypoint.FrontVisual Is sender.Tag) Then
                    objCurveSketchModel = objModel
                    intReselectIndex = i
                    enmLeftClickMode = E_LeftClickMode.ReselectCurvePoint
                    Me.Cursor = Cursors.Cross
                    Return
                End If
                i += 1
            Next
        Next
    End Sub

    Private Sub PopupTimerElapsed(sender As Object,
                                  e As ElapsedEventArgs) Handles objTimer.Elapsed

        Dispatcher.Invoke(myHideTooltipDelegate)
    End Sub

    Private myHideTooltipDelegate As HideTooltipDelegate = AddressOf HideTooltip

    Private Delegate Sub HideTooltipDelegate()

    Private Sub HideTooltip()

        'hide tooltip
        If (objMarkerTip IsNot Nothing) Then
            'If the mouse is over the Tooltip then reset the timer
            If (objMarkerTip.IsMouseOver) Then
                objTimer.Enabled = True
            Else
                objMarkerTip.IsOpen = False
            End If
        End If
    End Sub

    Friend Function AddLine(objFirstPoint As Point3D,
                            objSecondPoint As Point3D,
                            colColour As Color,
                            Optional ByVal thickness As Double = 0.1) As Delcam3DEntity

        Dim objLine As New Delcam3DLine(objFirstPoint, objSecondPoint, thickness)
        Dim objVisual As ModelUIElement3D = objLine.CreateLine(colColour)

        AddHandler objVisual.MouseEnter, AddressOf DisplayTooltip
        AddHandler objVisual.MouseLeave, AddressOf HideTooltip
        CType(GetTemplateChild(PART_Viewport3D), Viewport3D).Children.Add(objVisual)

        Return objLine
    End Function

    Private Sub DisplayTooltip(sender As Object,
                               e As MouseEventArgs)

        'display tooltip
        If (objLineTip Is Nothing) Then
            objLineTip = New DelcamTooltip
        End If
        For Each objModel As Delcam3DPointMeasureModel In lstPointMeasureModels
            If (objModel.MarkerModel.FrontVisual Is sender) Then
                objLineTip.Content =
                    String.Format("Position:" & vbNewLine & "     X={0}" & vbNewLine & "     Y={1}" & vbNewLine & "     Z={2}",
                                  Format(objModel.MarkerModel.Position.X, "#0.##"),
                                  Format(objModel.MarkerModel.Position.Y, "#0.##"),
                                  Format(objModel.MarkerModel.Position.Z, "#0.##"))
                objLineTip.IsOpen = True
                Return
            End If
        Next
        For Each objModel As Delcam3DDistanceMeasureModel In lstDistanceMeasureModels
            If (objModel.StartModel.FrontVisual Is sender) Then
                objLineTip.Content =
                    String.Format("Position:" & vbNewLine & "     X={0}" & vbNewLine & "     Y={1}" & vbNewLine & "     Z={2}",
                                  Format(objModel.StartModel.Position.X, "#0.##"),
                                  Format(objModel.StartModel.Position.Y, "#0.##"),
                                  Format(objModel.StartModel.Position.Z, "#0.##"))
                objLineTip.IsOpen = True
                Return
            ElseIf (objModel.EndModel.FrontVisual Is sender) Then
                objLineTip.Content =
                    String.Format("Position:" & vbNewLine & "     X={0}" & vbNewLine & "     Y={1}" & vbNewLine & "     Z={2}",
                                  Format(objModel.EndModel.Position.X, "#0.##"),
                                  Format(objModel.EndModel.Position.Y, "#0.##"),
                                  Format(objModel.EndModel.Position.Z, "#0.##"))
                objLineTip.IsOpen = True
                Return
            End If
            If (objModel.LineModel.FrontVisual Is sender) Then
                objLineTip.Content =
                    String.Format(
                        "Length:" & vbNewLine & "     {0} mm" & vbNewLine & vbNewLine & "Delta:" & vbNewLine & "     dX={1}" &
                        vbNewLine & "     dY={2}" & vbNewLine & "     dZ={3}",
                        Format(objModel.Distance, "#0.##"),
                        Format(objModel.EndModel.Position.X - objModel.StartModel.Position.X, "#0.##"),
                        Format(objModel.EndModel.Position.Y - objModel.StartModel.Position.Y, "#0.##"),
                        Format(objModel.EndModel.Position.Z - objModel.StartModel.Position.Z, "#0.##"))
                objLineTip.IsOpen = True
                Return
            End If
        Next
    End Sub

    Private Sub HideToolTip(sender As Object,
                            e As MouseEventArgs)

        'hide tooltip
        If (objLineTip IsNot Nothing) Then
            objLineTip.IsOpen = False
        End If
    End Sub

#End Region

#Region "Point Measure"

#Region "Events"

    Public Event PointMeasured(sender As Object, e As Delcam3DPointMeasuredEventArgs)

#End Region

#Region "Attributes"

    Private objPointMeasureModel As Delcam3DPointMeasureModel

#End Region

#Region "Operations"

    Private Sub PointMeasure_Execute(sender As Object,
                                     e As ExecutedRoutedEventArgs)

        AbortActiveCommand()

        Dim strModelName = e.Parameter

        lstPointMeasureModels.Remove(strModelName)

        objPointMeasureModel = New Delcam3DPointMeasureModel(strModelName)
        enmLeftClickMode = E_LeftClickMode.MeasurePoint
        Me.Cursor = Cursors.Cross
    End Sub

    Private Sub PointMeasure_CanExecute(sender As Object,
                                        e As CanExecuteRoutedEventArgs)

        e.CanExecute = lstModels.Count > 0
    End Sub

    Private Sub AbortPointMeasure()
    End Sub

    Private Sub PointMeasure_MouseLeftButtonDown(sender As Object,
                                                 e As MouseButtonEventArgs)

        Select Case enmLeftClickMode
            Case E_LeftClickMode.MeasurePoint
                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf PointMeasureHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))

                'Add the measured model
                lstPointMeasureModels.Add(objPointMeasureModel)

                'Raise the DistanceMeasured event
                RaiseEvent PointMeasured(Me, New Delcam3DPointMeasuredEventArgs(objPointMeasureModel))
            Case Else
                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf PointMeasureHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))

                'Raise the DistanceMeasured event
                RaiseEvent PointMeasured(Me, New Delcam3DPointMeasuredEventArgs(objPointMeasureModel))
        End Select
    End Sub

    Public Function PointMeasureHitTest(rawresult As HitTestResult) As HitTestResultBehavior

        Dim rayResult As RayHitTestResult = rawresult

        If (rayResult IsNot Nothing) Then
            Dim rayMeshResult As RayMeshGeometry3DHitTestResult = rayResult
            If rayMeshResult IsNot Nothing Then
                For Each objModel As Delcam3DModel In lstModels
                    If TypeOf objModel.FrontVisual Is ModelUIElement3D Then
                        If _
                            CType(CType(objModel.FrontVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                rayMeshResult.ModelHit) OrElse
                            ((objModel.BackVisual IsNot Nothing) AndAlso
                             CType(CType(objModel.BackVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                 rayMeshResult.ModelHit)) Then
                            Dim objPoint As Point3D = rayMeshResult.PointHit
                            Dim objNormal As Vector3D = rayMeshResult.MeshHit.Normals(rayMeshResult.VertexIndex1)
                            If enmLeftClickMode = E_LeftClickMode.RemeasurePoint Then
                                Dim objViewport As Viewport3D = GetTemplateChild(PART_Viewport3D)
                                objPointMeasureModel.MarkerModel.DeleteFromViewport(objViewport)
                            End If
                            objPointMeasureModel.MarkerModel = AddMarker(objPoint, objNormal)
                            enmLeftClickMode = E_LeftClickMode.None
                            Me.Cursor = Cursors.Arrow
                            Return HitTestResultBehavior.Stop
                        End If
                    End If
                Next
            End If
        End If

        Return HitTestResultBehavior.Continue
    End Function

#End Region

#End Region

#Region "Distance Measure"

#Region "Events"

    Public Event DistanceMeasured(sender As Object, e As Delcam3DDistanceMeasuredEventArgs)

#End Region

#Region "Attributes"

    Private objDistanceMeasureModel As Delcam3DDistanceMeasureModel

    Private objMarkerTip As New Popup
    Private WithEvents objTimer As New Timer
    Private objLineTip As DelcamTooltip

#End Region

#Region "Operations"

    Private Sub DistanceMeasure_Execute(sender As Object,
                                        e As ExecutedRoutedEventArgs)

        AbortActiveCommand()

        Dim strModelName = e.Parameter

        lstDistanceMeasureModels.Remove(strModelName)

        objDistanceMeasureModel = New Delcam3DDistanceMeasureModel(strModelName)
        enmLeftClickMode = E_LeftClickMode.MeasureDistanceStartPoint
        Me.Cursor = Cursors.Cross
    End Sub

    Private Sub DistanceMeasure_CanExecute(sender As Object,
                                           e As CanExecuteRoutedEventArgs)

        e.CanExecute = lstModels.Count > 0
    End Sub

    Private Sub AbortDistanceMeasure()
    End Sub

    Private Sub DistanceMeasure_MouseLeftButtonDown(sender As Object,
                                                    e As MouseButtonEventArgs)

        Select Case enmLeftClickMode
            Case E_LeftClickMode.MeasureDistanceStartPoint

                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf DistanceMeasureHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))

            Case E_LeftClickMode.MeasureDistanceEndPoint

                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf DistanceMeasureHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))

                'Add the measured model
                lstDistanceMeasureModels.Add(objDistanceMeasureModel)

                'Raise the DistanceMeasured event
                RaiseEvent DistanceMeasured(Me, New Delcam3DDistanceMeasuredEventArgs(objDistanceMeasureModel))

            Case E_LeftClickMode.RemeasureDistanceStartPoint

                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf DistanceMeasureHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))

                'Raise the DistanceMeasured event
                RaiseEvent DistanceMeasured(Me, New Delcam3DDistanceMeasuredEventArgs(objDistanceMeasureModel))

            Case E_LeftClickMode.RemeasureDistanceEndPoint

                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf DistanceMeasureHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))

                'Raise the DistanceMeasured event
                RaiseEvent DistanceMeasured(Me, New Delcam3DDistanceMeasuredEventArgs(objDistanceMeasureModel))

        End Select
    End Sub

    Public Function DistanceMeasureHitTest(rawresult As HitTestResult) As HitTestResultBehavior

        Dim rayResult As RayHitTestResult = rawresult

        If (rayResult IsNot Nothing) Then
            Dim rayMeshResult As RayMeshGeometry3DHitTestResult = rayResult
            If rayMeshResult IsNot Nothing Then
                For Each objModel As Delcam3DModel In lstModels
                    If TypeOf objModel.FrontVisual Is ModelUIElement3D Then
                        If _
                            CType(CType(objModel.FrontVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                rayMeshResult.ModelHit) OrElse
                            ((objModel.BackVisual IsNot Nothing) AndAlso
                             CType(CType(objModel.BackVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                 rayMeshResult.ModelHit)) Then
                            Select Case enmLeftClickMode
                                Case E_LeftClickMode.MeasureDistanceStartPoint
                                    Dim objPoint As Point3D = rayMeshResult.PointHit
                                    Dim objNormal As Vector3D = rayMeshResult.MeshHit.Normals(rayMeshResult.VertexIndex1)
                                    objDistanceMeasureModel.StartModel = AddMarker(objPoint, objNormal)
                                    enmLeftClickMode = E_LeftClickMode.MeasureDistanceEndPoint
                                Case E_LeftClickMode.MeasureDistanceEndPoint
                                    Dim objPoint As Point3D = rayMeshResult.PointHit
                                    Dim objNormal As Vector3D = rayMeshResult.MeshHit.Normals(rayMeshResult.VertexIndex1)
                                    objDistanceMeasureModel.EndModel = AddMarker(objPoint, objNormal)
                                    objDistanceMeasureModel.LineModel = AddLine(objDistanceMeasureModel.StartModel.Position,
                                                                                objPoint,
                                                                                Colors.Red)
                                    enmLeftClickMode = E_LeftClickMode.None
                                    Me.Cursor = Cursors.Arrow
                                Case E_LeftClickMode.RemeasureDistanceStartPoint
                                    Dim objPoint As Point3D = rayMeshResult.PointHit
                                    Dim objNormal As Vector3D = rayMeshResult.MeshHit.Normals(rayMeshResult.VertexIndex1)
                                    'Remove the last marker and add a new one
                                    Dim objViewport As Viewport3D = GetTemplateChild(PART_Viewport3D)
                                    objDistanceMeasureModel.StartModel.DeleteFromViewport(objViewport)
                                    objDistanceMeasureModel.StartModel = AddMarker(objPoint, objNormal)
                                    'Remove the line and add a new one
                                    objDistanceMeasureModel.LineModel.DeleteFromViewport(objViewport)
                                    objDistanceMeasureModel.LineModel = AddLine(objPoint,
                                                                                objDistanceMeasureModel.EndModel.Position,
                                                                                Colors.Red)
                                    enmLeftClickMode = E_LeftClickMode.None
                                    Me.Cursor = Cursors.Arrow
                                Case E_LeftClickMode.RemeasureDistanceEndPoint
                                    Dim objPoint As Point3D = rayMeshResult.PointHit
                                    Dim objNormal As Vector3D = rayMeshResult.MeshHit.Normals(rayMeshResult.VertexIndex1)
                                    'Remove the last marker and add a new one
                                    Dim objViewport As Viewport3D = GetTemplateChild(PART_Viewport3D)
                                    objDistanceMeasureModel.EndModel.DeleteFromViewport(objViewport)
                                    objDistanceMeasureModel.EndModel = AddMarker(objPoint, objNormal)
                                    'Remove the line and add a new one
                                    objDistanceMeasureModel.LineModel.DeleteFromViewport(objViewport)
                                    objDistanceMeasureModel.LineModel = AddLine(objDistanceMeasureModel.StartModel.Position,
                                                                                objPoint,
                                                                                Colors.Red)
                                    enmLeftClickMode = E_LeftClickMode.None
                                    Me.Cursor = Cursors.Arrow
                            End Select
                            Return HitTestResultBehavior.Stop
                        End If
                    End If
                Next
            End If
        End If

        Return HitTestResultBehavior.Continue
    End Function

#End Region

#End Region

#End Region

#Region "Sketch Curve Commands"

#Region "Commands"

    Public Shared ReadOnly SketchCurveCommand As RoutedCommand = New RoutedCommand("SketchCurveCommand", GetType(Delcam3DViewer))

#End Region

#Region "Attributes"

    Private objCurveSketchModel As Delcam3DPolylineModel
    Private intReselectIndex As Integer

#End Region

#Region "Operations"

    Private Sub InitialiseSketchCurveCommands()

        Dim _
            objSketchCurve As _
                New CommandBinding(SketchCurveCommand, AddressOf SketchCurve_Execute, AddressOf SketchCurve_CanExecute)

        Me.CommandBindings.Add(objSketchCurve)
    End Sub

    Private Sub SketchCurve_Execute(sender As Object,
                                    e As ExecutedRoutedEventArgs)

        Dim strModelName = e.Parameter

        lstCurveModels.Remove(strModelName)

        objCurveSketchModel = New Delcam3DPolylineModel(strModelName)
        enmLeftClickMode = E_LeftClickMode.SketchCurve
        Me.Cursor = Cursors.Cross
    End Sub

    Private Sub SketchCurve_CanExecute(sender As Object,
                                       e As CanExecuteRoutedEventArgs)

        e.CanExecute = lstModels.Count > 0
    End Sub

    Private Sub AbortSketchCurve()
    End Sub

    Private Sub SketchCurve_MouseLeftButtonDown(sender As Object,
                                                e As MouseButtonEventArgs)

        Select Case enmLeftClickMode
            Case E_LeftClickMode.SketchCurve
                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf SketchCurveHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))
            Case E_LeftClickMode.ReselectCurvePoint
                VisualTreeHelper.HitTest(CType(GetTemplateChild(PART_Viewport3D), Viewport3D),
                                         Nothing,
                                         AddressOf ReselectCurvePointHitTest,
                                         New PointHitTestParameters(e.GetPosition(Me)))
        End Select
    End Sub

    Public Function SketchCurveHitTest(rawresult As HitTestResult) As HitTestResultBehavior

        Dim rayResult As RayHitTestResult = rawresult
        Dim blnPointAdded = False

        If (rayResult IsNot Nothing) Then
            Dim rayMeshResult As RayMeshGeometry3DHitTestResult = rayResult
            Dim blnDone = False
            If rayMeshResult IsNot Nothing Then
                For Each objModel As Delcam3DModel In lstModels
                    If TypeOf objModel.FrontVisual Is ModelUIElement3D Then
                        If _
                            CType(CType(objModel.FrontVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                rayMeshResult.ModelHit) OrElse
                            ((objModel.BackVisual IsNot Nothing) AndAlso
                             CType(CType(objModel.BackVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                 rayMeshResult.ModelHit)) Then
                            blnPointAdded = True
                            If ((objCurveSketchModel.KeyPoints.Count > 1) AndAlso
                                (Math.Abs(
                                    (rayMeshResult.PointHit -
                                     objCurveSketchModel.KeyPoints(objCurveSketchModel.KeyPoints.Count - 1).Position).Length) < 1)) _
                                Then
                                blnDone = True
                            ElseIf ((objCurveSketchModel.KeyPoints.Count > 1) AndAlso
                                    (Math.Abs((rayMeshResult.PointHit - objCurveSketchModel.KeyPoints(0).Position).Length) < 1)) _
                                Then
                                blnDone = True
                                objCurveSketchModel.IsClosed = True
                            Else
                                objCurveSketchModel.KeyPoints.Add(AddMarker(rayMeshResult.PointHit,
                                                                            rayMeshResult.MeshHit.Normals(
                                                                                rayMeshResult.VertexIndex1)))
                            End If
                        End If
                    End If
                Next
            End If

            If (blnDone) Then

                Me.Cursor = Cursors.Wait
                CreateCurve(rayMeshResult)
                lstCurveModels.Add(objCurveSketchModel)

            Else

                'Add a line
                If (objCurveSketchModel.KeyPoints.Count > 1) Then
                    objCurveSketchModel.Lines.Add(
                        AddLine(objCurveSketchModel.KeyPoints(objCurveSketchModel.KeyPoints.Count - 2).Position,
                                objCurveSketchModel.KeyPoints(objCurveSketchModel.KeyPoints.Count - 1).Position,
                                Colors.Red))
                End If

            End If

        End If

        If (blnPointAdded) Then
            Return HitTestResultBehavior.Stop
        Else
            Return HitTestResultBehavior.Continue
        End If
    End Function

    Private Sub CreateCurve(rayMeshResult As RayMeshGeometry3DHitTestResult)

        'Remove all the previous lines
        Dim objViewport As Viewport3D = GetTemplateChild(PART_Viewport3D)
        For Each objLine As Delcam3DLine In objCurveSketchModel.Lines
            objLine.DeleteFromViewport(objViewport)
        Next

        Dim objPolyline As New Polyline
        For Each objMarker As Delcam3DMarker In objCurveSketchModel.KeyPoints
            objPolyline.Add(New Point(objMarker.Position.X, objMarker.Position.Y, objMarker.Position.Z))
        Next
        'If it is closed then add the first point at the end
        If (objCurveSketchModel.IsClosed) Then
            objPolyline.Add(objPolyline.Item(0))
        End If
        Dim objCurve As New Spline(objPolyline)
        objPolyline = New Polyline(objCurve, 0.1)

        Dim lstZones As New List(Of List(Of List(Of List(Of Integer))))
        For x = 0 To 10
            lstZones.Add(New List(Of List(Of List(Of Integer))))
            For y = 0 To 10
                lstZones(x).Add(New List(Of List(Of Integer)))
                For z = 0 To 10
                    lstZones(x)(y).Add(New List(Of Integer))
                Next
            Next
        Next
        Dim objModel As Delcam3DModel = Nothing
        For Each objPotentialModel As Delcam3DModel In lstModels
            If (objPotentialModel.FrontVisual Is rayMeshResult.VisualHit) Then
                objModel = objPotentialModel
                Exit For
            End If
        Next

        Dim intPointIndex = 0
        For Each objPoint As Point3D In rayMeshResult.MeshHit.Positions
            Dim x As Integer =
                    Math.Floor(
                        ((objPoint.X - objModel.BoundingBox.MinX)/(objModel.BoundingBox.MaxX - objModel.BoundingBox.MinX))*10)
            Dim y As Integer =
                    Math.Floor(
                        ((objPoint.Y - objModel.BoundingBox.MinY)/(objModel.BoundingBox.MaxY - objModel.BoundingBox.MinY))*10)
            Dim z As Integer =
                    Math.Floor(
                        ((objPoint.Z - objModel.BoundingBox.MinZ)/(objModel.BoundingBox.MaxZ - objModel.BoundingBox.MinZ))*10)
            lstZones(x)(y)(z).Add(intPointIndex)
            intPointIndex += 1
        Next

        For i = 0 To objPolyline.Count - 1
            Dim objPoint As Point3D = ProjectToSurface(
                New Point3D(objPolyline.Item(i).X, objPolyline.Item(i).Y, objPolyline.Item(i).Z),
                rayMeshResult.MeshHit.Positions,
                lstZones,
                objModel)
            objPolyline.Item(i) = New Point(objPoint.X, objPoint.Y, objPoint.Z)
        Next

        'Project curve to surface
        For i As Integer = objPolyline.Count - 2 To 0 Step - 1
            ProjectCurveToSurface(objPolyline,
                                  i,
                                  i + 1,
                                  rayMeshResult.MeshHit,
                                  rayMeshResult.MeshHit.Positions,
                                  lstZones,
                                  objModel)
        Next

        'Now draw the lines
        For i = 0 To objPolyline.Count - 2
            objCurveSketchModel.Lines.Add(AddLine(New Point3D(objPolyline.Item(i).X, objPolyline.Item(i).Y, objPolyline.Item(i).Z),
                                                  New Point3D(objPolyline.Item(i + 1).X,
                                                              objPolyline.Item(i + 1).Y,
                                                              objPolyline.Item(i + 1).Z),
                                                  Colors.PowderBlue))
        Next

        enmActiveCommand = E_ActiveCommand.None
        enmLeftClickMode = E_LeftClickMode.None
        Me.Cursor = Cursors.Arrow
    End Sub

    Private Sub ProjectCurveToSurface(objPolyline As Polyline,
                                      intFirstIndex As Integer,
                                      intSecondIndex As Integer,
                                      objMesh As MeshGeometry3D,
                                      lstPoints As Point3DCollection,
                                      lstZones As List(Of List(Of List(Of List(Of Integer)))),
                                      objModel As Delcam3DModel)

        Dim dblTolerance = 0.1

        'Put the first point onto the surface
        Dim objNearestPoint As Point3D
        'Dim objPoint As Point3D
        'With objPolyline.Points(intFirstIndex)
        'objPoint = New Point3D(.X, .Y, .Z)
        'End With

        'objNearestPoint = ProjectToSurface(objPoint, lstPoints, lstZones, objModel)

        'objPolyline.Points(intFirstIndex) = New Geometry.Point(objNearestPoint.X, objNearestPoint.Y, objNearestPoint.Z)

        'Find the midpoint
        Dim objVector As Vector = objPolyline.Item(intSecondIndex) - objPolyline.Item(intFirstIndex)
        objVector *= 0.5
        Dim dblMagnitude As Double = objVector.Magnitude
        If (dblMagnitude <= dblTolerance) Then
            Return
        End If

        Dim objMidpoint As Point = CType(objPolyline.Item(intFirstIndex), Point) + objVector

        'How far from the surface is the midpoint?
        Dim objMid As New Point3D(objMidpoint.X, objMidpoint.Y, objMidpoint.Z)

        'Refine list of near triangles
        'Dim lstNearPoints As New Point3DCollection
        'For i As Integer = 0 To lstPoints.Count - 1 Step 3
        '    Dim objNode1 As Point3D = lstPoints(i)
        '    Dim objNode2 As Point3D = lstPoints(i + 1)
        '    Dim objNode3 As Point3D = lstPoints(i + 2)
        '    If (Math.Abs((objMid - objNode1).Length) <= dblMagnitude) Then
        '        lstNearPoints.Add(objNode1)
        '        lstNearPoints.Add(objNode2)
        '        lstNearPoints.Add(objNode3)
        '    ElseIf (Math.Abs((objMid - objNode2).Length) <= dblMagnitude) Then
        '        lstNearPoints.Add(objNode1)
        '        lstNearPoints.Add(objNode2)
        '        lstNearPoints.Add(objNode3)
        '    ElseIf (Math.Abs((objMid - objNode3).Length) <= dblMagnitude) Then
        '        lstNearPoints.Add(objNode1)
        '        lstNearPoints.Add(objNode2)
        '        lstNearPoints.Add(objNode3)
        '    End If
        'Next

        objNearestPoint = ProjectToSurface(objMid, lstPoints, lstZones, objModel)

        If (objNearestPoint = Nothing) Then Return
        Dim dblDistance As Double = Math.Abs((objMid - objNearestPoint).Length)

        'Is the nearest point the start or end point?
        If _
            ((Math.Abs(
                (objNearestPoint -
                 New Point3D(objPolyline.Item(intFirstIndex).X,
                             objPolyline.Item(intFirstIndex).Y,
                             objPolyline.Item(intFirstIndex).Z)).Length) < dblTolerance) OrElse
             (Math.Abs(
                 (objNearestPoint -
                  New Point3D(objPolyline.Item(intSecondIndex).X,
                              objPolyline.Item(intSecondIndex).Y,
                              objPolyline.Item(intSecondIndex).Z)).Length) < dblTolerance)) Then
            Return
        End If

        If (dblDistance > dblTolerance) Then
            'insert the midpoint into the polyline
            objMidpoint = New Point(objNearestPoint.X, objNearestPoint.Y, objNearestPoint.Z)
            objPolyline.Insert(intSecondIndex, objMidpoint)
            ProjectCurveToSurface(objPolyline, intSecondIndex, intSecondIndex + 1, objMesh, lstPoints, lstZones, objModel)
            ProjectCurveToSurface(objPolyline, intFirstIndex, intSecondIndex, objMesh, lstPoints, lstZones, objModel)
        End If
    End Sub

    Private Function ProjectToSurface(objProjectedPoint As Point3D,
                                      lstPoints As Point3DCollection,
                                      lstZones As List(Of List(Of List(Of List(Of Integer)))),
                                      objModel As Delcam3DModel) As Point3D

        'Find out which zone the projected point is in
        Dim x As Integer =
                Math.Floor(
                    ((objProjectedPoint.X - objModel.BoundingBox.MinX)/(objModel.BoundingBox.MaxX - objModel.BoundingBox.MinX))*10)
        Dim y As Integer =
                Math.Floor(
                    ((objProjectedPoint.Y - objModel.BoundingBox.MinY)/(objModel.BoundingBox.MaxY - objModel.BoundingBox.MinY))*10)
        Dim z As Integer =
                Math.Floor(
                    ((objProjectedPoint.Z - objModel.BoundingBox.MinZ)/(objModel.BoundingBox.MaxZ - objModel.BoundingBox.MinZ))*10)

        'If it is outside the model then limit it to the maximum/minimum zone
        If (x < 0) Then x = 0
        If (x > 10) Then x = 10
        If (y < 0) Then y = 0
        If (y > 10) Then y = 10
        If (z < 0) Then z = 0
        If (z > 10) Then z = 10

        Return NearestPoint(objProjectedPoint, lstPoints, lstZones, x, y, z)
    End Function

    Private Function NearestPoint(objProjectedPoint As Point3D,
                                  lstPoints As Point3DCollection,
                                  lstZones As List(Of List(Of List(Of List(Of Integer)))),
                                  x As Integer,
                                  y As Integer,
                                  z As Integer)

        Dim objResultPoint As New Point3D(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity)

        Dim lstZonePoints As List(Of Integer) = lstZones(x)(y)(z)
        'Are there any points in this zone?
        If (lstZonePoints.Count > 0) Then
            'Yes. So find the nearest point in this zone.
            objResultPoint = NearestPointInZone(objProjectedPoint, lstPoints, lstZonePoints)
            'Then check the 6 surrounding zones in case there is a closer one (may need to check more)
            If (x > 0) Then
                Dim objCandidatePoint As Point3D = NearestPointInZone(objProjectedPoint, lstPoints, lstZones(x - 1)(y)(z))
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            End If
            If (x < 10) Then
                Dim objCandidatePoint As Point3D = NearestPointInZone(objProjectedPoint, lstPoints, lstZones(x + 1)(y)(z))
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            End If
            If (y > 0) Then
                Dim objCandidatePoint As Point3D = NearestPointInZone(objProjectedPoint, lstPoints, lstZones(x)(y - 1)(z))
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            End If
            If (y < 10) Then
                Dim objCandidatePoint As Point3D = NearestPointInZone(objProjectedPoint, lstPoints, lstZones(x)(y + 1)(z))
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            End If
            If (z > 0) Then
                Dim objCandidatePoint As Point3D = NearestPointInZone(objProjectedPoint, lstPoints, lstZones(x)(y)(z - 1))
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            End If
            If (z < 10) Then
                Dim objCandidatePoint As Point3D = NearestPointInZone(objProjectedPoint, lstPoints, lstZones(x)(y)(z + 1))
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            End If
        Else
            'Find the nearest zones (there may be more than one) with points in them
            Dim dblDistance As Double = Double.MaxValue
            'Work out the distance to the nearest zone (in steps)
            For i = 0 To 10
                For j = 0 To 10
                    For k = 0 To 10
                        If (lstZones(i)(j)(k).Count > 0) Then
                            Dim dblThisDistance As Double = Math.Abs(x - i) + Math.Abs(y - j) + Math.Abs(z - k)
                            If dblThisDistance < dblDistance Then
                                dblDistance = dblThisDistance
                            End If
                        End If
                    Next
                Next
            Next
            'Check the zones
            For i = 0 To 10
                For j = 0 To 10
                    For k = 0 To 10
                        If (lstZones(i)(j)(k).Count > 0) Then
                            Dim dblThisDistance As Double = Math.Abs(x - i) + Math.Abs(y - j) + Math.Abs(z - k)
                            If dblThisDistance = dblDistance Then
                                Dim objCandidatePoint As Point3D = NearestPoint(objProjectedPoint, lstPoints, lstZones, i, j, k)
                                If _
                                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                                    objResultPoint = objCandidatePoint
                                End If
                            End If
                        End If
                    Next
                Next
            Next

            'Dim lstChecked As New List(Of List(Of List(Of Boolean)))
            'For i As Integer = 0 To 10
            '    lstChecked.Add(New List(Of List(Of Boolean)))
            '    For j As Integer = 0 To 10
            '        lstChecked(i).Add(New List(Of Boolean))
            '        For k As Integer = 0 To 10
            '            lstChecked(i)(j).Add(False)
            '        Next
            '    Next
            'Next
            'lstZonePoints = NearestPopulatedZone(lstZones, x, y, z, lstChecked)
        End If

        Return objResultPoint
    End Function

    Private Function NearestPointInZone(objProjectedPoint As Point3D,
                                        lstPoints As Point3DCollection,
                                        lstZonePoints As List(Of Integer))

        Dim objResultPoint As New Point3D(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity)

        For i = 0 To lstZonePoints.Count - 1
            'If (objMesh.Positions(i) = objNearestVertex) Then
            'ElseIf (objMesh.Positions(i + 1) = objNearestVertex) Then
            'ElseIf (objMesh.Positions(i + 2) = objNearestVertex) Then
            'Else
            '    Continue For
            'End If
            Dim intIndex As Integer = lstZonePoints(i)
            Dim objNode1 As Point3D
            Dim objNode2 As Point3D
            Dim objNode3 As Point3D

            Select Case (intIndex Mod 3)
                Case 0
                    objNode1 = lstPoints(intIndex)
                    objNode2 = lstPoints(intIndex + 1)
                    objNode3 = lstPoints(intIndex + 2)
                Case 1
                    objNode1 = lstPoints(intIndex - 1)
                    objNode2 = lstPoints(intIndex)
                    objNode3 = lstPoints(intIndex + 1)
                Case 2
                    objNode1 = lstPoints(intIndex - 2)
                    objNode2 = lstPoints(intIndex - 1)
                    objNode3 = lstPoints(intIndex)
            End Select

            Dim vecNode1To2 As Vector3D = objNode2 - objNode1
            Dim vecNode2To1 As Vector3D = objNode1 - objNode2
            Dim vecNode1To3 As Vector3D = objNode3 - objNode1
            Dim vecNode3To1 As Vector3D = objNode1 - objNode3
            Dim vecNode2To3 As Vector3D = objNode3 - objNode2
            Dim vecNode3To2 As Vector3D = objNode2 - objNode3
            Dim vecNode1ToProjection As Vector3D = objProjectedPoint - objNode1
            Dim vecNode2ToProjection As Vector3D = objProjectedPoint - objNode2
            Dim vecNode3ToProjection As Vector3D = objProjectedPoint - objNode3

            Dim objProjectionVector As Vector3D = Vector3D.CrossProduct(vecNode1To2, vecNode1To3)
            objProjectionVector.Normalize()

            'This sign will tell us where the triangle is in relation to the projection point.
            'As we look to see where the triangle is in relation to the point we should always move
            'in the same direction in the axis between the point and each node.  If one is different
            'then one of the nodes is outside the triangle
            Dim intSign = 0
            Dim objToProjectionPoint As Vector3D
            Dim dblDotProduct As Double
            Dim blnAboveTriangle = True

            'Firstly get the edge from Node1 to Node2 and normalize it
            Dim objEdge1 As Vector3D = vecNode1To2
            objEdge1.Normalize()
            'Find the cross product of the triangle normal and the edge
            Dim objEdge1Norm As Vector3D = Vector3D.CrossProduct(objEdge1, objProjectionVector)
            'Now get the vector from Node2 to the projection point
            objToProjectionPoint = vecNode2ToProjection
            'Now calculate how far the edge is from the point where the projected point crosses the norm
            dblDotProduct = Vector3D.DotProduct(objToProjectionPoint, objEdge1Norm)
            'Work out which direction in that projected norm the projected point crosses
            If (dblDotProduct < 0) Then intSign = - 1
            If (dblDotProduct > 0) Then intSign = 1

            'Get the edge from Node2 to Node3
            Dim objEdge2 As Vector3D = vecNode2To3
            objEdge2.Normalize()
            'Find the cross product of the triangle normal and the edge
            Dim objEdge2Norm As Vector3D = Vector3D.CrossProduct(objEdge2, objProjectionVector)
            'Now get the vector from Node3 to the projection point
            objToProjectionPoint = vecNode3ToProjection
            'Now calculate how far the edge is from the point where the projected point crosses the norm
            dblDotProduct = Vector3D.DotProduct(objToProjectionPoint, objEdge2Norm)
            'Work out if the point is in the same direction in the axis as the last point
            If (intSign <> 0) Then
                If dblDotProduct < 0 And intSign > 0 Then blnAboveTriangle = False
                If dblDotProduct > 0 And intSign < 0 Then blnAboveTriangle = False
            Else
                If dblDotProduct < 0 Then intSign = - 1
                If dblDotProduct > 0 Then intSign = 1
            End If

            'Get the edge from Node3 to Node1
            Dim objEdge3 As Vector3D = vecNode3To1
            objEdge3.Normalize()
            'Find the cross product of the triangle normal and the edge
            Dim objEdge3Norm As Vector3D = Vector3D.CrossProduct(objEdge3, objProjectionVector)
            'Now get the vector from Node1 to the projection point
            objToProjectionPoint = vecNode1ToProjection
            'Now calculate how far the edge is from the point where the projected point crosses the norm
            dblDotProduct = Vector3D.DotProduct(objToProjectionPoint, objEdge3Norm)
            'Work out if the point is in the same direction in the axis as the last point
            If (intSign <> 0) Then
                If dblDotProduct < 0 And intSign > 0 Then blnAboveTriangle = False
                If dblDotProduct > 0 And intSign < 0 Then blnAboveTriangle = False
            Else
                If dblDotProduct < 0 Then intSign = - 1
                If dblDotProduct > 0 Then intSign = 1
            End If

            If (blnAboveTriangle) Then
                Dim objNorm As Vector3D = Vector3D.CrossProduct(objEdge1, objEdge2)
                objNorm.Normalize()

                Dim objCandidatePoint As Point3D = LineIntersectPlane(objProjectedPoint, objProjectionVector, objNode1, objNorm)
                If _
                    Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                    Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                    objResultPoint = objCandidatePoint
                End If
            Else
                Dim objCandidatePoint As Point3D
                Dim blnAlongAnEdge = False
                If AngleBetween(vecNode1To2, vecNode1ToProjection) <= 90 AndAlso
                   AngleBetween(vecNode2To1, vecNode2ToProjection) <= 90 Then
                    objCandidatePoint = objNode1 + (Vector3D.DotProduct(vecNode1ToProjection, objEdge1)*objEdge1)
                    If _
                        Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                        Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                        objResultPoint = objCandidatePoint
                    End If
                    blnAlongAnEdge = True
                End If
                If AngleBetween(vecNode2To3, vecNode2ToProjection) <= 90 AndAlso
                   AngleBetween(vecNode3To2, vecNode3ToProjection) <= 90 Then
                    objCandidatePoint = objNode2 + (Vector3D.DotProduct(vecNode2ToProjection, objEdge2)*objEdge2)
                    If _
                        Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                        Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                        objResultPoint = objCandidatePoint
                    End If
                    blnAlongAnEdge = True
                End If
                If AngleBetween(vecNode3To1, vecNode3ToProjection) <= 90 AndAlso
                   AngleBetween(vecNode1To3, vecNode1ToProjection) <= 90 Then
                    objCandidatePoint = objNode3 + (Vector3D.DotProduct(vecNode3ToProjection, objEdge3)*objEdge3)
                    If _
                        Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                        Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                        objResultPoint = objCandidatePoint
                    End If
                    blnAlongAnEdge = True
                End If
                If (blnAlongAnEdge = False) Then
                    If (Math.Abs((vecNode1ToProjection).Length) < Math.Abs((vecNode2ToProjection).Length) AndAlso
                        Math.Abs((vecNode1ToProjection).Length) < Math.Abs((vecNode3ToProjection).Length)) Then
                        objCandidatePoint = objNode1
                    ElseIf (Math.Abs((vecNode2ToProjection).Length) < Math.Abs((vecNode1ToProjection).Length) AndAlso
                            Math.Abs((vecNode2ToProjection).Length) < Math.Abs((vecNode3ToProjection).Length)) Then
                        objCandidatePoint = objNode2
                    Else
                        objCandidatePoint = objNode3
                    End If
                    If _
                        Math.Abs((objCandidatePoint - objProjectedPoint).Length) <
                        Math.Abs((objResultPoint - objProjectedPoint).Length) Then
                        objResultPoint = objCandidatePoint
                    End If
                End If
            End If
        Next

        Return objResultPoint
    End Function

    Private Function NearestPopulatedZone(lstZones As List(Of List(Of List(Of List(Of Integer)))),
                                          x As Integer,
                                          y As Integer,
                                          z As Integer,
                                          lstChecked As List(Of List(Of List(Of Boolean)))) As List(Of Integer)

        If (lstChecked(x)(y)(z) = True) Then Return New List(Of Integer)
        lstChecked(x)(y)(z) = True

        If (x > 0) Then
            If (lstZones(x - 1)(y)(z).Count > 0) Then Return lstZones(x - 1)(y)(z)
        End If
        If (x < 10) Then
            If (lstZones(x + 1)(y)(z).Count > 0) Then Return lstZones(x + 1)(y)(z)
        End If

        If (y > 0) Then
            If (lstZones(x)(y - 1)(z).Count > 0) Then Return lstZones(x)(y - 1)(z)
        End If
        If (y < 10) Then
            If (lstZones(x)(y + 1)(z).Count > 0) Then Return lstZones(x)(y + 1)(z)
        End If

        If (z > 0) Then
            If (lstZones(x)(y)(z - 1).Count > 0) Then Return lstZones(x)(y)(z - 1)
        End If
        If (z < 10) Then
            If (lstZones(x)(y)(z + 1).Count > 0) Then Return lstZones(x)(y)(z + 1)
        End If

        Dim maybe As New List(Of Integer)
        If (x > 0) Then
            maybe = NearestPopulatedZone(lstZones, x - 1, y, z, lstChecked)
            If maybe.Count > 0 Then Return maybe
        End If
        If (x < 10) Then
            maybe = NearestPopulatedZone(lstZones, x + 1, y, z, lstChecked)
            If maybe.Count > 0 Then Return maybe
        End If

        If (y > 0) Then
            maybe = NearestPopulatedZone(lstZones, x, y - 1, z, lstChecked)
            If maybe.Count > 0 Then Return maybe
        End If
        If (y < 10) Then
            maybe = NearestPopulatedZone(lstZones, x, y + 1, z, lstChecked)
            If maybe.Count > 0 Then Return maybe
        End If

        If (z > 0) Then
            maybe = NearestPopulatedZone(lstZones, x, y, z - 1, lstChecked)
            If maybe.Count > 0 Then Return maybe
        End If
        If (z < 10) Then
            maybe = NearestPopulatedZone(lstZones, x, y, z + 1, lstChecked)
            If maybe.Count > 0 Then Return maybe
        End If

        Return maybe
    End Function

    Private Function AngleBetween(objEdge As Vector3D,
                                  objToPoint As Vector3D) As Double

        Dim dblAngle As Double
        dblAngle = Vector3D.AngleBetween(objEdge, objToPoint)
        If (dblAngle > 180) Then
            dblAngle = 360 - dblAngle
        End If
        Return dblAngle
    End Function

    Private Function LineIntersectPlane(objProjectionPoint As Point3D,
                                        objProjectionVector As Vector3D,
                                        objPointOnPlane As Point3D,
                                        objPlaneNorm As Vector3D) As Point3D

        'As both the normal and projection vector are normalized this will give us the angle between them
        Dim dblDotProduct As Double = Vector3D.DotProduct(objProjectionVector, objPlaneNorm)

        'If the angle is zero then ignore this triangle
        If Math.Abs(dblDotProduct) < 0.0000001 Then Return Nothing

        'Create a line from the projection point to the edge of the plane
        Dim objProjectionToPoint As Vector3D = objPointOnPlane - objProjectionPoint
        'Then calculate the distance from the projection point to the plane down the projected vector
        'The dot product gives us the distance from the projection point to the plane (at right angles)
        Dim dblDistance As Double
        dblDistance = Vector3D.DotProduct(objProjectionToPoint, objPlaneNorm)/dblDotProduct

        'Calculate projection point
        Return objProjectionPoint + (objProjectionVector*dblDistance)
    End Function

    Public Function ReselectCurvePointHitTest(rawresult As HitTestResult) As HitTestResultBehavior

        Dim rayResult As RayHitTestResult = rawresult

        If (rayResult IsNot Nothing) Then
            Dim rayMeshResult As RayMeshGeometry3DHitTestResult = rayResult
            Dim blnDone = False
            If rayMeshResult IsNot Nothing Then
                For Each objModel As Delcam3DModel In lstModels
                    If TypeOf objModel.FrontVisual Is ModelUIElement3D Then
                        If _
                            CType(CType(objModel.FrontVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                rayMeshResult.ModelHit) OrElse
                            ((objModel.BackVisual IsNot Nothing) AndAlso
                             CType(CType(objModel.BackVisual, ModelUIElement3D).Model, Model3DGroup).Children.Contains(
                                 rayMeshResult.ModelHit)) Then
                            'Remove the old marker
                            Dim objViewport As Viewport3D = GetTemplateChild(PART_Viewport3D)
                            objCurveSketchModel.KeyPoints(intReselectIndex).DeleteFromViewport(objViewport)
                            'Add the new one
                            objCurveSketchModel.KeyPoints(intReselectIndex) = AddMarker(rayMeshResult.PointHit,
                                                                                        rayMeshResult.MeshHit.Normals(
                                                                                            rayMeshResult.VertexIndex1))
                            'Create the new curve
                            CreateCurve(rayMeshResult)
                            Return HitTestResultBehavior.Stop
                        End If
                    End If
                Next
            End If
        End If

        Return HitTestResultBehavior.Continue
    End Function

#End Region

#End Region

#End Region

#Region "Marker Creation"

    ''' <summary>
    ''' Creates a position marker with the specified name.  The position and the normal of the marker must be defined
    ''' </summary>
    ''' <param name="markerName">The name of the marker</param>
    ''' <param name="position">The position of the marker</param>
    ''' <param name="normal">The normal of the marker</param>
    Public Sub CreateMarker(markerName As String,
                            position As Point3D,
                            normal As Vector3D)

        ' Remove any markers of the same name
        If (lstPointMeasureModels.Contains(markerName)) Then
            lstPointMeasureModels.Remove(markerName)
        End If
        ' Create the new marker
        Dim pointMeasureModel As New Delcam3DPointMeasureModel(markerName)
        pointMeasureModel.MarkerModel = AddMarker(position, normal)
        lstPointMeasureModels.Add(pointMeasureModel)
    End Sub

#End Region
End Class
