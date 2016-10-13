Imports System

Friend Module DefaultFilter

  Public Function Filter(ByVal item As Object, ByVal filterValue As Object) As Boolean

    Dim result As Boolean = False

    If Not item Is Nothing AndAlso Not filterValue Is Nothing Then
      result = CStr(item).Contains(CStr(filterValue))
    End If

    Return result

  End Function

End Module
