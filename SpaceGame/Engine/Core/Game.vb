' ----------------------------------------------------------------------------
' "THE BEER-WARE LICENSE" (Revision 42):
' <Haydn Trigg@Vagyr> wrote this file. As long as you retain this notice you
' can do whatever you want with this stuff. If we meet some day, and you think
' this stuff is worth it, you can buy me a beer in return Haydn-Richard Trigg
' ----------------------------------------------------------------------------

'Visual Basic Isotope Framework port by Haydn Trigg
'Written by: Haydn Trigg
'Created : 2/5/2013 [dd/mm/yyyy]
'Modified: 16/5/2013 [dd/mm/yyyy]
'Version : 0.3.30.1 (Relsease, Revision, Version, Build)

#Region "Imports"
'System Imports
Imports System
Imports System.Threading

'OpenTK Imports
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
#End Region

'The "Game Engine" namespace.
Namespace Isotope
#Region "Main"
    'The Main Module for the program
    Module mainModule
        'The main function for the program. This is the first thing that is run.
        <STAThread()>
        Sub Main()
            'Creates a new game on the STAThread
            Dim game = New Game()
        End Sub
    End Module
#End Region

    Public Class Game
#Region "Variables & Properties"
        'The games viewable Viewport
        Dim WithEvents gViewport As Viewport

        'The games Update Threads
        Dim gUpdateThread As Thread
        Dim gEffectThread As Thread

        'Specifies if the game will use the EffectThread
        Dim bUsingEffectThread As Boolean

        'Initializes a generic Random number generation used throughout the game
        Dim gRandom As New Random

        'Keyboard States used throughout the game. Default value set to the current Keyboard state upon program launch.
        Dim gPreviousKeyboardState As OpenTK.Input.KeyboardState = OpenTK.Input.Keyboard.GetState()
        Dim gCurrentKeyboardState As OpenTK.Input.KeyboardState = OpenTK.Input.Keyboard.GetState()

        'Mouse States used throughout the game. Default value set to the current Keyboard state upon program launch.
        Dim gPreviousMouseState As OpenTK.Input.MouseState = OpenTK.Input.Mouse.GetState()
        Dim gCurrentMouseState As OpenTK.Input.MouseState = OpenTK.Input.Mouse.GetState()

        'Integer that contains an amount of CPU threads that are recognised by default to the Windows operating system
        Dim _CoreCount As Integer = System.Environment.ProcessorCount
        'Note: Using a Double integer to timing precision.
        'Constant update of 60 ticks per second.
        Private _UPDATETIME As Double = 1000D / 60D
        'Used to store the start time of the update call. This will remove the up/down fps.
        Private _STARTUPDATETIME As Double = 0
        'Used to store the start time of the effect call. This will remove the up/down fps.
        Private _STARTEFFECTTIME As Double = 0

        'Store the game time information. Total and Delta.
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

#End Region
#Region "Initializer"

        Public Sub New()
            'Create the gameTime
            gGameTime = getGameTime

            'Create the Viewport
            gViewport = New Viewport(640, 480, "Game")

            'Load the settings for the game.
            gViewport.VSync = OpenTK.VSyncMode.Adaptive

            'Load the game content ready for use
            gLoadContent()

            'WARNING: MUST BE STARTED BEFORE EFFECT THREAD DUE TO DEPENDENCY
            'Create the new threding for the game's update loop
            gUpdateThread = New Thread(Sub() Update())

            'Create the new threding for the game's update loop
            gEffectThread = New Thread(Sub() UpdateEffects())

            'Start the games update and effects
            gUpdateThread.Start()

            'Use the STAThread to run the Viewport
            gViewport.Run()
        End Sub

#End Region
#Region "Functions"

        'The game function that handels the effectThread
        Public Sub UpdateEffects()
            While gEffectThread.IsAlive And gUpdateThread.IsAlive
                _STARTEFFECTTIME = System.Diagnostics.Stopwatch.GetTimestamp

                'Update the effects
                gUpdateEffects(gGameTime)

                'Sleep for the appropriate time taking into account the time it takes to process this loop
                Thread.Sleep(GameMath.ClampDouble(_UPDATETIME - ((System.Diagnostics.Stopwatch.GetTimestamp - _STARTEFFECTTIME) / 1000), 1, _UPDATETIME))
            End While
            Exits()
        End Sub

        'The games update thread (TPS/UPS)
        Public Sub Update()
            'If the host computer has more then 2 virtual cores then enable the second thread.
            If _CoreCount >= 2 Then
                bUsingEffectThread = True
                gEffectThread.Start()
            End If
            While gUpdateThread.IsAlive

                _STARTUPDATETIME = System.Diagnostics.Stopwatch.GetTimestamp
                'Update the gametime ready for this update iteration
                gGameTime = getGameTime

                'Update the Keyboard+Mouse State if the client is currently viewing the game
                If gViewport.Focused Then
                    gCurrentKeyboardState = OpenTK.Input.Keyboard.GetState()
                    gCurrentMouseState = OpenTK.Input.Mouse.GetState()
                End If

                'Update the GameLogic
                gUpdate(gGameTime)

                'If not using a thread for the effects, then update the effects using this thread.
                If Not bUsingEffectThread Then
                    gUpdateEffects(gGameTime)
                End If

                'Update the Keyboard+Mouse State
                gPreviousMouseState = gCurrentMouseState
                gPreviousKeyboardState = gCurrentKeyboardState

                'Sleep for the appropriate time taking into account the time it takes to process this loop
                Thread.Sleep(GameMath.ClampDouble(_UPDATETIME - ((System.Diagnostics.Stopwatch.GetTimestamp - _STARTUPDATETIME) / 1000), 1, _UPDATETIME))
            End While
            Exits()
        End Sub


        'The games draw thread (FPS) [Main Thread]
        Public Sub Draw() Handles gViewport.RenderFrame
            If (gViewport.IsExiting) Then
                Exits()
            End If

            'Clear the OpenGL Device with Black ready to draw a frame.
            GL.ClearColor(Color4.Black)

            'Clears the color and depth mask.
            GL.Clear(ClearBufferMask.ColorBufferBit)
            'Creates a default Projection Matrix (Matrix.Identity)
            GL.MatrixMode(MatrixMode.Projection)
            'Loads the Matrix Identity (Matrix.Identity)
            GL.LoadIdentity()
            'Setup Orthographic Rendering see: http://en.wikipedia.org/wiki/Orthographic_projection
            GL.Ortho(0, gViewport.Width / gViewport.ViewportScale, gViewport.Height / gViewport.ViewportScale, 0, -1, 1)

            'Creates the Viewport at 0,0 with its Width and Height
            GL.Viewport(0, 0, gViewport.Width, gViewport.Height)
            'Enables rendering textures with the current pass.
            GL.Enable(EnableCap.Texture2D)

            '<<<ALL SPECIFIC DRAWING CODE BELOW HERE>>>

            'Begin the gDraw() Logic Function
            gDraw()

            '<<ALL SPECIFIC DRAWING CODE ABOVE HERE>>>
            'Disables Rendering Textures with the current pass.
            GL.Disable(EnableCap.Texture2D)
            'End Drawing things with OpenGL
            GL.[End]()
            'Flush the Device
            GL.Flush()
            'Swap the buffers around, 0-1, 1-0 "Double Buffering" ready for the next frame.
            gViewport.SwapBuffers()
        End Sub

        Private Sub Exits() Handles gViewport.Closing, gViewport.Closed, gViewport.Disposed
            End
        End Sub

#End Region
    End Class
End Namespace


