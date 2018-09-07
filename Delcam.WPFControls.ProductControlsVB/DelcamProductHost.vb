Imports System.Runtime.InteropServices
Imports System.Windows.Interop
Imports Autodesk.ProductInterface

Friend Class DelcamProductHost
    Inherits HwndHost

#Region "Attributes"

    Private _applicationHandle As Integer
    Private _hwndHost As IntPtr = IntPtr.Zero
    Private _application As Automation
    Private _product As DelcamProductBase

#End Region

#Region "Windows API Code"

    Public Enum WindowStyles
        WS_BORDER = &H800000
        WS_CAPTION = &HC00000
        WS_CHILD = &H40000000
        WS_CLIPCHILDREN = &H2000000
        WS_CLIPSIBLINGS = &H4000000
        WS_DISABLED = &H8000000
        WS_DLGFRAME = &H400000
        WS_EX_ACCEPTFILES = &H10&
        WS_EX_DLGMODALFRAME = &H1&
        WS_EX_NOPARENTNOTIFY = &H4&
        WS_EX_TOPMOST = &H8&
        WS_EX_TRANSPARENT = &H20&
        WS_EX_TOOLWINDOW = &H80&
        WS_GROUP = &H20000
        WS_HSCROLL = &H100000
        WS_MAXIMIZE = &H1000000
        WS_MAXIMIZEBOX = &H10000
        WS_MINIMIZE = &H20000000
        WS_MINIMIZEBOX = &H20000
        WS_OVERLAPPED = &H0&
        WS_POPUP = &H80000000
        WS_SYSMENU = &H80000
        WS_TABSTOP = &H10000
        WS_THICKFRAME = &H40000
        WS_VISIBLE = &H10000000
        WS_VSCROLL = &H200000
        '\\ New from 95/NT4 onwards
        WS_EX_MDICHILD = &H40
        WS_EX_WINDOWEDGE = &H100
        WS_EX_CLIENTEDGE = &H200
        WS_EX_CONTEXTHELP = &H400
        WS_EX_RIGHT = &H1000
        WS_EX_LEFT = &H0
        WS_EX_RTLREADING = &H2000
        WS_EX_LTRREADING = &H0
        WS_EX_LEFTSCROLLBAR = &H4000
        WS_EX_RIGHTSCROLLBAR = &H0
        WS_EX_CONTROLPARENT = &H10000
        WS_EX_STATICEDGE = &H20000
        WS_EX_APPWINDOW = &H40000
        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE Or WS_EX_CLIENTEDGE)
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE Or
                               WS_EX_TOOLWINDOW Or WS_EX_TOPMOST)
    End Enum

    Private Const GWL_STYLE& = (- 16)

    Public Declare Function GetWindowLong Lib "user32" _
        Alias "GetWindowLongA"(hwnd As Integer,
                               nIndex As Integer) As Integer
    Public Declare Function SetWindowLong Lib "user32" _
        Alias "SetWindowLongA"(hwnd As Integer,
                               nIndex As Integer,
                               dwNewLong As Integer) As Integer

    Private Declare Function SetParent Lib "user32"(hWndChild As Integer,
                                                    hWndNewParent As Integer) As Integer

    Private Declare Function CreateWindowEx Lib "user32" _
        Alias "CreateWindowExA"(dwExStyle As Integer,
                                lpClassName As String,
                                lpWindowName As String,
                                dwStyle As Integer,
                                x As Integer,
                                y As Integer,
                                nWidth As Integer,
                                nHeight As Integer,
                                hWndParent As IntPtr,
                                hMenu As IntPtr,
                                hInstance As IntPtr,
                                ByRef lpParam As Object) As Int32

    Private Declare Function SetWindowPos Lib "user32"(hwnd As Integer,
                                                       hwndInsertAfter As Integer,
                                                       X As Integer,
                                                       Y As Integer,
                                                       cx As Integer,
                                                       cy As Integer,
                                                       wFlags As Integer) As Integer

    Private Const HWND_TOP As Integer = 0
    Private Const SWP_NOACTIVATE As Integer = &H10

#End Region

#Region "Constructor"

    Public Sub New(application As Automation, product As DelcamProductBase)

        _application = Application
        _product = product
    End Sub

#End Region

#Region "Operations"

    Protected Overrides Function BuildWindowCore(hwndParent As HandleRef) As HandleRef

        _hwndHost = CreateWindowEx(0,
                                   "static",
                                   "",
                                   CType(WindowStyles.WS_CHILD Or WindowStyles.WS_VISIBLE, Integer),
                                   0,
                                   0,
                                   200,
                                   300,
                                   hwndParent.Handle,
                                   &H2,
                                   IntPtr.Zero,
                                   0)

        AddToHost()

        ModifyStyle(_applicationHandle,
                    WindowStyles.WS_CAPTION Or
                    WindowStyles.WS_SYSMENU Or
                    WindowStyles.WS_MINIMIZEBOX _
                    Or WindowStyles.WS_MAXIMIZEBOX,
                    0)

        'objApplication.IsVisible = True

        Return New HandleRef(Me, _hwndHost)
    End Function

    Protected Overrides Function WndProc(hwnd As IntPtr,
                                         msg As Integer,
                                         wParam As IntPtr,
                                         lParam As IntPtr,
                                         ByRef handled As Boolean) As IntPtr

        Const WM_PAINT = &HF

        If ((hwnd = _hwndHost) AndAlso (msg = WM_PAINT)) Then
            Dim ps As New PAINTSTRUCT
            BeginPaint(hwnd, ps)
            EndPaint(hwnd, ps)
            handled = True
            Return IntPtr.Zero
        Else
            Return MyBase.WndProc(hwnd, msg, wParam, lParam, handled)
        End If
    End Function

    <StructLayout(LayoutKind.Sequential, Pack := 4)>
    Public Structure PAINTSTRUCT
        Public hdc As IntPtr
        Public fErase As Integer
        Public rcPaint As Rect
        Public fRestore As Integer
        Public fIncUpdate As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst := 32)> Public rgbReserved As Byte()
    End Structure

    <DllImport("user32.dll")>
    Public Shared Function BeginPaint(
                                      hWnd As IntPtr,
                                      ByRef lpPaint As PAINTSTRUCT) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function EndPaint(
                                    hWnd As IntPtr,
                                    ByRef lpPaint As PAINTSTRUCT) As IntPtr
    End Function

    Protected Overrides Sub DestroyWindowCore(hwnd As HandleRef)

        If (_product IsNot Nothing) Then
            _product.Close()
        End If
    End Sub

    Private Sub ModifyStyle(lHwnd As Integer,
                            intRemove As Integer,
                            intAdd As Integer)

        Dim lngStyle As Long
        Dim lngOldStyle As Long

        lngOldStyle = GetWindowLong(lHwnd, GWL_STYLE)
        lngStyle = (lngOldStyle And (Not intRemove)) Or intAdd
        SetWindowLong(lHwnd, GWL_STYLE, CInt(lngStyle))
    End Sub

    Public Sub AddToHost()

        _application.IsVisible = True
        _applicationHandle = _application.MainWindowId
        SetParent(_applicationHandle, _hwndHost)
    End Sub

    Public Sub RemoveFromHost()

        SetParent(_application.MainWindowId, Nothing)
    End Sub

#End Region

#Region "Event Handlers"

    Private Sub DelcamProductHost_SizeChanged(sender As Object,
                                              e As SizeChangedEventArgs) Handles Me.SizeChanged

        If (_product IsNot Nothing) Then
            'Size should be different depending on whether GUI is shown or hidden
            If (_product.IsLooseEmbedded) Then
                SetWindowPos(_applicationHandle, HWND_TOP, 0, 0, Me.ActualWidth, Me.ActualHeight, SWP_NOACTIVATE)
            ElseIf (_application.Version.Major.ToString.Length = 4 AndAlso _application.Version.Major >= 2018) OrElse (_application.Version.Major.ToString.Length = 2 AndAlso _application.Version.Major >= 18) Then
                SetWindowPos(_applicationHandle, HWND_TOP, -10, -60, Me.ActualWidth + 20, Me.ActualHeight + 70, SWP_NOACTIVATE)
            Else
                SetWindowPos(_applicationHandle, HWND_TOP, -10, -30, Me.ActualWidth + 20, Me.ActualHeight + 40, SWP_NOACTIVATE)
            End If
        End If
    End Sub

#End Region
End Class
