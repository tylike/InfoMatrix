using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    [XafDefaultProperty("Caption")]
    public class FlowChartNode : BaseObject
    {
        public FlowChartNode(Session s)
            : base(s)
        {

        }
        private FlowChartSettings _FlowChartSettins;
        [Association]
        public FlowChartSettings FlowChartSettins
        {
            get { return _FlowChartSettins; }
            set { SetPropertyValue("FlowChartSettins", ref _FlowChartSettins, value); }
        }

        private string _Caption;
        [XafDisplayName("标题")]
        public virtual string Caption
        {
            get { return _Caption; }
            set { SetPropertyValue("Caption", ref _Caption, value); }

        }

        private NodeStyle _NodeStyle;
        [XafDisplayName("结点样式")]
        public NodeStyle NodeStyle
        {
            get { return _NodeStyle; }
            set { SetPropertyValue("NodeStyle", ref _NodeStyle, value); }
        }

        private string _ImageUrl;
        [XafDisplayName("图片地址")]
        public virtual string ImageUrl
        {
            get { return _ImageUrl; }
            set { SetPropertyValue("ImageUrl", ref _ImageUrl, value); }
        }

        public virtual string GetSmallImageUrl() {
            return ImageUrl;
        }
        public virtual string GetLargeImageUrl() {
            return ImageUrl;
        }

        private string _NodeSetting;
        [XafDisplayName("结点设置")]
        [ToolTip("指visjs结点的设置，填写的应为json格式的设置，点击帮肋查看详细语法.")]
        public string NodeSetting
        {
            get { return _NodeSetting; }
            set { SetPropertyValue("NodeSetting", ref _NodeSetting, value); }

        }

        [XafDisplayName("快速定位")]
        public bool QuickLocate { get; set; }

        private string _X;

        public string X
        {
            get { return _X; }
            set { SetPropertyValue("X", ref _X, value); }

        }

        private string _Y;

        public string Y
        {
            get { return _Y; }
            set { SetPropertyValue("Y", ref _Y, value); }

        }


        //borderWidth	Number	1	The width of the border of the node.
        //borderWidthSelected	Number	undefined	The width of the border of the node when it is selected. When undefined, the borderWidth is used
        //brokenImage	String	undefined	When the shape is set to image or circularImage, this option can be an URL to a backup image in case the URL supplied in the image option cannot be resolved.


        public string GetJSON()
        {
            return string.Format("{{ id: '{0}', label: '{1}',shape: '{2}',image: '{3}', physics:false,x:{4},y:{5} }}",
                this.Oid,
                this.Caption,
                this.NodeStyle.ToString(),
                GetLargeImageUrl(), X, Y);
        }
    }
}