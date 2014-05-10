
#Region "Imports"

'Import OpenTK
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
Imports System.IO
Imports OpenTK.Audio
Imports OpenTK.Audio.OpenAL

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
            EnemyExplosion
            BulletExplosion
            PlayerExplosion
            RandomExplosion
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
        Dim gEffectLevel As EffectLevel = EffectLevel.Ultra

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
        Dim fSpawnTime As Single
        'The players maximum spawn time
        Dim fSpawnTimeMax As Single

        'The controll state
        Dim gControllState

        'Controlls how fast the player can shoot. 8[1/3] times per second
        Dim fRefire As Single
        Dim fRefireTime As Single = 0.12F

        'Controlls how fast the player can shoot. 8[1/3] times per second
        Dim fEnemySpawn As Single
        Dim fEnemySpawnTime As Single = 7.0F

        'Controlls how long the intro goes for
        Dim fIntroFadeIn As Single = 0.5F
        Dim fIntroFadeOut As Single = 0.5F
        Dim fIntroTotalTime As Single = 5.0F

        'Menu Buttons
        Dim btnStartGame As MenuButton
        Dim btnExitGame As MenuButton

#End Region
#Region "Main Functions"
        Private Sub gLoadContent()


            gTextures.Add(LoadTexture("Content/Textures/Entity/Spinner/Spinner_PartB.png")) '0
            gTextures.Add(LoadTexture("Content/Textures/Entity/Arrow/ship.png")) '1
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
            gTextures.Add(LoadTexture("Content/Textures/Misc/menustrip_play.png")) '14
            gTextures.Add(LoadTexture("Content/Textures/Misc/menustrip_exit.png")) '15
            gTextures.Add(LoadTexture("Content/Textures/Entity/Arrow/gun.png")) '16
            gTextures.Add(LoadTexture("Content/Textures/Entity/Pulser/Pulser_PartA.png")) '17
            gTextures.Add(LoadTexture("Content/Textures/Entity/Pulser/Pulser_PartB.png")) '18
            'gViewport.WindowBorder = WindowBorder.Fixed


            gViewport.CursorVisible = True

            gViewport.Title = "Invaders Must Die!"
            gViewport.WindowState = WindowState.Maximized

            'Enable OpenGL Alpha Blending
            GL.Enable(EnableCap.Blend)
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)

            gGameState = GameState.Menu

            'Create the controll game state for the Controll Objects
            gControllState = New GameControll.ControllGameState
            gControllState.gPreviousKeyboardState = gPreviousKeyboardState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState

            btnStartGame = New MenuButton(New Vector2(630.0F, 615.0F), gTextures(14).Size, New Integer() {gTextures(14).ID})
            btnExitGame = New MenuButton(New Vector2(630.0F, 655.0F), gTextures(14).Size, New Integer() {gTextures(15).ID})
        End Sub

        Private Sub gUpdate(ByVal delta As Single)
            'Update the Viewport
            gViewport.Update(delta)

            'Update the ControllState
            gControllState.gPreviousKeyboardState = gPreviousKeyboardState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gPreviousMouseState = gPreviousMouseState
            gControllState.gViewport = gViewport

            'Update all of the KeyDown and KeyPress Events
            If GetKeyPress(Key.Number4) Then
                gEffectLevel = GameMath.ClampInteger(gEffectLevel - 1, 1, EffectLevel.Ultra)
            End If

            If gCurrentKeyboardState.IsKeyDown(Key.L) Then
                gViewport.ViewportRealSize += New Vector2(1.0F, 1.0F)
            End If
            If gCurrentKeyboardState.IsKeyDown(Key.K) Then
                gViewport.ViewportRealSize += New Vector2(1.0F, 1.0F)
            End If
            If GetKeyPress(Key.Number5) Then
                gEffectLevel = GameMath.ClampInteger(gEffectLevel + 1, 0, EffectLevel.Ultra)
            End If
            If gCurrentKeyboardState.IsKeyDown(Key.Q) Then
                createParticle(ParticleType.RandomExplosion)
            End If
            If GetKeyPress(Key.E) Then
                createParticle(New Vector2(500, 500), ParticleType.PlayerExplosion)
            End If

            'Determine how the game should update based on its current state
            Select Case gGameState
                Case GameState.Game
                    'Set the Viewport relative resolution
                    'gViewport.ViewportRealSize = New Vector2(800.0F, 800.0F)

                    Dim b As Boolean
                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    If gGameEntitys.Count > 0 Then
                        If gGameEntitys(0).eEntity = GameObject.ObjectType.Player Then
                            gViewport.Position = (gGameEntitys(0).vPosition * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale

                            'Increment the players spawn time counter
                            fSpawnTime += delta

                            'Run all player code below!!!

                            'Is the client asking the player to accelerate, decelerate to idle.
                            gGameEntitys(0).iAccelerating = 0

                            Dim gPlayer As PlayerShip = gGameEntitys(0)

                            gPlayer.vAcceleration.X = 0
                            gPlayer.vAcceleration.Y = 0

                            If gCurrentKeyboardState.IsKeyDown(Key.W) Then
                                gGameEntitys(0).iAccelerating = 1
                                gPlayer.vAcceleration.Y -= 1
                            End If

                            If gCurrentKeyboardState.IsKeyDown(Key.S) Then
                                gGameEntitys(0).iAccelerating = 1
                                gPlayer.vAcceleration.Y += 1
                            End If

                            If gCurrentKeyboardState.IsKeyDown(Key.A) Then
                                gGameEntitys(0).iAccelerating = 1
                                gPlayer.vAcceleration.X -= 1
                            End If

                            If gCurrentKeyboardState.IsKeyDown(Key.D) Then
                                gGameEntitys(0).iAccelerating = 1
                                gPlayer.vAcceleration.X += 1

                            End If
                            gPlayer.vAcceleration = GameMath.NormalizeVector2(gPlayer.vAcceleration)

                            'If gCurrentKeyboardState.IsKeyDown(Key.W) Then

                            'ElseIf gCurrentKeyboardState.IsKeyDown(Key.S) Then
                            '    gGameEntitys(0).iAccelerating = -1
                            'Else
                            '    gGameEntitys(0).iAccelerating = 0
                            'End If

                            If gCurrentKeyboardState.IsKeyDown(Key.ShiftLeft) Then
                                gGameEntitys(0).bBoosting = True
                            Else
                                gGameEntitys(0).bBoosting = False
                            End If

                            'Increment the refire counter
                            fRefire += delta
                            'Is the client asking to fire, is the player allowes to shoot yet?
                            If gCurrentMouseState.LeftButton And fRefire > fRefireTime Then
                                Dim p As PlayerShip = gGameEntitys(0)
                                Const fAngle As Single = 4.5
                                For i As Integer = 1 To p.iGunLevel
                                    Dim fStartAngle As Single = (-fAngle / 2) * p.iGunLevel
                                    Dim fCurrentAngle = fStartAngle + fAngle * i
                                    gGameEntitys.Add(New Bullet(gGameEntitys(0).vPosition, gViewport.MousePosition, New Vector2(40.0F, 20.0F), gGameEntitys(0), fCurrentAngle, New Integer() {gTextures(13).ID}, gRandom, New Color4(1.0F, 0.66666F, 0.2F, 0.0F), 5.0F, GameObject.ParticleAlgorithm.Circle))
                                Next

                                fRefire = 0.0F
                            End If

                            'Increment the enemy spawn counter.
                            fEnemySpawn += delta
                            'If the player is alive and its time to spawn some enemies, spawn some enemies.
                            If fEnemySpawn > fEnemySpawnTime Then
                                'Add 5 Spinners
                                For iii As Integer = 0 To 5
                                    'Generate a random position on the playboard.
                                    Dim vPosition As New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50)
                                    'Keep generating a new position until the distance is 100 units away from the player.
                                    While GameMath.Vector2Distance(vPosition, gGameEntitys(0).vPosition) < 250
                                        'System.Diagnostics.Debug.WriteLine("HERE 111")
                                        vPosition = New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50)
                                    End While
                                    gGameEntitys.Add(New Spinner(vPosition, New Vector2(60.0F, 60.0F), gRandom, New Integer() {gTextures(0).ID, gTextures(5).ID}))
                                Next
                                'Add 2 Revolvers
                                For iii As Integer = 0 To 2
                                    'Generate a random position on the playboard.
                                    Dim vPosition As New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50)
                                    'Keep generating a new position until the distance is 100 units away from the player.
                                    While GameMath.Vector2Distance(vPosition, gGameEntitys(0).vPosition) < 250
                                        vPosition = New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50)
                                    End While
                                    gGameEntitys.Add(New Revolver(vPosition, New Vector2(60.0F, 60.0F), gRandom, New Integer() {gTextures(9).ID, gTextures(10).ID, gTextures(11).ID}))
                                Next
                                'Add 2 Pulsers
                                For iii As Integer = 0 To 20
                                    'Generate a random position on the playboard.
                                    Dim vPosition As New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50)
                                    'Keep generating a new position until the distance is 100 units away from the player.
                                    While GameMath.Vector2Distance(vPosition, gGameEntitys(0).vPosition) < 250
                                        vPosition = New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50)
                                    End While
                                    gGameEntitys.Add(New Pulser(vPosition, New Vector2(60.0F, 60.0F), gRandom, New Integer() {gTextures(17).ID, gTextures(18).ID}))
                                Next
                                fEnemySpawn = 0.0F
                            End If

                        Else
                            'If the player is dead or not in the game yet
                            b = True
                            'Clear all of the enemys
                            gGameEntitys.Clear()
                            'Center the viewport's position
                            gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, delta * 2)

                            'Something else or dead
                            If fSpawnTime > fSpawnTimeMax Then
                                fSpawnTime = 0.0F
                                SpawnPlayer()
                                fEnemySpawn = 5
                            End If
                        End If
                    Else

                        b = True
                    End If
                    If b Then
                        gGameEntitys.Clear()
                        gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, delta * 2)
                        'Something else or dead
                        If fSpawnTime > fSpawnTimeMax Then
                            fSpawnTimeMax = fSpawnTime
                        End If
                        fSpawnTime -= delta
                        If fSpawnTime < -1 Then
                            SpawnPlayer()
                            fSpawnTime = 0.0F
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
                                g.Update(delta, gRandom, gViewport.MousePosition)

                            Else
                                g.Update(delta, gRandom, gGameEntitys(0).vPosition)
                            End If
                            If g.fLifespan >= g.fLifespanMax Then
                                gGameEntitys.Remove(g)
                            Else
                                gQuadTree.Insert(gGameEntitys(i).vPosition, i)
                            End If
                        Next
                    Catch ex As Exception
#If DEBUG Then
                        Console.WriteLine("PROBLEM WITH QUADTREE COLLISIONS!!!")
                        Console.WriteLine(ex.Message)
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
                                                If (gGameEntitys(CollisionIds(i)).eLifeState = GameObject.LifeState.Alive) Then
                                                    If GameMath.Vector2Distance(gGameEntitys(CollisionIds(i)).vPosition, g.vPosition) < gGameEntitys(CollisionIds(i)).vSize.X - 10 Then
                                                        'Destroy all objects
                                                        For Each gg As GameObject In gGameEntitys.ToArray
                                                            If Not gg.eEntity = GameObject.ObjectType.Bullet Then
                                                                createParticle(gg.vPosition, ParticleType.EnemyExplosion)
                                                            Else
                                                                createParticle(gg.vPosition, ParticleType.BulletExplosion)
                                                            End If
                                                        Next
                                                        createParticle(g.vPosition, ParticleType.PlayerExplosion)
                                                        gGameEntitys.Clear()
                                                        fSpawnTime = 0.0F
                                                    End If
                                                End If
                                            End If
                                        Next
                                    Case GameObject.ObjectType.Bullet
                                        Dim CollisionIds() As Integer = gQuadTree.GetWithin(g.vPosition, 100).ToArray
                                        For i As Integer = 0 To CollisionIds.Length - 1
                                            If gGameEntitys(CollisionIds(i)).eEntity = GameObject.ObjectType.Enemy Then
                                                If (gGameEntitys(CollisionIds(i)).eLifeState = GameObject.LifeState.Alive) Then

                                                    Dim fRadius As Single = gGameEntitys(CollisionIds(i)).vSize.X / 2
                                                    If GameMath.Vector2Distance(gGameEntitys(CollisionIds(i)).vPosition, g.vPosition) < Math.Sqrt(fRadius * fRadius + fRadius * fRadius) Then
                                                        'Destroy the enemy and the bullet
                                                        'createParticle(gGameEntitys(CollisionIds(i)).vPosition, ParticleType.Firework, GameObject.ParticleAlgorithm.Circle)
                                                        createParticle(g.vPosition, ParticleType.BulletExplosion, New Color4(1.0F, 0.35F, 0.125F, 0.0F))

                                                        gGameEntitys.Remove(g)

                                                        ' Decrease enemy health
                                                        gGameEntitys(CollisionIds(i)).fHealth -= 30
                                                        'gGameEntitys.RemoveAt(CollisionIds(i)) 'Remove entity
                                                    End If
                                                End If
                                            End If
                                        Next
                                        If g.vPosition.X > 995.0F Or g.vPosition.X < 5.0F Or g.vPosition.Y > 995.0F Or g.vPosition.Y < 5.0F Then
                                            createParticle(g.vPosition, ParticleType.BulletExplosion, New Color4(1.0F, 0.35F, 0.125F, 0.0F))

                                            gGameEntitys.Remove(g)
                                        Else

                                        End If
                                    Case GameObject.ObjectType.Enemy
                                        Dim e As Enemy = g
                                        If (e.bCreateExplosion) Then
                                            createParticle(g.vPosition, ParticleType.EnemyExplosion, e.eeExplosionEffect.cColor, e.eeExplosionEffect.fTime)
                                            e.bCreateExplosion = False
                                        End If
                                        If (g.eLifeState = GameObject.LifeState.Dead) Then
                                            gGameEntitys.Remove(g)
                                            Dim p As PlayerShip = gGameEntitys(0)
                                            p.iKills += 1
                                            'Create a bunch of smaller particles
                                            Dim v As New Vector2(0)
                                            For i As Integer = 0 To e.eeExplosionEffect.iDeathNumber
                                                If (e.eeExplosionEffect.fDeathRandomise <> 0.0) Then
                                                    Dim a As Single = gRandom.NextDouble() * Math.PI
                                                    v.X = Math.Cos(a)
                                                    v.Y = Math.Sin(a)
                                                    v *= gRandom.NextDouble() * 0.9 + 0.1
                                                    v *= e.eeExplosionEffect.fDeathRandomise
                                                End If
                                                createParticle(g.vPosition + v, ParticleType.EnemyExplosion, e.eeExplosionEffect.cColor, e.eeExplosionEffect.fDeathTime, e.eeExplosionEffect.fSpeed)
                                            Next
                                        End If

                                End Select
                            Next
                        Catch ex As Exception
#If DEBUG Then
                            Console.WriteLine(ex.Message)
#End If
                        End Try
                    End If

                Case GameState.Menu
                    'Set the Viewport relative resolution
                    gViewport.ViewportRealSize = New Vector2(1000.0F, 1000.0F)

                    'Center the Viewport
                    gViewport.Position = New Vector2(500.0F, 500.0F) * gViewport.ViewportScale

                    'Destroy any entitys on the screen to clear the game.
                    gGameEntitys.Clear()

                    'Spawning random fireworks
                    Dim f As Single = 1.0F / 10.0F * gEffectLevel
                    fEffectSpawn += delta
                    While fEffectSpawn > f
                        createParticle(ParticleType.RandomExplosion)
                        fEffectSpawn -= f
                    End While

                    'Iterate through all the games objects
                    For Each g As GameObject In gGameEntitys.ToArray
                        g.Update(delta, gRandom, gViewport.Position)
                        If g.fLifespan >= g.fLifespanMax Then
                            gGameEntitys.Remove(g)
                        End If
                    Next

                    'Move the viewport to follow the first object(the player) and ensure that it wont fly of the screen to far.
                    gViewport.Position = (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale

                    'Update the menu button
                    btnStartGame.gControllGameState = gControllState
                    btnStartGame.Update(delta, gRandom, gViewport.MousePosition)
                    btnExitGame.gControllGameState = gControllState
                    btnExitGame.Update(delta, gRandom, gViewport.MousePosition)

                    If gCurrentMouseState.LeftButton Then

                        If btnStartGame.bIsHovering Then
                            gGameState = GameState.Game
                            fSpawnTime = 0
                        End If
                        If btnExitGame.bIsHovering Then
                            Exits()
                        End If

                    End If

                Case GameState.Loading
                    If fTotalTime > fIntroTotalTime + 0.5F Then
                        gGameState = GameState.Menu
                        fTotalTime = 0
                    Else
                        gViewport.Position = GameMath.Lerp(gViewport.Position, (New Vector2(500, 500) * gViewport.ViewportScale - New Vector2(gViewport.Width / 2, gViewport.Height / 2)) / gViewport.ViewportScale, delta * 2)
                        gViewport.ViewportRealSize = New Vector2(gViewport.Width, gViewport.Height)
                    End If
            End Select
        End Sub


        'This is a function called from the Update Thread of the Particle Update thread to iterate through all of the particles.
        'If this is run from the (updateThreadParticle) then the particle count is allowed to be between Off-Ultra else its manually set between Off-Low
        Private Sub gUpdateEffects(ByVal delta As Single)
            Try
                'Iterate through all the games objects [Particles]
                For Each g As GameObject In gGameEffects.ToArray
                    If Not g.Equals(Nothing) Or g.Equals(vbNull) Then
                        g.Update(delta, gRandom, gViewport.Position)
                        If g.fLifespan >= g.fLifespanMax Then
                            gGameEffects.Remove(g)
                        End If
                    End If
                Next
            Catch ex As Exception
#If DEBUG Then
                Console.WriteLine(ex.Message)
#End If
            End Try
        End Sub

        'The primary draw function
        'This draw function has been setup for 2D Rendering using 3d primatives
        Private Sub gDraw(ByVal delta As Single)
            'Overdraw the device with a 1x1 Pixel. Useful for reducing artifacts on the last draw call.
            Draw2D(gViewport, gTextures(6).ID, New Vector2(Single.MaxValue, Single.MaxValue), New Vector2(1, 1))

            Select Case gGameState
                Case GameState.Game
                    'Draw the background
                    Draw2D(gViewport, gTextures(3).ID, gViewport.Position / 2 - New Vector2(1000, 1000), gTextures(3).Size * 1.5F)
                    Draw2D(gViewport, gTextures(2).ID, New Vector2(0, 0), gTextures(2).Size)

                    'Draw the game
                    DrawParticles(delta)
                    DrawEntitys(delta)

                    'Draw the hud
                    Draw2D(gViewport, gTextures(4).ID, New Vector2(-12, -12) / gViewport.ViewportScale + gViewport.MousePosition, New Vector2(24, 24) / gViewport.ViewportScale)

                Case GameState.Menu
                    If (fTotalTime < 1) Then
                        GL.Color4(New Color4(1.0F, 1.0F, 1.0F, fTotalTime))
                    End If

                    'Draw the background
                    Draw2D(gViewport, gTextures(3).ID, gViewport.Position / 2 - New Vector2(1000, 1000), gTextures(3).Size * 1.5F)

                    'Draw the game
                    DrawParticles(delta)

                    If (fTotalTime < 1) Then
                        GL.Color4(New Color4(1.0F, 1.0F, 1.0F, fTotalTime))
                    End If

                    'Draw logo
                    Draw2D(gViewport, gTextures(6).ID, New Vector2(500.0F, 500.0F) - New Vector2(700.0F, 288.2353F) / 2, New Vector2(700.0F, 288.2353F))

                    'Draw buttons
                    btnStartGame.Draw(delta, gViewport)
                    btnExitGame.Draw(delta, gViewport)

                    'Draw the hud
                    Draw2D(gViewport, gTextures(4).ID, New Vector2(-12, -12) / gViewport.ViewportScale + gViewport.MousePosition, New Vector2(24, 24) / gViewport.ViewportScale)

                Case GameState.Loading
                    'Draw Intro Screen
                    If fTotalTime > fIntroTotalTime - fIntroFadeIn Then
                        GL.Color4(New Color4(1.0F, 1.0F, 1.0F, 1.0F - CType((fTotalTime - (fIntroTotalTime - fIntroFadeIn)) / fIntroFadeOut, Single)))
                    Else
                        GL.Color4(New Color4(1.0F, 1.0F, 1.0F, CType(fTotalTime / fIntroFadeIn, Single) - 1.0F))
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
        Public Sub DrawParticles(ByVal delta As Single)
            'Iterate through all the games objects [Particles]
            Try
                For Each g As GameObject In gGameEffects.ToArray
                    'Select the type of object and do the appropriate update for it
                    If Not g.Equals(Nothing) Or g.Equals(vbNull) Then
                        g.Draw(delta, gViewport)
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
        Public Sub DrawEntitys(ByVal delta As Single)
            'Iterate through all the games objects
            Try
                For Each g As GameObject In gGameEntitys.ToArray
                    'Select the type of object and do the appropriate update for it
                    g.Draw(delta, gViewport)
                Next
            Catch ex As Exception
#If DEBUG Then
                Console.WriteLine(ex.Message)
#End If
            End Try
        End Sub

        'Spawns the Player is the client has died or is beginning the game.
        Public Sub SpawnPlayer()
            If Not gGameEntitys.Count = 0 Then
                gGameEntitys(0) = New PlayerShip(New Vector2(500, 500), New Vector2(60, 60), New Integer() {gTextures(1).ID, gTextures(16).ID})
            Else
                gGameEntitys.Add(New PlayerShip(New Vector2(500, 500), New Vector2(60, 60), New Integer() {gTextures(1).ID, gTextures(16).ID}))
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
            createParticle(New Vector2(gRandom.NextDouble * 900 + 50, gRandom.NextDouble * 900 + 50), _ParticleType, ZeroColorAlpha(RandomColor4()))
        End Sub

        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType)
            createParticle(_Position, _ParticleType, ZeroColorAlpha(RandomColor4()))
        End Sub
        'New Color4(1.0F, 0.35F, 0.125F, 0.0F)
        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType, ByVal _Color As Color4)
            createParticle(_Position, _ParticleType, _Color, 1.0)
        End Sub

        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType, ByVal _Color As Color4, ByVal _Time As Single)
            createParticle(_Position, _ParticleType, _Color, _Time, 150)
        End Sub
        Private Sub createParticle(ByVal _Position As Vector2, ByVal _ParticleType As ParticleType, ByVal _Color As Color4, ByVal _Time As Single, ByVal _Speed As Single)
            Select Case _ParticleType
                Case ParticleType.RandomExplosion
                    If gGameEffects.Count < gEffectLevel * 25 Then
                        If bUsingEffectThread Then
                            'If running from the bParticleThread
                            'Create a new Explosion Particle with whatever settings are supplied
                            gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures(8).ID}, gEffectLevel, ZeroColorAlpha(RandomColor4()), 2.0F, False, _Speed))
                        Else
                            'If Not running from the bParticleThread
                            'Create a new Explosion Particle with whatever settings are supplied with the exception the maximum particles can only be on Low
                            gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures(8).ID}, GameMath.ClampInteger(gEffectLevel, 0, EffectLevel.Low), ZeroColorAlpha(RandomColor4()), 2.0F, False, _Speed))
                        End If
                    End If
                Case ParticleType.PlayerExplosion
                    gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures(8).ID}, gEffectLevel * 5, New Color4(1.0F, 1.0F, 1.0F, 0.0F), 5.0F, True, _Speed))
                Case ParticleType.EnemyExplosion
                    gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures(8).ID}, 6, _Color, _Time, True, _Speed))
                Case ParticleType.BulletExplosion
                    gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures(8).ID}, 3, _Color, 0.35F, True, _Speed))
                    'gGameEffects.Add(New Explosion(_Position, gRandom, New Integer() {gTextures(8).ID}, 0.0F * gEffectLevel, New Color4(1.0F, 0.35F, 0.125F, 0.0F), 2.0F, Explosion.ParticleAlgorithm.Spread))
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


