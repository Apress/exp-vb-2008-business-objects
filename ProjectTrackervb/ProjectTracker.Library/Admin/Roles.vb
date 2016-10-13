Namespace Admin

  ''' <summary>
  ''' Used to maintain the list of roles
  ''' in the system.
  ''' </summary>
  <Serializable()> _
  Public Class Roles
    Inherits BusinessListBase(Of Roles, Role)

#Region " Business Methods "

    ''' <summary>
    ''' Remove a role based on the role's
    ''' id value.
    ''' </summary>
    ''' <param name="id">Id value of the role to remove.</param>
    Public Overloads Sub Remove(ByVal id As Integer)

      For Each item As Role In Me
        If item.Id = id Then
          Remove(item)
          Exit For
        End If
      Next

    End Sub

    ''' <summary>
    ''' Get a role bsaed on its id value.
    ''' </summary>
    ''' <param name="id">Id value of the role to return.</param>
    Public Function GetRoleById(ByVal id As Integer) As Role

      For Each item As Role In Me
        If item.Id = id Then
          Return item
        End If
      Next
      Return Nothing

    End Function

    Protected Overrides Function AddNewCore() As Object

      Dim item As Role = Role.NewRole
      Add(item)
      Return item

    End Function

#End Region

#Region " Authorization Rules "

    Public Shared Function CanAddObject() As Boolean

      Return Csla.ApplicationContext.User.IsInRole("Administrator")

    End Function

    Public Shared Function CanGetObject() As Boolean

      Return True

    End Function

    Public Shared Function CanDeleteObject() As Boolean

      Dim result As Boolean
      If Csla.ApplicationContext.User.IsInRole("Administrator") Then
        result = True
      End If
      Return result

    End Function

    Public Shared Function CanEditObject() As Boolean

      Return Csla.ApplicationContext.User.IsInRole("Administrator")

    End Function

#End Region

#Region " Factory Methods "

    Public Shared Function GetRoles() As Roles

      Return DataPortal.Fetch(Of Roles)()

    End Function

    Private Sub New()

      Me.AllowNew = True

    End Sub

#End Region

#Region " Data Access "

    Public Overrides Function Save() As Roles

      ' see if save is allowed
      If Not CanEditObject() Then
        Throw New System.Security.SecurityException( _
          "User not authorized to save roles")
      End If
      Return MyBase.Save()


    End Function

    Private Sub Roles_Saved(ByVal sender As Object, ByVal e As Csla.Core.SavedEventArgs) Handles Me.Saved

      ' this runs on the client and invalidates
      ' the RoleList cache
      RoleList.InvalidateCache()

    End Sub

    Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete( _
      ByVal e As Csla.DataPortalEventArgs)

      If ApplicationContext.ExecutionLocation = _
        ApplicationContext.ExecutionLocations.Server AndAlso _
        e.Operation = DataPortalOperations.Update Then

        ' this runs on the server and invalidates
        ' the RoleList cache
        RoleList.InvalidateCache()
      End If

    End Sub

    Private Overloads Sub DataPortal_Fetch()

      Me.RaiseListChangedEvents = False
      Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
        For Each value In ctx.DataContext.getRoles
          Me.Add(Role.GetRole(value))
        Next
      End Using
      Me.RaiseListChangedEvents = True

    End Sub

    <Transactional(TransactionalTypes.TransactionScope)> _
    Protected Overrides Sub DataPortal_Update()

      Me.RaiseListChangedEvents = False
      Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
        Child_Update()
      End Using
      Me.RaiseListChangedEvents = True

    End Sub

#End Region

  End Class

End Namespace
