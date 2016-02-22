using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    [XafDisplayName("流程导航")]
    public class FlowChartMenuItem : MenuItemBase
    {
        public FlowChartMenuItem(Session s)
            : base(s)
        {

        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            FlowChartSettings = new FlowChartSettings(Session);
        }

        protected override void OnDeleting()
        {
            FlowChartSettings.Delete();
            base.OnDeleting();
        }

        private FlowChartSettings _FlowChartSettings;
        [ImmediatePostData]
        public FlowChartSettings FlowChartSettings
        {
            get { return _FlowChartSettings; }
            set { SetPropertyValue("FlowChartSettings", ref _FlowChartSettings, value); }
        }

    }
}