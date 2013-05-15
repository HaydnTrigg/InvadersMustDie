
'Import parts of the OpenTK Framework
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input

'Import parts of the Isotope Framework
Imports HTSpaceGame.Isotope.Quadtree
Imports HTSpaceGame.Isotope
Imports HTSpaceGame.Isotope.GraphicsMath

'This is the master class for all of the games objects. It includes the functions avaliable to all objects aswell as 
'any variables that are specificaly required by any object.
Public Class GameObject
    Enum ObjectType
        Player = 0
        Enemy = 1
        Bullet = 2
        Other = 3
    End Enum


    'Stores the position of the Object
    Public vPosition As Vector2
    'Stores the size of the Object
    Public vSize As Vector2
    'Stores an array of integers used to indicate which textures the object should draw
    Public iTextureIdentification() As Integer

    Public fDumbness As Single
    Public fRotation As Single
    Public fSpeed As Single
    Public fAcceleration As Single = 250.0F
    Public fSpeedMax As Single = 200.0F
    Public bBoosting = False
    Public fBoostSpeed As Single = 20.0F
    Public fBoostSpeedMax As Single = 350.0F
    Public movement As New Vector2(1, 0)
    Public fLifetime As Single
    Public fLifetimeMax As Single = Single.MaxValue
    Public eEntity As ObjectType = ObjectType.Other

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        vPosition = _Position
        vSize = _Size
        iTextureIdentification = _TextureID
    End Sub

    'The Overridable Update Function which is avaliable to all objects
    Public Overridable Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
    End Sub
    'The Overridable Draw Function which is avaliable to all objects
    Public Overridable Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
    End Sub

End Class

'The object for the Players ship.
Public Class PlayerShip
    Inherits GameObject

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)
        'Add extra code below
        eEntity = ObjectType.Player
    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        If Not bBoosting Then
            fSpeed = GameMath.ClampF(fSpeed, 0, fSpeedMax)
        Else
            fSpeed += fAcceleration * gGameTime.ElapsedGameTime
            fSpeed = GameMath.ClampF(fSpeed, 0, fBoostSpeedMax)
        End If

        If VectorMath.Vector2Distance(_Target, vPosition) > 5 Then
            movement = _Target - vPosition

            fRotation = GameMath.Lerp(fRotation, movement.Rotation() + (Math.PI * 0.5F), 1)

            If (movement.X = 0 And movement.Y = 0) Then
                movement = New Vector2(0, 1)
            End If
            movement.Normalize()
        End If
        vPosition += movement * (gGameTime.ElapsedGameTime * fSpeed)
        vPosition = GameMath.ClampV(vPosition, vSize / 2, New Vector2(1000.0F) - vSize / 2)
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)
    End Sub
End Class

Public Class Spinner
    Inherits GameObject

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        iTextureIdentification = _TextureID

        'Setup the size of the Square Enemy o 60x60
        vSize = New Vector2(60, 60)
        'Set the Entity rotation to 0
        fRotation = 0
        'Set the Entity Speed between 60 and 140
        fSpeed = 60 + (_Random.NextDouble() * 80)
        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 150.0F

        'Add extra code below
        eEntity = ObjectType.Enemy

    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'fSpeed = GameMath.ClampF(fSpeed, 0, fSpeedMax)
        movement = GameMath.Lerp(movement, VectorMath.NormalizeVector2(_Target - vPosition), gGameTime.ElapsedGameTime / (1 + fDumbness))

        If (movement.X = 0 And movement.Y = 0) Then
            movement = New Vector2(0, 1)
            movement.Normalize()
        End If

        vPosition += movement * gGameTime.ElapsedGameTime * fSpeed
        vPosition = GameMath.ClampV(vPosition, vSize / 2, New Vector2(1000.0F) - vSize / 2)
        fRotation += gGameTime.ElapsedGameTime * (0.5F + gRandom.NextDouble()) * 3
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, 0)
    End Sub
End Class

Public Class Revolver
    Inherits GameObject

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        iTextureIdentification = _TextureID

        'Setup the size of the Square Enemy o 60x60
        vSize = New Vector2(60, 60)
        'Set the Entity rotation to 0
        fRotation = 0
        'Set the Entity Speed between 80 and 160
        fSpeed = 80 + (_Random.NextDouble() * 80)
        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 150.0F

        'Add extra code below

        eEntity = ObjectType.Enemy
    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'fSpeed = GameMath.ClampF(fSpeed, 0, fSpeedMax)
        movement = GameMath.Lerp(movement, VectorMath.NormalizeVector2(_Target - vPosition), gGameTime.ElapsedGameTime / (1 + fDumbness))

        If (movement.X = 0 And movement.Y = 0) Then
            movement = VectorMath.NormalizeVector2(New Vector2(0, 1))
        End If

        vPosition += movement * gGameTime.ElapsedGameTime * fSpeed
        vPosition = GameMath.ClampV(vPosition, vSize / 2, New Vector2(1000.0F) - vSize / 2)
        fRotation += gGameTime.ElapsedGameTime * (0.5F + gRandom.NextDouble()) * 3
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, -fRotation * 2.0F)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, fRotation * 1.5F)
        Draw2dRotated(gViewport, iTextureIdentification(2), vPosition, vSize, 0)
    End Sub
End Class

Public Class Explosion
    Inherits GameObject
    'GameMath.ClampI(_Random.NextDouble() * 6, 0, 6)

    'Array to hold all of the Particle positions
    Dim vPositions() As Vector2 = New Vector2() {}
    'Array to hold the Particle movement normals.
    Dim vParticleMovement() As Vector2 = New Vector2() {}
    'Array to hold all of the Particles speed.
    Dim vSpeed() As Single = New Single() {}
    'The color of the particles
    Dim cColorTarget As Color4
    'The current drawing color of the particles.
    Dim cDrawColor As Color4
    'The size the particles for drawing.
    Dim vDrawSize As New Vector2(12.0F, 1.50F)

    'Create a defined explosion effect
    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer, ByVal _ParticleLevel As Single, ByVal _ColorTarget As Color4, ByVal _Lifetime As Single)
        MyBase.New(_Position, _Size, _TextureID)
        Dim iParticleCount As Integer = GameMath.ClampI(50 * _ParticleLevel, 0, Integer.MaxValue) - 1
        'Create 200 points inside of Size
        vPositions = New Vector2(iParticleCount) {}
        vSpeed = New Single(iParticleCount) {}
        vParticleMovement = New Vector2(iParticleCount) {}
        For i As Integer = 0 To vParticleMovement.Length - 1
            vSpeed(i) = (_Random.NextDouble() + 1) * 100
            vParticleMovement(i) = New Vector2(_Size.X * _Random.NextDouble, _Size.Y * _Random.NextDouble) - _Size / 2
            vParticleMovement(i).Normalize()
            vPositions(i) = vPosition
        Next
        fLifetimeMax = _Lifetime
        cColorTarget = _ColorTarget
        eEntity = ObjectType.Other
    End Sub

    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As System.Random, ByVal _Target As Vector2)
        'Iterate through all of the particles inside of this Object
        For i As Integer = 0 To vParticleMovement.Length - 1
            'Slow the particle by a 7th of the current particles speed. X = X-X(delta/7)
            vSpeed(i) -= vSpeed(i) * gGameTime.ElapsedGameTime / -7

            'Check if the particle collides with the boundary on the X axis.
            If vPositions(i).X > 995.0F Or vPositions(i).X < 5.0F Then
                vParticleMovement(i).X *= -1
            Else
                'Check if the particle collides with the boundary on the Y axis.
                If vPositions(i).Y > 995.0F Or vPositions(i).Y < 5.0F Then
                    vParticleMovement(i).Y *= -1
                End If
            End If
            vPositions(i) = GameMath.ClampVS(vPositions(i) + (vParticleMovement(i) * vSpeed(i) * gGameTime.ElapsedGameTime), 4.0F, 4.0F, 996.0F, 996.0F)
        Next
        'Add the time to the particles current lifetime.
        fLifetime += gGameTime.ElapsedGameTime

        'Create a Lerp Percentage rather than calculate the percentage 4 times for each Color channel.
        Dim fColorPercentage As Single = fLifetime / fLifetimeMax
        cDrawColor.R = GameMath.Lerp(cColorTarget.R / 2 + 0.5F, cColorTarget.R, fColorPercentage)
        cDrawColor.G = GameMath.Lerp(cColorTarget.G / 2 + 0.5F, cColorTarget.G, fColorPercentage)
        cDrawColor.B = GameMath.Lerp(cColorTarget.B / 2 + 0.5F, cColorTarget.B, fColorPercentage)
        cDrawColor.A = GameMath.Lerp(1.0F, cColorTarget.A, fColorPercentage)
    End Sub
    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        GL.Color4(cDrawColor)
        'GL.BlendFunc(BlendingFactorSrc.Src1Color, BlendingFactorDest.OneMinusSrcColor)
        For i As Integer = 0 To vParticleMovement.Length - 1
            Draw2dRotated(gViewport, iTextureIdentification(0), vPositions(i), vDrawSize, vParticleMovement(i).Rotation)
        Next
    End Sub
End Class
