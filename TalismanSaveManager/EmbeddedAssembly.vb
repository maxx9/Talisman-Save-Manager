Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography

'https://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource

Public Class EmbeddedAssembly
    Shared dic As Dictionary(Of String, Assembly) = Nothing

    Public Shared Sub CreateAssembly(ByVal appName As String, ByVal fileName As String, doExtract As Boolean, doLoad As Boolean)
        If dic Is Nothing Then dic = New Dictionary(Of String, Assembly)()

        Dim ba As Byte() = LoadDllIntoMemory(appName, fileName)

        If doExtract Then
            Dim oFilePathName As String = Util.GetAssemblyRoot() & fileName
            ExtractDll(ba, oFilePathName)
            If doLoad Then
                LoadDllFromFile(oFilePathName)
            End If
        Else
            If doLoad Then
                LoadDllFromMemmory(ba)
            End If
        End If

    End Sub

    Private Shared Function LoadDllIntoMemory(ByVal appName As String, ByVal fileName As String) As Byte()
        Dim ba As Byte() = Nothing
        Dim curAsm As Assembly = Assembly.GetExecutingAssembly()

        Using stm As Stream = curAsm.GetManifestResourceStream(appName & "." & fileName)
            If stm Is Nothing Then Throw New Exception(appName & "." & fileName & " is not found in Embedded Resources.")
            ba = New Byte(CInt(stm.Length) - 1) {}
            stm.Read(ba, 0, CInt(stm.Length))
        End Using

        Return ba
    End Function

    Private Shared Sub LoadDllFromMemmory(ByVal ba As Byte())
        Dim asm As Assembly = Assembly.Load(ba)
        dic.Add(asm.FullName, asm)
    End Sub

    Private Shared Sub LoadDllFromFile(iFilePathName As String)
        Dim asm As Assembly = Assembly.LoadFile(iFilePathName)
        dic.Add(asm.FullName, asm)
    End Sub

    Private Shared Sub ExtractDll(ByVal ba As Byte(), ByVal iFilePathName As String)
        Dim fileOk As Boolean = False

        Using sha1 As SHA1CryptoServiceProvider = New SHA1CryptoServiceProvider()
            Dim fileHash As String = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", String.Empty)

            If File.Exists(iFilePathName) Then
                Dim bb As Byte() = File.ReadAllBytes(iFilePathName)
                Dim fileHash2 As String = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", String.Empty)

                If fileHash = fileHash2 Then
                    fileOk = True
                Else
                    fileOk = False
                End If
            Else
                fileOk = False
            End If
        End Using

        If Not fileOk Then
            System.IO.File.WriteAllBytes(iFilePathName, ba)
        End If
    End Sub

    Public Shared Sub Load(ByVal appName As String, ByVal fileName As String, doAddAssembly As Boolean)
        If dic Is Nothing Then dic = New Dictionary(Of String, Assembly)()
        Dim ba As Byte() = Nothing
        Dim asm As Assembly = Nothing
        Dim curAsm As Assembly = Assembly.GetExecutingAssembly()

        Using stm As Stream = curAsm.GetManifestResourceStream(appName & "." & fileName)
            If stm Is Nothing Then Throw New Exception(appName & "." & fileName & " is not found in Embedded Resources.")
            ba = New Byte(CInt(stm.Length) - 1) {}
            stm.Read(ba, 0, CInt(stm.Length))

            Try
                asm = Assembly.Load(ba)
                dic.Add(asm.FullName, asm)
                Return
            Catch
            End Try
        End Using

        Dim fileOk As Boolean = False
        Dim tempFile As String = ""

        Using sha1 As SHA1CryptoServiceProvider = New SHA1CryptoServiceProvider()
            Dim fileHash As String = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", String.Empty)
            'tempFile = Path.GetTempPath() & fileName
            tempFile = Util.GetAssemblyRoot() & fileName

            If File.Exists(tempFile) Then
                Dim bb As Byte() = File.ReadAllBytes(tempFile)
                Dim fileHash2 As String = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", String.Empty)

                If fileHash = fileHash2 Then
                    fileOk = True
                Else
                    fileOk = False
                End If
            Else
                fileOk = False
            End If
        End Using

        If Not fileOk Then
            System.IO.File.WriteAllBytes(tempFile, ba)
        End If

        If doAddAssembly Then
            asm = Assembly.LoadFile(tempFile)
            dic.Add(asm.FullName, asm)
        End If
    End Sub

    Public Shared Function [Get](ByVal assemblyFullName As String) As Assembly
        If dic Is Nothing OrElse dic.Count = 0 Then Return Nothing
        If dic.ContainsKey(assemblyFullName) Then Return dic(assemblyFullName)
        Return Nothing
    End Function
End Class
