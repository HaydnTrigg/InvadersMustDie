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
Imports OpenTK.Input
Imports HTSpaceGame.IsotopeVB.GraphicsMath
Imports System.Media
Imports HTSpaceGame.IsotopeVB.Quadtree


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

        Dim Sounds As New List(Of SoundPlayer)

        Dim gQuadTree As QuadTree(Of Integer)





        Private Sub gLoadContent()

            gTextures.Add(LoadTexture("Resources/Square.png")) '0
            gTextures.Add(LoadTexture("Resources/ship.png")) '1
            gTextures.Add(LoadTexture("Resources/BackgroundOverlay.png")) '2
            gTextures.Add(LoadTexture("Resources/background.png")) '3
            gTextures.Add(LoadTexture("Resources/Cursor.png")) '4
            gTextures.Add(LoadTexture("Resources/Skull.png")) '5
            gTextures.Add(LoadTexture("Resources/Title.png")) '6
            gTextures.Add(LoadTexture("Resources/Overdraw.png")) '6

            'gViewport.WindowBorder = WindowBorder.Fixed

            gViewport.CursorVisible = True

            gViewport.Title = "Invaders Must Die!"
            gViewport.WindowState = WindowState.Maximized

            'Enable OpenGL Alpha Blending
            GL.Enable(EnableCap.Blend)
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)

            Sounds.Add(New SoundPlayer("Resources/Music.wav"))
            Sounds.Add(New SoundPlayer("Resources/Lazer.wav"))
            For Each s As SoundPlayer In Sounds
                s.Load()
            Next

            'Add the Player Object at 0
            GameObjects.Add(New PlayerShip(New Vector2(0, 0), New Vector2(48, 48), New Integer() {gTextures(1).ID}))

            'Add some testing AI objects, square enemys
            For i As Integer = 1 To 0
                GameObjects.Add(New SquareEnemy(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {0, 5}))
                GameObjects(i).iTextureIdentification = New Integer() {gTextures(0).ID, gTextures(5).ID}
            Next

            'Sounds(0).PlayLooping()
            gGameState = GameState.Game
        End Sub

        Private Sub gUpdate(ByVal gGameTime As GameTime)
            Select Case gGameState
                Case GameState.Game
                    If GetAsyncKeyState(Keys.W) Then
                        GameObjects(0).fSpeed += GameObjects(0).fAcceleration * gGameTime.ElapsedGameTime
                    ElseIf GetAsyncKeyState(Keys.S) Then
                        GameObjects(0).fSpeed += GameObjects(0).fAcceleration * -1.0F * gGameTime.ElapsedGameTime
                    Else
                        GameObjects(0).fSpeed += GameObjects(0).fAcceleration * -0.5F * gGameTime.ElapsedGameTime
                    End If

                    If GetAsyncKeyState(Keys.LShiftKey) Then
                        GameObjects(0).bBoosting = True
                    Else
                        GameObjects(0).bBoosting = False
                    End If

                    If GetAsyncKeyState(Keys.Escape) Then
                        If gPauseState Then
                            gPauseState = False
                        Else
                            gPauseState = True
                        End If
                    End If

                    If OpenTK.Input.Mouse.GetState().LeftButton Then
                        'Sounds(1).Play()
                    End If

                    gQuadTree = New QuadTree(Of Integer)(New Vector2(0, 0), 0)

                    'Add all of the objects into the quadtree
                    For i As Integer = 0 To GameObjects.Count - 1
                        gQuadTree.Insert(GameObjects(i).vPosition, i)
                    Next

                    'Console.WriteLine(gQuadTree.GetWithin(GameObjects(0).vPosition, 100).Count.ToString())

                    'Iterate through all the games objects
                    GameObjects(0).Update(gGameTime, gRandom, gViewport.MousePosition)
                    For i As Integer = 1 To GameObjects.Count - 1
                        GameObjects(i).Update(gGameTime, gRandom, GameObjects(0).vPosition)
                    Next

                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    gViewport.ViewportPosition = GameMath.Lerp(gViewport.ViewportPosition, GameObjects(0).vPosition - New Vector2(gViewport.Width / 2, gViewport.Height / 2), gGameTime.ElapsedGameTime * 2)
                    gViewport.ViewportPosition.X = GameMath.Clamp(gViewport.ViewportPosition.X, -gViewport.ViewportBoundary, 1000 + gViewport.ViewportBoundary - gViewport.Width)
                    gViewport.ViewportPosition.Y = GameMath.Clamp(gViewport.ViewportPosition.Y, -gViewport.ViewportBoundary, 1000 + gViewport.ViewportBoundary - gViewport.Height)
                Case GameState.Menu

                Case GameState.Loading

            End Select
        End Sub



        'The primary draw function
        'This draw function has been setup for 2D Rendering using 3d primatives
        Private Sub gDraw_Main()
            Select Case gGameState
                Case GameState.Game
                    'Draw the background
                    Draw2d(gViewport, gTextures(3).ID, New Vector2(-gViewport.ViewportBoundary, -gViewport.ViewportBoundary) + gViewport.ViewportPosition / 2, gTextures(3).Size)
                    Draw2d(gViewport, gTextures(2).ID, New Vector2(-2, -2), gTextures(2).Size)

                    'Draw the game
                    'Iterate through all the games objects
                    For Each g As GameObject In GameObjects
                        'Select the type of object and do the appropriate update for it
                        g.Draw(gGameTime, gViewport)
                    Next

                    'Draw the hud
                    Draw2d(gViewport, gTextures(4).ID, gTextures(4).Size / -2 + gViewport.MousePosition, gTextures(4).Size)

                Case GameState.Menu
                    'Draw the background
                    Draw2d(gViewport, gTextures(3).ID, New Vector2(-gViewport.ViewportBoundary, -gViewport.ViewportBoundary) + gViewport.ViewportPosition / 2, gTextures(3).Size)

                    'Draw the hud
                    Draw2d(gViewport, gTextures(4).ID, gTextures(4).Size / -2 + gViewport.MousePosition, gTextures(4).Size)

                    'Draw the hud
                    Draw2d(gViewport, gTextures(0).ID, gTextures(4).Size / -2 + gViewport.MousePosition, gTextures(4).Size)

                Case GameState.Loading
                    'Nothing

            End Select
            'Begin Drawin the GDI+ Interface into the OpenGL Window
            SpriteBatch.DrawString("FPS:" + fps.iFrameRate.ToString() + vbNewLine + "Speed:" + GameObjects(0).fSpeed.ToString(), font, New SolidBrush(Color.FromArgb(0, 255, 0)), New Point(0, 0))
        End Sub

        Dim font As New Font(FontFamily.GenericSansSerif, 8.0F)


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

