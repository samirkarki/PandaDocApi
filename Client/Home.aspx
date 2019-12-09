<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Client.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="gv_documents" runat="server" AutoGenerateColumns="true"></asp:GridView>
    <asp:Button ID="btn_upload" OnClick="btn_upload_Click" runat="server" Text="Upload Document" />
        <asp:TextBox ID="txt_documentid" runat="server" placeholder="Document Id"></asp:TextBox>
        <asp:TextBox ID="txt_email" runat="server" placeholder="Email"></asp:TextBox>
        <asp:Button ID="btn_share" OnClick="btn_share_Click" runat="server" Text="Share doc" />
    </form>
    <iframe id="iframe_doc" runat="server" width="100%" height="100%" src="https://app.pandadoc.com/s/XBqnkxBKtZUsQz2WECWTg6"></iframe>
</body>
</html>
