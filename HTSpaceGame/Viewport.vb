﻿'Import the System and OpenTK Library's
Imports System.Drawing
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

'Usng the Isitope Visual Basic Namespace
Namespace IsotopeVB

    Public Class Viewport
        'This class will Inherit the GameWindow class "OpenTK.GameWindow"
        Inherits GameWindow

        'Stores the viewport's 2d relative position
        Public ViewportPosition As New Vector2(0, 0)
        Public ViewportBoundary As Single = 30.0F
        Public ViewportRealSize As New Vector2(400, 400)

        ReadOnly Property ViewportScale As Vector2
            Get
                Return ViewportSize / ViewportRealSize
            End Get
        End Property

        ReadOnly Property ViewportSize As Vector2
            Get
                Return New Vector2(Size.Width, Size.Height)
            End Get
        End Property

        ReadOnly Property MousePosition As Vector2
            Get
                Return New Vector2(Mouse.X, Mouse.Y) + ViewportPosition
            End Get
        End Property

        'The new function which will take in a title, width and height aswell
        Public Sub New(ByVal _Width As Integer, ByVal _Height As Integer, ByVal _Title As String)
            Me.Title = _Title
            Me.ClientSize = New Size(_Width, _Height)
        End Sub
    End Class
End Namespace