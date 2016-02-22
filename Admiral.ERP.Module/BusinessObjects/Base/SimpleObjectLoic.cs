using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects
{
    [DomainLogic(typeof(ISimpleObject))]
    public class SimpleObjectLoic
    {

        public static void AfterConstruction(ISimpleObject obj)
        {
            obj.CreateBy = SecuritySystem.CurrentUserName;
            obj.ModifyBy = obj.CreateBy;
            obj.CreateDate = DateTime.Now;
            obj.ModifyData = DateTime.Now;
        }

        public static void OnSaving(ISimpleObject obj)
        {
            obj.ModifyBy = SecuritySystem.CurrentUserName;
            obj.ModifyData = DateTime.Now;
        }
    }
}