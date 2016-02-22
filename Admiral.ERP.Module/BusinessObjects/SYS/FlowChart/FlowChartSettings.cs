using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public class FlowChartSettings : BaseObject
    {
        public FlowChartSettings(Session s)
            : base(s)
        {
            InitActions();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            InitActions();
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "QuickDesign") {
                InitActions();
            }
        }

        public virtual void InitActions()
        {
            Actions = new Dictionary<FlowChartAction, Action<FlowChartActionParameter>>();

            Action<FlowChartActionParameter> addNode = p =>
            {
                var node = p.ObjectSpace.CreateObject<AddNode>(); // new AddNode(Session);
                node.Node.X = p.Parameters[0];
                node.Node.Y = p.Parameters[1];

                node.X = node.Node.X;
                node.Y = node.Node.Y;

                p.ActionArgs.ShowViewParameters.CreatedView = p.Application.CreateDetailView(p.ObjectSpace, node, false);
                p.DialogController.Accepting += (s1, e1) =>
                {
                    p.FlowChartSettings.Nodes.Add(node.Node);
                };
            };

            Actions.Add(FlowChartAction.AddNode, addNode);

            Actions.Add(FlowChartAction.DoubleClickEmpty, addNode);

            Actions.Add(FlowChartAction.AddEdge, (p) =>
            {
                var edge = p.ObjectSpace.CreateObject<FlowChartEdge>();
                edge.From = p.FlowChartSettings.Nodes.Single(x => x.Oid == Guid.Parse(p.Parameters[0]));
                edge.To = p.FlowChartSettings.Nodes.Single(x => x.Oid == Guid.Parse(p.Parameters[1]));
                p.ActionArgs.ShowViewParameters.CreatedView = p.Application.CreateDetailView(p.ObjectSpace, edge, false);
                p.DialogController.Accepting += (s1, e1) =>
                {
                    p.FlowChartSettings.Edges.Add(edge);
                };
            });
            Action<FlowChartActionParameter> editEdge = p =>
            {
                var edge = p.FlowChartSettings.Edges.Single(x => x.Oid == Guid.Parse(p.Parameters[0]));
                Guid from = Guid.Empty;
                if (Guid.TryParse(p.Parameters[1], out from))
                {
                    edge.From = p.FlowChartSettings.Nodes.Single(x => x.Oid == from);
                    edge.To = p.FlowChartSettings.Nodes.Single(x => x.Oid == Guid.Parse(p.Parameters[2]));
                }
                p.ActionArgs.ShowViewParameters.CreatedView = p.Application.CreateDetailView(p.ObjectSpace, edge, false);
            };

            Actions.Add(FlowChartAction.EditEdge, editEdge);

            Action<FlowChartActionParameter> editNode = p =>
            {
                var node = p.FlowChartSettings.Nodes.Single(x => x.Oid == Guid.Parse(p.Parameters[0]));
                p.ActionArgs.ShowViewParameters.CreatedView = p.Application.CreateDetailView(p.ObjectSpace, node, false);
            };

            Actions.Add(FlowChartAction.MoveNode, p =>
            {
                var node = p.FlowChartSettings.Nodes.Single(x => x.Oid == Guid.Parse(p.Parameters[0]));
                node.X = p.Parameters[1];
                node.Y = p.Parameters[2];
            });

            Actions.Add(FlowChartAction.EditNode, editNode);

            Actions.Add(FlowChartAction.DeleteNode, p =>
            {
                var node = p.FlowChartSettings.Nodes.Single(x => x.Oid == Guid.Parse(p.Parameters[0]));
                var edges = p.FlowChartSettings.Edges.Where(x => x.From.Oid == node.Oid || x.To.Oid == node.Oid).ToList();
                p.FlowChartSettings.Nodes.Remove(node);
                node.Delete();
                foreach (var item in edges)
                {
                    p.FlowChartSettings.Edges.Remove(item);
                    item.Delete();
                }
            });

            Actions.Add(FlowChartAction.DeleteEdge, p =>
            {
                var edge = p.FlowChartSettings.Edges.Single(x => x.Oid == Guid.Parse(p.Parameters[0]));
                p.FlowChartSettings.Edges.Remove(edge);
                edge.Delete();
            });

            Actions.Add(FlowChartAction.DoubleClickNode, editNode);
            Actions.Add(FlowChartAction.DoubleClickEdge, editEdge);




        }

        private int _EdgesWidth;

        public int EdgesWidth
        {
            get { return _EdgesWidth; }
            set { SetPropertyValue("EdgesWidth", ref _EdgesWidth, value); }

        }

        public Dictionary<FlowChartAction, Action<FlowChartActionParameter>> Actions { get; private set; }

        public override void AfterConstruction()
        {
            FlowChartStyle = "width:100%;height:500px;border: 1px solid lightgray;";
            base.AfterConstruction();
        }

        private string _FlowChartStyle;
        [XafDisplayName("图表样式")]
        [ToolTip("是指图表元素绘制的div的css样式设置，如设置高度宽度等，如:width:100%;height:500px;此处必须符合css语法，否则可能显示不正常!")]
        public string FlowChartStyle
        {
            get { return _FlowChartStyle; }
            set { SetPropertyValue("FlowChartStyle", ref _FlowChartStyle, value); }
        }

        [Association,Agg]
        public XPCollection<FlowChartNode> Nodes { get { return GetCollection<FlowChartNode>("Nodes"); } }

        [Association,Agg]
        public XPCollection<FlowChartEdge> Edges { get { return GetCollection<FlowChartEdge>("Edges"); } }

        protected override void OnDeleting()
        {
            Session.Delete(Nodes);
            Session.Delete(Edges);
            
            base.OnDeleting();
        }

        public string GetNodesJSON()
        {
            return string.Join(",", Nodes.Select(x => x.GetJSON()));
        }
        public string GetEdgesJSON()
        {
            return string.Join(",", Edges.Select(x => x.GetJSON()));
        }

        private IModelBOModel BOS
        {
            get { return SystemHelper.Application.Model.BOModel; }
        }

        public FlowChartFormNode AddNode(string name, int x, int y)
        {
            var node = new FlowChartFormNode(Session);
            node.ModelClass = BOS.SingleOrDefault(t => t.Name == "BusinessObject.I" + name);
            node.X = x.ToString();
            node.Y = y.ToString();
            this.Nodes.Add(node);
            return node;
        }

        public FlowChartFormNode AddNode(FlowChartFormNode parent, string name, Position pos, int defaultStep = 200)
        {
            var x = 0;
            var y = 0;
            var s = defaultStep;
            switch (pos)
            {
                case Position.Left:
                    x -= s;
                    break;
                case Position.LeftTop:
                    x -= s;
                    y -= s;
                    break;
                case Position.Top:
                    y -= s;
                    break;
                case Position.RightTop:
                    x += s;
                    y -= s;
                    break;

                case Position.Right:
                    x += s;
                    break;
                case Position.RightBottom:
                    x += s;
                    y += s;
                    break;
                case Position.Bottom:
                    y += s;
                    break;
                case Position.LeftBottom:
                    x -= s;
                    y -= s;
                    break;

                default:
                    break;
            }
            var node = AddNode(name, int.Parse(parent.X) + x, int.Parse(parent.Y) + y);
            var edge = new FlowChartEdge(Session);
            edge.From = parent;
            edge.To = node;
            edge.Caption = "生成" + edge.To.Caption;
            edge.AutoGenerateMapping = true;
            this.Edges.Add(edge);
            
            return node;
        }


    }

    public enum Position
    {
        Left,
        Top,
        Right,
        Bottom,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }
}