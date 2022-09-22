Imports System.Reflection

Class Application

    Public Sub New()
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
    End Sub

    Private Shared Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal args As UnhandledExceptionEventArgs)
        MessageBox.Show(args.ExceptionObject.ToString())
    End Sub

End Class
