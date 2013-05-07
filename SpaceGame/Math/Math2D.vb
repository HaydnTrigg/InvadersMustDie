Public Class Math2D

    'Linear Interpolates between the two numbers
    Public Shared Function Lerp(ByVal A As Single, ByVal B As Single, ByVal amount As Single) As Single
        Return A + (B - A) * amount
    End Function

    'Linear Interpolates between two Vector2
    Public Shared Function Lerp(ByVal A As Vector2, ByVal B As Vector2, ByVal amount As Single) As Vector2
        Return A + (B - A) * amount
    End Function

    'Single Precision Clamp
    Public Shared Function Clamp(ByVal Value As Single, ByVal Min As Single, ByVal Max As Single) As Single
        If (Value > Max) Then
            Value = Max
        End If
        If (Value < Min) Then
            Value = Min
        End If
        Return Value
    End Function

    'Vector2 Clamp
    Public Shared Function Clamp(ByVal Value As Vector2, ByVal Min As Vector2, ByVal Max As Vector2) As Vector2
        Value.X = Math2D.Clamp(Value.X, Min.X, Max.X)
        Value.Y = Math2D.Clamp(Value.Y, Min.Y, Max.Y)
        Return Value
    End Function

    Public Shared Function NormalizeVector2(ByVal vector As Vector2) As Vector2
        Dim val As Single = 1.0F / Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y))
        vector.X *= val
        vector.Y *= val
        Return vector
    End Function

End Class
