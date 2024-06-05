Imports System.Data.SqlClient

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Not String.IsNullOrEmpty(Request.QueryString("ProductName")) Then
                Dim productNameSearch As String = Request.QueryString("ProductName")
                txtSearch.Text = productNameSearch
                ' Update the parameter for the SqlDataSource
                ' Update the SQL query with parameterized query
                SqlProductTable.SelectCommand = "SELECT * FROM [product] WHERE [name] LIKE @ProductName"

                ' Clear any existing parameters and add the new one
                SqlProductTable.SelectParameters.Clear()
                SqlProductTable.SelectParameters.Add("ProductName", "%" & productNameSearch & "%")

                ' Re-bind the GridView
                GridView1.DataBind()
            End If
        End If
    End Sub


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        'Dim productNameSearch As String = txtSearch.Text
        '' Redirect to the same page with the search query as a parameter
        'Response.Redirect("Default.aspx?ProductName=" & Server.UrlEncode(productNameSearch))
        Debug.Print(GetProducts())

    End Sub

    Protected Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        Dim productName As String = txtProductNameAddNew.Text
        Dim quantity As Integer = txtQuantity.Text

        If Integer.TryParse(txtQuantity.Text, quantity) Then
            ' Insert new product into the database
            Dim connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("database_demoConnectionString").ConnectionString
            Using connection As New SqlConnection(connectionString)
                Dim query As String = "INSERT INTO [product] (name, quanlity) VALUES (@name, @quanlity)"
                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@name", productName)
                    command.Parameters.AddWithValue("@quanlity", quantity)
                    connection.Open()
                    command.ExecuteNonQuery()
                End Using
            End Using

            ' Clear the textboxes
            txtProductNameAddNew.Text = ""
            txtQuantity.Text = ""

            ' Re-bind the GridView to show the new product
            GridView1.DataBind()
        Else

        End If

    End Sub
    <System.Web.Services.WebMethod()>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Shared Function GetProducts() As String

    End Function


End Class