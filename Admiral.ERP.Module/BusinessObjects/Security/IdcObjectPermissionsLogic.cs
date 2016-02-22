using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;

namespace Admiral.ERP.Module.BusinessObjects
{
    [DomainLogic(typeof(IDCObjectPermissions))]
    public class IDCObjectPermissionsLogic
    {
        public static Boolean? Get_EffectiveRead(IDCObjectPermissions objectPermissions)
        {
            Boolean? result;
            if (objectPermissions.AllowRead)
            {
                result = true;
            }
            else if (objectPermissions.Owner != null && objectPermissions.Owner.AllowRead)
            {
                result = null;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public static void Set_EffectiveRead(IDCObjectPermissions objectPermissions, Boolean? value)
        {
            objectPermissions.AllowRead = value ?? false;
        }
        public static Boolean? Get_EffectiveWrite(IDCObjectPermissions objectPermissions)
        {
            Boolean? result;
            if (objectPermissions.AllowWrite)
            {
                result = true;
            }
            else if (objectPermissions.Owner != null && objectPermissions.Owner.AllowWrite)
            {
                result = null;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public static void Set_EffectiveWrite(IDCObjectPermissions objectPermissions, Boolean? value)
        {
            objectPermissions.AllowWrite = value ?? false;
        }
        public static Boolean? Get_EffectiveDelete(IDCObjectPermissions objectPermissions)
        {
            Boolean? result;
            if (objectPermissions.AllowDelete)
            {
                result = true;
            }
            else if (objectPermissions.Owner != null && objectPermissions.Owner.AllowDelete)
            {
                result = null;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public static void Set_EffectiveDelete(IDCObjectPermissions objectPermissions, Boolean? value)
        {
            objectPermissions.AllowDelete = value ?? false;
        }
        public static Boolean? Get_EffectiveNavigate(IDCObjectPermissions objectPermissions)
        {
            Boolean? result;
            if (objectPermissions.AllowNavigate)
            {
                result = true;
            }
            else if (objectPermissions.Owner != null && objectPermissions.Owner.AllowNavigate)
            {
                result = null;
            }
            else
            {
                result = false;
            }
            return result;
        }
        public static void Set_EffectiveNavigate(IDCObjectPermissions objectPermissions, Boolean? value)
        {
            objectPermissions.AllowNavigate = value ?? false;
        }
        public static String Get_InheritedFrom(IDCObjectPermissions objectPermissions)
        {
            String result = "";
            if (objectPermissions.Owner != null)
            {
                if (objectPermissions.Owner.AllowRead)
                {
                    result = String.Concat(result, String.Format(CaptionHelper.GetLocalizedText("Messages", "Read") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(objectPermissions.Owner.TargetType.FullName)));
                }
                if (objectPermissions.Owner.AllowWrite)
                {
                    result = String.Concat(result, String.Format(CaptionHelper.GetLocalizedText("Messages", "Write") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(objectPermissions.Owner.TargetType.FullName)));
                }
                if (objectPermissions.Owner.AllowDelete)
                {
                    result = String.Concat(result, String.Format(CaptionHelper.GetLocalizedText("Messages", "Delete") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(objectPermissions.Owner.TargetType.FullName)));
                }
                if (objectPermissions.Owner.AllowNavigate)
                {
                    result = String.Concat(result, String.Format(CaptionHelper.GetLocalizedText("Messages", "Navigate") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(objectPermissions.Owner.TargetType.FullName)));
                }
            }
            return result;
        }
        public static IList<IOperationPermission> GetPermissions(IDCObjectPermissions objectPermissions)
        {
            IList<IOperationPermission> result = new List<IOperationPermission>();
            if (objectPermissions.Owner == null)
            {
            }
            else if (objectPermissions.Owner.TargetType == null)
            {
            }
            else
            {
                if (objectPermissions.AllowRead)
                {
                    result.Add(new ObjectOperationPermission(objectPermissions.Owner.TargetType, objectPermissions.Criteria, SecurityOperations.Read));
                }
                if (objectPermissions.AllowWrite)
                {
                    result.Add(new ObjectOperationPermission(objectPermissions.Owner.TargetType, objectPermissions.Criteria, SecurityOperations.Write));
                }
                if (objectPermissions.AllowDelete)
                {
                    result.Add(new ObjectOperationPermission(objectPermissions.Owner.TargetType, objectPermissions.Criteria, SecurityOperations.Delete));
                }
                if (objectPermissions.AllowNavigate)
                {
                    result.Add(new ObjectOperationPermission(objectPermissions.Owner.TargetType, objectPermissions.Criteria, SecurityOperations.Navigate));
                }
            }
            return result;
        }
    }
}