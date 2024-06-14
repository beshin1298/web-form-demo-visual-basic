<%@ Page Title="About" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Product.aspx.vb" Inherits="web_form_visual_basic.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <div class="d-block p-1">
            <label for="name">Name: </label>
            <input type="text" id="txtName" required />
        </div>
        <div class="d-block p-1">
            <label for="quantity">Quantity: </label>
            <input type="number" id="txtQuantity" required min="1" />
        </div>
        <div class="d-block p-1">
            <label for="image">Chọn ảnh hoặc chụp ảnh mới:</label>
            <input type="file" id="image" accept="image/*" capture="camera" />
        </div>

        <div class="d-block p-1">
            <button id="submit" onClick="submitData()">Gửi Dữ Liệu</button>
        </div>
    </div>
    <script>
        async function submitData() {
            console.log("hello")
            let name = document.getElementById("txtName").value;
            let quantity = document.getElementById("txtQuantity").value;
            let imageInput = document.getElementById("image");
            let file = imageInput.files[0];

            if (!name || !quantity || !file) {
                alert('Vui lòng nhập đủ thông tin và chọn hoặc chụp ảnh.');
                return;
            }

            const formData = new FormData();
            formData.append('name', name);
            formData.append('quanlity', quantity);
            formData.append('image', file);
            console.log(file)
            try {
                const apiResponse = await fetch('https://localhost:44328/api/product', {
                    method: 'POST',
                    body: formData
                });

                if (apiResponse.ok) {
                    alert('Product added successfully.');
                } else {
                    const errorData = await apiResponse.json();
                    console.log('Error when adding product: ' + errorData.message);
                }
            } catch (error) {
                console.error('Error when calling API:', error);
                console.log('Error when calling API.');
            }
        }

    </script>
</asp:Content>

