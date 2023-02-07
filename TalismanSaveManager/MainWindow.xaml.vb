Class MainWindow

    Dim allowRun As Boolean

    Public Sub New()
        InitializeComponent()
        Title = String.Format("{0} v{1}", Util.AppTitle, "1.0.1")
        init()
    End Sub

    Private Sub init()
        allowRun = True
        Dim _error As String = Util.init(Me)
        If _error <> "" Then
            showMessage(_error, "Sorry")
            allowRun = False
        End If

        tabCtrl1.SelectedIndex = 0
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If allowRun Then
            Util.dispose()
        End If
    End Sub

    Private Sub tabCtrl1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim _Header As String = CType(CType(sender, TabControl).SelectedItem, TabItem).Header
        Dim frame1 As New Frame()

        If allowRun Then
            Select Case _Header
                Case "Setup"
                    frame1.Content = New ControlSetup
                Case "Save"
                    frame1.Content = New ControlSave
                Case "Load"
                    frame1.Content = New ControlLoad
                Case Else 'Info
                    frame1.Content = New ControlInfo
            End Select
        Else
            frame1.Content = New ControlInfo
        End If

        stk1.Children.Clear()
        stk1.Children.Add(frame1)
    End Sub

#Region "Message Pop"
    Public Sub showMessage(iMessage As String, iButton As String)
        tblMessage.Text = iMessage
        btnMessage.Content = iButton
        popMessage.Visibility = System.Windows.Visibility.Visible
    End Sub

    Private Sub MessageOk_Click(sender As Object, e As RoutedEventArgs)
        tblMessage.Text = ""
        btnMessage.Content = ""
        popMessage.Visibility = System.Windows.Visibility.Collapsed
    End Sub
#End Region

End Class
