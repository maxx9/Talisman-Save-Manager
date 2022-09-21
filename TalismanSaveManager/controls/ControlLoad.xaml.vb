Imports System.IO

Public Class ControlLoad

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

            Dim matchedFile As String = Util.matchSaveFile(_CurrentSteamId, _CurrentSave.Length, _CurrentSave.LastWriteTime)
            If matchedFile = "" Then
                tblCurrentFileNote.Text = "No match found in existing save files. Save not saved."
                tblCurrentFileNote.Foreground = Brushes.Red
            Else
                Dim _Players As String = Util.getPlayersFromFileName(matchedFile)
                tblCurrentFileNote.Text = String.Format("Matches existing saved save with players:{0}{1}", vbCrLf, _Players)
            End If

        Else
            tblCurrentFileNote.Text = "No current save file exists in save folder for this user."
        End If

        grid_databind()
    End Sub
#End Region

#Region "Grid"
    Private Sub grid_databind()
        btnLoadSave.IsEnabled = False
        grdSaves.ItemsSource = Util.getSavedSaveListForUser(_CurrentSteamId)
    End Sub

    Private Sub grdSaves_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim _grid As DataGrid = CType(sender, DataGrid)
        btnLoadSave.IsEnabled = (_grid.SelectedIndex >= 0)
    End Sub

    Private Sub DataGrid_GotFocus(sender As Object, e As RoutedEventArgs)
        Dim _DataGrid = CType(sender, DataGrid)
        _DataGrid.Focus()
    End Sub

    Private Sub SaveDelete_Click(sender As Object, e As RoutedEventArgs)
        Dim oFileName = CType(sender, MenuItem).CommandParameter

        Dim iError As String = Util.tryDeleteSave(oFileName)
        If iError <> "" Then
            Util.displayMessage(iError, "Sorry")
        End If

        tblCurrent_databind()
    End Sub

    Private Sub LoadSave_Click(sender As Object, e As RoutedEventArgs)
        Dim _Save As SavedSave = CType(grdSaves.SelectedItem, SavedSave)

        Dim iError As String = Util.tryLoadSave(_CurrentSteamId, _Save)
        If iError <> "" Then
            Util.displayMessage(iError, "Sorry")
        Else
            Util.displayMessage("Save Loaded.", "Cool")
        End If

        tblCurrent_databind()
    End Sub
#End Region


End Class
