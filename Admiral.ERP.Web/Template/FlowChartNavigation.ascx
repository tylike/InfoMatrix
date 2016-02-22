<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FlowChartNavigation.ascx.cs" Inherits="Admiral.ERP.Web.Template.FlowChartNavigation" %>
<%@ Register assembly="DevExpress.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dxm" %>

<style type="text/css">
       .linkMenu 
       {
           background: none!important;
           border: 0!important; 
           /*color: #162436!important;*/
           padding: 0!important;
           text-decoration: none!important;
       }
       .linkMenu a:hover,
       .linkMenu a:hover *
       {
           text-decoration: underline!important;
       }
       .linkMenuItem,
       .linkMenuItem > div
       {
           padding: 0!important;
           font: 11px Tahoma!important;
       }
       .linkMenuSeparator
       {
           padding: 0 14px!important;
       }
       .linkMenuSeparator > *
       {
           background: #5386CB!important;
           margin: 4px 0!important;
           height: 10px!important;
           width: 1px!important;
       }
    </style>
<script src="/vis/vis.js"></script>
<link rel="stylesheet" href="/vis/vis.css" />
<style>
    .waps {
        width: 1004px;
        text-align: center;
        line-height: 28px;
        font-size: 12px;
        font-family: Arial, Helvetica, sans-serif;
    }

    .con {
        text-align: left;
        width: 500px;
        height: 400px;
        margin: 0px auto;
        /*padding: 100px 50px;*/
        background: rgba(255, 255, 255, 0.6) none repeat scroll 0 0 !important; /*实现FF背景透明，文字不透明*/
        filter: Alpha(opacity=80);
        background: #fff; /*实现IE背景透明*/
    }


        .con p {
            position: relative;
        }/*实现IE文字不透明*/
</style>
<script>
    <%if(Menu!=null){%>
    var drawed = false;
    var network = null;
    function draw(s) {
        var panel = document.getElementById('flowPanel');
        var container = document.getElementById('systemFlowChart');
        var p = $(panel);
        p.show();
        var ct = $(container);
        ct.show();

        //debugger;
        var w = $(window).width() - 2;
        p.width(w);
        ct.width(w);

        //if (panel.style.display == 'none') {
        //    debugger;
        //    panel.style.display = 'block';
        //    container.style.display = 'block';
        //    panel.offsetWidth = screen.width - 20;
        //    container.offsetWidth = panel.offsetWidth;

        //}
        if (!drawed) {

            drawed = true;

            var len = undefined;

            var nodes = [<%=string.Join(",", Menu.FlowChartSettings.Nodes.Select(x => x.GetJSON()))%>];
            var edges = [<%=string.Join(",",Menu.FlowChartSettings.Edges.Select(x=>x.GetJSON()))%>];

            // create a network
            var data = {
                nodes: nodes,
                edges: edges
            };
            var options = {
                interaction: {
                    navigationButtons: true,
                    keyboard: true
                },
                edges: { width: 2, arrows: 'to', smooth: false },
                nodes: {
                    size:40,borderWidthSelected:5,
                    shadow: {
                        enabled: false,
                        size: 50,
                        x: 5,
                        y: 5
                    },
                    shape: 'image',
                    image: '/image/采购退货单.png'
                }
            };

            network = new vis.Network(container, data, options);
            network.on('click', function (e) {
                if (e.nodes.length > 0) {
                    debugger;
                    <%=callbackManager.GetScript("ShowViewWindowController", "e.nodes[0]")%>
                }
            });
        }
        //debugger;
        setTimeout(function () {
            network.focus(s.item.name, { scale: 1, animation: true });
            network.selectNodes([s.item.name]);
        }, 100);
    }
    function FlowPanelMouseOut(p) {
        p.style.display = 'none'
        document.getElementById('flowPanel').style.display = 'none';
    }
    <%}%>
</script> 
<dxm:ASPxPanel ID="ASPxPanel1" runat="server" Width="200px"></dxm:ASPxPanel>
<dxm:ASPxMenu ID="ASPxMenu1" SeparatorHeight="0px" runat="server" EnableSubMenuFullWidth="True" ItemLinkMode="TextAndImage"  CssClass="ACH MainToolbar linkMenu">
                                            <ClientSideEvents Init="function(s, e) {
	}" PopUp="function(s, e) {    

}" ItemMouseOver="function(s, e) {
	draw(e);
}" />
            <Items>
                <dxm:MenuItem Text="开始" Name="Start">
                    <Items>
                        <dxm:MenuItem ClientVisible="False">
                        </dxm:MenuItem>
                    </Items>

                    <Image Height="16px" Url="~/Image/采购退货通知单.png" Width="16px">
                    </Image>

                </dxm:MenuItem>
                <dxm:MenuItem Text="采购" NavigateUrl="PMS">
                </dxm:MenuItem>
                <dxm:MenuItem Text="销售" NavigateUrl="CRM">
                </dxm:MenuItem>
                <dxm:MenuItem Text="库存" NavigateUrl="WareHouse">
                </dxm:MenuItem>
                <dxm:MenuItem Text="仓储" Name="WTS">
                </dxm:MenuItem>
                <dxm:MenuItem Text="物流" Name="EMS">
                </dxm:MenuItem>
                <dxm:MenuItem Text="生产" Name="Production">
                </dxm:MenuItem>
                <dxm:MenuItem Text="财务" Name="Final">
                </dxm:MenuItem>
                <dxm:MenuItem Text="系统" Name="System">
                </dxm:MenuItem>
            </Items>
</dxm:ASPxMenu>