using System;
using Admiral.ERP.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;

namespace Admiral.ERP.Module.Web
{
    /// <summary>
    /// 实现完全个性化的查象对象视图在列表上点击确定后，选取对象的动作
    /// </summary>
    public class SuperLookupViewController:ViewController
    {
        

        public SuperLookupViewController()
        {
            TargetObjectType = typeof (ISuperLookupHelper);
            TargetViewType = ViewType.DetailView;
        }

        private ListPropertyEditor[] listPropertyEditor;
        
        private void SubscribeToNestedListViewController()
        {
            foreach (var lpe in this.listPropertyEditor)
            {
                if (lpe.Frame != null)
                {
                    var pc = lpe.Frame.GetController<ListViewProcessCurrentObjectController>();
                    pc.CustomProcessSelectedItem += pc_CustomProcessSelectedItem;
                }
            }
        }

        void pc_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            var obj = this.View.CurrentObject as ISuperLookupHelper;
            if (obj != null)
            {
                obj.OnSelected(e.InnerArgs.CurrentObject);
                Frame.GetController<DialogController>().AcceptAction.DoExecute();
            }
        }
        protected override void OnViewControlsCreated()
        {
            var obj = this.View.CurrentObject as ISuperLookupHelper;
            listPropertyEditor = new ListPropertyEditor[obj.LookupViewProperties.Length];
            int i = 0;
            foreach (var p in obj.LookupViewProperties)
            {
                var lpe = (ListPropertyEditor) ((DetailView) View).FindItem(p);

                //if (lpe.Frame != null)
                //    lpe.ControlCreated += SuperLookupViewController_ControlCreated;
                listPropertyEditor[i] = lpe;
                i++;
            }
            SubscribeToNestedListViewController();
            base.OnViewControlsCreated();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            
        }

        protected override void OnDeactivated()
        {
            foreach (var lpe in this.listPropertyEditor)
            {
                if (lpe.Frame != null)
                {
                    lpe.Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= pc_CustomProcessSelectedItem;
                }
                lpe.ControlCreated -= SuperLookupViewController_ControlCreated;
            }
            base.OnDeactivated();
        }

        private void SuperLookupViewController_ControlCreated(object sender, EventArgs e)
        {
            SubscribeToNestedListViewController();
        }
    }
}