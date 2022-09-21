Imports System.Reflection

Class Application

    Public Sub New()
        If Util.initInit() Then
            EmbeddedAssembly.CreateAssembly(Assembly.GetEntryAssembly().GetName().Name, "msvcr120.dll", True, False)
            EmbeddedAssembly.CreateAssembly(Assembly.GetEntryAssembly().GetName().Name, "System.Data.SQLite.dll", True, True)
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
        End If
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
    End Sub

    Private Shared Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly
        Return EmbeddedAssembly.[Get](args.Name)
    End Function

    Private Shared Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal args As UnhandledExceptionEventArgs)
        MessageBox.Show(args.ExceptionObject.ToString())
    End Sub

End Class
