using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects
{
    /// <summary>
    /// 继承自本接口的，将默认使用分类列表视图来显示数据
    /// </summary>
    [DomainComponent, NonPersistentDc]
    public interface ICategoryBase : IName, ICaption
    {
    
    }
}