Imports System.Data.SQLite

Public Class Steam

    Private Sub New(iSteamId As String, iName As String)
        _SteamId = iSteamId
        _Name = iName
    End Sub

#Region "Properties"
    Private _SteamId As String
    Public ReadOnly Property SteamId As String
        Get
            Return _SteamId
        End Get
    End Property

    Private _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property
#End Region

#Region "DB Interaction"
    Public Shared Sub db_CREATE()
        Dim oCmd As String = "CREATE TABLE Steam (SteamId TEXT PRIMARY KEY, Name TEXT)"
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_DROP()
        Dim oCmd As String = "DROP TABLE Steam"
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Function db_SELECT(Optional iSteamId As String = "", Optional iName As String = "") As List(Of Steam)
        Dim oSteamId As String = Database.cleanInput(iSteamId)
        Dim oName As String = Database.cleanInput(iName)

        Dim oCmdWhere As String = ""
        If (oSteamId <> "") OrElse (oName <> "") Then
            Dim oCmd1 As String = If(oSteamId = "", "", String.Format("SteamId = '{0}'", oSteamId))
            Dim oCmd2 As String = If(oName = "", "", String.Format("Name = '{0}'", oName))
            oCmdWhere = String.Format("WHERE {0} {1} {2}", oCmd1, If((oCmd1 = "") Or (oCmd2 = ""), "", "AND"), oCmd2)
        End If
        Dim oCmd As String = String.Format("SELECT SteamId, Name FROM Steam {0} ORDER BY Name", oCmdWhere)

        Dim returnVal As New List(Of Steam)
        Dim reader As SQLiteDataReader = Database.ExecuteReader(oCmd)
        While reader.Read()
            Dim _Steam As New Steam(
                reader.GetString(0),
                reader.GetString(1)
            )
            returnVal.Add(_Steam)
        End While
        Return returnVal
    End Function

    Public Shared Sub db_INSERT(iSteamId As String, iName As String)
        Dim oSteamId As String = Database.cleanInput(iSteamId)
        Dim oName As String = Database.cleanInput(iName)
        Dim oCmd As String = String.Format("INSERT INTO Steam ('SteamId', 'Name') values ('{0}', '{1}')", oSteamId, oName)
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_UPDATE(iSteamId As String, iName As String)
        Dim oSteamId As String = Database.cleanInput(iSteamId)
        Dim oName As String = Database.cleanInput(iName)
        Dim oCmd As String = String.Format("UPDATE Steam SET Name = '{1}' WHERE SteamId = '{0}'", oSteamId, oName)
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_DELETE(iSteamId As String)
        Dim oSteamId As String = Database.cleanInput(iSteamId)
        Dim oCmd As String = String.Format("DELETE FROM Steam WHERE SteamId = '{0}'", oSteamId)
        Database.ExecuteScalar(oCmd)
    End Sub
#End Region

End Class
