Imports System.ComponentModel

Namespace Security

  ''' <summary>
  ''' Maintains a list of all the per-type
  ''' <see cref="AuthorizationRulesManager"/> objects
  ''' loaded in memory.
  ''' </summary>
  Friend Module SharedAuthorizationRules

    Private _managers As New Dictionary(Of Type, AuthorizationRulesManager)

    ''' <summary>
    ''' Gets the <see cref="AuthorizationRulesManager"/> for the 
    ''' specified object type, optionally creating a new instance 
    ''' of the object if necessary.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of business object for which the rules apply.
    ''' </param>
    ''' <param name="create">Indicates whether to create
    ''' a new instance of the object if one doesn't exist.</param>
    Friend Function GetManager(ByVal objectType As Type, ByVal create As Boolean) _
      As AuthorizationRulesManager

      Dim result As AuthorizationRulesManager = Nothing
      If Not _managers.TryGetValue(objectType, result) AndAlso create Then
        SyncLock _managers
          If Not _managers.TryGetValue(objectType, result) Then
            result = New AuthorizationRulesManager
            _managers.Add(objectType, result)
          End If
        End SyncLock
      End If
      Return result

    End Function

    ''' <summary>
    ''' Gets a value indicating whether a set of rules
    ''' have been created for a given <see cref="Type" />.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of business object for which the rules apply.
    ''' </param>
    ''' <returns><see langword="true" /> if rules exist for the type.</returns>
    Public Function RulesExistFor(ByVal objectType As Type) As Boolean

      Return _managers.ContainsKey(objectType)

    End Function

  End Module

End Namespace
