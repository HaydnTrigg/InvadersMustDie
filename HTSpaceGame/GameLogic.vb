'Visual Basic Isotope Framework port by Haydn Trigg
'Written by: Haydn Trigg
'Created : 5/8/2013
'Modified: 5/10/2013
'Version : 0.3.0
'This is the logic part of the game. This will include the load content, update and draw functions as individual parts of threads.


Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Collections.Generic
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Math
Imports OpenTK.Input



Namespace IsotopeVB
    Partial Public Class Game
        Dim GameObjects As New List(Of GameObject)
        Dim gTextures As New List(Of TextureID)


        Enum GameState
            Loading = 0
            Menu = 1
            Game = 2
        End Enum
        Dim gGameState As GameState = GameState.Loading
        Dim gPauseState As Boolean = False

        Private Sub gLoadContent()
            GameObjects.Add(New PlayerShip(1, New Vector2(0, 0), New Vector2(48, 48)))

            gTextures.Add(LoadTexture("Resources/Avatar.png"))
            gTextures.Add(LoadTexture("Resources/ship.png"))
            gTextures.Add(LoadTexture("Resources/BackgroundOverlay.png"))
            gTextures.Add(LoadTexture("Resources/background.png"))
            gTextures.Add(LoadTexture("Resources/Cursor.png"))

            gViewport.WindowBorder = WindowBorder.Fixed

            gViewport.CursorVisible = True

            'Enable OpenGL Alpha Blending
            GL.Enable(EnableCap.Blend)
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)

            gGameState = GameState.Game
        End Sub

        Private Sub gUpdate(ByVal gGameTime As GameTime)
            Dim vPlayerSpeed As Single
            If GetAsyncKeyState(Keys.W) Then
                vPlayerSpeed = -1
            End If
            If GetAsyncKeyState(Keys.S) Then
                vPlayerSpeed = 1
            End If

            If GetAsyncKeyState(Keys.Escape) Then
                If gPauseState Then
                    gPauseState = False
                Else
                    gPauseState = True
                End If
            End If

            'Iterate through all the games objects
            For Each g As GameObject In GameObjects
                'Select the type of object and do the appropriate update for it
                Select Case g.GetType()
                    Case GetType(Ship)
                        'Assign the ship its own independant object as its type using a cast
                        Dim gObject As Ship = CType(g, Ship)

                        gObject.Update(gGameTime)
                        g = gObject
                    Case GetType(PlayerShip)
                        'Assign the ship its own independant object as its type using a cast
                        Dim gObject As PlayerShip = CType(g, PlayerShip)
                        gObject.vSpeed = vPlayerSpeed
                        gObject.Update(gGameTime, gViewport.MousePosition)

                        g = gObject
                End Select
                g.vPosition.X = GameMath.Clamp(g.vPosition.X, 0, 1000 - g.vSize.X)
                g.vPosition.Y = GameMath.Clamp(g.vPosition.Y, 0, 1000 - g.vSize.Y)
            Next

            If GameMath.Vector2Distance(GameObjects(0).vPosition - New Vector2(gViewport.Width / 2, gViewport.Height / 2), gViewport.ViewportPosition) < 1.1F Then
                gViewport.ViewportPosition = GameObjects(0).vPosition - New Vector2(gViewport.Width / 2, gViewport.Height / 2)

            End If
            gViewport.ViewportPosition = GameMath.Lerp(gViewport.ViewportPosition, GameObjects(0).vPosition - New Vector2(gViewport.Width / 2, gViewport.Height / 2), gGameTime.ElapsedGameTime * 2)
            gViewport.ViewportPosition.X = GameMath.Clamp(gViewport.ViewportPosition.X, -gViewport.ViewportBoundary, 1000 + gViewport.ViewportBoundary - gViewport.Width)
            gViewport.ViewportPosition.Y = GameMath.Clamp(gViewport.ViewportPosition.Y, -gViewport.ViewportBoundary, 1000 + gViewport.ViewportBoundary - gViewport.Height)

        End Sub



        'The primary draw function
        'This draw function has been setup for 2D Rendering using 3d primatives
        Private Sub gDraw_Main()

            GL.ClearColor(Color4.CornflowerBlue)
            GL.Clear(ClearBufferMask.ColorBufferBit)

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

            gDraw_Secondary()

            GL.Disable(EnableCap.Texture2D)
            'End Drawing things with OpenGL
            GL.[End]()
            'Flush the Device
            GL.Flush()
            'Swap the buffers around, 0-1, 1-0 "Double Buffering" ready for the next frame.
            gViewport.SwapBuffers()

        End Sub

        'The secondary draw function
        'All logic drawing code is inside this function
        Private Sub gDraw_Secondary()
            Select Case gGameState
                Case GameState.Game
                    'Draw the background
                    Draw2d(gTextures(3).ID, New Vector2(-gViewport.ViewportBoundary, -gViewport.ViewportBoundary) + gViewport.ViewportPosition / 2, gTextures(3).Size)
                    Draw2d(gTextures(2).ID, New Vector2(0, 0), gTextures(2).Size)

                    'Draw the game
                    'Iterate through all the games objects
                    For Each g As GameObject In GameObjects
                        'Select the type of object and do the appropriate update for it
                        Select Case g.GetType()
                            Case GetType(Ship)
                                'Assign the ship its own independant object as its type using a cast
                                Dim gObject As Ship = CType(g, Ship)
                                Draw2d(gTextures(gObject.iTextureID).ID, gObject.vPosition, g.vSize)
                            Case GetType(PlayerShip)
                                'Assign the ship its own independant object as its type using a cast
                                Dim gObject As PlayerShip = CType(g, PlayerShip)
                                Draw2dRotated(gTextures(gObject.iTextureID).ID, gObject.vPosition, g.vSize, gGameTime.TotalGameTime * 18 / Math.PI)
                        End Select
                    Next

                    'Draw the hud
                    Draw2d(gTextures(4).ID, gTextures(4).Size / -2 + gViewport.MousePosition, gTextures(4).Size)
                Case GameState.Menu
                    'Menu Stuff

                Case GameState.Loading
                    'Nothing

            End Select
            
        End Sub



        Public Function LoadTexture(ByVal path As String) As TextureID
            Dim tid As New TextureID

            'Load image to a bitmap
            Dim Bitmap As New Bitmap(path)

            Dim texture As Integer
            'Generate texture
            GL.GenTextures(1, texture)
            GL.BindTexture(TextureTarget.Texture2D, texture)

            ' Store texture size
            tid.Width = Bitmap.Width
            tid.Height = Bitmap.Height
            tid.ID = texture

            Dim data As BitmapData = Bitmap.LockBits(New Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.[ReadOnly], System.Drawing.Imaging.PixelFormat.Format32bppPArgb)

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)

            Bitmap.UnlockBits(data)

            ' Setup filtering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))

            Return tid

        End Function

        Structure TextureID
            Public Width As Integer, Height As Integer, ID As Integer
            ReadOnly Property Size As Vector2
                Get
                    Return New Vector2(Width, Height)
                End Get
            End Property
        End Structure
    End Class
End Namespace

