using Admiral.ERP.Module.BusinessObjects.SYS;
using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects
{
    [DomainComponent, NonPersistentDc]
    public interface ICaption
    {
        [XafDisplayName("标题")]
        string Caption { get; set; }
    }
}