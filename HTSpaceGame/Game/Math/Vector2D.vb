Imports System.Windows.Forms
Imports System.Drawing

Public Class Vector2
    Public X As Single
    Public Y As Single

    Public Sub New(ByVal xcord As Single, ByVal ycord As Single)
        X = xcord
        Y = ycord
    End Sub

    Public Shared Operator -(ByVal Param1 As Vector2, ByVal Param2 As Vector2) As Vector2
        Return New Vector2(Param1.X - Param2.X, Param1.Y - Param2.Y)
    End Operator

    Public Shared Operator +(ByVal Param1 As Vector2, ByVal Param2 As Vector2) As Vector2
        Return New Vector2(Param1.X + Param2.X, Param1.Y + Param2.Y)
        Return Param1
    End Operator

    Public Shared Operator /(ByVal Param1 As Vector2, ByVal Param2 As Vector2) As Vector2
        'Never divide by 0
        If (Param2.X <> 0) Then
            Param2.X = 1
        End If
        If (Param2.Y <> 0) Then
            Param2.Y = 1
        End If
        Return New Vector2(Param1.X / Param2.X, Param1.Y / Param2.Y)
    End Operator

    Public Shared Operator *(ByVal Param1 As Vector2, ByVal Param2 As Vector2) As Vector2
        Return New Vector2(Param1.X * Param2.X, Param1.Y * Param2.Y)
    End Operator

    'Vector2 * Single
    Public Shared Operator *(ByVal Param1 As Vector2, ByVal Param2 As Single) As Vector2
        Return New Vector2(Param1.X * Param2, Param1.Y * Param2)
    End Operator

    Public Function createPoint(ByVal v As Vector2) As Point
        Return New Point(v.X, v.Y)
    End Function

    Public Function createPoint() As Point
        Return New Point(X, Y)
    End Function

    Public Function createPointF(ByVal v As Vector2) As PointF
        Return New PointF(v.X, v.Y)
    End Function

    Public Function createPointF() As PointF
        Return New PointF(X, Y)
    End Function

    Public Function createSize(ByVal v As Vector2) As Size
        Return New Size(v.X, v.Y)
    End Function

    Public Function createSize() As Size
        Return New Size(X, Y)
    End Function

    Public Function createSizeF(ByVal v As Vector2) As SizeF
        Return New SizeF(v.X, v.Y)
    End Function

    Public Function createSizeF() As SizeF
        Return New SizeF(X, Y)
    End Function



End Class
