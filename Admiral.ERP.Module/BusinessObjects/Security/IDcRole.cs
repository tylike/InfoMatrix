using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Admiral.ERP.Module.BusinessObjects
{
    [Domain]
    [ImageName("BO_Role")]
    [XafDisplayName("Role")]
    [XafDefaultProperty("Name")]
    public interface IDCRole : ISecurityRole, ISecuritySystemRole, IOperationPermissionProvider
    {
        [RuleRequiredField("IDCRole_Name_RuleRequiredField", DefaultContexts.Save)]
        [RuleUniqueValue("IDCRole_Name_RuleUniqueValue", DefaultContexts.Save, "The role with the entered Name was already registered within the system.")]
        new String Name { get; set; }

        Boolean IsAdministrative { get; set; }
        Boolean CanEditModel { get; set; }

        [Aggregated]
        IList<IDCTypePermissions> TypePermissions { get; }
    }
}