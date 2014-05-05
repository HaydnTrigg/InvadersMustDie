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

Public Class GameControll
    Enum ControllType
        Button = 0
        Other = 100
    End Enum
    Public eControllType As ControllType = ControllType.Other

    'Controll Game State
    Structure ControllGameState
        Public gPreviousKeyboardState, gCurrentKeyboardState As KeyboardState
        Public gPreviousMouseState, gCurrentMouseState As MouseState
        Public gViewport As Viewport
    End Structure
    Public gControllGameState As ControllGameState

    Public bIsHovering As Boolean

    Public cDrawColor As Color4


    'Stores the position of the Object
    Public vPosition As Vector2
    'Stores the size of the Object
    Public vSize As Vector2
    'Stores an array of integers used to indicate which textures the object should draw
    Public iTextureIdentification() As Integer

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        vPosition = _Position
        vSize = _Size
        iTextureIdentification = _TextureID
    End Sub

    'The Overridable Update Function which is avaliable to all Controll's
    Public Overridable Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
    End Sub
    'The Overridable Draw Function which is avaliable to all Controll's
    Public Overridable Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
    End Sub

End Class


'The object for main menu buttons
Public Class MenuButton
    Inherits GameControll

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)
        'Add extra code below
        eControllType = ControllType.Button
        cDrawColor = Color4.White
    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal delta As Single, ByVal gRandom As Random, ByVal _Target As Vector2)
        bIsHovering = False
        If CheckCollision(_Target, vPosition, vSize, gControllGameState.gViewport) Then
            cDrawColor.A = GameMath.ClampFloat(cDrawColor.A + 1.0F * delta, 0.5F, 1.0F)
            bIsHovering = True
        Else
            cDrawColor.A = GameMath.ClampFloat(cDrawColor.A - 1.0F * delta, 0.5F, 1.0F)
        End If
    End Sub

    Public Overrides Sub Draw(ByVal delta As Single, ByVal gViewport As Viewport)
        GL.Color4(cDrawColor)
        Draw2D(gViewport, iTextureIdentification(0), vPosition - vSize / 2, vSize)
        GL.Color4(Color4.White)
    End Sub

    Public Function CheckCollision(ByVal _Point1 As Vector2, ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Viewport As Viewport) As Boolean
        Dim objectrectangle As New System.Drawing.RectangleF(_Position.X - _Size.X / 2.0F, _Position.Y - _Size.Y / 2.0F, _Size.X, _Size.Y)
        If objectrectangle.IntersectsWith(New System.Drawing.RectangleF(_Point1.X, _Point1.Y, 1.0F, 1.0F)) Then
            Return True
        End If
        Return False
    End Function
End Class