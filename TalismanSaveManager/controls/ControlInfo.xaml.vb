Public Class ControlInfo

    Public Sub New()
        InitializeComponent()
        tbPath.Text = Util.pathTalismanDisplay
    End Sub

    Private Sub hpPath_Click(sender As Object, e As RoutedEventArgs)
        Process.Start(Util.pathTalisman)
    End Sub
End Class
