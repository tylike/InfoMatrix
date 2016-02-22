using System;
using System.Web.UI.WebControls;
using Admiral.ERP.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using PopupWindow = DevExpress.ExpressApp.Web.PopupWindow;

namespace Admiral.ERP.Module.Web.Editors
{
    // ReSharper disable once InconsistentNaming
    [PropertyEditor(typeof(ISuperLookupHelper),true)]
    public class ASPxObjectSelectorPropertyEditor : ASPxObjectPropertyEditorBase, ISupportViewShowing
    {
        // Fields
        private bool _buttonEditTextEnabled;
        private ASPxButtonEdit _control;
        private ObjectEditorHelper _helper;
        private PopupWindowShowAction _objectWindowAction;

        // Events
        event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification
        {
            add
            {
                viewShowingNotification += value;
            }
            remove
            {
                viewShowingNotification -= value;
            }
        }

        // ReSharper disable once InconsistentNaming
        private event EventHandler<EventArgs> viewShowingNotification;

        // Methods
        public ASPxObjectSelectorPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
            _buttonEditTextEnabled = true;
            skipEditModeDataBind = true;
        }

        protected override void ApplyReadOnly()
        {
            if ((Editor != null) && (Editor.Controls.Count > 0))
            {
                SetButtonEditTextEnabled(AllowEdit);
                _control.Buttons[0].Enabled = (PropertyValue != null) || AllowEdit;
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (!unwireEventsOnly)
            {
                _control = null;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        private void buttonEdit_Load(object sender, EventArgs e)
        {
            var editor = (ASPxButtonEdit)sender;
            editor.Load -= buttonEdit_Load;
            EditorWithButtonEditHelper.AssignButtonClickScript(editor, application, _objectWindowAction);
        }

        protected override WebControl CreateEditModeControlCore()
        {
            if (_objectWindowAction == null)
            {
                _objectWindowAction = new PopupWindowShowAction(null, "ShowObjectDetailViewPopup", PredefinedCategory.Unspecified.ToString());
                _objectWindowAction.Execute += objectWindowAction_OnExecute;
                _objectWindowAction.CustomizePopupWindowParams += objectWindowAction_OnCustomizePopupWindowParams;
                _objectWindowAction.Application = application;
            }
            _control = new ASPxButtonEdit {EnableClientSideAPI = true};
            _control.Load += buttonEdit_Load;
            _control.Buttons.Clear();
            _control.Buttons.Add("");
            ASPxImageHelper.SetImageProperties(_control.Buttons[0].Image, "Editor_Search");
            if (_control.Buttons[0].Image.IsEmpty)
            {
                _control.Buttons[0].Text = "Edit";
            }
            _control.Enabled = true;
            _control.ReadOnly = true;
            return _control;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_objectWindowAction != null)
                    {
                        _objectWindowAction.Execute -= objectWindowAction_OnExecute;
                        _objectWindowAction.CustomizePopupWindowParams -= objectWindowAction_OnCustomizePopupWindowParams;
                        _objectWindowAction.Dispose();
                        _objectWindowAction = null;
                    }
                    if (_control != null)
                    {
                        _control.Buttons.Clear();
                        _control.Dispose();
                        _control = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override object GetControlValueCore()
        {
            return MemberInfo.GetValue(CurrentObject);
        }

        protected override string GetPropertyDisplayValue()
        {
            return _helper.GetDisplayText(MemberInfo.GetValue(CurrentObject), EmptyValue, DisplayFormat);
        }

        protected override bool IsMemberSetterRequired()
        {
            return false;
        }

        private void objectWindowAction_OnCustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args)
        {
            var view = application.CreateDetailView(objectSpace, PropertyValue, false);
            view.AllowEdit.SetItemValue(typeof(ObjectEditorHelper).FullName, AllowEdit);

            args.DialogController.SaveOnAccept = false;
            args.View = view;
            //args.View.ObjectSpace.SetModified(((DetailView)args.View).CurrentObject);
            //OnViewShowingNotification();
        }

        private void objectWindowAction_OnExecute(object sender, PopupWindowShowActionExecuteEventArgs args)
        {
            if (args.PopupWindow.View.AllowEdit != null)
            {
                //args.PopupWindow.View.ObjectSpace.CommitChanges();
                PropertyValue = ((ObjectView)args.PopupWindow.View).CurrentObject;
            }
            var propertyDisplayValue = GetPropertyDisplayValue();
            ((PopupWindow)args.PopupWindow).ClosureScript = "if(window.dialogOpener) window.dialogOpener.resultObject = '" + ((propertyDisplayValue != null) ? propertyDisplayValue.Replace("'", @"\'") : "") + "';";
        }

        private void OnViewShowingNotification()
        {
            if (viewShowingNotification != null)
            {
                viewShowingNotification(this, EventArgs.Empty);
            }
        }

        protected override void ReadEditModeValueCore()
        {
            _control.Value = GetPropertyDisplayValue();
        }

        private void SetButtonEditTextEnabled(bool value)
        {
            if (_buttonEditTextEnabled != value)
            {
                _buttonEditTextEnabled = value;
                if (value)
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    _control.CssClass.Replace(" dxDisabled", "");
                }
                else
                {
                    _control.CssClass = _control.CssClass + " dxDisabled";
                }
            }
        }

        public override void Setup(IObjectSpace os, XafApplication app)
        {
            base.Setup(os, app);
            if (_helper == null)
            {
                _helper = new ObjectEditorHelper(MemberInfo.MemberTypeInfo, Model);
            }
        }
    }
}