#If Not CLIENTONLY Then
Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Object responsible for providing details about
  ''' data binding to a specific CSLA .NET object.
  ''' </summary>
  Public Class CslaDesignerDataSourceView
    Inherits DesignerDataSourceView

    Private _owner As CslaDataSourceDesigner = Nothing

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New(ByVal owner As CslaDataSourceDesigner, ByVal viewName As String)

      MyBase.New(owner, viewName)
      _owner = owner

    End Sub

    ''' <summary>
    ''' Returns a set of sample data used to populate
    ''' controls at design time.
    ''' </summary>
    ''' <param name="minimumRows">Minimum number of sample rows
    ''' to create.</param>
    ''' <param name="isSampleData">Returns True if the data
    ''' is sample data.</param>
    Public Overrides Function GetDesignTimeData( _
        ByVal minimumRows As Integer, _
        ByRef isSampleData As Boolean) As IEnumerable

      Dim schema As IDataSourceViewSchema = Me.Schema
      Dim result As New DataTable

      ' create the columns
      For Each item As IDataSourceFieldSchema In schema.GetFields
        result.Columns.Add(item.Name, item.DataType)
      Next

      ' create sample data
      For index As Integer = 1 To minimumRows
        Dim values(result.Columns.Count - 1) As Object
        Dim colIndex As Integer = 0
        For Each col As DataColumn In result.Columns
          If col.DataType.Equals(GetType(String)) Then
            values(colIndex) = "abc"
          ElseIf col.DataType.Equals(GetType(Date)) Then
            values(colIndex) = Today.ToShortDateString
          ElseIf col.DataType.Equals(GetType(DateTimeOffset)) Then
            values(colIndex) = Today.ToShortDateString
          ElseIf col.DataType.Equals(GetType(Boolean)) Then
            values(colIndex) = False
          ElseIf col.DataType.IsPrimitive Then
            values(colIndex) = index
          ElseIf col.DataType.Equals(GetType(Guid)) Then
            values(colIndex) = Guid.Empty
          ElseIf col.DataType.IsValueType Then
            values(colIndex) = _
              Activator.CreateInstance(col.DataType)
          Else
            values(colIndex) = Nothing
          End If
          colIndex += 1
        Next
        result.LoadDataRow(values, LoadOption.OverwriteChanges)
      Next

      isSampleData = True
      Return CType(result.DefaultView, IEnumerable)

    End Function

    ''' <summary>
    ''' Returns schema information corresponding to the properties
    ''' of the CSLA .NET business object.
    ''' </summary>
    ''' <remarks>
    ''' All public properties are returned except for those marked
    ''' with the <see cref="BrowsableAttribute">Browsable attribute</see>
    ''' as False.
    ''' </remarks>
    Public Overrides ReadOnly Property Schema() As IDataSourceViewSchema
      Get
        Return New ObjectSchema(_owner, _
          _owner.DataSourceControl.TypeName).GetViews(0)
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can retrieve
    ''' the total number of rows of data.
    ''' </summary>
    Public Overrides ReadOnly Property CanRetrieveTotalRowCount() As Boolean
      Get
        Return True
      End Get
    End Property

    Private Function GetObjectType() As Type

      Dim result As Type
      Try
        Dim typeService As ITypeResolutionService
        typeService = DirectCast( _
          _owner.Site.GetService( _
          GetType(ITypeResolutionService)), ITypeResolutionService)
        result = typeService.GetType(Me._owner.DataSourceControl.TypeName, True, False)

      Catch ex As Exception
        result = GetType(Object)
      End Try
      Return result

    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' delete the object.
    ''' </summary>
    ''' <remarks>
    ''' If this returns true, the web page must handle the
    ''' <see cref="CslaDataSource.DeleteObject">DeleteObject</see>
    ''' event.
    ''' </remarks>
    Public Overrides ReadOnly Property CanDelete() As Boolean
      Get
        Dim objectType As Type = GetObjectType()
        If GetType(Core.IUndoableObject).IsAssignableFrom(objectType) Then
          Return True
        ElseIf Not objectType.GetMethod("Remove") Is Nothing Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' insert an instance of the object.
    ''' </summary>
    ''' <remarks>
    ''' If this returns true, the web page must handle the
    ''' <see cref="CslaDataSource.InsertObject">InsertObject</see>
    ''' event.
    ''' </remarks>
    Public Overrides ReadOnly Property CanInsert() As Boolean
      Get
        Dim objectType As Type = GetObjectType()
        If GetType(Core.IUndoableObject).IsAssignableFrom(objectType) Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' update or edit the object.
    ''' </summary>
    ''' <remarks>
    ''' If this returns true, the web page must handle the
    ''' <see cref="CslaDataSource.UpdateObject">UpdateObject</see>
    ''' event.
    ''' </remarks>
    Public Overrides ReadOnly Property CanUpdate() As Boolean
      Get
        Dim objectType As Type = GetObjectType()
        If GetType(Core.IUndoableObject).IsAssignableFrom(objectType) Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the data source supports
    ''' paging.
    ''' </summary>
    Public Overrides ReadOnly Property CanPage() As Boolean
      Get
        Return _owner.DataSourceControl.TypeSupportsPaging
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the data source supports
    ''' sorting.
    ''' </summary>
    Public Overrides ReadOnly Property CanSort() As Boolean
      Get
        Return _owner.DataSourceControl.TypeSupportsSorting
      End Get
    End Property

  End Class

End Namespace
#End If
