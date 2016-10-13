Imports System

''' <summary>
''' Provides information about the DataPortal 
''' call.
''' </summary>
Public Class DataPortalEventArgs
  Inherits EventArgs

  Private _dataPortalContext As Server.DataPortalContext
  Private _operation As DataPortalOperations
  Private _exception As Exception
  Private _objectType As Type

  ''' <summary>
  ''' The DataPortalContext object passed to the
  ''' server-side DataPortal.
  ''' </summary>
  Public ReadOnly Property DataPortalContext() As Server.DataPortalContext
    Get
      Return _dataPortalContext
    End Get
  End Property

  ''' <summary>
  ''' Gets the requested data portal operation.
  ''' </summary>
  Public ReadOnly Property Operation() As DataPortalOperations
    Get
      Return _operation
    End Get
  End Property

  ''' <summary>
  ''' Gets a reference to any exception that occurred
  ''' during the data portal call.
  ''' </summary>
  ''' <remarks>
  ''' This property will return Nothing (null in C#) if no
  ''' exception occurred. Exceptions are returned only as part
  ''' of a data portal complete event or method.
  ''' </remarks>
  Public ReadOnly Property Exception() As Exception
    Get
      Return _exception
    End Get
  End Property

  ''' <summary>
  ''' Gets the object type being processed by the 
  ''' data portal.
  ''' </summary>
  Public ReadOnly Property ObjectType() As Type
    Get
      Return _objectType
    End Get
  End Property

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="dataPortalContext">
  ''' Data portal context object.
  ''' </param>
  ''' <param name="objectType">
  ''' Business object type.
  ''' </param>
  ''' <param name="operation">
  ''' Data portal operation being performed.
  ''' </param>
  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext, ByVal objectType As Type, ByVal operation As DataPortalOperations)
    _dataPortalContext = dataPortalContext
    _operation = operation
    _objectType = objectType
  End Sub

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="dataPortalContext">
  ''' Data portal context object.
  ''' </param>
  ''' <param name="objectType">
  ''' Business object type.
  ''' </param>
  ''' <param name="operation">
  ''' Data portal operation being performed.
  ''' </param>
  ''' <param name="exception">
  ''' Exception encountered during processing.
  ''' </param>
  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext, _
                 ByVal objectType As Type, _
                 ByVal operation As DataPortalOperations, _
                 ByVal exception As Exception)
    Me.New(dataPortalContext, objectType, operation)
    _exception = exception
  End Sub

End Class