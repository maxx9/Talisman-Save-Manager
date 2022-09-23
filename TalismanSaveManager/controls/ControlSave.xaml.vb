Imports System.IO

Public Class ControlSave

    Private _CurrentSteamId As String

    Public Sub New()
        InitializeComponent()
        combobox_databind()
    End Sub

#Region "ComboBox"
    Private Sub combobox_databind()
        _CurrentSteamId = KVP.db_SELECT("CurrentSteamId", "")(0).Value

        Dim _SteamList As New List(Of KeyValuePair(Of String, String))
        _SteamList.Add(New KeyValuePair(Of String, String)("", ""))
        For Each _Steam In Steam.db_SELECT("", "")
            _SteamList.Add(New KeyValuePair(Of String, String)(_Steam.SteamId, _Steam.Name & " (" & _Steam.SteamId & ")"))
        Next

        cbUser.ItemsSource = _SteamList
        cbUser.SelectedValue = _CurrentSteamId
        tblCurrent_databind()
    End Sub

    Private Sub cbUser_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim _cb As ComboBox = CType(sender, ComboBox)
        If _cb.SelectedValue IsNot Nothing Then
            KVP.db_UPDATE("CurrentSteamId", _cb.SelectedValue)
        End If
        _CurrentSteamId = KVP.db_SELECT("CurrentSteamId", "")(0).Value
        tblCurrent_databind()
    End Sub
#End Region

#Region "TextBlocks Current"
    Private Sub tblCurrent_databind()
        Dim _CurrentSaveStr As String = String.Format(Util.MainSaveFileTemplate, _CurrentSteamId)
        Dim _canSave As Boolean = False

        tblCurrentFileName.Text = ""
        tblCurrentFileDate.Text = ""
        tblCurrentFileSize.Text = ""
        tblCurrentFileNote.Text = ""
        tblCurrentFileNote.Foreground = Brushes.Black

        If _CurrentSteamId = "" Then
            tblCurrentFileNote.Text = "Please select a User to continue."
            tblCurrentFileNote.Foreground = Brushes.Red

        ElseIf File.Exists(_CurrentSaveStr) Then
            Dim _CurrentSave As New FileInfo(_CurrentSaveStr)
            tblCurrentFileName.Text = _CurrentSave.Name
            tblCurrentFileDate.Text = _CurrentSave.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss")
            tblCurrentFileSize.Text = _CurrentSave.Length & " Bytes"
            _canSave = True

            Dim matchedFile As String = Util.matchSaveFile(_CurrentSteamId, _CurrentSave.Length, _CurrentSave.LastWriteTime)
            If matchedFile = "" Then
                tblCurrentFileNote.Text = "No match found in existing save files. Save not saved."
            Else
                Dim _Players As String = Util.getPlayersFromFileName(matchedFile)
                tblCurrentFileNote.Text = String.Format("Matches existing saved save with players:{0}{1}", vbCrLf, _Players)
                tblCurrentFileNote.Foreground = Brushes.Green
            End If

        Else
            tblCurrentFileNote.Text = "No current save file exists in save folder for this user."
            tblCurrentFileNote.Foreground = Brushes.Red
        End If

        grid_databind(_canSave)
    End Sub
#End Region

#Region "Grid"
    Private Sub grid_databind(iCanSave As Boolean)
        grdPlayers.ItemsSource = Player.db_SELECT("", "")
        btnSaveSave.IsEnabled = iCanSave
    End Sub

    Private Sub DataGrid_GotFocus(sender As Object, e As RoutedEventArgs)
        Dim _DataGrid = CType(sender, DataGrid)
        _DataGrid.Focus()
    End Sub

    Private Sub SaveSave_Click(sender As Object, e As RoutedEventArgs)
        Dim oPlayers As String = ""

        For Each _Player As Player In grdPlayers.Items
            If _Player.Checked Then
                oPlayers &= If(oPlayers = "", "", "-") & _Player.Key.ToString
            End If
        Next

        Dim iError As String = Util.trySaveSave(_CurrentSteamId, oPlayers)
        If iError <> "" Then
            Util.displayMessage(iError, "Sorry")
        Else
            Util.displayMessage("Save Saved.", "Cool")
        End If

        tblCurrent_databind()
    End Sub
#End Region

End Class
