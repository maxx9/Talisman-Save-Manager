Imports System.Data.SQLite

Public Class Player

    Private Sub New(iKey As Integer, iName As String)
        _Key = iKey
        _Name = iName
        _Checked = False
    End Sub

#Region "Properties"
    Private _Key As Integer
    Public ReadOnly Property Key As Integer
        Get
            Return _Key
        End Get
    End Property

    Private _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property

    Private _Checked As String
    Public Property Checked As String
        Get
            Return _Checked
        End Get
        Set(value As String)
            _Checked = value
        End Set
    End Property
#End Region

#Region "DB Interaction"
    Public Shared Sub db_CREATE()
        Dim oCmd As String = "CREATE TABLE Player (Key INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)"
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_DROP()
        Dim oCmd As String = "DROP TABLE Player"
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Function db_SELECT(Optional iKey As String = "", Optional iName As String = "") As List(Of Player)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oName As String = Database.cleanInput(iName)

        Dim oCmdWhere As String = ""
        If (oKey <> "") OrElse (oName <> "") Then
            Dim oCmd1 As String = If(oKey = "", "", String.Format("Key = {0}", oKey))
            Dim oCmd2 As String = If(oName = "", "", String.Format("Name = '{0}'", oName))
            oCmdWhere = String.Format("WHERE {0} {1} {2}", oCmd1, If((oCmd1 = "") Or (oCmd2 = ""), "", "AND"), oCmd2)
        End If
        Dim oCmd As String = String.Format("SELECT Key, Name FROM Player {0} ORDER BY Name", oCmdWhere)

        Dim returnVal As New List(Of Player)
        Dim reader As SQLiteDataReader = Database.ExecuteReader(oCmd)
        While reader.Read()
            Dim _Player As New Player(
                reader.GetInt32(0),
                reader.GetString(1)
            )
            returnVal.Add(_Player)
        End While
        Return returnVal
    End Function

    Public Shared Sub db_INSERT(iName As String)
        Dim oName As String = Database.cleanInput(iName)
        Dim oCmd As String = String.Format("INSERT INTO Player ('Name') values ('{0}')", oName)
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_UPDATE(iKey As String, iName As String)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oName As String = Database.cleanInput(iName)
        Dim oCmd As String = String.Format("UPDATE Player SET Name = '{1}' WHERE Key = {0}", oKey, oName)
        Database.ExecuteScalar(oCmd)
    End Sub

    Public Shared Sub db_DELETE(iKey As Integer)
        Dim oKey As String = Database.cleanInput(iKey)
        Dim oCmd As String = String.Format("DELETE FROM Player WHERE Key = {0}", oKey)
        Database.ExecuteScalar(oCmd)
    End Sub
#End Region

End Class
