<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="web_form_visual_basic._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Label ID="lblSearch" runat="server" Text="Search by product name: ">

        </asp:Label><asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
        <div>
            <asp:Button ID="btnSearch" runat="server" Text="Search" />
        </div>
    </div>
    <div class="d-flex flex-row">
        <div class="p-2">
            <asp:SqlDataSource ID="SqlProductTable" runat="server" ConnectionString="<%$ ConnectionStrings:database_demoConnectionString %>" ProviderName="<%$ ConnectionStrings:database_demoConnectionString.ProviderName %>" SelectCommand="SELECT * FROM [product]"></asp:SqlDataSource>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" DataKeyNames="id" DataSourceID="SqlProductTable" GridLines="None" Height="232px" Width="933px">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="id" InsertVisible="False" ReadOnly="True" SortExpression="id" />
                    <asp:BoundField DataField="name" HeaderText="name" SortExpression="name" />
                    <asp:BoundField DataField="quanlity" HeaderText="quanlity" SortExpression="quanlity" />
                </Columns>
                <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />
                <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                <RowStyle BackColor="#DEDFDE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#594B9C" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#33276A" />
            </asp:GridView>
        </div>
        <div class="d-block p-1">
            <h1>Add new product</h1>
            <div class="p-3">
                <asp:Label ID="lblProductName" runat="server" Text="Name: ">
                </asp:Label><asp:TextBox ID="txtProductNameAddNew" runat="server"></asp:TextBox>
            </div>
            <div class="p-3">
                <asp:Label ID="lblQuantity" runat="server" Text="Quantity: ">
                </asp:Label><asp:TextBox ID="txtQuantity" type="number" runat="server"></asp:TextBox>
            </div>
            <div class="p-3">
                <asp:Button ID="btnAddNew" runat="server" Text="Add New Product" />
            </div>

        </div>
    </div>

</asp:Content>
