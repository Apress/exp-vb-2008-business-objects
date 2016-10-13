<Serializable()> _
Public Class ProjectList
  Inherits ReadOnlyListBase(Of ProjectList, ProjectInfo)

#Region " Factory Methods "

  Public Shared Function GetProjectList() As ProjectList

    Return DataPortal.Fetch(Of ProjectList)()

  End Function

  Public Shared Function GetProjectList(ByVal name As String) As ProjectList

    Return DataPortal.Fetch(Of ProjectList)(New SingleCriteria(Of ProjectList, String)(name))

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    ' fetch with no filter
    Fetch("")

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of ProjectList, String))

    Fetch(criteria.Value)

  End Sub

  Private Sub Fetch(ByVal nameFilter As String)

    RaiseListChangedEvents = False
    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      Dim data = From p In ctx.DataContext.Projects _
                 Select New ProjectInfo(p.Id, p.Name)
      If Not String.IsNullOrEmpty(nameFilter) Then _
        data = From p In ctx.DataContext.Projects _
               Where p.Name Like "*" & nameFilter & "*" _
               Select New ProjectInfo(p.Id, p.Name)
      IsReadOnly = False
      Me.AddRange(data)
      IsReadOnly = True
    End Using
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
