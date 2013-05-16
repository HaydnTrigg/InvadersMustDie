
#Region "Imports"

'Import OpenTK
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input

'Import the Isotope Framework
Imports Isotope
Imports Isotope.Library
Imports Isotope.Library.DrawSprite

'Import System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging

#End Region
Namespace Isotope
    'Create a partial class to extent on the Isotope.Game class
    Partial Public Class Game
#Region "Enumeration & Structures"

        'An enumeration of the current game state
        Enum GameState
            Loading = 0
            Menu = 1
            Game = 2
        End Enum

        'An enumeration of the current game effect level
        Enum EffectLevel
            None = 0
            Low = 1
            Medium = 2
            High = 3
            Ultra = 4
        End Enum

        'An enumeration of a type of particle
        Enum ParticleType
            Firework = 0
            PlayerExplosionFirework = 1
            Nova = 2
            Menu = 3
        End Enum

        'A structure to hold Texture Identitifaction Information
        Structure TextureID
            Public Width As Integer, Height As Integer, ID As Integer
            ReadOnly Property Size As Vector2
                Get
                    Return New Vector2(Width, Height)
                End Get
            End Property
        End Structure

#End Region
#Region "Variables & Properties"

        Public cColors() As Color4 = New Color4() {
            New Color4(1.0F, 0.0F, 0.0F, 1.0F),
            New Color4(0.0F, 1.0F, 0.0F, 1.0F),
            New Color4(0.0F, 0.5803922F, 1.0F, 1.0F),
            New Color4(1.0F, 0.3529412F, 0.20784314F, 1.0F),
            New Color4(255, 0.0F, 0.8627451F, 1.0F),
            New Color4(0.698039234F, 0.0F, 1.0F, 1.0F),
            New Color4(1.0F, 0.847058833F, 0.0F, 1.0F)
        }

        'The current GameState
        Dim gGameState As GameState = GameState.Menu

        'The current Level of Effects
        Dim gEffectLevel As EffectLevel = 1

        'The games QuadTree
        Dim gQuadTree As QuadTree(Of Integer)

        'All of the Entity objects currently in the game
        Dim gGameEntitys As New List(Of GameObject)

        'All of the Effect objects currently in the game
        Dim gGameEffects As New List(Of GameObject)

        'Stores information of the loaded textures
        Dim gTextures As New List(Of TextureID)

        'The time between spawning a new effect
        Dim fEffectSpawn As Single

        'The players current spawn time
        Dim pSpawnTime As Single
        'The players maximum spawn time
        Dim pSpawnTimeMax As Single

        'The controll state
        Dim gControllState

        'Controlls how fast the player can shoot. 8[1/3] times per second
        Dim fRefire As Single
        Dim fRefireTime As Single = 0.12F

        'Controlls how long the intro goes for
        Dim fIntroFadeIn As Single = 3.0F
        Dim fIntroFadeOut As Single = 3.0F
        Dim fIntroTotalTime As Single = 10.0F

#End Region
#Region "Main Functions"

        Private Sub gLoadContent()

            gTextures.Add(LoadTexture("Content/Textures/Entity/Spinner/Spinner_PartB.png")) '0
            gTextures.Add(LoadTexture("Content/Textures/Entity/Arrow\ship.png")) '1
            gTextures.Add(LoadTexture("Content/Textures/Misc/overlay.png")) '2
            gTextures.Add(LoadTexture("Content/Textures/Misc/background.png")) '3
            gTextures.Add(LoadTexture("Content/Textures/Misc/cursor.png")) '4
            gTextures.Add(LoadTexture("Content/Textures/Entity/Spinner/Spinner_PartA.png")) '5
            gTextures.Add(LoadTexture("Content/Textures/Misc/logo.png")) '6
            gTextures.Add(LoadTexture("Content/Textures/Engine/overdraw1x1.png")) '7
            gTextures.Add(LoadTexture("Content/Textures/Entity/Explosion/Explosion_PartA.png")) '8
            gTextures.Add(LoadTexture("Content/Textures/Entity/Revolver/Revolver_PartC.png")) '9
            gTextures.Add(LoadTexture("Content/Textures/Entity/Revolver/Revolver_PartB.png")) '10
            gTextures.Add(LoadTexture("Content/Textures/Entity/Revolver/Revolver_PartA.png")) '11
            gTextures.Add(LoadTexture("Content/Textures/Misc/Loading.png")) '12
            gTextures.Add(LoadTexture("Content/Textures/Entity/Bullet/Bullet_PartA.png")) '13

            'gViewport.WindowBorder = WindowBorder.Fixed

            gViewport.CursorVisible = True

            gViewport.Title = "Invaders Must Die!"
            gViewport.WindowState = WindowState.Maximized

            'Enable OpenGL Alpha Blending
            GL.Enable(EnableCap.Blend)
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)

            gGameState = GameState.Loading

            'Create the controll game state for the Controll Objects
            gControllState = New GameControll.ControllGameState
            gControllState.gPreviousKeyboardState = gPreviousKeyboardState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState

        End Sub

        Private Sub gUpdate(ByVal gGameTime As GameTime)
            'Update the Viewport
            gViewport.Update(gGameTime)

            'Update the ControllState
            gControllState.gPreviousKeyboardState = gPreviousKeyboardState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState


            'Update all of the KeyDown and KeyPress Events
            If GetKeyPress(Key.Number4) Then
                gEffectLevel = GameMath.ClampInteger(gEffectLevel - 1, 0, EffectLevel.Ultra)
            End If
            If GetKeyPress(Key.Number5) Then
                gEffectLevel = GameMath.ClampInteger(gEffectLevel + 1, 0, EffectLevel.Ultra)
            End If
            If gCurrentKeyboardState.IsKeyDown(Key.Q) Then
                createParticle(ParticleType.Firework)
            End If
            If GetKeyPress(Key.E) Then
                createParticle(New Vector2(500, 500), ParticleType.Nova)
            End If
            If GetKeyPress(Key.R) Then
                gGameState = GameState.Game
            End If
            If GetKeyPress(Key.H) Then
                'Add 5 Spinners
                For iii As Integer = 1 To 500
                    gGameEntitys.Add(New Spinner(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(0).ID, gTextures(5).ID}))
                Next
                'Add 2 Revolvers
                For iii As Integer = 1 To 2
                    gGameEntitys.Add(New Revolver(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(9).ID, gTextures(10).ID, gTextures(11).ID}))
                Next
            End If

            'Determine how the game should update based on its current state
            Select Case gGameState
                Case GameState.Game
                    fRefire += gGameTime.DeltaTime
                    If gCurrentMouseState.LeftButton Then
                        Console.WriteLine("MOUSE GOOD")
                        If fRefire > fRefireTime Then
                            Console.WriteLine("TIME GOOD")
                        End If

                    End If
                    If gCurrentMouseState.LeftButton And fRefire > fRefireTime Then
                        gGameEntitys.Add(New Spinner(New Vector2(gRandom.NextDouble * 800 + 100, gRandom.NextDouble * 800 + 100), New Vector2(48, 48), gRandom, New Integer() {gTextures(0).ID, gTextures(5).ID}))
                        fRefire -= fRefireTime
                    End If
                    Dim b As Boolean
                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    If gGameEntitys.Count > 0 Then
                        If gGameEntitys(0).eEntity = GameObject.ObjectType.Player Then
                            gViewport.Position = (gGameEntitys(0).vPosition * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale

                            pSpawnTime += gGameTime.DeltaTime

                            'Run all player code below!!!
                            If gCurrentKeyboardState.IsKeyDown(Key.W) Then
                                gGameEntitys(0).iAccelerating = 1
                            ElseIf gCurrentKeyboardState.IsKeyDown(Key.S) Then
                                gGameEntitys(0).iAccelerating = -1
                            Else
                                gGameEntitys(0).iAccelerating = 0
                            End If
                            If gCurrentKeyboardState.IsKeyDown(Key.ShiftLeft) Then
                                gGameEntitys(0).bBoosting = True
                            Else
                                gGameEntitys(0).bBoosting = False
                            End If



                        Else
                            b = True
                            gGameEntitys.Clear()
                            gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, gGameTime.DeltaTime * 2)
                            'Something else or dead
                            If pSpawnTime > pSpawnTimeMax Then
                                pSpawnTimeMax = pSpawnTime
                            End If
                            pSpawnTime -= gGameTime.DeltaTime
                            If pSpawnTime < -3 Then
                                SpawnPlayer()
                            End If
                        End If
                    Else
                        b = True
                    End If
                    If b Then
                        gGameEntitys.Clear()
                        gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, gGameTime.DeltaTime * 2)
                        'Something else or dead
                        If pSpawnTime > pSpawnTimeMax Then
                            pSpawnTimeMax = pSpawnTime
                        End If
                        pSpawnTime -= gGameTime.DeltaTime
                        If pSpawnTime < -3 Then
                            SpawnPlayer()
                            pSpawnTime = 0.0F
                        End If
                    End If

                    'Create a new Quadtree to calculate the Collisions
                    gQuadTree = New QuadTree(Of Integer)(New Vector2(0, 0), 0)

                    '"Try" and Iterate through all of the objects and Update
                    Try
                        Dim gGameObjectsTemp_1() As GameObject = gGameEntitys.ToArray
                        For i As Integer = 0 To gGameEntitys.Count - 1
                            Dim g As GameObject = gGameObjectsTemp_1(i)
                            If g.eEntity.Equals(GameObject.ObjectType.Player) Then
                                g.Update(gGameTime, gRandom, gViewport.MousePosition)

                            Else
                                g.Update(gGameTime, gRandom, gGameEntitys(0).vPosition)
                            End If
                            If g.fLifespan >= g.fLifespanMax Then
                                gGameEntitys.Remove(g)
                            Else
                                gQuadTree.Insert(gGameEntitys(i).vPosition, i)
                            End If
                        Next
                    Catch ex As Exception
#If DEBUG Then
                        Console.Write(ex.Message)
#End If
                        gQuadTree = Nothing
                    End Try

                    If Not IsNothing(gQuadTree) Then
                        Try
                            'Iterate through all of the games objects and check their collisions
                            For Each g As GameObject In gGameEntitys.ToArray
                                Select Case g.eEntity
                                    Case GameObject.ObjectType.Player
                                        Dim CollisionIds() As Integer = gQuadTree.GetWithin(g.vPosition, 100).ToArray
                                        For i As Integer = 0 To CollisionIds.Length - 1
                                            If gGameEntitys(CollisionIds(i)).eEntity = GameObject.ObjectType.Enemy Then
                                                If GameMath.Vector2Distance(gGameEntitys(CollisionIds(i)).vPosition, g.vPosition) < gGameEntitys(CollisionIds(i)).vSize.X - 10 Then
                                                    'Destroy the enemy and the bullet
                                                    For Each gg As GameObject In gGameEntitys.ToArray
                                                        createParticle(gg.vPosition, ParticleType.Firework)
                                                    Next
                                                    Dim v2 As Vector2 = g.vPosition
                                                    gGameEntitys.Clear()
                                                    pSpawnTime = 0.0F
                                                    createParticle(v2, ParticleType.PlayerExplosionFirework)
                                                End If
                                            End If
                                        Next
                                    Case GameObject.ObjectType.Bullet
                                        Dim CollisionIds() As Integer = gQuadTree.GetWithin(g.vPosition, g.vSize.X + g.vSize.Y).ToArray
                                        For i As Integer = 0 To CollisionIds.Length - 1
                                            If gGameEntitys(CollisionIds(CollisionIds(i))).eEntity = GameObject.ObjectType.Enemy Then
                                                If GameMath.Vector2Distance(gGameEntitys(CollisionIds(i)).vPosition, g.vPosition) > 100 Then
                                                    'Destroy the enemy and the bullet
                                                    Console.WriteLine("Enemy go boom")
                                                    'gGameEntitys.Clear()
                                                    'gGameState = GameState.Menu
                                                End If
                                            End If
                                        Next
                                End Select
                            Next
                        Catch ex As Exception
#If DEBUG Then
                            Console.Write(ex.Message)
#End If
                        End Try
                    End If

                Case GameState.Menu
                    'Set the Viewport to full screen.
                    gViewport.ViewportRealSize = New Vector2(1000, 1000)
                    'Center the Viewport
                    gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, gGameTime.DeltaTime * 2)

                    gGameEntitys.Clear()

                    'Spawning random fireworks
                    Dim f As Single = 1.0F / 5.0F * gEffectLevel
                    fEffectSpawn += gGameTime.DeltaTime
                    If fEffectSpawn > f Then
                        createParticle(ParticleType.Firework)
                        f = 0
                    End If

                    'Iterate through all the games objects
                    For Each g As GameObject In gGameEntitys.ToArray
                        g.Update(gGameTime, gRandom, gViewport.Position)
                        If g.fLifespan >= g.fLifespanMax Then
                            gGameEntitys.Remove(g)
                        End If
                    Next

                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    gViewport.Position = (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale

                Case GameState.Loading
                    If gGameTime.TotalTime > fIntroTotalTime + 0.5F Then
                        gGameState = GameState.Menu
                    Else
                        gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, gGameTime.DeltaTime * 2)
                        gViewport.ViewportRealSize = New Vector2(gViewport.Width, gViewport.Height)
                    End If
            End Select
        End Sub


        'This is a function called from the Update Thread of the Particle Update thread to iterate through all of the particles.
        'If this is run from the (updateThreadParticle) then the particle count is allowed to be between Off-Ultra else its manually set between Off-Low
        Private Sub gUpdateEffects(ByVal gGameTime As GameTime)
            Try
                'Iterate through all the games objects [Particles]
                For Each g As GameObject In gGameEffects.ToArray
                    If Not g.Equals(Nothing) Or g.Equals(vbNull) Then
                        g.Update(gGameTime, gRandom, gViewport.Position)
                        If g.fLifespan >= g.fLifespanMax Then
                            gGameEffects.Remove(g)
                        End If
                    End If
                Next
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
        End Sub

        'The primary draw function
        'This draw function has been setup for 2D Rendering using 3d primatives
        Private Sub gDraw()
            'Overdraw the device with a 1x1 Pixel. Useful for reducing artifacts on the last draw call.
            Draw2D(gViewport, gTextures(6).ID, New Vector2(Single.MaxValue, Single.MaxValue), New Vector2(1, 1))

            Select Case gGameState
                Case GameState.Game
                    'Draw the background
                    Draw2D(gViewport, gTextures(3).ID, gViewport.Position / 2 - New Vector2(1000, 1000), gTextures(3).Size * 1.5F)
                    Draw2D(gViewport, gTextures(2).ID, New Vector2(-2, -2), gTextures(2).Size)

                    'Draw the game
                    DrawParticles()
                    DrawEntitys()

                    'Draw the hud
                    Draw2D(gViewport, gTextures(4).ID, New Vector2(-12, -12) / gViewport.ViewportScale + gViewport.MousePosition, New Vector2(24, 24) / gViewport.ViewportScale)

                Case GameState.Menu
                    'Draw the game
                    DrawParticles()
                    'Draw the buttons

                    'Draw the background
                    Draw2D(gViewport, gTextures(6).ID, New Vector2(500, 600), gTextures(6).Size)
                    'Draw2d(gViewport, gTextures(3).ID, gViewport.Position / 2 - New Vector2(1000, 1000), gTextures(3).Size * 2)

                    'Draw the hud
                    Draw2D(gViewport, gTextures(4).ID, New Vector2(-12, -12) / gViewport.ViewportScale + gViewport.MousePosition, New Vector2(24, 24) / gViewport.ViewportScale)

                Case GameState.Loading
                    'Draw Intro Screen
                    If gGameTime.TotalTime > fIntroTotalTime - fIntroFadeIn Then
                        GL.Color4(New Color4(1.0F, 1.0F, 1.0F, 1.0F - CType((gGameTime.TotalTime - (fIntroTotalTime - fIntroFadeIn)) / fIntroFadeOut, Single)))
                    Else
                        GL.Color4(New Color4(1.0F, 1.0F, 1.0F, CType(gGameTime.TotalTime / fIntroFadeIn, Single) - 1.0F))
                    End If
                    'Fill the playing area with image.
                    Draw2D(gViewport, gTextures(12).ID, New Vector2(0, 0), New Vector2(1000, 1000))
                    GL.Color4(Color4.White)
            End Select

            'Overdraw the device with a 1x1 Pixel. Useful for reducing artifacts on the last draw call.
            Draw2D(gViewport, gTextures(6).ID, New Vector2(Single.MaxValue, Single.MaxValue), New Vector2(1, 1))
        End Sub

#End Region
#Region "Secondary Functions"

        'Draws the particles
        Public Sub DrawParticles()
            'Iterate through all the games objects [Particles]
            Try
                For Each g As GameObject In gGameEffects.ToArray
                    'Select the type of object and do the appropriate update for it
                    If Not g.Equals(Nothing) Or g.Equals(vbNull) Then
                        g.Draw(gGameTime, gViewport)
                    End If
                Next
            Catch ex As Exception
#If DEBUG Then
                Console.WriteLine(ex.Message)
#End If
            End Try
            'After drawing all of the particles reset the Drawing Color to White
            GL.Color4(1.0F, 1.0F, 1.0F, 1.0F)
        End Sub

        'Draw the entitys
        Public Sub DrawEntitys()
            'Iterate through all the games objects
            Try
                For Each g As GameObject In gGameEntitys.ToArray
                    'Select the type of object and do the appropriate update for it
                    g.Draw(gGameTime, gViewport)
                Next
            Catch ex As Exception
            End Try
        End Sub

        'Spawns the Player is the client has died or is beginning the game.
        Public Sub SpawnPlayer()
            If Not gGameEntitys.Count = 0 Then
                gGameEntitys(0) = New PlayerShip(New Vector2(500, 500), New Vector2(48, 48), New Integer() {gTextures(1).ID})
            Else
                gGameEntitys.Add(New PlayerShip(New Vector2(500, 500), New Vector2(48, 48), New Integer() {gTextures(1).ID}))
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

            'Setup filtering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))

            Return tid
        End Function

        Private Function GetKeyPress(ByVal _Key As Key) As Boolean
            If gCurrentKeyboardState.IsKeyDown(_Key) And Not gPreviousKeyboardState.IsKeyDown(_Key) Then
                Return True
            End If
            Return False
        End Function

        Private Sub createParticle(ByVal _ParticleType As ParticleType)
            createParticle(New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50), _ParticleType, CType(Math.Round(gRandom.NextDouble * 3, 0), Explosion.ParticleAlgorithm))
        End Sub

        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType)
            createParticle(_Position, _ParticleType, CType(Math.Round(gRandom.NextDouble * 3, 0), Explosion.ParticleAlgorithm))
        End Sub

        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType, ByVal _Algorithm As Explosion.ParticleAlgorithm)
                Select Case _ParticleType
                Case ParticleType.Firework
                    If gGameEffects.Count < gEffectLevel * 10 Then
                        If bUsingEffectThread Then
                            'If running from the bParticleThread
                            'Create a new Explosion Particle with whatever settings are supplied
                            gGameEffects.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gEffectLevel, ZeroColorAlpha(RandomColor4()), 2.0F, _Algorithm))
                        Else
                            'If Not running from the bParticleThread
                            'Create a new Explosion Particle with whatever settings are supplied with the exception the maximum particles can only be on Low
                            gGameEffects.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, GameMath.ClampInteger(gEffectLevel, 0, EffectLevel.Low), ZeroColorAlpha(RandomColor4()), 2.0F, _Algorithm))
                        End If
                    End If
                Case ParticleType.PlayerExplosionFirework
                    gGameEffects.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gEffectLevel * 20, New Color4(1.0F, 1.0F, 1.0F, 0.0F), 5.0F, Explosion.ParticleAlgorithm.Spread))
                Case ParticleType.Nova
                    gGameEffects.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, gEffectLevel * 20, ZeroColorAlpha(RandomColor4()), 9.0F, Explosion.ParticleAlgorithm.Spread))
                Case ParticleType.Menu
                    gGameEffects.Add(New Explosion(_Position, New Vector2(200, 200), gRandom, New Integer() {gTextures(8).ID}, 0.0F * gEffectLevel, ZeroColorAlpha(RandomColor4()), 5.0F, _Algorithm))
            End Select
        End Sub

        'Returns a RandomColor from the array cColors
        Private Function RandomColor4() As Color4
            Return cColors(GameMath.ClampInteger(gRandom.NextDouble() * cColors.Length - 1, 0, cColors.Length - 1))
        End Function
        'Returns a color with a premultiplied alpha value of 0
        Private Function ZeroColorAlpha(ByVal _Color As Color4) As Color4
            _Color.A = 0
            Return _Color
        End Function

#End Region
    End Class
End Namespace


