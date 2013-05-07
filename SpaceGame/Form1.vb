Imports System.Windows.Forms

'Create a partial class so it can be simplified with another class.
Partial Public Class Form1
    'Store the time information.
    Dim StartTime As DateTime = DateTime.Now() 'Total time
    Dim LastTime As DateTime = DateTime.Now() 'Total time last update
    'Returns a GameTime object storing the time information in an easy to use interface.
    ReadOnly Property getGameTime As GameTime
        Get
            Dim Now As DateTime = DateTime.Now
            Dim gameTime As GameTime = New GameTime(Now - StartTime, Now - LastTime)
            LastTime = Now
            Return gameTime
        End Get
    End Property

    'Initialized all of the games components
    Private Sub Initialize()
        'Initialize all of the games variables
        ship = New Ship(New Vector2(30, Me.Height / 2), 50.0F * UpdateTick.Interval)
        ship.Size = Textures(ship.TextureID).Size
        'Register all of the objects in the game
        For Each a As Asteroid In asteroids
            Me.Controls.Add(a)
        Next
        For Each b As Bullet In bullets
            Me.Controls.Add(b)
        Next
        Me.Controls.Add(ship)

        lblScore = New Label()
        lblScore.ForeColor = Color.FromArgb(255, 0, 255, 0)
        lblScore.Font = New Font("System", 18)
        lblScore.Location = New Point(5, 5)
        lblScore.Size = Me.Size
        Me.Controls.Add(lblScore)


        'background1 = New PictureBox
        'PictureBox2 = New PictureBox
        'background1.BackgroundImage = Textures(3)
        'PictureBox2.BackgroundImage = Textures(3)
        'background1.Size = Textures(3).Size
        'PictureBox2.Size = Textures(3).Size
        'background1.Location = New Point(0, 0)
        'PictureBox2.Location = New Point(PictureBox2.Width, 0)
        'background1.BackColor = Color.Red
        'PictureBox2.BackColor = Color.Red

        'Load all of the games content
    End Sub

    'Updates all of the objects and draws them to the screen.
    Private Sub Update()
        'Calculate the game time for use this tick.
        Dim gameTime As GameTime = getGameTime

        'Slowly speed up the game to make it harder : )
        gameTime.ElapsedGameTime *= 1 + Math2D.Clamp(Math.Pow(gameTime.TotalGameTime / 200, 1.5D), 0, 50)

        SpawnRegulator += gameTime.ElapsedGameTime

        'Update all of the objects in the game using their own update methods
        For Each a As Asteroid In asteroids
            a.UpdateObject(gameTime)
        Next
        For Each b As Bullet In bullets
            b.UpdateObject(gameTime)
        Next

        Dim d As Vector2 = New Vector2(0, 0)
        If GetAsyncKeyState(Convert.ToInt32(Keys.W)) Then
            d.Y -= 1
        End If
        If GetAsyncKeyState(Convert.ToInt32(Keys.S)) Then
            d.Y += 1
        End If
        If GetAsyncKeyState(Convert.ToInt32(Keys.A)) Then
            d.X -= 1
        End If
        If GetAsyncKeyState(Convert.ToInt32(Keys.D)) Then
            d.X += 1
        End If

        If GetAsyncKeyState(Convert.ToInt32(Keys.Space)) Then
            If Not isFireHeld Then
                'Create and add a new bullet to the list
                Dim b As Bullet = New Bullet(New Vector2(ship.ObjectPosition.X + ship.Size.Width, ship.ObjectPosition.Y + ship.Size.Height / 2), 800, New Vector2(1, 0), Textures(1).Size, 1)
                Me.Controls.Add(b)
                bullets.Add(b)
            End If
            isFireHeld = True
        Else
            isFireHeld = False
        End If
        If SpawnRegulator > 0.35F Then
            SpawnRegulator = 0
            If (asteroids.Count < 8) Then

                Dim a As Asteroid = New Asteroid(New Vector2(Me.ClientSize.Width + 200, r.NextDouble * (Me.ClientSize.Height - 128) + 64), 250, New Vector2(-1, r.NextDouble * 0.2D), New Size(Textures(2).Size.Height * r.NextDouble() + 32, Textures(2).Size.Width * r.NextDouble() + 32), 2)
                a.BackgroundImageLayout = ImageLayout.Stretch
                Me.Controls.Add(a)
                asteroids.Add(a)
            End If
        End If
        SpawnRegulator += gameTime.ElapsedGameTime
        ship.UpdateObject(Me, gameTime, d)

        lblScore.Text = "Score: " + score.ToString() + vbNewLine + "Speed: " + (1 + Math2D.Clamp(Math.Pow(gameTime.TotalGameTime / 200, 1.5D), 0, 50)).ToString()
        'If (background1.Location.X + background1.Width <= 0) Then
        'background1.Location = New Point(0, 0)
        'End If
        'background1.Location = New Point(background1.Location.X - 1, 0)
        'PictureBox2.Location = New Point(background1.Location.X - 1 + background2.Width, 0)
    End Sub
    Dim a As Boolean = True
    Dim Textures() As Image = New Image() {CType(My.Resources.ResourceManager.GetObject("ship"), Image), CType(My.Resources.ResourceManager.GetObject("bullet"), Image), CType(My.Resources.ResourceManager.GetObject("asteroid"), Image), CType(My.Resources.ResourceManager.GetObject("spacebackgound"), Image)}
    'Handels all of the objects and draws them to the screen.
    Private Sub Draw() Handles DrawTick.Tick

        'Draw all of the objects in the game using their own DrawableObjects.
        For Each a As Asteroid In asteroids
            a.BackgroundImage = Textures(a.TextureID)
            a.Parent = Me
            a.BringToFront()
        Next
        For Each b As Bullet In bullets
            b.BackgroundImage = Textures(b.TextureID)
            b.Parent = Me
            b.BringToFront()
        Next
        ship.BackgroundImage = Textures(ship.TextureID)
        ship.Parent = Me
        ship.BringToFront()
    End Sub
End Class
