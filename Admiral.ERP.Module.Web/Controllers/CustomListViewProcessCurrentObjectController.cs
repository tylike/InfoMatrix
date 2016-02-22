using Admiral.ERP.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Admiral.ERP.Module.Web
{
    public partial class CustomListViewProcessCurrentObjectController : ViewController,IModelExtender

    {
        public CustomListViewProcessCurrentObjectController()
        {

            //InitializeComponent();
            //RegisterActions(this);
            //TargetViewId = "TeamPotencial_Answers_ListView";
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ListViewProcessCurrentObjectController l = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (l != null)
            {
                //l.CustomProcessSelectedItem += l_CustomProcessSelectedItem;
            }

            ListViewController lvC = Frame.GetController<ListViewController>();
            if (lvC != null)
            {
                //lvC.InlineEditAction.Active.SetItemValue("ByInlineEditCurrentObjectController", false);
            }

        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ListViewProcessCurrentObjectController l = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (l != null)
            {
                l.CustomProcessSelectedItem -= l_CustomProcessSelectedItem;
            }
            ListViewController lvC = Frame.GetController<ListViewController>();
            if (lvC != null)
            {
                //lvC.InlineEditAction.Active.RemoveItem("ByInlineEditCurrentObjectController");
            }
        }

        void l_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            var nf = Frame as NestedFrame;
            if (nf != null && nf.ViewItem.CurrentObject is ISuperLookupHelper)
            {
                //var s = nf.ViewItem.CurrentObject as ISuperLookupHelper;
                //s.OnSelected(e.InnerArgs.CurrentObject);

                return;
            }

            if (View is ListView && ((ListView)View).Editor is ASPxGridListEditor)
            {
                return;
                ASPxGridView grid = ((ASPxGridListEditor)((ListView)View).Editor).Grid;
                if (grid != null)
                {
                    int visibleIndex = grid.FindVisibleIndexByKeyValue(View.ObjectSpace.GetKeyValue(e.InnerArgs.CurrentObject));
                    grid.StartEdit(visibleIndex);
                }
            }
        }


        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {

            var t = typeof(ModelDetailViewLayoutNodesGenerator);
            //EasyTypeGenerator
        }
    }
}
