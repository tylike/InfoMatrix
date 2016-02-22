using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI;
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

namespace Admiral.ERP.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class HideToolbarViewController :ViewController
    {

        public HideToolbarViewController()
        {
            TargetViewId = "t IFormCreator_ItemInfo_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated += View_ControlsCreated;
        }

        private void View_ControlsCreated(object sender, EventArgs e)
        {
            if (Frame != null && Frame.Template is ISupportActionsToolbarVisibility)
            {
                ((ISupportActionsToolbarVisibility) (Frame.Template)).SetVisible(false);
            }
        }

        //private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        //{
        //    if (e.Context == TemplateContext.NestedFrame)
        //    {
        //        if (Frame.Template == null || Frame.View == null || Frame.View.Id != "IFormCreator_ItemInfo_ListView") return;
        //        Debug.WriteLine(Frame.View.Id);

        //        var template = e.Template as ISupportActionsToolbarVisibility;
        //        if (template != null)
        //        {
        //            template.SetVisible(false);
        //        }
        //    }
        //}

        protected override void OnDeactivated()
        {
            //Application.CustomizeTemplate -= Application_CustomizeTemplate;
            View.ControlsCreated -= View_ControlsCreated;
            base.OnDeactivated();
        }


    }
}
