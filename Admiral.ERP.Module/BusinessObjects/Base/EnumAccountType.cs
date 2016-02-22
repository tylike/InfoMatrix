using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects
{
    /// <summary>
    /// 结算方式
    /// </summary>
    public enum EnumAccountType
    {
        [XafDisplayName("银行汇票")]
        BankDraft,
        [XafDisplayName("银行本票")]
        BankNote,
        [XafDisplayName("商业汇票")]
        CommercialDraft,
        [XafDisplayName("支票")]
        Check,
        [XafDisplayName("汇兑")]
        Exchange,
        [XafDisplayName("委托收款")]
        EntrustReceivables,
        [XafDisplayName("托收承付")]
        Acceptance
    }
}