using System;
using System.Collections.Specialized;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.Web.Editors
{
    //[PropertyEditor(typeof(Image),EditorAliases.ImagePropertyEditor,true)]
    public class AdmiralASPxImagePropertyEditor : ASPxImagePropertyEditor
    {
        public AdmiralASPxImagePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }

        protected override void ReadViewModeValueCore()
        {
            base.ReadViewModeValueCore();
        }
        



 

    }
}