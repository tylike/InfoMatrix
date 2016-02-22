using System;
using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Admiral.ERP.Module
{
    public interface IModelAdmiralDashboardView:IModelDashboardView
    {
        Guid Version { get; set; }
    }

    public interface IModelAdmiralDashboardViewItem : IModelDashboardViewItem
    {
        string ItemType { get; set; }
        string TargetFilterPropertyName { get; set; }
    }

    public interface IModelAdmiralDetailView : IModelDetailView
    {
        bool HasDatabaseInfo { get; set; }
    }

    //[DomainLogic(typeof(IModelDetailView))]
    //public class ModelAdmiralDetailViewLogic
    //{
    //    private bool? hasDbInfo;

    //    public bool HasDatabaseInfo(IModelDetailView dv, IObjectSpace os)
    //    {
    //        if (!hasDbInfo.HasValue)
    //        {
    //            var bo = os.FindObject<IBusinessObject>(new BinaryOperator("FullName", dv.ModelClass.Name));
    //            hasDbInfo = bo != null;
    //        }
    //        return hasDbInfo.Value;
    //    }
    //}

}