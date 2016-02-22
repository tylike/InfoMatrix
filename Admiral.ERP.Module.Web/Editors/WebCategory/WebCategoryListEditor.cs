using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Admiral.ERP.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    //[ListEditor(typeof(ICategoryBase), true)]
    public class WebCategoryListEditor : ListEditor,IComplexListEditor
    {
        public WebCategoryListEditor(IModelListView info)
            : base(info)
        {
            this.DoCommandAction = new SimpleAction(null, "ShowDetailView", PredefinedCategory.Unspecified);
            this.DoCommandAction.Application = _application;
            this.DoCommandAction.Execute += DoCommandAction_Execute;
        }

        private void DoCommandAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var os = _application.CreateObjectSpace();
            var t = this.Model.ModelClass.TypeInfo.Type;
            var obj = os.GetObjectByKey(t, os.GetObjectKey(t, currentSelect));
            e.ShowViewParameters.CreatedView = _application.CreateDetailView(os, Model.DetailView, true, obj);
        }

        private string currentSelect;

        private SimpleAction DoCommandAction;

        private DevExpress.Web.ASPxFileManager fileManager;

        protected override object CreateControlsCore()
        {
            fileManager = new DevExpress.Web.ASPxFileManager();
            fileManager.Width = Unit.Percentage(100);
            fileManager.CustomFileSystemProvider = new CategoryDataProvider("", _collectionSource, this.Model);
            fileManager.Height = 600;
            fileManager.SelectedFileOpened += fileManager_SelectedFileOpened;
            return fileManager;
        }

        void fileManager_SelectedFileOpened(object source, FileManagerFileOpenedEventArgs e)
        {
            currentSelect = e.File.Id;
            DoCommandAction.DoExecute();
        }

        protected override void AssignDataSourceToControl(object dataSource)
        {
            //throw new System.NotImplementedException();
        }

        public override void Refresh()
        {
            //throw new System.NotImplementedException();
        }

        public override IList GetSelectedObjects()
        {
            return new List<object>();
            //throw new System.NotImplementedException();
        }

        public override SelectionType SelectionType
        {
            get
            {
                return DevExpress.ExpressApp.SelectionType.None;
            }
        }

        public override IContextMenuTemplate ContextMenuTemplate
        {
            get
            {
                return null;
            }
        }

        private CollectionSourceBase _collectionSource;
        private XafApplication _application;
        public void Setup(CollectionSourceBase collectionSource, XafApplication application)
        {
            _collectionSource = collectionSource;
            _application = application;
        }
    }
}