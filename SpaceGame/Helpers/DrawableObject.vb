Public Class DrawableObject
    Inherits PictureBox

    Public TextureID As Integer
    Public DrawTarget As PictureBox
    Public Sub New(ByVal tid As Integer)
        TextureID = tid
        DrawTarget = New PictureBox()

    End Sub

    'Destroys all of the objects sub objects and ensures that it is no longer apart of the visual world.
    Public Sub Dispose()
        DrawTarget.Visible = False
    End Sub

    'Ensures that this Drawable Object is ontop
    Public Sub Draw(ByVal img As Image, ByVal ObjectPosition As Vector2)
        DrawTarget.Size = img.Size
        DrawTarget.BackColor = Color.FromArgb(0, 0, 0, 0)
        DrawTarget.Location = New Point(ObjectPosition.X, ObjectPosition.Y)
        DrawTarget.BackgroundImage = img
        DrawTarget.BringToFront()
    End Sub
End Class
