Public Class Ship
    Inherits Panel
    Public ObjectPosition As Vector2
    Public ObjectSpeed As Single
    Public TextureID As Integer

    'Update the object
    Public Sub UpdateObject(ByVal game As Form1, ByVal gameTime As GameTime, ByVal ObjectDirection As Vector2)
        'Calculate the total time and the delta time.
        ObjectPosition = Math2D.Clamp(ObjectPosition + (ObjectDirection * ObjectSpeed * gameTime.ElapsedGameTime), New Vector2(0, 0), New Vector2(game.ClientSize.Width - Width, game.ClientSize.Height - Height))
        Location = New Point(ObjectPosition.X, ObjectPosition.Y)

    End Sub

    Public Sub New(ByVal position As Vector2, ByVal speed As Single)
        ObjectPosition = position
        ObjectSpeed = speed
        BackColor = Color.FromArgb(0, 0, 0, 0)
        Me.DoubleBuffered = True
    End Sub
End Class
