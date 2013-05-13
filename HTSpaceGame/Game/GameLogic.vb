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

        Enum ParticleLevel
            None = 0
            Low = 1
            Medium = 2
            High = 3
            Ultra = 4

        End Enum
        Dim gGameState As GameState = GameState.Loading
        Dim gParticleLevel As ParticleLevel = 1
        Dim gPauseState As Boolean = False

        Dim gQuadTree As QuadTree(Of Integer)

        Private Sub gLoadContent()

            gTextures.Add(LoadTexture("Resources/Textures/Entity/Spinner/Spinner_PartB.png")) '0
            gTextures.Add(LoadTexture("Resources/Textures/Entity/Arrow\ship.png")) '1
            gTextures.Add(LoadTexture("Resources/Textures/Misc/overlay.png")) '2
            gTextures.Add(LoadTexture("Resources/Textures/Misc/background.png")) '3
            gTextures.Add(LoadTexture("Resources/Textures/Misc/cursor.png")) '4
            gTextures.Add(LoadTexture("Resources/Textures/Entity/Spinner/Spinner_PartA.png")) '5
            gTextures.Add(LoadTexture("Resources/Textures/Misc/logo.png")) '6
            gTextures.Add(LoadTexture("Resources/Textures/Engine/overdraw1x1.png")) '7
            gTextures.Add(LoadTexture("Resources/Textures/Entity/Explosion/Explosion_PartA.png")) '8
            gTextures.Add(LoadTexture("Resources/Textures/Entity/Revolver/Revolver_PartC.png")) '9
            gTextures.Add(LoadTexture("Resources/Textures/Entity/Revolver/Revolver_PartB.png")) '10
            gTextures.Add(LoadTexture("Resources/Textures/Entity/Revolver/Revolver_PartA.png")) '11

            'gViewport.WindowBorder = WindowBorder.Fixed

            gViewport.CursorVisible = True

            gViewport.Title = "Invaders Must Die!"
            gViewport.WindowState = WindowState.Maximized

            'Enable OpenGL Alpha Blending
            GL.Enable(EnableCap.Blend)
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)

            'Add the Player Object at 0



            'Sounds(0).PlayLooping()
            gGameState = GameState.Menu
        End Sub
        Dim fTimer As Single
        Dim pSpawnTime As Single
        Dim pSpawnTimeMax As Single


        Private Sub gUpdate(ByVal gGameTime As GameTime)
            gViewport.Update(gGameTime)
            If GetKeyPress(Key.Number1) Then
                gViewport.ViewportTargetSize = New Vector2(800, 800)
            End If
            If GetKeyPress(Key.Number2) Then
                gViewport.ViewportTargetSize = New Vector2(1600, 1600)
            End If
            If GetKeyPress(Key.Number3) Then
                gViewport.ViewportTargetSize = New Vector2(2400, 2400)
            End If



            If GetKeyPress(Key.Number4) Then
                gParticleLevel = ParticleLevel.None
            End If
            If GetKeyPress(Key.Number5) Then
                gParticleLevel = ParticleLevel.Low
            End If
            If GetKeyPress(Key.Number6) Then
                gParticleLevel = ParticleLevel.Ultra
            End If
            If GetAsyncKeyState(Keys.Q) Then
                createParticle(ParticleType.Firework)
            End If
            If GetKeyPress(Key.E) Then
                createParticle(New Vector2(500, 500), ParticleType.Nova)
            End If
            If GetKeyPress(Keys.F) Then
                If gViewport.WindowState = WindowState.Normal Then
                    gViewport.WindowState = WindowState.Fullscreen
                Else
                    gViewport.WindowState = WindowState.Normal
                End If
            End If

            If GetKeyPress(Key.H) Then
                'Add some testing AI objects, square enemys
                For iii As Integer = 1 To 5
                    GameObjects.Add(New Spinner(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(0).ID, gTextures(5).ID}))
                Next
                For iii As Integer = 1 To 2
                    GameObjects.Add(New Revolver(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(9).ID, gTextures(10).ID, gTextures(11).ID}))
                Next
            End If



            Select Case gGameState
                Case GameState.Game

                    Dim b As Boolean
                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    If GameObjects.Count > 0 Then
                        If GameObjects(0).eEntity = GameObject.ObjectType.Player Then
                            gViewport.ViewportPosition = (GameObjects(0).vPosition * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale

                            pSpawnTime += gGameTime.ElapsedGameTime

                            'Run all player code below!!!
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

                        Else
                            b = True
                            GameObjects.Clear()
                            gViewport.ViewportPosition = GameMath.Lerp(gViewport.ViewportPosition, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, gGameTime.ElapsedGameTime * 2)
                            'Something else or dead
                            If pSpawnTime > pSpawnTimeMax Then
                                pSpawnTimeMax = pSpawnTime
                            End If
                            pSpawnTime -= gGameTime.ElapsedGameTime
                            If pSpawnTime < -3 Then
                                SpawnPlayer()
                            End If
                        End If
                    Else
                        b = True
                    End If
                    If b Then
                        GameObjects.Clear()
                        gViewport.ViewportPosition = GameMath.Lerp(gViewport.ViewportPosition, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, gGameTime.ElapsedGameTime * 2)
                        'Something else or dead
                        If pSpawnTime > pSpawnTimeMax Then
                            pSpawnTimeMax = pSpawnTime
                        End If
                        pSpawnTime -= gGameTime.ElapsedGameTime
                        If pSpawnTime < -3 Then
                            SpawnPlayer()
                            pSpawnTime = 0.0F
                        End If
                    End If
                    Console.WriteLine(pSpawnTime)
                    Console.WriteLine(GameObjects.Count)

                    'Iterate through all the games objects
                    For Each g As GameObject In GameObjects.ToArray
                        Select Case g.eEntity
                            Case GameObject.ObjectType.Player
                                g.Update(gGameTime, gRandom, gViewport.MousePosition)
                                If g.fLifetime >= g.fLifetimeMax Then
                                    GameObjects.Remove(g)
                                End If
                            Case GameObject.ObjectType.Enemy
                                g.Update(gGameTime, gRandom, GameObjects(0).vPosition)
                                If g.fLifetime >= g.fLifetimeMax Then
                                    GameObjects.Remove(g)
                                End If
                            Case GameObject.ObjectType.Bullet
                                g.Update(gGameTime, gRandom, GameObjects(0).vPosition)
                                If g.fLifetime >= g.fLifetimeMax Then
                                    GameObjects.Remove(g)
                                End If
                        End Select
                    Next

                    gQuadTree = New QuadTree(Of Integer)(New Vector2(0, 0), 0)

                    'Iterate through all of the games objects
                    For i As Integer = 1 To GameObjects.Count - 1
                        'If the object is not an "Other" object
                        If Not GameObjects(i).eEntity = GameObject.ObjectType.Other Then
                            gQuadTree.Insert(GameObjects(i).vPosition, i)
                        End If
                    Next

                    Try
                        'Iterate through all of the games objects
                        For Each g As GameObject In GameObjects.ToArray
                            Select Case g.eEntity
                                Case GameObject.ObjectType.Player
                                    Dim CollisionIds() As Integer = gQuadTree.GetWithin(g.vPosition, 100).ToArray
                                    For i As Integer = 0 To CollisionIds.Length - 1
                                        If GameObjects(CollisionIds(i)).eEntity = GameObject.ObjectType.Enemy Then
                                            If GameMath.Vector2Distance(GameObjects(CollisionIds(i)).vPosition, g.vPosition) < GameObjects(CollisionIds(i)).vSize.X - 10 Then
                                                'Destroy the enemy and the bullet
                                                For Each gg As GameObject In GameObjects.ToArray
                                                    createParticle(gg.vPosition, ParticleType.Firework)
                                                Next
                                                Dim v2 As Vector2 = g.vPosition
                                                GameObjects.Clear()
                                                pSpawnTime = 0.0F
                                                createParticle(v2, ParticleType.PlayerExplosionFirework)
                                            End If
                                        End If
                                    Next
                                Case GameObject.ObjectType.Bullet
                                    Dim CollisionIds() As Integer = gQuadTree.GetWithin(g.vPosition, g.vSize.X + g.vSize.Y).ToArray
                                    For i As Integer = 0 To CollisionIds.Length - 1
                                        If GameObjects(CollisionIds(CollisionIds(i))).eEntity = GameObject.ObjectType.Enemy Then
                                            If GameMath.Vector2Distance(GameObjects(CollisionIds(i)).vPosition, g.vPosition) > 100 Then
                                                'Destroy the enemy and the bullet
                                                Console.WriteLine("Enemy go boom")
                                                'GameObjects.Clear()
                                                'gGameState = GameState.Menu
                                            End If
                                        End If
                                    Next
                            End Select
                        Next
                    Catch ex As Exception

                    End Try

                Case GameState.Menu

                    fTimer += gGameTime.ElapsedGameTime
                    Dim f As Single = 1.0F / 30
                    While fTimer > f
                        createParticle(ParticleType.Menu)

                        'GameObjects.Add(New Explosion(New Vector2(500, 500), New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gParticleLevel))

                        fTimer -= f
                    End While


                    'Iterate through all the games objects
                    For Each g As GameObject In GameObjects.ToArray
                        g.Update(gGameTime, gRandom, gViewport.ViewportPosition)
                        If g.fLifetime >= g.fLifetimeMax Then
                            GameObjects.Remove(g)
                        End If
                    Next

                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    gViewport.ViewportPosition = (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale

                    If GetAsyncKeyState(Keys.D0) Then
                        If Not GameObjects.Count = 0 Then
                            GameObjects(0) = New PlayerShip(New Vector2(500, 500), New Vector2(48, 48), New Integer() {gTextures(1).ID})
                        Else
                            GameObjects.Add(New PlayerShip(New Vector2(500, 500), New Vector2(48, 48), New Integer() {gTextures(1).ID}))
                        End If

                        'Add some testing AI objects, square enemys
                        For i As Integer = 1 To 5
                            GameObjects.Add(New Spinner(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(0).ID, gTextures(5).ID}))
                        Next
                        For i As Integer = 1 To 2
                            GameObjects.Add(New Revolver(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(9).ID, gTextures(10).ID, gTextures(11).ID}))
                        Next
                        gGameState = GameState.Game
                    End If

                Case GameState.Loading

            End Select
        End Sub

        'This is a function called from the Update Thread of the Particle Update thread to iterate through all of the particles.
        'If this is run from the (updateThreadParticle) then the particle count is allowed to be between Off-Ultra else its manually set between Off-Low
        Private Sub gUpdateEffects(ByVal gGameTime As GameTime)
#If Not DEBUG Then
            try
#End If
            'Iterate through all the games objects [Particles]
            For Each g As GameObject In gParticles.ToArray
                g.Update(gGameTime, gRandom, gViewport.ViewportPosition)
                If g.fLifetime >= g.fLifetimeMax Then
                    gParticles.Remove(g)
                End If
            Next
#If Not DEBUG Then
            Catch ex As Exception
            Console.WriteLine(ex.Message)
            End Try
#End If
        End Sub

        'The primary draw function
        'This draw function has been setup for 2D Rendering using 3d primatives
        Private Sub gDraw_Main()
            Select Case gGameState
                Case GameState.Game
                    'Draw the background
                    Draw2d(gViewport, gTextures(3).ID, gViewport.ViewportPosition / 2 - New Vector2(1000, 1000), gTextures(3).Size * 2)
                    Draw2d(gViewport, gTextures(2).ID, New Vector2(-2, -2), gTextures(2).Size)

                    'Draw the game
                    'Iterate through all the games objects
                    Try
                        For Each g As GameObject In GameObjects.ToArray
                            'Select the type of object and do the appropriate update for it
                            g.Draw(gGameTime, gViewport)
                        Next
                    Catch ex As Exception
                    End Try

                    'Draw the hud
                    Draw2d(gViewport, gTextures(4).ID, gTextures(4).Size / -2 + gViewport.MousePosition, gTextures(4).Size)
                Case GameState.Menu
                    'Draw the background
                    GL.ClearColor(Color.Black)
                    'Draw2d(gViewport, gTextures(3).ID, gViewport.ViewportPosition / 2 - New Vector2(1000, 1000), gTextures(3).Size * 2)

                    'Draw the game
                    'Iterate through all the games objects [Entity]
                        For Each g As GameObject In GameObjects.ToArray
                            'Select the type of object and do the appropriate update for it
                            g.Draw(gGameTime, gViewport)
                        Next

                        'Draw the hud
                        Draw2d(gViewport, gTextures(4).ID, gTextures(4).Size / -2 + gViewport.MousePosition, gTextures(4).Size)

                Case GameState.Loading
                        'Nothing

            End Select


            'Iterate through all the games objects [Particles]
            For Each g As GameObject In gParticles.ToArray
                'Select the type of object and do the appropriate update for it
                If Not g.Equals(Nothing) Or g.Equals(vbNull) Then
                    g.Draw(gGameTime, gViewport)
                End If

            Next


            'Overdraw the device with a 1x1 Pixel. Useful for reducing artifacts on the last draw call.
            Draw2d(gViewport, gTextures(6).ID, New Vector2(Single.MaxValue, Single.MaxValue), New Vector2(1, 1))

        End Sub

        Dim font As New Font(FontFamily.GenericSansSerif, 8.0F)

        Public Sub SpawnPlayer()
            If Not GameObjects.Count = 0 Then
                GameObjects(0) = New PlayerShip(New Vector2(500, 500), New Vector2(48, 48), New Integer() {gTextures(1).ID})
            Else
                GameObjects.Add(New PlayerShip(New Vector2(500, 500), New Vector2(48, 48), New Integer() {gTextures(1).ID}))
            End If
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


        Dim gParticles As New List(Of GameObject)
        Enum ParticleType
            Firework = 0
            PlayerExplosionFirework = 1
            Nova = 2
            Menu = 3
        End Enum
        Private Sub createParticle(ByVal _ParticleType As ParticleType)
            createParticle(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), _ParticleType)
        End Sub

        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType)
            Select Case _ParticleType
                Case ParticleType.Firework
                    If bEffectThread Then
                        'If running from the bParticleThread
                        'Create a new Explosion Particle with whatever settings are supplied
                        gParticles.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gParticleLevel))
                    Else
                        'If Not running from the bParticleThread
                        'Create a new Explosion Particle with whatever settings are supplied with the exception the maximum particles can only be on Low
                        gParticles.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, GameMath.ClampI(gParticleLevel, 0, ParticleLevel.Low)))
                    End If
                Case ParticleType.PlayerExplosionFirework
                    Dim e As New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gParticleLevel * 20, New Color4(1.0F, 1.0F, 1.0F, 0))
                    e.fLifetimeMax = 5
                    gParticles.Add(e)
                Case ParticleType.Nova
                    Dim e As New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gParticleLevel * 20)
                    e.fLifetimeMax = 9
                    gParticles.Add(e)
                Case ParticleType.Menu
                    Dim e As New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, 0.0F * gParticleLevel)
                    e.fLifetimeMax = 5
                    gParticles.Add(e)
            End Select
        End Sub

        Private Function GetKeyPress(ByVal _Key As Key) As Boolean
            If gCurrentKeyboardState.IsKeyDown(_Key) And Not gPreviousKeyboardState.IsKeyDown(_Key) Then
#If DEBUG And DEBUGNOTIFY Then
                    System.Diagnostics.Debug.WriteLine("Key Pressed: " + _Key.ToString())
#End If
                Return True
            End If
            Return False
        End Function
    End Class
End Namespace

