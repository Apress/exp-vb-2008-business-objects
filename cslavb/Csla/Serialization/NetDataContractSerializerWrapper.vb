Imports System.Runtime.Serialization

Namespace Serialization

  ''' Wraps the <see cref="NetDataContractSerializer"/>
  ''' in the 
  ''' <see cref="ISerializationFormatter"/>
  ''' interface so it can be used in a standardized
  ''' manner.
  Public Class NetDataContractSerializerWrapper
    Implements ISerializationFormatter

    Private _formatter As NetDataContractSerializer = New NetDataContractSerializer()

#Region "ISerializationFormatter Members"

    ''' <summary>
    ''' Converts a serialization stream into an
    ''' object graph.
    ''' </summary>
    ''' <param name="serializationStream">
    ''' Byte stream containing the serialized data.</param>
    ''' <returns>A deserialized object graph.</returns>
    Public Function Deserialize(ByVal serializationStream As System.IO.Stream) As Object _
      Implements ISerializationFormatter.Deserialize

      Return _formatter.Deserialize(serializationStream)

    End Function

    ''' <summary>
    ''' Converts an object graph into a byte stream.
    ''' </summary>
    ''' <param name="serializationStream">
    ''' Stream that will contain the the serialized data.</param>
    ''' <param name="graph">Object graph to be serialized.</param>
    Public Sub Serialize(ByVal serializationStream As System.IO.Stream, ByVal graph As Object) _
      Implements ISerializationFormatter.Serialize

      _formatter.Serialize(serializationStream, graph)

    End Sub

#End Region

    ''' <summary>
    ''' Gets a reference to the underlying
    ''' <see cref="NetDataContractSerializer"/>
    ''' object.
    ''' </summary>
    Public ReadOnly Property Formatter() As NetDataContractSerializer
      Get
        Return _formatter
      End Get
    End Property

  End Class

End Namespace