using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;

namespace Admiral.ERP.Module.BusinessObjects
{
    [DomainLogic(typeof(IDCRole))]
    public class IDCRoleLogic
    {
        public static IEnumerable<IOperationPermissionProvider> GetChildren()
        {
            return new IOperationPermissionProvider[0];
        }
        public static IEnumerable<IOperationPermission> GetPermissions(IDCRole role)
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            if (role.IsAdministrative)
            {
                result.Add(new IsAdministratorPermission());
            }
            if (role.CanEditModel)
            {
                result.Add(new ModelOperationPermission());
            }
            foreach (IDCTypePermissions persistentPermission in role.TypePermissions)
            {
                result.AddRange(persistentPermission.GetPermissions());
            }
            return result;
        }
    }
}