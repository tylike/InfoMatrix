using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;

namespace Admiral.ERP.Module.BusinessObjects
{
    [Domain]
    [ImageName("BO_User")]
    [XafDisplayName("User")]
    [XafDefaultProperty("UserName")]
    public interface IDCUser : ISecurityUser, ISecurityUserWithRoles, IOperationPermissionProvider, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser
    {
        new String UserName { get; set; }
        new Boolean IsActive { get; set; }
        new Boolean ChangePasswordOnFirstLogon { get; set; }
        [Browsable(false)]
        String StoredPassword { get; set; }
        [XafDisplayName("Roles")]
        [RuleRequiredField("IDCUser_StoredRoles_RuleRequiredField", DefaultContexts.Save, TargetCriteria = "IsActive=True", CustomMessageTemplate = "An active user must have at least one role assigned.")]
        IList<IDCRole> StoredRoles { get; }

        [Browsable(false)]
        new IList<ISecurityRole> Roles { get; }
    }
}