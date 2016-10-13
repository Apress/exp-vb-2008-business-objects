﻿Imports ProjectTracker.Library

''' <summary>
''' Interaction logic for MainForm.xaml
''' </summary>
Partial Public Class MainForm
  Inherits Window

#Region "Navigation and Plumbing"

  Private Shared _principal As ProjectTracker.Library.Security.PTPrincipal
  Private Shared _mainForm As MainForm

  Private _currentControl As UserControl

  Public Sub New()

    InitializeComponent()

    _mainForm = Me

    AddHandler Csla.DataPortal.DataPortalInitInvoke, AddressOf DataPortal_DataPortalInitInvoke

  End Sub

  Private Sub MainForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    ProjectTracker.Library.Security.PTPrincipal.Logout()
    _principal = CType(Csla.ApplicationContext.User, ProjectTracker.Library.Security.PTPrincipal)

    Me.Title = "Project Tracker"

  End Sub

  ''' <summary>
  ''' This method ensures that the thread about to do
  ''' data access has a valid PTPrincipal object, and is
  ''' needed because of the way WPF doesn't move the 
  ''' main thread's principal object to other threads
  ''' automatically.
  ''' </summary>
  ''' <param name="obj"></param>
  Private Sub DataPortal_DataPortalInitInvoke(ByVal obj As Object)
    If Not ReferenceEquals(Csla.ApplicationContext.User, _principal) Then
      Csla.ApplicationContext.User = _principal
    End If
  End Sub


  Public Shared Sub ShowControl(ByVal control As UserControl)
    _mainForm.ShowUserControl(control)
  End Sub

  Private Sub ShowUserControl(ByVal control As UserControl)

    UnHookTitleEvent(_currentControl)

    contentArea.Children.Clear()
    If Not control Is Nothing Then
      contentArea.Children.Add(control)
    End If
    _currentControl = control

    HookTitleEvent(_currentControl)

  End Sub

  Private Sub UnHookTitleEvent(ByVal control As UserControl)

    Dim form As EditForm = TryCast(control, EditForm)
    If Not form Is Nothing Then
      RemoveHandler form.TitleChanged, AddressOf SetTitle
    End If

  End Sub

  Private Sub HookTitleEvent(ByVal control As UserControl)

    SetTitle(control, EventArgs.Empty)
    Dim form As EditForm = TryCast(control, EditForm)
    If Not form Is Nothing Then
      AddHandler form.TitleChanged, AddressOf SetTitle
    End If

  End Sub

  Private Sub SetTitle(ByVal sender As Object, ByVal e As EventArgs)

    Dim form As EditForm = TryCast(sender, EditForm)
    If Not form Is Nothing AndAlso (Not String.IsNullOrEmpty(form.Title)) Then
      _mainForm.Title = String.Format("Project Tracker - {0}", (CType(sender, EditForm)).Title)
    Else
      _mainForm.Title = String.Format("Project Tracker")
    End If

  End Sub

#End Region

#Region "Menu items"

  Private Sub NewProject(ByVal sender As Object, ByVal e As EventArgs)
    Try
      Dim frm As ProjectEdit = New ProjectEdit(Guid.Empty)
      ShowControl(frm)
    Catch ex As System.Security.SecurityException
      MessageBox.Show(ex.ToString())
    End Try
  End Sub

  Private Sub ShowProjectList(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As ProjectList = New ProjectList()
    ShowControl(frm)
  End Sub

  Private Sub ShowResourceList(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As ResourceList = New ResourceList()
    ShowControl(frm)
  End Sub

  Private Sub NewResource(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As ResourceEdit = New ResourceEdit(0)
    ShowControl(frm)
  End Sub

  Private Sub ShowRolesEdit(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As RolesEdit = New RolesEdit()
    ShowControl(frm)
  End Sub

#End Region

#Region "Login/Logout"

  Private Sub LogInOut(ByVal sender As Object, ByVal e As EventArgs)
    If Csla.ApplicationContext.User.Identity.IsAuthenticated Then
      ProjectTracker.Library.Security.PTPrincipal.Logout()
      CurrentUser.Text = "Not logged in"
      LoginButtonText.Text = "Log in"
    Else
      Dim frm As Login = New Login()
      frm.ShowDialog()
      If frm.Result Then
        Dim username As String = frm.UsernameTextBox.Text
        Dim password As String = frm.PasswordTextBox.Password
        ProjectTracker.Library.Security.PTPrincipal.Login(username, password)
      End If
      If (Not Csla.ApplicationContext.User.Identity.IsAuthenticated) Then
        ProjectTracker.Library.Security.PTPrincipal.Logout()
        CurrentUser.Text = "Not logged in"
        LoginButtonText.Text = "Log in"
      Else
        CurrentUser.Text = String.Format("Logged in as {0}", Csla.ApplicationContext.User.Identity.Name)
        LoginButtonText.Text = "Log out"
      End If
    End If
    _principal = _
      CType(Csla.ApplicationContext.User, ProjectTracker.Library.Security.PTPrincipal)
    Dim p As IRefresh = TryCast(_currentControl, IRefresh)
    If Not p Is Nothing Then
      p.Refresh()
    End If
  End Sub

#End Region

  Private Sub CloseProject(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    Dim frm As New ProjectSelect()
    Dim result As Boolean = frm.ShowDialog
    If result Then
      Dim id As Guid = frm.ProjectId
      Try
        ProjectCloser.CloseProject(id)
        MessageBox.Show("Project closed", _
          "Close project", MessageBoxButton.OK, MessageBoxImage.Information)

      Catch ex As Csla.DataPortalException
        MessageBox.Show(ex.BusinessException.Message, _
          "Close project", MessageBoxButton.OK, MessageBoxImage.Exclamation)

      Catch ex As Exception
        MessageBox.Show(ex.ToString, _
          "Close project", MessageBoxButton.OK, MessageBoxImage.Warning)
      End Try
    End If

  End Sub

End Class