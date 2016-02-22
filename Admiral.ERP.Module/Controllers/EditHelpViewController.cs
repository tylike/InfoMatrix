using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Admiral.ERP.Module.BusinessObjects.SYS;
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

namespace Admiral.ERP.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppWindowControllertopic.
    public partial class EditHelpViewController : ViewController<DetailView>
    {
        public EditHelpViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof (IHelpInfo);
            // Target required Windows (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target Window.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void EditViewHelp_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            this.View.ViewEditMode = this.View.ViewEditMode == ViewEditMode.Edit ? ViewEditMode.View : ViewEditMode.Edit;

            base.View.BreakLinksToControls();
            base.View.CreateControls();

        }
    }
}
