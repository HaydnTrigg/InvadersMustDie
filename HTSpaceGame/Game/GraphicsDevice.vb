'Visual Basic Isotope Framework port by Haydn Trigg
'Written by: Haydn Trigg
'Created : 5/8/2013
'Modified: 5/10/2013
'Version : 0.3.0
'This is a graphics device class. This stores the spritebatch for drawing with and is sent to the form for drawing every frame.
'This class also serves the purpose of storing resolution and other key propertys that are required to draw every frame.

Imports System
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing
Public Class GraphicsDevice

    Public Width As Integer
    Public Height As Integer
    Public ViewPosition As New Vector2(-10, -10)

    Sub New(ByVal _Width As Integer, ByVal _Height As Integer)
        Width = _Width
        Height = _Height

        'Create a backbuffer bitmap
        BackBuffer = New Bitmap(Width, Height)
        'Create a backbuffer graphics interface
        SpriteBatch = Graphics.FromImage(BackBuffer)

    End Sub

    Public Sub Initialize(ByVal _InterpolationMode As Drawing2D.InterpolationMode, ByVal _SmoothingMode As Drawing2D.SmoothingMode)
        'Assign some quality settings to the SpriteBatch
        SpriteBatch.InterpolationMode = _InterpolationMode
        SpriteBatch.SmoothingMode = Drawing2D.SmoothingMode.Default
        'Default 2D Rendering techiniques forces to the SpriteBatch
        SpriteBatch.CompositingMode = Drawing2D.CompositingMode.SourceOver
        SpriteBatch.CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
    End Sub

    Sub ChangeResulution(ByVal _Width As Integer, ByVal _Height As Integer)
        Width = _Width
        Height = _Height

        'Create a backbuffer bitmap

        BackBuffer = New Bitmap(GameMath.ClampI(Width, 1, Integer.MaxValue), GameMath.ClampI(Height, 1, Integer.MaxValue))

        'Store the old spritebatch
        Dim oldSpriteBatch As Graphics = SpriteBatch

        'Create a backbuffer graphics interface
        SpriteBatch = Graphics.FromImage(BackBuffer)

        'Assign some quality settings to the SpriteBatch
        SpriteBatch.InterpolationMode = oldSpriteBatch.InterpolationMode
        SpriteBatch.SmoothingMode = oldSpriteBatch.SmoothingMode
        'Default 2D Rendering techiniques forces to the SpriteBatch
        SpriteBatch.CompositingMode = Drawing2D.CompositingMode.SourceOver
        SpriteBatch.CompositingQuality = Drawing2D.CompositingQuality.AssumeLinear
    End Sub

    Public BackBuffer As Bitmap
    Public SpriteBatch As Graphics

End Class
