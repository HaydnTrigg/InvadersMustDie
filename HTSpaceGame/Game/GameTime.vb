Public Class GameTime

    Public TotalGameTime As Double
    Public ElapsedGameTime As Single

    Public Sub New(ByVal TimeSpanTotal As TimeSpan, ByVal TimeSpanElapsed As TimeSpan)
        TotalGameTime = TimeSpanTotal.TotalSeconds
        ElapsedGameTime = TimeSpanElapsed.TotalSeconds
    End Sub

End Class
