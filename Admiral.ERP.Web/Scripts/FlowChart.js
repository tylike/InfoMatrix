if (!window.FlowChart) {
    window.FlowChart = function FlowChart(s, e, config) {
        function DoCommand(para, callback, p) {
            config.DoCommand(para, callback, p);
        }
        // create a network
        var container = document.getElementById(s.name);
        var data = {
            nodes: config.nodes,
            edges: config.edges
        };

        var manipulation = {
            enabled: true,

            addNode: function (p, callback) {
                DoCommand('AddNode:' + p.x + ',' + p.y, function (d) { callback(d); }, p);
            },
            editNode: function (data, callback) {
                DoCommand('EditNode:' + data.id, function (d) { callback(d); }, data);
            },
            editEdge: function (data, callback) {
                DoCommand('EditEdge:' + data.id + ',' + data.from + ',' + data.to, function (d) { callback(d); }, data);
            },
            addEdge: function (edgeData, callback) {
                if (edgeData.from === edgeData.to) {
                    var r = confirm('您想将结点连接到自己?');
                    if (r === true) {
                        DoCommand('AddEdge:' + edgeData.from + ',' + edgeData.to, function (d) { callback(d); }, edgeData);
                    }
                }
                else {
                    DoCommand('AddEdge:' + edgeData.from + ',' + edgeData.to, function (d) { callback(d); }, edgeData);
                }
            },
            deleteNode: function (data, callback) {
                DoCommand('DeleteNode:' + data.nodes[0], function (d) { callback(d); }, data);
            },
            deleteEdge: function (data, callback) {
                DoCommand('DeleteEdge:' + data.edges[0], function (d) { callback(d); }, data);
            },
        };


        var options = {
            autoResize: false,
            interaction: {
                navigationButtons: true,
                keyboard: true
            },
                locales: {
                    // create a new locale (text strings should be replaced with localized strings)
                    zhcn: {
                        edit: '编辑',
                        del: '删除选中',
                        back: '后退',
                        addNode: '填加结点',
                        addEdge: '填加动作',
                        editNode: '编辑结点',
                        editEdge: '编辑动作',
                        addDescription: '点击空白区域，填加一个结点。',
                        edgeDescription: '点击一个结点拖放以另一个结点，将生成一个动作，并且连接这两个结点。',
                        editEdgeDescription: '点击控制一个点，拖拽他们连接。',
                        createEdgeError: '不能连接一个动作到一个群集。',
                        deleteClusterError: '集群不能删除。'
                    }
                }
            , locale: 'zhcn',

            edges: { width: 2, arrows: 'to', smooth: false },
            nodes: {
                shadow: {
                    enabled: false,
                    size: 10,
                    x: 5,
                    y: 5
                },
                shape: 'image',
                image: '/image/采购退货单.png'
            }
        };
        if (config.manipulation) {
            options.manipulation = manipulation;
        }

        var n = new vis.Network(container, data, options);
        if (config.manipulation) {
            //setTimeout(function () { n.editNode(); }, 100);
        }
        n.on('doubleClick', function (p) {
            if (p.nodes.length == 1) {
                DoCommand('DoubleClickNode:' + p.nodes[0] + ',' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
            else if (p.edges.length == 1) {
                DoCommand('DoubleClickEdge:' + p.edges[0] + ',' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
            else {
                DoCommand('DoubleClickEmpty:' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
        });
        n.on('click', function (p) {
            if (p.nodes.length == 1) {
                DoCommand('ClickNode:' + p.nodes[0] + ',' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
            else if (p.edges.length == 1) {
                DoCommand('ClickEdge:' + p.edges[0] + ',' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
            else {
                DoCommand('ClickEmpty:' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
        });
        n.on('dragEnd', function (p) {
            if (p.nodes.length == 1) {
                DoCommand('MoveNode:' + p.nodes[0] + ',' + p.pointer.canvas.x + ',' + p.pointer.canvas.y);
            }
        });
    }
}