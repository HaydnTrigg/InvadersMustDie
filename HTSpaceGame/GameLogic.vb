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
            GameObjects.Add(New Ship(New Vector2(0, 0), New Vector2(24, 24)))

            gTextures.Add(Bitmap.FromFile("Resources/Avatar.png"))
            gTextures.Add(Bitmap.FromFile("Resources/ship.png"))


        End Sub

        Private Sub gUpdate(ByVal gGameTime As GameTime)
                If GetAsyncKeyState(Keys.W) Then
                'GameObjects(0).vPosition.Y -= 130 * gGameTime.ElapsedGameTime
                gGraphicsDevice.ViewPosition.Y -= 130 * gGameTime.ElapsedGameTime
                End If
                If GetAsyncKeyState(Keys.S) Then
                'GameObjects(0).vPosition.Y += 130 * gGameTime.ElapsedGameTime
                gGraphicsDevice.ViewPosition.Y += 130 * gGameTime.ElapsedGameTime
                End If
                If GetAsyncKeyState(Keys.A) Then
                'GameObjects(0).vPosition.X -= 130 * gGameTime.ElapsedGameTime
                gGraphicsDevice.ViewPosition.X -= 130 * gGameTime.ElapsedGameTime
                End If
                If GetAsyncKeyState(Keys.D) Then
                'GameObjects(0).vPosition.X += 130 * gGameTime.ElapsedGameTime
                gGraphicsDevice.ViewPosition.X += 130 * gGameTime.ElapsedGameTime
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
                End Select
            Next
        End Sub

        Private Sub gDraw()
            GraphicsMath.DrawImageRotatedAroundCenter(gGraphicsDevice, New RectangleF(New Vector2(50, 50).createPointF, New SizeF(100, 100)), gTextures(0), Math.PI + gGameTime.TotalGameTime)

            'Iterate through all the games objects
            For Each g As GameObject In GameObjects
                'Select the type of object and do the appropriate update for it
                Select Case g.GetType()
                    Case GetType(Ship)
                        'Assign the ship its own independant object as its type using a cast
                        Dim gObject As Ship = CType(g, Ship)

                        GraphicsMath.DrawImageRotatedAroundCenter(gGraphicsDevice, New RectangleF(gObject.vPosition.createPointF, gObject.vSize.createSizeF), gTextures(0), Math.PI - gGameTime.TotalGameTime)

                End Select
            Next
            gSpriteBatch.DrawString("FPS: " + fps.iFrameRate.ToString(), New Font("Arial", 12), New SolidBrush(Color.FromArgb(255, 0, 0)), New PointF(10, 10))

        End Sub

    End Class
End Namespace

