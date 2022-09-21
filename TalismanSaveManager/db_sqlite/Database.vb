Imports System.IO
Imports System.Data.SQLite
'https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki

Public Class Database

    Private Shared connection As SQLiteConnection

#Region "Main"
    Public Shared Sub initDB()
        Dim doSetup As Boolean = False
        If Not File.Exists(Util.fileDb) Then
            doSetup = True
        End If

        Dim conStr As String = "Data Source=" & Util.fileDb
        connection = New SQLiteConnection(conStr)
        connection.Open()

        If doSetup Then
            KVP.db_CREATE()
            Steam.db_CREATE()
            Player.db_CREATE()
        End If
    End Sub

    Public Shared Sub disposeDB()
        connection.Close()
    End Sub
#End Region

#Region "Execute"
    Public Shared Sub ExecuteScalar(iCommand As String)
        Using command As New SQLiteCommand(connection)
            command.CommandText = iCommand
            command.ExecuteScalar()
        End Using
    End Sub

    Public Shared Function ExecuteReader(iCommand As String)
        Dim reader As SQLiteDataReader = Nothing
        Using command As New SQLiteCommand(connection)
            command.CommandText = iCommand
            reader = command.ExecuteReader()
        End Using
        Return reader
    End Function
#End Region

#Region "Extras"
    Public Shared Function cleanInput(iStr As String) As String
        Return Text.RegularExpressions.Regex.Replace(iStr, "[^A-Za-z0-9]", "")
    End Function
#End Region

End Class
