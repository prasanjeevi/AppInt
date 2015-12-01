<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Restinfinity.Net.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Api Manager</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Service:<asp:DropDownList runat="server" ID="ddlService" AutoPostBack="true" OnSelectedIndexChanged="ddlService_SelectedIndexChanged" />
    </div>
        <div>
             Methods:<asp:DropDownList runat="server" ID="ddlMethod" OnSelectedIndexChanged="ddlMethod_SelectedIndexChanged"/>
        </div>
        <div runat="server" id="divResult">

        </div>
    </form>
</body>
</html>
