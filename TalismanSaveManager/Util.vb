Imports System.IO

Public Class Util

    Public Shared AppTitle As String = "Talisman Save Manager"

    Public Shared pathTalismanDisplay As String
    Public Shared pathTalisman As String

    Public Shared pathRoot As String
    Public Shared pathSaves As String
    Public Shared fileDb As String

    Public Shared MainSaveFileTemplate As String
    Public Shared OtherSaveFileTemplate As String

    Private Shared myWindow As Window

    Public Shared Function initInit() As Boolean
        Dim appData As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        pathTalismanDisplay = "%APPDATA%\Nomad Games\Talisman\saved_game\"
        pathTalisman = appData & "\Nomad Games\Talisman\saved_game\"

        pathRoot = pathTalisman & "TalSaveDir\"
        pathSaves = pathRoot & "saves\"
        fileDb = pathRoot & "config.db"

        MainSaveFileTemplate = pathTalisman & "{0}.ff1.cloud"
        OtherSaveFileTemplate = pathSaves & "{0}#{1}#{2}"

        If Not Directory.Exists(pathTalisman) Then
            Return False
        End If

        If Not Directory.Exists(pathRoot) Then
            Directory.CreateDirectory(pathRoot)
        End If

        If Not Directory.Exists(pathSaves) Then
            Directory.CreateDirectory(pathSaves)
        End If

        Return True
    End Function

    Public Shared Function init(iWindow As Window) As String
        myWindow = iWindow

        If Not Directory.Exists(pathTalisman) Then
            Return "Talisman Save Folder not found." & vbCrLf & "Launch the game at least once first."
        End If

        Database.initDB()

        Return ""
    End Function

    Public Shared Sub dispose()
        Database.disposeDB()
    End Sub

    Public Shared Function GetAssemblyRoot() As String
        If Directory.Exists(pathRoot) Then
            Return pathRoot
        End If
        Return Path.GetTempPath()
    End Function

    Public Shared Sub displayMessage(iMessage As String, iButton As String)
        'MessageBox.Show(myWindow, iMessage)
        CType(myWindow, MainWindow).showMessage(iMessage, iButton)
    End Sub

    'Public Shared Sub writeLog(iMsg As String, isNew As Boolean)
    '    Dim file As System.IO.StreamWriter
    '    file = My.Computer.FileSystem.OpenTextFileWriter("c:\temp\log.txt", Not isNew)
    '    file.WriteLine(iMsg)
    '    file.Close()
    'End Sub

#Region "File Checks"

    Public Shared Function allowDeleteSteam(iSteamId As String) As Boolean
        Dim dir_saves As New DirectoryInfo(pathSaves)
        For Each _file As FileInfo In dir_saves.GetFiles
            If iSteamId = getFileNamePart(_file.Name, "SteamId") Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Shared Function allowDeletePlayer(iKey As Integer) As Boolean
        Dim dir_saves As New DirectoryInfo(pathSaves)
        For Each _file As FileInfo In dir_saves.GetFiles
            Dim _split1 As String() = getFileNamePart(_file.Name, "Players").Split("-")
            For Each _U As String In _split1
                If _U = iKey Then
                    Return False
                End If
            Next
        Next
        Return True
    End Function

    Public Shared Function getPlayersFromFileName(iFileName As String) As String
        Dim returnVal As String = ""
        Dim _split1 As String() = getFileNamePart(iFileName, "Players").Split("-")
        For Each _U As String In _split1
            Dim _Ps As List(Of Player) = Player.db_SELECT(CInt(_U), "")
            If _Ps.Count > 0 Then
                returnVal &= If(returnVal = "", "", ", ") & _Ps(0).Name
            Else
                returnVal &= If(returnVal = "", "", ", ") & "???"
            End If
        Next
        Return returnVal
    End Function

    Public Shared Function getSavedSaveListForUser(iSteamId As String) As List(Of SavedSave)
        Dim returnVal As New List(Of SavedSave)
        Dim dir_saves As New DirectoryInfo(pathSaves)
        For Each _file As FileInfo In dir_saves.GetFiles
            If iSteamId = getFileNamePart(_file.Name, "SteamId") Then
                Dim _DateTime As String = formatDateTime(getFileNamePart(_file.Name, "DateTime"))
                Dim _Players As String = getPlayersFromFileName(_file.Name)
                returnVal.Add(New SavedSave(_file.Name, _DateTime, iSteamId, _Players))
            End If
        Next
        Return returnVal
    End Function

    Public Shared Function matchSaveFile(iSteamId As String, iSize As Long, iDate As Date) As String
        Dim dir_saves As New DirectoryInfo(pathSaves)
        For Each _file As FileInfo In dir_saves.GetFiles
            If (iSteamId = getFileNamePart(_file.Name, "SteamId")) AndAlso (_file.Length = iSize) AndAlso (_file.LastWriteTime = iDate) Then
                Return _file.Name
            End If
        Next
        Return ""
    End Function

    Public Shared Function getFileNamePart(iFileName As String, Optional iPart As String = "SteamId") As String
        Dim _split1 As String() = iFileName.Split("#")

        Select Case iPart
            Case "DateTime"
                If _split1.Count > 0 Then
                    Return _split1(0)
                End If
            Case "SteamId"
                If _split1.Count > 1 Then
                    Return _split1(1)
                End If
            Case "Players"
                If _split1.Count > 2 Then
                    Return _split1(2)
                End If
            Case Else
                Return ""
        End Select

        Return ""
    End Function

    Public Shared Function trySaveSave(iSteamId As String, iPlayers As String) As String
        Dim _CurrentSaveStr As String = String.Format(MainSaveFileTemplate, iSteamId)

        If File.Exists(_CurrentSaveStr) Then
            Dim _CurrentSave As New FileInfo(_CurrentSaveStr)
            Dim _Date As String = _CurrentSave.LastWriteTime.ToString("yyyy-MM-dd-HH-mm-ss")
            Dim _FinalSaveStr As String = String.Format(OtherSaveFileTemplate, _Date, iSteamId, iPlayers)

            If File.Exists(_FinalSaveStr) Then
                Return "This save is already saved because it already exists."
            Else
                File.Copy(_CurrentSaveStr, _FinalSaveStr)
            End If

        Else
            Return "No current save file exists in save folder for this user."
        End If


        Return ""
    End Function

    Public Shared Function tryLoadSave(iSteamId As String, iSave As SavedSave) As String
        Dim _SavedSaveStr As String = pathSaves & iSave.FileName
        Dim _CurrentSaveStr As String = String.Format(MainSaveFileTemplate, iSteamId)

        If Not File.Exists(_SavedSaveStr) Then
            Return "Saved save file missing."
        End If

        If File.Exists(_CurrentSaveStr) Then
            Try
                'File.Delete(_CurrentSaveStr)
                My.Computer.FileSystem.DeleteFile(_CurrentSaveStr, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
            Catch ex As Exception
                Return "Could not delete current active save file. Is it in use?"
            End Try
        End If

        File.Copy(_SavedSaveStr, _CurrentSaveStr)

        Return ""
    End Function

    Public Shared Function tryDeleteSave(iFileName As String) As String
        Dim _SavedSaveStr As String = pathSaves & iFileName

        If File.Exists(_SavedSaveStr) Then
            Try
                'File.Delete(_SavedSaveStr)
                My.Computer.FileSystem.DeleteFile(_SavedSaveStr, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
            Catch ex As Exception
                Return "Could not delete saved save file. Is it in use?"
            End Try
        End If

        Return ""
    End Function

    Public Shared Function formatDateTime(iDateTime As String) As String
        Dim _split As String() = iDateTime.Split("-")

        Dim oYear As String = 0
        Dim oMonth As String = 0
        Dim oDay As String = 0
        Dim oHour As String = 0
        Dim oMinute As String = 0
        Dim oSecond As String = 0

        If _split.Count > 0 Then oYear = CInt(_split(0))
        If _split.Count > 1 Then oMonth = CInt(_split(1))
        If _split.Count > 2 Then oDay = CInt(_split(2))
        If _split.Count > 3 Then oHour = CInt(_split(3))
        If _split.Count > 4 Then oMinute = CInt(_split(4))
        If _split.Count > 5 Then oSecond = CInt(_split(5))

        Return New Date(oYear, oMonth, oDay, oHour, oMinute, oSecond).ToString("yyyy/MM/dd HH:mm:ss")
    End Function

#End Region

End Class

Public Class SavedSave
    Public Property FileName As String
    Public Property DateTime As String
    Public Property Steam As String
    Public Property Players As String

    Public Sub New(iFileName As String, iDateTime As String, iSteam As String, iPlayers As String)
        FileName = iFileName
        DateTime = iDateTime
        Steam = iSteam
        Players = iPlayers
    End Sub
End Class