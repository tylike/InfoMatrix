using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Admiral.ERP.Module.Web.Editors
{
    [PropertyEditor(typeof(string),false)]
    public class WebViewIdPropertyEditor : WebPropertyEditor, IComplexViewItem
    {
        // Fields
        private XafApplication _application;
        private IEnumerable<ViewIdPropertyEditorHelper.ViewsDataSource> _dataSource;
        private ASPxLookupDropDownEdit _lookupControl;
        private readonly ViewType _viewType;

        // Methods
        public WebViewIdPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
            this._viewType = ViewType.Any;
        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if ((this._lookupControl != null) && (this._lookupControl.DropDown != null))
            {
                this._lookupControl.DropDown.ValueChanged -= new EventHandler(this.EditValueChangedHandler);
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        protected override object CreateControlCore()
        {
            return base.CreateControlCore();
            //return this.Helper.CreatePropertyEditorTemplate();
        }

        protected override WebControl CreateEditModeControlCore()
        {
            this._lookupControl = new ASPxLookupDropDownEdit();
            this._lookupControl.DropDown.ValueChanged += new EventHandler(this.EditValueChangedHandler);
            return this._lookupControl;
        }

        protected override object GetControlValueCore()
        {
            return this._lookupControl.DropDown.Value;
        }

        protected override void ReadEditModeValueCore()
        {
            string value = string.Empty;
            if ((base.PropertyValue != null) && (base.ViewEditMode == ViewEditMode.Edit))
            {
                value = base.PropertyValue as string;
            }
            this._lookupControl.DropDown.Value = this._dataSource.Any<ViewIdPropertyEditorHelper.ViewsDataSource>(a => (a.Id == value)) ? value : string.Empty;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this._application = application;
        }

        protected override void SetupControl(WebControl control)
        {
            ASPxLookupDropDownEdit edit = control as ASPxLookupDropDownEdit;
            if ((edit != null) && (base.ViewEditMode == ViewEditMode.Edit))
            {
                edit.DropDown.AllowUserInput = true;
                edit.DropDown.IncrementalFilteringDelay = 200;
                this._dataSource = ViewIdPropertyEditorHelper.GetDataSource(this._application);
                edit.DropDown.DataSource = this._dataSource;
                edit.DropDown.Columns.Add("Id","Id");
                edit.DropDown.Columns.Add("Caption", "标题");
                edit.DropDown.Columns.Add("ViewType", "视图类型");
                edit.DropDown.ValueField = "Id";
                edit.DropDown.TextField = "Id";
                edit.DropDown.TextFormatString = "{0:f}";
                //edit.DropDown.NullText = EditorsLocalizer.Active.GetLocalizedString(ItemsStringId.ViewIdPropertyEditorViewNotFound);
                edit.AddingEnabled = false;
                edit.ClearingEnabled = false;
            }
        }
    }
}