Imports System
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing

Public Class ViewportWindow
    Inherits System.Windows.Forms.Form
    'Creates the render form. This is the Window that you see on the screen
    Sub New()
        'Prevents the Update call having a blank image buffer "Artifact Resolver"
        Me.DoubleBuffered = True

        'Disable Crossthread checking
        'Thanks to Codeorder [2nd Post] @ http://www.daniweb.com/software-development/vbnet/threads/356904/cross-thread-operation-not-valid-control-datagridview1-accessed-from-a-thread
        Control.CheckForIllegalCrossThreadCalls = False
    End Sub


    Delegate Sub gDrawDelegate(ByVal drawobjects() As Panel)
    Public Sub gDraw(ByVal drawobjects() As Panel)
        If Me.InvokeRequired Then
            Dim d As New gDrawDelegate(AddressOf gDraw)
            Invoke(d, New Object() {drawobjects})
        Else
            Me.Controls.Clear()
            For Each p As Panel In drawobjects
                'Adds the object/panel to the forms drawing components
                Me.Controls.Add(p)
                'Places the panel on the top to be renderd last
                p.BringToFront()
            Next
            Me.Update()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'ViewportWindow
        '
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Name = "ViewportWindow"
        Me.ResumeLayout(False)

    End Sub
End Class
