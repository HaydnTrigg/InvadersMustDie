Namespace Isotope
    Public Class GameTime
#Region "Variables & Properties"

        'Store the total amount of game time
        Public TotalTime As Double
        'Store the total amount of time since the last update called Delta Time
        Public DeltaTime As Single

#End Region
#Region "Initializers"

        Public Sub New(ByVal TimeSpanTotal As TimeSpan, ByVal TimeSpanElapsed As TimeSpan)
            TotalTime = TimeSpanTotal.TotalSeconds / 1
            DeltaTime = TimeSpanElapsed.TotalSeconds / 1
        End Sub

#End Region
    End Class
End Namespace
