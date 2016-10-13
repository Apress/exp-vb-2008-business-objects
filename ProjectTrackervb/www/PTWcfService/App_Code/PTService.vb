﻿Imports System.Collections.Generic
Imports ProjectTracker.Library

Public Class PTService
  Implements IPTService

#Region "IPTService Members"

  Public Function GetProjectList() As ProjectData() Implements IPTService.GetProjectList

    ' TODO: comment out the following if using the
    ' PTWcfServiceAuth components to require a
    ' username/password from the caller
    ProjectTracker.Library.Security.PTPrincipal.Logout()

    Try
      Dim list As ProjectList = ProjectList.GetProjectList()
      Dim result As List(Of ProjectData) = New List(Of ProjectData)()
      For Each item As ProjectInfo In list
        Dim info As ProjectData = New ProjectData()
        Csla.Data.DataMapper.Map(item, info)
        result.Add(info)
      Next item
      Return result.ToArray()

    Catch ex As Csla.DataPortalException
      Throw ex.BusinessException

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

  End Function

#End Region

End Class
