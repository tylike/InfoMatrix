<%@ Control Language="C#" CodeBehind="NestedFrameControl1.ascx.cs" ClassName="NestedFrameControl1" Inherits="Admiral.ERP.Web.NestedFrameControl1"%>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<div class="NestedFrame">
    <cc3:XafUpdatePanel ID="UPToolBar" CssClass="ToolBarUpdatePanel" runat="server">
        <cc2:ActionContainerHolder runat="server" ID="ToolBar" CssClass="ToolBar" ContainerStyle="ToolBar" Orientation="Horizontal" Menu-Width="100%" Menu-ItemAutoWidth="False" >
            <ActionContainers>
                <cc2:WebActionContainer ContainerId="ObjectsCreation" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="Link" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="Edit" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="RecordEdit" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="View" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="Reports" IsDropDown="false" PaintStyle="CaptionAndImage"/> 
                <cc2:WebActionContainer ContainerId="Export" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="Diagnostic" IsDropDown="false" PaintStyle="CaptionAndImage"/>
                <cc2:WebActionContainer ContainerId="Filters" IsDropDown="false" PaintStyle="CaptionAndImage"/>                                                            
             </ActionContainers>
         </cc2:ActionContainerHolder>
    </cc3:XafUpdatePanel>
    <cc4:ViewSiteControl ID="viewSiteControl" runat="server" Control-CssClass="NestedFrameViewSite" />

</div>
