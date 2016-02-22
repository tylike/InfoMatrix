using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;

namespace Admiral.ERP.Module.BusinessObjects
{
    [Domain]
    [ImageName("BO_Security_Permission_Member")]
    [XafDisplayName("Member Operation Permissions")]
    [DefaultListViewOptions(true, NewItemRowPosition.Top)]
    public interface IDCMemberPermissions
    {
        [FieldSize(FieldSizeAttribute.Unlimited)]
        [VisibleInListView(true)]
        string Members { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        bool AllowRead { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        bool AllowWrite { get; set; }
        [VisibleInListView(false), VisibleInDetailView(false)]
        IDCTypePermissions Owner { get; set; }

        [NonPersistentDc]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [XafDisplayName("Read")]
        bool? EffectiveRead { get; set; }
        [NonPersistentDc]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [XafDisplayName("Write")]
        bool? EffectiveWrite { get; set; }
        string InheritedFrom { get; }

        IList<IOperationPermission> GetPermissions();
    }
}