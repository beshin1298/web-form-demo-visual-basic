Imports System.Data.SqlClient
Imports System.IdentityModel.Tokens.Jwt
Imports System.Net.Http.Headers
Imports System.Security.Claims
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web.Http.Filters
Imports System.Web.Http.Results
Imports Microsoft.IdentityModel.Tokens

Public Class JwtAuthenticationAttribute
    Inherits Attribute
    Implements IAuthenticationFilter

    Public Async Function AuthenticateAsync(context As HttpAuthenticationContext, cancellationToken As CancellationToken) As Task Implements IAuthenticationFilter.AuthenticateAsync
        Dim authHeader As AuthenticationHeaderValue = context.Request.Headers.Authorization

        If authHeader Is Nothing OrElse authHeader.Scheme.ToLower() <> "bearer" Then
            context.ErrorResult = New UnauthorizedResult(New AuthenticationHeaderValue() {New AuthenticationHeaderValue("Bearer")}, context.Request)
            Return
        End If

        Dim tokenString As String = authHeader.Parameter.Replace("Bearer ", String.Empty)

        Dim tokenFromDatabase As String = Await GetTokenFromDatabaseAsync(tokenString)

        If String.IsNullOrEmpty(tokenFromDatabase) Then
            context.ErrorResult = New UnauthorizedResult(New AuthenticationHeaderValue() {New AuthenticationHeaderValue("Bearer")}, context.Request)
            Return
        End If


        Dim tokenHandler As New JwtSecurityTokenHandler()
        Dim keyString As String = "Beshin1298@"
        Dim key As Byte() = Encoding.UTF8.GetBytes(keyString)

        Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(keyString)

        If keyBytes.Length < 32 Then
            Dim paddedBytes As Byte() = New Byte(31) {}
            Buffer.BlockCopy(keyBytes, 0, paddedBytes, 0, keyBytes.Length)
            keyBytes = paddedBytes
        End If

        Dim signingKey As New SymmetricSecurityKey(keyBytes)

        Dim validationParameters As New TokenValidationParameters() With {
            .ValidateIssuer = False,
            .ValidateAudience = False,
            .IssuerSigningKey = signingKey
        }

        Try
            Dim claimsPrincipal As ClaimsPrincipal = tokenHandler.ValidateToken(tokenString, validationParameters, Nothing)
            context.Principal = claimsPrincipal
        Catch ex As SecurityTokenValidationException
            context.ErrorResult = New UnauthorizedResult(New AuthenticationHeaderValue() {New AuthenticationHeaderValue("Bearer")}, context.Request)
        End Try
    End Function

    Public Function ChallengeAsync(context As HttpAuthenticationChallengeContext, cancellationToken As CancellationToken) As Task Implements IAuthenticationFilter.ChallengeAsync
        Return Task.FromResult(0)
    End Function

    Public ReadOnly Property AllowMultiple As Boolean Implements IFilter.AllowMultiple
        Get
            Return False
        End Get
    End Property

    Private Async Function GetTokenFromDatabaseAsync(token As String) As Task(Of String)

        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString
        Dim resultToken As String = Nothing

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT token FROM [tokens] WHERE token = @token"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@token", token)

                Await connection.OpenAsync()
                Using reader As SqlDataReader = Await command.ExecuteReaderAsync()
                    If reader.Read() Then
                        resultToken = reader.GetString(0)
                    End If
                End Using
            End Using
        End Using

        Return resultToken
    End Function
End Class
