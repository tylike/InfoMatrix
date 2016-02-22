using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Admiral.ERP.Module.BusinessObjects.SYS;

namespace Admiral.ERP.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class UpdateImageNameViewController : ViewController
    {
        public UpdateImageNameViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof (FlowChartMenuItem);
            TargetViewType = ViewType.DetailView;
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void UpdateImageName_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var t = e.CurrentObject as FlowChartMenuItem;
            foreach (FlowChartFormNode item in t.FlowChartSettings.Nodes)
            {
                item.ImageUrl = item.GetImageName();
            }
            ObjectSpace.CommitChanges();
        }

        private void CreateDefaultFlow_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var flow = this.View.CurrentObject as FlowChartMenuItem;
            var set = flow.FlowChartSettings;
            var start = set.AddNode("PmsPlan", 0, 0);

            var p1 = set.AddNode(start, "PmsQuery", Position.Right);
            p1 = set.AddNode(p1, "PmsQuote", Position.Right);
            p1.QuickLocate = true;
            p1 = set.AddNode(p1, "PmsContract", Position.Right);
            p1 = set.AddNode(p1, "PmsOrder", Position.Right);
            p1.QuickLocate = true;
            p1 = set.AddNode(p1, "PmsArriveNotice", Position.Right);
            p1 = set.AddNode(p1, "PmsArriveQCRequest", Position.Right);
            var p2 = set.AddNode(p1, "PmsArriveQCReport", Position.Right);
            p1 = set.AddNode(p2, "PmsStockIn", Position.Right);
            p1.QuickLocate = true;
            p1 = set.AddNode(p2, "PmsReturnNotice", Position.Top);
            p1 = set.AddNode(p1, "PmsReturn", Position.Right);


            //p1 = set.AddNode("SmsPlan", 0, 500);
            p1 = set.AddNode("SmsQuery", int.Parse(p1.X)+300, 0);
            p1.QuickLocate = true;
            p1 = set.AddNode(p1, "SmsQuote", Position.Right);
            p1 = set.AddNode(p1, "SmsContract", Position.Right);
            var smsOrder = set.AddNode(p1, "SmsOrder", Position.Right);
            smsOrder.QuickLocate = true;
            p1 = set.AddNode(smsOrder, "SmsNotice", Position.Right);

            p1 = set.AddNode(p1, "SmsStockOutQCRequest", Position.Right);

            p1 = set.AddNode(p1, "SmsStockOutQCReport", Position.Right);
            var report = p1;

            p1 = set.AddNode(p1, "SmsStockOutQCNGProcess", Position.Bottom);

            var sso = set.AddNode(report, "SmsStockOut", Position.Right);
            sso.QuickLocate = true;

            p1 = set.AddNode(smsOrder, "SmsReturnNotice", Position.Top);
            p1 = set.AddNode(p1, "SmsReturn", Position.Right);
            p1 = set.AddNode(p1, "SmsReturnQCRequest", Position.Right);
            var srreport = set.AddNode(p1, "SmsReturnQCReport", Position.Right);
            srreport.QuickLocate = true;
            p1 = set.AddNode(srreport, "SmsReturnStockIn", Position.Right);
            p1 = set.AddNode(srreport, "SmsReturnProcess", Position.Top);

            p1 = set.AddNode("ScmStocktakingNotice", int.Parse(sso.X) + 300, 0);
            var st = set.AddNode(p1,"ScmStocktaking", Position.Right);
            st.QuickLocate = true;
            var last = set.AddNode(st, "ScmStocktakingIn", Position.Right);
            p1 = set.AddNode(st, "ScmStocktakingOut", Position.Top);

            var db = set.AddNode("ScmTransfer", int.Parse(last.X) + 300, 0);
            db.QuickLocate = true;
            p1 = set.AddNode(db, "ScmTransferOut", Position.Right);
            p1 = set.AddNode(db, "ScmTransferIn", Position.Bottom);
            View.Refresh();

        }
    }
}
