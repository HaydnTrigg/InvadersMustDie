Imports System.Windows.Forms
Imports System.Drawing

Public Class GameObject
    Public vPosition As Vector2
    Public vSize As Vector2
    Public iTextureID As Integer

    Sub New(ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2)
        vPosition = _Position
        vSize = _Size
        iTextureID = _TextureID
    End Sub

    Public Sub Update()
    End Sub

    Public Function createRenderBox() As RenderBox
        Dim p As New RenderBox
        p.Location = vPosition.createPoint()
        p.Size = vSize.createSize()
        p.BackColor = Color.Red
        p.TextureID = 0
        Return p
    End Function

End Class

Public Class Ship
    Inherits GameObject

    Public fRotation As Single


    'Public oGun As New ShipGun(Me)

    Sub New(ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2)
        MyBase.New(_TextureID, _Position, _Size)
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overloads Sub Update(ByVal gGameTime As GameTime)
        vPosition.X += gGameTime.ElapsedGameTime


        'Update the ships gun
        'oGun.Update(gGameTime, Me)
    End Sub
End Class

Public Class PlayerShip
    Inherits Ship

    Public vSpeed As Single

    Sub New(ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2)
        MyBase.New(_TextureID, _Position, _Size)
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overloads Sub Update(ByVal gGameTime As GameTime, ByVal _MousePosition As Vector2)
        If GameMath.Vector2Distance(_MousePosition, vPosition + vSize / 2) > 34 Then

            Dim movement As Vector2 = _MousePosition - vPosition - vSize / 2
            If (movement.X = 0 And movement.Y = 0) Then
                movement = New Vector2(0, 1)
            End If
            movement.Normalize()





            vPosition += movement * (gGameTime.ElapsedGameTime * 130) * Math.Sqrt(Math.Sqrt(Math.Sqrt(Math.Sqrt(GameMath.Vector2Distance(_MousePosition, vPosition - vSize / 2)))))
        End If
        'Update the ships gun
        'oGun.Update(gGameTime, Me)
    End Sub

End Class

Public Class ShipGun
    Inherits GameObject

    Public fRotation As Single
    Public vOffset As Vector2

    Sub New(ByVal _TextureID As Integer, ByVal _Parent As Ship, ByVal _vOffset As Vector2)
        'Set the position to the parent
        MyBase.New(_TextureID, _Parent.vPosition, _Parent.vSize)
        vOffset = _vOffset
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overloads Sub Update(ByVal gGameTime As GameTime, ByVal _Parent As Ship)
        vPosition = _Parent.vPosition + vOffset
    End Sub
End Class

Public Class Bullet
    Inherits GameObject

    Public vDirection As Vector2
    Public fRotation As Single

    Sub New(ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2)
        MyBase.New(_TextureID, _Position, _Size)
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overloads Sub Update(ByVal gGameTime As GameTime)
        vPosition.X += gGameTime.ElapsedGameTime
    End Sub
End Class