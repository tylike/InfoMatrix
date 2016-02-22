using Admiral.ERP.Module.Editors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.Web.Editors
{
    public static class ViewIdPropertyEditorHelper
    {
        // Fields
        public const string CaptionField = "Caption";
        public const string IdField = "Id";
        public const string ViewTypeField = "ViewType";

        // Methods
        public static IEnumerable<ViewsDataSource> GetDataSource(XafApplication application)
        {
            if (application == null)
            {
                return null;
            }
            var viewType = ViewType.Any;
            IModelViews views = application.Model.Views;
            List<ViewsDataSource> list = new List<ViewsDataSource>();
            switch (viewType)
            {
                case ViewType.Any:
                    list.AddRange(from modelView in views select new ViewsDataSource(modelView));
                    return list;

                case ViewType.DetailView:
                    list.AddRange(from modelView in views
                                  where modelView is IModelDetailView
                                  select new ViewsDataSource(modelView));
                    return list;

                case ViewType.ListView:
                    list.AddRange(from modelView in views
                                  where modelView is IModelListView
                                  select new ViewsDataSource(modelView));
                    return list;

                case ViewType.DashboardView:
                    list.AddRange(from modelView in views
                                  where modelView is IModelDashboardView
                                  select new ViewsDataSource(modelView));
                    return list;
            }
            return list;
        }

        // Nested Types
        public class ViewsDataSource
        {
            // Methods
            public ViewsDataSource(IModelView modelView)
            {
                this.Id = modelView.Id;
                this.Caption = modelView.Caption;
                this.ViewType = CheckViewType(modelView);
            }

            private static string CheckViewType(IModelView modelView)
            {
                if (modelView is IModelDetailView)
                {
                    return "DetailView";
                }
                if (modelView is IModelListView)
                {
                    return "ListView";
                }
                if (modelView is IModelDashboardView)
                {
                    return "DashboardView";
                }
                return "Error";
            }

            // Properties
            public string Caption { get; private set; }

            public string Id { get; private set; }

            public string ViewType { get; private set; }
        }
    }

    [PropertyEditor(typeof(object), false)]
    public class WebAnyDataSelectPropertyEditor : WebPropertyEditor, IComplexViewItem
    { 
    // Fields
        private XafApplication _application;

        //private ASPxLookupDropDownEdit _lookupControl;

        private ASPxDropDownEdit _lookupControl;

        List<AnyDataSelectSetting> setting;
        protected List<AnyDataSelectSetting> Setting
        {
            get
            {
                if (setting == null)
                {
                    var dsdp = this.CurrentObject as IAnyDataChoiceProvider;
                    if (dsdp == null)
                    {
                        throw new Exception(this.CurrentObject.GetType().FullName + "没有实现IAnyDataSelectDataProvider!");
                    }
                    setting = dsdp.GetAnyDataSelectSetting(this.PropertyName,_application);
                    if (setting == null)
                    {
                        throw new Exception("所属对象没有返回AnyDataSelectSetting对象！");
                    }
                }
                return setting;
            }
        }

        // Methods
        public WebAnyDataSelectPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {

        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            //if ((this._lookupControl != null) && (this._lookupControl.DropDown != null))
            //{
            //    this._lookupControl.DropDown.ValueChanged -= new EventHandler(this.EditValueChangedHandler);
            //}
            base.BreakLinksToControl(unwireEventsOnly);
        }

        protected override WebControl CreateEditModeControlCore()
        {
            this._lookupControl = new ASPxDropDownEdit();
            //this._lookupControl = new ASPxLookupDropDownEdit();
            //this._lookupControl.DropDown.ValueChanged += new EventHandler(this.EditValueChangedHandler);
            return this._lookupControl;
        }

        protected override object GetControlValueCore()
        {
            return this._lookupControl.Value;//.DropDown.Value;
        }

        protected override void ReadEditModeValueCore()
        {
            string value = string.Empty;
            if ((base.PropertyValue != null) && (base.ViewEditMode == ViewEditMode.Edit))
            {
                value = base.PropertyValue as string;
            }
            //this._lookupControl.Value = setting.FindObject(value) ?? string.Empty;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this._application = application;
        }

        protected override void SetupControl(WebControl control)
        {

            var edit = control as ASPxDropDownEdit;
            if ((edit != null) && (base.ViewEditMode == ViewEditMode.Edit))
            {
                edit.DropDownWindowTemplate = new TabsGridTemplate(Setting);
                //edit.DropDown.AllowUserInput = true;
                //edit.DropDown.IncrementalFilteringDelay = 200;
                //edit.DropDown.DataSource = Setting.DataSource;
                //foreach (var item in Setting.Columns)
                //{
                //    edit.DropDown.Columns.Add(item.PropertyName, item.Caption);
                //}

                //edit.DropDown.ValueField = Setting.DataSourceKey;
                //edit.DropDown.TextField = setting.DataSourceCaption;
                //edit.DropDown.TextFormatString = "{0:f}";
                //edit.DropDown.NullText = EditorsLocalizer.Active.GetLocalizedString(ItemsStringId.ViewIdPropertyEditorViewNotFound);
                //edit.AddingEnabled = false;
                //edit.ClearingEnabled = false;
            }
        }

        #region 实现回发功能
        protected override void SetImmediatePostDataScript(string script)
        {

            base.SetImmediatePostDataScript(script);
            if (Editor != null)
            {
                //ASPxPropertyEditor.AssignClientHandlerSafe((Editor as ASPxDropDownEdit), script, "ValueChanged", "AnyDataSourceChanged");
            }
        }

        protected override void SetImmediatePostDataCompanionScript(string script)
        {
            base.SetImmediatePostDataCompanionScript(script);
            if (Editor != null)
            {
                //ASPxPropertyEditor.AssignClientHandlerSafe((Editor as ASPxDropDownEdit), script, "GotFocus", "AnyDataSourceCompare");
            }
        }

        protected override void WriteValueCore()
        {
            this.PropertyValue = (this.Editor as ASPxDropDownEdit).Value;
            //base.WriteValueCore();
        }
        #endregion
    }

    public abstract class ModelNodeSelectPropertyEditor : WebPropertyEditor,IComplexViewItem
    {

        // Fields
        protected XafApplication Application { get; private set; }

        protected ASPxLookupDropDownEdit LookupControl{get;private set;}

        protected abstract AnyDataSelectSetting Setting { get; }

        // Methods
        public ModelNodeSelectPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {

        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if ((this.LookupControl != null) && (this.LookupControl.DropDown != null))
            {
                this.LookupControl.DropDown.ValueChanged -= this.EditValueChangedHandler;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        protected override WebControl CreateEditModeControlCore()
        {
            this.LookupControl = new ASPxLookupDropDownEdit();
            this.LookupControl.DropDown.ValueChanged += this.EditValueChangedHandler;
            return this.LookupControl;
        }



        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.Application = application;
        }

        protected override void SetupControl(WebControl control)
        {

            var edit = control as ASPxLookupDropDownEdit;
            if ((edit != null) && (base.ViewEditMode == ViewEditMode.Edit))
            {
                edit.DropDown.AllowUserInput = true;
                edit.DropDown.IncrementalFilteringDelay = 200;
                edit.DropDown.DataSource = Setting.DataSource;
                foreach (var item in Setting.Columns)
                {
                    edit.DropDown.Columns.Add(item.PropertyName, item.Caption);
                }

                edit.DropDown.ValueField = Setting.DataSourceKey;
                edit.DropDown.TextField = Setting.DataSourceCaption;
                edit.DropDown.TextFormatString = "{0:f}";
                //edit.DropDown.NullText = EditorsLocalizer.Active.GetLocalizedString(ItemsStringId.ViewIdPropertyEditorViewNotFound);
                edit.AddingEnabled = false;
                edit.ClearingEnabled = false;
            }
        }

        #region 实现回发功能
        protected override void SetImmediatePostDataScript(string script)
        {

            base.SetImmediatePostDataScript(script);
            if (Editor != null)
            {
                //ASPxPropertyEditor.AssignClientHandlerSafe((Editor as ASPxLookupDropDownEdit).DropDown, script, "ValueChanged", "AnyDataSourceChanged");
            }
        }

        protected override void SetImmediatePostDataCompanionScript(string script)
        {
            base.SetImmediatePostDataCompanionScript(script);
            if (Editor != null)
            {
                //ASPxPropertyEditor.AssignClientHandlerSafe((Editor as ASPxLookupDropDownEdit).DropDown, script, "GotFocus", "AnyDataSourceCompare");
            }
        }

        
        #endregion
    }

    public class TabsGridTemplate : ITemplate
    {
        List<AnyDataSelectSetting> Setting;
        public TabsGridTemplate(List<AnyDataSelectSetting> setting)
        {
            this.Setting = setting;
        }

        public void InstantiateIn(Control container)
        {
            var tabs = new ASPxPageControl();
            tabs.Width = Unit.Percentage(100);
            tabs.Height = Unit.Percentage(100);
            foreach (var item in Setting)
            {
                var tab = tabs.TabPages.Add(item.PropertyName);
                var grid = new ASPxGridView();
                foreach (var c in item.Columns)
                {
                    var col = new GridViewDataColumn(c.PropertyName, c.Caption);
                    grid.Columns.Add(col);
                }
                grid.Width = Unit.Percentage(100);
                
                grid.DataSource = item.DataSource;
                tab.Controls.Add(grid);
            }
            container.Controls.Add(tabs);
        }
    }

    //[PropertyEditor(typeof(Enum), false)]
    //public class RadioButtonEnumPropertyEditor : WebPropertyEditor
    //{
    //    // Fields
    //    private readonly Dictionary<ASPxRadioButton, object> _controlsHash;
    //    private readonly EnumDescriptor _enumDescriptor;

    //    // Methods
    //    public RadioButtonEnumPropertyEditor(Type objectType, IModelMemberViewItem info)
    //        : base(objectType, info)
    //    {
    //        this._controlsHash = new Dictionary<ASPxRadioButton, object>();
    //        this._enumDescriptor = new EnumDescriptor(base.MemberInfo.MemberType);
    //    }

    //    public override void BreakLinksToControl(bool unwireEventsOnly)
    //    {
    //        if (base.Editor != null)
    //        {
    //            foreach (ASPxRadioButton button in base.Editor.Controls.OfType<ASPxRadioButton>())
    //            {
    //                button.CheckedChanged -= new EventHandler(this.RadioButtonCheckedChanged);
    //            }
    //            if (!unwireEventsOnly)
    //            {
    //                this._controlsHash.Clear();
    //            }
    //        }
    //        base.BreakLinksToControl(unwireEventsOnly);
    //    }

    //    protected override WebControl CreateEditModeControlCore()
    //    {
    //        Panel panel = new Panel();
    //        this._controlsHash.Clear();
    //        TaskCreationHelper currentObject = base.CurrentObject as TaskCreationHelper;
    //        int num = 0;
    //        foreach (object obj2 in this._enumDescriptor.Values)
    //        {
    //            ASPxRadioButton key = new ASPxRadioButton
    //            {
    //                ID = "radioButton_" + num++
    //            };
    //            this._controlsHash.Add(key, obj2);
    //            key.Text = this._enumDescriptor.GetCaption(obj2);
    //            key.CheckedChanged += new EventHandler(this.RadioButtonCheckedChanged);
    //            key.GroupName = base.propertyName;
    //            key.ToolTip = CaptionHelper.GetLocalizedText(@"EnumsToolTips\" + base.MemberInfo.MemberType.FullName, obj2.ToString());
    //            panel.Controls.Add(key);
    //            ASPxLabel child = new ASPxLabel
    //            {
    //                Text = key.ToolTip,
    //                EncodeHtml = true
    //            };
    //            panel.Controls.Add(child);
    //            if (currentObject != null)
    //            {
    //                if (obj2.Equals(TaskCreationParameters.AddToTask))
    //                {
    //                    if (currentObject.Task is DocflowTasksGroup)
    //                    {
    //                        key.Enabled = false;
    //                        child.Enabled = false;
    //                    }
    //                    else if (!((DocflowTask)currentObject.Task).IsVisible)
    //                    {
    //                        key.Enabled = false;
    //                        child.Enabled = false;
    //                    }
    //                }
    //                else if (obj2.Equals(TaskCreationParameters.Clone))
    //                {
    //                    if (currentObject.Task is DocflowTasksGroup)
    //                    {
    //                        key.Enabled = false;
    //                        child.Enabled = false;
    //                    }
    //                    else if (!((DocflowTask)currentObject.Task).IsClonable)
    //                    {
    //                        key.Enabled = false;
    //                        child.Enabled = false;
    //                    }
    //                }
    //                else if (obj2.Equals(SignatureCreationParameters.AddToSignature))
    //                {
    //                    if (currentObject.Task is DocflowTasksGroup)
    //                    {
    //                        key.Enabled = false;
    //                        child.Enabled = false;
    //                    }
    //                    else if (!((DocflowTask)currentObject.Task).IsApprovalSignature)
    //                    {
    //                        key.Enabled = false;
    //                        child.Enabled = false;
    //                    }
    //                }
    //            }
    //        }
    //        return panel;
    //    }

    //    protected override object GetControlValueCore()
    //    {
    //        return (from radioButton in base.Editor.Controls.OfType<ASPxRadioButton>().Cast<ASPxRadioButton>()
    //                where radioButton.Checked
    //                select this._controlsHash[radioButton]).FirstOrDefault<object>();
    //    }

    //    protected override string GetPropertyDisplayValue()
    //    {
    //        return this._enumDescriptor.GetCaption(base.PropertyValue);
    //    }

    //    private void RadioButtonCheckedChanged(object sender, EventArgs e)
    //    {
    //        base.EditValueChangedHandler(sender, e);
    //    }

    //    protected override void ReadEditModeValueCore()
    //    {
    //        object propertyValue = base.PropertyValue;
    //        if (propertyValue != null)
    //        {
    //            foreach (ASPxRadioButton button in base.Editor.Controls.OfType<ASPxRadioButton>())
    //            {
    //                bool flag = button.Checked || propertyValue.Equals(this._controlsHash[button]);
    //                button.Checked = flag;
    //                if (flag && !button.Enabled)
    //                {
    //                    button.Checked = false;
    //                    ASPxRadioButton button2 = base.Editor.Controls.OfType<ASPxRadioButton>().FirstOrDefault<ASPxRadioButton>(x => x.Enabled);
    //                    if (button2 != null)
    //                    {
    //                        button2.Checked = true;
    //                        object obj3 = this._controlsHash[button2];
    //                        base.PropertyValue = obj3;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    protected override void SetImmediatePostDataScript(string script)
    //    {
    //        foreach (ASPxRadioButton button in this._controlsHash.Keys)
    //        {
    //            button.ClientSideEvents.CheckedChanged = script;
    //        }
    //    }
    //}

 

}
