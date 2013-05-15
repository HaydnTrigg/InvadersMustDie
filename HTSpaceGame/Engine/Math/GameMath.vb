
' ----------------------------------------------------------------------------
' "THE BEER-WARE LICENSE" (Revision 42):
' <Haydn Trigg@Vagyr> wrote this file. As long as you retain this notice you
' can do whatever you want with this stuff. If we meet some day, and you think
' this stuff is worth it, you can buy me a beer in return Haydn-Richard Trigg
' ----------------------------------------------------------------------------

'Visual Basic Isotope Framework by Haydn Trigg
'Written by: Haydn Trigg
'Created : 2/5/2013 [dd/mm/yyyy]
'Modified: 15/5/2013 [dd/mm/yyyy]
'Version : 0.20.1
'Isotope General GameMath Library
'Includes many useful functions that can aid the ease of developing a game.

Imports OpenTK
Imports OpenTK.Graphics.OpenGL
Imports HTSpaceGame.IsotopeVB

Namespace Isotope
    'General Purpose Math Functions
    'Needs Optimisation
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
        Public Shared Function ClampV(ByVal value As Vector2, ByVal min As Vector2, ByVal max As Vector2) As Vector2
            Return New Vector2(ClampF(value.X, min.X, max.X), ClampF(value.Y, min.Y, max.Y))
        End Function

        'Single precision clamp function for code optimisation taking some single values instead of Vector2's
        Public Shared Function ClampVS(ByVal value As Vector2, ByVal minX As Single, ByVal minY As Single, ByVal maxX As Single, ByVal maxY As Single) As Vector2
            Return New Vector2(ClampF(value.X, minX, maxX), ClampF(value.Y, minY, maxY))
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
        Public Shared Function ClampD(ByVal value As Double, ByVal min As Double, ByVal max As Double) As Double
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

    End Class

    Public Class VectorMath
        'Optimized 15/5/2013
        'Returns the distance between two Vector2 points.
        Public Shared Function Vector2Distance(ByVal a As Vector2, ByVal b As Vector2) As Single
            'c = sqrt(a^2 + b^2)
            Return Math.Sqrt(Vector2DistanceSquared(a, b))
        End Function

        'Optimized 15/5/2013
        'Return the distance squared between two Vector2 points.
        Public Shared Function Vector2DistanceSquared(ByVal a As Vector2, ByVal b As Vector2) As Single
            'c^2 = a^2 + b^2
            Return a.X * a.X - b.X * b.X + a.Y * a.Y - b.Y * b.Y
        End Function

        Public Shared Function NormalizeVector2(ByVal vector As Vector2) As Vector2
            Dim val As Single = 1.0F / CSng(Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y)))
            
            Return New Vector2(vector.X * val,vector.Y * val)
        End Function
    End Class

    Public Class GraphicsMath




    End Class
End Namespace
