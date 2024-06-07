Imports System.Data.SqlClient
Imports System.Web.Http


<JwtAuthenticationAttribute>
Public Class ProductController
    Inherits ApiController

    ' GET api/<controller>

    Public Function GetValues() As IHttpActionResult
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString
        Dim products As New List(Of Object)()

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM [product]"

            Using command As New SqlCommand(query, connection)
                connection.Open()
                Using reader As SqlDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim product As New Dictionary(Of String, Object)()
                        For i As Integer = 0 To reader.FieldCount - 1
                            product.Add(reader.GetName(i), reader(i))
                        Next
                        products.Add(product)
                    End While
                End Using
            End Using
        End Using

        Return Json(products)
    End Function

    ' GET api/<controller>/5
    Public Function GetValue(ByVal id As Integer) As String
        Return "value"
    End Function

    ' POST api/<controller>
    Public Function PostProduct(<FromBody> product As Product) As IHttpActionResult
        If product Is Nothing Then
            Return BadRequest("Invalid product data.")
        End If

        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO [product] (name, quanlity) VALUES (@name, @quanlity)"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@name", product.Name)
                command.Parameters.AddWithValue("@quanlity", product.Quanlity)
                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using

        Return Ok("Product added successfully.")
    End Function

    ' PUT api/<controller>/5
    Public Sub PutValue(ByVal id As Integer, <FromBody()> ByVal value As String)

    End Sub

    ' DELETE api/<controller>/5
    Public Sub DeleteValue(ByVal id As Integer)

    End Sub
End Class
Public Class Product
    Public Property Name As String
    Public Property Quanlity As Integer
End Class
