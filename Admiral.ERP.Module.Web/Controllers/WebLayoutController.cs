using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Admiral.ERP.Module.BusinessObjects.SYS;
using Admiral.ERP.Module.Web.Editors.Layout;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.Web.Controllers
{
    public class WebLayoutController : ViewController<DetailView>
    {
        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.Object == View.CurrentObject)
            {
                WebApplication.Redirect(HttpContext.Current.Request.RawUrl, false);
            }
        }

        private void View_ControlsCreating(object sender, EventArgs e)
        {
            UpdateLayoutManagerTemplates();
            //WebWindow.CurrentRequestWindow.RegisterClientScript("initMenu", "InitPopupMenuHandler();");
        }

        private void UpdateLayoutManagerTemplates()
        {
            var app = Application as WebApplication;
            string str = showAddationInfo.GetHashCode().ToString();
            app.PopupWindowManager.GetShowPopupWindowScript(this.showAddationInfo, "", "", false, false);
            //RaiseXafCallback(globalCallbackControl, 'PopupWindowHandler', '|false||Default.aspx?Dialog=true&ActionID=36087977|false', '', false);
            var showHelp = View.ObjectTypeInfo.FindAttribute<ShowHelpAttribute>();
            var editorSetup = View.ObjectTypeInfo.FindAttribute<EditorSetupAttribute>();
            var b = IsShowEditorSetup(editorSetup);

            LayoutBaseTemplate itemTemplate = new AdmiralLayoutItemTemplate(str,
                showHelp == null || showHelp.ShowHelp,
                b
                );

            WebLayoutManager layoutManager = (WebLayoutManager) View.LayoutManager;
            layoutManager.LayoutItemTemplate = itemTemplate;
            LayoutBaseTemplate groupTemplate = new AdmiralLayoutGroupTemplate();
            layoutManager.LayoutGroupTemplate = groupTemplate;
            layoutManager.TabbedGroupTemplate = new AdmiralLayoutTabbedGroupTemplate();
        }

        private bool IsShowEditorSetup(EditorSetupAttribute editorSetup)
        {
            var showEditorSetup = editorSetup == null || editorSetup.EditorSetup;
            if (showEditorSetup)
            {
                var bo = ObjectSpace.FindObject<IBusinessObject>(new BinaryOperator("FullName", View.Model.ModelClass.Name));
                showEditorSetup = bo != null;
            }
            return showEditorSetup;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreating += new EventHandler(View_ControlsCreating);
            //ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
        }

        protected override void OnDeactivated()
        {
            View.ControlsCreating -= new EventHandler(View_ControlsCreating);
            //ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
            base.OnDeactivated();
        }

        private PopupWindowShowAction showAddationInfo;
        public WebLayoutController()
        {
            showAddationInfo = new PopupWindowShowAction(this.Container);
            showAddationInfo.Caption = "更多设置";
            showAddationInfo.CustomizePopupWindowParams += showAddationInfo_CustomizePopupWindowParams;
            showAddationInfo.Category = "Hidden";
           
            this.Actions.Add(showAddationInfo);
            this.RegisterActions(this.Container);
        }

        private void showAddationInfo_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace();
            var para = WebWindow.CurrentRequestPage.Request["para"];
            var menuItem = WebWindow.CurrentRequestPage.Request["MenuItem"];
            var viewItem = this.View.Model.Items[para];
            var isView = ViewEditMode.View;
            if (menuItem == "Help")
            {
                var obj = os.FindObject<IHelpInfo>(new BinaryOperator("ItemID", this.View.Id + "." + viewItem.Id));
                if (obj == null)
                {
                    obj = os.CreateObject<IHelpInfo>();
                    obj.ItemID = this.View.Id + "." + viewItem.Id;
                    obj.Title = "“"+viewItem.Caption + "”的帮助";
                    obj.Content = "该主题还没有维护帮助内容，请在这里录入内容！";
                    isView = ViewEditMode.Edit;
                }
                //new AddationInfo(os, viewItem);
                var view = Application.CreateDetailView(os, obj);
                view.ViewEditMode = isView;
                e.View = view;
            }
            else if (menuItem == "EditorSetup")
            {
                var obj = os.FindObject<IViewItemSetup>(new BinaryOperator("ItemID", this.View.Id + "." + viewItem.Id));
                if (obj == null)
                {
                    var pe = viewItem as IModelPropertyEditor;
                    if (pe != null)
                    {
                        var peiobj = os.CreateObject<IPropertyEditorInfo>();
                        peiobj.ItemID = View.Id + "." + viewItem.Id;

                        var propertyInfo = os.FindObject<IProperty>(CriteriaOperator.Parse("Owner.FullName=? && Name=?", pe.ModelMember.ModelClass.Name,pe.PropertyName));
                        if (propertyInfo == null)
                        {
                            throw new Exception("错误，没有找到属性信息！请确认本视图是否可以设置！");
                        }
                        peiobj.PropertyName = propertyInfo;
                        obj = peiobj;
                    }
                }
                //new AddationInfo(os, viewItem);
                var view = Application.CreateDetailView(os, obj);
                e.View = view;
            }
            //e.DialogController.SaveOnAccept = false;
        }
    }
}
