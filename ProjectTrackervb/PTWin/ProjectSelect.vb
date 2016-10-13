Imports System.Windows.Forms
Imports System.ComponentModel

Public Class ProjectSelect

  Private _projectId As Guid

  Public ReadOnly Property ProjectId() As Guid
    Get
      Return _projectId
    End Get
  End Property

  Private Sub AcceptValue()

    _projectId = CType(Me.ProjectListListBox.SelectedValue, Guid)
    Me.DialogResult = Windows.Forms.DialogResult.OK
    Me.Close()

  End Sub

  Private Sub OK_Button_Click( _
      ByVal sender As System.Object, ByVal e As System.EventArgs) _
      Handles OK_Button.Click

    AcceptValue()

  End Sub

  Private Sub ProjectListListBox_MouseDoubleClick( _
    ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
    Handles ProjectListListBox.MouseDoubleClick

    AcceptValue()

  End Sub

  Private Sub Cancel_Button_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles Cancel_Button.Click

    Me.Close()

  End Sub

  Private Sub ProjectSelect_Load( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

    DisplayList(ProjectList.GetProjectList)

  End Sub

  Private Sub DisplayList(ByVal list As ProjectList)

    Dim sortedList = From p In list Order By p.Name
    Me.ProjectListBindingSource.DataSource = sortedList

  End Sub

  Private Sub GetListButton_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles GetListButton.Click

    DisplayList(ProjectList.GetProjectList(NameTextBox.Text))

  End Sub

End Class
