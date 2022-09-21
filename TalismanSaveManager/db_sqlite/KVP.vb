Imports System.Data.SQLite

Public Class KVP

    Private Sub New(iKey As String, iValue As String)
        _Key = iKey
        _Value = iValue
    End Sub

#Region "Properties"
    Private _Key As String
    Public ReadOnly Property Key As String
        Get
            Return _Key
        End Get
    End Property

    Private _Value As String
    Public ReadOnly Property Value As String
        Get
            Return _Value
        End Get
    End Property
#End Region

#Region "DB Interaction"
    Public Shared Sub db_CREATE()
        Dim oCmd As String = "CREATE TABLE KVP (Key TEXT PRIMARY KEY, Value TEXT)"
        Database.ExecuteScalar(oCmd)
        db_INSERT("CurrentSteamId", "")
    End Sub

    Public Shared Sub db_DROP()
        Dim oCmd As String = "DROP TABLE KVP"
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Function db_SELECT(Optional iKey As String = "", Optional iValue As String = "") As List(Of KVP)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oValue As String = Database.cleanInput(iValue)

        Dim oCmdWhere As String = ""
        If (oKey <> "") OrElse (oValue <> "") Then
            Dim oCmd1 As String = If(oKey = "", "", String.Format("Key = '{0}'", oKey))
            Dim oCmd2 As String = If(oValue = "", "", String.Format("Value = '{0}'", oValue))
            oCmdWhere = String.Format("WHERE {0} {1} {2}", oCmd1, If((oCmd1 = "") Or (oCmd2 = ""), "", "AND"), oCmd2)
        End If
        Dim oCmd As String = String.Format("SELECT Key, Value FROM KVP {0} ORDER BY Key", oCmdWhere)

        Dim returnVal As New List(Of KVP)
        Dim reader As SQLiteDataReader = Database.ExecuteReader(oCmd)
        While reader.Read()
            Dim _KVP As New KVP(
                reader.GetString(0),
                reader.GetString(1)
            )
            returnVal.Add(_KVP)
        End While
        Return returnVal
    End Function

    Public Shared Sub db_INSERT(iKey As String, iValue As String)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oValue As String = Database.cleanInput(iValue)
        Dim oCmd As String = String.Format("INSERT INTO KVP ('Key', 'Value') values ('{0}', '{1}')", oKey, oValue)
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_UPDATE(iKey As String, iValue As String)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oValue As String = Database.cleanInput(iValue)
        Dim oCmd As String = String.Format("UPDATE KVP SET Value = '{1}' WHERE Key = '{0}'", oKey, oValue)
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_DELETE(iKey As String)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oCmd As String = String.Format("DELETE FROM KVP WHERE Key = '{0}'", oKey)
        Database.ExecuteScalar(oCmd)
    End Sub
#End Region

End Class
