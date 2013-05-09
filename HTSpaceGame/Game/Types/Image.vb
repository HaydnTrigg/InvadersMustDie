Imports System.Drawing
Imports System.Drawing.Imaging
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL


Namespace IsotopeVB
    Class GLTexture
        Inherits IProperties
        Public bitmap As Bitmap
        ' Used to load image
        Public texture As Integer

        Public rebuild As Boolean = True

        Public Size As Vector2

        ''' <summary>
        ''' Creates 4 vertices and texcoords for quad. Also loads the texture aswell
        ''' </summary>
        Public Sub New(ByVal path As String)
            Load(path)
        End Sub


        ''' <summary>
        ''' Loads image from harddisk into memory.
        ''' </summary>
        ''' <param name="path">Image path.</param>
        Public Sub Load(ByVal path As String)
            ' Load image
            bitmap = New Bitmap(path)

            ' Generate texture
            GL.GenTextures(1, texture)
            GL.BindTexture(TextureTarget.Texture2D, texture)

            ' Store texture size
            w = bitmap.Width
            h = bitmap.Height

            Dim data As BitmapData = bitmap.LockBits(New Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.[ReadOnly], System.Drawing.Imaging.PixelFormat.Format32bppPArgb)

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)

            bitmap.UnlockBits(data)

            ' Setup filtering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))

            Size = New Vector2(bitmap.Width, bitmap.Height)
            Console.WriteLine("SIZEX " + Size.X.ToString())
        End Sub


        ''' <summary>
        ''' Deletes texture from memory.
        ''' </summary>
        Public Sub Free()
            GL.DeleteTextures(1, texture)
        End Sub
    End Class




End Namespace