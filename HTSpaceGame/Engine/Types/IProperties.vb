Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL


Namespace IsotopeVB
    Class IProperties
        Public w As Integer, h As Integer, originX As Integer, originY As Integer
        Public rotation As Single, scaleX As Single = 1.0F, scaleY As Single = 1.0F
        Public origin As Boolean, blending As Boolean
        Public r As Byte = 255, g As Byte = 255, b As Byte = 255, a As Byte = 255
        ' Blending colors

        ''' <summary>
        ''' Sets new size.
        ''' </summary>
        ''' <param name="w">New width.</param>
        ''' <param name="h">New height.</param>
        Public Sub SetSize(ByVal w As Integer, ByVal h As Integer)
            Me.w = w
            Me.h = h
        End Sub


        ''' <summary>
        ''' Sets blending.
        ''' </summary>
        Public Sub SetBlending()
            blending = True
        End Sub



        ''' <summary>
        ''' Sets blending.
        ''' </summary>
        ''' <param name="a">Alpha intensity.</param>
        Public Sub SetBlending(ByVal a As Byte)
            blending = True

            Me.a = a
        End Sub


        ''' <summary>
        ''' Sets blending.
        ''' </summary>
        ''' <param name="r">Red intensity.</param>
        ''' <param name="g">Green intensity.</param>
        ''' <param name="b">Blue intensity.</param>
        ''' <param name="a">Alpha intensity.</param>
        Public Sub SetBlending(ByVal r As Byte, ByVal g As Byte, ByVal b As Byte, ByVal a As Byte)
            blending = True

            Me.r = r
            Me.g = g
            Me.b = b
            Me.a = a
        End Sub


        ''' <summary>
        ''' Disables blending.
        ''' </summary>
        Public Sub DisableBlending()
            blending = False
        End Sub


        ''' <summary>
        ''' Prepares drawing.
        ''' </summary>
        ''' <param name="x">X position.</param>
        ''' <param name="y">Y position.</param>
        ''' <param name="w">Width of frame.</param>
        ''' <param name="h">Height of frame.</param>
        Protected Sub Begin(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
            ' Enable blending if allowed
            If blending Then
                GL.Enable(EnableCap.Blend)
                GL.Color4(r, g, b, a)
            End If

            ' Rotate around user specified origin
            If origin Then
                GL.Translate(originX, originY, 0.0)
                GL.Rotate(rotation, 0.0F, 0.0F, 1.0F)
                GL.Translate(-originX, -originY, 0.0)
            Else
                ' Else use frame center as origin
                GL.Translate(x + w \ 2, y + h \ 2, 0.0)
                GL.Rotate(rotation, 0.0F, 0.0F, 1.0F)
                GL.Translate(-(x + w \ 2), -(y + h \ 2), 0.0)
            End If

            ' Scale
            GL.Scale(scaleX, scaleY, 0.0F)
        End Sub


        ''' <summary>
        ''' Ends drawing.
        ''' </summary>
        Protected Sub [End]()
            ' Translate
            GL.LoadIdentity()
            GL.Translate(0.375, 0.375, 0.0)

            ' Disable blending
            If blending Then
                GL.Disable(EnableCap.Blend)

                ' Set white color
                GL.Color4(CByte(255), CByte(255), CByte(255), CByte(255))
            End If
        End Sub
    End Class
End Namespace