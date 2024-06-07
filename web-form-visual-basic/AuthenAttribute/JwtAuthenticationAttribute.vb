Imports System.Security.Claims

Public Class JwtAuthenticationAttribute
    Inherits JwtAuthenticationBaseAttribute

    Protected Overrides Function CheckRoles(claimsPrincipal As ClaimsPrincipal, rolesFromDatabase As List(Of String)) As Boolean

        Return True
    End Function
End Class