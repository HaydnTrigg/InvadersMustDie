Imports System
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing

Public Class ViewportWindow
    Inherits System.Windows.Forms.Form

    Dim Viewable As Graphics

    'Creates the render form. This is the Window that you see on the screen
    Sub New()

        'Prevents the Update call having a blank image buffer "Artifact Resolver"
        Me.DoubleBuffered = True
        Me.ClientSize = New Size(400, 400)
        'Disable Crossthread checking
        'Thanks to Codeorder [2nd Post] @ http://www.daniweb.com/software-development/vbnet/threads/356904/cross-thread-operation-not-valid-control-datagridview1-accessed-from-a-thread
        Control.CheckForIllegalCrossThreadCalls = False

        CreateNewGraphics()
    End Sub

    Private Sub CreateNewGraphics() Handles Me.ResizeEnd, Me.SizeChanged
        'Double Buffering for Backbuffer drawing @ http://www.developerfusion.com/code/4668/double-buffering-in-net/
        'This code has now been replaced by the GraphiceDevice.SpriteBatch.
        'Create the viewable graphics device to render the spritebatch onto the form
        Viewable = Me.CreateGraphics()
        Viewable.SmoothingMode = Drawing2D.SmoothingMode.None
        Viewable.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        Viewable.CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
        Viewable.CompositingMode = Drawing2D.CompositingMode.SourceCopy
    End Sub

    Delegate Sub gDrawDelegate(ByVal BackBuffer As Bitmap)
    Public Sub gDraw(ByVal BackBuffer As Bitmap)
        If Me.InvokeRequired Then
            'Send the backbuffer to the Viewport thread which ownes this instance of the class
            Dim d As New gDrawDelegate(AddressOf gDraw)
            Invoke(d, New Object() {BackBuffer})
        Else
            'Draw the backbuffer to the screen.
            Viewable.DrawImageUnscaled(BackBuffer, 0, 0)

            'Force the screen to update and un-updated regions that re invalidated.
            'Me.Update()

            'STAThread drawing code end. The graphics card will now wait for the next frame or the default update operation. DoubleBuffering will prevent
            'any arfifacts.


        End If
    End Sub


End Class
