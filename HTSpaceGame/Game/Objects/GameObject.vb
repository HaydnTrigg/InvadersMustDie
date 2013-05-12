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
Imports HTSpaceGame.IsotopeVB

'This is the master class for all of the games objects. It includes the functions avaliable to all objects aswell as 
'any variables that are specificaly required by any object.
Public Class GameObject
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
    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        If Not bBoosting Then
            fSpeed = GameMath.Clamp(fSpeed, 0, fSpeedMax)
        Else
            fSpeed += fAcceleration * gGameTime.ElapsedGameTime
            fSpeed = GameMath.Clamp(fSpeed, 0, fBoostSpeedMax)
        End If

        If GameMath.Vector2Distance(_Target, vPosition) > 5 Then
            movement = _Target - vPosition

            fRotation = GameMath.Lerp(fRotation, movement.Rotation() + (Math.PI * 0.5F), 1)

            If (movement.X = 0 And movement.Y = 0) Then
                movement = New Vector2(0, 1)
            End If
            movement.Normalize()
        End If
        vPosition += movement * (gGameTime.ElapsedGameTime * fSpeed)
        vPosition = GameMath.Clamp(vPosition, vSize / 2, New Vector2(1000.0F) - vSize / 2)
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


    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'fSpeed = GameMath.ClampF(fSpeed, 0, fSpeedMax)
        movement = GameMath.Lerp(movement, GameMath.NormalizeVector2(_Target - vPosition), gGameTime.ElapsedGameTime / (1 + fDumbness))

        If (movement.X = 0 And movement.Y = 0) Then
            movement = GameMath.NormalizeVector2(New Vector2(0, 1))
        End If

        vPosition += movement * gGameTime.ElapsedGameTime * fSpeed
        vPosition = GameMath.Clamp(vPosition, vSize / 2, New Vector2(1000.0F) - vSize / 2)
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


    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
        'fSpeed = GameMath.ClampF(fSpeed, 0, fSpeedMax)
        movement = GameMath.Lerp(movement, GameMath.NormalizeVector2(_Target - vPosition), gGameTime.ElapsedGameTime / (1 + fDumbness))

        If (movement.X = 0 And movement.Y = 0) Then
            movement = GameMath.NormalizeVector2(New Vector2(0, 1))
        End If

        vPosition += movement * gGameTime.ElapsedGameTime * fSpeed
        vPosition = GameMath.Clamp(vPosition, vSize / 2, New Vector2(1000.0F) - vSize / 2)
        fRotation += gGameTime.ElapsedGameTime * (0.5F + gRandom.NextDouble()) * 3
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        Draw2dRotated(gViewport, iTextureIdentification(0), vPosition, vSize, -fRotation * 1.1)
        Draw2dRotated(gViewport, iTextureIdentification(1), vPosition, vSize, fRotation)
        Draw2dRotated(gViewport, iTextureIdentification(2), vPosition, vSize, 0)
    End Sub
End Class

Public Class Explosion
    Inherits GameObject

    Dim vPositions() As Vector2 = New Vector2() {}
    Dim vParticleMovement() As Vector2 = New Vector2() {}
    Dim vSpeed() As Single = New Single() {}
    Dim cColor As Color4
    Dim cDrawColor As Color4

    'Create a completely random explosion effect
    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        Me.New(_Position, _Size, _Random, _TextureID,(New Color4() {New Color4(1.0F, 0.0F, 0.0F, 0.0F), New Color4(0.0F, 1.0F, 0.0F, 0.0F), New Color4(0.0F, 0.5803922F, 1.0F, 0.0F), New Color4(1.0F, 0.3529412F, 0.20784314F, 0.0F), New Color4(255, 0.0F, 0.8627451F, 0.0F), New Color4(0.698039234F, 0.0F, 1.0F, 0.0F), New Color4(1.0F, 0.847058833F, 0.0F, 0)})(GameMath.ClampI(_Random.NextDouble() * 6, 0, 6)))
    End Sub

    'Create a defined explosion effect
    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer, ByVal _Color As Color4)
        MyBase.New(_Position, _Size, _TextureID)
        Dim iParticleCount As Integer = 100
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
        fLifetimeMax = 2.0F
        cColor = _Color
    End Sub

    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As System.Random, ByVal _Target As Vector2)
        For i As Integer = 0 To vParticleMovement.Length - 1
            vSpeed(i) -= gGameTime.ElapsedGameTime / 4
            If vPositions(i).X > 995 Or vPositions(i).X < 5 Then
                vParticleMovement(i).X *= -1
            End If
            If vPositions(i).Y > 995 Or vPositions(i).Y < 5 Then
                vParticleMovement(i).Y *= -1
            End If

            vPositions(i) = GameMath.Clamp(vPositions(i), New Vector2(5, 5), New Vector2(995, 995))


            vPositions(i) += vParticleMovement(i) * vSpeed(i) * gGameTime.ElapsedGameTime
        Next
        fLifetime += gGameTime.ElapsedGameTime
        cDrawColor.R = GameMath.Lerp(cColor.R / 2 + 0.5F, cColor.R, fLifetime / fLifetimeMax)
        cDrawColor.G = GameMath.Lerp(cColor.G / 2 + 0.5F, cColor.G, fLifetime / fLifetimeMax)
        cDrawColor.B = GameMath.Lerp(cColor.B / 2 + 0.5F, cColor.B, fLifetime / fLifetimeMax)
        cDrawColor.A = GameMath.Lerp(1.0F, cColor.A, fLifetime / fLifetimeMax)
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
        GL.Color4(cDrawColor)
        'GL.BlendFunc(BlendingFactorSrc.Src1Color, BlendingFactorDest.OneMinusSrcColor)
        For i As Integer = 0 To vParticleMovement.Length - 1
            Draw2dRotated(gViewport, iTextureIdentification(0), vPositions(i), New Vector2(18, 1), vParticleMovement(i).Rotation)
        Next
        GL.Color4(1.0F, 1.0F, 1.0F, 1.0F)
    End Sub
End Class
