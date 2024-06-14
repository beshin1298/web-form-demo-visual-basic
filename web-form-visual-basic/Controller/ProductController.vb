Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Http



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
    ' POST api/<controller>
    <HttpPost>
    Public Function PostProduct() As IHttpActionResult
        Dim httpRequest = HttpContext.Current.Request

        If httpRequest.Files.Count = 0 OrElse String.IsNullOrEmpty(httpRequest.Form("name")) OrElse String.IsNullOrEmpty(httpRequest.Form("quanlity")) Then
            Return BadRequest("Invalid product data or image.")
        End If

        Dim name = httpRequest.Form("name")
        Dim quanlity = Integer.Parse(httpRequest.Form("quanlity"))
        Dim postedFile = httpRequest.Files("image")

        ' Lưu ảnh vào thư mục
        Dim imagePath As String = SaveImage(postedFile)

        If String.IsNullOrEmpty(imagePath) Then
            Return InternalServerError(New Exception("Failed to save image."))
        End If

        Dim connectionString As String = ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO [product] (name, quanlity, image) VALUES (@name, @quanlity, @image)"
            Using command As New SqlCommand(query, connection)
                command.Parameters.AddWithValue("@name", name)
                command.Parameters.AddWithValue("@quanlity", quanlity)
                command.Parameters.AddWithValue("@image", imagePath)
                connection.Open()
                command.ExecuteNonQuery()
            End Using
        End Using

        Return Ok("Product added successfully.")
    End Function

    Private Function SaveImage(file As HttpPostedFile) As String
        Try
            Dim fileName = Path.GetFileName(file.FileName)
            Dim filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/"), fileName)
            file.SaveAs(filePath)
            Return "/Images/" & fileName
        Catch ex As Exception
            Return Nothing
        End Try
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

    Public Property Image As HttpPostedFileBase


    Public Property ImageUrl As String
End Class
