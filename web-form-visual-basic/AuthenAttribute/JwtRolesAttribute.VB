Imports System.Security.Claims

Public Class JwtRolesAttribute
    Inherits JwtAuthenticationBaseAttribute

    Private ReadOnly _roles As String()

    Public Sub New(ParamArray roles As String())
        _roles = roles
    End Sub

    Protected Overrides Function CheckRoles(claimsPrincipal As ClaimsPrincipal, rolesFromDatabase As List(Of String)) As Boolean
        Return rolesFromDatabase.Any(Function(r) _roles.Contains(r))
    End Function
End Class
