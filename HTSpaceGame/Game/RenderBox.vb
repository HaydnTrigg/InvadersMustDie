Imports System.Windows.Forms
Imports System.Drawing

Public Class RenderBox
    Inherits Panel
    Public TextureID As Integer = 0
    Sub New()
    End Sub
    Sub New(ByVal i As Integer)
        TextureID = i
    End Sub
End Class
