<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Designer.aspx.cs" Inherits="Admiral.ERP.Web.Designer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    
    <script type="text/javascript" src="/gl/jquery.js"></script>
<%--    <script type="text/javascript" src="/gl/goldenlayout.min.js"></script>--%>
    <script src="/gl/goldenlayout.js"></script>

    <link href="/gl/css/goldenlayout-base.css" rel="stylesheet" />
<%--    <link href="/gl/css/goldenlayout-dark-theme.css" rel="stylesheet" />--%>
    <link href="/gl/css/goldenlayout-light-theme.css" rel="stylesheet" />
    <style>
        h2{
  font: 14px Arial, sans-serif;
  color:#fff;
  padding: 10px;
  text-align: center;
}

html, body,form{
  height: 100%;
}

*{
  margin: 0;
  padding: 0;
  list-style-type:none;
}

#wrapper{
  height: 100%;
  position: relative;
  width: 100%;
  overflow: hidden;
}

#menuContainer{
  width: 20%;
  height: 100%;
  position:absolute;
  top: 0;
  left: 0;
  background: #222;
}

#menuContainer li{
  cursor: move;
  border-bottom: 1px solid #000;
  border-top: 1px solid #333;
  cursor: pointer;
  padding: 10px 5px;
  color: #BBB;
  background: #1a1a1a;
  font: 12px Arial, sans-serif;
}

#menuContainer li:hover{
  background: #111;
  color: #CCC;
}

#layoutContainer{
  width: 80%;
  height: 100%;
  position:absolute;
  top: 0;
  left: 20%;
  box-shadow: -3px 0px 9px 0px rgba( 0, 0, 0, 0.4 );
}
    </style>
    <script>
        $(document).ready(function() {
            var config = {
                type:'root',
                content: []
            };

            var myLayout = new window.GoldenLayout(config, $('#layoutContainer'));

            myLayout.registerComponent('Field', function(container, state) {
                container.getElement().html('<h2>' + state.text + '</h2>');
            });

            myLayout.init();

            var addMenuItem = function(title, text,type) {
                var element = $('<li>' + text + '</li>');
                $('#menuContainer').append(element);
                var newItemConfig = null;
                if(type == 'stack' || type == 'column' || type=='row') 
                {
                    newItemConfig = {
                        title: title,
                        type: type,
                        
                        content: []
                    };
                } else {
                    newItemConfig = {
                        title: title,
                        type: type,
                        componentName: 'Field',
                        componentState: { text: text }
                    };
                }

                myLayout.createDragSource(element, newItemConfig);
            };

            addMenuItem('布局项目', '字段','component');
            addMenuItem('选项卡', '选项卡','stack');
            addMenuItem('行布局', '行布局', 'row');
            addMenuItem('列布局', '列布局', 'column');

        });
        

    </script>
</head>
<body>
    <form id="form1" runat="server">
<div id="wrapper">
  <ul id="menuContainer"></ul>
  <div id="layoutContainer"></div>
</div>
    </form>
</body>
</html>
