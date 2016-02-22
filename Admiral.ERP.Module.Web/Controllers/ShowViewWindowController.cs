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
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web;

namespace Admiral.ERP.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppWindowControllertopic.
    public partial class ShowViewWindowController : WindowController, IXafCallbackHandler 
    {
        public ShowViewWindowController()
        {
            InitializeComponent();
            TargetWindowType = WindowType.Main;
            // Target required Windows (via the TargetXXX properties) and create their Actions.
        }
        
        protected override void OnActivated()
        {
            

            base.OnActivated();
            if (ShowView.Items.Count <= 0) {
                var os = Application.CreateObjectSpace();
                var mi = os.FindObject<FlowChartMenuItem>(null);
                if (mi != null)
                {
                    foreach (var item in mi.FlowChartSettings.Nodes)
                    {
                        ShowView.Items.Add(new ChoiceActionItem(item.Caption, item));
                    }
                }
            }
            Frame.TemplateChanged += Frame_TemplateChanged;
            
            // Perform various tasks depending on the target Window.
        }

        void Frame_TemplateChanged(object sender, EventArgs e)
        {
             XafCallbackManager callbackManager = ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            callbackManager.RegisterHandler("ShowViewWindowController", this);
        }
        
        protected override void OnDeactivated()
        {
            //Window.ProcessActionContainer -= Window_ProcessActionContainer;
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void ShowView_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var form = e.SelectedChoiceActionItem.Data as FlowChartFormNode;
            var bo = Application.Model.BOModel.Single(x => x.Name == form.FormID);
            var os = Application.CreateObjectSpace();
            e.ShowViewParameters.CreatedView = Application.CreateListView(os, bo.TypeInfo.Type, true);
        }

        public void ProcessAction(string parameter)
        {
            var t = this.ShowView.Items.Single(x => (x.Data as FlowChartFormNode).Oid == Guid.Parse(parameter));
            this.ShowView.DoExecute(t);
        }
    }
}
