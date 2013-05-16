Imports Isotope
Imports OpenTK.Input

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
    End Structure
    Public gControllGameState As ControllGameState

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
    Public Overridable Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
    End Sub
    'The Overridable Draw Function which is avaliable to all Controll's
    Public Overridable Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
    End Sub

End Class


'The object for main menu buttons
Public Class MenuButton
    Inherits GameControll

    Sub New(ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _TextureID() As Integer)
        MyBase.New(_Position, _Size, _TextureID)
        'Add extra code below
        eControllType = ControllType.Button
    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime, ByVal gRandom As Random, ByVal _Target As Vector2)
    End Sub

    Public Overrides Sub Draw(ByVal gGameTime As GameTime, ByVal gViewport As Viewport)
    End Sub
End Class