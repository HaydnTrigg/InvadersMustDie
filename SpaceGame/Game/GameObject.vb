#Region "Imports"

'Import parts of the OpenTK Framework
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input

'Import parts of the Isotope Framework
Imports Isotope
Imports Isotope.Library
Imports Isotope.Library.DrawSprite

#End Region
'This is the master class for all of the games objects. It includes the functions avaliable to all objects aswell as 
'any variables that are specificaly required by any object.
Public Class GameObject
#Region "Enumeration & Structures"

    Enum ObjectType
        Player = 0
        Enemy = 1
        Bullet = 2
        Other = 3
    End Enum
    Enum ParticleAlgorithm
        Spread = 0
        Circle = 1
    End Enum

#End Region
#Region "Variables and Properties"

    'Stores the position of the Object
    Public vPosition As Vector2

    'Stores the size of the Object
    Public vSize As Vector2

    'Stores an array of integers used to indicate which textures the object should draw
    Public iTextureIdentification() As Integer

    'Specifies the type of explosion this object will create/is
    Public eParticleAlgorithm As ParticleAlgorithm

    'Specifies the Rotation of the object in Radians
    Public fRotation As Single

    'Specifies the current speed of the object
    Public fSpeed As Single

    'Specifies the maximum current speed of the object
    Public fSpeedMax As Single

    'Specifies the maximum acceleration of the object
    Public fAcceleration As Single

    'Specifies if the object is accelerating
    Public iAccelerating = 0

    'Specifies if the object is boosting
    Public bBoosting = False

    'Specifies the boosting acceleration
    Public fAccelerationBoost As Single

    'Specifies the maximum speed when boosting
    Public fBoostSpeedMax As Single

    'Specifies the vMovement normal of the object, [Normalized]
    Public vMovement As New Vector2(0, 0)

    'Specifies the objects current lifespan
    Public fLifespan As Single

    'Specifies how long the objects lifespan is
    Public fLifespanMax As Single = Single.MaxValue

    'Specifies the type of object
    Public eEntity As ObjectType = ObjectType.Other

    'Create's a percentage of how long the object will live.
    ReadOnly Property fLifespanPercentage As Single
        Get
            Return fLifespan / fLifespanMax
        End Get
    End Property

    'Unused Variables for reference
    'Public fDumbness As Single
    'Public eBonus As *BONUSENUM*
    'Public iScore As Integer

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        vPosition = _Position
        vSize = _Size
        iTextureIdentification = _TextureID
    End Sub

#End Region
#Region "Main Methods"

    'The Overridable Update Function which is avaliable to all objects
    Public Overridable Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
    End Sub
    'The Overridable Draw Function which is avaliable to all objects
    Public Overridable Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
    End Sub

#End Region
End Class

Public Class PlayerShip
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        'Specifies that this object is a "Player"
        eEntity = ObjectType.Player

        'Defines the Acceleration of the Object
        fAcceleration = 75.0F

        'Defines the extra boost acceleration of the Object
        fAccelerationBoost = 25.0F

        'Defines the maximum normal speed
        fSpeedMax = 250.0F

        'Defines the extra boost speed of the Object
        fBoostSpeedMax = 100.0F
    End Sub

#End Region
#Region "Main Methods"
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'If bosting then accelerate with boost speed else use normal speed and check if going to fast.
        If bBoosting Then
            fSpeed = GameMath.ClampFloat(fSpeed + (fAcceleration + fAccelerationBoost) * gGameTime.DeltaTime, 0, fSpeedMax + fBoostSpeedMax)
        Else
            If fSpeed > fSpeedMax Then
                fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration * gGameTime.DeltaTime, 0, fSpeedMax + fBoostSpeedMax)
            Else
                'Determine if accelerating, idle or in reverse.
                Select Case iAccelerating
                    Case 1
                        fSpeed = GameMath.ClampFloat(fSpeed + (fAcceleration + fAccelerationBoost) * gGameTime.DeltaTime, 0, fSpeedMax)
                    Case 0
                        fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration / 5 * gGameTime.DeltaTime, 0, fSpeedMax + fBoostSpeedMax)
                    Case -1
                        fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration * gGameTime.DeltaTime, 0, fSpeedMax + fBoostSpeedMax)
                End Select
            End If
        End If

        Dim v As Vector2 = _Target - vPosition
        If (v.X = 0 And v.Y = 0) Then
            v = New Vector2(0, 1)
        End If
        v.Normalize()
        'Smooth the vMovement of the players object
        vMovement = GameMath.Lerp(vMovement, v, gGameTime.DeltaTime * 8)
        'Calculate the rotation to draw with
        fRotation = vMovement.Rotation() + (Math.PI * 0.5F)

        'Calculate the vMovement
        vPosition = GameMath.ClampVector(vPosition + vMovement * (gGameTime.DeltaTime * fSpeed), vSize / 2, New Vector2(1000.0F) - vSize / 2)
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)
    End Sub
#End Region
End Class

Public Class Spinner
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 60.0F + (_Random.NextDouble() * 80.0F)

        'Add extra code below
        eEntity = ObjectType.Enemy
    End Sub

#End Region
#Region "Main Methods"

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'Accelerate the object and clamp its speed to the maximum.
        fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * gGameTime.DeltaTime, 0, fSpeedMax)

        'Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
        vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), gGameTime.DeltaTime)

        'Calculate and clamp the position.
        vPosition = GameMath.ClampVector(vPosition + vMovement * gGameTime.DeltaTime * fSpeed, vSize / 2, New Vector2(1000.0F) - vSize / 2)

        'Spin the entity around and change it a little so it dosen't mimic every other entity.
        fRotation += gGameTime.DeltaTime * (0.5F + gRandom.NextDouble()) * 3
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, 0)
    End Sub

#End Region
End Class

Public Class Revolver
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 80 + (_Random.NextDouble() * 80)

        'Add extra code below
        eEntity = ObjectType.Enemy
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'Accelerate the object and clamp its speed to the maximum.
        fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * gGameTime.DeltaTime, 0, fSpeedMax)

        'Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
        vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), gGameTime.DeltaTime)

        'Calculate and clamp the position.
        vPosition = GameMath.ClampVector(vPosition + vMovement * gGameTime.DeltaTime * fSpeed, vSize / 2, New Vector2(1000.0F) - vSize / 2)

        'Spin the entity around and change it a little so it dosen't mimic every other entity.
        fRotation += gGameTime.DeltaTime * (0.5F + gRandom.NextDouble()) * 3
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, -fRotation * 2.0F)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, fRotation * 1.5F)
        Draw2dRotated(gViewport, iTextureIdentification(2), vPosition, vSize, 0)
    End Sub

#End Region
End Class

Public Class Bullet
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"

    'The color of the particles
    Dim cColorTarget As Color4

    'The current drawing color of the particles.
    Dim cDrawColor As Color4

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Movement As Vector2, ByVal _TextureID() As Integer, ByVal _ColorTarget As Color4, ByVal _Lifespan As Single, ByVal _Algorithm As ParticleAlgorithm)
        MyBase.New(_Position, _Size, _TextureID)

        'Define the fastest speed of the particles
        fSpeedMax = 200.0F

        'Define the lifespan of the particle.
        fLifespanMax = 5.0F

        'Define the color of the particle.
        cColorTarget = _ColorTarget

        'Define the type of object as "Other"
        eEntity = ObjectType.Bullet

        'Define an algorithm to use with the particles.
        eParticleAlgorithm = _Algorithm

        'Assign the objects movement.
        vMovement = _Movement
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As System.Random, ByVal _Movement As Vector2)

        vPosition += vMovement * fSpeed * gGameTime.DeltaTime
        fRotation = vMovement.Rotation

        'Calculate the current color of the particle object.
        cDrawColor.R = GameMath.Lerp(cColorTarget.R / 2 + 0.5F, cColorTarget.R, fLifespanPercentage)
        cDrawColor.G = GameMath.Lerp(cColorTarget.G / 2 + 0.5F, cColorTarget.G, fLifespanPercentage)
        cDrawColor.B = GameMath.Lerp(cColorTarget.B / 2 + 0.5F, cColorTarget.B, fLifespanPercentage)
        cDrawColor.A = GameMath.Lerp(1.0F, cColorTarget.A, fLifespanPercentage)

        'Add time to the particles current Lifespan.
        fLifespan += gGameTime.DeltaTime
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        'Draw the bullet with the specified color
        GL.Color4(cDrawColor)

        'Draw the object with rotation and a texture.
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)

        'Reset the drawing color back to the default
        GL.Color4(Color4.White)
    End Sub

#End Region
End Class

Public Class Explosion
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"

    'Array to hold all of the Particle positions
    Dim vPositions() As Vector2 = New Vector2() {}

    'Array to hold the Particle vMovement normals.
    Dim vParticleMovement() As Vector2 = New Vector2() {}

    'Array to hold all of the Particles speed.
    Dim vSpeed() As Single = New Single() {}

    'The color of the particles
    Dim cColorTarget As Color4

    'The current drawing color of the particles.
    Dim cDrawColor As Color4

    'The size the particles for drawing.
    Dim vDrawSize As New Vector2(18.0F, 2.0F)

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer, ByVal _ParticleLevel As Single, ByVal _ColorTarget As Color4, ByVal _Lifespan As Single, ByVal _Algorithm As ParticleAlgorithm)
        MyBase.New(_Position, _Size, _TextureID)

        'Calculate the amount of particles that will be created.
        Dim iParticleCount As Integer = GameMath.ClampInteger(50 * _ParticleLevel, 0, Integer.MaxValue) - 1

        'Create 50*x Positions, Speed and Movement values for each particle.
        vPositions = New Vector2(iParticleCount) {}
        vSpeed = New Single(iParticleCount) {}
        vParticleMovement = New Vector2(iParticleCount) {}
        For i As Integer = 0 To vParticleMovement.Length - 1
            vSpeed(i) = (_Random.NextDouble() + 1) * 100
            vParticleMovement(i) = New Vector2(_Random.NextDouble, _Random.NextDouble) - 0.5F
            vParticleMovement(i).Normalize()
            vPositions(i) = vPosition
        Next

        'Define the fastest speed of the particles
        fSpeedMax = 200.0F

        'Define the lifespan of the particle.
        fLifespanMax = _Lifespan

        'Define the color of the particle.
        cColorTarget = _ColorTarget

        'Define the type of object as "Other"
        eEntity = ObjectType.Other

        'Define an algorithm to use with the particles.
        eParticleAlgorithm = _Algorithm
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As System.Random, ByVal _Target As Vector2)
        'Iterate through all of the particles inside of this Object
        For i As Integer = 0 To vParticleMovement.Length - 1
            Select Case eParticleAlgorithm
                Case ParticleAlgorithm.Spread
                    'Slow the particle by a 7th of the current particles speed. X = X-X(delta/7)
                    vSpeed(i) = GameMath.ClampFloat(vSpeed(i) - Math.Sqrt(vSpeed(i)) * gGameTime.DeltaTime / 3.5F, 0.1F, fSpeedMax)
                Case ParticleAlgorithm.Circle
                    'Use SpreadRoot Algorithm
                    vSpeed(i) = GameMath.ClampFloat(vSpeed(i) - Math.Sqrt(vSpeed(i)) * gGameTime.DeltaTime * fLifespanPercentage * 250, fSpeedMax * (1 - fLifespanPercentage), fSpeedMax)
            End Select

            'Check if the particle collides with the boundary on the X axis.
            If vPositions(i).X > 995.0F Or vPositions(i).X < 5.0F Then
                vParticleMovement(i).X *= -1
            Else
                'Check if the particle collides with the boundary on the Y axis.
                If vPositions(i).Y > 995.0F Or vPositions(i).Y < 5.0F Then
                    vParticleMovement(i).Y *= -1
                End If
            End If
            vPositions(i) = GameMath.ClampVectorSingle(vPositions(i) + (vParticleMovement(i) * vSpeed(i) * gGameTime.DeltaTime), 4.0F, 4.0F, 996.0F, 996.0F)
        Next

        'Calculate the current color of the particle object.
        cDrawColor.R = GameMath.Lerp(cColorTarget.R / 2 + 0.5F, cColorTarget.R, fLifespanPercentage)
        cDrawColor.G = GameMath.Lerp(cColorTarget.G / 2 + 0.5F, cColorTarget.G, fLifespanPercentage)
        cDrawColor.B = GameMath.Lerp(cColorTarget.B / 2 + 0.5F, cColorTarget.B, fLifespanPercentage)
        cDrawColor.A = GameMath.Lerp(1.0F, cColorTarget.A, fLifespanPercentage)

        'Add time to the particles current Lifespan.
        fLifespan += gGameTime.DeltaTime
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        'Draw anything with the specified drawing color
        GL.Color4(cDrawColor)

        'Iterate through all of the particles and draw them.
        For i As Integer = 0 To vParticleMovement.Length - 1
            Draw2dRotated(gViewport, iTextureIdentification(0), vPositions(i), vDrawSize, vParticleMovement(i).Rotation)
        Next
    End Sub

#End Region
End Class


