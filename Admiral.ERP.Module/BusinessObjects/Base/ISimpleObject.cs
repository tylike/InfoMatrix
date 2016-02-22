using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;

namespace Admiral.ERP.Module.BusinessObjects
{
    [NonPersistentDc]
    [DomainComponent]
    [DefineLayoutGroup("SOI", Direction = FlowDirection.Horizontal, Index = 300)]
    [IgnoreFormConvert]
    public interface ISimpleObject
    {
        [XafDisplayName("创建时间")]
        [ModelDefault("AllowEdit", "False")]
        [LayoutGroup("SOI", 100)]
        [ModelDefault("PropertyEditorType", "Admiral.ERP.Module.Web.Editors.ReadOnlyPropertyEditor")]
        DateTime CreateDate { get; set; }

        [XafDisplayName("修改时间")]
        [ModelDefault("AllowEdit", "False")]
        [LayoutGroup("SOI", 300)]
        [ModelDefault("PropertyEditorType", "Admiral.ERP.Module.Web.Editors.ReadOnlyPropertyEditor")]
        DateTime ModifyData { get; set; }

        [XafDisplayName("创建者")]
        [LayoutGroup("SOI", 200)]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("PropertyEditorType", "Admiral.ERP.Module.Web.Editors.ReadOnlyPropertyEditor")]
        string CreateBy { get; set; }

        [XafDisplayName("修改者")]
        [ModelDefault("AllowEdit", "False")]
        [LayoutGroup("SOI", 400)]
        [ModelDefault("PropertyEditorType", "Admiral.ERP.Module.Web.Editors.ReadOnlyPropertyEditor")]
        string ModifyBy { get; set; }
    }
}