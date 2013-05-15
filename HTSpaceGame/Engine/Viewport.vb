'Import the System and OpenTK Library's
Imports System.Drawing
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

'Usng the Isitope Visual Basic Namespace
Namespace Isotope

    Public Class Viewport
        'This class will Inherit the GameWindow class "OpenTK.GameWindow"
        Inherits GameWindow

        'Stores the viewport's 2d relative position
        Public ViewportPosition As New Vector2(0, 0)
        Public ViewportBoundary As Single = 25.0F
        Public ViewportRealSize As New Vector2(1600, 1600)
        Public ViewportTargetSize As New Vector2(800, 800)

        ReadOnly Property ViewportScale As Single
            Get
                Return ViewportSize.X / ViewportRealSize.X
            End Get
        End Property

        ReadOnly Property ViewportSize As Vector2
            Get
                Return New Vector2(Size.Width, Size.Height)
            End Get
        End Property

        ReadOnly Property MousePosition As Vector2
            Get
                Return New Vector2(Mouse.X, Mouse.Y) / ViewportScale + ViewportPosition
            End Get
        End Property

        'The new function which will take in a title, width and height aswell
        Public Sub New(ByVal _Width As Integer, ByVal _Height As Integer, ByVal _Title As String)
            MyBase.New(_Width, _Height, New GraphicsMode(32, 24, 8, 0), _Title)


        End Sub

        Public Sub Update(ByVal gGameTime As GameTime)
            ViewportRealSize = GameMath.Lerp(ViewportRealSize, ViewportTargetSize, gGameTime.ElapsedGameTime * 1.75F)
            Dim v As Vector2 = ViewportRealSize - ViewportTargetSize
            If ModS(v.X) < 3.5F Or ModS(v.Y) < 3.5F Then
                ViewportRealSize = ViewportTargetSize
            End If
        End Sub
        Public Function ModS(ByVal _Single As Single) As Single
            If _Single < 0 Then
                Return _Single * -1
            End If
            Return _Single
        End Function

    End Class
End Namespace