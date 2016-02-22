using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;

namespace Admiral.ERP.Module.BusinessObjects{
    [DomainLogic(typeof(IDCUser))]
    public class IdcUserLogic
    {
        public static Boolean ComparePassword(IDCUser user, String password)
        {
            return UserImpl.ComparePassword(user.StoredPassword, password);
        }
        public static void SetPassword(IDCUser user, String password)
        {
            user.StoredPassword = UserImpl.GeneratePassword(password);
        }
        public static IList<ISecurityRole> Get_Roles(IDCUser user)
        {
            return new ListConverter<ISecurityRole, IDCRole>(user.StoredRoles);
        }
        public static IEnumerable<IOperationPermission> GetPermissions()
        {
            return new IOperationPermission[0];
        }
        public static IEnumerable<IOperationPermissionProvider> GetChildren(IDCUser user)
        {
            return new ListConverter<IOperationPermissionProvider, IDCRole>(user.StoredRoles);
        }
    }
}