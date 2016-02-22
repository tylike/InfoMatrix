using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Filtering;
using Guard = DevExpress.Utils.Guard;
using DevExpress.ExpressApp.Web;
namespace Admiral.ERP.Module.Web.Editors
{
    public class ASPxCriteriaPropertyEditor : WebPropertyEditor, IComplexViewItem, IDependentPropertyEditor, ITestable, ITestableContainer
    {
        // Fields
        private ASPxFilterControl criteriaEdit;
        private List<IDisposable> disposablePropertyEditors;
        private CriteriaEditorHelper editorHelper;
        private bool filterExpressionUpdating;
        private static bool showApplyButton;
        private List<ITestable> testableControls;
        private List<ICustomValueParser> valueParsers;

        // Events
        public event EventHandler<CustomCreateDataColumnInfoListEventArgs> CustomCreateDataColumnInfoList;

        public event EventHandler<CustomCreatePropertyEditorEventArgs> CustomCreatePropertyEditor;

        public event EventHandler<CustomCreatePropertyEditorModelEventArgs> CustomCreatePropertyEditorModel;

        public event EventHandler TestableControlsCreated;

        // Methods
        public ASPxCriteriaPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
            this.disposablePropertyEditors = new List<IDisposable>();
            this.testableControls = new List<ITestable>();
            this.valueParsers = new List<ICustomValueParser>();
        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (this.criteriaEdit != null)
            {
                ICallbackManagerHolder page = this.criteriaEdit.Page as ICallbackManagerHolder;
                if (page != null)
                {
                    page.CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(this.CallbackManager_PreRenderInternal);
                }
                foreach (IDisposable disposable in this.disposablePropertyEditors)
                {
                    disposable.Dispose();
                }
                this.disposablePropertyEditors.Clear();
                this.valueParsers.Clear();
                this.criteriaEdit.Columns.Clear();
                this.criteriaEdit.ParseValue -= new FilterControlParseValueEventHandler(this.criteriaEdit_ParseValue);
                this.criteriaEdit.PreRender -= new EventHandler(this.criteriaEdit_PreRender);
                this.criteriaEdit.Load -= new EventHandler(this.criteriaEdit_Load);
                this.criteriaEdit.CustomValueDisplayText -= new FilterControlCustomValueDisplayTextEventHandler(this.criteriaEdit_CustomValueDisplayText);
                this.criteriaEdit.Model.CreateCriteriaParseContext -= new EventHandler<CreateCriteriaParseContextEventArgs>(this.Model_CreateCriteriaParseContext);
                if (!unwireEventsOnly)
                {
                    this.criteriaEdit = null;
                }
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        private void CallbackManager_PreRenderInternal(object sender, EventArgs e)
        {
            ((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(this.CallbackManager_PreRenderInternal);
            if (this.criteriaEdit != null)
            {
                this.WritePatchedFilterString();
            }
        }

        private string ConvertConstantValueToDisplayText(string listFieldName, string propertyName, string constValue, CriteriaLexerToken constToken)
        {
            string str = string.IsNullOrEmpty(listFieldName) ? propertyName : (listFieldName + "." + propertyName);
            FilterControlColumn column = this.criteriaEdit.Columns[str];
            FilterControlLookupEditColumn column2 = column as FilterControlLookupEditColumn;
            if (column2 == null)
            {
                return constValue;
            }
            object editValue = null;
            if (constToken.CriteriaOperator is OperandValue)
            {
                editValue = ((OperandValue)constToken.CriteriaOperator).Value;
            }
            if (editValue == null)
            {
                return string.Empty;
            }
            ASPxComboBox box = (ASPxComboBox)column2.PropertiesLookupEdit.CreateEdit(new CreateEditControlArgs(editValue, null, null, null, null, EditorInplaceMode.StandAlone, false));
            return ('\'' + box.Text + '\'');
        }

        private string ConvertPropertyNameToDisplayText(string listFieldName, string fieldName)
        {
            FilterControlColumn column = !string.IsNullOrEmpty(listFieldName) ? this.criteriaEdit.Columns[listFieldName] : null;
            FilterControlColumn column2 = this.criteriaEdit.Columns[fieldName];
            if (column2 == null)
            {
                return fieldName;
            }
            string displayName = column2.DisplayName;
            while ((column2 = (FilterControlColumn)((IBoundProperty)column2).Parent) != column)
            {
                displayName = column2.DisplayName + '.' + displayName;
            }
            return displayName;
        }

        private void CreateButtons(Panel panel)
        {
            ASPxButton child = DevExpress.ExpressApp.Web.RenderHelper.CreateASPxButton();
            child.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Apply");
            panel.Controls.Add(child);
        }

        protected virtual IEnumerable<DataColumnInfo> CreateDataColumnInfoList()
        {
            CustomCreateDataColumnInfoListEventArgs e = new CustomCreateDataColumnInfoListEventArgs();
            if (this.CustomCreateDataColumnInfoList != null)
            {
                this.CustomCreateDataColumnInfoList(this, e);
            }
            if (e.List == null)
            {
                e.List = this.editorHelper.GetDataColumnInfos();
            }
            return e.List;
        }

        protected override WebControl CreateEditModeControlCore()
        {
            this.CreateFilterControl();
            if (ShowApplyButton)
            {
                Panel panel = new Panel();
                panel.Controls.Add(this.criteriaEdit);
                this.CreateButtons(panel);
                return panel;
            }
            return this.criteriaEdit;
        }

        private void CreateFilterControl()
        {
            this.criteriaEdit = new ASPxFilterControl();
            this.criteriaEdit.Width = Unit.Percentage(100.0);
            this.criteriaEdit.ParseValue += this.criteriaEdit_ParseValue;
            this.criteriaEdit.PreRender += this.criteriaEdit_PreRender;
            this.criteriaEdit.CustomValueDisplayText += this.criteriaEdit_CustomValueDisplayText;
            this.criteriaEdit.EnablePopupMenuScrolling = true;
            DevExpress.ExpressApp.Web.RenderHelper.SetupASPxWebControl(this.criteriaEdit);
            this.criteriaEdit.BeforeGetCallbackResult += this.criteriaEdit_BeforeGetCallbackResult;
            this.criteriaEdit.Load += this.criteriaEdit_Load;
            this.criteriaEdit.Model.CreateCriteriaParseContext += this.Model_CreateCriteriaParseContext;
        }

        protected virtual FilterControlColumn CreateFilterControlColumn(DataColumnInfo columnInfo, CriteriaEditorHelper editorHelper, List<ITestable> testableControls)
        {
            FilterControlColumn column = null;
            ITypeInfo info2;
            IMemberInfo memberInfo = editorHelper.FilteredTypeInfo.FindMember(columnInfo.Name);
            if ((memberInfo == null) || !memberInfo.IsVisible)
            {
                return null;
            }
            IModelMemberViewItem modelDetailViewItem = this.CreatePropertyEditorModel(memberInfo, out info2);
            ASPxPropertyEditor testable = this.CreatePropertyEditor(false, modelDetailViewItem, info2, editorHelper.Application, editorHelper.ObjectSpace);
            if (testable != null)
            {
                testable.ViewEditMode = ViewEditMode.Edit;
                testable.CreateControl();
                if (((testable.TestControl != null) && !(testable is ASPxLookupPropertyEditor)) && !SimpleTypes.IsClass(columnInfo.Type))
                {
                    testableControls.Add(new TestableUnknownClientIdWrapper(testable));
                }
            }
            if (testable is ICustomValueParser)
            {
                this.valueParsers.Add((ICustomValueParser)testable);
            }
            if (testable != null)
            {
                this.disposablePropertyEditors.Add(testable);
                if ((testable is ASPxLookupPropertyEditor) || SimpleTypes.IsClass(columnInfo.Type))
                {
                    FilterControlLookupEditColumn column2 = new FilterControlLookupEditColumn
                    {
                        PropertiesLookupEdit = { ObjectSpace = editorHelper.ObjectSpace, ObjectTypeInfo = editorHelper.FilteredTypeInfo, MemberInfo = memberInfo, Model = modelDetailViewItem }
                    };
                    column2.PropertiesLookupEdit.Model.LookupEditorMode = LookupEditorMode.AllItems;
                    column2.TestCaption = testable.TestCaption;
                    testableControls.Add(new TestableUnknownClientIdWrapper(column2));
                    column = column2;
                }
                else
                {
                    column = new FilterControlPropertyEditorColumn(memberInfo.MemberType)
                    {
                        PropertiesASPxPropertyEditor = { ASPxPropertyEditor = testable }
                    };
                }
            }
            if (column == null)
            {
                column = this.CreateFilterControlColumnByType(columnInfo.Type);
            }
            column.PropertyName = columnInfo.Name.Replace("!", "");
            column.DisplayName = CaptionHelper.GetMemberCaption(editorHelper.FilteredTypeInfo, column.PropertyName);
            return column;
        }

        private FilterControlColumn CreateFilterControlColumnByType(Type type)
        {
            if (type.Equals(typeof(DateTime)))
            {
                return new FilterControlDateColumn();
            }
            if (type.Equals(typeof(bool)))
            {
                return new FilterControlCheckColumn();
            }
            if (type.Equals(typeof(int)) || type.Equals(typeof(decimal)))
            {
                return new FilterControlSpinEditColumn();
            }
            if (!type.Equals(typeof(float)) && !type.Equals(typeof(double)))
            {
                return new FilterControlTextColumn();
            }
            return new FilterControlSpinEditColumn { PropertiesSpinEdit = { NumberType = SpinEditNumberType.Float } };
        }

        protected ASPxPropertyEditor CreatePropertyEditor(bool needProtectedContent, IModelMemberViewItem modelDetailViewItem, ITypeInfo objectType, XafApplication application, IObjectSpace objectSpace)
        {
            CustomCreatePropertyEditorEventArgs e = new CustomCreatePropertyEditorEventArgs();
            if (this.CustomCreatePropertyEditor != null)
            {
                this.CustomCreatePropertyEditor(this, e);
            }
            if (((e.PropertyEditor == null) && (modelDetailViewItem != null)) && (modelDetailViewItem.PropertyEditorType != null))
            {
                e.PropertyEditor = this.editorHelper.Application.EditorFactory.CreateDetailViewEditor(needProtectedContent, modelDetailViewItem, objectType.Type, application, objectSpace) as ASPxPropertyEditor;
            }
            if (e.PropertyEditor != null)
            {
                e.PropertyEditor.ImmediatePostData = false;
            }
            return e.PropertyEditor;
        }

        private IModelMemberViewItem CreatePropertyEditorModel(IMemberInfo memberInfo, out ITypeInfo typeInfo)
        {
            typeInfo = null;
            CustomCreatePropertyEditorModelEventArgs e = new CustomCreatePropertyEditorModelEventArgs();
            if (this.CustomCreatePropertyEditorModel != null)
            {
                this.CustomCreatePropertyEditorModel(this, e);
            }
            if (e.PropertyEditorModel == null)
            {
                e.PropertyEditorModel = this.editorHelper.CreateColumnInfoNode(memberInfo, out typeInfo);
            }
            return e.PropertyEditorModel;
        }

        private void criteriaEdit_BeforeGetCallbackResult(object sender, EventArgs e)
        {
            this.filterExpressionUpdating = true;
            try
            {
                this.WritePatchedFilterString();
                base.EditValueChangedHandler(sender, e);
            }
            finally
            {
                this.filterExpressionUpdating = false;
            }
        }

        private void criteriaEdit_CustomValueDisplayText(object sender, FilterControlCustomValueDisplayTextEventArgs e)
        {
            foreach (ICustomValueParser parser in this.valueParsers)
            {
                string displayText = parser.GetDisplayText(e.Value, e.PropertyInfo);
                if (!string.IsNullOrEmpty(displayText))
                {
                    e.DisplayText = displayText;
                    break;
                }
            }
        }

        private void criteriaEdit_Load(object sender, EventArgs e)
        {
            ICallbackManagerHolder page = ((Control)sender).Page as ICallbackManagerHolder;
            if (page != null)
            {
                page.CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(this.CallbackManager_PreRenderInternal);
            }
        }

        private void criteriaEdit_ParseValue(object sender, FilterControlParseValueEventArgs e)
        {
            foreach (ICustomValueParser parser in this.valueParsers)
            {
                object obj2 = parser.TryParse(e.Text, e.PropertyInfo);
                if (obj2 != null)
                {
                    e.Value = obj2;
                    e.Handled = true;
                    break;
                }
            }
        }

        private void criteriaEdit_PreRender(object sender, EventArgs e)
        {
            if ((((ASPxFilterControl)sender).Page != null) && !((ASPxFilterControl)sender).Page.IsCallback)
            {
                this.WritePatchedFilterString();
            }
        }

        protected override object GetControlValueCore()
        {
            this.criteriaEdit.ApplyFilter();
            return this.editorHelper.ConvertFromOldFormat(this.criteriaEdit.FilterExpression, base.CurrentObject);
        }

        protected override string GetPropertyDisplayValue()
        {
            string objectDisplayText = ReflectionHelper.GetObjectDisplayText(base.PropertyValue);
            return this.editorHelper.ConvertFromOldFormat(objectDisplayText, base.CurrentObject);
        }

        public ITestable[] GetTestableControls()
        {
            return this.testableControls.ToArray();
        }

        private void Model_CreateCriteriaParseContext(object sender, CreateCriteriaParseContextEventArgs e)
        {
            e.Context = this.editorHelper.ObjectSpace.CreateParseCriteriaScope();
        }

        protected void OnTestableControlsCreated()
        {
            if (this.TestableControlsCreated != null)
            {
                this.TestableControlsCreated(this, EventArgs.Empty);
            }
        }

        private void PopulateColumns(CriteriaEditorHelper editorHelper, List<ITestable> testableControls)
        {
            this.valueParsers.Add(new ReadOnlyParametersValueParser());
            this.criteriaEdit.Columns.Clear();
            foreach (DataColumnInfo info in this.CreateDataColumnInfoList())
            {
                if (!CriteriaPropertyEditorHelper.IgnoredMemberTypes.Contains(info.Type))
                {
                    FilterControlColumn column = this.CreateFilterControlColumn(info, editorHelper, testableControls);
                    if (column != null)
                    {
                        this.criteriaEdit.Columns.Add(column);
                    }
                }
            }
        }

        protected override void ReadEditModeValueCore()
        {
            if (!this.filterExpressionUpdating)
            {
                Guard.ArgumentNotNull(this.editorHelper, "helper");
                this.criteriaEdit.FilterExpression = this.editorHelper.ConvertFromOldFormat((string)base.PropertyValue, base.CurrentObject);
                this.editorHelper.Owner = base.CurrentObject;
                this.testableControls.Clear();
                this.PopulateColumns(this.editorHelper, this.testableControls);
                if (this.testableControls.Count > 0)
                {
                    this.OnTestableControlsCreated();
                }
                base.AllowEdit.SetItemValue("The data type is defined", this.editorHelper.FilteredTypeInfo != null);
            }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            if (this.editorHelper == null)
            {
                this.editorHelper = new CriteriaEditorHelper(application, objectSpace, base.MemberInfo, base.ObjectTypeInfo);
            }
            this.editorHelper.SetupObjectSpace(objectSpace);
        }

        internal void WritePatchedFilterString()
        {
            using (this.editorHelper.ObjectSpace.CreateParseCriteriaScope())
            {
                string data = new CriteriaLexerTokenHelper(this.criteriaEdit.FilterExpression).ConvertConstants(new ConvertConstantDelegate(this.ConvertConstantValueToDisplayText));
                this.criteriaEdit.JSProperties["cpPatchedFilterString"] = new CriteriaLexerTokenHelper(data).ConvertProperties(true, new ConvertPropertyDelegate(this.ConvertPropertyNameToDisplayText));
            }
        }

        // Properties
        string ITestable.ClientId
        {
            get
            {
                if (base.Editor != null)
                {
                    return base.Editor.ClientID;
                }
                return base.ClientId;
            }
        }

        string ITestable.TestCaption
        {
            get
            {
                return ((IModelViewItem)base.Model).Caption;
            }
        }

        IJScriptTestControl ITestable.TestControl
        {
            get
            {
                return new JSASPxCriteriaPropertyEditorTestControl();
            }
        }

        public ASPxFilterControl FilterControl
        {
            get
            {
                return this.criteriaEdit;
            }
        }

        public IList<string> MasterProperties
        {
            get
            {
                return new string[] { this.editorHelper.CriteriaObjectTypeMemberInfo.Name };
            }
        }

        public static bool ShowApplyButton
        {
            get
            {
                return showApplyButton;
            }
            set
            {
                showApplyButton = value;
            }
        }
    }

 

}