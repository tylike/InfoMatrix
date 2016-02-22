using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    [NonPersistent]
    [XafDisplayName("填加节点")]
    public class AddNode : BaseObject
    {

        public AddNode(Session s)
            : base(s)
        {

        }

        FlowChartFormNode _tempFlowChartFormNode;
        FlowChartNode _tempFlowChartNode;

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NodeType = SYS.NodeType.Form;
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsLoading && !IsSaving)
            {
                if ( propertyName == "NodeType")
                {
                    if (Node != null)
                        Node.Delete();

                    if (NodeType == SYS.NodeType.Form)
                    {
                        Node = new FlowChartFormNode(Session);
                    }
                    else
                    {
                        Node = new FlowChartNode(Session);
                    }
                    Node.X = this.X;
                    Node.Y = this.Y;
                }
            }
        }
        private NodeType _NodeType;
        [XafDisplayName("类型")][ImmediatePostData]
        public NodeType NodeType
        {
            get { return _NodeType; }
            set
            {
                SetPropertyValue("NodeType", ref _NodeType, value);
            }
        }

        private FlowChartNode _Node;

        public FlowChartNode Node
        {
            get { return _Node; }
            set { SetPropertyValue("Node", ref _Node, value); }

        }

        private string _X;
        [Browsable(false)]
        public string X
        {
            get { return _X; }
            set { SetPropertyValue("X", ref _X, value); }

        }

        private string _Y;
        [Browsable(false)]
        public string Y
        {
            get { return _Y; }
            set { SetPropertyValue("Y", ref _Y, value); }

        }



    }
}