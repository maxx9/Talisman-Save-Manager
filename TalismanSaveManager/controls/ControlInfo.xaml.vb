Public Class ControlInfo

    Public Sub New()
        InitializeComponent()
        tblPath.Text = "Default Talisman Save Location: " & vbCrLf & Util.pathTalismanDisplay
    End Sub

End Class
