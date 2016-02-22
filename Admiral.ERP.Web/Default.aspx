<%@ Page Language="C#" AutoEventWireup="true" Inherits="Default" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Default.aspx.cs" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.ExpressApp.Web.Templates" TagPrefix="cc3" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Main Page</title>
    <meta http-equiv="Expires" content="0" />
    <script src="Scripts/jquery-2.1.4.js"></script>
    <script src="Scripts/FlowChart.js"></script>
    <script>
        function InitPopupMenuHandler(s, e) {
            var caption = $(".Caption");//document.getElementById('gridCell');
            caption.each(function(idx, el) {
                ASPxClientUtils.AttachEventToElement(el, 'contextmenu', OnContextMenu);
            });
            //caption.click(function(s, e) {
            //    OnGridContextMenu(e);
            //});
            //var imgButton = document.getElementById('ImgButton1');
            //ASPxClientUtils.AttachEventToElement(imgButton, 'contextmenu', OnPreventContextMenu);
        }
        function OnContextMenu(evt, actionID, viewItemID,showHelp,showEditorSetup) {
            PropertyEditorMenu.XafActionID = actionID;
            PropertyEditorMenu.ViewItemID = viewItemID;

            PropertyEditorMenu.GetItem(0).SetVisible(showEditorSetup);
            PropertyEditorMenu.GetItem(1).SetVisible(showHelp);


            PropertyEditorMenu.ShowAtPos(evt.clientX + ASPxClientUtils.GetDocumentScrollLeft(), evt.clientY + ASPxClientUtils.GetDocumentScrollTop());
            
            return OnPreventContextMenu(evt);
        }
        function OnPreventContextMenu(evt) {
            return ASPxClientUtils.PreventEventAndBubble(evt);
        }
        
    </script>
    <style>
        .NestedFrameControl > .NestedFrame > .NestedFrameViewSite > .ListViewItem {
            padding: 0px !important;
        }

        .Layout {
            padding-left: 0px !important;
        }
    </style>
</head>
<body class="VerticalTemplate">
    <form id="form2" runat="server">
        <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server" UpdatePanelForASPxGridListCallback="False">
            <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
        </cc3:XafUpdatePanel>
        <cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
        <div runat="server" id="Content" />
        <dx:ASPxPopupMenu ID="PropertyEditorMenu" runat="server" ClientInstanceName="PropertyEditorMenu"
             ShowPopOutImages="True"
            PopupHorizontalAlign="OutsideRight" PopupVerticalAlign="TopSides" PopupAction="LeftMouseClick">
            <ClientSideEvents ItemClick="function(s, e) {
                debugger;
                
                RaiseXafCallback(globalCallbackControl, 'PopupWindowHandler', '|false||Default.aspx?para='+s.ViewItemID+'&MenuItem='+e.item.name+'&Dialog=true&ActionID='+s.XafActionID+'|true||false', '', false);
                
}" />
            <Items>
                <dx:MenuItem Text="编辑器设置" Name="EditorSetup">
                </dx:MenuItem>
                <dx:MenuItem Text="帮助" Name="Help">
                </dx:MenuItem>
            </Items>
   <%--         <ClientSideEvents Init="InitPopupMenuHandler" />--%>
            <ItemStyle Width="143px"></ItemStyle>
        </dx:ASPxPopupMenu>

    </form>
</body>
</html>
