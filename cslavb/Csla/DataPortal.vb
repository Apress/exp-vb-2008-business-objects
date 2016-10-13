Imports System
Imports System.ComponentModel
Imports System.Reflection
Imports Csla.Reflection
Imports System.Windows
Imports Csla.Serialization.Mobile
Imports Csla.Server

''' <summary>
''' This is the client-side DataPortal as described in
''' Chapter 4.
''' </summary>
Public Module DataPortal

#Region " DataPortal events "

  ''' <summary>
  ''' Raised by DataPortal before it starts
  ''' setting up to call a server-side
  ''' DataPortal method.
  ''' </summary>
  Public Event DataPortalInitInvoke As Action(Of System.Object)
  ''' <summary>
  ''' Raised by DataPortal prior to calling the 
  ''' requested server-side DataPortal method.
  ''' </summary>
  Public Event DataPortalInvoke As Action(Of DataPortalEventArgs)
  ''' <summary>
  ''' Raised by DataPortal after the requested 
  ''' server-side DataPortal method call is complete.
  ''' </summary>
  Public Event DataPortalInvokeComplete As Action(Of DataPortalEventArgs)

  Private Sub OnDataPortalInitInvoke(ByVal e As Object)
    'TODO: Do these need to be sync'd?
    RaiseEvent DataPortalInitInvoke(e)

  End Sub

  Private Sub OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

    RaiseEvent DataPortalInvoke(e)

  End Sub

  Private Sub OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

    RaiseEvent DataPortalInvokeComplete(e)

  End Sub

#End Region

#Region " Data Access methods "

  Private Const EmptyCriteria As Integer = 1

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>A new object, populated with default values.</returns>
  Public Function Create(Of T)(ByVal criteria As Object) As T
    Return DirectCast(Create(GetType(T), criteria), T)
  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <returns>A new object, populated with default values.</returns>
  Public Function Create(Of T)() As T
    Return DirectCast(Create(GetType(T), EmptyCriteria), T)
  End Function

  Friend Function Create(ByVal objectType As Type) As Object
    Return Create(objectType, EmptyCriteria)
  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>A new object, populated with default values.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")> _
  Public Function Create(ByVal criteria As Object) As Object

    Return Create(MethodCaller.GetObjectType(criteria), criteria)

  End Function

  Friend Function Create( _
    ByVal objectType As Type, ByVal criteria As Object) As Object

    Dim result As Server.DataPortalResult
    Dim dpContext As Server.DataPortalContext = Nothing
    Try
      OnDataPortalInitInvoke(Nothing)

      If Not Csla.Security.AuthorizationRules.CanCreateObject(objectType) Then
        Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
          "create", _
          objectType.Name))
      End If

      Dim method = Server.DataPortalMethodCache.GetCreateMethod(objectType, criteria)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(method.RunLocal)

      dpContext = New Server.DataPortalContext( _
        GetPrincipal(), proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Create))

      Try
        result = proxy.Create(objectType, criteria, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Create {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Create))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Create, ex))
      Throw
    End Try

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>An object populated with values from the database.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Fetch(Of T)(ByVal criteria As Object) As T

    Return DirectCast(Fetch(GetType(T), criteria), T)

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <returns>An object populated with values from the database.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Fetch(Of T)() As T

    Return DirectCast(Fetch(GetType(T), EmptyCriteria), T)

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>An object populated with values from the database.</returns>
  Public Function Fetch(ByVal criteria As Object) As Object

    Return Fetch(MethodCaller.GetObjectType(criteria), criteria)

  End Function

  Friend Function Fetch(ByVal objectType As Type) As Object
    Return Fetch(objectType, EmptyCriteria)
  End Function

  Friend Function Fetch( _
    ByVal objectType As Type, ByVal criteria As Object) As Object

    Dim result As Server.DataPortalResult
    Dim dpContext As Server.DataPortalContext = Nothing
    Try
      OnDataPortalInitInvoke(Nothing)

      If Not Csla.Security.AuthorizationRules.CanGetObject(objectType) Then
        Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
          "get", _
          objectType.Name))
      End If

      Dim method = Server.DataPortalMethodCache.GetFetchMethod(objectType, criteria)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(method.RunLocal)

      dpContext = New Server.DataPortalContext( _
        GetPrincipal(), proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Fetch))

      Try
        result = proxy.Fetch(objectType, criteria, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Dim innerMessage As String = String.Empty
        If (TypeOf ex.InnerException Is Csla.Reflection.CallMethodException) Then
          If ex.InnerException.InnerException IsNot Nothing Then
            innerMessage = ex.InnerException.InnerException.Message
          End If
        Else
          innerMessage = ex.InnerException.Message
        End If


        Throw New DataPortalException( _
          String.Format("DataPortal.Fetch {0} ({1})", My.Resources.Failed, innerMessage), _
          ex.InnerException, result.ReturnObject)
        
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Fetch))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Fetch, ex))
      Throw
    End Try

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called to execute a Command object on the server.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' To be a Command object, the object must inherit from
  ''' <see cref="CommandBase">CommandBase</see>.
  ''' </para><para>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </para><para>
  ''' On the server, the Command object's DataPortal_Execute() method will
  ''' be invoked. Write any server-side code in that method.
  ''' </para>
  ''' </remarks>
  ''' <typeparam name="T">Specific type of the Command object.</typeparam>
  ''' <param name="obj">A reference to the Command object to be executed.</param>
  ''' <returns>A reference to the updated Command object.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", _
    MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Execute(Of T)(ByVal obj As T) As T

    Return DirectCast(Update(obj), T)

  End Function

  ''' <summary>
  ''' Called to execute a Command object on the server.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </para><para>
  ''' On the server, the Command object's DataPortal_Execute() method will
  ''' be invoked. Write any server-side code in that method.
  ''' </para>
  ''' </remarks>
  ''' <param name="obj">A reference to the Command object to be executed.</param>
  ''' <returns>A reference to the updated Command object.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Execute(ByVal obj As CommandBase) As CommandBase

    Return DirectCast(Update(obj), CommandBase)

  End Function

  ''' <summary>
  ''' Called by the business object's Save() method to
  ''' insert, update or delete an object in the database.
  ''' </summary>
  ''' <remarks>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </remarks>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <param name="obj">A reference to the business object to be updated.</param>
  ''' <returns>A reference to the updated business object.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Update(Of T)(ByVal obj As T) As T

    Return DirectCast(Update(CObj(obj)), T)

  End Function

  ''' <summary>
  ''' Called by the business object's Save() method to
  ''' insert, update or delete an object in the database.
  ''' </summary>
  ''' <remarks>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </remarks>
  ''' <param name="obj">A reference to the business object to be updated.</param>
  ''' <returns>A reference to the updated business object.</returns>
  Public Function Update(ByVal obj As Object) As Object

    Dim result As Server.DataPortalResult = Nothing
    Dim dpContext As Server.DataPortalContext = Nothing
    Dim operation As DataPortalOperations = DataPortalOperations.Update
    Dim objectType = obj.GetType()
    Try
      OnDataPortalInitInvoke(Nothing)

      Dim method As DataPortalMethodInfo = Nothing
      Dim factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType)
      If factoryInfo IsNot Nothing Then
        Dim factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName)
        Dim bbase As Core.BusinessBase = DirectCast(obj, Core.BusinessBase)
        If bbase IsNot Nothing AndAlso bbase.IsDeleted Then
          If Not Csla.Security.AuthorizationRules.CanDeleteObject(objectType) Then
            Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
                "delete", objectType.Name))
          End If
          If factoryType IsNot Nothing Then
            method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, New Object() {obj})
          End If
        Else
          If Not Csla.Security.AuthorizationRules.CanEditObject(objectType) Then
            Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
                "save", objectType.Name))
          End If
          If factoryType IsNot Nothing Then
            method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, New Object() {obj})
          End If
        End If

        If method Is Nothing Then
          method = New DataPortalMethodInfo()
        End If
      Else
        Dim methodName As String
        If TypeOf obj Is CommandBase Then
          methodName = "DataPortal_Execute"
          operation = DataPortalOperations.Execute
          If Not Csla.Security.AuthorizationRules.CanEditObject(objectType) Then
            Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
              "execute", _
              objectType.Name))
          End If

        ElseIf TypeOf obj Is Core.BusinessBase Then
          Dim tmp As Core.BusinessBase = DirectCast(obj, Core.BusinessBase)
          If tmp.IsDeleted Then
            methodName = "DataPortal_DeleteSelf"
            If Not Csla.Security.AuthorizationRules.CanDeleteObject(objectType) Then
              Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
                "delete", _
                objectType.Name))
            End If
          Else
            If tmp.IsNew Then
              methodName = "DataPortal_Insert"
              If Not Csla.Security.AuthorizationRules.CanCreateObject(objectType) Then
                Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
                  "create", _
                  objectType.Name))
              End If

            Else
              methodName = "DataPortal_Update"
              If Not Csla.Security.AuthorizationRules.CanEditObject(objectType) Then
                Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
                  "save", _
                  objectType.Name))
              End If
            End If
          End If
        Else
          methodName = "DataPortal_Update"
          If Not Csla.Security.AuthorizationRules.CanEditObject(objectType) Then
            Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
              "save", _
              objectType.Name))
          End If
        End If

        method = Server.DataPortalMethodCache.GetMethodInfo(obj.GetType, methodName)

      End If

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(method.RunLocal)

      dpContext = New Server.DataPortalContext(GetPrincipal, proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, objectType, operation))

      Try
        If Not proxy.IsServerRemote AndAlso ApplicationContext.AutoCloneOnUpdate Then
          ' when using local data portal, automatically
          ' clone original object before saving
          Dim cloneable As ICloneable = TryCast(obj, ICloneable)
          If cloneable IsNot Nothing Then
            obj = cloneable.Clone
          End If
        End If
        result = proxy.Update(obj, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Update {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, operation))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, operation, ex))
      Throw
    End Try

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by a Shared (static in C#) method in the business class to cause
  ''' immediate deletion of a specific object from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage( _
    "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", _
    MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Sub Delete(Of T)(ByVal criteria As Object)

    Delete(GetType(T), criteria)

  End Sub

  ''' <summary>
  ''' Called by a Shared (static in C#) method in the business class to cause
  ''' immediate deletion of a specific object from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage( _
    "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", _
    MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Sub Delete(ByVal criteria As Object)

    Dim objectType = MethodCaller.GetObjectType(criteria)
    Delete(objectType, criteria)

  End Sub

  ''' <summary>
  ''' Called by a Shared (static in C#) method in the business class to cause
  ''' immediate deletion of a specific object from the database.
  ''' </summary>
  ''' <param name="objectType">Type of business object to delete.</param>
  ''' <param name="criteria">Object-specific criteria.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Private Sub Delete(ByVal objectType As Type, ByVal criteria As Object)

    Dim result As Server.DataPortalResult = Nothing
    Dim dpContext As Server.DataPortalContext = Nothing
    Try
      OnDataPortalInitInvoke(Nothing)

      If Not Csla.Security.AuthorizationRules.CanDeleteObject(objectType) Then
        Throw New System.Security.SecurityException(String.Format(My.Resources.UserNotAuthorizedException, _
          "delete", _
          objectType.Name))
      End If

      Dim method = Server.DataPortalMethodCache.GetMethodInfo(objectType, "DataPortal_Delete", criteria)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(method.RunLocal)

      dpContext = New Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Delete))

      Try
        result = proxy.Delete(objectType, criteria, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Delete {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Delete))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Delete, ex))
      Throw
    End Try

  End Sub

#End Region

#Region "Async Data Access Methods"

#Region "Begin Create"

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' create a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to create.
  ''' </typeparam>
  ''' <param name="callback">
  '''  Reference to method that will handle the 
  '''  asynchronous callback when the operation
  ''' is complete.
  ''' </param>  
  Public Sub BeginCreate(Of T As IMobileObject)(ByVal callback As EventHandler(Of DataPortalResult(Of T)))
    BeginCreate(Of T)(DataPortal(Of T).EmptyCriteria, callback, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' create a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to create.
  ''' </typeparam>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>    
  Public Sub BeginCreate(Of T As IMobileObject)(ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)
    BeginCreate(Of T)(DataPortal(Of T).EmptyCriteria, callback, userState)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' create a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to create.
  ''' </typeparam>
  ''' <param name="criteria">
  ''' Criteria describing the object to create.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  Public Sub BeginCreate(Of T As IMobileObject)(ByVal criteria As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)))
    BeginCreate(Of T)(criteria, callback, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' create a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to create.
  ''' </typeparam>
  ''' <param name="criteria">
  ''' Criteria describing the object to create.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>   
  Public Sub BeginCreate(Of T As IMobileObject)(ByVal criteria As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)
    Dim dp As DataPortal(Of T) = New DataPortal(Of T)()
    AddHandler dp.CreateCompleted, callback
    dp.BeginCreate(criteria, userState)
  End Sub

#End Region

#Region "Begin Fetch"

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' fetch a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to fetch.
  ''' </typeparam>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>  
  Public Sub BeginFetch(Of T As IMobileObject)(ByVal callback As EventHandler(Of DataPortalResult(Of T)))
    BeginFetch(Of T)(DataPortal(Of T).EmptyCriteria, callback, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' fetch a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to fetch.
  ''' </typeparam>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>  
  Public Sub BeginFetch(Of T As IMobileObject)(ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)
    BeginFetch(Of T)(DataPortal(Of T).EmptyCriteria, callback, userState)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' fetch a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to fetch.
  ''' </typeparam>
  ''' <param name="criteria">
  ''' Criteria describing the object to fetch.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>  
  Public Sub BeginFetch(Of T As IMobileObject)(ByVal criteria As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)))
    BeginFetch(Of T)(criteria, callback, Nothing)
  End Sub


  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' fetch a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to fetch.
  ''' </typeparam>
  ''' <param name="criteria">
  ''' Criteria describing the object to fetch.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>  
  Public Sub BeginFetch(Of T As IMobileObject)(ByVal criteria As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)
    Dim dp As DataPortal(Of T) = New DataPortal(Of T)()
    AddHandler dp.FetchCompleted, callback
    dp.BeginFetch(criteria, userState)
  End Sub

#End Region

#Region " Begin Update "

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' update a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to update.
  ''' </typeparam>
  ''' <param name="obj">
  ''' Business object to update.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  Public Sub BeginUpdate(Of T As IMobileObject)(ByVal obj As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)))

    BeginUpdate(Of T)(obj, callback, Nothing)

  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' update a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to update.
  ''' </typeparam>
  ''' <param name="obj">
  ''' Business object to update.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>
  Public Sub BeginUpdate(Of T As IMobileObject)(ByVal obj As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)

    Dim dp As DataPortal(Of T) = New DataPortal(Of T)()
    AddHandler dp.UpdateCompleted, callback
    dp.BeginUpdate(obj, userState)

  End Sub

#End Region

#Region "Begin Delete"

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' delete a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to delete.
  ''' </typeparam>
  ''' <param name="criteria">
  ''' Criteria describing the object to delete.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>  
  Public Sub BeginDelete(Of T As IMobileObject)(ByVal criteria As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)))
    BeginDelete(Of T)(criteria, callback, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' delete a business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to delete.
  ''' </typeparam>
  ''' <param name="criteria">
  ''' Criteria describing the object to delete.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>  
  Public Sub BeginDelete(Of T As IMobileObject)(ByVal criteria As Object, ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)
    Dim dp As DataPortal(Of T) = New DataPortal(Of T)
    AddHandler dp.DeleteCompleted, callback
    dp.BeginDelete(criteria, userState)
  End Sub

#End Region

#Region "Begin Execute"

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' execute a command object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of object to execute.
  ''' </typeparam>
  ''' <param name="obj">
  ''' Reference to the object to execute.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>  
  Public Sub BeginExecute(Of T As IMobileObject)(ByVal obj As T, ByVal callback As EventHandler(Of DataPortalResult(Of T)))
    BeginExecute(Of T)(obj, callback, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an asynchronous data portal operation to
  ''' execute a command object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of object to execute.
  ''' </typeparam>
  ''' <param name="obj">
  ''' Reference to the object to execute.
  ''' </param>
  ''' <param name="callback">
  ''' Reference to method that will handle the 
  ''' asynchronous callback when the operation
  ''' is complete.
  ''' </param>
  ''' <param name="userState">User state object.</param>  
  Public Sub BeginExecute(Of T As IMobileObject)(ByVal obj As T, ByVal callback As EventHandler(Of DataPortalResult(Of T)), ByVal userState As Object)
    Dim dp As DataPortal(Of T) = New DataPortal(Of T)()
    AddHandler dp.ExecuteCompleted, callback
    dp.BeginExecute(obj, userState)
  End Sub

#End Region

#End Region

#Region " Child Data Access methods "

  ''' <summary>
  ''' Creates and initializes a new
  ''' child business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to create.
  ''' </typeparam>
  ''' <param name="parameters">
  ''' Parameters passed to child create method.
  ''' </param>
  Public Function CreateChild(Of T)(ByVal ParamArray parameters() As Object) As T

    Dim portal As New Server.ChildDataPortal
    Return DirectCast(portal.Create(GetType(T), parameters), T)

  End Function

  ''' <summary>
  ''' Creates and loads an existing
  ''' child business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to retrieve.
  ''' </typeparam>
  ''' <param name="parameters">
  ''' Parameters passed to child fetch method.
  ''' </param>
  Public Function FetchChild(Of T)(ByVal ParamArray parameters() As Object) As T

    Dim portal As New Server.ChildDataPortal
    Return DirectCast(portal.Fetch(GetType(T), parameters), T)

  End Function

  ''' <summary>
  ''' Inserts, updates or deletes an existing
  ''' child business object.
  ''' </summary>
  ''' <param name="child">
  ''' Business object to update.
  ''' </param>
  ''' <param name="parameters">
  ''' Parameters passed to child update method.
  ''' </param>
  Public Sub UpdateChild(ByVal child As Object, ByVal ParamArray parameters() As Object)

    Dim portal As New Server.ChildDataPortal
    portal.Update(child, parameters)

  End Sub

#End Region

#Region " DataPortal Proxy "

  Private _proxyType As Type

  Private Function GetDataPortalProxy( _
    ByVal forceLocal As Boolean) As DataPortalClient.IDataPortalProxy

    If DataPortal.IsInDesignMode Then
      Return New DataPortalClient.DesignTimeProxy()
    Else
      If forceLocal Then
        Return New DataPortalClient.LocalProxy()
      Else
        If _proxyType Is Nothing Then

          Dim proxyTypeName As String = ApplicationContext.DataPortalProxy

          If proxyTypeName = "Local" Then
            _proxyType = GetType(DataPortalClient.LocalProxy)
          Else
           _proxyType = Type.GetType(proxyTypeName, true, true)
          End If
        End If
        Return CType(Activator.CreateInstance(_proxyType), DataPortalClient.IDataPortalProxy)
      End If
    End If
  End Function

  ''' <summary>
  ''' Resets the data portal proxy type, so the
  ''' next data portal call will reload the proxy
  ''' type based on current configuration values.
  ''' </summary>
  Public Sub ResetProxyType()
    _proxyType = Nothing
  End Sub

  ''' <summary>
  ''' Releases any remote data portal proxy object, so
  ''' the next data portal call will create a new
  ''' proxy instance.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  <Obsolete("Proxies no longer cached")> _
  Public Sub ReleaseProxy()

  End Sub

#End Region

#Region " Security "

  Private Function GetPrincipal() As System.Security.Principal.IPrincipal
    If ApplicationContext.AuthenticationType = "Windows" Then
      ' Windows integrated security 
      Return Nothing

    Else
      ' we assume using the CSLA framework security
      Return ApplicationContext.User
    End If
  End Function

#End Region

#Region " Helper methods "

  Private Function RunLocal(ByVal method As MethodInfo) As Boolean

    Return Attribute.IsDefined(method, GetType(RunLocalAttribute), False)

  End Function

#End Region

#Region "Design Time Support"

  Private _isInDesignMode As Boolean = False
  Private _isInDesignModeHasBeenSet As Boolean = False
  Private _designModeLock As Object = New Object()

  ''' <summary>
  ''' Gets a value indicating whether the code is running
  ''' in WPF design mode.
  ''' </summary>
  ''' <remarks></remarks>
  Public ReadOnly Property IsInDesignMode() As Boolean
    Get
      If Not _isInDesignModeHasBeenSet Then
        If Application.Current IsNot Nothing AndAlso Application.Current.Dispatcher IsNot Nothing Then
          Dim func As New Func(Of Boolean) _
            (Function() _
              CBool(IIf(Not Application.Current.MainWindow Is Nothing, _
              DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow), _
              False)))

          Dim params() As Object = Nothing
          Dim tmp As Boolean = CType(Application.Current.Dispatcher.Invoke(func, params), Boolean)

          SyncLock _designModeLock
            If Not _isInDesignModeHasBeenSet Then
              _isInDesignMode = tmp
              _isInDesignModeHasBeenSet = True
            End If
          End SyncLock
        End If
      End If

      Return _isInDesignMode
    End Get
  End Property

#End Region
End Module
