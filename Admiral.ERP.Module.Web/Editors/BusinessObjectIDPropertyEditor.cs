using System;
using System.Linq;
using Admiral.ERP.Module.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Admiral.ERP.Module.Web.Editors
{
    [PropertyEditor(typeof (IModelClass), AdmiralEditors.BusinessObjectSelector)]
    public class BusinessObjectIDPropertyEditor : ModelNodeSelectPropertyEditor
    {
        private AnyDataSelectSetting _setting;

        protected override AnyDataSelectSetting Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new AnyDataSelectSetting(Model.Application.BOModel.ToList(), PropertyName, "Name", "Caption",
                        x =>
                        {
                            if (Model.Application.BOModel.Any(y => y.Name == x))
                                return x;
                            return "";
                        }, 
                        Application
                        );
                    _setting.AddColumn("Name", "名称");
                    _setting.AddColumn("Caption", "标题");

                }
                return _setting;
            }
        }

        protected override void WriteValueCore()
        {
            this.PropertyValue = Model.Application.BOModel.Single(x => x.Name == ((ASPxLookupDropDownEdit) Editor).DropDown.Value);
            //base.WriteValueCore();
        }

        protected override object GetControlValueCore()
        {
            return this.LookupControl.DropDown.Value;
        }

        protected override void ReadEditModeValueCore()
        {
            string value = string.Empty;
            if ((PropertyValue != null) && (ViewEditMode == ViewEditMode.Edit))
            {
                value = (PropertyValue as IModelClass).Name;
            }
            LookupControl.DropDown.Value = value;
                //Setting.FindObject(value) ?? string.Empty;
        }
        public BusinessObjectIDPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }
    }
}