Public Class ControlSetup

    Private ObjectRename As Object

    Public Sub New()
        InitializeComponent()
        ObjectRename = Nothing
        grids_databind()
    End Sub

#Region "Buttons"
    Private Sub SteamAdd_Click(sender As Object, e As RoutedEventArgs)
        Dim oSteamId As String = Database.cleanInput(txtSteamId.Text)
        Dim oName As String = Database.cleanInput(txtSteamName.Text)

        If (oSteamId = "") OrElse (oName = "") Then
            Util.displayMessage("You forgot to write a name or steam id.", "Sorry")
        ElseIf Steam.db_SELECT(oSteamId, "").Count <> 0 Then
            Util.displayMessage("This ID already exists.", "Sorry")
        Else
            Steam.db_INSERT(oSteamId, oName)
            txtSteamId.Text = ""
            txtSteamName.Text = ""
        End If
        grids_databind()
    End Sub

    Private Sub PlayerAdd_Click(sender As Object, e As RoutedEventArgs)
        Dim oName As String = Database.cleanInput(txtPlayerName.Text)

        If oName = "" Then
            Util.displayMessage("You forgot to write a name.", "Sorry")
        ElseIf Player.db_SELECT("", oName).Count <> 0 Then
            Util.displayMessage("This player already exists.", "Sorry")
        Else
            Player.db_INSERT(oName)
            txtPlayerName.Text = ""
        End If

        grids_databind()
    End Sub
#End Region

#Region "Grids"
    Private Sub DataGrid_GotFocus(sender As Object, e As RoutedEventArgs)
        Dim _DataGrid = CType(sender, DataGrid)
        _DataGrid.Focus()
    End Sub

    Private Sub grids_databind()
        grdSteams.ItemsSource = Steam.db_SELECT("", "")
        grdPlayers.ItemsSource = Player.db_SELECT("", "")
    End Sub
#End Region

#Region "Grid Menus"
    Private Sub SteamRename_Click(sender As Object, e As RoutedEventArgs)
        Dim oSteamId As String = CType(sender, MenuItem).CommandParameter
        Dim _Steam As Steam = Steam.db_SELECT(oSteamId, "")(0)
        ObjectRename = _Steam
        tblRename.Text = "Rename Steam"
        txtRename.Text = _Steam.Name
        popRename.Visibility = System.Windows.Visibility.Visible
        grids_databind()
    End Sub

    Private Sub SteamDelete_Click(sender As Object, e As RoutedEventArgs)
        Dim oSteamId = CType(sender, MenuItem).CommandParameter
        If Util.allowDeleteSteam(oSteamId) Then
            Steam.db_DELETE(oSteamId)
            grids_databind()
        Else
            Util.displayMessage("Cannot delete this user because it has saved games.", "Sorry")
        End If
    End Sub

    Private Sub PlayerRename_Click(sender As Object, e As RoutedEventArgs)
        Dim oKey As Integer = CType(sender, MenuItem).CommandParameter
        Dim _Player As Player = Player.db_SELECT(oKey, "")(0)
        ObjectRename = _Player
        tblRename.Text = "Rename Player"
        txtRename.Text = _Player.Name
        popRename.Visibility = System.Windows.Visibility.Visible
        grids_databind()
    End Sub

    Private Sub PlayerDelete_Click(sender As Object, e As RoutedEventArgs)
        Dim oKey = CType(sender, MenuItem).CommandParameter
        If Util.allowDeletePlayer(oKey) Then
            Player.db_DELETE(oKey)
            grids_databind()
        Else
            Util.displayMessage("Cannot delete this player because it is in saved games.", "Sorry")
        End If
    End Sub
#End Region

#Region "Popups"

    Private Sub RenameOk_Click(sender As Object, e As RoutedEventArgs)
        Dim oName As String = Database.cleanInput(txtRename.Text)

        If oName = "" Then
            Util.displayMessage("You forgot to write a name.", "Sorry")
            Return
        End If

        If ObjectRename IsNot Nothing Then

            If ObjectRename.GetType() Is GetType(Steam) Then
                Steam.db_UPDATE(CType(ObjectRename, Steam).SteamId, oName)
            End If

            If ObjectRename.GetType() Is GetType(Player) Then
                Dim _Players As List(Of Player) = Player.db_SELECT("", oName)
                If (_Players.Count = 0) Then
                    Player.db_UPDATE(CType(ObjectRename, Player).Key, oName)
                Else
                    If (_Players(0).Key = CType(ObjectRename, Player).Key) AndAlso (_Players(0).Name.ToLower() = oName.ToLower()) Then
                        Player.db_UPDATE(CType(ObjectRename, Player).Key, oName)
                    Else
                        Util.displayMessage("This player already exists.", "Sorry")
                        Return
                    End If
                End If
            End If

            End If

        ObjectRename = Nothing
        tblRename.Text = ""
        txtRename.Text = ""
        popRename.Visibility = System.Windows.Visibility.Collapsed
        grids_databind()
    End Sub

    Private Sub RenameCancel_Click(sender As Object, e As RoutedEventArgs)
        ObjectRename = Nothing
        tblRename.Text = ""
        txtRename.Text = ""
        popRename.Visibility = System.Windows.Visibility.Collapsed
        grids_databind()
    End Sub
#End Region

End Class
