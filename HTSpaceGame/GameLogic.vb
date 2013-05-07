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

        Private Sub gLoadContent()
            GameObjects.Add(New Ship(New Vector2(20, 20), New Vector2(30, 60)))
        End Sub

        Private Sub gUpdate(ByVal gGameTime As GameTime)
                If GetAsyncKeyState(Keys.W) Then
                    GameObjects(0).vPosition.Y -= 130 * gGameTime.ElapsedGameTime
                End If
                If GetAsyncKeyState(Keys.S) Then
                    GameObjects(0).vPosition.Y += 130 * gGameTime.ElapsedGameTime
                End If
                If GetAsyncKeyState(Keys.A) Then
                    GameObjects(0).vPosition.X -= 130 * gGameTime.ElapsedGameTime
                End If
                If GetAsyncKeyState(Keys.D) Then
                    GameObjects(0).vPosition.X += 130 * gGameTime.ElapsedGameTime
                End If



            'Iterate through all the games objects
            For Each g As GameObject In GameObjects
                g.Update(gGameTime)
            Next
        End Sub

        Private Function gDraw() As RenderBox()
            'Creates a list array of panels for the frame
            Dim spriteBatch As New List(Of RenderBox)

            'Iterate through all the games objects [drawable components]
            For Each g As GameObject In GameObjects
                spriteBatch.Add(g.createRenderBox())
            Next

            'Linear Interpoleate between the previous frame and the next frame using the difference of time between the last update
            'This will cause the game to appear more smooth to the end user

            'Set the current component to the top

            Return spriteBatch.ToArray()
        End Function

    End Class
End Namespace

