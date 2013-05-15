Imports System.Collections.Generic
Imports OpenTK
Imports HTSpaceGame.Isotope

Namespace Isotope.Quadtree
    Public Class QuadTree(Of T)
        Public Loc As Vector2
        Public Item As T
        Public UpperLeft As QuadTree(Of T)
        Public UpperRight As QuadTree(Of T)
        Public LowerLeft As QuadTree(Of T)
        Public LowerRight As QuadTree(Of T)

        Public Sub New(ByVal loc__1 As Vector2, ByVal i As T)
            Item = i
            Loc = loc__1
            UpperLeft = Nothing
            UpperRight = Nothing
            LowerLeft = Nothing
            LowerRight = Nothing
        End Sub

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
            If VectorMath.Vector2DistanceSquared(Loc, target) < dsquared Then
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

#Region "Static methods"
        'Public Const Leaf As QuadTree(Of T) = Nothing

        Public Shared Function Build(ByVal locfunc As Func(Of T, Vector2), ByVal items As ICollection(Of T)) As QuadTree(Of T)
            ' Gotta sorta hand-start the iterator...
            Dim e As IEnumerator(Of T) = items.GetEnumerator()
            e.MoveNext()
            Dim q As New QuadTree(Of T)(locfunc(e.Current), e.Current)
            While e.MoveNext()
                q.Insert(locfunc(e.Current), e.Current)
            End While
            Return q
        End Function

        ' I was going to try different methods of building the tree, like taking the items collection
        ' and sorting or splitting it into bits and then inserting each separately...
        ' ...but the current method works REALLY WELL anyway.
        '
        '                public static QuadTree<T> Build(Func<T, Vector2d> locfunc, ICollection<T> items) {
        '                      return null;
        '             }
        '              

#End Region
    End Class
End Namespace