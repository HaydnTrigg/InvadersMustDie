Public Class FPS

    Dim frameRate As Integer = 0
    Dim frameCounter As Integer = 0
    Dim elapsedTime As TimeSpan = TimeSpan.Zero

    Public Sub Update(ByVal gGameTime As GameTime)
        elapsedTime = TimeSpan.FromSeconds(elapsedTime.TotalSeconds + gGameTime.ElapsedGameTime)

        If (elapsedTime > TimeSpan.FromSeconds(1)) Then
            elapsedTime -= TimeSpan.FromSeconds(1)
            frameRate = frameCounter
            frameCounter = 0
            System.Diagnostics.Debug.WriteLine("" + frameRate.ToString())
        End If
    End Sub
    Public Sub Draw()
        frameCounter += 1

    End Sub


End Class
