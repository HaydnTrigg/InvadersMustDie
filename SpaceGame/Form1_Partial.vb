




Partial Public Class Form1
    Private WithEvents UpdateTick As Timer
    Private WithEvents DrawTick As Timer
    Private ship As Ship

    Dim asteroids As New List(Of Asteroid)
    Dim bullets As New List(Of Bullet)

    Private SpawnRegulator As Single

    'Variables to help with creating bullets
    Dim isFireHeld As Boolean

    'Random Object
    Dim r As Random = New Random()

    'Stores the players score
    Dim score As UInt64
    Dim lblScore As Label

    'Load the "user32.dll" so the Update function can use the benefits of individual keys.
    Public Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal vKey As Int32) As UShort
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.DoubleBuffered = True
        Me.BackColor = Color.CornflowerBlue



        'Create the tick event at 20 Ticks per second. (20UPS)
        UpdateTick = New Timer()
        UpdateTick.Interval = 5
        UpdateTick.Start()

        'Create the DrawTick event at 60 Ticks per second. (60FPS)
        DrawTick = New Timer()
        DrawTick.Interval = 100 / 60
        DrawTick.Start()

        'Initialize the game
        Initialize()
    End Sub

    'PreUpdate Function
    Public Sub PreUpdate() Handles UpdateTick.Tick
        'Update the game
        Update()

        'Clearnup any bullets and asteroids no longer apart of the game
        IterateBulletsDistance()
        IterateAsteroidsDistance()
        IterateCollisions()
    End Sub

    Private Sub IterateBulletsDistance()
        For i As Integer = 0 To bullets.Count - 1
            If (bullets(i).Location.X - bullets(i).Width - 500 > Me.ClientSize.Width) Then
                Me.Controls.Remove(bullets(i))
                bullets.RemoveAt(i)
                i = bullets.Count
                'Iterate through the bullets again to see if there are any more that need removing
                IterateBulletsDistance()
            End If
        Next
    End Sub

    Private Sub IterateAsteroidsDistance()
        For i As Integer = 0 To asteroids.Count - 1
            If (asteroids(i).Location.X + asteroids(i).Width + 500 < 0) Then
                Me.Controls.Remove(asteroids(i))
                asteroids.RemoveAt(i)
                i = asteroids.Count
                'Iterate through the bullets again to see if there are any more that need removing
                IterateAsteroidsDistance()
            End If
        Next
    End Sub

    Private Sub IterateCollisions()
        For i As Integer = 0 To asteroids.Count - 1
            Dim ra As Rectangle = New Rectangle(asteroids(i).Location, asteroids(i).Size)

            For ii As Integer = 0 To bullets.Count - 1
                Dim rb As Rectangle = New Rectangle(bullets(ii).Location, bullets(ii).Size)
                If (ra.IntersectsWith(rb)) Then
                    score += Math.Round((asteroids(ii).Width * asteroids(ii).Height) / 200) * 100
                    Me.Controls.Remove(asteroids(i))
                    asteroids.RemoveAt(i)
                    Me.Controls.Remove(bullets(ii))
                    bullets.RemoveAt(ii)
                    i = asteroids.Count()
                    ii = bullets.Count()
                    IterateCollisions()
                End If
            Next
            Dim rs As Rectangle = New Rectangle(ship.Location, ship.Size)
            If (ra.IntersectsWith(rs)) Then
                'Enf of game, show the death menu with high scores.
            End If
        Next
    End Sub
End Class
