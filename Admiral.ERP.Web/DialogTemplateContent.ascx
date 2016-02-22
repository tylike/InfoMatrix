<%@ Control Language="C#" CodeBehind="DialogTemplateContent.ascx.cs" ClassName="DialogTemplateContent" Inherits="Admiral.ERP.Web.DialogTemplateContent"%>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<style>
    .DialogContent .ContentCell {
        vertical-align: top;
        padding: 5px 10px 5px 10px;
    }
</style>
<div class="Dialog" id="DialogContent">
     <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
        <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
    </cc3:XafUpdatePanel>
    <table Width="100%" height="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td valign="top" height="100%">
                <cc3:XafUpdatePanel ID="UPVH" runat="server" UpdatePanelForASPxGridListCallback="False">
                    <div style="display: none">
                        <cc4:ViewImageControl ID="VIC" runat="server" Control-UseLargeImage="false" />
                        <cc4:ViewCaptionControl ID="VCC" runat="server" DetailViewCaptionMode="ViewAndObjectCaption" />
                    </div>
                </cc3:XafUpdatePanel>
                <table class="DialogContent Content" border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="ContentCell">
                            <cc3:XafUpdatePanel ID="UPEI" runat="server" UpdatePanelForASPxGridListCallback="False">
                                <tc:ErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server" />
                            </cc3:XafUpdatePanel>
                            <asp:Table ID="Table1" runat="server" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="0">
                                <asp:TableRow ID="TableRow5" runat="server">
                                    <asp:TableCell runat="server" ID="TableCell1" HorizontalAlign="Center">
                                        <cc3:XafUpdatePanel ID="UPSAC" runat="server">
                                            <table border="0" cellpadding="0" cellspacing="0" width="100%" >
                                                <tr>
                                                    <td align="left" valign="top">
                                                        <cc2:ActionContainerHolder ID="OCC" runat="server" ContainerStyle="Buttons" Orientation="Horizontal" style="float: left" >
                                                            <ActionContainers>
                                                                <cc2:WebActionContainer ContainerId="ObjectsCreation" IsDropDown="false" PaintStyle="CaptionAndImage" />
                                                            </ActionContainers>
                                                        </cc2:ActionContainerHolder>
                                                    </td>
                                                    <td align="right" valign="top">
                                                        <cc2:ActionContainerHolder ID="SAC" runat="server" CssClass="HContainer" Orientation="Horizontal" ContainerStyle="Buttons" >
                                                            <ActionContainers>
                                                                <cc2:WebActionContainer ContainerId="Search" IsDropDown="false" />
                                                                <cc2:WebActionContainer ContainerId="FullTextSearch" IsDropDown="false" />
                                                            </ActionContainers>
                                                        </cc2:ActionContainerHolder>
                                                    </td>
                                                </tr>
                                            </table>
                                        </cc3:XafUpdatePanel>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow ID="TableRow2" runat="server">
                                    <asp:TableCell runat="server" ID="ViewSite">
                                        <cc3:XafUpdatePanel ID="UPVSC" runat="server" UpdatePanelForASPxGridListCallback="False">
                                            <cc4:ViewSiteControl ID="VSC" runat="server" />
                                        </cc3:XafUpdatePanel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </td>
                    </tr>
                </table>
                <table class="DockBottom" border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <cc3:XafUpdatePanel ID="UPPAC" runat="server">
                                <cc2:ActionContainerHolder runat="server" ID="PAC" ContainerStyle="Buttons" Orientation="Horizontal" >
                                    <menu width="100%" itemautowidth="False" horizontalalign="Right" />
                                    <ActionContainers>
                                        <cc2:WebActionContainer ContainerId="PopupActions" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                                        <cc2:WebActionContainer ContainerId="Diagnostic" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                                    </ActionContainers>
                                </cc2:ActionContainerHolder>
                            </cc3:XafUpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
