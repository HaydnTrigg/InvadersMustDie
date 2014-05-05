'Import the OpenTK Library's
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Namespace Isotope
    'Create the class Viewport.
    Public Class Viewport
        'This class will Inherit the GameWindow class "OpenTK.GameWindow"
        Inherits GameWindow

#Region "PublicField"
        Public Position As New Vector2(0, 0)
        Public ViewportRealSize As New Vector2(1600, 1600)
#End Region

#Region "PrivateField"


#End Region

        'Returns the current scale of the Viewport
        ReadOnly Property ViewportScale As Single
            Get
                If Width >= Height Then
                    Return Width / ViewportRealSize.X
                Else
                    Return Height / ViewportRealSize.Y
                End If

            End Get
        End Property

        'Returns the current mouse position.
        ReadOnly Property MousePosition As Vector2
            Get
                'Returns the correct mouse position based on Scale and the Position of the Viewport.
                Return New Vector2(Mouse.X, Mouse.Y) / ViewportScale + Position
            End Get
        End Property


        'Creates a New Viewport with some specified properties.
        Public Sub New(ByVal _Width As Integer, ByVal _Height As Integer, ByVal _Title As String)
            MyBase.New(_Width, _Height, New GraphicsMode(32, 24, 8, 2), _Title)
        End Sub

        'Updates the Viewport
        Public Sub Update(ByVal delta As Single)
        End Sub
        Public Function ModS(ByVal _Single As Single) As Single
            If _Single < 0 Then
                Return _Single * -1
            End If
            Return _Single
        End Function

    End Class
End Namespace