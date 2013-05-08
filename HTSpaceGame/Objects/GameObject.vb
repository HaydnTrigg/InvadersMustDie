Imports System.Windows.Forms
Imports System.Drawing

Public Class GameObject
    Public vPosition As Vector2
    Public vSize As Vector2

    Sub New(ByVal position As Vector2, ByVal size As Vector2)
        vPosition = position
        vSize = size
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
    Public iTextureID As Integer

    'Public oGun As New ShipGun(Me)

    Sub New(ByVal position As Vector2, ByVal size As Vector2)
        MyBase.New(position, size)
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

    Public vMovingVector As New Vector2(0, 0)

    Sub New(ByVal position As Vector2, ByVal size As Vector2)
        MyBase.New(position, size)
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overloads Sub Update(ByVal gGameTime As GameTime)
        vPosition += vMovingVector * gGameTime.ElapsedGameTime * 130

        'Update the ships gun
        'oGun.Update(gGameTime, Me)
    End Sub

End Class

Public Class ShipGun
    Inherits GameObject

    Public fRotation As Single
    Public iTextureID As Integer
    Public vOffset As Vector2
    Sub New(ByVal _Parent As Ship, ByVal _vOffset As Vector2)
        'Set the position to the parent
        MyBase.New(_Parent.vPosition, _Parent.vSize)
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
    Public iTextureID As Integer

    Sub New(ByVal position As Vector2, ByVal size As Vector2, ByVal direction As Vector2, ByVal rotation As Single, ByVal iTextureID As Integer)
        MyBase.New(position, size)
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overloads Sub Update(ByVal gGameTime As GameTime)
        vPosition.X += gGameTime.ElapsedGameTime
    End Sub
End Class