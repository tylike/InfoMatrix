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
using DevExpress.Xpo;

namespace Admiral.ERP.Module.Controllers.ComplexListView
{
    //用于分类列表视图中的 选类 列表
    //1.隐藏无用的按钮
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class CategorizedListViewController : ViewController
    {
        public CategorizedListViewController()
        {
            InitializeComponent();
            TargetViewType = ViewType.ListView;
            //TargetViewNesting = Nesting.Nested;
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        private IModelAdmiralDashboardViewItem ViewItem
        {
            get
            {
                var nf = Frame as NestedFrame;
                if (nf != null && nf.ViewItem is DashboardViewItem)
                {
                    var vi = (nf.ViewItem as DashboardViewItem).Model as IModelAdmiralDashboardViewItem;
                    if (vi != null && vi.ItemType == "CategoryListView")
                    {
                        return vi;
                    }
                }
                return null;
            }
        }

        public DashboardView Dashboard
        {
            get { return ((Frame as NestedFrame).ViewItem as DashboardViewItem).View as DashboardView; }
        }

        private bool IsInCategoryListView
        {
            get
            {
                return ViewItem != null;
            }
        }

        private void Frame_ViewChanged(object sender, ViewChangedEventArgs e)
        {
            if (Frame.View != null)
            {
                HideActions();
            }
        }

        protected virtual void HideActions()
        {
            if (IsInCategoryListView)
            {
                var actions = new List<string>() {"New", "Edit", "Delete", "Diagnostic Info", "ListViewShowObject"};
                var name = GetType().Name;
                //先隐藏所有可见按钮
                foreach (var controller2 in Frame.Controllers)
                {
                    foreach (var base3 in controller2.Actions)
                    {
                        if (actions.Contains(base3.Id))
                        {
                            base3.Active.RemoveItem(name);
                        }
                        else
                        {
                            base3.Active.SetItemValue(name, false);
                        }
                    }
                }
            }
        }

        protected void ShowAction()
        {
            if (IsInCategoryListView)
            {
                var name = GetType().Name;
                //先隐藏所有可见按钮
                foreach (var controller2 in Frame.Controllers)
                {
                    foreach (var base3 in controller2.Actions)
                    {
                        base3.Active.RemoveItem(name);
                    }
                }
            }
        }

        protected override void OnActivated()
        {
            if (IsInCategoryListView)
            {
                var lvpcoc = Frame.GetController<ListViewProcessCurrentObjectController>();
                if (lvpcoc != null)
                {
                    lvpcoc.CustomProcessSelectedItem += lvpcoc_CustomProcessSelectedItem;
                } 
                Frame.ViewChanged += Frame_ViewChanged;
            }

            base.OnActivated();
        }

        private void lvpcoc_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            if (Dashboard != null)
            {
                var listView = Dashboard.Items.OfType<DashboardViewItem>().SingleOrDefault(x =>
                {
                    var t = x.Model as IModelAdmiralDashboardViewItem;
                    return t != null && t.ItemType == "ListView";
                });
                if (listView != null)
                {
                    var lv = listView.InnerView as ListView;
                    if (lv != null)
                    {
                        var editor = lv.Editor as ISupportFilter;
                        var mem = lv.Model.ModelClass.FindMember(ViewItem.TargetFilterPropertyName);
                        if (mem != null)
                        {
                            e.Handled = true;
                            var xp = e.InnerArgs.CurrentObject as XPCustomObject;
                            var key = mem.MemberInfo.MemberTypeInfo.DefaultMember.Name;
                            editor.Filter = CriteriaOperator.Parse(mem.Name + "." + key + "=?", xp.GetMemberValue(key)).ToString();
                        }
                    }
                    //listView.Control
                }
            }
        }

        protected override void OnDeactivated()
        {
            if (IsInCategoryListView)
            {
                var lvpcoc = Frame.GetController<ListViewProcessCurrentObjectController>();
                if (lvpcoc != null)
                {
                    lvpcoc.CustomProcessSelectedItem += lvpcoc_CustomProcessSelectedItem;
                }

                ShowAction();

                Frame.ViewChanged -= Frame_ViewChanged;
            }
            base.OnDeactivated();
        }

    }
}
