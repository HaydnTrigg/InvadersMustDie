'Isotope Framework by Haydn Trigg
'Written by: Haydn Trigg
'Created : 5/2/2013
'Modified: 5/2/2013
'Version : N/A


Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing

Namespace HTSpaceGame

    'Launch the main function inside of the HTSpaceGame namespace.
    Module mainModule

        Sub Main()
            Dim game = New Game()

        End Sub
    End Module

    'The main game class
    Public Class Game

        Dim Viewport As New Form
        Private updateThread As Thread
        Private drawThread As Thread




        Public Const SA_SIZE As Integer = 32

        'Windows Graphics Device Intefrace Structure http://msdn.microsoft.com/en-us/library/windows/desktop/dd183565%28v=vs.85%29.aspx

        Structure DEVMODE
            Public iSpecVersion As Short
            Public iDriverVersion As Short
            Public iSize As Short
            Public iDriverExtra As Short
            Public iFields As Integer
            Public iOrientation As Short
            Public iPaperSize As Short
            Public iPaperLength As Short
            Public iPaperWidth As Short
            Public iScale As Short
            Public iCopies As Short
            Public iDefaultSource As Short
            Public iPrintQuality As Short
            Public iColor As Short
            Public iDuplex As Short
            Public iYRes As Short
            Public iTTOption As Short
            Public iCollate As Short
            Public lpszFormName As String
            Public iLogPixels As Short
            Public iBitsPerPixel As Integer
            Public lPelsWidth As Integer
            Public lPelsHeight As Integer
            Public lDisplayFlags As Integer
            Public lDisplayFreq As Integer
            Public lICMMethod As Integer
            Public lICMIntent As Integer
            Public lMediaType As Integer
            Public lDitherType As Integer
            Public lReserved1 As Integer
            Public lReserved2 As Integer
            Public lPanWidth As Integer
            Public lPanHeight As Integer
        End Structure
        Dim dm As New DEVMODE
        'Information and documentation @ http://msdn.microsoft.com/en-au/library/windows/desktop/dd145203%28v=vs.85%29.aspx
        'Microsoft Windows API Graphics Device Interface EnumDisplaySettings http://msdn.microsoft.com/en-us/library/windows/desktop/dd162611%28v=vs.85%29.aspx
        Public Declare Auto Function EnumDisplaySettings Lib "user32" (ByVal lpszDeviceName As String, ByVal lModeNum As Integer, ByRef lpdm As DEVMODE) As Boolean


        'Integer to return a refresh rate. "ID116"
        Private Const _REFRESH As Long = 116
        'Constant update of 20 ticks per second.
        Private Const _UPDATETIME As Integer = 50
        'Variable update of # frames per second.
        Private _DRAWTIME As Integer = 0

        Public Sub New()
            'Setup the display frequency
            Dim lMode As Integer = -1
            Dim b As Boolean = EnumDisplaySettings(Nothing, lMode, dm)
            _DRAWTIME = 1000 / 60

            Console.WriteLine("REFRESH: " + dm.lDisplayFreq.ToString())
            Console.WriteLine("_DRAWTIME: " + _DRAWTIME.ToString())

            Viewport.Show()


            updateThread = New Thread(Sub() Update())
            'drawThread = New Thread(Sub() Draw())
            updateThread.Start()
            'drawThread.Start()
            Draw()

        End Sub

        'The games update thread (TPS/UPS)
        Public Sub Update()
            While (updateThread.IsAlive)

                'Console.WriteLine("Update")
                Thread.Sleep(_UPDATETIME)
            End While
        End Sub

        'The games draw thread (FPS)
        Public Sub Draw()
            While (updateThread.IsAlive)

                'Update the viewport and redraw all of its elements.
                'Viewport.Update()
                'Console.WriteLine("Draw")

                    If (_DRAWTIME = 0) Then
                        'If unlimited frames, then run with a 1ms delay.
                        Thread.Sleep(1)
                    Else
                        'If framerate is not infinity, iterate with the set delay.
                        Thread.Sleep(_DRAWTIME)
                    End If

            End While
        End Sub




    End Class
End Namespace

