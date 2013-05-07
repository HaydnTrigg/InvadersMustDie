Imports System.Windows.Forms
Imports System.Drawing

Public Class GameObject
    Public vPosition As Vector2
    Public vSize As Vector2

    Sub New(ByVal position As Vector2, ByVal size As Vector2)
        vPosition = position
        vSize = size
    End Sub

    Public Overridable Sub Update(ByVal gGameTime As GameTime)

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

    Sub New(ByVal position As Vector2, ByVal size As Vector2)
        MyBase.New(position, size)
        'Add extra code below
    End Sub

    'Override Update statement.
    Public Overrides Sub Update(ByVal gGameTime As GameTime)
        vPosition.X += gGameTime.ElapsedGameTime
    End Sub
End Class