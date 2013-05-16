'http://alopex.li/wiki/QuadTree

#Region "Imports"

'System Imports
Imports System.Collections.Generic

'OpenTK Imports
Imports OpenTK

#End Region

Namespace Isotope.Library
    Public Class QuadTree(Of T)
#Region "Variables & Properties"

        Public Loc As Vector2
        Public Item As T
        Public UpperLeft As QuadTree(Of T)
        Public UpperRight As QuadTree(Of T)
        Public LowerLeft As QuadTree(Of T)
        Public LowerRight As QuadTree(Of T)

#End Region
#Region "Initializers"


        Public Sub New(ByVal loc__1 As Vector2, ByVal i As T)
            Item = i
            Loc = loc__1
            UpperLeft = Nothing
            UpperRight = Nothing
            LowerLeft = Nothing
            LowerRight = Nothing
        End Sub

#End Region
#Region "Main Functions"

        Public Sub Insert(ByVal loc__1 As Vector2, ByVal item As T)
            If loc__1.X > Loc.X Then
                If loc__1.Y > Loc.Y Then
                    If UpperRight Is Nothing Then
                        UpperRight = New QuadTree(Of T)(loc__1, item)
                    Else
                        UpperRight.Insert(loc__1, item)
                    End If
                Else
                    ' loc.Y <= Loc.Y
                    If LowerRight Is Nothing Then
                        LowerRight = New QuadTree(Of T)(loc__1, item)
                    Else
                        LowerRight.Insert(loc__1, item)
                    End If
                End If
            Else
                ' loc.X <= Loc.X
                If loc__1.Y > Loc.Y Then
                    If UpperLeft Is Nothing Then
                        UpperLeft = New QuadTree(Of T)(loc__1, item)
                    Else
                        UpperLeft.Insert(loc__1, item)
                    End If
                Else
                    ' loc.Y <= Loc.Y
                    If LowerLeft Is Nothing Then
                        LowerLeft = New QuadTree(Of T)(loc__1, item)
                    Else
                        LowerLeft.Insert(loc__1, item)
                    End If
                End If
            End If
        End Sub

        Public Function GetWithin(ByVal target As Vector2, ByVal d As Double) As List(Of T)
            Return GetWithin(target, d, New List(Of T)())
        End Function

        ' Returns a list of items within d of the target
        ' Upon consideration, it feels weird not making this tail-recursive.
        Public Function GetWithin(ByVal target As Vector2, ByVal d As Double, ByVal ret As List(Of T)) As List(Of T)
            Dim dsquared As Double = d * d
            ' First, we check and see if the current item is in range
            If GameMath.Vector2DistanceSquared(Loc, target) < dsquared Then
                ret.Add(Item)
            End If

            If target.X + d > Loc.X OrElse target.X > Loc.X Then
                If target.Y + d > Loc.Y OrElse target.Y > Loc.Y Then
                    If UpperRight IsNot Nothing Then
                        UpperRight.GetWithin(target, d, ret)
                    End If
                End If
                If target.Y - d <= Loc.Y OrElse target.Y <= Loc.Y Then
                    If LowerRight IsNot Nothing Then
                        LowerRight.GetWithin(target, d, ret)
                    End If
                End If
            End If
            If target.X - d <= Loc.X OrElse target.X <= Loc.X Then
                If target.Y + d > Loc.Y OrElse target.Y > Loc.Y Then
                    If UpperLeft IsNot Nothing Then
                        UpperLeft.GetWithin(target, d, ret)
                    End If
                End If
                If target.Y - d <= Loc.Y OrElse target.Y <= Loc.Y Then
                    If LowerLeft IsNot Nothing Then
                        LowerLeft.GetWithin(target, d, ret)
                    End If
                End If
            End If

            Return ret
        End Function

#End Region
    End Class
End Namespace