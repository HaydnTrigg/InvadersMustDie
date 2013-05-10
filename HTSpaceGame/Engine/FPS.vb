Public Class FPS

    Public iFrameRate As Integer
    Private iFrameCounter As Integer
    Private tsElapsedTime As TimeSpan = TimeSpan.Zero

    'Called when the update call is made
    Public Sub Update(ByVal gGameTime As GameTime)
        tsElapsedTime = TimeSpan.FromSeconds(tsElapsedTime.TotalSeconds + gGameTime.ElapsedGameTime)

        If (tsElapsedTime > TimeSpan.FromSeconds(1)) Then
            tsElapsedTime -= TimeSpan.FromSeconds(1)
            iFrameRate = iFrameCounter
            iFrameCounter = 0
        End If
    End Sub

    'Called when the draw call is made
    Public Sub Draw()
        iFrameCounter += 1
    End Sub
End Class
