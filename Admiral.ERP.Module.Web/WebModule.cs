using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using System.Web;

namespace Admiral.ERP.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic.
    public sealed partial class ERPAspNetModule : ModuleBase {
        public ERPAspNetModule() {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
            //application.CreateCustomModelDifferenceStore += application_CreateCustomModelDifferenceStore;
            //application.CreateCustomUserModelDifferenceStore += application_CreateCustomUserModelDifferenceStore;

        }

        void application_CreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e)
        {
            if (DesignMode)
            {
                e.Store = new FileModelStore(@"C:\TFS\IMatrix\ERP\Admiral.ERP.Web", "ModelDiffUsers");
                
            }
            else
            {
                e.Store = new FileModelStore(HttpContext.Current.Server.MapPath("~/"), "ModelDiffUsers");
            }
            e.Handled = true;
        }

        void application_CreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e)
        {
            e.Store = new FileModelStore(HttpContext.Current.Server.MapPath("~/"), "ModelDiff");
            
            e.Handled = true;
        }

    }
}
