
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
Imports System.Runtime.InteropServices
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports HTSpaceGame.IsotopeVB.GraphicsMath

#Const DEBUGNOTIFY = False

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

        'Does the game use an effect thread?
        Dim bEffectThread As Boolean

        'The threads
        Private updateThread As Thread
        Private effectThread As Thread

        'The generic random number generator
        Dim gRandom As New Random

        'Keyboard States
        Dim gPreviousKeyboardState As OpenTK.Input.KeyboardState = OpenTK.Input.Keyboard.GetState()
        Dim gCurrentKeyboardState As OpenTK.Input.KeyboardState = OpenTK.Input.Keyboard.GetState()

        'Mouse States
        Dim gPreviousMouseState As OpenTK.Input.MouseState = OpenTK.Input.Mouse.GetState()
        Dim gCurrentMouseState As OpenTK.Input.MouseState = OpenTK.Input.Mouse.GetState()

        'Gamepad States - BROKEN
        'Dim gPreviousGamePadState As OpenTK.Input.GamePadState = OpenTK.Input.GamePad.GetState()
        'Dim gCurrentGamePadState As OpenTK.Input.GamePadState = OpenTK.Input.GamePad.GetState()

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

        'Integer that contains an amount of CPU Master Threads avaliable to the computer, recognised as cores
        Dim gCoreCount As Integer = System.Environment.ProcessorCount
        'Integer to return a refresh rate. "ID116"
        Private Const _REFRESH As Long = 116
        'Constant update of 60 ticks per second.
        Private _UPDATETIME As Double = 1000D / 60D
        'Used to store the start time of the update call. This will remove the up/down fps.
        Private _STARTUPDATETIME As Double = 0
        'Used to store the start time of the effect call. This will remove the up/down fps.
        Private _STARTEFFECTTIME As Double = 0
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
        'Public BackBuffer As Bitmap
        'Public SpriteBatch As Graphics

        Public Sub New()
            'Create the gameTime
            gGameTime = getGameTime

            'Create the Viewport
            gViewport = New Viewport(800, 800, "Game")

            gViewport.VSync = OpenTK.VSyncMode.Adaptive

            'Load the game content ready for use
            gLoadContent()

            'WARNING: MUST BE STARTED BEFORE EFFECT THREAD DUE TO DEPENDENCY
            'Create the new threding for the game's update loop
            updateThread = New Thread(Sub() Update())

            'Create the new threding for the game's update loop
            effectThread = New Thread(Sub() UpdateEffects())

            'Start the games update and effects
            updateThread.Start()


            'Use the STAThread to run the Viewport
            gViewport.Run()
        End Sub

        'The game function that handels the effectThread
        Public Sub UpdateEffects()
            While effectThread.IsAlive And updateThread.IsAlive
#If Not DEBUG Then
            Try
#End If
                _STARTEFFECTTIME = System.Diagnostics.Stopwatch.GetTimestamp
                'DEBUGTEXT Console.WriteLine(gGameTime.TotalGameTime.ToString() + ":EFFECT")
                'Update the effects
                gUpdateEffects(gGameTime)
#If DEBUG And DEBUGNOTIFY Then
                Console.WriteLine("UpdateEffects(" + gGameTime.TotalGameTime.ToString() + ") Finished")
#End If

                Thread.Sleep(GameMath.Clamp(_UPDATETIME - ((System.Diagnostics.Stopwatch.GetTimestamp - _STARTEFFECTTIME) / 1000), 0, _UPDATETIME))
#If Not DEBUG Then
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
#End If
            End While
            Exits()
        End Sub

        'The games update thread (TPS/UPS)
        Public Sub Update()
            If gCoreCount >= 2 Then
                bEffectThread = True
                effectThread.Start()
            End If
            While updateThread.IsAlive
#If Not DEBUG Then
            Try
#End If
                _STARTUPDATETIME = System.Diagnostics.Stopwatch.GetTimestamp
                'Update the gametime ready for this update iteration
                gGameTime = getGameTime

                'Update the Keyboard+Mouse State
                gCurrentKeyboardState = OpenTK.Input.Keyboard.GetState()
                gCurrentMouseState = OpenTK.Input.Mouse.GetState()

                'Get the pointer position ready to use this update iteration

                'GetCursorPos(gPointerPosition)

                gUpdate(gGameTime)

                'Update the fps counter
                fps.Update(gGameTime)

                If Not bEffectThread Then
                    gUpdateEffects(gGameTime)
                End If

                'Update the Keyboard+Mouse State
                gPreviousMouseState = gCurrentMouseState
                gPreviousKeyboardState = gCurrentKeyboardState
#If DEBUG And DEBUGNOTIFY Then
                Console.WriteLine("Update(" + gGameTime.TotalGameTime.ToString() + ") Finished")
#End If
                Thread.Sleep(GameMath.Clamp(_UPDATETIME - ((System.Diagnostics.Stopwatch.GetTimestamp - _STARTUPDATETIME) / 1000), 0, _UPDATETIME))
#If Not DEBUG Then
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
#End If
            End While
            Exits()
        End Sub


        'The games draw thread (FPS) [Main Thread]
        Public Sub Draw() Handles gViewport.RenderFrame
#If Not DEBUG Then
            Try
#End If
            If (gViewport.IsExiting) Then
                Exits()
            End If

            'Update the Frames Per Second Counter
            fps.Draw()

            GL.ClearColor(Color4.Black)
            'Clears the Bugger Masks ready for 2D Drawing
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadIdentity()
            'Setup Orthographic Rendering see: http://en.wikipedia.org/wiki/Orthographic_projection
            GL.Ortho(0, gViewport.Width / gViewport.ViewportScale, gViewport.Height / gViewport.ViewportScale, 0, -1, 1)
            'Creates the Viewport at 0,0 with its Width and Height
            GL.Viewport(0, 0, gViewport.Width, gViewport.Height)

            GL.Enable(EnableCap.Texture2D)

            'Clears the GDI+ Backbuffer
            'SpriteBatch.Clear(Color.FromArgb(0, 0, 0, 0))
            '<<<ALL SPECIFIC DRAWING CODE BELOW HERE>>>

            'Begin the gDraw() Logic Function
            gDraw_Main()
            'gDraw_Backbuffer()

            '<<ALL SPECIFIC DRAWING CODE ABOVE HERE>>>

            GL.Disable(EnableCap.Texture2D)
            'End Drawing things with OpenGL
            GL.[End]()
            'Flush the Device
            GL.Flush()
            'Swap the buffers around, 0-1, 1-0 "Double Buffering" ready for the next frame.
            gViewport.SwapBuffers()
#If DEBUG And DEBUGNOTIFY Then
            Console.WriteLine("Draw(" + gGameTime.TotalGameTime.ToString() + ") Finished")
#End If
#If Not DEBUG Then
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
#End If
        End Sub

        Private Sub Exits() Handles gViewport.Closing, gViewport.Closed, gViewport.Disposed
            updateThread.Abort()
            End
        End Sub

        'Private Sub gDraw_Backbuffer()
        'Dim data As BitmapData = BackBuffer.LockBits(New Rectangle(0, 0, BackBuffer.Width, BackBuffer.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        '   GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
        '   BackBuffer.UnlockBits(data)
        '   DrawBackbuffer(New Vector2(0, 0), New Vector2(gViewport.Width, gViewport.Height))
        'End Sub
    End Class
End Namespace

