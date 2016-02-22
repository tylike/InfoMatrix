using System;
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    [PropertyEditor(typeof(Color), EditorAliases.ColorPropertyEditor, true)] 
    // Dennis: Tells XAF to use our custom implementation for Color type properties by default.
    public class ASPxColorEditPropertyEditor : ASPxPropertyEditor
    {
        public ASPxColorEditPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
        // Dennis: Creates a control for the Edit mode.

        protected override WebControl CreateEditModeControlCore()
        {
            ASPxColorEdit result = new ASPxColorEdit();
            RenderHelper.SetupASPxWebControl(result);
            return result;
        }
        // Dennis: Creates a control for the View mode, because the default label does not meet our needs. We are almost fine with the ASPxColorEdit control, just need to make it readonly for the View mode.
        protected override WebControl CreateViewModeControlCore()
        {
            //base.CreateViewModeControlCore();
            ASPxColorEdit result = (ASPxColorEdit)CreateEditModeControlCore();
            result.DropDownButton.Visible = false;
            result.DisabledStyle.Border.BorderColor = Color.Transparent;
            result.ReadOnly = true;
            result.ClientEnabled = false;
            return result;
        }
        // Dennis: Since we provide a custom control for the View mode, we need to manually bind it to data.
        protected override void ReadViewModeValueCore()
        {
            base.ReadViewModeValueCore();
            if (InplaceViewModeEditor is ASPxColorEdit && PropertyValue is Color)
            {
                ((ASPxColorEdit)InplaceViewModeEditor).Color = (Color)PropertyValue;
            }
        }
        // Dennis: Sets common options for the underlying controls.
        protected override void SetupControl(WebControl control)
        {
            base.SetupControl(control);
            if (control is ASPxColorEdit)
            {
                ASPxColorEdit result = (ASPxColorEdit)control;
                result.AllowNull = AllowNull;
                result.AllowUserInput = false; //Dennis: The control can display a hex value at the moment, so it is better to prevent user input by default. Track https://www.devexpress.com/issue=S33627.
                result.EnableCustomColors = true;
                result.ColorChanged += new EventHandler(EditValueChangedHandler);
            }
        }
        // Dennis: Unsubsribes from events and releases other resources.
        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (Editor != null)
            {
                ((ASPxColorEdit)Editor).ColorChanged -= new EventHandler(EditValueChangedHandler);
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }
        // Dennis: This is required for the ImmediatePostData attribute operation.
        protected override void SetImmediatePostDataScript(string script)
        {
            Editor.ClientSideEvents.ColorChanged = script;
        }
        public new ASPxColorEdit Editor
        {
            get { return (ASPxColorEdit)base.Editor; }
        }
    }
}