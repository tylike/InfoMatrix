using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects
{
    [Domain( DomainComponetReisterType.SharePart)]
    [XafDisplayName("订单")]
    public interface IFormItemBase : ISimpleObject
    {
        IFormBase GetMaster();
        void SetMaster(IFormBase master);
        //IFormBase Master { get; set; }
    }
}