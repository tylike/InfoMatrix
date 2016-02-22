using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admiral.ERP.Module.BusinessObjects
{
    [DomainComponent]
    [NonPersistentDc]
    [IgnoreFormConvert]
    public interface ICode
    {
        [XafDisplayName("编号")]
        [ModelDefault("AllowEdit", "False")]
        [LayoutGroup("BIR1",100)]
        string Code { get; set; }
    }

    [DomainLogic(typeof(ICode))]
    public class CodeLogic
    {
        public void AfterConstruction(ICode obj)
        {
            obj.Code = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }
}
