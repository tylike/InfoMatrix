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
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Admiral.ERP.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class CustomSearchViewController : ViewController<ListView>
    {
        public CustomSearchViewController()
        {
            InitializeComponent();
            this.CustomSearch.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            var def = new ChoiceActionItem("默认方案", "Default");
            this.CustomSearch.Items.Add(def);
        }

        private void CustomSearch_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {

            var editor = View.Editor as ASPxGridListEditor;
            if (editor != null)
            {
                editor.Grid.ShowFilterControl();
            }
        }
    }
}
