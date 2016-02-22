using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.BusinessObjects
{
    [Domain]
    [ImageName("BO_Security_Permission_Object")]
    [XafDisplayName("Object Operation Permissions")]
    [DefaultListViewOptions(true, NewItemRowPosition.Top)]
    public interface IDCObjectPermissions
    {
        [VisibleInListView(true)]
        [FieldSize(FieldSizeAttribute.Unlimited)]
        [CriteriaOptions("Owner.TargetType")]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        String Criteria { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        Boolean AllowRead { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        Boolean AllowWrite { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        Boolean AllowDelete { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        Boolean AllowNavigate { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        IDCTypePermissions Owner { get; set; }

        [NonPersistentDc]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [XafDisplayName("Read")]
        Boolean? EffectiveRead { get; set; }
        [NonPersistentDc]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [XafDisplayName("Write")]
        Boolean? EffectiveWrite { get; set; }
        [NonPersistentDc]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [XafDisplayName("Delete")]
        Boolean? EffectiveDelete { get; set; }
        [NonPersistentDc]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [XafDisplayName("Navigate")]
        Boolean? EffectiveNavigate { get; set; }
        String InheritedFrom { get; }

        IList<IOperationPermission> GetPermissions();
    }
}