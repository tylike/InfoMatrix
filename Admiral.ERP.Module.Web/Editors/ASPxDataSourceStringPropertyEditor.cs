using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;

namespace Admiral.ERP.Module.Web.Editors
{
    [PropertyEditor(typeof(string), false)]
    public class ASPxDataSourceStringPropertyEditor : ASPxStringPropertyEditor
    {
        // Methods
        public ASPxDataSourceStringPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override WebControl CreateEditModeControlCore()
        {
            bool flag = false;
            if (base.CurrentObject != null)
            {
                Func<IMemberInfo, bool> predicate = null;
                DataSourcePropertyAttribute dataSourcePropertyAttribute = base.MemberInfo.FindAttribute<DataSourcePropertyAttribute>();
                if (dataSourcePropertyAttribute != null)
                {
                    FieldInfo info = typeof(ASPxStringPropertyEditor).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).FirstOrDefault<FieldInfo>(x => x.Name.Equals("predefinedValues"));
                    if (info != null)
                    {
                        if (predicate == null)
                        {
                            predicate = x => x.Name.Equals(dataSourcePropertyAttribute.DataSourceProperty);
                        }
                        IMemberInfo info2 = base.ObjectTypeInfo.Members.FirstOrDefault<IMemberInfo>(predicate);
                        if (info2 != null)
                        {
                            object obj2 = info2.GetValue(base.CurrentObject);
                            if (obj2 is string)
                            {
                                info.SetValue(this, obj2);
                            }
                            IEnumerable<string> source = obj2 as IEnumerable<string>;
                            if (source != null)
                            {
                                string str = source.Aggregate<string, string>(string.Empty, (current, s) => current + s + ";");
                                if (str.Length > 0)
                                {
                                    str = str.Remove(str.Length - 1);
                                    info.SetValue(this, str);
                                }
                            }
                        }
                    }
                    flag = true;
                }
            }
            WebControl control = base.CreateEditModeControlCore();
            if (flag && (control is ASPxDropDownEditBase))
            {
                ((ASPxDropDownEditBase)control).AllowUserInput = false;
                ((ASPxDropDownEditBase)control).ShowShadow = true;
                ((ASPxDropDownEditBase)control).AllowMouseWheel = true;
                if (control is ASPxComboBox)
                {
                    ((ASPxComboBox)control).DropDownStyle = DropDownStyle.DropDownList;
                }
            }
            return control;
        }
    }
}