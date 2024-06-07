Imports System.Data.SqlClient
Imports System.Web.Http


Namespace Controller
    <JwtAuthenticationAttribute>
    <JwtRolesAttribute("Admin")>
    Public Class CategoryController
        Inherits ApiController

        <HttpGet>
        Function GetValues() As IHttpActionResult
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString
            Dim categories As New List(Of Object)()

            Using connection As New SqlConnection(connectionString)
                Dim query As String = "SELECT * FROM [categories]"

                Using command As New SqlCommand(query, connection)
                    connection.Open()
                    Using reader As SqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            Dim product As New Dictionary(Of String, Object)()
                            For i As Integer = 0 To reader.FieldCount - 1
                                product.Add(reader.GetName(i), reader(i))
                            Next
                            categories.Add(product)
                        End While
                    End Using
                End Using
            End Using

            Return Json(categories)
        End Function
    End Class
End Namespace