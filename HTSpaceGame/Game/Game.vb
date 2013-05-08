'Visual Basic Isotope Framework port by Haydn Trigg
'Written by: Haydn Trigg
'Created : 5/2/2013
'Modified: 5/8/2013
'Version : 0.3.0
'This is the primary backbone framework for the engine.

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
    Partial Public Class Game

        'Display Window(form)
        Dim WithEvents Viewport As New ViewportWindow

        'The threads
        Private updateThread As Thread
        Private drawThread As Thread

        'The generic random number generator
        Dim r As New Random

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

        'Load the "user32.dll" so the Update function can use the benefits of individual keys.
        Public Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Int32) As UShort




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
        'Constant update of 20 ticks per second.
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

        'The graphics device
        Dim gGraphicsDevice As GraphicsDevice
        ReadOnly Property gSpriteBatch As Graphics
            Get
                Return gGraphicsDevice.SpriteBatch
            End Get
        End Property

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

        Public Sub New()
                'Setup the display frequency
                Dim lMode As Integer = -1
                Dim b As Boolean = EnumDisplaySettings(Nothing, lMode, dm)
                'Not working, different type of frequency. Find alternative.

                'Load the game content ready for use
                gLoadContent()

                'Create the new threding for the game's update loop
                updateThread = New Thread(Sub() Update())

                drawThread = New Thread(Sub() Draw())


                'Show the viewport
                Viewport.Show()

                Viewport.Text = "SpaceGame"
                'Make the viewport backcolor black so it dosen't interfere with the GraphicsDevice, also reduces apperance of window resizing opposed to white
                Viewport.BackColor = Color.Black

                gGraphicsDevice = New GraphicsDevice(Viewport.ClientSize.Width, Viewport.ClientSize.Height)
                'Initialize the graphics device with the specified quality of the game.
                'Use high quality bicubic and bilinear interpolation for better image quality
                gGraphicsDevice.Initialize(Drawing2D.InterpolationMode.High, Drawing2D.SmoothingMode.AntiAlias)


                'Start the games update and draw threads
                updateThread.Start()
                drawThread.Start()
                'Use the STAThread to become the main thread for the viewport draw calls.
                Application.Run(Viewport)


        End Sub
        Dim gGameTime As GameTime
        'The games update thread (TPS/UPS)
        Public Sub Update()
            While (updateThread.IsAlive)
                _STARTUPDATETIME = System.Diagnostics.Stopwatch.GetTimestamp
                'Console.WriteLine("Update")

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
        Public Sub Draw()
            While (updateThread.IsAlive)
                _STARTDRAWTIME = System.Diagnostics.Stopwatch.GetTimestamp
                If (Viewport.IsDisposed) Then
                    Exits()
                    Exit While
                End If

                'Check if the Viewport resolution isnt the same as the GraphicsDevices instanced resolution.
                If Not (gGraphicsDevice.Width = Viewport.ClientSize.Width AndAlso gGraphicsDevice.Height = Viewport.ClientSize.Height) Then
                    gGraphicsDevice.ChangeResulution(Viewport.ClientSize.Width, Viewport.ClientSize.Height)
                    System.Diagnostics.Debug.WriteLine("Resolution has changed to: " + gGraphicsDevice.Width.ToString() + "x" + gGraphicsDevice.Height.ToString())
                End If

                'Update the Frames Per Second Counter
                fps.Draw()

                'Clear the backbuffer so a new round of images can be drawn without intefering with the originals.
                gGraphicsDevice.SpriteBatch.Clear(Color.CornflowerBlue)

                '<<<ALL SPECIFIC DRAWING CODE BELOW HERE>>>

                'Begin the gDraw() Logic Function
                gDraw()

                '<<ALL SPECIFIC DRAWING CODE ABOVE HERE>>>

                'Send the viewport backbuffer to be drawn by the graphics card inside of the Viewport and its thread (STAThread)
                Viewport.gDraw(gGraphicsDevice.BackBuffer)

                If (_DRAWTIME <= 1) Then
                    'If unlimited frames, then run with a 1ms delay. This will stop an overload of CPU.
                    Thread.Sleep(1)
                Else
                    'If framerate is not infinity, iterate with the set delay. Calculate the time difference aswell to ensure there are no problems.
                    'Calculate the time difference between the draw call and the next frame
                    Thread.Sleep(Math.Floor(GameMath.Clamp(_DRAWTIME - ((System.Diagnostics.Stopwatch.GetTimestamp - _STARTDRAWTIME) / 10000), 0, _DRAWTIME)))
                End If
            End While
            'Catch any errors and report them to the console.
        End Sub

        Private Sub Exits() Handles Viewport.FormClosing, Viewport.Disposed
            updateThread.Abort()
            drawThread.Abort()
            End
        End Sub
    End Class
End Namespace

