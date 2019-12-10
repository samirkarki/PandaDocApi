<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Client.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server"> 
        <asp:GridView ID="gv_documents" runat="server" DataKeyNames="Id" AutoGenerateColumns="false" HeaderStyle-BackColor="#3AC0F2" HeaderStyle-ForeColor="White" OnSelectedIndexChanged = "OnSelectedIndexChanged" AutoGenerateSelectButton="True">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" />
                <asp:BoundField DataField="Name" HeaderText="Doc Name"/>
                <asp:BoundField DataField="DocumentStatus" HeaderText="Status"/>
                <asp:BoundField DataField="DateCreated" HeaderText="Created on"/>
                <asp:BoundField DataField="DateModified" HeaderText="Modified on"/>
            </Columns>
        </asp:GridView>
        <br />
        <br />

        <asp:Button ID="btn_upload" OnClick="btn_upload_Click" runat="server" Text="Upload New Document" />
        <br />
        <br />
        <asp:TextBox ID="txt_documentid" runat="server" placeholder="Document Id"></asp:TextBox>
        <asp:TextBox ID="txt_email" runat="server" placeholder="Email"></asp:TextBox>
        <asp:Button ID="btn_share" OnClick="btn_share_Click" runat="server" Text="Share doc" />
    </form>
    <iframe id="iframe_doc" runat="server" width="100%" height="900px" src="https://app.pandadoc.com/s/XBqnkxBKtZUsQz2WECWTg6"></iframe>
</body>
</html>
