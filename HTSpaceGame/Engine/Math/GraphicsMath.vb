Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
Namespace IsotopeVB
    Public Class GraphicsMath

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

        'Based from: http://jelle.druyts.net/2004/05/26/RotatingAnImageAroundItsCenterInNET.aspx
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
