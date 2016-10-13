﻿Imports System.Linq.Expressions

Namespace Core
  Friend Class ExpressionTreeHelpers
    Friend Shared Function IsMemberEqualsValueExpression(ByVal exp As Expression, ByVal declaringType As Type, ByVal memberName As String) As Boolean
      If exp.NodeType <> ExpressionType.Equal Then
        Return False
      End If

      Dim be As BinaryExpression = CType(exp, BinaryExpression)

      ' Assert.
      If ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) AndAlso ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName) Then
        Throw New Exception("Cannot have 'member' == 'member' in an expression!")
      End If

      Return (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) OrElse ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName))
    End Function

    Friend Shared Function IsSpecificMemberExpression(ByVal exp As Expression, ByVal declaringType As Type, ByVal memberName As String) As Boolean
      Return ((TypeOf exp Is MemberExpression) AndAlso ((CType(exp, MemberExpression)).Member.DeclaringType Is declaringType) AndAlso ((CType(exp, MemberExpression)).Member.Name = memberName))
    End Function

    Friend Shared Function GetValueFromEqualsExpression(ByVal be As BinaryExpression, ByVal memberDeclaringType As Type, ByVal memberName As String) As String
      If be.NodeType <> ExpressionType.Equal Then
        Throw New Exception("There is a bug in this program.")
      End If

      If be.Left.NodeType = ExpressionType.MemberAccess Then
        Dim [me] As MemberExpression = CType(be.Left, MemberExpression)

        If [me].Member.DeclaringType Is memberDeclaringType AndAlso [me].Member.Name = memberName Then
          Return GetValueFromExpression(be.Right)
        End If
      ElseIf be.Right.NodeType = ExpressionType.MemberAccess Then
        Dim [me] As MemberExpression = CType(be.Right, MemberExpression)

        If [me].Member.DeclaringType Is memberDeclaringType AndAlso [me].Member.Name = memberName Then
          Return GetValueFromExpression(be.Left)
        End If
      End If

      ' We should have returned by now.
      Throw New Exception("There is a bug in this program.")
    End Function

    Friend Shared Function GetValueFromExpression(ByVal expression As Expression) As String
      If expression.NodeType = ExpressionType.Constant Then
        Return CStr((CType(expression, ConstantExpression)).Value)
      Else
        Throw New InvalidQueryException(String.Format("The expression type {0} is not supported to obtain a value.", expression.NodeType))
      End If
    End Function
  End Class
End Namespace