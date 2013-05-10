
' ----------------------------------------------------------------------------
' "THE BEER-WARE LICENSE" (Revision 42):
' <Haydn Trigg@Vagyr> wrote this file. As long as you retain this notice you
' can do whatever you want with this stuff. If we meet some day, and you think
' this stuff is worth it, you can buy me a beer in return Haydn-Richard Trigg
' ----------------------------------------------------------------------------

'Visual Basic Isotope Framework port by Haydn Trigg
'Written by: Haydn Trigg
'Created : 5/2/2013
'Modified: 5/8/2013
'Version : 0.7.1
'This is the primary backbone framework for the engine.

Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Collections.Generic
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
Imports HTSpaceGame.IsotopeVB.GraphicsMath
Imports System.Media

Namespace IsotopeVB
    'Launch the main function inside of the HTSpaceGame namespace.
    Module mainModule

        Sub Main()
            Dim game = New Game()
        End Sub
    End Module

    'The main game class
    Partial Public Class Game

        'Display Window(form)
        Dim WithEvents gViewport As Viewport

        'The threads
        Private updateThread As Thread

        'The generic random number generator
        Dim gRandom As New Random

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

        'Load the "user32.dll" and create the GetKeyboardState Function
        <DllImport("user32.dll")>
        Private Shared Function GetKeyboardState(ByVal keyState() As Byte) As Boolean
        End Function
        'Load the "user32.dll" and create the GetAsyncKeyState Function
        <DllImport("user32.dll")>
        Private Shared Function GetAsyncKeyState(ByVal vKey As Int32) As UShort
        End Function

        'Pointer Position Api Structure
        Private Structure PointerPositionAPI
            Public X As Long
            Public Y As Long
        End Structure

        'Resource found from http://www.daniweb.com/software-development/vbnet/threads/11533/how-can-i-get-mouse-position by Skullmagic
        'Load the "user32.dll" so the Update function can use the benefits of the mouse position.
        Private Declare Function GetCursorPos Lib "user32" (ByVal lpPoint As PointerPositionAPI) As Long
        Dim gPointerPosition As PointerPositionAPI


        'Integer to return a refresh rate. "ID116"
        Private Const _REFRESH As Long = 116
        'Constant update of 60 ticks per second.
        Private _UPDATETIME As Double = 1000D / 60D
        'Used to store the start time of the draw call. This will remove the up/down fps.
        Private _STARTUPDATETIME As Double = 0
        'Variable update of # frames per second.
        'Note: Using a Double integer to timing precision.
        Private _DRAWTIME As Double = 0
        'Used to store the start time of the draw call. This will remove the up/down fps.
        Private _STARTDRAWTIME As Double = 0

        'FPS Counter Object
        Dim fps As New FPS()

        'Store the time information.
        Dim StartTime As DateTime = DateTime.Now() 'Total time
        Dim LastTime As DateTime = DateTime.Now() 'Total time last update
        'Returns a GameTime object storing the time information in an easy to use interface.
        ReadOnly Property getGameTime As GameTime
            Get
                Dim Now As DateTime = DateTime.Now
                Dim gameTime As GameTime = New GameTime(Now - StartTime, Now - LastTime)
                LastTime = Now
                Return gameTime
            End Get
        End Property
        Dim gGameTime As GameTime

        'GDI+ Backbuffer and Graphics Device for traditional WindowsForms rendering
        Public BackBuffer As Bitmap
        Public SpriteBatch As Graphics

        Public Sub New()
            'Setup the display frequency
            Dim lMode As Integer = -1
            Dim b As Boolean = EnumDisplaySettings(Nothing, lMode, dm)
            'Not working, different type of frequency. Find alternative.

            'Create the new threding for the game's update loop
            updateThread = New Thread(Sub() Update())

            'Create the Viewport
            gViewport = New Viewport(600, 600, "Game")

            gViewport.VSync = OpenTK.VSyncMode.Adaptive

            'Setup the GDI+ Backbuffers using the ResolutionChange Sub Function
            ResolutionChange()

            'Load the game content ready for use
            gLoadContent()

            'Start the games update and draw threads
            updateThread.Start()

            'Use the STAThread to run the Viewport
            gViewport.Run()
        End Sub

        'The games update thread (TPS/UPS)
        Public Sub Update()
            While updateThread.IsAlive
                _STARTUPDATETIME = System.Diagnostics.Stopwatch.GetTimestamp
                'Update the gametime ready for this update iteration
                gGameTime = getGameTime

                'Get the pointer position ready to use this update iteration

                'GetCursorPos(gPointerPosition)

                gUpdate(gGameTime)

                'Update the fps counter
                fps.Update(gGameTime)

                Thread.Sleep(GameMath.Clamp(_UPDATETIME - ((System.Diagnostics.Stopwatch.GetTimestamp - _STARTUPDATETIME) / 1000), 0, _UPDATETIME))
            End While
            'Catch any errors and report them to the console.
        End Sub


        'The games draw thread (FPS) [Main Thread]
        Public Sub Draw() Handles gViewport.RenderFrame
            If (gViewport.IsExiting) Then
                Exits()
            End If

            'Update the Frames Per Second Counter
            fps.Draw()

            'Clears the Viewport to the popular 2D CornflowerBlue
            GL.ClearColor(Color.CornflowerBlue)
            'Clears the Bugger Masks ready for 2D Drawing
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadIdentity()
            'Setup Orthographic Rendering see: http://en.wikipedia.org/wiki/Orthographic_projection
            GL.Ortho(0, gViewport.Width, gViewport.Height, 0, -1, 1)
            'Creates the Viewport at 0,0 with its Width and Height
            GL.Viewport(0, 0, gViewport.Width, gViewport.Height)

            GL.Enable(EnableCap.Texture2D)

            'Clears the GDI+ Backbuffer
            SpriteBatch.Clear(Color.FromArgb(0, 0, 0, 0))
            '<<<ALL SPECIFIC DRAWING CODE BELOW HERE>>>

            'Begin the gDraw() Logic Function
            gDraw_Main()

                    Dim data As BitmapData = BackBuffer.LockBits(New Rectangle(0, 0, BackBuffer.Width, BackBuffer.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
            BackBuffer.UnlockBits(data)

            DrawBackbuffer(New Vector2(0, 0), New Vector2(gViewport.Width, gViewport.Height))

            '<<ALL SPECIFIC DRAWING CODE ABOVE HERE>>>

            GL.Disable(EnableCap.Texture2D)
            'End Drawing things with OpenGL
            GL.[End]()
            'Flush the Device
            GL.Flush()
            'Swap the buffers around, 0-1, 1-0 "Double Buffering" ready for the next frame.
            gViewport.SwapBuffers()
        End Sub

        Private Sub Exits() Handles gViewport.Closing, gViewport.Closed, gViewport.Disposed
            updateThread.Abort()
            End
        End Sub

        Private Sub ResolutionChange() Handles gViewport.Resize
            'Create a Bitmap Backbuffer and GDI+ Graphics Interface "SpriteBatch"
            BackBuffer = New Bitmap(GameMath.ClampI(gViewport.Width, 1, Integer.MaxValue), GameMath.ClampI(gViewport.Height, 1, Integer.MaxValue))
            SpriteBatch = Graphics.FromImage(BackBuffer)
        End Sub
    End Class
End Namespace

