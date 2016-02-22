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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Admiral.ERP.Module.Controllers.Designer
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class ViewDesignerViewController : ViewController
        //<DetailView>
    {
        public ViewDesignerViewController()
        {
            InitializeComponent();
            //TargetViewType = ViewType.DetailView;
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
            var item = this.ModelSetup.Items.SingleOrDefault(x => x.Data == "BusinessView");
            if (item != null)
            {
                item.Items.Clear();
                var m = View.Model as IModelObjectView;
                if (m != null)
                {
                    var views = Application.Model.Views.OfType<IModelObjectView>().Where(x => x.ModelClass == m.ModelClass);
                    foreach (var v in views)
                    {
                        var rv = new ChoiceActionItem(v.Caption + "(" + v.Id + ")", v);
                        item.Items.Add(rv);
                    }
                }
            }
            var rvs = this.ModelSetup.Items.SingleOrDefault(x => x.Data == "RelationView");
            if (rvs != null)
            {
                rvs.Items.Clear();
                IEnumerable<IModelView> views = null;
                if (this.View.Model is IModelDetailView)
                {
                    var m = this.View.Model as IModelDetailView;
                    views = m.Items.OfType<IModelPropertyEditor>().Where(x => x.View != null).Select(x => x.View);
                    views = views.Union(m.Items.OfType<IModelPropertyEditorLinkView>().Where(x => x.LinkView != null).Select(x => x.LinkView)) ;
                }
                else if (this.View.Model is IModelListView)
                {
                    var m = this.View.Model as IModelListView;
                    views = m.Columns.OfType<IModelColumn>().Where(x => x.View != null).Select(x => x.View);
                    views = views.Union(m.Columns.OfType<IModelPropertyEditorLinkView>().Where(x => x.LinkView != null).Select(x => x.LinkView));
                }

                if (views != null)
                {
                    foreach (var v in views)
                    {
                        var rv = new ChoiceActionItem(v.Caption + "(" + v.Id + ")", v);
                        rvs.Items.Add(rv);
                    }
                    rvs.Active["Empty"] = views.Any();
                }


            }
            // Access and customize the target View control.
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }


        private void ModelSetup_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var os = Application.CreateObjectSpace();

            if (e.SelectedChoiceActionItem.Data == "ViewSetup" || e.SelectedChoiceActionItem.Data is IModelView)
            {
                IModelView view = this.View.Model;
                if (e.SelectedChoiceActionItem.Data is IModelView)
                {
                    view = e.SelectedChoiceActionItem.Data as IModelView;
                }

                if (view is IModelDetailView)
                {
                    var obj = os.CreateObject<DetailViewInfo>();
                    obj.LoadDetailViewInfo(view as IModelDetailView);
                    var v = Application.CreateDetailView(os, obj);
                    v.ViewEditMode = ViewEditMode.Edit;
                    e.ShowViewParameters.CreatedView = v;
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    return;
                }

                if (view is IModelListView)
                {
                    var obj = os.CreateObject<ListViewInfo>();
                    obj.LoadViewInfo(view as IModelObjectView);
                    //obj.LoadDetailViewInfo(this.View.Model as IModelDetailView);
                    var v = Application.CreateDetailView(os, obj);
                    v.ViewEditMode = ViewEditMode.Edit;
                    e.ShowViewParameters.CreatedView = v;

                    var dc = new DialogController();
                    dc.Accepting += (s, p) =>
                    {
                        view.Caption = obj.Caption;
                        Application.SaveModelChanges();
                        this.View.Refresh();
                    };

                    e.ShowViewParameters.Controllers.Add(dc);
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    return;
                }
            }

            if (e.SelectedChoiceActionItem.Data == "BusinessObjectSetup")
            {
                var bo = os.FindObject<IBusinessObject>(new BinaryOperator("FullName", (this.View.Model as IModelObjectView).ModelClass.Name));
                if (bo != null)
                {
                    var v = Application.CreateDetailView(os, bo);
                    v.ViewEditMode = ViewEditMode.Edit;
                    e.ShowViewParameters.CreatedView = v;
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    return;
                }
            }

        }

        
    }
}
