using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Admiral.ERP.Module.BusinessObjects.SYS
{
    public class FlowChartActionParameter {

        public SimpleActionExecuteEventArgs ActionArgs { get; set; }

        public string[] Parameters { get; set; }
        public XafApplication Application { get; set; }
        public IObjectSpace ObjectSpace { get; set; }
        public FlowChartSettings FlowChartSettings { get; set; }

        public DialogController DialogController { get; set; }
    }
}