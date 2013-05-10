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

Public Class SquareEnemy
    Inherits GameObject

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Random As Random, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)

        iTextureIdentification = _TextureID

        'Setup the size of the Square Enemy o 60x60
        vSize = New Vector2(60, 60)
        'Set the Entity rotation to 0
        fRotation = 0
        'Set the Entity Speed between 100 and 140
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