Imports System
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing
Public Class GameMath

    'Single precision clamp function for code optimisation
    Public Shared Function ClampF(ByVal value As Single, ByVal min As Single, ByVal max As Single) As Single
        'Create a brand new decimal/integer to avoid cross memory referencing
        Dim i As New Single
        'Assign the decimal/integer
        i = value
        'Clamp the deimal/integer based on min and max variables
        If (i > max) Then
            i = max
        End If
        If (i < min) Then
            i = min
        End If
        'Return the new integer ready for direct use in code
        Return i
    End Function

    'Single precision clamp function for code optimisation
    Public Shared Function Clamp(ByVal value As Vector2, ByVal min As Vector2, ByVal max As Vector2) As Vector2
        Return New Vector2(ClampF(value.X, min.X, max.X), ClampF(value.Y, min.Y, max.Y))
    End Function

    'Integer based clamp function for code optimisation
    Public Shared Function ClampI(ByVal value As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
        'Create a brand new decimal/integer to avoid cross memory referencing
        Dim i As New Integer
        'Assign the decimal/integer
        i = value
        'Clamp the deimal/integer based on min and max variables
        If (i > max) Then
            i = max
        End If
        If (i < min) Then
            i = min
        End If
        'Return the new integer ready for direct use in code
        Return i
    End Function

    'Generic double precision clamp useful for any instance in code. Will be automatically casted.
    Public Shared Function Clamp(ByVal value As Double, ByVal min As Double, ByVal max As Double) As Double
        'Create a brand new decimal/integer to avoid cross memory referencing
        Dim i As New Double
        'Assign the decimal/integer
        i = value
        'Clamp the deimal/integer based on min and max variables
        If (i > max) Then
            i = max
        End If
        If (i < min) Then
            i = min
        End If
        'Return the new integer ready for direct use in code
        Return i
    End Function

    'Linear Interpolates between the two numbers
    Public Shared Function Lerp(ByVal A As Single, ByVal B As Single, ByVal amount As Single) As Single
        Return A + (B - A) * amount
    End Function

    'Linear Interpolates between two Vector2
    Public Shared Function Lerp(ByVal A As Vector2, ByVal B As Vector2, ByVal amount As Single) As Vector2
        Return A + (B - A) * amount
    End Function

    Public Shared Function Vector2Distance(ByVal a As Vector2, ByVal b As Vector2) As Single
        'c = sqrt(a^2 + b^2)
        Return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2))
    End Function

    Public Shared Function Vector2DistanceSquared(ByVal a As Vector2, ByVal b As Vector2) As Single
        'c^2 = a^2 + b^2
        Return Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)
    End Function

    Public Shared Function NormalizeVector2(ByVal vector As Vector2) As Vector2
        Dim val As Single = 1.0F / CSng(Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y)))
        Return New Vector2(vector.X * val, vector.Y * val)
    End Function
End Class
