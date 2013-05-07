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

    'Integer based clamp function for code optimisation
    Public Shared Function ClampI(ByVal value As Single, ByVal min As Single, ByVal max As Single) As Single
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


End Class
