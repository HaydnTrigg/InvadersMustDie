'Create the class Vector2 and does not allow another class to Inherit it.
Public NotInheritable Class Vector2
#Region "PublicField"
    'Create an X and a Y co-ordinate
    Public X As Single
    Public Y As Single

#End Region

#Region "PrivateField"
    Private Shared zeroVector As New Vector2(0.0F, 0.0F)
#End Region

#Region "Constructors"
    Public Sub New(ByVal _X As Single, ByVal _Y As Single)
        X = _X
        Y = _Y
    End Sub

    Public Sub New(ByVal _Value As Single)
        X = _Value
        Y = _Value
    End Sub
#End Region

#Region "Overrides"

    'Overrides the "ToString" Function allowing the correct display of this object to a String format.
    Public Overrides Function ToString() As String
        Dim sb As New System.Text.StringBuilder(32)
        sb.Append("{X:")
        sb.Append(Me.X)
        sb.Append(" Y:")
        sb.Append(Me.Y)
        sb.Append("}")
        Return sb.ToString()
    End Function
#End Region

#Region "Operators"
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

    'Vector2 - Single
    Public Shared Operator -(ByVal Param1 As Vector2, ByVal Param2 As Single) As Vector2
        Return New Vector2(Param1.X - Param2, Param1.Y - Param2)
    End Operator

    'Vector2 + Single
    Public Shared Operator +(ByVal Param1 As Vector2, ByVal Param2 As Single) As Vector2
        Return New Vector2(Param1.X + Param2, Param1.Y + Param2)
    End Operator

    'Vector2 * Single
    Public Shared Operator /(ByVal Param1 As Vector2, ByVal Param2 As Single) As Vector2
        If (Param2 = 0) Then
            Param2 = 1
        End If
        Return New Vector2(Param1.X / Param2, Param1.Y / Param2)
    End Operator
#End Region

#Region "Functions"

    'Calculates a Rotation from a Normalized Vector2, in Radians
    Public Function Rotation() As Single
        'Calculate the rotation and cast to single precision
        Rotation = CType(Math.Atan2(Y, -X), Single)
    End Function

    'Self normalize the Vector2
    Public Sub Normalize()
        Dim val As Single = 1.0F / CSng(Math.Sqrt((X * X) + (Y * Y)))
        X *= val
        Y *= val
    End Sub


#End Region






End Class
