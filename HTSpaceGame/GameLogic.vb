'Visual Basic Isotope Framework port by Haydn Trigg
'Written by: Haydn Trigg
'Created : 5/8/2013
'Modified: 5/10/2013
'Version : 0.3.0
'This is the logic part of the game. This will include the load content, update and draw functions as individual parts of threads.


Imports System.Windows.Forms
Imports System.Drawing

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq

Namespace HTSpaceGame
    Partial Public Class Game
        Dim GameObjects As New List(Of GameObject)
        Dim gTextures As New List(Of Bitmap)

        Private Sub gLoadContent()
            GameObjects.Add(New PlayerShip(New Vector2(0, 0), New Vector2(24, 24)))

            gTextures.Add(Bitmap.FromFile("Resources/Avatar.png", Imaging.PixelFormat.Format16bppArgb1555))
            gTextures.Add(Bitmap.FromFile("Resources/ship.png", Imaging.PixelFormat.Format16bppArgb1555))
            gTextures.Add(Bitmap.FromFile("Resources/BackgroundOverlay.png", Imaging.PixelFormat.Format16bppArgb1555))

        End Sub

        Private Sub gUpdate(ByVal gGameTime As GameTime)
            Dim vPlayerControl As New Vector2(0, 0)
            If GetAsyncKeyState(Keys.W) Then
                vPlayerControl.Y -= 1
            End If
            If GetAsyncKeyState(Keys.S) Then
                vPlayerControl.Y += 1
            End If
            If GetAsyncKeyState(Keys.A) Then
                vPlayerControl.X -= 1
            End If
            If GetAsyncKeyState(Keys.D) Then
                vPlayerControl.X += 1
            End If

            'Iterate through all the games objects
            For Each g As GameObject In GameObjects
                'Select the type of object and do the appropriate update for it
                Select Case g.GetType()
                    Case GetType(Ship)
                        'Assign the ship its own independant object as its type using a cast
                        Dim gObject As Ship = CType(g, Ship)

                        gObject.Update(gGameTime)
                        g = gObject
                    Case GetType(PlayerShip)
                        'Assign the ship its own independant object as its type using a cast
                        Dim gObject As PlayerShip = CType(g, PlayerShip)
                        gObject.vMovingVector = vPlayerControl
                        gObject.Update(gGameTime)
                        g = gObject
                End Select
                g.vPosition.X = GameMath.Clamp(g.vPosition.X, 0, 1000)
                g.vPosition.Y = GameMath.Clamp(g.vPosition.Y, 0, 1000)
            Next

            If GameMath.Vector2Distance(GameObjects(0).vPosition - New Vector2(gGraphicsDevice.Width / 2, gGraphicsDevice.Height / 2), gGraphicsDevice.ViewPosition) < 1.1F Then
                gGraphicsDevice.ViewPosition = GameObjects(0).vPosition - New Vector2(gGraphicsDevice.Width / 2, gGraphicsDevice.Height / 2)

            End If
            gGraphicsDevice.ViewPosition = GameMath.Lerp(gGraphicsDevice.ViewPosition, GameObjects(0).vPosition - New Vector2(gGraphicsDevice.Width / 2, gGraphicsDevice.Height / 2), gGameTime.ElapsedGameTime * 2)
            gGraphicsDevice.ViewPosition.X = GameMath.Clamp(gGraphicsDevice.ViewPosition.X, 0, 1000 - gGraphicsDevice.Width)
            gGraphicsDevice.ViewPosition.Y = GameMath.Clamp(gGraphicsDevice.ViewPosition.Y, 0, 1000 - gGraphicsDevice.Height)

        End Sub

        Private Sub gDraw()
            If Not gGraphicsDevice.Equals(vbNull) Then

                'Clear the backbuffer so a new round of images can be drawn without intefering with the originals.
                gGraphicsDevice.SpriteBatch.Clear(Color.Black)


                'GraphicsMath.DrawImageRotatedAroundCenter(gGraphicsDevice, New RectangleF(New Vector2(500, 500).createPointF, New SizeF(1000, 1000)), gTextures(2), 0)
                gGraphicsDevice.SpriteBatch.DrawImage(gTextures(2), (gGraphicsDevice.ViewPosition * -1).createPointF)

                'Iterate through all the games objects
                For Each g As GameObject In GameObjects
                    'Select the type of object and do the appropriate update for it
                    Select Case g.GetType()
                        Case GetType(Ship)
                            'Assign the ship its own independant object as its type using a cast
                            Dim gObject As Ship = CType(g, Ship)

                            GraphicsMath.DrawImageRotatedAroundCenter(gGraphicsDevice, New RectangleF(gObject.vPosition.createPointF, gObject.vSize.createSizeF), gTextures(gObject.iTextureID), gObject.fRotation)
                        Case GetType(ShipGun)
                            'Assign the ship its own independant object as its type using a cast
                            Dim gObject As Ship = CType(g, Ship)

                            GraphicsMath.DrawImageRotatedAroundCenter(gGraphicsDevice, New RectangleF(gObject.vPosition.createPointF, gObject.vSize.createSizeF), gTextures(gObject.iTextureID), gObject.fRotation)
                        Case GetType(PlayerShip)
                            'Assign the ship its own independant object as its type using a cast
                            Dim gObject As Ship = CType(g, Ship)

                            GraphicsMath.DrawImageRotatedAroundCenter(gGraphicsDevice, New RectangleF(gObject.vPosition.createPointF, gObject.vSize.createSizeF), gTextures(gObject.iTextureID), gObject.fRotation)

                    End Select
                Next
                gSpriteBatch.DrawString("FPS: " + fps.iFrameRate.ToString(), New Font("Arial", 12), New SolidBrush(Color.FromArgb(255, 0, 0)), New PointF(10, 10))

            End If
        End Sub

    End Class
End Namespace

