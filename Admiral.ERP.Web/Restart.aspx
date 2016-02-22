<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Restart.aspx.cs" Inherits="Admiral.ERP.Web.Restart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="Scripts/jquery-2.1.4.js"></script>
    <script>
        $(document).ready(function() {
            $.ajax({
                url:'/Restart.ashx',
                success: function (d) {
                    window.location.href = '/';
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        正在重新启动系统....</div>
    </form>
    
</body>
</html>
