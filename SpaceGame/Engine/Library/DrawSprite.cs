using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Isotope.Library;
using OpenTK.Graphics.OpenGL;

namespace Isotope.Library
{
    public class DrawSprite
    {

        public static void BindTexture2D(int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        }

        public static void Draw2D(Viewport _Viewport, int _TextureID, Vector2 _Position, Vector2 _Size)
        {
            Vector2 vRelativePosition = _Position - _Viewport.Position;

            //Assign's the current texture to the graphics device.
            BindTexture2D(_TextureID);

            //Begins drawing Quads
            GL.Begin(BeginMode.Quads);

            //Creates a quad with some properties and calculates the rotation
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(vRelativePosition.X, vRelativePosition.Y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(vRelativePosition.X + _Size.X, vRelativePosition.Y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(vRelativePosition.X + _Size.X, vRelativePosition.Y + _Size.Y);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(vRelativePosition.X, vRelativePosition.Y + _Size.Y);

            //End drawing ready for another draw call.
            GL.End();
        }

        public static void Draw2DOnscreen(Viewport _Viewport, int _TextureID, Vector2 _Position, Vector2 _Size, bool _Center)
        {
            Vector2 v = _Size;
            v /= _Viewport.ViewportScale;

            //Assign's the current texture to the graphics device.
            BindTexture2D(_TextureID);
            //Begins drawing Quads
            GL.Begin(BeginMode.Quads);
            if (_Center)
            {
                //Creates a quad with some properties
                GL.TexCoord2(0f, 0f);
                GL.Vertex2(_Position.X - _Size.X / 2f, _Position.Y - _Size.Y / 2f);
                GL.TexCoord2(1f, 0f);
                GL.Vertex2(_Position.X + _Size.X / 2f, _Position.Y - _Size.Y / 2f);
                GL.TexCoord2(1f, 1f);
                GL.Vertex2(_Position.X + _Size.X / 2f, _Position.Y + _Size.Y / 2f);
                GL.TexCoord2(0f, 1f);
                GL.Vertex2(_Position.X - _Size.X / 2f, _Position.Y + _Size.Y / 2f);
            }
            else
            {
                //Creates a quad with some properties
                GL.TexCoord2(0f, 0f);
                GL.Vertex2(_Position.X, _Position.Y);
                GL.TexCoord2(1f, 0f);
                GL.Vertex2(_Position.X + _Size.X, _Position.Y);
                GL.TexCoord2(1f, 1f);
                GL.Vertex2(_Position.X + _Size.X, _Position.Y + _Size.Y);
                GL.TexCoord2(0f, 1f);
                GL.Vertex2(_Position.X, _Position.Y + _Size.Y);
            }
            //End drawing ready for another draw call.
            GL.End();
        }

        //public Shared Sub Draw2dRotated(ByVal _Viewport As Viewport, ByVal _TextureID As Integer, ByVal _Position As Vector2, ByVal _Size As Vector2, ByVal _Rotation As Single)
        //    Dim vRelativePosition As Vector2 = _Position - _Viewport.Position

        //    ' Points around origin 0,0 using the size of the quad
        //    Dim P0 As New Vector2(_Size.X / 2, -_Size.Y / 2)
        //    Dim P1 As New Vector2(-_Size.X / 2, -_Size.Y / 2)
        //    Dim P2 As New Vector2(-_Size.X / 2, _Size.Y / 2)
        //    Dim P3 As New Vector2(_Size.X / 2, _Size.Y / 2)

        //    'Length of the points from the origin
        //    Dim L0 As Single = Vector2.Length(P0)
        //    Dim L1 As Single = Vector2.Length(P1)
        //    Dim L2 As Single = Vector2.Length(P2)
        //    Dim L3 As Single = Vector2.Length(P3)

        //    'Angle of the points from the origin
        //    Dim A0 As Single = Vector2.Angle(P0)
        //    Dim A1 As Single = Vector2.Angle(P1)
        //    Dim A2 As Single = Vector2.Angle(P2)
        //    Dim A3 As Single = Vector2.Angle(P3)

        //    'Angle of points from origin + rotation
        //    Dim A2_0 As Single = A0 - _Rotation
        //    Dim A2_1 As Single = A1 - _Rotation
        //    Dim A2_2 As Single = A2 - _Rotation
        //    Dim A2_3 As Single = A3 - _Rotation

        //    'Points from origin that have been rotated
        //    Dim P2_0 As Vector2 = New Vector2(Math.Cos(A2_0), Math.Sin(A2_0)) * L0 + vRelativePosition
        //    Dim P2_1 As Vector2 = New Vector2(Math.Cos(A2_1), Math.Sin(A2_1)) * L1 + vRelativePosition
        //    Dim P2_2 As Vector2 = New Vector2(Math.Cos(A2_2), Math.Sin(A2_2)) * L2 + vRelativePosition
        //    Dim P2_3 As Vector2 = New Vector2(Math.Cos(A2_3), Math.Sin(A2_3)) * L3 + vRelativePosition


        //    BindTexture2D(_TextureID)
        //    GL.Begin(BeginMode.Quads)

        //    GL.TexCoord2(0.0F, 0.0F)
        //    GL.Vertex2(P2_0.X, P2_0.Y)

        //    GL.TexCoord2(1.0F, 0.0F)
        //    GL.Vertex2(P2_1.X, P2_1.Y)

        //    GL.TexCoord2(1.0F, 1.0F)
        //    GL.Vertex2(P2_2.X, P2_2.Y)

        //    GL.TexCoord2(0.0F, 1.0F)
        //    GL.Vertex2(P2_3.X, P2_3.Y)

        //    GL.End()

        //End Sub

        //Based from: http://jelle.druyts.net/2004/05/26/RotatingAnImageAroundItsCenterInNET.aspx
        public static void Draw2dRotated(Viewport _Viewport, int _TextureID, Vector2 _Position, Vector2 _Size, float _Rotation)
        {
            Vector2 vRelativePosition = _Position - _Viewport.Position;
            //Creates a temporary storate for a Vector2
            Vector2 vTemp = default(Vector2);

            //Assign's the current texture to the graphics device.
            BindTexture2D(_TextureID);

            //Begins drawing Quads
            GL.Begin(BeginMode.Quads);

            //Creates a quad with some properties and calculates the rotation
            GL.TexCoord2(0f, 0f);
            vTemp = RotatePoint(new Vector2(-_Size.X / 2, -_Size.Y / 2), _Rotation) + vRelativePosition;
            GL.Vertex2(vTemp.X, vTemp.Y);
            GL.TexCoord2(1f, 0f);
            vTemp = RotatePoint(new Vector2(_Size.X / 2, -_Size.Y / 2), _Rotation) + vRelativePosition;
            GL.Vertex2(vTemp.X, vTemp.Y);
            GL.TexCoord2(1f, 1f);
            vTemp = RotatePoint(new Vector2(_Size.X / 2, _Size.Y / 2), _Rotation) + vRelativePosition;
            GL.Vertex2(vTemp.X, vTemp.Y);
            GL.TexCoord2(0f, 1f);
            vTemp = RotatePoint(new Vector2(-_Size.X / 2, _Size.Y / 2), _Rotation) + vRelativePosition;
            GL.Vertex2(vTemp.X, vTemp.Y);

            //End drawing ready for another draw call.
            GL.End();

        }

        //Mathematic Information: http://www.puz.com/sw/amorphous/theory/
        //Rotates a Vector2 position as a point on an angle
        private static Vector2 RotatePoint(Vector2 p, double angle)
        {
            return new Vector2((float)(p.X * Math.Cos(angle) + p.Y * Math.Sin(angle)), (float)(-p.X * Math.Sin(angle) + p.Y * Math.Cos(angle)));
        }
    }
}
