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

    Enum LifeState
        Alive
        Dying
        Dead
    End Enum

    Structure Particle

        'Array to hold all of the Particle positions
        Dim position As Vector2

        'Array to hold the Particle vMovement normals.
        Dim direction As Vector2
        ' :: PRE CALCULATED VARIABLES ::
        Dim movement As Vector2
        Dim rotation As Single

        'Array to hold all of the Particles speed.
        Dim speed As Single
    End Structure

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

    'Specifies the current life state of the object
    Public eLifeState As LifeState = LifeState.Alive

    'Specifies how much health the object has left
    Public fHealth As Single = 100.0
    Public fMaxHealth As Single = 0

    'Specifies how long it takes to die
    Public fDieTime As Single = 1.0
    Public fDieTimeAccumulator As Single = 0.0

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
    Public Overridable Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
    End Sub
    'The Overridable Draw Function which is avaliable to all objects
    Public Overridable Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
    End Sub

#End Region
End Class

Public Class PlayerShip
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"
    Public vAcceleration As Vector2 = New Vector2(0)

    Public iKills As Integer
    Public iGunLevel As Integer
#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        'Specifies that this object is a "Player"
        eEntity = ObjectType.Player

        'Defines the Acceleration of the Object
        fAcceleration = 125.0F

        'Defines the extra boost acceleration of the Object
        fAccelerationBoost = 50.0F

        'Defines the maximum normal speed
        fSpeedMax = 350.0F

        'Defines the extra boost speed of the Object
        fBoostSpeedMax = 150.0F
        iGunLevel = 1
    End Sub

#End Region
#Region "Main Methods"
    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
        'If bosting then accelerate with boost speed else use normal speed and check if going to fast.
        If bBoosting Then
            fSpeed = GameMath.ClampFloat(fSpeed + (fAcceleration + fAccelerationBoost) * delta, 0, fSpeedMax + fBoostSpeedMax)
        Else
            If fSpeed > fSpeedMax Then
                fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration * delta, 0, fSpeedMax + fBoostSpeedMax)
            Else
                'Determine if accelerating, idle or in reverse.
                Select Case iAccelerating
                    Case 1
                        fSpeed = GameMath.ClampFloat(fSpeed + (fAcceleration + fAccelerationBoost) * delta, 0, fSpeedMax)
                    Case 0
                        fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration / 5 * delta, 0, fSpeedMax + fBoostSpeedMax)
                    Case -1
                        fSpeed = GameMath.ClampFloat(fSpeed - fAcceleration * delta, 0, fSpeedMax + fBoostSpeedMax)
                End Select
            End If
        End If

        'Weapon Level
        If iKills > 20 Then
            iGunLevel = 2
            If iKills > 40 Then
                iGunLevel = 3
                If iKills > 75 Then
                    iGunLevel = 4
                    If iKills > 100 Then
                        iGunLevel = 5
                    End If
                End If
            End If
        End If

        Dim v As Vector2 = _Target - vPosition
        If (v.X = 0 And v.Y = 0) Then
            v = New Vector2(0, 1)
        End If
        v.Normalize()
        'Calculate the rotation to draw with
        fRotation = v.Rotation() + (Math.PI / 2)

        'Smooth the vMovement of the players object
        'vMovement = GameMath.Lerp(vMovement, v, delta * 8)

        vMovement = GameMath.Lerp(vMovement, vAcceleration, delta * 8)

        'Calculate the vMovement
        vPosition = GameMath.ClampVector(vPosition + vMovement * (delta * fSpeed), vSize / 2, New Vector2(1000.0F) - vSize / 2)
    End Sub
    Dim tt As Single = 0.0
    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        tt += delta

        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One) 'Use additive blending with transparency
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, 0)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, fRotation)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha) 'Use linear transparency blending [default]



    End Sub
#End Region
End Class

Public Class Enemy
    Inherits GameObject
#Region "Enumeration & Structures"
    Public Structure ExplosionEffect
        Dim cColor As Color4
        Dim fTime As Single
        Dim fDeathRandomise As Single
        Dim iDeathNumber As Integer
        Dim fDeathTime As Single
        Dim fSpeed As Single

        Public Sub New(ByVal color As Color4, ByVal time As Single, ByVal speed As Single, ByVal deathrandomise As Single, ByVal deathnumber As Integer, ByVal deathtime As Single)
            cColor = color
            fTime = time
            fDeathRandomise = deathrandomise
            iDeathNumber = deathnumber
            fDeathTime = deathtime
            fSpeed = speed
        End Sub
    End Structure

#End Region
#Region "Variables and Properties"
    Public eeExplosionEffect As New ExplosionEffect(New Color4(1.0F, 1.0F, 1.0F, 1.0F), 0.5F, 50, 0, 1, 1)

    Public bCreateExplosion = False

    Public fHealthPercent = 1.0

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        'Add extra code below
        eEntity = ObjectType.Enemy

        If (eLifeState = LifeState.Dead) Then
            bCreateExplosion = True
        End If
    End Sub

#End Region
#Region "Main Methods"

    'Override Update statement.
    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
        fHealthPercent = fHealth / fMaxHealth
    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
    End Sub

#End Region
End Class

Public Class Revolver
    Inherits Enemy

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"
    Dim bCanHaveExplosion As Boolean() = {True, True}
#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _Random, _TextureID)

        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 90.0F + (_Random.NextDouble() * 120.0F)

        fDieTime = 0.15

        fMaxHealth = 133 + (_Random.NextDouble() * 20)
        fHealth = fMaxHealth

        'Add extra code below
        eEntity = ObjectType.Enemy

        eeExplosionEffect = New ExplosionEffect(New Color4(0.1F, 1.0F, 0.1F, 1.0F), 0.5F, 75, 100.0, 5, 0.4)
    End Sub

#End Region
#Region "Main Methods"

    'Override Update statement.
    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
        MyBase.Update(delta, gRandom, _Target)

        Select Case eLifeState
            Case LifeState.Alive
                ' :: ALIVE ::
                'Accelerate the object and clamp its speed to the maximum.
                fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * delta, 0, fSpeedMax)

                'Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
                vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), delta)

                'Calculate and clamp the position.
                vPosition = GameMath.ClampVector(vPosition + vMovement * delta * fSpeed, vSize / 2, New Vector2(1000.0F) - vSize / 2)

                If (fHealth <= 0) Then
                    eLifeState = GameObject.LifeState.Dying
                End If

                'Spin the entity around and change it a little so it dosen't mimic every other entity.
                fRotation += delta * (0.5F + Rnd()) * fHealthPercent

            Case LifeState.Dying
                ' :: DYING/DEAD ::
                vSize += delta * 1500
                fDieTimeAccumulator += delta
                If (fDieTimeAccumulator >= fDieTime) Then
                    eLifeState = LifeState.Dead
                End If
        End Select

        If (fHealthPercent < 0.65) Then
            If (bCanHaveExplosion(0)) Then
                bCanHaveExplosion(0) = False
                bCreateExplosion = True
            End If

            If (fHealthPercent < 0.35) Then
                If (bCanHaveExplosion(1)) Then
                    bCanHaveExplosion(1) = False
                    bCreateExplosion = True
                End If
            End If
        End If
    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One) 'Use additive blending with transparency
        If (fHealthPercent > 0.65) Then 'atleast 80%
            Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize * fHealthPercent, -fRotation * 2.0F)
        End If
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize * Math.Max(fHealthPercent, 0.75), fRotation * 1.5F)
        Draw2dRotated(gViewport, iTextureIdentification(2), vPosition, vSize, 0)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha) 'Use linear transparency blending [default]
    End Sub

#End Region
End Class

Public Class Spinner
    Inherits Enemy

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"
    Dim bCanHaveExplosion As Boolean() = {True}
#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _Random, _TextureID)

        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 80 + (_Random.NextDouble() * 80)

        fDieTime = 0.15

        fMaxHealth = 53 + (_Random.NextDouble() * 10)
        fHealth = fMaxHealth
        'Add extra code below
        eEntity = ObjectType.Enemy

        eeExplosionEffect = New ExplosionEffect(New Color4(1.0F, 0.1F, 0.1F, 1.0F), 0.5F, 75, 0.0, 1, 0.8)
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
        MyBase.Update(delta, gRandom, _Target)

        Select Case eLifeState
            Case LifeState.Alive
                ' :: ALIVE ::
                'Accelerate the object and clamp its speed to the maximum.
                fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * delta, 0, fSpeedMax)

                'Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
                vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), delta)

                'Calculate and clamp the position.
                vPosition = GameMath.ClampVector(vPosition + vMovement * delta * fSpeed, vSize / 2, New Vector2(1000.0F) - vSize / 2)

                If (fHealth <= 0) Then
                    eLifeState = GameObject.LifeState.Dying
                End If

                'Spin the entity around and change it a little so it dosen't mimic every other entity.
                fRotation += delta * (0.5F + Rnd()) * 2.5 * fHealthPercent

            Case LifeState.Dying
                ' :: DYING/DEAD ::
                vSize += delta * 1500
                fDieTimeAccumulator += delta
                If (fDieTimeAccumulator >= fDieTime) Then
                    eLifeState = LifeState.Dead
                End If
        End Select

        If (fHealthPercent < 0.5) Then
            If (bCanHaveExplosion(0)) Then
                bCanHaveExplosion(0) = False
                bCreateExplosion = True
            End If
        End If

    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One) 'Use additive blending with transparency
        'Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize * Math.Max(fHealthPercent, 0.6), fRotation)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, 0)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha) 'Use linear transparency blending [default]
    End Sub

#End Region
End Class

Public Class Pulser
    Inherits Enemy

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"
    Dim bCanHaveExplosion As Boolean() = {True}
    Dim fPulseTime As Single
#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _Random, _TextureID)

        'Set the Entity's acceleration to 150
        fAcceleration = 150.0F
        'Set the maximum speed of the Entity to 150
        fSpeedMax = 80 + (_Random.NextDouble() * 80)

        fDieTime = 0.15

        fMaxHealth = 23 + (_Random.NextDouble() * 10)
        fHealth = fMaxHealth
        'Add extra code below
        eEntity = ObjectType.Enemy

        eeExplosionEffect = New ExplosionEffect(New Color4(1.0F, 0.55F, 0.1F, 1.0F), 0.5F, 80, 0.0, 1, 0.4)

        fPulseTime += _Random.NextDouble() * 100
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
        MyBase.Update(delta, gRandom, _Target)

        Select Case eLifeState
            Case LifeState.Alive
                ' :: ALIVE ::
                'Accelerate the object and clamp its speed to the maximum.
                fSpeed = GameMath.ClampFloat(fSpeed + fAcceleration * delta, 0, fSpeedMax)

                'Smooth the vMovement using linear interpolation against a calculated normal specifying the direction of the target.
                vMovement = GameMath.Lerp(vMovement, GameMath.NormalizeVector2(_Target - vPosition), delta)

                'Calculate and clamp the position.
                vPosition = GameMath.ClampVector(vPosition + vMovement * delta * fSpeed, vSize / 2, New Vector2(1000.0F) - vSize / 2)

                If (fHealth <= 0) Then
                    eLifeState = GameObject.LifeState.Dying
                End If

                'Spin the entity around and change it a little so it dosen't mimic every other entity.
                fRotation += delta * (0.5F + Rnd()) * 4.5 * fHealthPercent

            Case LifeState.Dying
                ' :: DYING/DEAD ::
                vSize += delta * 1500
                fDieTimeAccumulator += delta
                If (fDieTimeAccumulator >= fDieTime) Then
                    eLifeState = LifeState.Dead
                End If
        End Select

        If (fHealthPercent < 0.5) Then
            If (bCanHaveExplosion(0)) Then
                bCanHaveExplosion(0) = False
                bCreateExplosion = True
            End If
        End If

    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        fPulseTime += delta * (Math.PI / (2 * 2)) * 3

        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One) 'Use additive blending with transparency
        'Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, 0)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize * Math.Max(fHealthPercent * Math.Abs(Math.Sin(fPulseTime)), 0.6), fRotation)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha) 'Use linear transparency blending [default]
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

    Dim vInertia As Vector2



#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _MousePosition As Vector2, ByVal _Size As Vector2, ByVal _Parent As PlayerShip, ByVal angle As Single, ByVal _TextureID() As Integer, ByVal gRandom As System.Random, ByVal _ColorTarget As Color4, ByVal _Lifespan As Single, ByVal _Algorithm As ParticleAlgorithm)
        MyBase.New(_Position, _Size, _TextureID)

        'Define the acceleration of the of the object
        fSpeed = 750.0F
        fSpeedMax = fSpeed + 100.0F
        'fSpeed = 0.0F 'Inertia test

        'Define the lifespan of the particle.
        fLifespanMax = 5.0F

        'Define the color of the particle.
        cColorTarget = _ColorTarget

        'Define the type of object as "Other"
        eEntity = ObjectType.Bullet

        'Define an algorithm to use with the particles.
        eParticleAlgorithm = _Algorithm

        'Assign the objects movement.
        Dim vTargetDirection = _MousePosition - _Position
        Dim fTargetAngle = Math.Atan2(vTargetDirection.Y, vTargetDirection.X) + (angle / 180)
        Dim fRandomAngle = ((gRandom.NextDouble() - 0.5) * 2) * (3 / 180) * Math.PI 'Random Angle between +- 3 degrees
        vMovement.X = Math.Cos(fTargetAngle + fRandomAngle)
        vMovement.Y = Math.Sin(fTargetAngle + fRandomAngle)

        vInertia = _Parent.vMovement * _Parent.fSpeed
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As System.Random, ByVal _Movement As Vector2)

        'vPosition += ((vInertia + vMovement * fSpeed)) * delta
        vPosition += ((vMovement * fSpeed)) * delta
        fRotation = vMovement.Rotation

        'Calculate the current color of the particle object.
        cDrawColor.R = GameMath.Lerp(cColorTarget.R / 2 + 0.5F, cColorTarget.R, fLifespanPercentage)
        cDrawColor.G = GameMath.Lerp(cColorTarget.G / 2 + 0.5F, cColorTarget.G, fLifespanPercentage)
        cDrawColor.B = GameMath.Lerp(cColorTarget.B / 2 + 0.5F, cColorTarget.B, fLifespanPercentage)
        cDrawColor.A = GameMath.Lerp(1.0F, cColorTarget.A, fLifespanPercentage)

        'Add time to the particles current Lifespan.
        fLifespan += delta
    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        ' :: SHIP ::
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One) 'Use additive blending with transparency
        'Draw the bullet with the specified color
        GL.Color4(cDrawColor)

        'GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.OneMinusDstAlpha)

        'Draw the object with rotation and a texture. Also rotate the texture 180 degrees.
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, fRotation + Math.PI)

        'Reset the drawing color back to the default
        GL.Color4(Color4.White)

        ' :: ENGINE PARTICLES ::


        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha) 'Use linear transparency blending [default]
    End Sub

#End Region
End Class

Public Class Explosion
    Inherits GameObject

#Region "Enumeration & Structures"

#End Region
#Region "Variables and Properties"
    'Arrray to hold all of the Particles
    Dim vParticles() As Particle = New Particle() {}

    'The color of the particles
    Dim cColorTarget As Color4

    'The current drawing color of the particles.
    Dim cDrawColor As Color4

    'The size the particles for drawing.
    Dim vDrawSize As New Vector2(18.0F, 2.0F)

    Dim bCollisionCheck As Boolean

    Dim fSpeedMul As Single = 2.0

#End Region
#Region "Initializers"

    Sub New(ByVal _Position As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer, ByVal _ParticleLevel As Single, ByVal _ColorTarget As Color4, ByVal _Lifespan As Single, ByVal _CollisionCheck As Boolean, ByVal _Speed As Single)
        MyBase.New(_Position, New Vector2(0), _TextureID)

        bCollisionCheck = _CollisionCheck ' Assign the collision checking parameter

        'Calculate the amount of particles that will be created.
        Dim fRavg As Single = (1.5 * 200 / 100) * _Lifespan 'Calculate the average radius of the spread
        Dim fC = 2.0 * Math.PI * fRavg 'Calculate the average circumference
        Dim iParticleCount As Integer = GameMath.ClampInteger(fC * _ParticleLevel, 0, Integer.MaxValue) - 1
        iParticleCount *= 3

        ' Create an array to store all the particles
        vParticles = New Particle(iParticleCount) {}

        For i As Integer = 0 To iParticleCount
            vParticles(i).position = vPosition 'Set starting position

            Dim angle As Double = _Random.NextDouble() * 2.0 * Math.PI '0-360 degrees in radians
            vParticles(i).direction = New Vector2(Math.Cos(angle), Math.Sin(angle)) 'Set the direction as a vector

            vParticles(i).speed = (_Random.NextDouble() + 1) * _Speed

            vParticles(i).movement = vParticles(i).direction * vParticles(i).speed
            vParticles(i).rotation = vParticles(i).movement.Rotation
        Next





        ''Create 50*x Positions, Speed and Movement values for each particle.
        'vPositions = New Vector2(iParticleCount) {}
        'vSpeed = New Single(iParticleCount) {}
        'vParticleMovement = New Vector2(iParticleCount) {}
        'For i As Integer = 0 To vParticleMovement.Length - 1
        '    vSpeed(i) = (_Random.NextDouble() + 1) * 200
        '    vParticleMovement(i) = New Vector2(_Random.NextDouble, _Random.NextDouble) - 0.5F
        '    vParticleMovement(i).Normalize()
        '    vPositions(i) = vPosition
        'Next

        'Define the fastest speed of the particles
        fSpeedMax = 400.0F

        'Define the lifespan of the particle.
        fLifespanMax = _Lifespan

        'Define the color of the particle.
        cColorTarget = _ColorTarget
        cColorTarget.A = 0

        'Define the type of object as "Other"
        eEntity = ObjectType.Other

        'Define an algorithm to use with the particles.
        'eParticleAlgorithm = _Algorithm
    End Sub

#End Region
#Region "Main Methods"

    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As System.Random, ByVal _Target As Vector2)
        'Iterate through all of the particles inside of this Object
        'For i As Integer = 0 To vParticles.Length - 1
        '    'Select Case eParticleAlgorithm
        '    '    Case ParticleAlgorithm.Spread
        '    '        'Slow the particle by a 7th of the current particles speed. X = X-X(delta/7)
        '    '        vSpeed(i) = GameMath.ClampFloat(vSpeed(i) - Math.Sqrt(vSpeed(i)) * delta / 3.5F, 0.1F, fSpeedMax)
        '    '    Case ParticleAlgorithm.Circle
        '    '        'Use SpreadRoot Algorithm
        '    '        vSpeed(i) = GameMath.ClampFloat(vSpeed(i) - Math.Sqrt(vSpeed(i)) * delta * fLifespanPercentage * 250, fSpeedMax * (1 - fLifespanPercentage), fSpeedMax)
        '    'End Select


        '    'Check if the particle collides with the boundary on the X axis.
        '    If vPositions(i).X > 995.0F Or vPositions(i).X < 5.0F Then
        '        vParticleMovement(i).X *= -1
        '    Else
        '        'Check if the particle collides with the boundary on the Y axis.
        '        If vPositions(i).Y > 995.0F Or vPositions(i).Y < 5.0F Then
        '            vParticleMovement(i).Y *= -1
        '        End If
        '    End If
        '    vPositions(i) = GameMath.ClampVectorSingle(vPositions(i) + (vParticleMovement(i) * vSpeed(i) * delta), 4.0F, 4.0F, 996.0F, 996.0F)
        'Next

        fSpeedMul -= delta * 1.75
        fSpeedMul = GameMath.ClampFloat(fSpeedMul, 1, Single.MaxValue)

        For i As Integer = 0 To vParticles.Length - 1
            'Check if the particle collides with the boundary on the X axis.
            If (bCollisionCheck) Then

                If vParticles(i).position.X >= 995.0F Or vParticles(i).position.X <= 5.0F Then
                    vParticles(i).movement.X *= -1
                    vParticles(i).rotation = vParticles(i).movement.Rotation
                Else
                    'Check if the particle collides with the boundary on the Y axis.
                    If vParticles(i).position.Y >= 995.0F Or vParticles(i).position.Y <= 5.0F Then
                        vParticles(i).movement.Y *= -1
                        vParticles(i).rotation = vParticles(i).movement.Rotation
                    End If
                End If
                'vParticles(i).position = GameMath.ClampVectorSingle(vParticles(i).position + (vParticles(i).direction * vParticles(i).speed * delta), 4.0F, 4.0F, 996.0F, 996.0F)
                vParticles(i).position = GameMath.ClampVectorSingle(vParticles(i).position + (vParticles(i).movement * fSpeedMul * delta), 5.0F, 5.0F, 995.0F, 995.0F)
            Else
                vParticles(i).position += (vParticles(i).movement * fSpeedMul * delta)
            End If

        Next

        'Calculate the current color of the particle object.
        cDrawColor.R = GameMath.ClampFloat(GameMath.Lerp((cColorTarget.R / 2 + 0.5F) * 2.0F, cColorTarget.R * 2.0F, fLifespanPercentage), 0, 1)
        cDrawColor.G = GameMath.ClampFloat(GameMath.Lerp((cColorTarget.G / 2 + 0.5F) * 2.0F, cColorTarget.G * 2.0F, fLifespanPercentage), 0, 1)
        cDrawColor.B = GameMath.ClampFloat(GameMath.Lerp((cColorTarget.B / 2 + 0.5F) * 2.0F, cColorTarget.B * 2.0F, fLifespanPercentage), 0, 1)
        cDrawColor.A = GameMath.Lerp(1.0F, cColorTarget.A, fLifespanPercentage)

        'Add time to the particles current Lifespan.
        fLifespan += delta
    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        'Draw anything with the specified drawing color
        GL.Color4(cDrawColor)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One) 'Use additive blending with transparency
        'Iterate through all of the particles and draw them.
        For i As Integer = 0 To vParticles.Length - 1
            Draw2dRotated(gViewport, iTextureIdentification(0), vParticles(i).position, vDrawSize, vParticles(i).rotation)
        Next
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha) 'Use linear transparency blending [default]
    End Sub

#End Region
End Class


