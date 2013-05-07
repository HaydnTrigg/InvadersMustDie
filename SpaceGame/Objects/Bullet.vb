Public Class Bullet
    Inherits Panel
    Public ObjectPosition As Vector2
    Public ObjectSpeed As Single
    Public ObjectDirection As Vector2
    Public TextureID As Integer

    'Update the object
    Public Sub UpdateObject(ByVal gameTime As GameTime)
        'Calculate the total time and the delta time.
        ObjectPosition += New Vector2(1, 0) * ObjectSpeed * gameTime.ElapsedGameTime
        'ObjectPosition = Math2D.Clamp(ObjectPosition + (ObjectDirection * ObjectSpeed * gameTime.ElapsedGameTime), New Vector2(0, 0), New Vector2(game.ClientSize.Width - Width, game.ClientSize.Height - Height))
        Location = New Point(ObjectPosition.X, ObjectPosition.Y)

    End Sub

    Public Sub New(ByVal position As Vector2, ByVal speed As Single, ByVal direction As Vector2, ByVal s As Size, ByVal i As Integer)
        Size = s
        TextureID = i
        ObjectPosition = position - New Vector2(Size.Width / 2, Size.Height / 2)
        Location = New Point(ObjectPosition.X, ObjectPosition.Y)
        ObjectSpeed = speed
        ObjectDirection = New Vector2(direction.X, direction.Y)
        BackColor = Color.FromArgb(0, 0, 0, 0)
        Me.DoubleBuffered = True
    End Sub
End Class
