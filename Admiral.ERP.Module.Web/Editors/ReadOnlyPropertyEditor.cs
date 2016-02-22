using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    [PropertyEditor(typeof(object), false)]
    public class ReadOnlyPropertyEditor : ASPxPropertyEditor
    {
        public ReadOnlyPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {

        }

        protected override void ReadEditModeValueCore()
        {
            if (!SetValue())
            {
                base.ReadEditModeValueCore();
            }
        }

        protected override void ReadViewModeValueCore()
        {
            if (!SetValue())
            {
                base.ReadViewModeValueCore();
            }
        }

        private bool SetValue()
        {
            //if (this.ASPxEditor == null)
            //    return true;
            var editor = ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit ? this.ASPxEditor : this.InplaceViewModeEditor;

            Action<string> sv = (value) =>
            {
                if (editor is ASPxLabel)
                {
                    (editor as ASPxLabel).Value = value;
                }
                else
                {
                    (editor as Label).Text = value;
                }
            };
            
            if (editor == null)
                return true;
            if (this.MemberInfo.MemberType == typeof(bool))
            {
                if (this.PropertyValue != null)
                {
                    var vle = (bool)base.PropertyValue ? "是" : "否";
                    //editor = vle;
                    sv(vle);
                    return true;
                }
            }

            if (this.MemberInfo.MemberType == typeof(decimal))
            {
                if (this.PropertyValue != null)
                {
                    sv(((decimal)base.PropertyValue).ToString("0.00"));
                    return true;
                }
            }

            if (this.MemberInfo.MemberType == typeof(string))
            {
                sv((string)base.PropertyValue);
            }
            sv(ReflectionHelper.GetObjectDisplayText(base.PropertyValue));
            return true;

        }

        ASPxLabel label;
        protected override WebControl CreateEditModeControlCore()
        {
            return CreateLabel();
        }

        protected override WebControl CreateViewModeControlCore()
        {
            var ctrl = CreateLabel();
            ctrl.Font.Bold = false;
            return ctrl;
        }

        private WebControl CreateLabel()
        {

            var editor = new ASPxLabel();
            editor.Font.Bold = true;
            editor.EncodeHtml = false;
            label = editor;
            return editor;
        }

    }
}