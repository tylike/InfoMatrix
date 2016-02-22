using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;

namespace Admiral.ERP.Module.BusinessObjects
{

    [NonPersistentDc]
    [DomainComponent]
    [XafDefaultProperty("Name")]
    public interface IName
    {
        [XafDisplayName("名称")]
        [RuleRequiredField(DefaultContexts.Save)]
        string Name { get; set; }

    }
}
