Imports System.Drawing
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
Namespace IsotopeVB
    Public Class GraphicsMath



        ' Draws an image onto the given graphics object. The image is rotated by a specified angle
        ' (in radians) around its center and then drawn at the given center point.
        Public Shared Sub DrawImageRotatedAroundCenter(ByVal _Viewport As Viewport, ByVal g As Graphics, ByVal r As Rectangle, ByVal img As Bitmap, ByVal angle As Double)
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



        'Rotates a point around an origin(p) by an angle in radians.
        Private Shared Function RotatePoint(ByVal p As Vector2, ByVal angle As Double) As Vector2
            Return New Vector2(p.X * Math.Cos(angle) + p.Y * Math.Sin(angle), -p.X * Math.Sin(angle) + p.Y * Math.Cos(angle))
        End Function

        Public Shared Sub Draw2d(ByVal _Viewport As Viewport, ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2)
            Dim vRelativePosition As Vector2 = _Position - _Viewport.ViewportPosition

            GL.BindTexture(TextureTarget.Texture2D, _TextureID)
            GL.Begin(BeginMode.Quads)
            GL.TexCoord2(0.0, 0.0)
            GL.Vertex2(vRelativePosition.X, vRelativePosition.Y)

            GL.TexCoord2(1.0, 0.0)
            GL.Vertex2(vRelativePosition.X + _Size.X, vRelativePosition.Y)

            GL.TexCoord2(1.0, 1.0)
            GL.Vertex2(vRelativePosition.X + _Size.X, vRelativePosition.Y + _Size.Y)

            GL.TexCoord2(0.0, 1.0)
            GL.Vertex2(vRelativePosition.X, vRelativePosition.Y + _Size.Y)

            GL.End()

        End Sub

        Public Shared Sub DrawBackbuffer(ByVal _Position As Vector2, ByVal _Size As Vector2)

            GL.Begin(BeginMode.Quads)
            GL.TexCoord2(0.0, 0.0)
            GL.Vertex2(_Position.X, _Position.Y)

            GL.TexCoord2(1.0, 0.0)
            GL.Vertex2(_Position.X + _Size.X, _Position.Y)

            GL.TexCoord2(1.0, 1.0)
            GL.Vertex2(_Position.X + _Size.X, _Position.Y + _Size.Y)

            GL.TexCoord2(0.0, 1.0)
            GL.Vertex2(_Position.X, _Position.Y + _Size.Y)

            GL.End()

        End Sub

        Public Shared Sub Draw2dRotated(ByVal _Viewport As Viewport, ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Rotation As Single)
            Dim vRelativePosition As Vector2 = _Position - _Viewport.ViewportPosition
            Dim vTemp As Vector2

            GL.BindTexture(TextureTarget.Texture2D, _TextureID)
            GL.Begin(BeginMode.Quads)
            GL.TexCoord2(0.0, 0.0)
            vTemp = RotatePoint(New Vector2(-_Size.X / 2, -_Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)

            GL.TexCoord2(1.0, 0.0)
            vTemp = RotatePoint(New Vector2(_Size.X / 2, -_Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)

            GL.TexCoord2(1.0, 1.0)
            vTemp = RotatePoint(New Vector2(_Size.X / 2, _Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)

            GL.TexCoord2(0.0, 1.0)
            vTemp = RotatePoint(New Vector2(-_Size.X / 2, _Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)

            GL.End()

        End Sub
    End Class
End Namespace
