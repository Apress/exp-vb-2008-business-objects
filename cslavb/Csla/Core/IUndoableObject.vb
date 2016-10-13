Namespace Core

  ''' <summary>
  ''' Defines the methods required to participate
  ''' in n-level undo within the CSLA .NET framework.
  ''' </summary>
  ''' <remarks>
  ''' This interface is used by Csla.Core.UndoableBase
  ''' to initiate begin, cancel and apply edit operations.
  ''' </remarks>
  Public Interface IUndoableObject

    ''' <summary>
    ''' Gets the current edit level of the object.
    ''' </summary>
    ReadOnly Property EditLevel() As Integer
    ''' <summary>
    ''' Copies the state of the object and places the copy
    ''' onto the state stack.
    ''' </summary>
    ''' <param name="parentEditLevel">
    ''' Parent object's edit level.
    ''' </param>
    ''' <param name="parentBindingEdit">
    ''' Indicates whether parent is in edit mode due to data binding.
    ''' </param>
    Sub CopyState(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean)
    ''' <summary>
    ''' Restores the object's state to the most recently
    ''' copied values from the state stack.
    ''' </summary>
    ''' <remarks>
    ''' Restores the state of the object to its
    ''' previous value by taking the data out of 
    ''' the stack and restoring it into the fields
    ''' of the object.
    ''' </remarks>
    ''' <param name="parentEditLevel">
    ''' Parent object's edit level.
    ''' </param>
    ''' <param name="parentBindingEdit">
    ''' Indicates whether parent is in edit mode due to data binding.
    ''' </param>
    Sub UndoChanges(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean)
    ''' <summary>
    ''' Accepts any changes made to the object since the last
    ''' state copy was made.
    ''' </summary>
    ''' <remarks>
    ''' The most recent state copy is removed from the state
    ''' stack and discarded, thus committing any changes made
    ''' to the object's state.
    ''' </remarks>
    ''' <param name="parentEditLevel">
    ''' Parent object's edit level.
    ''' </param>
    ''' <param name="parentBindingEdit">
    ''' Indicates whether parent is in edit mode due to data binding.
    ''' </param>
    Sub AcceptChanges(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean)
  End Interface

End Namespace
