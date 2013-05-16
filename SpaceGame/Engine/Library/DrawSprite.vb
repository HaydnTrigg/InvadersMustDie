Imports Isotope.Library
Imports OpenTK.Graphics.OpenGL

Namespace Isotope.Library
    Public Class DrawSprite
        Public Shared Sub Draw2D(ByVal _Viewport As Viewport, ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2)
            Dim vRelativePosition As Vector2 = _Position - _Viewport.Position

            'Assign's the current texture to the graphics device.
            GL.BindTexture(TextureTarget.Texture2D, _TextureID)

            'Begins drawing Quads
            GL.Begin(BeginMode.Quads)

            'Creates a quad with some properties and calculates the rotation
            GL.TexCoord2(0.0F, 0.0F)
            GL.Vertex2(vRelativePosition.X, vRelativePosition.Y)
            GL.TexCoord2(1.0F, 0.0F)
            GL.Vertex2(vRelativePosition.X + _Size.X, vRelativePosition.Y)
            GL.TexCoord2(1.0F, 1.0F)
            GL.Vertex2(vRelativePosition.X + _Size.X, vRelativePosition.Y + _Size.Y)
            GL.TexCoord2(0.0F, 1.0F)
            GL.Vertex2(vRelativePosition.X, vRelativePosition.Y + _Size.Y)

            'End drawing ready for another draw call.
            GL.End()
        End Sub

        Public Shared Sub DrawBackbuffer(ByVal _Position As Vector2, ByVal _Size As Vector2)
            'Begins drawing Quads
            GL.Begin(BeginMode.Quads)

            'Creates a quad with some properties
            GL.TexCoord2(0.0F, 0.0F)
            GL.Vertex2(_Position.X, _Position.Y)
            GL.TexCoord2(1.0F, 0.0F)
            GL.Vertex2(_Position.X + _Size.X, _Position.Y)
            GL.TexCoord2(1.0F, 1.0F)
            GL.Vertex2(_Position.X + _Size.X, _Position.Y + _Size.Y)
            GL.TexCoord2(0.0F, 1.0F)
            GL.Vertex2(_Position.X, _Position.Y + _Size.Y)

            'End drawing ready for another draw call.
            GL.End()
        End Sub


        'Based from: http://jelle.druyts.net/2004/05/26/RotatingAnImageAroundItsCenterInNET.aspx
        Public Shared Sub Draw2dRotated(ByVal _Viewport As Viewport, ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Rotation As Single)
            Dim vRelativePosition As Vector2 = _Position - _Viewport.Position
            'Creates a temporary storate for a Vector2
            Dim vTemp As Vector2

            'Assign's the current texture to the graphics device.
            GL.BindTexture(TextureTarget.Texture2D, _TextureID)

            'Begins drawing Quads
            GL.Begin(BeginMode.Quads)

            'Creates a quad with some properties and calculates the rotation
            GL.TexCoord2(0.0F, 0.0F)
            vTemp = RotatePoint(New Vector2(-_Size.X / 2, -_Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)
            GL.TexCoord2(1.0F, 0.0F)
            vTemp = RotatePoint(New Vector2(_Size.X / 2, -_Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)
            GL.TexCoord2(1.0F, 1.0F)
            vTemp = RotatePoint(New Vector2(_Size.X / 2, _Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)
            GL.TexCoord2(0.0F, 1.0F)
            vTemp = RotatePoint(New Vector2(-_Size.X / 2, _Size.Y / 2), _Rotation) + vRelativePosition
            GL.Vertex2(vTemp.X, vTemp.Y)

            'End drawing ready for another draw call.
            GL.End()
        End Sub

        'Mathematic Information: http://www.puz.com/sw/amorphous/theory/
        'Rotates a Vector2 position as a point on an angle
        Private Shared Function RotatePoint(ByVal p As Vector2, ByVal angle As Double) As Vector2
            Return New Vector2(p.X * Math.Cos(angle) + p.Y * Math.Sin(angle), -p.X * Math.Sin(angle) + p.Y * Math.Cos(angle))
        End Function
    End Class
End Namespace