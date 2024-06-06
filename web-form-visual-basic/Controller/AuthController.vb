Imports System.Data.SqlClient
Imports System.IdentityModel.Tokens.Jwt
Imports System.Security.Claims
Imports System.Security.Cryptography
Imports System.Web.Http
Imports Microsoft.IdentityModel.Tokens

Public Class AuthController
    Inherits ApiController
    Dim keyString As String = "Beshin1298@"
    ' POST api/auth/login
    <HttpPost>
    <Route("api/auth/login")>
    Public Function Login(<FromBody> loginModel As LoginModel) As IHttpActionResult
        If loginModel Is Nothing OrElse String.IsNullOrEmpty(loginModel.Username) OrElse String.IsNullOrEmpty(loginModel.Password) Then
            Return BadRequest("Invalid login information.")
        End If

        Dim hashedPassword As String = PasswordHasher.HashPassword(loginModel.Password)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString
        Dim userId As Integer? = Nothing
        Dim userRole As String = Nothing


        Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(keyString)

        ' Đảm bảo khóa có đủ kích thước (256 bit)
        If keyBytes.Length < 32 Then
            Dim paddedBytes As Byte() = New Byte(31) {}
            Buffer.BlockCopy(keyBytes, 0, paddedBytes, 0, keyBytes.Length)
            keyBytes = paddedBytes
        End If

        Dim signingKey As New SymmetricSecurityKey(keyBytes)




        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT ID, role FROM [users] WHERE username = @username AND password = @password"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@username", loginModel.Username)
                command.Parameters.AddWithValue("@password", hashedPassword)

                connection.Open()
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader.Read() Then
                    userId = reader.GetInt32(0)
                    userRole = If(Not reader.IsDBNull(1), reader.GetString(1), "user")
                End If
            End Using
        End Using

        If userId Is Nothing Then
            Return Unauthorized()
        End If

        Dim tokenHandler As New JwtSecurityTokenHandler()
        Dim key As Byte() = Encoding.UTF8.GetBytes("your_secret_key_here")
        Dim tokenDescriptor As New SecurityTokenDescriptor() With {
            .Subject = New ClaimsIdentity(New Claim() {
                New Claim(ClaimTypes.Name, loginModel.Username),
                New Claim(ClaimTypes.Role, userRole)
            }),
            .Expires = DateTime.UtcNow.AddHours(1),
            .SigningCredentials = New SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
        }
        Dim token As SecurityToken = tokenHandler.CreateToken(tokenDescriptor)
        Dim tokenString As String = tokenHandler.WriteToken(token)
        Dim newTokenString As String
        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT token, expiration FROM [tokens] WHERE user_id = @user_id AND expiration > @current_time"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@user_id", userId)
                command.Parameters.AddWithValue("@current_time", DateTime.UtcNow)

                connection.Open()
                Dim reader As SqlDataReader = command.ExecuteReader()
                If reader.Read() Then

                    Dim existingToken As String = reader.GetString(0)
                    Dim expiration As DateTime = reader.GetDateTime(1)

                    Dim newExpiration As DateTime = DateTime.UtcNow.AddHours(1)
                    UpdateTokenExpiration(existingToken, newExpiration)
                    Return Ok(New With {.token = existingToken, .role = userRole})
                Else
                    newTokenString = GenerateNewToken(userId, userRole)
                    Return Ok(New With {.token = newTokenString, .role = userRole})
                End If
            End Using
        End Using
    End Function

    ' POST api/auth/register
    <HttpPost>
    <Route("api/auth/register")>
    Public Function Register(<FromBody> registerModel As RegisterModel) As IHttpActionResult
        If registerModel Is Nothing OrElse String.IsNullOrEmpty(registerModel.Username) OrElse String.IsNullOrEmpty(registerModel.Password) Then
            Return BadRequest("Invalid registration information.")
        End If

        Dim hashedPassword As String = PasswordHasher.HashPassword(registerModel.Password)
        Dim defaultRole As String = "user"

        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO [users] (username, pass, role) VALUES (@username, @password, @role)"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@username", registerModel.Username)
                command.Parameters.AddWithValue("@password", hashedPassword)
                command.Parameters.AddWithValue("@role", defaultRole)

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using

        Return Ok("Registration successful.")
    End Function
    Private Sub UpdateTokenExpiration(existingToken As String, newExpiration As DateTime)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "UPDATE [tokens] SET expiration = @expiration WHERE token = @token"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@expiration", newExpiration)
                command.Parameters.AddWithValue("@token", existingToken)

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Private Function GenerateNewToken(userId As Integer, userRole As String) As String


        Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(keyString)

        If keyBytes.Length < 32 Then
            Dim paddedBytes As Byte() = New Byte(31) {}
            Buffer.BlockCopy(keyBytes, 0, paddedBytes, 0, keyBytes.Length)
            keyBytes = paddedBytes
        End If

        Dim signingKey As New SymmetricSecurityKey(keyBytes)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString
        Dim tokenHandler As New JwtSecurityTokenHandler()
        Dim tokenDescriptor As New SecurityTokenDescriptor() With {
        .Subject = New ClaimsIdentity(New Claim() {
            New Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            New Claim(ClaimTypes.Role, userRole)
        }),
        .Expires = DateTime.UtcNow.AddHours(1),
        .SigningCredentials = New SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
    }
        Dim token As SecurityToken = tokenHandler.CreateToken(tokenDescriptor)
        Dim tokenString As String = tokenHandler.WriteToken(token)
        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO [tokens] (user_id, token, expiration) VALUES (@user_id, @token, @expiration)"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@user_id", userId)
                command.Parameters.AddWithValue("@token", tokenString)
                command.Parameters.AddWithValue("@expiration", DateTime.UtcNow.AddHours(12))

                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using
        Return tokenString
    End Function

End Class

Public Class LoginModel
    Public Property Username As String
    Public Property Password As String
End Class

Public Class RegisterModel
    Public Property Username As String
    Public Property Password As String
End Class

Public Class PasswordHasher
    Public Shared Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim hashedBytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
            Return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower()
        End Using
    End Function
End Class
