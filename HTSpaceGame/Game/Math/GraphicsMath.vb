Imports System.Drawing
Public Class GraphicsMath



    ' Draws an image onto the given graphics object. The image is rotated by a specified angle
    ' (in radians) around its center and then drawn at the given center point.
    Public Shared Sub DrawImageRotatedAroundCenter(ByVal g As Graphics, ByVal r As Rectangle, ByVal img As Bitmap, ByVal angle As Double)
        Dim center As New Vector2(r.X, r.Y)
        ' Think of the image as a rectangle that needs to be drawn rotated.
        ' Rotate the coordinates of the rectangle's corners.
        Dim upperLeft As Vector2 = RotatePoint(New Vector2(-r.Width / 2, r.Height / 2), angle)
        Dim upperRight As Vector2 = RotatePoint(New Vector2(r.Width / 2, r.Height / 2), angle)
        Dim lowerLeft As Vector2 = RotatePoint(New Vector2(-r.Width / 2, -r.Height / 2), angle)

        ' Create the points array by offsetting the coordinates with the center.
        Dim points() As PointF = {(upperLeft + center).createPointF, (upperRight + center).createPointF, (lowerLeft + center).createPointF}

        ' Draw the rotated image.
        g.DrawImage(img, points)
    End Sub

    ' Draws an image onto the given graphics object. The image is rotated by a specified angle
    ' (in radians) around its center and then drawn at the given center point.
    Public Shared Sub DrawImageRotatedAroundCenter(ByVal g As GraphicsDevice, ByVal r As RectangleF, ByVal img As Bitmap, ByVal angle As Double)


        'Check if the object is on the screen, this will optimise everything and only draw what is needed
        'r is the real location of the rectangle. Calculate the intersection based on the bottom right corner so it is always visible and wont dissappear half
        'way down at the bottom or the right side of the screen. Also adjust for the View Position
        If (r).IntersectsWith(New RectangleF(g.ViewPosition.X + r.Width / 2, g.ViewPosition.Y + r.Width / 2, g.Width, g.Height)) Then

            'Apply the view position offset to the draw position
            Dim center As New Vector2(r.X - g.ViewPosition.X, r.Y - g.ViewPosition.Y)
            ' Think of the image as a rectangle that needs to be drawn rotated.
            ' Rotate the coordinates of the rectangle's corners.
            Dim upperLeft As Vector2 = RotatePoint(New Vector2(-r.Width / 2, r.Height / 2), angle)
            Dim upperRight As Vector2 = RotatePoint(New Vector2(r.Width / 2, r.Height / 2), angle)
            Dim lowerLeft As Vector2 = RotatePoint(New Vector2(-r.Width / 2, -r.Height / 2), angle)

            ' Create the points array by offsetting the coordinates with the center.
            Dim points() As PointF = {(upperLeft + center).createPointF, (upperRight + center).createPointF, (lowerLeft + center).createPointF}

            ' Draw the rotated image.
            g.SpriteBatch.DrawImage(img, points)
        End If
    End Sub

    'Rotates a point around an origin(p) by an angle in radians.
    Private Shared Function RotatePoint(ByVal p As Vector2, ByVal angle As Double) As Vector2
        Return New Vector2(p.X * Math.Cos(angle) + p.Y * Math.Sin(angle), -p.X * Math.Sin(angle) + p.Y * Math.Cos(angle))
    End Function
End Class
