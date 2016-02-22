using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

namespace Admiral.ERP.Module.BusinessObjects
{
    [DomainLogic(typeof(IFormBase))]
    public class FormBaseLogic
    {
        public virtual string ItemsPropertyName
        {
            get
            {
                return "Items";
            }
        }
        public virtual IEnumerable<IFormItemBase> Get_BaseItems(IFormBase form)
        {
            return (form as XPBaseObject).GetMemberValue(this.ItemsPropertyName) as IEnumerable<IFormItemBase>;
            //throw new Exception("请继承自FormBaseLogic并重载Get_Items方法，实现返回明细项目的集合!或在自己的逻辑类中实现Get_Items方法!");
        }
    }
}