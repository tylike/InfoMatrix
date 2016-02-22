using DevExpress.ExpressApp.DC;

namespace Admiral.ERP.Module.BusinessObjects
{
    /// <summary>
    /// 付款方式
    /// </summary>
    public enum EnumPaymentType
    {
        [XafDisplayName("现金")]
        Cash,
        [XafDisplayName("银行转账")]
        BankTransfer,
        [XafDisplayName("汇款")]
        Remittance,
        [XafDisplayName("支票")]
        Check,
        [XafDisplayName("本票")]
        PromissoryNote,
        [XafDisplayName("电汇")]
        TelegraphicTransfer,
        [XafDisplayName("信用证")]
        LetterCredit
    }
}